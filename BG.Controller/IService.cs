using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BG.Model;
using Economic.Api;

namespace BG.Controller
{
    public interface IService
    {

        //Users
        User CreateUserIfNotExists(string username, string password, int economicId, bool isAdmin);

        bool IsLocal { get; }

        bool UserExists(string username, string password);

        User GetUserFromCache(string username, string password);

        DailyReport GetUserCurrentDailyReport(User user);

        DailyReport GetDailyReport(User user);

        void SetCurrentDailyReportStartAndBreak(DateTime startDate ,DailyReport currentDailyReport);

        User GetUserFromCacheById(int id);

        void UpdateUser(int id, string newUsername, string newPassword, bool isAdmin, int economicId);

        void DeleteUser(int id);

        //Projects
        EconomicProject CreateEconomicProjectIfNotExists(int economicProjectId, string name, Group group);

        bool ProjectExists(int economicProjectId);

        EconomicProject GetEconomicProjectFromCache(int economicProjectId);

        void ExistsBreakToDailyReport(DailyReport currentDailyreport);

        //Group


        //TaskTypes
        EconomicTaskType CreateEconomicTaskTypeIfNotExists(int economicTaskTypeId, string name);

        bool TaskTypeExists(int economicTaskTypeId);

        EconomicTaskType GetEconomicTaskTypeFromCache(int economicTaskTypeId);

        void UpdateTasksAndEndTime(User user, int daysToMove);

        void CreateNewDailyReportForUser(User user);

        bool SaveTask(EconomicProject project, EconomicTaskType type, DateTime start, DateTime end, User user);

        void UpdateTask(EconomicProject project, EconomicTaskType taskType, Task updateTask, DailyReport dailyReport,
                        DateTime startTime, DateTime endTime, bool moveTasks);

        Task CreateTask(DateTime timeStarted, DateTime timeEnded, DailyReport dailyReport,
                                   EconomicProject economicProject, EconomicTaskType economicTaskType,bool moveTasks);

        void DeleteTask(Task task);

        //Routine

        IEnumerable<Route>GetRoutesFromCache();

        Activity CreateActivityToRoutine(EconomicProject project, EconomicTaskType taskType, double totalDuration);

        Activity GetActivityFromCacheById(int activityId);

        void DeleteActivity(int activityId);

        Activity UpdateActivityToRoutine(int activityId, EconomicProject project, EconomicTaskType taskType, double totalDuration);

        void CreateRoutineAndAssociateToEmployee(string routineName,Route route, List<User> employee, List<DateTime> dates, List<Activity> activities);

        Routine GetRoutineFromCacheById(int routineId);

        void RoutineAccepted(User user, Routine routine, Activity activity);

        IEnumerable<Routine> GetAllRoutinesFromCache();

        Date CreateDate(DateTime selectedDate);

        void ChangeDateInRoutine(int allRoutinesSelectedRoutineId, Date date, string text);

        void DeleteDateFromRoutine(int allRoutinesSelectedRoutineId, int dateId);

        Date GetDateFromCacheById(int dateId);

        void DeleteOldRoutines(User user);
        bool SendMail(Task task, User user);

        //Accessor til test
        ModelContainer Container { get; }

        //Accessor til test
        EconomicSession EconomicSession { get; }

        IEnumerable<EconomicProject> GetAllEconomicProjectsFromCache();

        IEnumerable<EconomicTaskType> GetAllEconomicTaskTypeFromCache();

        IEnumerable<EconomicTaskType> GetTaskTypesFromEconomic();

        IEnumerable<Group> GetAllGroupsFromCache();

        void RefreshCache();

        void RecreateModelContainer();

        IEnumerable<Task> GetTasksForUser(User user);

        void UpdateDailyReportTimes(User user, string date, int hour, int minute,
                                                          bool isEnd, bool moveTasks);
        
        void UploadToEconomic(User u);

        IEnumerable<User> GetUsersFromCache();
        bool UserIsMemberOfGroup(int groupId, int userId);

        void UpdateGroupUsers(int groupId, Dictionary<int, bool> dictionaryGroupUpdates);
        IEnumerable<EconomicTaskType> GetAllTasksFromSelectedProject(int projectId);

        bool UserHasDailyReport(User user);

        List<int> GetHours();

        List<int> GetMinutes();

        void ChangePauseUpDown(bool up, bool down, DailyReport dailyReport);

        int UpdateCache();
        int CleanCache();
        void CheckEconomicProjectNames();
        void UpdateGroup(int groupId, List<User> usersSignUp, List<User> usersSignOff);

        List<ITimeEntry> Flashback(DateTime fromDate, DateTime toDate, User user);
    }
}
