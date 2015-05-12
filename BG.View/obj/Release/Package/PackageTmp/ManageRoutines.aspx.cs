using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BG.Controller;
using BG.Model;

namespace BG.View
{
    public partial class ManageRoutines : System.Web.UI.Page
    {
        protected List<DateTime> dates;
        protected List<Activity> activities;
        protected int activityID;
        protected List<User> employees;
        protected string routineName;
        protected string pushed;
        protected User user;
        protected List<Activity> routineActivities;

        protected void Page_Load(object sender, EventArgs e)
        {
            user = (User)Session["LoggedInUser"];
            if (user == null) Response.Redirect("Default.aspx");

            if (Session["activities"] != null)
            {
                activities = Session["activities"] as List<Activity>;
            }
            else
            {
                activities = new List<Activity>();
            }
            if (Session["activityID"] != null)
            {
                activityID = Convert.ToInt32(Session["activityID"]);
            }

            if (!string.IsNullOrEmpty(Session["routineName"] as string))
            {
                routineName = Session["routineName"] as string;
                TextBoxRoutinename.Text = routineName;
            }
            else
            {
                routineName = string.Empty;
            }
            if (Session["dates"] != null)
            {
                dates = Session["dates"] as List<DateTime>;
                if (dates.Count == 1)
                {
                    LabelSelectedDateToRoutineFrom.Text = dates.ElementAt(0).ToShortDateString();
                }
                else if (dates.Count == 2)
                {
                    LabelSelectedDateToRoutineFrom.Text = dates.ElementAt(0).ToShortDateString();
                    LabelSelectedDateToRoutineTo.Text = dates.ElementAt(1).ToShortDateString();
                }
            }
            else
            {
                dates = new List<DateTime>();
            }

            if (Session["employees"] != null)
            {
                employees = Session["employees"] as List<User>;
            }
            else
            {
                employees = new List<User>();
            }

            if (Session["routineActivities"] != null)
            {
                routineActivities = Session["routineActivities"] as List<Activity>;
            }
            else
            {
                routineActivities = new List<Activity>();
            }
            LabelErrorRoutine.Text = string.Empty;
            LabelErrorAddEmployee.Text = string.Empty;
            LabelErrorEmployee.Text = string.Empty;
            LabelErrorGroup.Text = string.Empty;
            if (!IsPostBack)
            {
                PopulateAllGroups();
                PopulateEmployees();


                LabelUpdateCacheText.Text = "The update-button will update the cache with the newest projects from economic.";
                LabelCleanCacheText.Text = "The clean-button will delete projects from cache which" + "<br/>" + "are not active in economic anymore.";
                LabelCompareCacheText.Text = "The compare-button will compare each project in cache with" + "<br/>" + "projects from economic. Then change the name in cache to the correct.";
            }
            //PopulateGridview();
        }

        //-------------------------------------------------MANAGE EMPLOYEE -----------------------------------------------------------------
        //TODO MANAGE EMPLOYEE

        protected void DropdownlistAllEmployeesOnselectedIndexChanged(object sender, EventArgs args)
        {
            var dropdownlistAllEmployees = sender as DropDownList;
            if (dropdownlistAllEmployees.SelectedIndex > 0)
            {
                try
                {
                    var selectedUser =
                        Service.Instance.GetUserFromCacheById(Convert.ToInt32(dropdownlistAllEmployees.SelectedValue));
                    TextBoxUsername.Text = selectedUser.Username;
                    TextboxPassword.Text = selectedUser.Password;
                    CheckBoxIsAdmin.Checked = selectedUser.IsAdmin;
                    TextboxEconomicId.Text = selectedUser.EconomicUserId.ToString();

                    ButtonAcceptEmployee.Enabled = false;
                }
                catch (Exception ex)
                {
                    LabelErrorEmployee.Text = ex.Message;
                }
            }
        }

        private void PopulateEmployees()
        {
            DropdownlistAllEmployees.DataSource = Service.Instance.GetUsersFromCache().OrderBy(e => e.Username);
            DropdownlistAllEmployees.DataValueField = "Id";
            DropdownlistAllEmployees.DataTextField = "Username";
            DropdownlistAllEmployees.DataBind();
            DropdownlistAllEmployees.Items.Insert(0, new ListItem("Select an employee"));
        }

