using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BG.Controller;
using BG.Model;


namespace BG.View
{
    public partial class NewTaskType : System.Web.UI.Page
    {
        protected IEnumerable<EconomicProject> economicProjects;
        protected string selectedTime;
        protected List<DateTime> dates;
        private User currentUser;
        private int projectID;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (Session["LoggedInUser"] != null)
                {
                    currentUser = (User) Session["LoggedInUser"];
                }
                else
                {
                    Response.Redirect("Default.aspx");
                }
                if (Request["projectID"] != null)
                {
                    projectID = Convert.ToInt32(Request["projectID"]);
                }
                else
                {
                    projectID = -1;
                }
                if (Session["dates"] != null)
                {
                    dates = Session["dates"] as List<DateTime>;
                }

                economicProjects =
                    Service.Instance.GetAllEconomicProjectsFromCache()
                        .Where(p => currentUser.Group.Any(g => g.Id == p.Group.Id))
                        .OrderBy(p => p.Name);
                if (!IsPostBack)
                {
                    if (projectID > -1)
                    {
                        PopulateDropdownListTaskTypes(
                            Service.Instance.GetAllEconomicProjectsFromCache().FirstOrDefault(ec => ec.Id == projectID));

                    }

                    InitiateDropdowns();

                    var time = Request["time"];
                    if (string.IsNullOrEmpty(time))
                    {
                        SetDefaultTime();
                    }
                    else
                    {
                        selectedTime = time;
                        dates = new List<DateTime>();
                        string[] times = time.Split('t');
                        dates.Add(Convert.ToDateTime(times[0]));
                        listboxTimeStartHour.SelectedValue = times[1];
                        listboxTimeStartMin.SelectedValue = times[2];
                        dates.Add(Convert.ToDateTime(times[3]));
                        listboxTimeEndHour.SelectedValue = times[4];
                        listboxTimeEndMin.SelectedValue = times[5];
                        Session["dates"] = dates;
                        var start = new DateTime();
                        start = dates[0];
                        start = start.AddHours(Convert.ToInt32(times[1]));
                        start = start.AddMinutes(Convert.ToInt32(times[2]));

                        var end = new DateTime();
                        end = dates[1];
                        end = end.AddHours(Convert.ToInt32(times[4]));
                        end = end.AddMinutes(Convert.ToInt32(times[5]));
                        LabelSuggestedStarttime.Text += "<br/>" + start.ToString();
                        LabelSuggestedEndtime.Text += "<br/>" + end.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                labelErrorText.Text = ex.Message;
            }
        }

        private void PopulateDropdownListTaskTypes(EconomicProject selectedProject)
        {
            var taskTypes = new List<EconomicTaskType>();
            foreach (var taskType in selectedProject.EconomicTaskTypes)
            {
                if (selectedProject.Group.Id == 1)
                {
                    if (taskType.EconomicTaskTypeId > 100 && taskType.EconomicTaskTypeId < 201)
                    {
                        if (taskTypes.Contains(taskType) == false && taskTypes.Any(t => t.EconomicTaskTypeId == taskType.EconomicTaskTypeId) == false)
                        {
                            taskTypes.Add(taskType);
                        }
                    }
                }
                else if (selectedProject.Group.Id == 2)
                {
                    if (taskType.EconomicTaskTypeId > 200 && taskType.EconomicTaskTypeId < 301)
                    {
                        if (taskTypes.Contains(taskType) == false && taskTypes.Any(t => t.EconomicTaskTypeId == taskType.EconomicTaskTypeId) == false)
                        {
                            taskTypes.Add(taskType);
                        }
                    }
                }
                else if (selectedProject.Group.Id == 3)
                {
                    if (taskType.EconomicTaskTypeId > 300 && taskType.EconomicTaskTypeId < 401)
                    {
                        if (taskTypes.Contains(taskType) == false && taskTypes.Any(t => t.EconomicTaskTypeId == taskType.EconomicTaskTypeId) == false)
                        {
                            taskTypes.Add(taskType);
                        }
                    }
                }
            }
            listboxTaskTypes.DataSource = taskTypes;
            listboxTaskTypes.DataValueField = "EconomicTaskTypeId";
            listboxTaskTypes.DataTextField = "Name";
            listboxTaskTypes.DataBind();
            listboxTaskTypes.Items.Insert(0, new ListItem("Select a task type"));
            listboxTaskTypes.SelectedIndex = 0;
            labelTaskType.Text+="("+selectedProject.Name+")";
        }      

