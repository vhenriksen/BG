using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using BG.Controller;
using BG.Model;

namespace BG.View
{
    public partial class PerformedActivities : System.Web.UI.Page
    {
        protected List<Activity> Activities;
        protected Dictionary<string, DropDownList> DropDownLists;
        private User user;
        private int routineId;
        protected void Page_Load(object sender, EventArgs e)
        {
            user = Session["LoggedInUser"] as User;
            if (Session["activities"] != null)
            {
                Activities = Session["activities"] as List<Activity>;
            }
            else
            {
                Activities = new List<Activity>();
            }
            if (Session["routineId"] != null)
            {
                routineId = Convert.ToInt32(Session["routineId"]);
            }
            DropDownLists = new Dictionary<string, DropDownList>();
            LabelError.Text = string.Empty;
            GenerateDivWithActivities(Activities);

        }

        protected void ButtonAccept_OnClick(object sender, EventArgs e)
        {
            try
            {
                foreach (var activity in Activities)
                {
                    var hoursDropDown = DropDownLists["h" + activity.Id];
                    var minutesDropDown = DropDownLists["m" + activity.Id];

                    var hours = Convert.ToInt32(hoursDropDown.SelectedValue)*60;
                    var minutes = Convert.ToInt32(minutesDropDown.SelectedValue);
                    activity.Minutes = hours + minutes;

                    Service.Instance.RoutineAccepted(user, activity.Routine, activity);

                    var activityUser = ActivityUser.CreateActivityUser(0, activity.Id, user.Id);
                    activity.ActivityUser.Add(activityUser);

                    if (activity.ActivityUser.Count >= 1 && activity.ActivityTaken == null)
                    {
                        activity.ActivityTaken = DateTime.Now;
                        activity.IsUsed = true;
                    }
                    activityUser.Activity = activity;
                    activity.TakenBy = activityUser;
                    Service.Instance.Container.SaveChanges();

                }
                Session["LoggedInUser"] = user;
                Session["activities"] = null;
                Response.Redirect("Home.aspx");
            }
            catch (Exception ex)
            {
                LabelError.Text = "Something went wrong. Please try go back or restart webapp.";
            }
        }

        protected void ButtonBack_OnClick(object sender, EventArgs e)
        {
            Session["LoggedInUser"] = user;
            Session["activities"] = null;
            Response.Redirect("SelectedRoutine.aspx?routineId=" + routineId);
        }

        private void GenerateDivWithActivities(List<Activity> activities)
        {
            try
            {
                foreach (var activity in activities)
                {
                    var panel = new Panel();
                    panel.Attributes.Add("data-demo-html", "true");

                    var table = new Table();
                    var row1 = new TableRow();
                    table.Controls.Add(row1);

                    var col1 = new TableCell();
                    row1.Controls.Add(col1);
                    col1.ColumnSpan = 3;

                    var label = new Label();
                    label.Text = activity.EconomicProject.Name + "<br/>" + activity.EconomicTaskType.Name;
                    col1.Controls.Add(label);

                    var row2 = new TableRow();
                    var col21 = new TableCell();
                    var col22 = new TableCell();

                    var dropdownHours = new DropDownList();
                    dropdownHours.ID = "h" + activity.Id;
                    DropDownLists.Add(dropdownHours.ID, dropdownHours);
                    var split = (activity.Minutes / 60).ToString().Split(',');


                    var dropdownMin = new DropDownList();
                    dropdownMin.ID = "m" + activity.Id;
                    DropDownLists.Add(dropdownMin.ID, dropdownMin);
                    SetHoursAndMinutes(dropdownHours, dropdownMin);

                    dropdownHours.SelectedIndex = Convert.ToInt32(split[0]);
                    if (split.Length > 1)
                    {
                        var stringMin = "0," + split[1];
                        var minute = Convert.ToDouble(stringMin);
                        var calMinute = Convert.ToInt32(minute * 60).ToString();
                        dropdownMin.SelectedValue = calMinute;
                    }

                    col21.Controls.Add(dropdownHours);
                    col22.Controls.Add(dropdownMin);

                    row2.Controls.Add(col21);
                    row2.Controls.Add(col22);

                    table.Controls.Add(row2);
                    panel.Controls.Add(table);

                    var labelSeparator = new Label();
                    labelSeparator.Text = "<hr/>";

                    DivText.Controls.Add(panel);
                    DivText.Controls.Add(labelSeparator);
                }
            }
            catch (Exception ex)
            {
                LabelError.Text = "Sorry something went wrong. Please try to go back or restart webapp.";
            }
        }
        private void SetHoursAndMinutes(DropDownList hours, DropDownList minutes)
        {

            foreach (var hour in Service.Instance.GetHours())
            {
                string display = hour <= 9 ? "0" + hour.ToString() : hour.ToString();

                hours.Items.Add(new ListItem(display + " hours", (hour).ToString()));
            }

            foreach (var minute in Service.Instance.GetMinutes())
            {
                string display = minute <= 9 ? "0" + minute.ToString() : minute.ToString();

                minutes.Items.Add(new ListItem(display + " min (" + ((double)minute / (double)60).ToString("0.00") + " hours)", minute.ToString()));
            }
        }


    }
}