        protected void ButtonDeleteEmployeeClick(object sender, EventArgs args)
        {
            if (DropdownlistAllEmployees.SelectedIndex > 0)
            {
                try
                {
                    Service.Instance.DeleteUser(Convert.ToInt32(DropdownlistAllEmployees.SelectedValue));
                    ResetEmployeeField();
                    PopulateEmployees();
                    Session["employees"] = employees;
                }
                catch (Exception ex)
                {
                    LabelErrorEmployee.Text = ex.Message;
                }
            }
            else
            {
                LabelErrorEmployee.Text = "You are missing some properties to fullfill employee:" + "<br/>" +
                                          "Employee.";
            }
        }

        protected void ButtonUpdateEmployeeClick(object sender, EventArgs args)
        {
            if (isValidEmployee())
            {
                if (DropdownlistAllEmployees.SelectedIndex > 0)
                {
                    try
                    {
                        Service.Instance.UpdateUser(Convert.ToInt32(DropdownlistAllEmployees.SelectedValue),
                            TextBoxUsername.Text, TextboxPassword.Text,
                            CheckBoxIsAdmin.Checked, Convert.ToInt32(TextboxEconomicId.Text));
                        ResetEmployeeField();
                        PopulateEmployees();
                        DropdownlistAllEmployees.SelectedIndex = 0;
                    }
                    catch (Exception ex)
                    {
                        LabelErrorEmployee.Text = ex.Message;
                    }
                }
                else
                {
                    LabelErrorEmployee.Text += "Employee.";
                }
            }
        }

        protected void ButtonAccepEmployeeClick(object sender, EventArgs args)
        {
            if (isValidEmployee())
            {
                Service.Instance.CreateUserIfNotExists(TextBoxUsername.Text, TextboxPassword.Text,
                                                       Convert.ToInt32(TextboxEconomicId.Text), CheckBoxIsAdmin.Checked);
                ResetEmployeeField();
                PopulateEmployees();
                DropdownlistAllEmployees.SelectedIndex = 0;
                Session["employees"] = employees;
            }
            else
            {
                LabelErrorEmployee.Visible = true;
            }
        }

        private bool isValidEmployee()
        {
            var isValid = true;
            string text = "You are missing some properties to fullfill employee:" + "<br/>";
            if (TextBoxUsername.Text == string.Empty)
            {
                text += "Username." + "<br/>";
                isValid = false;
            }
            if (TextboxPassword.Text == string.Empty)
            {
                text += "Password." + "<br/>";
                isValid = false;
            }
            if (TextboxEconomicId.Text == string.Empty)
            {
                text += "EconomicID." + "<br/>";
                isValid = false;
            }
            if (isValid == false)
            {
                LabelErrorEmployee.Text = text;
            }
            return isValid;
        }

        protected void ButtonCancelEmployeeClick(object sender, EventArgs args)
        {
            ResetEmployeeField();
        }

        private void ResetEmployeeField()
        {
            ButtonAcceptEmployee.Enabled = true;
            ButtonUpdateEmployee.Enabled = true;
            ButtonDeleteEmployee.Enabled = true;
            TextBoxUsername.Text = string.Empty;
            TextboxPassword.Text = string.Empty;
            TextboxEconomicId.Text = string.Empty;
            CheckBoxIsAdmin.Checked = false;
        }

        //-------------------------------------------------------------------MANAGE GROUP -------------------------------------------------------------------
        //TODO MANAGE GROUP

        public void PopulateAllGroups()
        {
            DropdownlistAllGroups.DataSource =
                Service.Instance.GetAllGroupsFromCache().Where(g => g.Id >= 1 && g.Id <= 3).OrderBy(gr => gr.Name);
            DropdownlistAllGroups.DataValueField = "Id";
            DropdownlistAllGroups.DataTextField = "Name";
            DropdownlistAllGroups.DataBind();
            DropdownlistAllGroups.Items.Insert(0, new ListItem("Select a group"));
        }