        private bool TimeEnteredIsValid(DailyReport dailyReport, DateTime taskStart, DateTime taskEnd)
        {
            bool isValid = true;
            labelErrorText.Text = string.Empty;
            if ((taskStart.Hour == taskEnd.Hour && taskStart.Minute == taskEnd.Minute) && (dailyReport.DayStarted == taskStart && dailyReport.DayEnded == taskEnd))
            {
                isValid = false;
                labelErrorText.Text += "Start time cannot be equal to endtime<br/>";
            }
            if (taskStart < dailyReport.DayStarted)
            {
                isValid = false;
                labelErrorText.Text += "Start time is before day start<br/>Daystarted: " + dailyReport.DayStarted.Value.ToShortTimeString() + "<br/>";
            }
            if (taskEnd < taskStart)
            {
                isValid = false;
                labelErrorText.Text += "End time can not be earlier than start time <br/>";
            }
            if (dailyReport.DayEnded.HasValue)
            {
                if (taskEnd > dailyReport.DayEnded.Value)
                {
                    isValid = false;
                    labelErrorText.Text += "End time is after day end<br/>Dayend: " + dailyReport.DayEnded.Value.ToShortTimeString();
                }
            }
            if (dailyReport.Break.BreakStarted <= taskStart && dailyReport.Break.BreakEnded >= taskEnd)
            {
                isValid = false;
                labelErrorText.Text += "Unable to use the break as task <br/>";
            }
            return isValid;
        }

        private bool IsValidObjects(EconomicProject economicProject, EconomicTaskType economicTaskType,
            DailyReport dailyReport)
        {
            var isValid = true;
            var text = string.Empty;
            if (economicProject == null)
            {
                isValid = false;
                text += "Unable to find economic project."+"<br/>";
            }
            if (economicTaskType == null)
            {
                isValid = false;
                text += "Unable to find economic task type."+"<br/>";
            }
            if (dailyReport == null)
            {
                isValid = false;
                text += "Unable to find daily report."+"<br/>";
            }
            if (isValid == false)
            {
                labelErrorText.Text = text;
            }
            return isValid;
        }
        protected void ButtonOk_Click(object sender, EventArgs e)
        {
            try
            {
                if (projectID > -1)
                {
                    if (listboxTaskTypes.SelectedIndex > 0)
                    {
                        var economicProject =
                            Service.Instance.GetAllEconomicProjectsFromCache()
                                .FirstOrDefault(p => p.Id == Convert.ToInt32(projectID));

                        var economicTaskType =
                            Service.Instance.GetEconomicTaskTypeFromCache(
                                Convert.ToInt32(listboxTaskTypes.SelectedItem.Value));

                        var dailyReport = currentUser.DailyReport.OrderByDescending(d => d.DayStarted).FirstOrDefault();
                        if (IsValidObjects(economicProject, economicTaskType, dailyReport))
                        {
                            var dateStarted = dailyReport.DayStarted.Value;

                            var timeStarted = new DateTime(dateStarted.Year, dateStarted.Month, dateStarted.Day,
                                Convert.ToInt32(listboxTimeStartHour.SelectedValue),
                                Convert.ToInt32(listboxTimeStartMin.SelectedValue), 0);

                            var timeEnded = new DateTime(dateStarted.Year, dateStarted.Month, dateStarted.Day,
                                Convert.ToInt32(listboxTimeEndHour.SelectedValue),
                                Convert.ToInt32(listboxTimeEndMin.SelectedValue), 0);
                            if (dates != null)
                            {
                                if (dates.Any())
                                {
                                    timeStarted = dates[0];
                                    timeStarted =
                                        timeStarted.AddHours(Convert.ToInt32(listboxTimeStartHour.SelectedValue));
                                    timeStarted =
                                        timeStarted.AddMinutes(Convert.ToInt32(listboxTimeStartMin.SelectedValue));

                                    timeEnded = dates[1];
                                    timeEnded = timeEnded.AddHours(Convert.ToInt32(listboxTimeEndHour.SelectedValue));
                                    timeEnded = timeEnded.AddMinutes(Convert.ToInt32(listboxTimeEndMin.SelectedValue));
                                }
                            }
                            Session["dates"] = null;
                            // hvis dailyReport strækker sig over 2 dage (ex 22:00 - 06:00)
                            if (CheckboxPassesMidnight.Checked)
                            {
                                var newStart = Convert.ToDateTime(DropdownListStartBeforeMidnight.SelectedValue);
                                var newEnd = Convert.ToDateTime(DropdownListEndAfterMidnight.SelectedValue);
                                newStart = newStart.AddMinutes(Convert.ToInt32(listboxTimeStartMin.SelectedValue));
                                newStart = newStart.AddHours(Convert.ToInt32(listboxTimeStartHour.SelectedValue));
                                newEnd = newEnd.AddMinutes(Convert.ToInt32(listboxTimeEndMin.SelectedValue));
                                newEnd = newEnd.AddHours(Convert.ToInt32(listboxTimeEndHour.SelectedValue));
                                if (newEnd > newStart)
                                {
                                    timeStarted = newStart;
                                    timeEnded = newEnd;
                                }
                                else if (newEnd < newStart)
                                {
                                    labelErrorText.Text = "Endtime can not be less than starttime.";
                                }
                            }

                            if (TimeEnteredIsValid(dailyReport, timeStarted, timeEnded))
                            {
                                Service.Instance.CreateTask(timeStarted, timeEnded, dailyReport, economicProject,
                                    economicTaskType, CheckBoxMoveTasks.Checked);
                                Response.Redirect("Home.aspx");
                            }
                        }
                    }
                    else
                    {
                        labelErrorText.Text += "Remember to choose task type";
                    }
                }
                else
                {
                    labelErrorText.Text += "Rememeber to choose project";
                }
            }
            catch (Exception ex)
            {
                labelErrorText.Text += ex.Message;
            }
        }

