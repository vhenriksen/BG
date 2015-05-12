using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BG.Controller;
using BG.Model;

namespace BG.View
{
    public partial class DayStartedEnded : System.Web.UI.Page
    {
        protected bool daystartedbutton;
        private User user;

        protected void Page_Load(object sender, EventArgs e)
        {
            user = (User)Session["LoggedInUser"];
            if (user == null) Response.Redirect("Default.aspx");

            daystartedbutton = (bool)Session["daystartedbutton"];

            if (daystartedbutton)
            {
                ShowDayStarted();

                if (!IsPostBack)
                {
                    SetTimeOnStartAndEnd();

                    LoadDayStartedSuggestion();

                    //Dagen starter altid kl 7
                    listboxTimeStartHour.SelectedValue = 7.ToString();
                    listboxTimeStartMin.SelectedValue = 0.ToString();

                    if (Service.Instance.GetUserCurrentDailyReport(user).DayStarted.HasValue)
                    {
                        CheckBoxMoveTasks.Visible = true;
                        CheckBoxMoveTasks.Enabled = true;
                        LoadSelectedDayAndTimeStarted(Service.Instance.GetUserCurrentDailyReport(user).DayStarted.Value);
                    }
                    else
                    {
                        CheckBoxMoveTasks.Visible = false;
                    }
                }
            }
            else
            {
                ShowDayEnded();
                if (!IsPostBack)
                {
                    CheckBoxMoveTasks.Enabled = false;
                    SetTimeOnStartAndEnd();

                    LoadDayEndedSuggestion();
                }
            }
        }

        protected void ButtonCancel_Click(object sender, EventArgs e)
        {
            Session["daystartedbutton"] = null;
            Response.Redirect("Home.aspx");
        }

        protected void ButtonOk_Click(object sender, EventArgs e)
        {
            DailyReport currentDailyReport = null;
            try
            {
                currentDailyReport = Service.Instance.GetDailyReport(user);
            }
            catch (Exception ex)
            {
                LabelEndTimeError.Text = ex.Message;
            }
            if (daystartedbutton)
                {
                    // Opretter en dailyReport hvis der ikke er nogen, uden start tid og slut tid.
                    DateTime startDate = DateTime.Parse(DropDownStartDate.SelectedItem.Text);
                    startDate = startDate.AddHours(Convert.ToInt32(listboxTimeStartHour.SelectedValue));
                    startDate = startDate.AddMinutes(Convert.ToInt32(listboxTimeStartMin.SelectedValue));
                    if (currentDailyReport.DayStarted.HasValue && currentDailyReport.Task.Count >= 0)//Update
                    {
                        if (startDate != currentDailyReport.DayStarted)
                        {
                            try
                            {
                                SaveSelectedDayAndTimeStarted(CheckBoxMoveTasks.Checked);
                                Session["daystartedbutton"] = null;
                                Response.Redirect("Home.aspx");
                            }
                            catch (Exception ex)
                            {
                                LabelStartTimeError.Text = ex.Message;
                            }
                        }
                        else
                        {
                            Session["daystartedbutton"] = null;
                            Response.Redirect("Home.aspx");
                        }
                    }
                    else //Create new
                    {
                        try
                        {
                            Service.Instance.SetCurrentDailyReportStartAndBreak(startDate, currentDailyReport);
                            Session["daystartedbutton"] = null;
                            Response.Redirect("Home.aspx");
                        }
                        catch (Exception ex)
                        {
                            LabelEndTimeError.Text = ex.Message;
                        }

                    }

                }
                else
                {
                    DateTime endDate = DateTime.Parse(DropDownEndDate.SelectedItem.Text);
                    endDate = endDate.AddHours(Convert.ToInt32(listboxTimeEndHour.SelectedValue));
                    endDate = endDate.AddMinutes(Convert.ToInt32(listboxTimeEndMin.SelectedValue));
                    try
                    {
                        SaveSelectedDayAndTimeEnded(CheckBoxMoveTasks.Checked);
                        Session["daystartedbutton"] = null;
                        Response.Redirect("Home.aspx");

                    }
                    catch (Exception ex)
                    {
                        LabelEndTimeError.Text = ex.Message;
                    }

                }
            
        }

        private void SaveSelectedDayAndTimeStarted(bool moveTasks)
        {

            Service.Instance.UpdateDailyReportTimes(user, DropDownStartDate.SelectedItem.Text,
                                                               Convert.ToInt32(listboxTimeStartHour.SelectedValue),
                                                               Convert.ToInt32(listboxTimeStartMin.SelectedValue),
                                                               false, moveTasks);


        }

