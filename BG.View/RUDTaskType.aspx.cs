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
    public partial class RUDTaskType : System.Web.UI.Page
    {
        private User user;
        protected List<EconomicProject> economicProjects;
        private Task task;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["LoggedInUser"] != null)
            {
                user = (User)Session["LoggedInUser"];
            }
            else
            {
                Response.Redirect("Default.aspx");
            }
            var taskId = -1;
            if (Request["taskID"] != null)
            {
                taskId = Convert.ToInt32(Request["taskID"]);
            }

            if (taskId > -1)
            {
                economicProjects = new List<EconomicProject>();
                foreach (var g in user.Group)
                {
                    foreach (var p in Service.Instance.GetAllEconomicProjectsFromCache())
                    {
                        if (p.Group.Id == g.Id)
                        {
                            economicProjects.Add(p);
                        }
                    }
                }
                task = Service.Instance.GetTasksForUser(user).FirstOrDefault(t => t.Id == taskId);

                if (task.isEmailed)
                {
                    ButtonEmail.Enabled = false;
                }
                else
                {
                    ButtonEmail.Enabled = true;
                }

                if (!IsPostBack)
                {

                    if (Request["taskID"] != null)
                    {
                        PopulateDropdownListProjects(economicProjects.OrderBy(p => p.Name).ToList());
                        listboxProjects.SelectedValue = task.EconomicProject.EconomicProjectId + "";

                        PopulateDropdownListTaskTypes(task.EconomicProject);
                        listboxTaskTypes.SelectedValue = task.EconomicTask.EconomicTaskTypeId + "";
                        LabelSelectedProject.Text = task.EconomicProject.Name;
                    }
                    SetTimeOnStartAndEnd(task.TimeStarted.Value, task.TimeEnded.Value);
                }
                LabelTasktypeStartTime.Text = "Current: " + task.TimeStarted.Value.ToString();
                LabelTasktypeEndTime.Text = "Current: " + task.TimeEnded.Value.ToString();
            }
        }

        private void PopulateDropdownListProjects(List<EconomicProject> economicProjects)
        {
            listboxProjects.DataSource = economicProjects;
            listboxProjects.DataValueField = "EconomicProjectId";
            listboxProjects.DataTextField = "Name";
            listboxProjects.DataBind();
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
            //listboxTaskTypes.SelectedIndex = 0;
        }

        protected void ButtonUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                if (listboxProjects.SelectedIndex >= 0)
                {
                    if (listboxTaskTypes.SelectedIndex > 0)
                    {
                        var dailyReport = user.DailyReport.First();
                        var updateTask = task;
                        var newProject =
                            Service.Instance.GetEconomicProjectFromCache(Convert.ToInt32(listboxProjects.SelectedValue));
                        var newEconomicTaskType =
                            Service.Instance.GetEconomicTaskTypeFromCache(Convert.ToInt32(listboxTaskTypes.SelectedValue));
                        if (IsValidTask(dailyReport,newProject,newEconomicTaskType))
                        {
                            var timeStarted = new DateTime(dailyReport.DayStarted.Value.Year,
                                dailyReport.DayStarted.Value.Month,
                                dailyReport.DayStarted.Value.Day,
                                Convert.ToInt32(listboxTimeStartHour.SelectedValue),
                                Convert.ToInt32(listboxTimeStartMin.SelectedValue), 0);

                            var timeEnded = new DateTime(dailyReport.DayStarted.Value.Year,
                                dailyReport.DayStarted.Value.Month,
                                dailyReport.DayStarted.Value.Day,
                                Convert.ToInt32(listboxTimeEndHour.SelectedValue),
                                Convert.ToInt32(listboxTimeEndMin.SelectedValue), 0);

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
                                    Service.Instance.UpdateTask(newProject, newEconomicTaskType, updateTask, dailyReport,
                                        timeStarted, timeEnded, CheckBoxMove.Checked);
                                    Session["taskID"] = null;
                                    Response.Redirect("Home.aspx");
                                }
                                else if (newEnd <= newStart)
                                {
                                    labelErrorText.Text = "Endtime can not be less than starttime." + "<br/>";
                                }
                            }
                            else
                            {
                                if (timeEnded <= timeStarted)
                                {
                                    labelErrorText.Text = "Endtime can not be less than or equal to starttime." + "<br/>";
                                }
                                else
                                {
                                    Service.Instance.UpdateTask(newProject, newEconomicTaskType, updateTask, dailyReport,
                                        timeStarted, timeEnded, CheckBoxMove.Checked);
                                    Session["taskID"] = null;
                                    Response.Redirect("Home.aspx");
                                }
                            }
                        }
                    }
                    else
                    {
                        labelErrorText.Text = "Remember to choose task type." + "<br/>";
                    }
                }
                else
                {
                    labelErrorText.Text = "Remember to choose a project." + "<br/>";
                }
            }
            catch (Exception ex)
            {
                labelErrorText.Text = ex.Message;
            }
        }

        private bool IsValidTask(DailyReport dailyReport, EconomicProject newProject, EconomicTaskType newEconomicTaskType)
        {
            var isValid = true;
            if (dailyReport == null)
            {
                isValid = false;
                throw new ArgumentNullException("Unable to get daily report." + "<br/>");
            }
            if (newProject == null)
            {
                isValid = false;
                throw new ArgumentNullException("Unable to get project." + "<br/>");
            }
            if (newEconomicTaskType == null)
            {
                isValid = false;
                throw new ArgumentNullException("Unable to get task type." + "<br/>");
            }
            
            return isValid;
        }

        protected void CheckboxPassesMidnightOnCheckedChanged(object sender, EventArgs args)
        {
            var checkBox = sender as CheckBox;
            if (checkBox.Checked)
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
                DropdownListStartBeforeMidnight.Visible = false;
                DropdownListEndAfterMidnight.Visible = false;
                LabelExample.Visible = false;
            }
        }

        protected void ListboxProjectsSelectedIndexChanged(object sender, EventArgs e)
        {
            var dropdownProjects = sender as DropDownList;
            if (dropdownProjects.SelectedIndex > 0)
            {
                var project = Service.Instance.GetEconomicProjectFromCache(Convert.ToInt32(dropdownProjects.SelectedValue));
                LabelSelectedProject.Text = project.Name;
                listboxProjects.SelectedValue = project.EconomicProjectId + "";
                PopulateDropdownListTaskTypes(project);
                LabelSelectedProject.Text = project.Name;
            }
        }


        private void SetTimeOnStartAndEnd(DateTime startTime, DateTime endTime)
        {
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

            listboxTimeStartHour.SelectedValue = startTime.Hour.ToString();
            listboxTimeStartMin.SelectedValue = startTime.Minute.ToString();
            listboxTimeEndHour.SelectedValue = endTime.Hour.ToString();
            listboxTimeEndMin.SelectedValue = endTime.Minute.ToString();

            var dailyReport = user.DailyReport.OrderBy(d => d.DayStarted).First();
            var dayStarted = dailyReport.DayStarted;
            DropdownListStartBeforeMidnight.Items.Clear();
            DropdownListEndAfterMidnight.Items.Clear();
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

        protected void ButtonDelete_OnClick(object sender, EventArgs e)
        {
            try
            {
                Service.Instance.DeleteTask(task);
                Session["taskID"] = null;
                Response.Redirect("Home.aspx");
            }
            catch (Exception ex)
            {
                labelErrorText.Text = ex.Message;
            }
            
        }

        protected void ButtonCancel_Click(object sender, EventArgs e)
        {
            Session["taskID"] = null;
            Response.Redirect("Home.aspx");
        }

        protected void ButtonEmail_Click(object sender, EventArgs e)
        {
            try
            {
                var isMailed=Service.Instance.SendMail(task, user);
                if (isMailed)
                {
                    task.isEmailed = true;
                    Service.Instance.Container.SaveChanges();
                    ButtonEmail.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                labelErrorText.Text = ex.Message;
            }
            
        }
    }
}