        protected void ButtonCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("Home.aspx");
        }

        protected void CheckboxPassesMidnightOnCheckedChanged(object sender, EventArgs args)
        {
            var checkBox = sender as CheckBox;
            if(checkBox.Checked)
            {
                LabelNewEnd.Visible = true;
                LabelNewStart.Visible = true;
                DropdownListStartBeforeMidnight.Visible = true;
                DropdownListEndAfterMidnight.Visible = true;
                LabelExample.Visible = true;
            }
            else
            {
                LabelNewEnd.Visible = false;
                LabelNewStart.Visible = false;
                DropdownListStartBeforeMidnight.Visible =false;
                DropdownListEndAfterMidnight.Visible = false;
                LabelExample.Visible = false;
            }
        }

        private void SetDefaultTime()
        {
            var dailyReport = currentUser.DailyReport.OrderBy(d => d.DayStarted).First(); // Henter nyeste report.
            var count = dailyReport.Task.Count;
            if (count < 1)
            {
                var startTime = dailyReport.DayStarted.Value;
                listboxTimeStartHour.SelectedValue = startTime.Hour.ToString();
                listboxTimeStartMin.SelectedValue = startTime.Minute.ToString();
                LabelSuggestedStarttime.Text += "<br/>" + startTime.ToString();
                startTime = startTime.AddMinutes(30);
                listboxTimeEndHour.SelectedValue = startTime.Hour.ToString();
                listboxTimeEndMin.SelectedValue = startTime.Minute.ToString();
                LabelSuggestedEndtime.Text += "<br/>" + startTime.ToString();
            }
            else
            {
                var task = dailyReport.Task.OrderByDescending(d => d.TimeEnded).First(); //Henter den sidste task som er tilføjet.
                var nextTaskStartTime = task.TimeEnded.Value;

                listboxTimeStartHour.SelectedValue = nextTaskStartTime.Hour.ToString();
                listboxTimeStartMin.SelectedValue = nextTaskStartTime.Minute.ToString();
                LabelSuggestedStarttime.Text += "<br/>" + nextTaskStartTime.ToString();
                nextTaskStartTime = nextTaskStartTime.AddMinutes(30);
                listboxTimeEndHour.SelectedValue = nextTaskStartTime.Hour.ToString();
                listboxTimeEndMin.SelectedValue = nextTaskStartTime.Minute.ToString();
                LabelSuggestedEndtime.Text += "<br/>" + nextTaskStartTime.ToString();
            }
        }