        private void PopulateGridview()
        {
            if (DropdownlistAllGroups.SelectedIndex > 0)
            {
                var groupId = Convert.ToInt32(DropdownlistAllGroups.SelectedValue);
                var users = Service.Instance.GetUsersFromCache().Select((employee) => new
                {
                    EmployeeId = employee.Id,
                    Employee = employee.Username,
                    IsMember = Service.Instance.UserIsMemberOfGroup(groupId, employee.Id),
                }).ToList();
                //foreach (var user in Service.Instance.GetUsersFromCache())
                //{
                //    var checkbox = new CheckBox();
                //    checkbox.ID = user.Id + "";
                //    checkbox.Text = user.Username;
                //    if (user.Group.Any(g=>g.Id==groupId))
                //    {
                //        checkbox.Checked = true;
                //    }

                //    divCheckbox.Controls.Add(checkbox);
                //}
                GridviewGroupEmployee.DataSource = users;
                this.DataBind();
            }
        }

        protected void ButtonUpdate_Click(object sender, EventArgs e)
        {
            if (DropdownlistAllGroups.SelectedIndex > 0)
            {
                try
                {
                    Dictionary<int, bool> dictionaryGroupUpdates = new Dictionary<int, bool>();
                    for (int i = 0; i < GridviewGroupEmployee.Rows.Count; i++)
                    {
                        bool isMember =
                            ((CheckBox)GridviewGroupEmployee.Rows[i].Cells[1].FindControl("CheckBoxIsMember")).Checked;
                        int employeeId = Convert.ToInt32(GridviewGroupEmployee.DataKeys[i].Values["EmployeeId"]);

                        dictionaryGroupUpdates.Add(employeeId, isMember);
                    }
                    //var div = divCheckbox.Controls;
                    var groupId = Convert.ToInt32(DropdownlistAllGroups.SelectedValue);
                    //var usersSignUp = new List<User>();
                    //var usersSignOff = new List<User>();
                    //foreach (var control in divCheckbox.Controls)
                    //{
                    //    if (control is CheckBox)
                    //    {
                    //        var checkbox = control as CheckBox;
                    //        var userId = checkbox.ID;
                    //        if (checkbox.Checked)
                    //        {
                    //            usersSignUp.Add(Service.Instance.GetUserFromCacheById(Convert.ToInt32(userId)));
                    //        }
                    //        else
                    //        {
                    //            usersSignOff.Add(Service.Instance.GetUserFromCacheById(Convert.ToInt32(userId)));
                    //        }
                    //    }
                    //}
                    //Service.Instance.UpdateGroup(groupId, usersSignUp, usersSignOff);
                    Service.Instance.UpdateGroupUsers(groupId, dictionaryGroupUpdates);
                    DropdownlistAllGroups.SelectedIndex = 0;
                    GridviewGroupEmployee.Visible = false;
                    ButtonUpdate.Enabled = false;
                }
                catch (Exception ex)
                {
                    LabelErrorGroup.Text = ex.Message;
                }
            }
            else
            {
                LabelErrorGroup.Text = "Select a group";
            }
        }

        protected void DropdownlistAllGroups_SelectedIndexChanged(object sender, EventArgs e)
        {
            var dropDownList = sender as DropDownList;
            if (dropDownList.SelectedIndex > 0)
            {
                PopulateGridview();
                ButtonUpdate.Enabled = true;
            }
        }

        //--------------------------------------------------------------------MANAGE ROUTINE-------------------------------------------------------------
        //TODO ROUTINE
        private void PopulateDropdownListRoute()
        {
            DropdownlistRoutes.DataSource = Service.Instance.GetRoutesFromCache().OrderBy(r => r.Name).ToList();
            DropdownlistRoutes.DataValueField = "Id";
            DropdownlistRoutes.DataTextField = "Name";
            DropdownlistRoutes.DataBind();
            DropdownlistRoutes.Items.Insert(0, new ListItem("Select a route"));
            DropdownlistRoutes.SelectedIndex = 0;
        }

        private void PopulateDropdownListEmployees()
        {
            DropdownlistEmployeeRoutine.DataSource = Service.Instance.GetUsersFromCache().OrderBy(emp => emp.Username).ToList();
            DropdownlistEmployeeRoutine.DataValueField = "Id";
            DropdownlistEmployeeRoutine.DataTextField = "Username";
            DropdownlistEmployeeRoutine.DataBind();
            DropdownlistEmployeeRoutine.Items.Insert(0, new ListItem("Select an employee"));
            DropdownlistEmployeeRoutine.SelectedIndex = 0;
        }

