using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Objects;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Web;
using BG.Model;
using Economic.Api;
using Economic.Api.Exceptions;

namespace BG.Controller
{
    public class Service : IService
    {

        public static IService Instance
        {
            get { return instance ?? (instance = new Service()); }
        }

        private static IService instance;

        //Accessor til test



        public ModelContainer Container { get; private set; }

        private static Dictionary<int, EconomicProject> economicProjectCache;
        private static Dictionary<int, EconomicTaskType> economicTaskTypeCache;

        public void RecreateModelContainer()
        {
            if (Container != null)
            {
                Container.Dispose();
                Container = null;
            }

            Container = new ModelContainer(IsLocal);
        }

        public bool IsLocal
        {
            get
            {

                if (isLocal == null)
                {
                    bool local = true;
                    if (HttpContext.Current != null && HttpContext.Current.Request != null)
                    {
                        local = HttpContext.Current.Request.IsLocal;
                    }

                    isLocal = local;
                }

                return (bool)isLocal;
            }
        }

        private bool? isLocal;

        //Accessor til test
        public EconomicSession EconomicSession
        {
            get
            {
                if (economicSession == null)
                {
                    economicSession = new EconomicSession();
                    economicSession.Connect(000000, "xxx", "xxxxxxxx");
                }
                return economicSession;
            }
        }

        private EconomicSession economicSession;

        public Service(bool? isLocal = null)
        {
            this.isLocal = isLocal;
            RecreateModelContainer();
            economicProjectCache = new Dictionary<int, EconomicProject>();
            economicTaskTypeCache = new Dictionary<int, EconomicTaskType>();
        }


        //-----------------------------------------------USER------------------------------------------------------------------
        //TODO USER

        public User CreateUserIfNotExists(string username, string password, int economicId, bool isAdmin)
        {
            if (UserExists(username, password) == false)
            {
                return CreateUser(username, password, economicId, isAdmin);
            }
            else
            {
                return GetUserFromCache(username, password);
            }
        }

        /// <summary>
        /// Returns a boolean indicating wether or not the user exists, given the username or password.
        /// </summary>
        /// <param name="username">The username of the user.</param>
        /// <param name="password">The password of the user.</param>
        /// <returns>Wether or not the login was successful.</returns>
        public bool UserExists(string username, string password)
        {
            return GetUserFromCache(username, password) != null;
        }

        public void UpdateUser(int id, string newUsername, string newPassword, bool isAdmin, int economicId)
        {
            try
            {
                var updateUser = GetUserFromCacheById(id);
                updateUser.Username = newUsername;
                updateUser.Password = newPassword;
                updateUser.IsAdmin = isAdmin;
                updateUser.EconomicUserId = economicId;
                Container.SaveChanges();
            }
            catch (Exception ex)
            {
                SendMailToDeveloper("(ExceptionID=13)", ex.Message, ex.StackTrace);
                throw new Exception("Unable to update the employee" + "<br/>" + "A message has been sent to the developer. Please try again.");
            }
        }

        public void DeleteUser(int id)
        {
            try
            {
                var user = GetUserFromCacheById(id);
                var toDelete = new List<DailyReport>();
                foreach (var d in user.DailyReport)
                {
                    toDelete.Add(d);
                }
                for (int i = 0; i < user.Group.ToList().Count; i++)
                {
                    user.Group.Remove(user.Group.ToList()[i]);
                }

                for (int i = 0; i < toDelete.Count; i++)
                {
                    var dId = toDelete[i].Id;
                    var daily = Container.DailyReportSet.FirstOrDefault(d => d.Id == dId);
                    Container.BreakSet.DeleteObject(daily.Break);
                    var taskToDelete = new List<Task>();
                    for (int j = 0; j < daily.Task.Count; j++)
                    {
                        var tId = daily.Task.ToList()[j].Id;
                        var task = Container.TaskSet.FirstOrDefault(t => t.Id == tId);
                        taskToDelete.Add(task);
                    }
                    for (int c = 0; c < taskToDelete.Count; c++)
                    {
                        var dTask = daily.Task.FirstOrDefault(t => t.Id == taskToDelete[c].Id);
                        daily.Task.Remove(dTask);
                    }
                    for (int x = 0; x < taskToDelete.Count; x++)
                    {
                        Container.TaskSet.DeleteObject(taskToDelete[x]);
                    }
                    Container.DailyReportSet.DeleteObject(daily);
                }
                Container.UserSet.DeleteObject(user);

                Container.SaveChanges();
            }
            catch (Exception ex)
            {
                SendMailToDeveloper("(ExceptionID=14)", ex.Message, ex.StackTrace);
                throw new Exception("Unable to delete employee" + "<br/>" + "A message has been sent to the developer. Please try again.");
            }
        }

        public User CreateUser(string username, string password, int economicId, bool isAdmin)
        {
            var user = User.CreateUser(0, economicId, username, password, isAdmin);
            if (user == null)
            {
                throw new Exception("Unable to create a new employee" + "<br/>" + "A message has been sent to the developer. Please try again.");
            }
            Container.UserSet.AddObject(user);
            Container.SaveChanges();
            return user;
        }

        public User GetUserFromCacheById(int id)
        {
            return Container.UserSet.FirstOrDefault(u => u.Id == id);
        }

        //------------------------------------------------------------------------------------------------------------------------------------------

        // Bliver ikke brugt.
        public void UpdateTasksAndEndTime(User user, int daysToMove)
        {
            foreach (Task t in GetUserCurrentDailyReport(user).Task)
            {
                t.TimeStarted = t.TimeStarted.Value.AddDays(daysToMove);
                t.TimeEnded = t.TimeEnded.Value.AddDays(daysToMove);
            }

            if (GetUserCurrentDailyReport(user).DayEnded.HasValue)
            {
                GetUserCurrentDailyReport(user).DayEnded =
                    GetUserCurrentDailyReport(user).DayEnded.Value.AddDays(daysToMove);
            }

            Container.SaveChanges();
        }

        //--------------------------------------------------------ECONOMIC ----------------------------------------------------------------------------


        public EconomicTaskType CreateEconomicTaskTypeIfNotExists(int economicTaskTypeId, string name)
        {
            if (TaskTypeExists(economicTaskTypeId) == false)
            {
                return EconomicTaskType.CreateEconomicTaskType(0, economicTaskTypeId, name);
            }
            else
            {
                return Container.EconomicTaskTypeSet.FirstOrDefault(t => t.EconomicTaskTypeId == economicTaskTypeId);
            }
        }

        public bool TaskTypeExists(int economicTaskTypeId)
        {
            return GetEconomicTaskTypeFromCache(economicTaskTypeId) != null;
        }

        public EconomicTaskType GetEconomicTaskTypeFromCache(int economicTaskTypeId)
        {
            try
            {
                return Container.EconomicTaskTypeSet.FirstOrDefault(
                    p => p.EconomicTaskTypeId == economicTaskTypeId);
            }
            catch (Exception ex)
            {
                SendMailToDeveloper("(ExceptionID=7)", ex.Message, ex.StackTrace);
                throw new Exception("Unable to find economic task type" + "<br/>" +
                                    "A message has been sent to the developer");
            }
        }

        public EconomicProject CreateEconomicProjectIfNotExists(int economicProjectId, string name, Group group)
        {
            if (ProjectExists(economicProjectId) == false)
            {
                var project = EconomicProject.CreateEconomicProject(0, economicProjectId, name);
                var cache = Container.EconomicCacheSet.First();
                project.Group = group;
                cache.EconomicProject.Add(project);
                return project;
            }
            else
            {
                return Container.EconomicProjectSet.FirstOrDefault(p => p.EconomicProjectId == economicProjectId);
            }
        }

        public bool ProjectExists(int economicProjectId)
        {
            return GetEconomicProjectFromCache(economicProjectId) != null;
        }

        public EconomicProject GetEconomicProjectFromCache(int economicProjectId)
        {
            try
            {
                return Container.EconomicProjectSet.FirstOrDefault(
                    p => p.EconomicProjectId == economicProjectId);
            }
            catch (Exception ex)
            {
                SendMailToDeveloper("(ExceptionID=9)", ex.Message, ex.StackTrace);
                throw new Exception("Unable to find project in cache" + "/n" +
                                    "A message has been sent to the developer. Please try again.");
            }

        }

        //-------------------------------------------------------------DAILYREPORT ---------------------------------------------------------------------
        //TODO DailyReport

        public void SetCurrentDailyReportStartAndBreak(DateTime startDate, DailyReport currentDailyReport)
        {
            try
            {
                currentDailyReport.DayStarted = startDate;
                currentDailyReport.Break.BreakStarted = startDate.AddMinutes(270);
                currentDailyReport.Break.BreakEnded = currentDailyReport.Break.BreakStarted.Value.AddMinutes(30);
                Container.SaveChanges();
            }
            catch (Exception ex)
            {
                SendMailToDeveloper("(ExceptionID=4)", ex.Message, ex.StackTrace);
                throw new Exception("Unable to set current daily report day started" + "/n" +
                                    "A message has been sent to the developer. Please try again.");
            }

        }

        public void CreateNewDailyReportForUser(User user)
        {
            try
            {
                DailyReport dailyReport = DailyReport.CreateDailyReport(0);
                Container.DailyReportSet.AddObject(dailyReport);
                user.DailyReport.Add(dailyReport);
                Break bre = Break.CreateBreak(0);
                Container.BreakSet.AddObject(bre);
                dailyReport.Break = bre;
                Container.SaveChanges();
            }
            catch (Exception ex)
            {
                SendMailToDeveloper("(ExceptionID: 1)", ex.Message, ex.StackTrace);
                throw new Exception("Unable to create new daily report for: " + user.Username + "<br/>" +
                                    ". A message has been sent to the developer. Please try again.");
            }
        }

        public bool UserHasDailyReport(User user)
        {
            return user.DailyReport.Any();
        }

        public void ExistsBreakToDailyReport(DailyReport currentDailyreport)
        {
            if (currentDailyreport.Break == null)
            {
                var report = Container.DailyReportSet.First(d => d.Id == currentDailyreport.Id);
                Break bre = Break.CreateBreak(0);
                Container.BreakSet.AddObject(bre);
                report.Break = bre;
                bre.DailyReport = report;
                Container.SaveChanges();
            }
        }


