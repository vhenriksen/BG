using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BG.Controller;
using BG.Model;
using Economic.Api.Client.WebService;

namespace BG.View
{
    public partial class workday : System.Web.UI.Page
    {
        private User user;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["LoggedInUser"] != null)
            {
                user = Session["LoggedInUser"] as User;
            }
            else
            {
                Response.Redirect("Home.aspx");
            }
            if (user.DailyReport.Any())
            {
                var dailyReport = user.DailyReport.FirstOrDefault();
                if (dailyReport.DayStarted.HasValue && dailyReport.DayEnded.HasValue)
                {
                    ButtonUploadWorkday.Enabled = true;
                }
            }
            LabelWorkdayError.Text = string.Empty;
            LabelFlashbackError.Text = string.Empty;
        }

        protected void ButtonViewWorkday_OnClick(object sender, EventArgs e)
        {
            ViewWorkday();
        }

        private void ViewWorkday()
        {
            var dailyReport = user.DailyReport.FirstOrDefault();
            if (dailyReport != null)
            {
                var tasks = dailyReport.Task.OrderBy(t => t.TimeStarted);
                if (tasks.Any())
                {
                    var divDataTabel = divData;
                    divWorkday.InnerHtml = "Day started: " + dailyReport.DayStarted.Value.ToString("g") + "<br/>";
                    if (dailyReport.DayEnded.HasValue)
                    {
                        divWorkday.InnerHtml += "Day ended: " + dailyReport.DayEnded.Value.ToString("g");
                    }
                    else
                    {
                        divWorkday.InnerHtml += "Day ended: Not set";
                    }
                    var table = new Table();
                    var viewPause = false;
                    for (int i = 0; i < tasks.Count(); i++)
                    {
                        var currentTask = tasks.ElementAt(i);

                        var pause = dailyReport.Break;
                        if (pause.BreakStarted < currentTask.TimeStarted && viewPause == false)
                        {
                            var rowBreak = new TableRow();
                            var cellEmpty = new TableCell();
                            rowBreak.Cells.Add(cellEmpty);

                            var cellBreak = new TableCell();
                            var labelBreak = new Label();
                            labelBreak.Text = "Break";
                            cellBreak.Controls.Add(labelBreak);
                            rowBreak.Cells.Add(cellBreak);

                            var cellBreakTime = new TableCell();
                            var labelBreakTime = new Label();
                            var startBreak = pause.BreakStarted.Value.ToString("t");
                            var endBreak = pause.BreakEnded.Value.ToString("t");
                            labelBreakTime.Text = startBreak + "-" + endBreak;
                            viewPause = true;

                            cellBreakTime.Controls.Add(labelBreakTime);
                            rowBreak.Cells.Add(cellBreakTime);

                            table.Rows.Add(rowBreak);
                        }

                        var row = new TableRow();
                        var cellEmptys = new TableCell();
                        row.Cells.Add(cellEmptys);

                        var cellProjectTask = new TableCell();
                        var labelProjectTask = new Label();
                        labelProjectTask.Text = currentTask.EconomicProject.Name + "</br>" +
                                              currentTask.EconomicTask.Name;
                        cellProjectTask.Controls.Add(labelProjectTask);
                        row.Cells.Add(cellProjectTask);

                        var cellTime = new TableCell();
                        var labelTime = new Label();
                        var start = currentTask.TimeStarted.Value.ToString("t");
                        var end = currentTask.TimeEnded.Value.ToString("t");
                        labelTime.Text = start + "-" + end;
                        cellTime.Controls.Add(labelTime);
                        row.Cells.Add(cellTime);

                        table.Rows.Add(row);
                    }

                    var rowNothing = new TableRow();
                    for (int i = 0; i < 3; i++)
                    {
                        var cellNothing = new TableCell();
                        rowNothing.Cells.Add(cellNothing);
                    }
                    table.Rows.Add(rowNothing);

                    var rowUsedTime = new TableRow();
                    var cellUsedTime = new TableCell();
                    var labelUsedTime = new Label();
                    labelUsedTime.Text = "Consumed time of";
                    labelUsedTime.Font.Bold = true;
                    cellUsedTime.Controls.Add(labelUsedTime);
                    rowUsedTime.Cells.Add(cellUsedTime);

                    var cellTasksCount = new TableCell();
                    var labelTasksCount = new Label();
                    labelTasksCount.Text = tasks.Count() + " tasks excl. break";
                    labelTasksCount.Font.Bold = true;
                    cellTasksCount.Controls.Add(labelTasksCount);
                    rowUsedTime.Cells.Add(cellTasksCount);

                    var cellOverallTime = new TableCell();
                    var labelOverallTime = new Label();
                    var result = CalculateDailyreport(tasks);
                    var hours = result.ElementAt(0);
                    var min = result.ElementAt(1);
                    if (hours > 0 && min > 0)
                    {
                        labelOverallTime.Text = hours + " hours and " + min.ToString("####") + " minutes" + "</br>" + "will be uploaded to economic";
                    }
                    else if (hours > 0 && min <= 0)
                    {
                        labelOverallTime.Text = hours + " hours" + "</br>" + "will be uploaded to economic";
                    }
                    else if (hours <= 0 && min > 0)
                    {
                        labelOverallTime.Text = min.ToString("####") + " minutes" + "</br>" + "will be uploaded to economic";
                    }
                    labelOverallTime.Font.Bold = true;
                    cellOverallTime.Controls.Add(labelOverallTime);
                    rowUsedTime.Cells.Add(cellOverallTime);

                    table.Rows.Add(rowUsedTime);

                    divDataTabel.Controls.Add(table);
                }
            }
        }

        private List<int> CalculateDailyreport(IEnumerable<Task> tasks)
        {
            var result = new List<int>();
            var hour = 0;
            var min = 0;
            foreach (var task in tasks)
            {
                var timespan = task.TimeEnded.Value.Subtract(task.TimeStarted.Value);
                hour += timespan.Hours;
                min += timespan.Minutes;
                if (min >= 60)
                {
                    hour += 1;
                    min = min - 60;
                }
            }
            result.Add(hour);
            result.Add(min);
            return result;
        }

        protected void ButtonUploadWorkday_OnClick(object sender, EventArgs e)
        {
            try
            {
                Service.Instance.UploadToEconomic(user);
                divWorkday.Visible = false;
                ButtonUploadWorkday.Enabled = false;

            }
            catch (Exception ex)
            {
                LabelWorkdayError.Text = ex.Message;
            }
        }

        protected void ButtonCancel_OnClick(object sender, EventArgs e)
        {
            Session["LoggedInUser"] = user;
            Response.Redirect("Home.aspx");
        }

        protected void ButtonFlashback_OnClick(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(LabelFromDate.Text) && !string.IsNullOrEmpty(LabelToDate.Text))
                {
                    var fromDate = Convert.ToDateTime(LabelFromDate.Text);
                    var toDate = Convert.ToDateTime(LabelToDate.Text);
                    if (fromDate < toDate)
                    {
                        SummaryOfMonthlySalary(fromDate, toDate);
                        LabelFromDate.Text = string.Empty;
                        LabelToDate.Text = string.Empty;
                        ButtonFlashback.Enabled = false;
                    }
                    else
                    {
                        LabelFlashbackError.Text = "From date must be less than to date";
                    }
                }
                else
                {
                    LabelFlashbackError.Text = "Select from and to date.";
                }
            }
            catch (Exception ex)
            {
                LabelFlashbackError.Text = ex.Message;
            }
        }

        private void SummaryOfMonthlySalary(DateTime fromDate, DateTime toDate)
        {
            var timeEntries = Service.Instance.Flashback(fromDate, toDate, user);
            var table = new Table();
            foreach (var timeEntry in timeEntries)
            {
                var tableRow = new TableRow();

                var tableCellDate = new TableCell();
                var labelDate = new Label();
                labelDate.Text = timeEntry.Date.ToString("d");
                tableCellDate.Controls.Add(labelDate);

                tableRow.Cells.Add(tableCellDate);

                var splitTime = timeEntry.NumberOfHours.ToString("##.###").Split(',');

                var tableCellHour = new TableCell();
                var labelHour = new Label();
                var hour = Convert.ToInt32(splitTime.ElementAt(0));
                if (hour > 1)
                {
                    labelHour.Text = hour + " hours";
                }
                else if (hour == 1)
                {
                    labelHour.Text = hour + " hour";
                }
                else
                {
                    labelHour.Text = string.Empty;
                }
                tableCellHour.Controls.Add(labelHour);

                tableRow.Cells.Add(tableCellHour);

                var minutes = 0.0;
                if (splitTime.Count() == 2)
                {
                    var min = "0," + splitTime.ElementAt(1);
                    var convertToDouble = Convert.ToDouble(min);
                    minutes = convertToDouble * 60;
                }
                var tableCellMin = new TableCell();
                var labelMin = new Label();
                if (minutes > 1)
                {
                    labelMin.Text = minutes.ToString("####") + " minutes";
                }
                else if (minutes == 1)
                {
                    labelMin.Text = minutes.ToString("####") + " minute";
                }
                else
                {
                    labelMin.Text = string.Empty;
                }

                tableCellMin.Controls.Add(labelMin);

                tableRow.Cells.Add(tableCellMin);

                table.Rows.Add(tableRow);
            }
            var tableRowSum = new TableRow();

            var tableCellSumText = new TableCell();
            var labelSumText = new Label();
            labelSumText.Font.Bold = true;
            labelSumText.Text = "Summary work";
            tableCellSumText.Controls.Add(labelSumText);

            tableRowSum.Cells.Add(tableCellSumText);

            var totalSumSplit = timeEntries.Sum(t => t.NumberOfHours).ToString("##.###").Split(',');

            var tableCellSumHour = new TableCell();
            var labelCellSumHour = new Label();
            labelCellSumHour.Font.Bold = true;
            var hours = Convert.ToInt32(totalSumSplit.ElementAt(0));
            if (hours > 1)
            {
                labelCellSumHour.Text = hours + " hours";
            }
            else if (hours == 1)
            {
                labelCellSumHour.Text = hours + " hour";
            }
            else
            {
                labelCellSumHour.Text = string.Empty;
            }
            tableCellSumHour.Controls.Add(labelCellSumHour);

            tableRowSum.Cells.Add(tableCellSumHour);

            var totalMinutes = 0.0;
            if (totalSumSplit.Count() == 2)
            {
                var totalMin = "0," + totalSumSplit.ElementAt(1);
                var totalConvertToDouble = Convert.ToDouble(totalMin);
                totalMinutes = totalConvertToDouble * 60;
            }
            var tableCellSumMin = new TableCell();
            var labelCellSumMin = new Label();
            labelCellSumMin.Font.Bold = true;
            if (totalMinutes > 1)
            {
                labelCellSumMin.Text = totalMinutes.ToString("####") + " minutes";
            }
            else if (totalMinutes == 1)
            {
                labelCellSumMin.Text = totalMinutes.ToString("####") + " minute";
            }
            else
            {
                labelCellSumMin.Text = string.Empty;
            }
            tableCellSumMin.Controls.Add(labelCellSumMin);

            tableRowSum.Cells.Add(tableCellSumMin);

            table.Rows.Add(tableRowSum);

            tableViewOfMonthPay.Controls.Add(table);
        }

        protected void CalenderFromDate_OnSelectionChanged(object sender, EventArgs e)
        {
            var calender = sender as Calendar;
            LabelFromDate.Text = calender.SelectedDate.ToString("d");
            if (!string.IsNullOrEmpty(LabelToDate.Text))
            {
                ButtonFlashback.Enabled = true;
            }
        }

        protected void CalendarToDate_OnSelectionChanged(object sender, EventArgs e)
        {
            var calender = sender as Calendar;
            LabelToDate.Text = calender.SelectedDate.ToString("d");
            if (!string.IsNullOrEmpty(LabelFromDate.Text))
            {
                ButtonFlashback.Enabled = true;
            }
        }

        protected void CalendarFromDateSumOfTwoDays_OnSelectionChanged(object sender, EventArgs e)
        {
            var calender = sender as Calendar;
            LabelFromDateSumOfTwoDays.Text = calender.SelectedDate.ToString("d");
            if (!string.IsNullOrEmpty(LabelToDateSumOfTwoDays.Text))
            {
                ButtonSummaryTwoDays.Enabled = true;
            }
        }

        protected void CalendarToDateSumOfTwoDays_OnSelectionChanged(object sender, EventArgs e)
        {
            var calender = sender as Calendar;
            LabelToDateSumOfTwoDays.Text = calender.SelectedDate.ToString("d");
            if (!string.IsNullOrEmpty(LabelFromDateSumOfTwoDays.Text))
            {
                var fromDate = Convert.ToDateTime(LabelFromDateSumOfTwoDays.Text);
                var toDate = Convert.ToDateTime(LabelToDateSumOfTwoDays.Text);
                var timeInterval = toDate.Date.Subtract(fromDate.Date);

                if (timeInterval.Days >= 0)
                {
                    if (timeInterval.Days <= 1)
                    {
                        ButtonSummaryTwoDays.Enabled = true;
                    }
                    else
                    {
                        LabelFlashbackError.Text = "To date must only be the same as 'from date' or add 1 day to 'to date'" + "<br/>" + "Ex. From: 09-01-14 To: 09-01-14" +"<br/>" + "Ex. From: 09-01-14 To: 10-01-14";
                    }
                }
                else
                {
                    LabelFlashbackError.Text = "To date can not be less than from date." + "<br/>" + "Ex. From: 09-01-14 To: 09-01-14" +"<br/>" + "Ex. From: 09-01-14 To: 10-01-14";
                }
            }
        }

        protected void ButtonSummaryTwoDays_OnClick(object sender, EventArgs e)
        {
            try
            {
                var fromDate = Convert.ToDateTime(LabelFromDateSumOfTwoDays.Text);
                var toDate = Convert.ToDateTime(LabelToDateSumOfTwoDays.Text);
                var timeEntries = Service.Instance.Flashback(fromDate, toDate, user);
                SummaryOfTwoDays(timeEntries);
                LabelFromDateSumOfTwoDays.Text = string.Empty;
                LabelToDateSumOfTwoDays.Text = string.Empty;
                ButtonSummaryTwoDays.Enabled = false;
            }
            catch (Exception ex)
            {
                LabelFlashbackError.Text = ex.Message;
            }
        }

        private void SummaryOfTwoDays(List<Economic.Api.ITimeEntry> timeEntries)
        {
            var table = new Table();
            var currentDate = new DateTime();
            foreach (var timeEntry in timeEntries)
            {
                var tableRow = new TableRow();
                var tableCellDate = new TableCell();
                var labelCellDate = new Label();
                if (timeEntry.Date > currentDate)
                {
                    currentDate = timeEntry.Date;
                    labelCellDate.Text = timeEntry.Date.ToString("d");
                }
                else
                {
                    labelCellDate.Text = string.Empty;
                }
                tableCellDate.Controls.Add(labelCellDate);
                tableRow.Cells.Add(tableCellDate);

                var tableCellProject = new TableCell();
                var labelCellProject = new Label();
                labelCellProject.Text = timeEntry.Project.Name+", ";
                tableCellProject.Controls.Add(labelCellProject);
                tableRow.Cells.Add(tableCellProject);

                var tableCellTasktype = new TableCell();
                var labelCellTasktype = new Label();
                labelCellTasktype.Text = timeEntry.Activity.Name;
                tableCellTasktype.Controls.Add(labelCellTasktype);
                tableRow.Cells.Add(tableCellTasktype);

                table.Rows.Add(tableRow);
            }
            tableViewOfSummaryOfTwoDays.Controls.Add(table);
        }
    }
}