        private void SaveSelectedDayAndTimeEnded(bool moveTasks)
        {
            Service.Instance.UpdateDailyReportTimes(user,
                                                    DropDownEndDate.SelectedItem.Text,
                                                    Convert.ToInt32(listboxTimeEndHour.SelectedValue),
                                                    Convert.ToInt32(listboxTimeEndMin.SelectedValue),
                                                    true, moveTasks);
        }

        private void SetTimeOnStartAndEnd()
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
        }

        private void LoadSelectedDayAndTimeStarted(DateTime dayAndTimeStarted)
        {
            DropDownStartDate.SelectedIndex =
                DropDownStartDate.Items.IndexOf(new ListItem(dayAndTimeStarted.ToShortDateString()));

            listboxTimeStartHour.SelectedValue = dayAndTimeStarted.Hour.ToString();
            listboxTimeStartMin.SelectedValue = dayAndTimeStarted.Minute.ToString();
        }

        private void LoadDayStartedSuggestion()
        {
            DropDownStartDate.Items.Clear();

            DateTime dayStartedSuggestion = DateTime.Now;
            for (int i = 30; i > 0; i--)
            {
                DropDownStartDate.Items.Add(dayStartedSuggestion.ToShortDateString());
                dayStartedSuggestion = dayStartedSuggestion.AddDays(-1);
            }

            DropDownStartDate.SelectedIndex = 0;
        }

        private void LoadDayEndedSuggestion()
        {
            DropDownEndDate.Items.Clear();
            DateTime selectedEndDate = new DateTime();
            var dailyReport = user.DailyReport.FirstOrDefault();
            if (!dailyReport.DayEnded.HasValue)
            {
                if (dailyReport.Task.Any())
                {
                    var tasksAfterBreak =
                        dailyReport.Task.Where(t => t.TimeEnded > dailyReport.Break.BreakEnded).ToList();
                    if (tasksAfterBreak.Any())
                    {
                        var lastTask = dailyReport.Task.LastOrDefault(t => t.TimeEnded > dailyReport.Break.BreakEnded);
                        selectedEndDate = lastTask.TimeEnded.Value;
                        listboxTimeEndHour.SelectedValue = lastTask.TimeEnded.Value.Hour.ToString();
                        listboxTimeEndMin.SelectedValue = lastTask.TimeEnded.Value.Minute.ToString();
                    }
                    else
                    {
                        selectedEndDate = dailyReport.Break.BreakEnded.Value;
                        listboxTimeEndHour.SelectedValue = selectedEndDate.Hour.ToString();
                        listboxTimeEndMin.SelectedValue = selectedEndDate.Minute.ToString();
                    }
                }
                else
                {
                    selectedEndDate = dailyReport.Break.BreakEnded.Value;
                    listboxTimeEndHour.SelectedValue = selectedEndDate.Hour.ToString();
                    listboxTimeEndMin.SelectedValue = selectedEndDate.Minute.ToString();
                }
            }
            else
            {
                selectedEndDate = dailyReport.DayEnded.Value;
                listboxTimeEndHour.SelectedValue = selectedEndDate.Hour.ToString();
                listboxTimeEndMin.SelectedValue = selectedEndDate.Minute.ToString();
            }
            DropDownEndDate.Items.Add(selectedEndDate.AddDays(1).ToShortDateString());
            var tmp = selectedEndDate;
            for (int i = 5; i >= 0; i--)
            {
                DropDownEndDate.Items.Add(selectedEndDate.ToShortDateString());
                selectedEndDate = selectedEndDate.AddDays(-1);

            }
            DropDownEndDate.SelectedIndex =
                DropDownEndDate.Items.IndexOf(new ListItem(tmp.Date.ToShortDateString()));


        }

        private void ShowDayStarted()
        {
            DropDownEndDate.Visible = false;
            listboxTimeEndHour.Visible = false;
            listboxTimeEndMin.Visible = false;
            labelEndHour.Visible = false;
            labelEndMin.Visible = false;
            labelDayEnd.Visible = false;
            DivEndtime1.Visible = false;
            DivEndtime2.Visible = false;
            LabelStartTimeError.Visible = true;
            LabelEndTimeError.Visible = false;
        }

        private void ShowDayEnded()
        {
            LabelStartTimeError.Visible = false;
            LabelEndTimeError.Visible = true;
            DropDownStartDate.Visible = false;
            listboxTimeStartHour.Visible = false;
            listboxTimeStartMin.Visible = false;
            labelStartHour.Visible = false;
            labelStartMin.Visible = false;
            labelStartDate.Visible = false;
            DivStarttime1.Visible = false;
            DivStarttime2.Visible = false;
            headertext.InnerText = "End Time";
        }
    }
}