        //--------------------------------------------------------------UPDATE START AND END TIME -------------------------------------------------------------
        //TODO UPDATE START AND END TIME
        public void UpdateDailyReportTimes(User user, string date, int hour, int minute, bool isEnd, bool moveTasks)
        {

            DateTime selectedDate = DateTime.Parse(date);
            selectedDate = selectedDate.AddHours(hour);
            selectedDate = selectedDate.AddMinutes(minute);
            try
            {
                UpdateDay(GetDailyReport(user), selectedDate, moveTasks, isEnd);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            try
            {
                Container.SaveChanges();
            }
            catch (Exception ex)
            {
                SendMailToDeveloper("(ExceptionID=3)", ex.Message, ex.StackTrace);
                throw new Exception("Unable to update current daily report time for: " + user.Username + "/n" +
                                    "A message has been sent to the developer. Please try again.");
            }
        }

        private void UpdateDay(DailyReport dailyReport, DateTime newDate, bool moveTasks, bool isEnd)
        {

            var bre = dailyReport.Break;
            //Start senere
            if (!isEnd)
            {
                if (dailyReport.DayEnded.HasValue)
                {
                    if (newDate >= dailyReport.DayEnded)
                    {
                        throw new Exception("There is less than 30 min. from your starttime to your endtime");
                    }
                }
                if (dailyReport.DayStarted < newDate)
                {
                    var taskHit =
                        dailyReport.Task.FirstOrDefault(t => t.TimeStarted < newDate && t.TimeEnded > newDate);
                    if (taskHit == null)
                    {
                        if (moveTasks)
                        {
                            TimeSpan timeSpan = newDate - dailyReport.DayStarted.Value;
                            foreach (var task in dailyReport.Task)
                            {
                                AddTimeSpanToTask(timeSpan, task);
                            }
                            bre.BreakStarted = bre.BreakStarted.Value.Add(timeSpan);
                            bre.BreakEnded = bre.BreakEnded.Value.Add(timeSpan);
                            if (dailyReport.DayEnded.HasValue)
                            {
                                dailyReport.DayEnded = dailyReport.DayEnded.Value.Add(timeSpan);
                            }
                            dailyReport.DayStarted = newDate;
                        }
                        else
                        {
                            if (bre.BreakStarted < newDate && bre.BreakEnded > newDate)
                            {
                                if (dailyReport.DayEnded.HasValue)
                                {
                                    var breakStartBefore = bre.BreakStarted;
                                    bre.BreakStarted = newDate;
                                    bre.BreakEnded = bre.BreakStarted.Value.AddMinutes(35);

                                    if (bre.BreakEnded > dailyReport.DayEnded)
                                    {
                                        bre.BreakStarted = breakStartBefore;
                                        bre.BreakEnded = bre.BreakStarted.Value.AddMinutes(30);
                                        throw new Exception(
                                            "There is less than 30 min. from your starttime to your endtime.");
                                    }
                                    if (bre.BreakEnded <= dailyReport.DayEnded)
                                    {
                                        bre.BreakEnded = bre.BreakEnded.Value.AddMinutes(-5);
                                    }
                                }
                                else
                                {
                                    bre.BreakStarted = newDate;
                                    bre.BreakEnded = bre.BreakStarted.Value.AddMinutes(30);
                                }
                                var taskShortenDown =
                                    dailyReport.Task.FirstOrDefault(
                                        t => t.TimeStarted < bre.BreakEnded && t.TimeEnded > bre.BreakEnded);
                                var tasksToBeDeleted =
                                    dailyReport.Task.Where(
                                        t =>
                                            t.TimeEnded <= bre.BreakStarted).ToList();
                                if (tasksToBeDeleted.Any())
                                {
                                    foreach (var taskAfterBreak in tasksToBeDeleted)
                                    {
                                        dailyReport.Task.Remove(taskAfterBreak);
                                        Container.TaskSet.DeleteObject(taskAfterBreak);
                                    }
                                }
                                if (taskShortenDown != null)
                                {
                                    taskShortenDown.TimeStarted = bre.BreakEnded;
                                }
                                dailyReport.DayStarted = newDate;
                            }
                            else if (newDate <= bre.BreakStarted)
                            {
                                var tasksToBeDeleted = dailyReport.Task.Where(t => t.TimeEnded <= newDate).ToList();
                                if (tasksToBeDeleted.Any())
                                {
                                    foreach (var taskDel in tasksToBeDeleted)
                                    {
                                        dailyReport.Task.Remove(taskDel);
                                        Container.TaskSet.DeleteObject(taskDel);
                                    }
                                }
                                dailyReport.DayStarted = newDate;
                            }
                            else if (newDate == bre.BreakEnded ||
                                     dailyReport.DayStarted < newDate && bre.BreakEnded < newDate)
                            {
                                if (dailyReport.DayEnded.HasValue)
                                {
                                    var breakStartBefore = bre.BreakStarted;
                                    bre.BreakStarted = newDate;
                                    bre.BreakEnded = bre.BreakStarted.Value.AddMinutes(35);

                                    if (bre.BreakEnded > dailyReport.DayEnded)
                                    {
                                        bre.BreakStarted = breakStartBefore;
                                        bre.BreakEnded = bre.BreakStarted.Value.AddMinutes(30);
                                        throw new Exception(
                                            "There is less than 30 min. from your starttime to your endtime.");
                                    }
                                    if (bre.BreakEnded <= dailyReport.DayEnded)
                                    {
                                        bre.BreakEnded = bre.BreakEnded.Value.AddMinutes(-5);
                                    }
                                }
                                else
                                {
                                    bre.BreakStarted = newDate;
                                    bre.BreakEnded = bre.BreakStarted.Value.AddMinutes(30);
                                }
                                var tasksToBeDeleted =
                                    dailyReport.Task.Where(t => t.TimeEnded <= bre.BreakEnded).ToList();
                                var task =
                                    dailyReport.Task.FirstOrDefault(
                                        t =>
                                            t.TimeStarted < bre.BreakEnded && t.TimeEnded > bre.BreakEnded &&
                                            t.TimeEnded <= dailyReport.DayEnded);
                                if (tasksToBeDeleted.Any())
                                {
                                    foreach (var taskDel in tasksToBeDeleted)
                                    {
                                        dailyReport.Task.Remove(taskDel);
                                        Container.TaskSet.DeleteObject(taskDel);
                                    }
                                }
                                if (task != null)
                                {
                                    task.TimeStarted = bre.BreakEnded;
                                }
                                dailyReport.DayStarted = newDate;

                            }

                        }
                    }
                    else
                    {
                        if (moveTasks)
                        {
                            TimeSpan timeSpan = newDate - dailyReport.DayStarted.Value;
                            foreach (var task in dailyReport.Task)
                            {
                                AddTimeSpanToTask(timeSpan, task);
                            }
                            bre.BreakStarted = bre.BreakStarted.Value.Add(timeSpan);
                            bre.BreakEnded = bre.BreakEnded.Value.Add(timeSpan);
                            if (dailyReport.DayEnded.HasValue)
                            {
                                dailyReport.DayEnded = dailyReport.DayEnded.Value.Add(timeSpan);
                            }
                            dailyReport.DayStarted = newDate;
                        }
                        else
                        {
                            if (newDate <= bre.BreakStarted)
                            {
                                var tasksToBeDeleted =
                                    dailyReport.Task.Where(t => t.TimeEnded <= newDate).
                                        ToList();
                                if (tasksToBeDeleted.Any())
                                {
                                    foreach (var task in tasksToBeDeleted)
                                    {
                                        dailyReport.Task.Remove(task);
                                        Container.TaskSet.DeleteObject(task);
                                    }
                                }
                                if (newDate < bre.BreakStarted)
                                {
                                    taskHit.TimeStarted = newDate;
                                }
                                dailyReport.DayStarted = newDate;

                            }
                            else if (newDate >= bre.BreakEnded)
                            {
                                if (dailyReport.DayEnded.HasValue)
                                {
                                    var breakStartBefore = bre.BreakStarted;
                                    bre.BreakStarted = newDate;
                                    bre.BreakEnded = bre.BreakStarted.Value.AddMinutes(35);

                                    if (bre.BreakEnded > dailyReport.DayEnded)
                                    {
                                        bre.BreakStarted = breakStartBefore;
                                        bre.BreakEnded = bre.BreakStarted.Value.AddMinutes(30);
                                        throw new Exception(
                                            "There is less than 30 min. from your starttime to your endtime.");
                                    }
                                    if (bre.BreakEnded <= dailyReport.DayEnded)
                                    {
                                        bre.BreakEnded = bre.BreakEnded.Value.AddMinutes(-5);
                                    }

                                    var tasksToBeDeleted =
                                        dailyReport.Task.Where(t => t.TimeEnded <= bre.BreakEnded).ToList();
                                    var task =
                                        dailyReport.Task.FirstOrDefault(
                                            t =>
                                                t.TimeStarted < bre.BreakEnded && t.TimeEnded > bre.BreakEnded);
                                    if (tasksToBeDeleted.Any())
                                    {
                                        foreach (var taskDel in tasksToBeDeleted)
                                        {
                                            dailyReport.Task.Remove(taskDel);
                                            Container.TaskSet.DeleteObject(taskDel);
                                        }
                                    }
                                    if (task != null)
                                    {
                                        task.TimeStarted = bre.BreakEnded;
                                    }
                                    dailyReport.DayStarted = newDate;
                                }
                                else
                                {
                                    bre.BreakStarted = newDate;
                                    bre.BreakEnded = bre.BreakStarted.Value.AddMinutes(30);
                                    var tasksToBeDeleted =
                                        dailyReport.Task.Where(t => t.TimeEnded <= bre.BreakEnded).ToList();
                                    var task =
                                        dailyReport.Task.FirstOrDefault(
                                            t =>
                                                t.TimeStarted < bre.BreakEnded && t.TimeEnded > bre.BreakEnded);
                                    if (tasksToBeDeleted.Any())
                                    {
                                        foreach (var taskDel in tasksToBeDeleted)
                                        {
                                            dailyReport.Task.Remove(taskDel);
                                            Container.TaskSet.DeleteObject(taskDel);
                                        }
                                    }
                                    if (task != null)
                                    {
                                        task.TimeStarted = bre.BreakEnded;
                                    }
                                    dailyReport.DayStarted = newDate;
                                }
                            }
                        }
                    }
                }
                //Start tidligere eller slut senere.
                else if (dailyReport.DayStarted > newDate && !isEnd)
                {
                    UpdateDayStartOrDayEnded(dailyReport, isEnd, newDate);
                }
            }
            //Slut tidligere.
            if (isEnd)
            {
                if (newDate <= dailyReport.DayStarted)
                {
                    throw new Exception("There is less than 30 min. from your endtime to your starttime");
                }

                if (dailyReport.DayEnded < newDate)
                {
                    UpdateDayStartOrDayEnded(dailyReport, isEnd, newDate);
                }
                else
                {
                    SetEndTime(dailyReport, newDate, bre);
                }
            }

        }

        private void SetEndTime(DailyReport dailyReport, DateTime newDate, Break bre)
        {
            var taskHit = dailyReport.Task.FirstOrDefault(t => t.TimeStarted < newDate && t.TimeEnded > newDate);
            // Ikke pause

            if (taskHit == null)
            {

                if (newDate > bre.BreakStarted && newDate < bre.BreakEnded)
                {
                    var breakEnded = bre.BreakEnded;
                    bre.BreakEnded = newDate;
                    bre.BreakStarted = bre.BreakEnded.Value.AddMinutes(-35);
                    if (bre.BreakStarted < dailyReport.DayStarted)
                    {
                        bre.BreakEnded = breakEnded;
                        bre.BreakStarted = bre.BreakEnded.Value.AddMinutes(-30);
                        throw new Exception("There is less than 30 min. from your endtime to your starttime.");
                    }
                    if (bre.BreakStarted >= dailyReport.DayStarted)
                    {
                        bre.BreakStarted = bre.BreakStarted.Value.AddMinutes(5);
                    }
                    var taskShortenDown =
                        dailyReport.Task.FirstOrDefault(
                            t => t.TimeStarted < bre.BreakStarted && t.TimeEnded > bre.BreakStarted);
                    var tasksToBeDeleted =
                        dailyReport.Task.Where(
                            t => t.TimeStarted >= bre.BreakStarted)
                            .ToList();
                    if (tasksToBeDeleted.Any())
                    {
                        foreach (var task in tasksToBeDeleted)
                        {
                            dailyReport.Task.Remove(task);
                            Container.TaskSet.DeleteObject(task);
                        }
                    }
                    if (taskShortenDown != null)
                    {
                        taskShortenDown.TimeEnded = bre.BreakStarted;
                    }
                    dailyReport.DayEnded = newDate;
                }
                else if (newDate >= bre.BreakEnded)
                {
                    if (newDate == bre.BreakEnded)
                    {
                        var breakEnded = bre.BreakEnded;
                        bre.BreakEnded = newDate;
                        bre.BreakStarted = bre.BreakEnded.Value.AddMinutes(-35);
                        if (bre.BreakStarted < dailyReport.DayStarted)
                        {
                            bre.BreakEnded = breakEnded;
                            bre.BreakStarted = bre.BreakEnded.Value.AddMinutes(-30);
                            throw new Exception("There is less than 30 min. from your endtime to your starttime.");
                        }
                        if (bre.BreakStarted >= dailyReport.DayStarted)
                        {
                            bre.BreakStarted = bre.BreakStarted.Value.AddMinutes(5);
                        }
                    }
                    var tasksAfterBreakToBeDeleted =
                        dailyReport.Task.Where(t => t.TimeStarted >= newDate).ToList();
                    if (tasksAfterBreakToBeDeleted.Any())
                    {
                        foreach (var taskAfter in tasksAfterBreakToBeDeleted)
                        {
                            dailyReport.Task.Remove(taskAfter);
                            Container.TaskSet.DeleteObject(taskAfter);
                        }
                    }
                    dailyReport.DayEnded = newDate;
                }
                else if (newDate <= bre.BreakStarted)
                {
                    var breakEnded = bre.BreakEnded;
                    bre.BreakEnded = newDate;
                    bre.BreakStarted = bre.BreakEnded.Value.AddMinutes(-35);
                    if (bre.BreakStarted < dailyReport.DayStarted)
                    {
                        bre.BreakEnded = breakEnded;
                        bre.BreakStarted = bre.BreakEnded.Value.AddMinutes(-30);
                        throw new Exception("There is less than 30 min. from your endtime to your starttime.");
                    }
                    if (bre.BreakStarted >= dailyReport.DayStarted)
                    {
                        bre.BreakStarted = bre.BreakStarted.Value.AddMinutes(5);
                    }
                    var tasksToBeDeleted =
                        dailyReport.Task.Where(t => t.TimeStarted >= bre.BreakStarted).ToList();
                    var taskBeforeBreakHit =
                        dailyReport.Task.FirstOrDefault(
                            t => t.TimeStarted < bre.BreakStarted && t.TimeEnded > bre.BreakStarted);
                    if (tasksToBeDeleted.Any())
                    {
                        foreach (var task in tasksToBeDeleted)
                        {
                            dailyReport.Task.Remove(task);
                            Container.TaskSet.DeleteObject(task);
                        }
                    }
                    if (taskBeforeBreakHit != null)
                    {
                        taskBeforeBreakHit.TimeEnded = bre.BreakStarted;
                    }
                    dailyReport.DayEnded = newDate;
                }
            }
            else
            {
                if (newDate > bre.BreakEnded)
                {
                    var breakEnded = bre.BreakEnded;
                    bre.BreakEnded = newDate;
                    bre.BreakStarted = bre.BreakEnded.Value.AddMinutes(-35);
                    if (bre.BreakStarted < dailyReport.DayStarted)
                    {
                        bre.BreakEnded = breakEnded;
                        bre.BreakStarted = bre.BreakEnded.Value.AddMinutes(-30);
                        throw new Exception("There is less than 30 min. from your endtime to your starttime.");
                    }
                    if (bre.BreakStarted >= dailyReport.DayStarted)
                    {
                        bre.BreakStarted = bre.BreakStarted.Value.AddMinutes(5);
                    }
                    var tasksToBeDeleted = dailyReport.Task.Where(t => t.TimeStarted >= newDate).ToList();
                    if (tasksToBeDeleted.Any())
                    {
                        foreach (var taskDel in tasksToBeDeleted)
                        {
                            dailyReport.Task.Remove(taskDel);
                            Container.TaskSet.DeleteObject(taskDel);
                        }
                    }
                    taskHit.TimeEnded = newDate;
                    dailyReport.DayEnded = newDate;
                }
                else if (newDate < bre.BreakStarted)
                {
                    var breakEnded = bre.BreakEnded;
                    bre.BreakEnded = newDate;
                    bre.BreakStarted = bre.BreakEnded.Value.AddMinutes(-35);
                    if (bre.BreakStarted < dailyReport.DayStarted)
                    {
                        bre.BreakEnded = breakEnded;
                        bre.BreakStarted = bre.BreakEnded.Value.AddMinutes(-30);
                        throw new Exception("There is less than 30 min. from your endtime to your starttime.");
                    }
                    if (bre.BreakStarted >= dailyReport.DayStarted)
                    {
                        bre.BreakStarted = bre.BreakStarted.Value.AddMinutes(5);
                    }
                    var tasksToBeDeleted =
                        dailyReport.Task.Where(t => t.TimeStarted >= bre.BreakStarted).ToList();
                    var taskBeforeBreakHit =
                        dailyReport.Task.FirstOrDefault(
                            t => t.TimeStarted < bre.BreakStarted && t.TimeEnded > bre.BreakStarted);
                    if (tasksToBeDeleted.Any())
                    {
                        foreach (var task in tasksToBeDeleted)
                        {
                            dailyReport.Task.Remove(task);
                            Container.TaskSet.DeleteObject(task);
                        }
                    }
                    if (taskBeforeBreakHit != null)
                    {
                        taskBeforeBreakHit.TimeEnded = bre.BreakStarted;
                    }
                    dailyReport.DayEnded = newDate;
                }
            }

        }

        private void UpdateDayStartOrDayEnded(DailyReport dailyReport, bool isEnd, DateTime newDate)
        {
            if (newDate < dailyReport.DayStarted && !isEnd)
            {
                dailyReport.DayStarted = newDate;
            }
            if (newDate > dailyReport.DayEnded && isEnd)
            {
                dailyReport.DayEnded = newDate;
            }
        }

        //---------------------------------------------------------------ROUTINE----------------------------------------------------------------------------
        //TODO ROUTINE

        public IEnumerable<Route> GetRoutesFromCache()
        {
            return Container.RouteSet.ToList();
        }

        public Activity CreateActivityToRoutine(EconomicProject project, EconomicTaskType taskType, double totalDuration)
        {
            var activity = Activity.CreateActivity(0, totalDuration, false);
            activity.EconomicProject = project;
            activity.EconomicTaskType = taskType;
            Container.ActivitySet.AddObject(activity);
            Container.SaveChanges();
            return activity;
        }

        public Activity GetActivityFromCacheById(int activityId)
        {
            return Container.ActivitySet.FirstOrDefault(a => a.Id == activityId);
        }

        public void DeleteActivity(int activityId)
        {
            Container.ActivitySet.DeleteObject(GetActivityFromCacheById(activityId));
            Container.SaveChanges();
        }


        public Activity UpdateActivityToRoutine(int activityId, EconomicProject project, EconomicTaskType taskType,
            double totalDuration)
        {
            var activity = GetActivityFromCacheById(activityId);
            activity.EconomicProject = project;
            activity.EconomicTaskType = taskType;
            activity.Minutes = totalDuration;
            Container.SaveChanges();
            return activity;
        }


        public void CreateRoutineAndAssociateToEmployee(string routineName, Route route, List<User> employees,
            List<DateTime> dates, List<Activity> activities)
        {
            var routine = Routine.CreateRoutine(0, routineName, route.Id);
            foreach (var activity in activities)
            {
                routine.Activities.Add(activity);
            }
            foreach (var date in dates)
            {
                var theDate = Date.CreateDate(0, date);
                routine.Dates.Add(theDate);
                Container.DateSet.AddObject(theDate);
            }
            foreach (var employee in employees)
            {
                routine.Users.Add(employee);
            }

            routine.Route = route;
            Container.RoutineSet.AddObject(routine);
            Container.SaveChanges();
        }

        public Routine GetRoutineFromCacheById(int routineId)
        {
            try
            {
                return Container.RoutineSet.FirstOrDefault(r => r.Id == routineId);

            }
            catch (Exception ex)
            {
                SendMailToDeveloper("(ExceptionID=8)", ex.Message, ex.StackTrace);
                throw new Exception("Unable to get routine from cache" + "<br/>" +
                                    "A message has been sent to the developer. Please try again.");
            }
        }

        public IEnumerable<Routine> GetAllRoutinesFromCache()
        {
            return Container.RoutineSet.ToList();
        }

        public void RoutineAccepted(User user, Routine routine, Activity activity)
        {
            var dailyReport = user.DailyReport.FirstOrDefault();
            ReplaceActivitiesWithTasks(routine, dailyReport, activity);

            Container.SaveChanges();
        }

        private void ReplaceActivitiesWithTasks(Routine routine, DailyReport dailyReport, Activity activity)
        {
            DateTime? activityStart = new DateTime();
            DateTime? activityEnd = new DateTime();

            var theActivity = routine.Activities.First(a => a.Id == activity.Id);
            if (!dailyReport.Task.Any())
            {
                activityStart = dailyReport.DayStarted;
                activityEnd = activityStart.Value.AddMinutes(theActivity.Minutes);
            }
            else
            {
                var lastAddedTask = dailyReport.Task.OrderBy(t => t.TimeStarted).Last();
                activityStart = lastAddedTask.TimeEnded;
                activityEnd = activityStart.Value.AddMinutes(theActivity.Minutes);
            }

            var task = CreateTask(activityStart.Value, activityEnd.Value, dailyReport, theActivity.EconomicProject,
                theActivity.EconomicTaskType, true);
            task.Activity = theActivity;
            theActivity.Task = task;
        }

        public Date CreateDate(DateTime selectedDate)
        {
            var date = Date.CreateDate(0, selectedDate);
            Container.DateSet.AddObject(date);
            Container.SaveChanges();
            return date;
        }

        public void ChangeDateInRoutine(int allRoutinesSelectedRoutineId, Date date, string text)
        {
            var routine = GetRoutineFromCacheById(allRoutinesSelectedRoutineId);
            Date oldDate = null;
            if (text == "From")
            {
                oldDate = routine.Dates.OrderBy(d => d.TheDate).ElementAt(0);
                routine.Dates.Add(date);
                Container.DateSet.DeleteObject(oldDate);
            }
            else if (text == "To")
            {
                oldDate = routine.Dates.OrderBy(d => d.TheDate).ElementAt(1);
                routine.Dates.Add(date);
                Container.DateSet.DeleteObject(oldDate);
            }
            Container.SaveChanges();
        }

        public void DeleteDateFromRoutine(int allRoutinesSelectedRoutineId, int dateId)
        {
            var routine = GetRoutineFromCacheById(allRoutinesSelectedRoutineId);
            routine.Dates.Remove(routine.Dates.FirstOrDefault(d => d.Id == dateId));
            Container.DateSet.DeleteObject(Container.DateSet.FirstOrDefault(d => d.Id == dateId));
            Container.SaveChanges();
        }

        public Date GetDateFromCacheById(int dateId)
        {
            return Container.DateSet.FirstOrDefault(d => d.Id == dateId);
        }


        //---------------------------------------------------------------GETTER -------------------------------------------------------------------------------
        //TODO GETTER

        public DailyReport GetDailyReport(User user)
        {
            try
            {
                if (!UserHasDailyReport(user))
                {
                    CreateNewDailyReportForUser(user);
                }
                return GetUserCurrentDailyReport(user);
            }
            catch (Exception ex)
            {
                SendMailToDeveloper("(ExceptionID: 2)", ex.Message, ex.StackTrace);
                throw new Exception("Unable to get current daily report for: " + user.Username + "<br/>" +
                                    ". A message has been sent to the developer. Please try again.");
            }
        }

        public IEnumerable<Task> GetTasksForUser(User user)
        {
            if (!UserHasDailyReport(user))
            {
                CreateNewDailyReportForUser(user);
            }

            return GetUserCurrentDailyReport(user).Task.ToList().OrderBy(t => t.TimeStarted);
        }

        public DailyReport GetUserCurrentDailyReport(User user)
        {
            try
            {
                return user.DailyReport.OrderByDescending(d => d.DayStarted).FirstOrDefault();
            }
            catch (Exception ex)
            {
                SendMailToDeveloper("(ExceptionID=3)", ex.Message, ex.StackTrace);
                throw new Exception("Unable to get current daily report for: " + user.Username + "<br/>" +
                                    ". A message has been sent to the developer. Please try again.");
            }

        }


        public User GetUserFromCache(string username, string password)
        {
            var users = Container.UserSet;
            User result = null;
            foreach (var user in users)
            {
                if (user.Username.Equals(username, StringComparison.OrdinalIgnoreCase) && user.Password == password)
                {
                    result = user;
                    break;
                }
            }
            return result;
        }

        /// <summary>
        /// Gets the Projects from SQL server.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<EconomicProject> GetAllEconomicProjectsFromCache()
        {
            try
            {
                return Container.EconomicProjectSet.ToList();
            }
            catch (Exception ex)
            {
                SendMailToDeveloper("(ExceptionID=5)", ex.Message, ex.StackTrace);
                throw new Exception("Unable to get economic project(s) from cache" + "<br/>" +
                                    "A message has been sent to the developer. Please try again.");
            }

        }

        /// <summary>
        /// Gets the Projects tasks from SQL server.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<EconomicTaskType> GetAllEconomicTaskTypeFromCache()
        {
            try
            {
                return Container.EconomicTaskTypeSet.ToList();

            }
            catch (Exception ex)
            {
                SendMailToDeveloper("(ExceptionID=6)", ex.Message, ex.StackTrace);
                throw new Exception("Unable to get economic task type(s) from cache" + "<br/>" +
                                    "A message has been sent to the developer. Please try again.");
            }

        }

        /// <summary>
        /// Gets the groups from SQL server.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Group> GetAllGroupsFromCache()
        {
            return Container.GroupSet.ToList();
        }

        /// <summary>
        /// Gets the users from SQL server and associated groups.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<User> GetUsersFromCache()
        {
            return Container.UserSet.ToList();
        }

        public IEnumerable<EconomicTaskType> GetAllTasksFromSelectedProject(int projectId)
        {
            return GetAllEconomicProjectsFromCache().First(p => p.Id == projectId).EconomicTaskTypes.ToList();
        }

        //------------------------------------------------------------GROUP -------------------------------------------------------------------------------------
        public void UpdateGroup(int groupId, List<User> usersSignUp, List<User> usersSignOff)
        {
            var group = Container.GroupSet.FirstOrDefault(g => g.Id == groupId);
            foreach (var user in usersSignUp)
            {
                if (!user.Group.Any(g => g.Id == groupId))
                {
                    user.Group.Add(group);
                }
            }

            foreach (var user in usersSignOff)
            {
                if (user.Group.Any(g => g.Id == groupId))
                {
                    user.Group.Remove(group);
                }
            }
            Container.SaveChanges();
        }

        private bool GroupExists(int number)
        {
            return Container.GroupSet.Any(g => g.EconomicGroupNumber == number);
        }

        public Group CreateGroupIfNotExists(int number, string name)
        {
            if (!GroupExists(number))
            {
                var group = Group.CreateGroup(0, name, number);
                Container.GroupSet.AddObject(group);
                return group;
            }
            else
            {
                return Container.GroupSet.FirstOrDefault(g => g.Id == number);
            }
        }

        public bool UserIsMemberOfGroup(int groupId, int userId)
        {
            return Container.UserSet.FirstOrDefault(u => u.Id == userId).Group.Any(g => g.Id == groupId);
        }

        public void UpdateGroupUsers(int groupId, Dictionary<int, bool> dictionaryGroupUpdates)
        {
            Group group = GetAllGroupsFromCache().First(g => g.Id == groupId);

            foreach (var dictionaryGroupUpdate in dictionaryGroupUpdates)
            {
                User user = GetUsersFromCache().First(u => u.Id == dictionaryGroupUpdate.Key);
                bool signUserUp = dictionaryGroupUpdate.Value;

                if (signUserUp && !group.User.Any(u => u == user))
                {
                    group.User.Add(user);
                }
                else if (!signUserUp && group.User.Any(u => u == user))
                {
                    group.User.Remove(user);
                }
            }

            Container.SaveChanges();
        }

        //------------------------------------------------------------------------------DB OG ECONOMIC DB-------------------------------------------------------

        /// <summary>
        /// Updating our SQL server from e-conomic data, if there is any changes.
        /// </summary>
        public void RefreshCache()
        {
            var cache = Container.EconomicCacheSet.FirstOrDefault();
            if (cache == null)
            {
                cache = EconomicCache.CreateEconomicCache(0, DateTime.UtcNow.AddHours(1).AddMonths(-1));
                Container.EconomicCacheSet.AddObject(cache);
                Container.SaveChanges();
            }
            Debug.Print("START");
            cache.LastUpdated = DateTime.UtcNow.AddHours(1);
            Container.SaveChanges();
            Debug.Print("1176");
            var groupsFromEconomic = GetGroupsFromEconomic();
            var groupsFromCache = GetAllGroupsFromCache();

            //Opretter grupper fra economic der ikke er i databasen
            foreach (var groupEconomic in groupsFromEconomic)
            {
                //Hvis gruppe findes i economic og lokalt
                if (groupsFromCache.Any(g => g.EconomicGroupNumber == groupEconomic.EconomicGroupNumber))
                {
                    //Opdater variabler lokalt
                    groupsFromCache.First(g => g.EconomicGroupNumber == groupEconomic.EconomicGroupNumber).Name =
                        groupEconomic.Name;
                }
                else
                {
                    //Den findes ikke lokalt, opret den
                    Group newGroup = new Group
                    {
                        EconomicGroupNumber = groupEconomic.EconomicGroupNumber,
                        Name = groupEconomic.Name
                    };
                    Container.AddToGroupSet(newGroup);
                }
            }

            foreach (var groupCache in groupsFromCache)
            {
                //Hvis gruppe findes lokalt men ikke i economic, slet lokal
                if (!groupsFromEconomic.Any(g => g.EconomicGroupNumber == groupCache.EconomicGroupNumber))
                {
                    //TODO (?): Sletter entity framework selv tilknyttede links til diverse andre entities?JA
                    Container.GroupSet.DeleteObject(groupCache);
                }
            }

            //Container.SaveChanges();
            Debug.Print("1209");

            var projectsFromDatabase = GetAllEconomicProjectsFromCache().ToList();
            var taskTypesFromDatabase = GetAllEconomicTaskTypeFromCache().ToList();

            var removedProjects = new HashSet<EconomicProject>();
            var removedTaskTypes = new HashSet<EconomicTaskType>();

            //deletes projects from cache no longer in e-conomic
            foreach (var o1 in projectsFromDatabase)
            {
                foreach (var o2 in projectsFromDatabase)
                {
                    if (o1 != o2 && o1.EconomicProjectId == o2.EconomicProjectId && !removedProjects.Contains(o1) &&
                        !removedProjects.Contains(o2))
                    {
                        foreach (var taskType in o2.EconomicTaskTypes.ToList())
                        {
                            foreach (var task in taskType.Task.ToList())
                            {
                                Container.DeleteObject(task);
                            }
                            Container.DeleteObject(taskType);
                        }
                        Container.DeleteObject(o2);
                        removedProjects.Add(o2);
                    }
                }
            }

            //deletes taskTypes(activities) from cache no longer in economic
            foreach (var o1 in taskTypesFromDatabase)
            {
                foreach (var o2 in taskTypesFromDatabase)
                {
                    if (o1 != o2 && o1.EconomicTaskTypeId == o2.EconomicTaskTypeId && !removedTaskTypes.Contains(o1) &&
                        !removedTaskTypes.Contains(o2))
                    {
                        foreach (var task in o2.Task.ToList())
                        {
                            Container.DeleteObject(task);
                        }
                        Container.DeleteObject(o2);
                        removedTaskTypes.Add(o2);
                    }
                }
            }
            Container.SaveChanges();
            Debug.Print("1257");
            projectsFromDatabase = GetAllEconomicProjectsFromCache().ToList();

            var unconvertedTaskTypesFromEconomic = GetTaskTypesFromEconomic();
            var projectsFromEconomicRaw = economicProjectCache.Values.ToList();
            var projectsFromEconomic = projectsFromEconomicRaw.ToList();

            //Console.Beep();

            //Updaterer projects i local ud fra economic data hvis den eksisterer i economic, ellers slettes projektet
            foreach (var databaseProject in projectsFromDatabase.ToList())
            {

                var found = false;
                foreach (var economicProject in projectsFromEconomic)
                {

                    if (databaseProject.EconomicProjectId == economicProject.EconomicProjectId)
                    {
                        databaseProject.Name = economicProject.Name;
                        databaseProject.Group = economicProject.Group;
                        found = true;
                        break;
                    }

                }

                if (!found)
                {
                    foreach (var taskType in databaseProject.EconomicTaskTypes.ToList())
                    {
                        foreach (var task in taskType.Task.ToList())
                        {
                            Container.DeleteObject(task);
                        }
                        Container.DeleteObject(taskType);
                    }
                    Container.EconomicProjectSet.DeleteObject(databaseProject);
                    projectsFromDatabase.Remove(databaseProject);
                    projectsFromEconomic.Remove(databaseProject);
                }
            }

            //Kan ikke ske endnu, da cache link på projekter endnu ikke er gemt.
            // Container.SaveChanges();

            projectsFromDatabase = GetAllEconomicProjectsFromCache().ToList();

            //Sætter variabel cache. Denne burde måske lægge i GetTaskTypesFromEconomic(), da det er her alle andre variabler læses ind.
            foreach (var economicProject in projectsFromEconomic)
            {
                var projectExistsInDatabase =
                    projectsFromDatabase.Any(p => p.EconomicProjectId == economicProject.EconomicProjectId);
                if (!projectExistsInDatabase)
                {
                    economicProject.EconomicCache = cache;

                    cache.EconomicProject.Add(economicProject);
                    Container.EconomicProjectSet.AddObject(economicProject);
                }
            }
            Container.SaveChanges();
            Debug.Print("1319");
            foreach (var databaseTaskType in taskTypesFromDatabase.ToList())
            {

                var found = false;
                foreach (var economicTaskType in unconvertedTaskTypesFromEconomic)
                {
                    if (databaseTaskType.EconomicTaskTypeId == economicTaskType.EconomicTaskTypeId)
                    {
                        databaseTaskType.Name = economicTaskType.Name;
                        found = true;
                    }
                }

                if (!found)
                {
                    foreach (var task in databaseTaskType.Task.ToList())
                    {
                        Container.DeleteObject(task);
                    }
                    Container.DeleteObject(databaseTaskType);
                    taskTypesFromDatabase.Remove(databaseTaskType);
                }
            }
            Container.SaveChanges();
            Debug.Print("1344");
            foreach (var rawEconomicTaskType in unconvertedTaskTypesFromEconomic)
            {

                var task =
                    taskTypesFromDatabase.FirstOrDefault(
                        t => t.EconomicTaskTypeId == rawEconomicTaskType.EconomicTaskTypeId);
                if (task == null)
                {
                    task = EconomicTaskType.CreateEconomicTaskType(0,
                        rawEconomicTaskType.EconomicTaskTypeId,
                        rawEconomicTaskType.Name);

                    Container.EconomicTaskTypeSet.AddObject(task);
                }

                var economicProject = projectsFromDatabase.FirstOrDefault(
                    p => rawEconomicTaskType.EconomicProject.Any(t => t.EconomicProjectId == p.EconomicProjectId));
                if (economicProject != null && !task.EconomicProject.Contains(economicProject))
                {
                    task.EconomicProject.Add(economicProject);
                    economicProject.EconomicTaskTypes.Add(task);
                }

                Container.SaveChanges();
            }

            Debug.Print("END");
        }

        public int UpdateCache()
        {
            var newEconomicProjects = EconomicSession.Project.GetAllUpdated(DateTime.Now.AddDays(-7), false).ToList();
            Console.WriteLine(newEconomicProjects.Count);

            var index = 0;

            Debug.Print("Fetching projects ...");

            var convertedProjects = newEconomicProjects.Select((source) =>
            {

                var mainProject = source.MainProject;
                if (mainProject != null)
                {

                    string name = string.Empty;
                    try
                    {
                        name = source.Name;
                    }
                    catch (WebException)
                    {
                    }

                    int number = source.Number;

                    Debug.Print("Project #" + ++index + " (" + number +
                                ": " + name + " {StatusSource:" + source.IsClosed + " } Group: " +
                                source.ProjectGroup.Number + ") fetched.");

                    if (source.IsClosed || source.MainProject.IsClosed)
                    {
                        //Hvis task eller den hovedsag task hører til er lukket, medtages den ikke
                        return null;
                    }
                    else
                    {
                        return new
                        {
                            Name = name,
                            Number = number,
                            Activities = source.GetActivities(),
                            Group = source.ProjectGroup
                        };
                    }
                }
                else
                {
                    return null;
                }
            }).ToList();

            Debug.Print("Done fetching all projects.");
            Debug.Print("Fetching activities ...");

            var affectedUpdate = 0;
            foreach (var project in convertedProjects)
            {

                if (project != null)
                {

                    EconomicProject cacheProject = null;
                    if (economicProjectCache.ContainsKey(project.Number))
                    {
                        cacheProject = economicProjectCache[project.Number];
                    }
                    else
                    {
                        if (GetEconomicProjectFromCache(project.Number) != null)
                        {
                            var group =
                                GetAllGroupsFromCache().First(g => g.EconomicGroupNumber == project.Group.Number);
                            cacheProject = CreateEconomicProjectIfNotExists(project.Number, project.Name, group);
                            economicProjectCache.Add(project.Number, cacheProject);
                        }
                        else
                        {
                            var group =
                                GetAllGroupsFromCache().First(g => g.EconomicGroupNumber == project.Group.Number);
                            cacheProject = CreateEconomicProjectIfNotExists(project.Number, project.Name, group);
                            economicProjectCache.Add(project.Number, cacheProject);
                            affectedUpdate++;
                        }

                    }

                    Debug.Print("Fetching activities for project \"" + cacheProject.Name + "\" ...");

                    foreach (var activity in project.Activities)
                    {
                        var activityNumber = activity.Number;

                        EconomicTaskType taskType = null;
                        if (economicTaskTypeCache.ContainsKey(activityNumber))
                        {
                            taskType = economicTaskTypeCache[activityNumber];
                        }
                        else
                        {
                            var activityName = activity.Name;

                            Debug.Print(" > " + activityName + " (" + activityNumber + ")");

                            taskType = new EconomicTaskType()
                            {
                                Name = activityName,
                                EconomicTaskTypeId = activityNumber
                            };

                            if (!taskType.EconomicProject.Contains(cacheProject))
                            {
                                taskType.EconomicProject.Add(cacheProject);
                            }

                            economicTaskTypeCache.Add(activityNumber, taskType);
                        }
                        if (!cacheProject.EconomicTaskTypes.Contains(taskType))
                        {
                            cacheProject.EconomicTaskTypes.Add(taskType);
                        }

                    }
                }
            }
            Container.SaveChanges();
            return affectedUpdate;
        }

        public void CheckEconomicProjectNames()
        {
            var allproject = EconomicSession.Project.GetAll();
            var allprojectsFromCache = GetAllEconomicProjectsFromCache();
            var numberOfOpenProjects = new Dictionary<int, string>();
            foreach (var project in allproject)
            {
                if (project.IsMainProject == false && project.MainProject != null)
                {
                    var open = EconomicSession.Project.GetOpenSubProjects(project.MainProject);
                    foreach (var eProject in open)
                    {
                        var num = eProject.Number;
                        var name = eProject.Name;
                        if (numberOfOpenProjects.Keys.Contains(num) == false)
                        {
                            numberOfOpenProjects.Add(num, name);
                        }
                    }
                }
                else if (project.IsMainProject == true && project.MainProject == null)
                {
                    var open = EconomicSession.Project.GetOpenSubProjects(project);
                    foreach (var eProject in open)
                    {
                        var num = eProject.Number;
                        var name = eProject.Name;
                        if (numberOfOpenProjects.Keys.Contains(num) == false)
                        {
                            numberOfOpenProjects.Add(num, name);
                        }
                    }
                }
            }
            var allProjectsFromCacheInclusiveNew = GetAllEconomicProjectsFromCache();
            for (int i = 0; i < numberOfOpenProjects.Keys.Count; i++)
            {
                var cacheProject =
                    allProjectsFromCacheInclusiveNew.FirstOrDefault(
                        e => e.EconomicProjectId == numberOfOpenProjects.Keys.ElementAt(i));
                if (cacheProject != null)
                {
                    var projectName = numberOfOpenProjects.Values.ElementAt(i);
                    if (cacheProject.Name.Equals(projectName) == false)
                    {
                        cacheProject.Name = numberOfOpenProjects.Values.ElementAt(i);
                    }
                }
            }
        }

        public int CleanCache()
        {
            //var allproject = EconomicSession.Project.GetAll();
            var allprojectsFromCache = GetAllEconomicProjectsFromCache();
            var numberOfOpenProjects = new List<int>();
            //foreach (var project in allproject)
            //{
            //    if (project.IsMainProject == false && project.MainProject != null)
            //    {
            //        var open = EconomicSession.Project.GetOpenSubProjects(project.MainProject);
            //        foreach (var eProject in open)
            //        {
            //            var num = eProject.Number;
            //            if (numberOfOpenProjects.Contains(num) == false)
            //            {
            //                numberOfOpenProjects.Add(num);
            //            }
            //        }
            //    }
            //    else if (project.IsMainProject == true && project.MainProject == null)
            //    {
            //        var open = EconomicSession.Project.GetOpenSubProjects(project);
            //        foreach (var eProject in open)
            //        {
            //            var num = eProject.Number;
            //            if (numberOfOpenProjects.Contains(num) == false)
            //            {
            //                numberOfOpenProjects.Add(num);
            //            }
            //        }
            //    }
            //}
            // Remaining in leftProjectsInCache must be deleted.
            var leftProjectsInCache = new List<EconomicProject>();
            leftProjectsInCache.Add(GetAllEconomicProjectsFromCache().FirstOrDefault(e => e.EconomicProjectId == 50019));
            //foreach (var num in numberOfOpenProjects)
            //{
            //    EconomicProject foundProject = allprojectsFromCache.FirstOrDefault(e => e.EconomicProjectId == num);
            //    if (foundProject != null)
            //    {
            //        leftProjectsInCache.Remove(foundProject);
            //    }
            //    //TODO:Else{ den skal tilføjes, gøres i UpdateCache()}
            //}


            for (int i = 0; i < leftProjectsInCache.Count; i++)
            {
                var deleteProject = leftProjectsInCache.ElementAt(i);
                for (int k = 0; k < deleteProject.Activities.Count; k++)
                {
                    var activity = deleteProject.Activities.ElementAt(k);
                    var routine = activity.Routine;
                    if (routine != null)
                    {
                        routine.Activities.Remove(activity);
                    }
                    activity.Task = null;
                    activity.TakenBy = null;
                    for (int l = 0; l < activity.ActivityUser.Count; l++)
                    {
                        activity.ActivityUser.Remove(activity.ActivityUser.ElementAt(l));
                    }
                    Container.ActivitySet.DeleteObject(activity);
                }

                var tasks = Container.TaskSet.OrderBy(t => t.Id).ToList();
                for (int x = 0; x < tasks.Count; x++)
                {
                    var task = tasks.ElementAt(x);
                    if (task.EconomicProject.EconomicProjectId == deleteProject.EconomicProjectId)
                    {
                        Container.TaskSet.DeleteObject(task);
                    }
                }
                var taskTypes = deleteProject.EconomicTaskTypes;
                for (var j = 0; j < taskTypes.Count; j++)
                {

                    var anTaskType = taskTypes.ElementAt(j);
                    deleteProject.EconomicTaskTypes.Remove(anTaskType);
                }
                Container.EconomicProjectSet.DeleteObject(deleteProject);
                Container.SaveChanges();
            }

            return leftProjectsInCache.Count;
        }

        //SKAL BRUGES TIL SØRENS REDIGERINGSSIDE
        public IEnumerable<Group> GetGroupsFromEconomic()
        {
            var allGroups = EconomicSession.ProjectGroup.GetAll();
            var convertedGroups = allGroups.Select((source) =>
            {
                return new Group()
                {
                    Name = source.Name,
                    EconomicGroupNumber = source.Number
                };

            }
                ).ToList();

            return convertedGroups;
        }


        public IEnumerable<EconomicTaskType> GetTaskTypesFromEconomic()
        {
            var result = new List<EconomicTaskType>();

            var index = 0;

            Debug.Print("Fetching projects ...");
            //Alle sager som er åbne (iikke i afsluttede sager) er debitornavn på. GetAll() henter også afsluttede som har fejl som Søren ikk kan rette.
            var allProjects = EconomicSession.Project.GetAll();
            var convertedProjects = allProjects.Select((source) =>
            {

                var mainProject = source.MainProject;
                if (mainProject != null)
                {

                    string name = string.Empty;
                    try
                    {
                        name = source.Name;
                    }
                    catch (WebException)
                    {
                    }

                    int number = source.Number;

                    Debug.Print("Project #" + ++index + " (" + number +
                                ": " + name + " {StatusSource:" + source.IsClosed + " } Group: " +
                                source.ProjectGroup.Number + ") fetched.");


                    if (source.IsClosed || source.MainProject.IsClosed)
                    {
                        //Hvis task eller den hovedsag task hører til er lukket, medtages den ikke
                        return null;
                    }
                    else
                    {
                        return new
                        {
                            Name = name,
                            Number = number,
                            Activities = source.GetActivities(),
                            Group = source.ProjectGroup
                        };
                    }
                }
                else
                {
                    return null;
                }
            }).ToList();

            Debug.Print("Done fetching all projects.");
            Debug.Print("Fetching activities ...");

            foreach (var project in convertedProjects)
            {

                if (project != null)
                {

                    EconomicProject cacheProject = null;
                    if (economicProjectCache.ContainsKey(project.Number))
                    {
                        cacheProject = economicProjectCache[project.Number];
                    }
                    else
                    {
                        cacheProject = new EconomicProject() { Name = project.Name, EconomicProjectId = project.Number };
                        cacheProject.Group =
                            GetAllGroupsFromCache().First(g => g.EconomicGroupNumber == project.Group.Number);
                        economicProjectCache.Add(project.Number, cacheProject);
                    }

                    Debug.Print("Fetching activities for project \"" + cacheProject.Name + "\" ...");

                    foreach (var activity in project.Activities)
                    {
                        var activityNumber = activity.Number;

                        EconomicTaskType taskType = null;
                        if (economicTaskTypeCache.ContainsKey(activityNumber))
                        {
                            taskType = economicTaskTypeCache[activityNumber];
                        }
                        else
                        {
                            var activityName = activity.Name;

                            Debug.Print(" > " + activityName + " (" + activityNumber + ")");

                            taskType = new EconomicTaskType()
                            {
                                Name = activityName,
                                EconomicTaskTypeId = activityNumber
                            };

                            if (!taskType.EconomicProject.Contains(cacheProject))
                            {
                                taskType.EconomicProject.Add(cacheProject);
                            }

                            economicTaskTypeCache.Add(activityNumber, taskType);
                        }
                        if (!cacheProject.EconomicTaskTypes.Contains(taskType))
                        {
                            cacheProject.EconomicTaskTypes.Add(taskType);
                        }
                        result.Add(taskType);
                    }
                }
            }

            return result;
        }

        //---------------------------------------------------------------------TASK OG PAUSE ----------------------------------------------------------------------
        //TODO TASK AND PAUSE

        public void ChangePauseUpDown(bool up, bool down, DailyReport dailyReport)
        {
            var tasks = dailyReport.Task.ToList();
            var bre = dailyReport.Break;
            if (up)
            {
                var taskBeforeBreakNoTidshul = tasks.FirstOrDefault(t => t.TimeEnded.Equals(bre.BreakStarted));
                if (taskBeforeBreakNoTidshul != null)
                {
                    TimeSpan timeSpan = taskBeforeBreakNoTidshul.TimeEnded.Value -
                                        taskBeforeBreakNoTidshul.TimeStarted.Value;
                    bre.BreakStarted = taskBeforeBreakNoTidshul.TimeStarted;
                    bre.BreakEnded = bre.BreakStarted.Value.AddMinutes(30);
                    taskBeforeBreakNoTidshul.TimeStarted = bre.BreakEnded;
                    taskBeforeBreakNoTidshul.TimeEnded = taskBeforeBreakNoTidshul.TimeStarted.Value.Add(timeSpan);
                }
                else
                {
                    TimeSpan timeSpanFirst = new TimeSpan();
                    TimeSpan timeSpanSecond = new TimeSpan();
                    Task task = null;
                    var tasksBeforeBreak = tasks.OrderBy(t => t.TimeStarted).Where(t => t.TimeEnded < bre.BreakStarted);
                    if (tasksBeforeBreak.Any())
                    {
                        for (int i = 0; i < tasksBeforeBreak.Count(); i++)
                        {
                            if (i + 1 < tasksBeforeBreak.Count() && tasksBeforeBreak.ElementAt(i + 1) != null)
                            {
                                var taskFirst = tasksBeforeBreak.ElementAt(i);
                                var taskSecond = tasksBeforeBreak.ElementAt(i + 1);
                                timeSpanFirst = bre.BreakStarted.Value - taskFirst.TimeEnded.Value;
                                timeSpanSecond = bre.BreakStarted.Value - taskSecond.TimeEnded.Value;
                                if (timeSpanSecond < timeSpanFirst)
                                {
                                    task = taskSecond;
                                }
                            }
                            if (tasksBeforeBreak.Count().Equals(1))
                            {
                                task = tasksBeforeBreak.ElementAt(i);
                            }
                        }
                        bre.BreakStarted = task.TimeEnded;
                        bre.BreakEnded = bre.BreakStarted.Value.AddMinutes(30);
                    }
                    else
                    {
                        bre.BreakStarted = dailyReport.DayStarted.Value;
                        bre.BreakEnded = bre.BreakStarted.Value.AddMinutes(30);
                    }
                }
            }
            if (down)
            {
                var taskAfterBreakNoTidshul = tasks.FirstOrDefault(t => t.TimeStarted.Equals(bre.BreakEnded));
                if (taskAfterBreakNoTidshul != null)
                {
                    TimeSpan timeSpan = taskAfterBreakNoTidshul.TimeEnded.Value -
                                        taskAfterBreakNoTidshul.TimeStarted.Value;

                    taskAfterBreakNoTidshul.TimeStarted = bre.BreakStarted;
                    taskAfterBreakNoTidshul.TimeEnded = taskAfterBreakNoTidshul.TimeStarted.Value.Add(timeSpan);
                    bre.BreakStarted = taskAfterBreakNoTidshul.TimeEnded;
                    bre.BreakEnded = bre.BreakStarted.Value.AddMinutes(30);
                }
                else
                {
                    TimeSpan timeSpanFirst = new TimeSpan();
                    TimeSpan timeSpanAfterFirst = new TimeSpan();
                    Task task = null;
                    var tasksAfterBreak =
                        tasks.OrderBy(t => t.TimeEnded).Where(t => t.TimeStarted > bre.BreakEnded).ToList();
                    if (tasksAfterBreak.Any())
                    {
                        for (int i = 0; i < tasksAfterBreak.Count(); i++)
                        {
                            if (i + 1 < tasksAfterBreak.Count() && tasksAfterBreak.ElementAt(i + 1) != null)
                            {
                                var taskFirst = tasksAfterBreak.ElementAt(i);
                                var taskAfterFirst = tasksAfterBreak.ElementAt(i + 1);
                                timeSpanFirst = taskFirst.TimeStarted.Value - bre.BreakEnded.Value;
                                timeSpanAfterFirst = taskAfterFirst.TimeStarted.Value - bre.BreakEnded.Value;
                                if (timeSpanFirst < timeSpanAfterFirst)
                                {
                                    task = taskFirst;
                                    break;
                                }
                            }
                            if (tasksAfterBreak.Count().Equals(1))
                            {
                                task = tasksAfterBreak.ElementAt(i);
                            }
                        }
                        bre.BreakEnded = task.TimeStarted;
                        bre.BreakStarted = bre.BreakEnded.Value.AddMinutes(-30);
                    }
                    else
                    {
                        if (dailyReport.DayEnded.HasValue)
                        {
                            bre.BreakEnded = dailyReport.DayEnded;
                            bre.BreakStarted = bre.BreakEnded.Value.AddMinutes(-30);
                        }
                    }
                }
            }
            MergeEqualTasksInSequence(dailyReport);
            Container.SaveChanges();
        }

        private void AdjustTasksToFitBreak(DailyReport dailyReport, Task newTask, bool moveTasks)
        {
            var bre = dailyReport.Break;

            // Newtask starter før pause og slutter indenfor.
            if (bre.BreakEnded >= newTask.TimeEnded && newTask.TimeEnded > bre.BreakStarted &&
                newTask.TimeStarted < bre.BreakStarted)
            {
                // En task som starter før newtask og slutter inden i newtask tidsrum.
                foreach (
                    var task in
                        dailyReport.Task.Where(
                            t =>
                                t.TimeStarted < newTask.TimeStarted && t.TimeEnded > newTask.TimeStarted &&
                                t.TimeEnded <= newTask.TimeEnded))
                {
                    task.TimeEnded = newTask.TimeStarted;
                    break;
                }

                var tasksWithinNewtask =
                    dailyReport.Task.Where(
                        t => t.TimeStarted >= newTask.TimeStarted && t.TimeEnded < newTask.TimeEnded).ToList();

                var tasksAfterNewTask = dailyReport.Task.Where(t => t.TimeStarted >= newTask.TimeEnded).ToList();
                var taskBeforeBreak =
                    dailyReport.Task.FirstOrDefault(
                        t =>
                            t.TimeStarted < newTask.TimeStarted && t.TimeEnded < newTask.TimeEnded &&
                            bre.BreakStarted < newTask.TimeEnded);
                if (moveTasks)
                {
                    if (tasksWithinNewtask.Any())
                    {
                        var firstTaskWithinNewtask = tasksWithinNewtask.OrderBy(t => t.TimeStarted).First();
                        TimeSpan timeSpanToMoveTasksWithin = newTask.TimeEnded.Value -
                                                             firstTaskWithinNewtask.TimeStarted.Value;
                        foreach (var taskWithin in tasksWithinNewtask)
                        {
                            AddTimeSpanToTask(timeSpanToMoveTasksWithin, taskWithin);
                        }
                        bre.BreakStarted = bre.BreakStarted.Value.Add(timeSpanToMoveTasksWithin);
                        bre.BreakEnded = bre.BreakStarted.Value.AddMinutes(30);

                        foreach (var taskAfter in tasksAfterNewTask)
                        {
                            AddTimeSpanToTask(timeSpanToMoveTasksWithin, taskAfter);
                        }

                    }
                    if (taskBeforeBreak != null)
                    {
                        TimeSpan timeSpanToMove = newTask.TimeEnded.Value - bre.BreakStarted.Value;
                        bre.BreakStarted = bre.BreakStarted.Value.Add(timeSpanToMove);
                        bre.BreakEnded = bre.BreakEnded.Value.Add(timeSpanToMove);
                        foreach (var taskAfter in tasksAfterNewTask)
                        {
                            AddTimeSpanToTask(timeSpanToMove, taskAfter);
                        }

                    }

                }
                else
                {
                    //Overwrite

                    if (tasksWithinNewtask.Any())
                    {
                        DeleteTasksWithinNewtask(dailyReport, newTask, bre);
                    }
                    bre.BreakStarted = newTask.TimeEnded;
                    bre.BreakEnded = bre.BreakStarted.Value.AddMinutes(30);
                    var tasksAfterBreakToBeDeleted =
                        tasksAfterNewTask.Where(
                            t => t.TimeStarted > bre.BreakStarted && t.TimeEnded <= bre.BreakEnded).ToList();
                    foreach (var taskDel in tasksAfterBreakToBeDeleted)
                    {
                        dailyReport.Task.Remove(taskDel);
                        Container.TaskSet.DeleteObject(taskDel);
                    }

                    var taskAfterBreak =
                        tasksAfterNewTask.FirstOrDefault(
                            t => t.TimeEnded > bre.BreakEnded && t.TimeStarted < bre.BreakEnded);
                    if (taskAfterBreak != null)
                    {
                        taskAfterBreak.TimeStarted = bre.BreakEnded;
                    }
                }
                //Slut tid skal justeres
                if (dailyReport.Task.Count > 0)
                {
                    AdjustDayEnded(dailyReport, dailyReport.Task.OrderBy(t => t.TimeEnded).First());
                }
            }
            // Newtask starter i pause og slutter efter.
            else if (newTask.TimeStarted < bre.BreakEnded && newTask.TimeStarted >= bre.BreakStarted &&
                     newTask.TimeEnded > bre.BreakEnded)
            {

                ShortenTasksThatStartsBeforeAndEndsAfterNewtask(dailyReport, newTask);
                if (moveTasks)
                {
                    // Finder overlap mellem break og newtask og adderer med newtask tid.
                    var timeSpanOverlapBreak = (bre.BreakEnded.Value - newTask.TimeStarted.Value);
                    var timeSpan = (newTask.TimeEnded.Value - newTask.TimeStarted.Value);
                    // Tilføjer tiden til de resterende tasks efter pausen.
                    foreach (var task in dailyReport.Task.Where(t => t.TimeStarted >= bre.BreakEnded))
                    {
                        task.TimeStarted = task.TimeStarted.Value.Add(timeSpan);
                        task.TimeEnded = task.TimeEnded.Value.Add(timeSpan);
                    }
                    // Sætter newtask tid så den begynder efter pausen.
                    newTask.TimeStarted = newTask.TimeStarted.Value.Add(timeSpanOverlapBreak);
                    newTask.TimeEnded = newTask.TimeEnded.Value.Add(timeSpanOverlapBreak);
                    if (dailyReport.Task.Count > 0)
                    {
                        AdjustDayEnded(dailyReport, newTask);
                    }
                }
                else
                {
                    //Overwrite

                    // Sletter tasks som ligger indenfor newtask tidsrum og før pausen.
                    DeleteTasksWithinNewtask(dailyReport, newTask, bre);
                    // Fjerner dem som ligger inden for newtask's tidsrum.
                    var tasksToBeDeleted =
                        dailyReport.Task.Where(
                            t =>
                                t.TimeStarted >= newTask.TimeStarted && t.TimeStarted < newTask.TimeEnded &&
                                t.TimeEnded <= newTask.TimeEnded).ToList();
                    foreach (var delTask in tasksToBeDeleted)
                    {
                        dailyReport.Task.Remove(delTask);
                        Container.TaskSet.DeleteObject(delTask);
                    }
                    // Den finder den task som starter i newtask tidsrum og slutter efter, og sætter start på task til newtask sluttid.
                    foreach (
                        var task in
                            dailyReport.Task.Where(
                                t =>
                                    t.TimeStarted >= newTask.TimeStarted && t.TimeStarted < newTask.TimeEnded &&
                                    t.TimeEnded > newTask.TimeEnded))
                    {
                        task.TimeStarted = newTask.TimeEnded;
                        break;
                    }
                    newTask.TimeStarted = bre.BreakEnded;
                    if (dailyReport.Task.Count > 0)
                    {
                        AdjustDayEnded(dailyReport, newTask);
                    }
                }
            }
            // Newtask starter før pause og slutter efter pause.
            else if (newTask.TimeStarted < bre.BreakStarted && newTask.TimeEnded > bre.BreakEnded)
            {

                ShortenTasksThatStartsBeforeAndEndsAfterNewtask(dailyReport, newTask);
                if (moveTasks)
                {
                    var timeSpan = newTask.TimeEnded.Value - bre.BreakStarted.Value;
                    bre.BreakStarted = bre.BreakStarted.Value.Add(timeSpan);
                    bre.BreakEnded = bre.BreakEnded.Value.Add(timeSpan);
                    foreach (var task in dailyReport.Task.Where(t => t.TimeStarted > newTask.TimeStarted))
                    {
                        task.TimeStarted = task.TimeStarted.Value.Add(timeSpan);
                        task.TimeEnded = task.TimeEnded.Value.Add(timeSpan);
                    }
                    if (dailyReport.Task.Count > 0)
                    {
                        AdjustDayEnded(dailyReport, dailyReport.Task.OrderByDescending(t => t.TimeEnded).First());
                    }
                }
                else
                {
                    //overwrite

                    // Sletter tasks som ligger indenfor newtask tidsrum og før pausen.
                    DeleteTasksWithinNewtask(dailyReport, newTask, bre);
                    //Dele newtask i en del før pause og efter pause samt overskrive tasks den måtte ramme.
                    var taskCopy = Task.CreateTask(0, newTask.isEmailed);
                    taskCopy.EconomicProject = newTask.EconomicProject;
                    taskCopy.EconomicTask = newTask.EconomicTask;
                    //Den første overskrivning
                    taskCopy.TimeStarted = newTask.TimeStarted;
                    taskCopy.TimeEnded = bre.BreakStarted;
                    dailyReport.Task.Add(taskCopy);
                    Container.TaskSet.AddObject(taskCopy);
                    // Den anden overskrivning
                    newTask.TimeStarted = bre.BreakEnded;
                    // Fjerner dem som ligger inden for newtask's tidsrum.
                    var tasksToBeDeleted =
                        dailyReport.Task.Where(
                            t =>
                                t.TimeStarted >= newTask.TimeStarted && t.TimeStarted < newTask.TimeEnded &&
                                t.TimeEnded <= newTask.TimeEnded).ToList();
                    foreach (var delTask in tasksToBeDeleted)
                    {
                        dailyReport.Task.Remove(delTask);
                        Container.TaskSet.DeleteObject(delTask);
                    }
                    // Den finder den task som starter i newtask tidsrum og slutter efter, og sætter start på task til newtask sluttid.
                    foreach (
                        var task in
                            dailyReport.Task.Where(
                                task =>
                                    task.TimeStarted >= newTask.TimeStarted && task.TimeStarted < newTask.TimeEnded &&
                                    task.TimeEnded > newTask.TimeEnded))
                    {
                        task.TimeStarted = newTask.TimeEnded;
                        break;
                    }
                    if (dailyReport.Task.Count > 0)
                    {
                        AdjustDayEnded(dailyReport, dailyReport.Task.OrderByDescending(t => t.TimeEnded).First());
                    }
                }
            }
            //Alle andre situationer, hvor newtask ikke ligger indenfor pausens tidsrum.
            else
            {
                // En task som starter før newtask og slutter inden i newtask tidsrum.
                foreach (
                    var task in
                        dailyReport.Task.Where(
                            t =>
                                t.TimeStarted < newTask.TimeStarted && t.TimeEnded > newTask.TimeStarted &&
                                t.TimeEnded <= newTask.TimeEnded))
                {
                    task.TimeEnded = newTask.TimeStarted;
                    break;
                }

                // En task som starter før og slutter efter newtask.
                var taskOverlap =
                    dailyReport.Task.FirstOrDefault(
                        t => t.TimeStarted < newTask.TimeStarted && t.TimeEnded > newTask.TimeEnded);

                if (moveTasks)
                {

                    if (taskOverlap != null)
                    {
                        var timeEndedOverlap = taskOverlap.TimeEnded;
                        ShortenTasksThatStartsBeforeAndEndsAfterNewtask(dailyReport, newTask);
                        TimeSpan timeSpan = newTask.TimeEnded.Value - newTask.TimeStarted.Value;
                        foreach (var taskMove in dailyReport.Task.Where(t => t.TimeStarted > taskOverlap.TimeEnded))
                        {
                            AddTimeSpanToTask(timeSpan, taskMove);
                        }
                        if (bre.BreakStarted >= taskOverlap.TimeEnded)
                        {
                            bre.BreakStarted = bre.BreakStarted.Value.Add(timeSpan);
                            bre.BreakEnded = bre.BreakEnded.Value.Add(timeSpan);
                        }
                        //Dele newtask
                        var taskCopy = Task.CreateTask(0, newTask.isEmailed);
                        taskCopy.EconomicProject = taskOverlap.EconomicProject;
                        taskCopy.EconomicTask = taskOverlap.EconomicTask;
                        //Den første overskrivning (efter indsættelse)
                        taskCopy.TimeStarted = newTask.TimeEnded;
                        taskCopy.TimeEnded = timeEndedOverlap.Value.Add(timeSpan);
                        dailyReport.Task.Add(taskCopy);
                        Container.TaskSet.AddObject(taskCopy);
                        // Den anden overskrivning (før)
                        taskOverlap.TimeEnded = newTask.TimeStarted;
                    }
                    else
                    {
                        var task =
                            dailyReport.Task.FirstOrDefault(
                                t =>
                                    t.TimeStarted < newTask.TimeEnded && t.TimeStarted > newTask.TimeStarted &&
                                    t.TimeEnded > newTask.TimeEnded);
                        if (task != null)
                        {
                            TimeSpan timeSpan = newTask.TimeEnded.Value - task.TimeStarted.Value;
                            foreach (
                                var source in
                                    dailyReport.Task.Where(
                                        t => t.TimeStarted > newTask.TimeStarted && t.TimeEnded > newTask.TimeEnded)
                                )
                            {
                                AddTimeSpanToTask(timeSpan, source);
                            }
                            if (bre.BreakStarted >= newTask.TimeEnded)
                            {
                                bre.BreakStarted = bre.BreakStarted.Value.Add(timeSpan);
                                bre.BreakEnded = bre.BreakEnded.Value.Add(timeSpan);
                            }
                        }
                        //Tasks som ligger indenfor tidsrummet af newtask skal rykkes.
                        var tasksWithinNewTask =
                            dailyReport.Task.Where(
                                t => t.TimeStarted >= newTask.TimeStarted && t.TimeEnded <= newTask.TimeEnded)
                                .ToList();
                        if (tasksWithinNewTask.Count > 0)
                        {
                            TimeSpan timeSpan = new TimeSpan();
                            foreach (var tasksWithIn in tasksWithinNewTask)
                            {
                                timeSpan = timeSpan.Add(tasksWithIn.TimeEnded.Value - tasksWithIn.TimeStarted.Value);
                            }
                            foreach (var source in dailyReport.Task.Where(t => t.TimeStarted >= newTask.TimeEnded))
                            {
                                AddTimeSpanToTask(timeSpan, source);
                            }
                            if (bre.BreakStarted >= newTask.TimeEnded)
                            {
                                bre.BreakStarted = bre.BreakStarted.Value.Add(timeSpan);
                                bre.BreakEnded = bre.BreakEnded.Value.Add(timeSpan);
                            }

                            var firstTaskWithIn = tasksWithinNewTask.OrderBy(t => t.TimeStarted).First();
                            TimeSpan timeBeforeFirstTask = firstTaskWithIn.TimeStarted.Value -
                                                           newTask.TimeStarted.Value;
                            TimeSpan timeToMoveTasksWithin = (newTask.TimeEnded.Value - newTask.TimeStarted.Value) -
                                                             timeBeforeFirstTask;
                            foreach (var source in tasksWithinNewTask)
                            {
                                AddTimeSpanToTask(timeToMoveTasksWithin, source);
                            }
                        }

                    }
                }
                else
                {
                    //Overwrite
                    DeleteTasksWithinNewtask(dailyReport, newTask, bre);
                    if (taskOverlap != null)
                    {
                        //Dele newtask i en del før pause og efter pause samt overskrive tasks den måtte ramme.
                        var taskCopy = Task.CreateTask(0, newTask.isEmailed);
                        taskCopy.EconomicProject = taskOverlap.EconomicProject;
                        taskCopy.EconomicTask = taskOverlap.EconomicTask;
                        //Den første overskrivning (efter )
                        taskCopy.TimeStarted = newTask.TimeEnded;
                        taskCopy.TimeEnded = taskOverlap.TimeEnded;
                        dailyReport.Task.Add(taskCopy);
                        Container.TaskSet.AddObject(taskCopy);
                        // Den anden overskrivning (før)
                        taskOverlap.TimeEnded = newTask.TimeStarted;
                    }
                    else
                    {
                        ShortenTasksThatStartsBeforeAndEndsAfterNewtask(dailyReport, newTask);
                        // Finder som starter før newtask og slutter inden i newtask tidsrum.
                        var task =
                            dailyReport.Task.FirstOrDefault(
                                t =>
                                    t.TimeStarted < newTask.TimeEnded && t.TimeStarted > newTask.TimeStarted &&
                                    t.TimeEnded > newTask.TimeEnded);
                        if (task != null)
                        {
                            task.TimeStarted = newTask.TimeEnded;
                        }

                        var taskThatStartsIntheMiddleAndEndsAfterNewtask =
                            dailyReport.Task.FirstOrDefault(
                                t =>
                                    t.TimeStarted >= newTask.TimeStarted && t.TimeStarted < newTask.TimeEnded &&
                                    t.TimeEnded > newTask.TimeEnded);
                        if (taskThatStartsIntheMiddleAndEndsAfterNewtask != null)
                        {
                            taskThatStartsIntheMiddleAndEndsAfterNewtask.TimeStarted = newTask.TimeEnded;
                        }
                    }
                }
                if (dailyReport.Task.Count > 0)
                {
                    AdjustDayEnded(dailyReport, newTask);
                }
            }
            MergeEqualTasksInSequence(dailyReport);
        }

        private void ShortenTasksThatStartsBeforeAndEndsAfterNewtask(DailyReport dailyReport, Task newTask)
        {
            //Forkorter tiden på den task som starter før og slutter efter newtask start.
            foreach (
                var task in
                    dailyReport.Task.Where(t => t.TimeStarted < newTask.TimeStarted && t.TimeEnded > newTask.TimeStarted)
                )
            {
                task.TimeEnded = newTask.TimeStarted;
                break;
            }
        }

        private void AddTimeSpanToTask(TimeSpan timeSpan, Task task)
        {
            task.TimeStarted = task.TimeStarted.Value.Add(timeSpan);
            task.TimeEnded = task.TimeEnded.Value.Add(timeSpan);
        }

        private void DeleteTasksWithinNewtask(DailyReport dailyReport, Task newTask, Break bre)
        {
            var taskToBeDeletedBeforeNewTask =
                dailyReport.Task.Where(
                    t =>
                        t.TimeStarted >= newTask.TimeStarted && t.TimeEnded <= bre.BreakStarted &&
                        t.TimeEnded <= newTask.TimeEnded)
                    .
                    ToList();
            foreach (var delTask in taskToBeDeletedBeforeNewTask)
            {
                dailyReport.Task.Remove(delTask);
                Container.TaskSet.DeleteObject(delTask);
            }
        }

        private void AdjustDayEnded(DailyReport dailyReport, Task newTask)
        {
            if (dailyReport.DayEnded.HasValue)
            {
                if (newTask.TimeEnded > dailyReport.DayEnded)
                {
                    dailyReport.DayEnded = newTask.TimeEnded;
                }
                if (dailyReport.Break.BreakEnded > dailyReport.DayEnded)
                {
                    dailyReport.DayEnded = dailyReport.Break.BreakEnded;
                }
            }
        }

        private void MergeEqualTasksInSequence(DailyReport dailyReport)
        {
            var tasksToBeDeleted = new List<Task>();
            for (int i = 0; i < dailyReport.Task.Count(); i++)
            {
                if (i + 1 < dailyReport.Task.Count)
                {
                    var firstTask = dailyReport.Task.OrderBy(t => t.TimeStarted).ElementAt(i);
                    var secondTask = dailyReport.Task.OrderBy(t => t.TimeStarted).ElementAt(i + 1);
                    if (firstTask.EconomicProject.Equals(secondTask.EconomicProject) &&
                        firstTask.EconomicTask.Equals(secondTask.EconomicTask) &&
                        firstTask.TimeEnded.Value.Equals(secondTask.TimeStarted.Value))
                    {
                        firstTask.TimeEnded = secondTask.TimeEnded;
                        tasksToBeDeleted.Add(secondTask);
                        i++;
                    }
                }
            }
            foreach (var task in tasksToBeDeleted)
            {
                dailyReport.Task.Remove(task);
                Container.TaskSet.DeleteObject(task);
            }

        }

        public Task CreateTask(DateTime timeStarted, DateTime timeEnded, DailyReport dailyReport,
            EconomicProject economicProject, EconomicTaskType economicTaskType, bool moveTasks)
        {
            try
            {
                var newTask = Task.CreateTask(0, false);
                newTask.TimeStarted = timeStarted;
                newTask.TimeEnded = timeEnded;
                newTask.EconomicProject = economicProject;
                newTask.EconomicTask = economicTaskType;
                AdjustTasksToFitBreak(dailyReport, newTask, moveTasks);
                dailyReport.Task.Add(newTask);
                Container.TaskSet.AddObject(newTask);
                Container.SaveChanges();
                return newTask;
            }
            catch (Exception ex)
            {
                SendMailToDeveloper("(ExceptionID=10)", ex.Message, ex.StackTrace);
                throw new Exception("Unable to create task" + "<br/>" +
                                    "A message has been sent to the developer. Please try again.");
            }
        }

        public void UpdateTask(EconomicProject project, EconomicTaskType taskType, Task updateTask,
            DailyReport dailyReport, DateTime startTime, DateTime endTime, bool moveTasks)
        {
            try
            {
                dailyReport.Task.Remove(updateTask);
                Container.TaskSet.DeleteObject(updateTask);
                Container.SaveChanges();
                CreateTask(startTime, endTime, dailyReport, project, taskType, moveTasks);
            }
            catch (Exception ex)
            {
                SendMailToDeveloper("(ExceptionID=11)", ex.Message, ex.StackTrace);
                throw new Exception("Unable to update task" + "<br/>" +
                                    "A message has been sent to the developer. Please try again.");
            }
        }

        public void DeleteTask(Task task)
        {
            try
            {
                Container.TaskSet.DeleteObject(task);
                Container.SaveChanges();
            }
            catch (Exception ex)
            {
                SendMailToDeveloper("(ExceptionID=12)", ex.Message, ex.StackTrace);
                throw new Exception("Unable to delete task" + "<br/>" +
                                    "A message has been sent to the developer. Please try again.");
            }
        }

        /// <summary>
        /// Saves the data about the job the employee just ended in SQL server.
        /// </summary>
        /// <param name="project">Where the employee were</param>
        /// <param name="type">What he was doing</param>
        /// <param name="start">When he started</param>
        /// <param name="end">When he ended</param>
        /// <param name="user">The employee himself</param>
        /// <returns>
        /// If the task was saved in SQL server
        /// </returns>
        public bool SaveTask(EconomicProject project, EconomicTaskType type, DateTime start, DateTime end, User user)
        {
            var currentDailyReport = (user.DailyReport).LastOrDefault();

            if (currentDailyReport is DailyReport)
            {

                DailyReport dr = currentDailyReport as DailyReport;

                Task task = new Task();
                task.EconomicProject = project;
                task.EconomicTask = type;
                task.TimeStarted = start;
                task.TimeEnded = end;

                dr.Task.Add(task);
                Container.SaveChanges();

                return true;
            }
            else
            {
                return false;
            }

        }

        //--------------------------------------------------------------UPLOAD --------------------------------------------------------------------------------
        // Finder ud af hvor mange timer der er brugt på dagen, noterer det i e-conomic (ex. 7.4 timer)
        //TODO UPLOAD
        public void UploadToEconomic(User u)
        {
            try
            {
                var user = u;
                foreach (var dailyReport in user.DailyReport.ToList())
                {
                    var tasks = dailyReport.Task.ToList();

                    foreach (var task in tasks)
                    {
                        UploadTasks(task, user);
                    }
                    //TODO
                    Container.BreakSet.DeleteObject(dailyReport.Break);
                    Container.DailyReportSet.DeleteObject(dailyReport);
                }

                Container.SaveChanges();
            }
            catch (Exception ex)
            {
                SendMailToDeveloper("(ExceptionID=15)", ex.Message, ex.StackTrace);
                throw new Exception("Unable to upload tasks" + "<br/>" +
                                    "A message has been sent to the developer. Please try again.");
            }
        }

        private void UploadTasks(Task task, User user)
        {
            var project = EconomicSession.Project.FindByNumber(task.EconomicProject.EconomicProjectId);
            var iActivity = EconomicSession.Activity.FindByNumber(task.EconomicTask.EconomicTaskTypeId);
            var iEmployee = EconomicSession.Employee.FindByNumber(user.EconomicUserId);
            var dateTime = task.TimeStarted.Value;
            var text = "";
            decimal numberOfHours = task.TimeEnded.Value.Hour - task.TimeStarted.Value.Hour;
            if (numberOfHours < 0)
            {
                numberOfHours += 24;
            }
            numberOfHours += ((decimal)task.TimeEnded.Value.Minute - task.TimeStarted.Value.Minute) / 60;

            EconomicSession.TimeEntry.Create(project, iActivity, iEmployee, dateTime, text, numberOfHours);

            Container.TaskSet.DeleteObject(task);
        }

        public List<ITimeEntry> Flashback(DateTime fromDate, DateTime toDate, User user)
        {
            try
            {
                var timeEntries = new List<ITimeEntry>();
                var i = fromDate;
                while (i <= toDate)
                {
                    var listOfTimeEntries =
                        EconomicSession.Employee.FindByNumber(user.EconomicUserId).GetTimeEntriesByDate(i, i);
                    if (listOfTimeEntries.Any())
                    {
                        foreach (var timeEntry in listOfTimeEntries)
                        {
                            timeEntries.Add(timeEntry);
                        }
                    }
                    i = i.AddDays(1);
                }
                return timeEntries;
            }
            catch (Exception ex)
            {
                SendMailToDeveloper("(ExceptionID=16)", ex.Message, ex.StackTrace);
                throw new Exception("Unable to get time entries from economic." + "<br/>" +
                                    "A message has been sent to the developer. Please try again.");
            }
        }

        public bool SendMail(Task task, User user)
        {
            var isEmailed = true;
            var debtor = EconomicSession.Project.FindByNumber(task.EconomicProject.EconomicProjectId).Debtor;
            if (debtor.Attention != null)
            {
                var email = debtor.Attention.Email;
                if (email != null)
                {
                    var attentionName = debtor.Attention.Name;
                    var station = debtor.Name;
                    var stringbuilder = new StringBuilder();
                    stringbuilder.AppendLine("Hej " + attentionName);
                    stringbuilder.AppendLine("<br/>");
                    stringbuilder.AppendLine("<br/>");
                    stringbuilder.AppendLine("Vi har d. " + task.TimeEnded.Value.ToShortDateString() +
                                             " været hos jer og udført følgende:");
                    stringbuilder.AppendLine("<br/>");
                    stringbuilder.AppendLine("<br/>");
                    stringbuilder.AppendLine("<table>");

                    stringbuilder.AppendLine("<tr>");
                    stringbuilder.AppendLine("<td>");
                    stringbuilder.AppendLine("<p>");
                    stringbuilder.AppendLine("Arbejdets art:");
                    stringbuilder.AppendLine("</p>");
                    stringbuilder.AppendLine("</td>");
                    stringbuilder.AppendLine("<td>");
                    stringbuilder.AppendLine("<p>");
                    stringbuilder.AppendLine(task.EconomicTask.Name);
                    stringbuilder.AppendLine("</p>");
                    stringbuilder.AppendLine("</td>");
                    stringbuilder.AppendLine("</tr>");

                    stringbuilder.AppendLine("<tr>");
                    stringbuilder.AppendLine("<td>");
                    stringbuilder.AppendLine("<p>");
                    stringbuilder.AppendLine("Arbejdets start:");
                    stringbuilder.AppendLine("</p>");
                    stringbuilder.AppendLine("</td>");
                    stringbuilder.AppendLine("<td>");
                    stringbuilder.AppendLine("<p>");
                    stringbuilder.AppendLine(task.TimeStarted.Value.ToShortTimeString());
                    stringbuilder.AppendLine("</p>");
                    stringbuilder.AppendLine("</td>");
                    stringbuilder.AppendLine("</tr>");

                    stringbuilder.AppendLine("<tr>");
                    stringbuilder.AppendLine("<td>");
                    stringbuilder.AppendLine("<p>");
                    stringbuilder.AppendLine("Arbejdets slut:");
                    stringbuilder.AppendLine("</p>");
                    stringbuilder.AppendLine("</td>");
                    stringbuilder.AppendLine("<td>");
                    stringbuilder.AppendLine("<p>");
                    stringbuilder.AppendLine(task.TimeEnded.Value.ToShortTimeString());
                    stringbuilder.AppendLine("</p>");
                    stringbuilder.AppendLine("</td>");
                    stringbuilder.AppendLine("</tr>");

                    stringbuilder.AppendLine("<tr>");
                    stringbuilder.AppendLine("<td>");
                    stringbuilder.AppendLine("<p>");
                    stringbuilder.AppendLine("Tidsforbrug:");
                    stringbuilder.AppendLine("</p>");
                    stringbuilder.AppendLine("</td>");
                    stringbuilder.AppendLine("<td>");
                    stringbuilder.AppendLine("<p>");
                    var timeUsed = task.TimeEnded.Value - task.TimeStarted.Value;
                    stringbuilder.AppendLine(timeUsed.TotalMinutes.ToString() + " min.");
                    stringbuilder.AppendLine("</p>");
                    stringbuilder.AppendLine("</td>");
                    stringbuilder.AppendLine("</tr>");

                    stringbuilder.AppendLine("<tr>");
                    stringbuilder.AppendLine("<td>");
                    stringbuilder.AppendLine("<p>");
                    stringbuilder.AppendLine("Dato for udførelse:");
                    stringbuilder.AppendLine("</p>");
                    stringbuilder.AppendLine("</td>");
                    stringbuilder.AppendLine("<td>");
                    stringbuilder.AppendLine("<p>");
                    stringbuilder.AppendLine(task.TimeEnded.Value.ToShortDateString());
                    stringbuilder.AppendLine("</p>");
                    stringbuilder.AppendLine("</td>");
                    stringbuilder.AppendLine("</tr>");

                    stringbuilder.AppendLine("<tr>");
                    stringbuilder.AppendLine("<td>");
                    stringbuilder.AppendLine("<p>");
                    stringbuilder.AppendLine("Arbejdet udført af:");
                    stringbuilder.AppendLine("</p>");
                    stringbuilder.AppendLine("</td>");
                    stringbuilder.AppendLine("<td>");
                    stringbuilder.AppendLine("<p>");
                    stringbuilder.AppendLine(user.Username);
                    stringbuilder.AppendLine("</p>");
                    stringbuilder.AppendLine("</td>");
                    stringbuilder.AppendLine("</tr>");

                    stringbuilder.AppendLine("</table>");
                    stringbuilder.AppendLine("<br/>");
                    stringbuilder.AppendLine("Ha' en rigtig god dag.");
                    stringbuilder.AppendLine("<br/>");
                    stringbuilder.AppendLine("<br/>");
                    stringbuilder.AppendLine("Med venlig hilsen");
                    stringbuilder.AppendLine("<br/>");
                    stringbuilder.AppendLine("BG anlægsteknik");
                    stringbuilder.AppendLine("<br/>");
                    stringbuilder.AppendLine("Kileparken 24");
                    stringbuilder.AppendLine("<br/>");
                    stringbuilder.AppendLine("8381 Tilst");
                    stringbuilder.AppendLine("<br/>");
                    stringbuilder.AppendLine("<br/>");
                    stringbuilder.AppendLine("Tlf: 2484 4203");
                    stringbuilder.AppendLine("<br/>");
                    stringbuilder.AppendLine("www.bganlaegsteknik.dk");
                    stringbuilder.AppendLine("<br/>");
                    stringbuilder.AppendLine("Vedligehold vinter/sommer");
                    stringbuilder.AppendLine("<br/>");
                    stringbuilder.AppendLine("Belægning");
                    stringbuilder.AppendLine("<br/>");

                    MailMessage mailMessage;
                    mailMessage = new MailMessage();
                    mailMessage.From = new MailAddress("bogholderi@bganlaegsteknik.dk");
                    //mailMessage.From = new MailAddress("test@bganlaegsteknikapp.dk");
                    mailMessage.To.Add(new MailAddress(email));
                    //mailMessage.To.Add(new MailAddress("hosbjerg1@gmail.com"));
                    mailMessage.To.Add(new MailAddress("info@bganlaegsteknik.dk"));
                    mailMessage.Subject = station;
                    mailMessage.Body = stringbuilder.ToString();
                    mailMessage.IsBodyHtml = true;
                    mailMessage.Priority = MailPriority.Normal;

                    SmtpClient smtpClient = new SmtpClient();
                    smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtpClient.Host = "mail.bganlaegsteknik.dk";
                    smtpClient.Port = 587;
                    smtpClient.UseDefaultCredentials = false;
                    NetworkCredential myCredential = new NetworkCredential("bogholderi@bganlaegsteknik.dk",
                        "solveig1234");
                    //NetworkCredential myCredential = new NetworkCredential("test@bganlaegsteknikapp.dk", "bgtest");
                    smtpClient.Credentials = myCredential;
                    smtpClient.Send(mailMessage);
                    isEmailed = true;
                }
                else
                {
                    isEmailed = false;
                    throw new ArgumentNullException("There is no email associated to the attention, it will not be emailed." + "<br/>" + "Contact Søren.");
                }
            }
            else
            {
                isEmailed = false;
                throw new ArgumentNullException("There is no debtor associated to the project, it will not be emailed." + "<br/>" + "Contact Søren.");
            }
            return isEmailed;
        }
        private void SendMailToDeveloper(string number, string message, string stackTrace)
        {
            MailMessage mailMessage;
            mailMessage = new MailMessage();
            mailMessage.From = new MailAddress("bogholderi@bganlaegsteknik.dk");
            mailMessage.To.Add("hosbjerg1@gmail.com");
            mailMessage.Subject = "Exception from BG web-app (" + number + ")";
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("Exception message");
            builder.AppendLine(message);
            builder.AppendLine("/n");
            builder.AppendLine("Exception stacktrace");
            builder.AppendLine(stackTrace);
            mailMessage.Body = builder.ToString();
            mailMessage.Priority = MailPriority.Normal;

            SmtpClient smtpClient = new SmtpClient();
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtpClient.Host = "mail.bganlaegsteknik.dk";
            smtpClient.Port = 587;

            NetworkCredential myCredential = new NetworkCredential("bogholderi@bganlaegsteknik.dk", "solveig1234");
            smtpClient.Credentials = myCredential;

            smtpClient.Send(mailMessage);
        }

        public void DeleteOldRoutines(User user)
        {
            var deleteRoutines = new List<Routine>();
            foreach (var routine in user.Routines.OrderBy(r => r.Id))
            {
                var i = routine.Dates.Where(d => d.TheDate < DateTime.Now).Count();
                if (i == 2)
                {
                    deleteRoutines.Add(routine);
                }
                else
                {
                    var found = false;
                    foreach (var activity in routine.Activities)
                    {
                        if (activity.IsUsed == false)
                        {
                            found = true;
                            break;
                        }
                    }
                    if (found == false)
                    {
                        deleteRoutines.Add(routine);
                    }
                }
            }
            foreach (var deleteRoutine in deleteRoutines)
            {
                Container.DeleteObject(GetRoutineFromCacheById(deleteRoutine.Id));
                Container.SaveChanges();
            }


        }

        //---------------------------------------------------------------------Extra ------------------------------------------------------------------------

        public List<int> GetHours()
        {
            List<int> hours = new List<int>();
            for (int i = 0; i < 24; i++)
            {
                hours.Add(i);
            }

            return hours;
        }

        public List<int> GetMinutes()
        {
            List<int> minutes = new List<int>();
            for (int i = 0; i < 60; i += 5)
            {
                minutes.Add(i);
            }
            return minutes;
        }

        //public DateTime? newt { get; set; }
    }
}