        private void InitiateDropdowns()
        {
            var dailyReport = currentUser.DailyReport.OrderBy(d => d.DayStarted).First(); // Henter nyeste report.
            foreach (var hour in Service.Instance.GetHours())
            {
                string display = hour <= 9 ? "0" + hour.ToString() : hour.ToString();

                listboxTimeStartHour.Items.Add(new ListItem(display, hour.ToString()));
                listboxTimeEndHour.Items.Add(new ListItem(display, hour.ToString()));
            }

            foreach (var minute in Service.Instance.GetMinutes())
            {
                string display = minute <= 9 ? "0" + minute.ToString() : minute.ToString();

                listboxTimeStartMin.Items.Add(new ListItem(display, minute.ToString()));
                listboxTimeEndMin.Items.Add(new ListItem(display, minute.ToString()));
            }

            listboxTimeStartHour.Items.Insert(0, new ListItem("Select start hour"));
            listboxTimeEndHour.Items.Insert(0, new ListItem("Select end hour"));
            listboxTimeStartMin.Items.Insert(0, new ListItem("Select start minute"));
            listboxTimeEndMin.Items.Insert(0, new ListItem("Select end minute"));

            LabelPause.Text += "<br/>" + dailyReport.Break.BreakStarted.Value.ToShortTimeString() + "-" + dailyReport.Break.BreakEnded.Value.ToShortTimeString();

            var dayStarted = dailyReport.DayStarted;
            dayStarted = dayStarted.Value.AddDays(1);
            DropdownListStartBeforeMidnight.Items.Add(new ListItem(dayStarted.Value.ToShortDateString(), dayStarted.Value.ToShortDateString()));
            dayStarted = dayStarted.Value.AddDays(-1);
            DropdownListStartBeforeMidnight.Items.Add(new ListItem(dayStarted.Value.ToShortDateString(), dayStarted.Value.ToShortDateString()));
            dayStarted = dayStarted.Value.AddDays(-1);
            DropdownListStartBeforeMidnight.Items.Add(new ListItem(dayStarted.Value.ToShortDateString(), dayStarted.Value.ToShortDateString()));
            if (dailyReport.DayEnded.HasValue)
            {
                var dayEnded = dailyReport.DayEnded;
                dayEnded = dayEnded.Value.AddDays(1);
                DropdownListEndAfterMidnight.Items.Add(new ListItem(dayEnded.Value.ToShortDateString(),
                                                                    dayEnded.Value.ToShortDateString()));
                dayEnded = dayEnded.Value.AddDays(-1);
                DropdownListEndAfterMidnight.Items.Add(new ListItem(dayEnded.Value.ToShortDateString(),
                                                                    dayEnded.Value.ToShortDateString()));
                dayEnded = dayEnded.Value.AddDays(-1);
                DropdownListEndAfterMidnight.Items.Add(new ListItem(dayEnded.Value.ToShortDateString(),
                                                                    dayEnded.Value.ToShortDateString()));
            }
            else
            {
                var dayEnded = dailyReport.DayStarted;
                dayEnded = dayEnded.Value.AddDays(1);
                DropdownListEndAfterMidnight.Items.Add(new ListItem(dayEnded.Value.ToShortDateString(), dayEnded.Value.ToShortDateString()));
                dayEnded = dayEnded.Value.AddDays(-1);
                DropdownListEndAfterMidnight.Items.Add(new ListItem(dayEnded.Value.ToShortDateString(), dayEnded.Value.ToShortDateString()));
                dayEnded = dayEnded.Value.AddDays(-1);
                DropdownListEndAfterMidnight.Items.Add(new ListItem(dayEnded.Value.ToShortDateString(), dayEnded.Value.ToShortDateString()));
            }
        }
    }
}