        private void PopulateDropdownListProjects()
        {
            var groups = new List<Group>();
            if (employees.Any())
            {
                groups = employees.ElementAt(0).Group.ToList();

                var projects = new List<EconomicProject>();
                foreach (var g in groups)
                {
                    foreach (var project in Service.Instance.GetAllEconomicProjectsFromCache())
                    {
                        if (project.Group.Id == g.Id)
                        {
                            projects.Add(project);
                        }
                    }
                }
                DropdownlistAllProjects.DataSource = projects.OrderBy(p => p.Name).ToList();
                DropdownlistAllProjects.DataValueField = "EconomicProjectId";
                DropdownlistAllProjects.DataTextField = "Name";
                DropdownlistAllProjects.DataBind();
                DropdownlistAllProjects.Items.Insert(0, new ListItem("Select a project"));
                DropdownlistAllProjects.SelectedIndex = 0;
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
            DropdownlistTasks.DataSource = taskTypes.OrderBy(t => t.Name).ToList();
            DropdownlistTasks.DataValueField = "EconomicTaskTypeId";
            DropdownlistTasks.DataTextField = "Name";
            DropdownlistTasks.DataBind();
            DropdownlistTasks.Items.Insert(0, new ListItem("Select a task type"));
            DropdownlistTasks.SelectedIndex = 0;
            SetHoursAndMinutesAndDuplicate();
        }
        protected void TextboxRoutinenameTextChanged(object sender, EventArgs args)
        {
            var textbox = sender as TextBox;
            routineName = textbox.Text;
            Session["routineName"] = routineName;
        }

        protected void CalendarRoutineDatesSelectionChanged(object sender, EventArgs e)
        {
            var calendar = sender as Calendar;

            if (calendar.SelectedDate != null)
            {
                if (dates.Count == 1)
                {
                    if (calendar.SelectedDate < dates[0])
                    {
                        dates.Insert(0, calendar.SelectedDate);
                    }
                    else
                    {
                        dates.Insert(1, calendar.SelectedDate);
                    }
                    LabelSelectedDateToRoutineFrom.Text = dates.ElementAt(0).ToShortDateString();
                    LabelSelectedDateToRoutineTo.Text = dates.ElementAt(1).ToShortDateString();

                    PopulateDropdownListRoute();
                }
                else if (dates.Count == 0)
                {
                    dates.Add(calendar.SelectedDate);
                    LabelSelectedDateToRoutineFrom.Text = dates.ElementAt(0).ToShortDateString();
                }
                else if (dates.Count == 2)
                {
                    if ((calendar.SelectedDate > dates[0] || calendar.SelectedDate < dates[0]) && calendar.SelectedDate < dates[1])
                    {
                        dates.RemoveAt(0);
                        dates.Insert(0, calendar.SelectedDate);
                    }
                    else if (calendar.SelectedDate > dates[1])
                    {
                        dates.RemoveAt(1);
                        dates.Insert(1, calendar.SelectedDate);
                    }
                    LabelSelectedDateToRoutineFrom.Text = dates.ElementAt(0).ToShortDateString();
                    LabelSelectedDateToRoutineTo.Text = dates.ElementAt(1).ToShortDateString();

                    PopulateDropdownListRoute();
                }

            }

            Session["dates"] = dates;
        }
        protected void DropdownlistRoutes_SelectedIndexChanged(object sender, EventArgs e)
        {
            PopulateDropdownListEmployees();
        }

        protected void DropdownlistEmployeeOnSelectedIndexChanged(object sender, EventArgs args)
        {
            var dropdownEmployeeRoutine = sender as DropDownList;
            if (dropdownEmployeeRoutine.SelectedIndex > 0)
            {
                var currentAssociatedEmployees = employees;
                var tmpList = new List<User>();

                var employee = Service.Instance.GetUserFromCacheById(Convert.ToInt32(dropdownEmployeeRoutine.SelectedValue));
                if (currentAssociatedEmployees.Any())
                {
                    if (!currentAssociatedEmployees.Contains(employee))
                    {
                        foreach (var emp in currentAssociatedEmployees)
                        {
                            foreach (var g in emp.Group)
                            {
                                if (employee.Group.Contains(g) && !tmpList.Contains(employee))
                                {
                                    tmpList.Add(employee);
                                }
                            }
                        }
                    }
                }
                else
                {
                    tmpList.Add(employee);
                }
                for (int i = 0; i < tmpList.Count; i++)
                {
                    currentAssociatedEmployees.Add(tmpList.ElementAt(i));
                }
                DropdownlistEmployeeRoutine.SelectedIndex = 0;
                employees = currentAssociatedEmployees;
                Session["employees"] = employees;
                GridviewAddEmployee.DataSource = employees;
                GridviewAddEmployee.DataBind();
                PopulateDropdownListProjects();
            }
        }

        protected void GridviewAddEmployee_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            var gridview = sender as GridView;
            gridview.PageIndex = e.NewPageIndex;
            Session["employees"] = employees;
            GridviewAddEmployee.DataSource = employees;
            this.DataBind();
        }

