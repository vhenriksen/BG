using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BG.Model;
using BG.Controller;

namespace BG.View
{
    public partial class Home : System.Web.UI.Page
    {

        //data used for the aspx markup files.
        protected DailyReport currentDailyreport;
        protected IEnumerable<Task> tasks;
        protected int taskCount;
        protected User currentUser;
        protected int routineId;
        private IService service;

        protected void Page_Load(object sender, EventArgs e)
        {

            currentUser = (User)Session["LoggedInUser"];
            service = Service.Instance;

            if (currentUser == null) Response.Redirect("Default.aspx");


            if (!currentUser.IsAdmin)
            {
                ButtonManageRutines.Visible = false;
            }
            LabelUsername.Text = currentUser.Username;
            tasks = service.GetTasksForUser(currentUser);
            taskCount = tasks.Count();
            currentDailyreport = service.GetUserCurrentDailyReport(currentUser);
            service.ExistsBreakToDailyReport(currentDailyreport);
            if (!IsPostBack)
            {
                if (currentUser.Routines.Any())
                {
                    var nextRoutine = new DateTime();
                    var found = false;
                    foreach (var routine in currentUser.Routines)
                    {
                        foreach (var date in routine.Dates)
                        {
                            if (date.TheDate.Date >= DateTime.Now.Date)
                            {
                                if (nextRoutine.Date < DateTime.Now.Date)
                                {
                                    nextRoutine = date.TheDate.Date;
                                }
                                var ticksDate = date.TheDate.Date.Ticks - DateTime.Now.Date.Ticks;
                                var nextRoutineTicks = nextRoutine.Date.Ticks - DateTime.Now.Date.Ticks;
                                if (ticksDate <= nextRoutineTicks)
                                {
                                    if (routine.Activities.Any(a => a.IsUsed == false))
                                    {
                                        nextRoutine = date.TheDate.Date;
                                        found = true;
                                    }
                                }
                            }
                        }
                    }
                    if (found)
                    {
                        ButtonRutines.Text += "(" + nextRoutine.ToString("d") + ")";
                    }
                }
            }

            if (currentDailyreport.DayStarted == null)
                {
                    ButtonDayEnd.Enabled = false;
                    ButtonNewTask.Enabled = false;
                    ButtonRutines.Enabled = false;
                }
                else
                {
                    ButtonDayStart.Text =
                        currentDailyreport.DayStarted.Value.ToString("d") + " " +
                        currentDailyreport.DayStarted.Value.ToString("t");
                }

                if (currentDailyreport.DayEnded.HasValue)
                {
                    ButtonDayEnd.Text = currentDailyreport.DayEnded.Value.ToString("d") + " " +
                                        currentDailyreport.DayEnded.Value.ToString("t");
                }


                if (currentDailyreport.DayStarted != null)
                {

                    if (currentDailyreport.Break.BreakStarted == null && currentDailyreport.Break.BreakEnded == null)
                    {
                        service.SetCurrentDailyReportStartAndBreak(currentDailyreport.DayStarted.Value,
                            currentDailyreport);
                    }

                    LabelPause.Text = "</br>" + currentDailyreport.Break.BreakStarted.Value.ToShortTimeString() + "-" +
                                      currentDailyreport.Break.BreakEnded.Value.ToShortTimeString() + " Pause" + "</br>";
                }
            
        }

        protected void ButtonPauseUpClick(object sender, EventArgs args)
        {
            var up = true;
            var down = false;
            if (currentDailyreport.DayStarted < currentDailyreport.Break.BreakStarted)
            {
                Service.Instance.ChangePauseUpDown(up, down, currentDailyreport);
            }
            Response.Redirect("Home.aspx");
        }
        protected void ButtonPauseDownClick(object sender, EventArgs args)
        {
            var up = false;
            var down = true;
            if (currentDailyreport.DayEnded.HasValue)
            {
                if (currentDailyreport.DayEnded > currentDailyreport.Break.BreakEnded)
                {
                    Service.Instance.ChangePauseUpDown(up, down, currentDailyreport);
                }
            }
            else
            {
                Service.Instance.ChangePauseUpDown(up, down, currentDailyreport);
            }
            Response.Redirect("Home.aspx");
        }

        protected void ButtonRutinesClick(object sender, EventArgs args)
        {
            Session["LoggedInUser"] = currentUser;
            Response.Redirect("Routines.aspx");
        }
        protected void ButtonManageRutinesClick(object sender, EventArgs args)
        {
            Response.Redirect("ManageRoutines.aspx");
        }

        protected void ButtonDayStartedClick(object sender, EventArgs e)
        {
            Session["daystartedbutton"] = true;
            Response.Redirect("DayStartedEnded.aspx");
        }

        protected void ButtonDayEndedClick(object sender, EventArgs e)
        {
            Session["daystartedbutton"] = false;
            Response.Redirect("DayStartedEnded.aspx");
        }

        protected void ButtonNewTaskClick(object sender, EventArgs e)
        {
            if (tasks.Any())
            {
                var lastTaskTypeId = tasks.OrderByDescending(x => x.TimeEnded).First().EconomicTask.Id;
                Session["lastTaskTypeId"] = lastTaskTypeId;
            }
            else
            {
                Session["lastTaskTypeId"] = -1;
            }

            Response.Redirect("NewTaskType.aspx");
        }
        protected void ButtonLogoutClick(object sender, EventArgs e)
        {
            Session["LoggedInUser"] = null;
            Response.Redirect("Default.aspx");
        }

        public string CalculateTimeSpent()
        {
            decimal timeSpent = 0;
            foreach (var task in currentUser.DailyReport.First().Task)
            {
                decimal numberOfHours = task.TimeEnded.Value.Hour - task.TimeStarted.Value.Hour;
                if (numberOfHours < 0)
                {
                    numberOfHours += 24;
                }
                numberOfHours += ((decimal)task.TimeEnded.Value.Minute - task.TimeStarted.Value.Minute) / 60;
                timeSpent += numberOfHours;
            }
            return timeSpent.ToString();
        }

        protected void ButtonWorkday_OnClick(object sender, EventArgs e)
        {
            Session["LoggedInUser"] = currentUser;
            Response.Redirect("workday.aspx");
        }
    }
}