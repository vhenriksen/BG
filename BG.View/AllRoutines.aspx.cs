using BG.Controller;
using BG.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BG.View
{
    public partial class AllRoutines : System.Web.UI.Page
    {
        protected List<Activity> activities;
        private List<Activity> currentActivities;
        protected List<Routine> allRoutines;
        private List<User> allEmployees;
        private List<User> associatedEmployees;
        private List<Group> groupsInRoutine;
        protected string pushed;
        protected User user;
        private int routineId;
        private int activityId;

        private SortDirection sortRoute;
        private SortDirection sortProject;
        private SortDirection sortTaken;

        protected void Page_Load(object sender, EventArgs e)
        {

            user = Session["LoggedInUser"] as User;

            if (Session["sortRoute"] != null)
            {
                sortRoute = (SortDirection)Session["sortRoute"];
            }
            else
            {
                sortRoute = SortDirection.Ascending;
            }
            if (Session["sortProject"] != null)
            {
                sortProject = (SortDirection)Session["sortProject"];
            }
            else
            {
                sortProject = SortDirection.Ascending;
            }
            if (Session["sortTaken"] != null)
            {
                sortTaken = (SortDirection)Session["sortTaken"];
            }
            else
            {
                sortTaken = SortDirection.Ascending;
            }
            if (Session["allTheRoutines"] != null)
            {
                allRoutines = Session["allTheRoutines"] as List<Routine>;
            }
            else
            {
                allRoutines = Service.Instance.GetAllRoutinesFromCache().ToList();

            }
            if (Session["allActivities"] != null)
            {
                activities = Session["allActivities"] as List<Activity>;
            }
            else
            {
                activities = new List<Activity>();
            }
            if (Session["groups"] != null)
            {
                groupsInRoutine = Session["groups"] as List<Group>;
            }
            else
            {
                groupsInRoutine = new List<Group>();
            }
            if (Session["currentActivities"] != null)
            {
                currentActivities = Session["currentActivities"] as List<Activity>;
            }
            else
            {
                currentActivities = new List<Activity>();
            }

            if (!string.IsNullOrEmpty(Session["pushed"] as string))
            {
                pushed = Session["pushed"] as string;
            }
            if (Session["routineId"] != null)
            {
                routineId = Convert.ToInt32(Session["routineId"]);
            }
            if (Session["allEmployees"] != null)
            {
                allEmployees = Session["allEmployees"] as List<User>;
            }
            else
            {
                allEmployees = new List<User>();
            }
            if (Session["associatedEmployees"] != null)
            {
                associatedEmployees = Session["associatedEmployees"] as List<User>;
            }
            else
            {
                associatedEmployees = new List<User>();
            }
            if (Session["activityId"] != null || Convert.ToInt32(Session["activityId"]) > -1)
            {
                activityId = Convert.ToInt32(Session["activityId"]);
            }
            else
            {
                activityId = -1;
            }
            LabelErrorActivity.Text = string.Empty;
            LabelAllRoutinesError.Text = string.Empty;
            LabelActivitiesError.Text = string.Empty;
            LabelCalendarError.Text = string.Empty;
            LabelEmployeesError.Text = string.Empty;
            if (!IsPostBack)
            {
                PopulateGridviewAllRoutines();
            }
        }
        private void UpdateGridviewRoutineAndSelectedRoutine()
        {
            var routine = Service.Instance.GetRoutineFromCacheById(routineId);
            LabelSelectedRoutineName.Text = "Routine: " + routine.Name;

            //Activities            
            activities = routine.Activities.ToList();
            PopulateGridviewActivities();
            //Associated employee(s)
            LabelSelectedRoutineAssociatedEmployees.Text = string.Empty;
            for (int i = 0; i < routine.Users.Count; i++)
            {
                if (i == routine.Users.Count - 1)
                {
                    LabelSelectedRoutineAssociatedEmployees.Text += routine.Users.ElementAt(i).Username;
                }
                else
                {
                    LabelSelectedRoutineAssociatedEmployees.Text += routine.Users.ElementAt(i).Username + ", ";
                }
            }

            //Dates
            var dates = routine.Dates.OrderBy(d => d.TheDate).ToList();
            LabelSelectedDatesFrom.Text = dates.ElementAt(0).TheDate.ToShortDateString();
            LabelSelectedDatesTo.Text = dates.ElementAt(1).TheDate.ToShortDateString();

            LabelSelectedRoutineFrom.Text = dates.ElementAt(0).TheDate.ToShortDateString();
            LabelSelectedRoutineTo.Text = dates.ElementAt(1).TheDate.ToShortDateString();

            //Reusable routine
            if (routine.Activities.Count(a => a.IsUsed) == routine.Activities.Count)
            {
                divReusableRoutineButton.Visible = true;
                ButtonSelectedRoutineReuse.Text = "Reuse (" + routine.Name + ")";
            }
            ShowDivActivities(true);
            PopulateGridviewAllRoutines();
            ButtonManageRoutine.Enabled = true;
        }

        protected void GridviewAllRoutines_OnPageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            var gridview = sender as GridView;
            gridview.PageIndex = e.NewPageIndex;
            PopulateGridviewAllRoutines();
        }

        protected void GridviewAllRoutinesOnSelectedChanging(object sender, GridViewSelectEventArgs args)
        {
            var gridview = sender as GridView;
            routineId = Convert.ToInt32(gridview.DataKeys[args.NewSelectedIndex].Values["Id"]);
            Session["routineId"] = routineId;
            UpdateGridviewRoutineAndSelectedRoutine();

        }
        protected void GridviewAllRoutinesOnRowDataBound(object sender, GridViewRowEventArgs args)
        {
            var gridview = sender as GridView;
            if (args.Row.RowType == DataControlRowType.DataRow)
            {
                var routineId = ((Routine)args.Row.DataItem).Id;
                var routine = Service.Instance.GetRoutineFromCacheById(Convert.ToInt32(routineId));
                if (routine != null)
                {
                    var label = (Label)args.Row.FindControl("LabelAllEmployeeToRoutine");
                    for (int i = 0; i < routine.Users.Count; i++)
                    {
                        if (i == routine.Users.Count - 1)
                        {
                            label.Text += routine.Users.ElementAt(i).Username;
                        }
                        else
                        {
                            label.Text += routine.Users.ElementAt(i).Username + ", ";
                        }
                    }
                }
            }
        }
        protected void GridviewAllRoutinesOnRowDeleting(object sender, GridViewDeleteEventArgs args)
        {
            var gridview = sender as GridView;
            var routineId = Convert.ToInt32(gridview.DataKeys[args.RowIndex].Values["Id"]);
            Service.Instance.Container.DeleteObject(Service.Instance.GetRoutineFromCacheById(routineId));
            Service.Instance.Container.SaveChanges();
            switch (sortRoute)
            {
                case SortDirection.Ascending:
                    allRoutines = Service.Instance.GetAllRoutinesFromCache().OrderByDescending(r => r.Route.Name).ToList();
                    sortRoute = SortDirection.Descending;
                    break;
                case SortDirection.Descending:
                    allRoutines = Service.Instance.GetAllRoutinesFromCache().OrderBy(r => r.Route.Name).ToList();
                    sortRoute = SortDirection.Ascending;
                    break;
            }
            Session["allTheRoutines"] = allRoutines;
            Session["sortRoute"] = sortRoute;
            ShowDivActivities(false);
            ButtonManageRoutine.Enabled = false;
            PopulateGridviewAllRoutines();
            divActivities.Visible = false;
        }
        protected void GridviewAllRoutines_Sorting(object sender, GridViewSortEventArgs e)
        {
            var gridview = sender as GridView;
            try
            {
                switch (e.SortExpression)
                {
                    case "Route":
                        if (sortRoute == SortDirection.Ascending)
                        {
                            allRoutines = Service.Instance.GetAllRoutinesFromCache().OrderByDescending(r => r.Route.Name).ToList();
                            GridviewAllRoutines.DataSource = allRoutines;
                            GridviewAllRoutines.DataBind();
                            sortRoute = SortDirection.Descending;
                        }
                        else if (sortRoute == SortDirection.Descending)
                        {
                            allRoutines = Service.Instance.GetAllRoutinesFromCache().OrderBy(r => r.Route.Name).ToList();
                            GridviewAllRoutines.DataSource = allRoutines;
                            GridviewAllRoutines.DataBind();
                            sortRoute = SortDirection.Ascending;
                        }
                        Session["sortRoute"] = sortRoute;
                        Session["allTheRoutines"] = allRoutines;
                        break;
                }
            }
            catch (Exception ex)
            {
                LabelAllRoutinesError.Text = "Something went wrong when sorting";
            }
        }
        protected void GridviewSelectedRoutineTasks_OnRowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var labelTaken = (Label)e.Row.FindControl("LabelSelectedRoutineTaskTaken");
                var labelTakeByEmployee = (Label)e.Row.FindControl("LabelSelectedRoutineTaskTakenBy");

                if (labelTaken.Text == "True")
                {
                    labelTaken.Text = "Taken";
                }
                else
                {
                    labelTaken.Text = "Not taken";
                }

                if (!string.IsNullOrEmpty(labelTakeByEmployee.Text))
                {
                    labelTakeByEmployee.Text = Service.Instance.GetUserFromCacheById(Convert.ToInt32(labelTakeByEmployee.Text)).Username;
                }
            }
        }

        protected void GridviewSelectedRoutineTasks_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            var gridview = sender as GridView;
            gridview.PageIndex = e.NewPageIndex;
            //PopulateGridviewActivities();
            UpdateGridviewRoutineAndSelectedRoutine();
        }

        protected void ButtonSelectedFrom_OnClick(object sender, EventArgs e)
        {
            pushed = "From";
            Session["pushed"] = pushed;
            divCalendar.Visible = true;
            divActivities.Visible = false;
            divManageEmployees.Visible = false;
            SetButtons(false);
            ButtonSelectedFrom.Text = "You are changing this now";
        }

        protected void ButtonSelectedTo_OnClick(object sender, EventArgs e)
        {
            pushed = "To";
            Session["pushed"] = pushed;
            divCalendar.Visible = true;
            divActivities.Visible = false;
            divManageEmployees.Visible = false;
            SetButtons(false);
            ButtonSelectedTo.Text = "You are changing this now";
        }
        protected void CalendarAllRoutines_SelectionChanged(object sender, EventArgs e)
        {
            var calendar = sender as Calendar;
            if (calendar.SelectedDate != null)
            {
                var date = Service.Instance.CreateDate(calendar.SelectedDate);
                Service.Instance.ChangeDateInRoutine(routineId, date, pushed);

                divCalendar.Visible = false;
                divActivities.Visible = false;
                divManageEmployees.Visible = false;
                SetButtons(true);
                ButtonSelectedFrom.Text = "Change";
                ButtonSelectedTo.Text = "Change";
                UpdateGridviewRoutineAndSelectedRoutine();
            }
        }
        protected void GridviewSelectedRoutineTasks_Sorting(object sender, GridViewSortEventArgs e)
        {
            try
            {
                var gridview = sender as GridView;
                switch (e.SortExpression)
                {
                    case "Project":
                        if (sortProject == SortDirection.Ascending)
                        {
                            activities = Service.Instance.GetAllRoutinesFromCache().FirstOrDefault(r => r.Id == routineId).Activities.OrderByDescending(a => a.EconomicProject.Name).ToList();
                            sortProject = SortDirection.Descending;
                        }
                        else if (sortProject == SortDirection.Descending)
                        {
                            activities = Service.Instance.GetAllRoutinesFromCache().FirstOrDefault(r => r.Id == routineId).Activities.OrderBy(a => a.EconomicProject.Name).ToList();
                            sortProject = SortDirection.Ascending;
                        }
                        Session["activities"] = activities;
                        Session["sortProject"] = sortProject;
                        PopulateGridviewActivities();
                        break;
                    case "Taken":
                        if (sortTaken == SortDirection.Ascending)
                        {
                            activities = Service.Instance.GetAllRoutinesFromCache().FirstOrDefault(r => r.Id == routineId).Activities.OrderByDescending(a => a.IsUsed).ToList();
                            sortTaken = SortDirection.Descending;
                        }
                        else if (sortTaken == SortDirection.Descending)
                        {
                            activities = Service.Instance.GetAllRoutinesFromCache().FirstOrDefault(r => r.Id == routineId).Activities.OrderBy(a => a.IsUsed).ToList();
                            sortTaken = SortDirection.Ascending;
                        }
                        Session["activities"] = activities;
                        Session["sortTaken"] = sortTaken;
                        PopulateGridviewActivities();
                        break;
                }
            }
            catch (Exception ex)
            {
                LabelActivitiesError.Visible = true;
                LabelActivitiesError.Text = "Something went wrong when sorting";
            }
        }
        protected void ButtonSelectedRoutineClose_Click(object sender, EventArgs e)
        {
            Session["activities"] = null;
            Session["pushed"] = null;
            Session["associatedEmployees"] = null;
            Session["allEmployees"] = null;
            PopulateGridviewAllRoutines();
            ShowDivActivities(false);
            ButtonManageRoutine.Enabled = false;
            divActivities.Visible = false;
            divManageEmployees.Visible = false;
        }

        protected void ButtonSelectedRoutineAcceptChanges_Click(object sender, EventArgs e)
        {
            Session["pushed"] = null;
            Session["associatedEmployees"] = null;
            Session["allEmployees"] = null;
            ButtonSelectedRoutineClose.Enabled = true;
            divManageSelectedRoutine.Visible = false;
            ButtonManageRoutine.Enabled = true;
            divActivities.Visible = false;
            divManageEmployees.Visible = false;
            UpdateGridviewRoutineAndSelectedRoutine();
        }
        protected void ButtonSelectedRoutineReuse_Click(object sender, EventArgs e)
        {
            var routine = Service.Instance.GetRoutineFromCacheById(routineId);
            foreach (var activity in routine.Activities.OrderBy(a => a.Id))
            {
                activity.ActivityTaken = null;
                foreach (var activityUser in activity.ActivityUser.OrderBy(au => au.Id))
                {
                    activityUser.Activity = null;
                }
                activity.ActivityTaken = null;
                activity.IsUsed = false;
            }
            Service.Instance.Container.SaveChanges();
            Session["pushed"] = null;
            Session["associatedEmployees"] = null;
            Session["allEmployees"] = null;
            UpdateGridviewRoutineAndSelectedRoutine();
            ButtonManageRoutine.Enabled = false;
            ShowDivActivities(false);
            divActivities.Visible = false;
            divManageEmployees.Visible = false;
        }
        protected void ButtonManageRoutine_Click(object sender, EventArgs e)
        {
            ButtonSelectedRoutineClose.Enabled = false;
            ButtonManageRoutine.Enabled = false;
            SetButtons(true);
            divActivities.Visible = true;
            divManageEmployees.Visible = true;
            divManageSelectedRoutine.Visible = true;
            DropdownlistPopulateAllEmployees();
            GridviewPopulateCurrentAssociatedEmployees();
            currentActivities = Service.Instance.GetRoutineFromCacheById(routineId).Activities.ToList();
            Session["currentActivities"] = currentActivities;
            GridviewPopulateCurrentActivities(currentActivities);
        }

        protected void ButtonBackToAdmin_Click(object sender, EventArgs e)
        {
            Session["LoggedInUser"] = user;
            Session["sortRoute"] = null;
            Session["sortProject"] = null;
            Session["sortTaken"] = null;
            Session["groups"] = null;
            Session["currentActivities"] = null;
            Session["allTheRoutines"] = null;
            Session["allActivities"] = null;
            Session["pushed"] = null;
            Session["routineId"] = null;
            Session["allEmployees"] = null;
            Session["associatedEmployees"] = null;
            Session["activityId"] = null;
            Response.Redirect("ManageRoutines.aspx", true);
        }

        private void SetButtons(bool isOn)
        {
            ButtonSelectedTo.Enabled = isOn;
            ButtonSelectedFrom.Enabled = isOn;
            ButtonSelectedRoutineOk.Enabled = isOn;
            ButtonSelectedRoutineReuse.Enabled = isOn;
        }

        private void PopulateGridviewActivities()
        {

            GridviewSelectedRoutineTasks.DataSource = activities;
            GridviewSelectedRoutineTasks.DataBind();
            Session["allActivities"] = activities;

        }
        private void PopulateGridviewAllRoutines()
        {
            GridviewAllRoutines.DataSource = allRoutines;
            GridviewAllRoutines.DataBind();
            Session["allTheRoutines"] = allRoutines;
        }

        private void ShowDivActivities(bool isOn)
        {
            divActivitiesToRoutine.Visible = isOn;
        }

        //---------------------------------------------------------------EMPLOYEE-----------------------------------------------------------------

        protected void DropdownlistAllEmployees_SelectedIndexChanged(object sender, EventArgs e)
        {
            var dropdownlist = sender as DropDownList;
            var currentAssociatedEmployees = associatedEmployees;
            var tmpList = new List<User>();
            if (dropdownlist.SelectedIndex > 0)
            {
                var selectedEmployee = Service.Instance.GetUserFromCacheById(Convert.ToInt32(dropdownlist.SelectedValue));
                if (currentAssociatedEmployees.Any() && !currentAssociatedEmployees.Contains(selectedEmployee))
                {
                    foreach (var ase in currentAssociatedEmployees)
                    {
                        foreach (var g in ase.Group)
                        {
                            if (selectedEmployee.Group.Contains(g) && !tmpList.Contains(selectedEmployee))
                            {
                                tmpList.Add(selectedEmployee);
                            }
                        }
                    }
                }
                for (int i = 0; i < tmpList.Count; i++)
                {
                    currentAssociatedEmployees.Add(tmpList.ElementAt(i));
                }
                var currentAssociatedEmployeesInCache = Service.Instance.GetRoutineFromCacheById(routineId).Users;
                for (int j = 0; j < currentAssociatedEmployees.Count; j++)
                {
                    if (currentAssociatedEmployeesInCache.Contains(currentAssociatedEmployees.ElementAt(j)) == false)
                    {
                        Service.Instance.GetRoutineFromCacheById(routineId).Users.Add(currentAssociatedEmployees.ElementAt(j));
                    }
                }
                Service.Instance.Container.SaveChanges();
                Session["associatedEmployees"] = currentAssociatedEmployees;
                GridviewAssociatedEmployees.DataSource = associatedEmployees;
                GridviewAssociatedEmployees.DataBind();
                UpdateGridviewRoutineAndSelectedRoutine();
            }
        }

        private void DropdownlistPopulateAllEmployees()
        {
            allEmployees = Service.Instance.GetUsersFromCache().ToList();
            Session["allEmployees"] = allEmployees;
            DropdownlistAllEmployees.DataSource = allEmployees;
            DropdownlistAllEmployees.DataValueField = "Id";
            DropdownlistAllEmployees.DataTextField = "Username";
            DropdownlistAllEmployees.DataBind();
            DropdownlistAllEmployees.Items.Insert(0, new ListItem("Select an employee"));
        }

        private void GridviewPopulateCurrentAssociatedEmployees()
        {
            associatedEmployees = Service.Instance.GetRoutineFromCacheById(routineId).Users.ToList();
            Session["associatedEmployees"] = associatedEmployees;
            GridviewAssociatedEmployees.DataSource = associatedEmployees;
            GridviewAssociatedEmployees.DataBind();
            var groups = new List<Group>();
            foreach (var ae in associatedEmployees)
            {
                foreach (var g in ae.Group)
                {
                    if (!groups.Contains(g))
                    {
                        groups.Add(g);
                    }
                }
            }
            Session["groups"] = groups;
            PopulateDropdownListProjects(groups);
        }

        protected void GridviewAssociatedEmployees_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            var gridview = sender as GridView;
            var empId = Convert.ToInt32(gridview.DataKeys[e.RowIndex].Value);
            var selectedRoutine = Service.Instance.GetRoutineFromCacheById(routineId);
            selectedRoutine.Users.Remove(Service.Instance.GetUserFromCacheById(empId));
            Service.Instance.Container.SaveChanges();
            GridviewPopulateCurrentAssociatedEmployees();
            DropdownlistPopulateAllEmployees();
            UpdateGridviewRoutineAndSelectedRoutine();
        }
        //------------------------------------------------------------ACTIVITIES-------------------------------------------------------------

        private void PopulateDropdownListProjects(List<Group> groups)
        {
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
            DropdownlisAllProjects.DataSource = projects.OrderBy(p => p.Name);
            DropdownlisAllProjects.DataTextField = "Name";
            DropdownlisAllProjects.DataValueField = "EconomicProjectId";
            DropdownlisAllProjects.DataBind();
            DropdownlisAllProjects.Items.Insert(0, new ListItem("Select a project"));
            DropdownlisAllProjects.SelectedIndex = 0;
        }

        protected void DropdownlisAllProjects_SelectedIndexChanged(object sender, EventArgs e)
        {
            var dropdown = sender as DropDownList;
            if (dropdown.SelectedIndex > 0)
            {
                var selectedProject = Service.Instance.GetEconomicProjectFromCache(Convert.ToInt32(dropdown.SelectedValue));
                PopulateDropdownListEconomicTaskTypes(selectedProject);

                PopulateDropdownListHoursAndMinutes();
            }
        }

        private void PopulateDropdownListEconomicTaskTypes(EconomicProject selectedProject)
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
            DropdownlistAllTaskTypesForProject.DataSource = taskTypes;
            DropdownlistAllTaskTypesForProject.DataTextField = "Name";
            DropdownlistAllTaskTypesForProject.DataValueField = "EconomicTaskTypeId";
            DropdownlistAllTaskTypesForProject.DataBind();
            DropdownlistAllTaskTypesForProject.Items.Insert(0, new ListItem("Select a task type"));
            DropdownlistAllTaskTypesForProject.SelectedIndex = 0;
        }

        private void PopulateDropdownListHoursAndMinutes()
        {
            DropdownlistHours.Items.Clear();
            DropdownlistMinutes.Items.Clear();
            foreach (var hour in Service.Instance.GetHours())
            {
                string display = hour <= 9 ? "0" + hour.ToString() : hour.ToString();

                DropdownlistHours.Items.Add(new ListItem(display + " hours", (hour * 60).ToString()));
            }
            DropdownlistHours.Items.Insert(0, new ListItem("Select hour"));

            foreach (var minute in Service.Instance.GetMinutes())
            {
                string display = minute <= 9 ? "0" + minute.ToString() : minute.ToString();

                DropdownlistMinutes.Items.Add(new ListItem(display + " min (" + ((double)minute / (double)60).ToString("0.00") + " hours)", minute.ToString()));
            }
            DropdownlistMinutes.Items.Insert(0, new ListItem("Select minute"));
        }

        protected void ButtonAddActivityToRoutine_Click(object sender, EventArgs e)
        {
            if (IsValidToCreateActivity())
            {
                var project = Service.Instance.GetEconomicProjectFromCache(Convert.ToInt32(DropdownlisAllProjects.SelectedValue));
                var tasktype = Service.Instance.GetEconomicTaskTypeFromCache(Convert.ToInt32(DropdownlistAllTaskTypesForProject.SelectedValue));
                var totalDuration = Convert.ToInt32(DropdownlistHours.SelectedValue) + Convert.ToInt32(DropdownlistMinutes.SelectedValue);
                var activity = Service.Instance.CreateActivityToRoutine(project, tasktype, totalDuration);
                var routine = Service.Instance.GetRoutineFromCacheById(routineId);
                routine.Activities.Add(activity);
                currentActivities = routine.Activities.ToList();
                Session["currentActivities"] = currentActivities;
                ResetActivityFields();
                UpdateGridviewRoutineAndSelectedRoutine();
            }

        }
        private void ResetActivityFields()
        {
            DropdownlistHours.Items.Clear();
            DropdownlistMinutes.Items.Clear();
            DropdownlistAllTaskTypesForProject.Items.Clear();
            GridviewPopulateCurrentActivities(currentActivities);
            PopulateDropdownListProjects(groupsInRoutine);
        }
        private bool IsValidToCreateActivity()
        {
            var isValid = true;
            var text = "You are missing to fullfill the following:" + "<br/>";
            if (DropdownlisAllProjects.SelectedIndex == 0)
            {
                text += "Project" + "<br/>";
                isValid = false;
            }
            if (DropdownlistAllTaskTypesForProject.SelectedIndex == 0)
            {
                text += "Tasktype" + "<br/>";
                isValid = false;
            }
            if ((DropdownlistHours.SelectedIndex == 0 && DropdownlistMinutes.SelectedIndex == 0) || (DropdownlistHours.SelectedIndex == 0 || DropdownlistMinutes.SelectedIndex == 0))
            {
                text += "Hours and minutes" + "<br/>";
                isValid = false;
            }
            if (DropdownlistHours.SelectedIndex == 1 && DropdownlistMinutes.SelectedIndex == 1)
            {
                text += "Hours and minutes" + "<br/>";
                isValid = false;
            }

            if (isValid == false)
            {
                LabelErrorActivity.Text = text;
            }
            return isValid;
        }

        protected void GridviewAllActivitiesForRoutine_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            var gridview = sender as GridView;
            var activityId = Convert.ToInt32(gridview.DataKeys[e.RowIndex].Value);
            var selectedRoutine = Service.Instance.GetRoutineFromCacheById(routineId);
            var foundActivity = selectedRoutine.Activities.FirstOrDefault(a => a.Id == activityId);
            selectedRoutine.Activities.Remove(foundActivity);
            Service.Instance.DeleteActivity(activityId);
            currentActivities = selectedRoutine.Activities.ToList();
            Session["currentActivities"] = currentActivities;
            GridviewPopulateCurrentActivities(currentActivities);
            UpdateGridviewRoutineAndSelectedRoutine();
        }


        protected void ButtonUpdateActivity_Click(object sender, EventArgs e)
        {
            if (activityId > -1 && IsValidToCreateActivity())
            {
                var project = Service.Instance.GetEconomicProjectFromCache(Convert.ToInt32(DropdownlisAllProjects.SelectedValue));
                var tasktype = Service.Instance.GetEconomicTaskTypeFromCache(Convert.ToInt32(DropdownlistAllTaskTypesForProject.SelectedValue));
                var totalDuration = Convert.ToInt32(DropdownlistHours.SelectedValue) + Convert.ToInt32(DropdownlistMinutes.SelectedValue);

                Service.Instance.UpdateActivityToRoutine(activityId, project, tasktype, totalDuration);
                Session["activityId"] = null;
                ButtonAddActivityToRoutine.Enabled = true;
                ButtonUpdate.Enabled = false;
                currentActivities = Service.Instance.GetAllRoutinesFromCache().FirstOrDefault(r => r.Id == routineId).Activities.ToList();
                Session["currentActivities"] = currentActivities;
                ResetActivityFields();
                UpdateGridviewRoutineAndSelectedRoutine();
            }
        }

        private void GridviewPopulateCurrentActivities(List<Activity> activities)
        {
            GridviewAllActivitiesForRoutine.DataSource = activities;
            GridviewAllActivitiesForRoutine.DataBind();
        }

        protected void GridviewAllActivitiesForRoutine_SelectedIndexChanging(object sender, GridViewSelectEventArgs e)
        {
            var gridview = sender as GridView;
            var activity = Service.Instance.GetActivityFromCacheById(Convert.ToInt32(gridview.DataKeys[e.NewSelectedIndex].Value));
            DropdownlisAllProjects.SelectedValue = activity.EconomicProject.EconomicProjectId.ToString();
            PopulateDropdownListEconomicTaskTypes(activity.EconomicProject);
            PopulateDropdownListHoursAndMinutes();
            DropdownlistAllTaskTypesForProject.SelectedValue = activity.EconomicTaskType.EconomicTaskTypeId.ToString();
            if (activity.Minutes >= 60)
            {
                var split = (activity.Minutes / 60).ToString().Split(',');
                var splitZero = Convert.ToDouble(split[0]);
                var hour = Convert.ToInt32(splitZero * 60).ToString();
                DropdownlistHours.SelectedValue = hour;
                if (split.Length > 1)
                {
                    var stringMin = "0," + split[1];
                    var minute = Convert.ToDouble(stringMin);
                    var calMinute = Convert.ToInt32(minute * 60).ToString();
                    DropdownlistMinutes.SelectedValue = calMinute;
                }
                else
                {
                    DropdownlistMinutes.SelectedIndex = 1;
                }
            }
            else
            {
                DropdownlistHours.SelectedIndex = 1;
                DropdownlistMinutes.SelectedValue = activity.Minutes.ToString();
            }
            ButtonAddActivityToRoutine.Enabled = false;
            ButtonUpdate.Enabled = true;
            Session["activityId"] = activity.Id;
        }
    }
}