        protected void GridviewAddEmployeeOnRowDeleting(object sender, GridViewDeleteEventArgs args)
        {
            var gridview = sender as GridView;
            int employeeId = Convert.ToInt32(gridview.DataKeys[args.RowIndex].Value);
            employees.Remove(Service.Instance.GetUserFromCacheById(employeeId));
            Session["employees"] = employees;
            GridviewAddEmployee.DataSource = employees;
            GridviewAddEmployee.DataBind();
            if (!employees.Any())
            {
                DropdownlistAllProjects.Items.Clear();
                DropdownlistHourAfterComma.Items.Clear();
                DropdownlistHourBeforeComma.Items.Clear();
                DropdownlistTasks.Items.Clear();
            }
        }

        protected void DropdownlistAllProjectsSelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedProjectId = sender as DropDownList;
            var selectedProject = Service.Instance.GetEconomicProjectFromCache(Convert.ToInt32(selectedProjectId.SelectedValue));
            PopulateDropdownListTaskTypes(selectedProject);
        }
        private bool IsValidActivity()
        {
            var isValid = true;
            string text = "You are missing some properties to fullfill an activity:" + "</br>";
            if (DropdownlistAllProjects.SelectedIndex == 0)
            {
                text += "Project." + "</br>";
                isValid = false;
            }
            if (DropdownlistTasks.SelectedIndex == 0)
            {
                text += "Task." + "</br>";
                isValid = false;
            }
            if (DropdownlistHourAfterComma.SelectedIndex == 0 && DropdownlistHourBeforeComma.SelectedIndex == 0)
            {
                text += "Hour is 0." + "</br>";
                isValid = false;
            }

            if (isValid == false)
            {
                LabelErrorRoutine.Text = text;
            }
            return isValid;
        }
        protected void ButtonAddActivityToRoutineList(object sender, EventArgs args)
        {
            if (IsValidActivity())
            {
                var project = Service.Instance.GetEconomicProjectFromCache(Convert.ToInt32(DropdownlistAllProjects.SelectedValue));
                var taskType = Service.Instance.GetEconomicTaskTypeFromCache(Convert.ToInt32(DropdownlistTasks.SelectedValue));
                var totalDuration = Convert.ToInt32(DropdownlistHourBeforeComma.SelectedValue) +
                                    Convert.ToInt32(DropdownlistHourAfterComma.SelectedValue);
                var activity = Service.Instance.CreateActivityToRoutine(project, taskType, totalDuration);
                activities.Add(activity);

                Session["activities"] = activities;
                GridviewTasksToThisRoutine.DataSource = activities;
                GridviewTasksToThisRoutine.DataBind();
                ResetTasksRoutine();
            }
        }

        protected void ButtonCancelActivityClick(object sender, EventArgs args)
        {
            ResetTasksRoutine();
            ButtonAddNewRoutineToList.Enabled = true;
            ButtonUpdateActivity.Enabled = false;
        }

        protected void ButtonUpdateActivityClick(object sender, EventArgs args)
        {
            if (IsValidActivity())
            {
                var project = Service.Instance.GetEconomicProjectFromCache(Convert.ToInt32(DropdownlistAllProjects.SelectedValue));
                var taskType = Service.Instance.GetEconomicTaskTypeFromCache(Convert.ToInt32(DropdownlistTasks.SelectedValue));
                var totalDuration = Convert.ToInt32(DropdownlistHourBeforeComma.SelectedValue) +
                                    Convert.ToInt32(DropdownlistHourAfterComma.SelectedValue);
                activities.Remove(Service.Instance.GetActivityFromCacheById(activityID));
                var activity = Service.Instance.UpdateActivityToRoutine(activityID, project, taskType, totalDuration);
                activities.Add(activity);
                Session["activities"] = activities;
                Session["activityID"] = null;
                GridviewTasksToThisRoutine.DataSource = activities;
                GridviewTasksToThisRoutine.DataBind();
                ResetTasksRoutine();
                ButtonAddNewRoutineToList.Enabled = true;
                ButtonUpdateActivity.Enabled = false;
            }
        }

