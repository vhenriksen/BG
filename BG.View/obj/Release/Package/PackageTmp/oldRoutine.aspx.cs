using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BG.Controller;
using BG.Model;

namespace BG.View
{
    public partial class oldRoutine : System.Web.UI.Page
    {
        private User user;
        private Routine _oldRoutine;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["LoggedInUser"] != null)
            {
                user = Session["LoggedInUser"] as User;
            }
            else
            {
                Response.Redirect("Default.aspx");
            }
            if (Request["oldRoutineId"] != null && Convert.ToInt32(Request["oldRoutineId"]) > -1)
            {
                var id = Convert.ToInt32(Request["oldRoutineId"]);
                _oldRoutine = Service.Instance.GetRoutineFromCacheById(id);
            }
            headRoutinename.InnerHtml = _oldRoutine.Name;
            if (!IsPostBack)
            {
                PopulateDropdownlists();
                var dates = _oldRoutine.Dates.OrderBy(d => d.TheDate);
                var fromDate = dates.ElementAt(0);
                var toDate = dates.ElementAt(1);
                DropdownlistFromDateDay.SelectedValue = fromDate.TheDate.Day.ToString();
                DropdownlistFromDateMonth.SelectedValue = fromDate.TheDate.Month.ToString();
                DropdownlistFromDateYear.SelectedValue = fromDate.TheDate.Year.ToString();
                DropdownlistToDateDay.SelectedValue = toDate.TheDate.Day.ToString();
                DropdownlistToDateMonth.SelectedValue = toDate.TheDate.Month.ToString();
                DropdownlistToDateYear.SelectedValue = toDate.TheDate.Year.ToString();
            }
        }

        private void PopulateDropdownlists()
        {
            for (int i = 1; i < 32; i++)
            {
                DropdownlistFromDateDay.Items.Add(new ListItem(i + "", i + ""));
                DropdownlistToDateDay.Items.Add(new ListItem(i + "", i + ""));
            }
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            var months = CultureInfo.CurrentCulture.DateTimeFormat.MonthGenitiveNames;
            for (int i = 0; i < months.Count() - 1; i++)
            {
                DropdownlistFromDateMonth.Items.Add(new ListItem(months.ElementAt(i), (i + 1) + ""));
                DropdownlistToDateMonth.Items.Add(new ListItem(months.ElementAt(i), (i + 1) + ""));
            }
            for (int i = 1; i <= 3; i++)
            {
                var year = DateTime.Now.Year + i;
                if (i == 1)
                {
                    DropdownlistFromDateYear.Items.Add(new ListItem(DateTime.Now.Year - 1 + "", DateTime.Now.Year - 1 + ""));
                    DropdownlistToDateYear.Items.Add(new ListItem(DateTime.Now.Year - 1 + "", DateTime.Now.Year - 1 + ""));
                    DropdownlistFromDateYear.Items.Add(new ListItem(DateTime.Now.Year + "", DateTime.Now.Year + ""));
                    DropdownlistToDateYear.Items.Add(new ListItem(DateTime.Now.Year + "", DateTime.Now.Year + ""));
                }
                DropdownlistFromDateYear.Items.Add(new ListItem(year + "", year + ""));
                DropdownlistToDateYear.Items.Add(new ListItem(year + "", year + ""));
            }
        }

        protected void ButtonAcceptOldRoutine_OnClick(object sender, EventArgs e)
        {
            try
            {
                string stringDate = DropdownlistFromDateDay.SelectedValue + "/" + 
                                    DropdownlistFromDateMonth.SelectedValue +"/" +
                                    DropdownlistFromDateYear.SelectedValue;
                DateTime fromDateValue;
                if (DateTime.TryParse(stringDate, out fromDateValue))
                {
                    string stringToDate = DropdownlistToDateDay.SelectedValue + "/" +
                                          DropdownlistToDateMonth.SelectedValue + "/" +
                                          DropdownlistToDateYear.SelectedValue;
                    DateTime toDateValue;
                    if (DateTime.TryParse(stringToDate, out toDateValue))
                    {
                        var dates = _oldRoutine.Dates.OrderBy(d => d.TheDate);
                        dates.ElementAt(0).TheDate = fromDateValue;
                        dates.ElementAt(1).TheDate = toDateValue;
                        foreach (var activity in _oldRoutine.Activities)
                        {
                            activity.IsUsed = false;
                            activity.ActivityTaken = null;
                            activity.TakenBy = null;
                        }
                        Service.Instance.Container.SaveChanges();
                        Response.Redirect("Routines.aspx");
                    }
                    else
                    {
                        LabelError.Text = "To date is not valid.";
                    }
                }
                else
                {
                    LabelError.Text = "From date is not valid.";
                }
            }
            catch (Exception ex)
            {
                LabelError.Text = ex.Message;
            }
        }
    }
}