        protected void GridviewActivitiesOnSelect(object sender, GridViewSelectEventArgs args)
        {
            var gridview = sender as GridView;
            activityID = Convert.ToInt32(gridview.DataKeys[args.NewSelectedIndex].Value);
            var activity = Service.Instance.GetActivityFromCacheById(activityID);
            DropdownlistAllProjects.SelectedValue = activity.EconomicProject.EconomicProjectId.ToString();
            DropdownlistTasks.SelectedValue = activity.EconomicTaskType.EconomicTaskTypeId.ToString();
            if (activity.Minutes >= 60)
            {
                var split = (activity.Minutes / 60).ToString().Split(',');
                var splitZero = Convert.ToDouble(split[0]);
                var hour = Convert.ToInt32(splitZero * 60).ToString();
                DropdownlistHourBeforeComma.SelectedValue = hour;
                if (split.Length > 1)
                {
                    var stringMin = "0," + split[1];
                    var minute = Convert.ToDouble(stringMin);
                    var calMinute = Convert.ToInt32(minute * 60).ToString();
                    DropdownlistHourAfterComma.SelectedValue = calMinute;
                }
                else
                {
                    DropdownlistHourAfterComma.SelectedIndex = 1;
                }
            }
            else
            {
                DropdownlistHourBeforeComma.SelectedIndex = 0;
                DropdownlistHourAfterComma.SelectedValue = activity.Minutes.ToString();
            }

            ButtonAddNewRoutineToList.Enabled = false;
            ButtonUpdateActivity.Enabled = true;
            Session["activityID"] = activityID;
        }

        protected void GridviewActivitiesOnDeleting(object sender, GridViewDeleteEventArgs args)
        {
            var gridview = sender as GridView;
            var activityId = Convert.ToInt32(gridview.DataKeys[args.RowIndex].Value);
            var activity = Service.Instance.GetActivityFromCacheById(activityId);
            activities.Remove(activity);
            Service.Instance.DeleteActivity(activityId);
            Session["activities"] = activities;
            GridviewTasksToThisRoutine.DataSource = activities;
            GridviewTasksToThisRoutine.DataBind();
        }

        private void ResetTasksRoutine()
        {
            try
            {
                Session["employees"] = employees;
                Session["dates"] = dates;
                Session["routineName"] = routineName;
                DropdownlistHourAfterComma.SelectedIndex = 0;
                DropdownlistHourBeforeComma.SelectedIndex = 0;
                DropdownlistTasks.SelectedIndex = 0;
                DropdownlistAllProjects.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                LabelErrorRoutine.Text = "Somethng went wrong";
            }
        }

        protected void ButtonCreateAndAssociateRoutineClick(object sender, EventArgs args)
        {
            if (IsValidRoutine())
            {
                var route = Service.Instance.GetRoutesFromCache().FirstOrDefault(r => r.Id == Convert.ToInt32(DropdownlistRoutes.SelectedValue));
                Service.Instance.CreateRoutineAndAssociateToEmployee(TextBoxRoutinename.Text, route, employees, dates, activities);
                DropdownlistEmployeeRoutine.Items.Clear();
                DropdownlistRoutes.Items.Clear();
                DropdownlistHourAfterComma.Items.Clear();
                DropdownlistHourBeforeComma.Items.Clear();
                DropdownlistTasks.Items.Clear();
                DropdownlistAllProjects.Items.Clear();
                dates.RemoveRange(0, dates.Count);
                activities.RemoveRange(0, activities.Count);
                employees.RemoveRange(0, employees.Count);
                TextBoxRoutinename.Text = string.Empty;
                LabelSelectedDateToRoutineFrom.Text = string.Empty;
                LabelSelectedDateToRoutineTo.Text = string.Empty;
                CalendarRoutineDates.SelectedDate = DateTime.Now.Date;
                GridviewAddEmployee.DataSource = employees;
                GridviewAddEmployee.DataBind();
                GridviewTasksToThisRoutine.DataSource = activities;
                GridviewTasksToThisRoutine.DataBind();
                Session["activities"] = null;
                Session["activityID"] = null;
                Session["dates"] = null;
                Session["routineName"] = null;
                Session["employees"] = null;
            }
            else
            {
                LabelErrorRoutine.Visible = true;
            }
        }

        private bool IsValidRoutine()
        {
            var isValid = true;
            string text = "You are missing some properties to fullfill the routine:" + "</br>";
            if (DropdownlistRoutes.SelectedIndex == 0)
            {
                text += "Route." + "</br>";
                isValid = false;
            }
            employees = Session["employees"] as List<User>;
            if (employees == null || !employees.Any())
            {
                text += "Employee." + "</br>";
                isValid = false;
            }
            if (routineName == null)
            {
                text += "Routinename." + "</br>";
                isValid = false;
            }
            if (dates == null || !dates.Any() || dates.Count < 2)
            {
                text += "Dates." + "</br>";
                isValid = false;
            }
            if (activities == null || !activities.Any())
            {
                text += "Activities." + "</br>";
                isValid = false;
            }
            if (isValid == false)
            {
                LabelErrorRoutine.Text = text;
            }
            return isValid;
        }

        protected void GridviewActivitiesPageIndexChanging(object sender, GridViewPageEventArgs args)
        {
            var gridview = sender as GridView;
            gridview.PageIndex = args.NewPageIndex;
            Session["employees"] = employees;
        }

        private void SetHoursAndMinutesAndDuplicate()
        {
            DropdownlistHourBeforeComma.Items.Clear();
            DropdownlistHourAfterComma.Items.Clear();
            foreach (var hour in Service.Instance.GetHours())
            {
                string display = hour <= 9 ? "0" + hour.ToString() : hour.ToString();

                DropdownlistHourBeforeComma.Items.Add(new ListItem(display + " hours", (hour * 60).ToString()));
            }

            foreach (var minute in Service.Instance.GetMinutes())
            {
                string display = minute <= 9 ? "0" + minute.ToString() : minute.ToString();

                DropdownlistHourAfterComma.Items.Add(new ListItem(display + " min (" + ((double)minute / (double)60).ToString("0.00") + " hours)", minute.ToString()));
            }
            DropdownlistHourBeforeComma.SelectedIndex = 0;
            DropdownlistHourAfterComma.SelectedIndex = 0;

        }

        //----------------------------------------------------------------------EXTRA ----------------------------------------------------------------
        protected void ButtonUpdateCacheClick(object sender, EventArgs args)
        {
            var affectedUpdate = Service.Instance.UpdateCache();
            LabelUpdateCache.Text = affectedUpdate + " has been added within the last 7 days." + "<br/>"
                                    + " Cache is now up to date.";
        }
        protected void ButtonCleanCache_Click(object sender, EventArgs e)
        {
            var affectedDelete = Service.Instance.CleanCache();
            LabelCleanCache.Text = affectedDelete + " has been deleted from cache.";
        }


        protected void ButtonHome_Click(object sender, EventArgs e)
        {
            Session["LoggedInUser"] = user;
            Session["dates"] = null;
            Session["allRoutinesSelectedRoutineId"] = null;
            Session["allRoutineDates"] = null;
            Session["routineName"] = null;
            Session["employees"] = null;
            Session["activityID"] = null;
            Session["activities"] = null;
            Session["route"] = null;

            Response.Redirect("Home.aspx");
        }

        protected void ButtonViewAllRoutines_Click(object sender, EventArgs e)
        {
            Session["LoggedInUser"] = user;
            Session["dates"] = null;
            Session["allRoutinesSelectedRoutineId"] = null;
            Session["allRoutineDates"] = null;
            Session["routineName"] = null;
            Session["employees"] = null;
            Session["activityID"] = null;
            Session["activities"] = null;
            Session["route"] = null;
            Response.Redirect("AllRoutines.aspx");
        }

        protected void ButtonCompare_Click(object sender, EventArgs e)
        {
            LabelCompareCache.Text = "This might take several minutes, please be patient";
            Service.Instance.CheckEconomicProjectNames();
            LabelCompareCache.Text = "Done";
        }


    }
}