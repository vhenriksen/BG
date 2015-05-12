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
    public partial class Routines : System.Web.UI.Page
    {
        private IEnumerable<Routine> routines;
        protected List<Routine> oldRoutines;
        protected List<Routine> fixedRoutines; 
        protected void Page_Load(object sender, EventArgs e)
        {
            var user = Session["LoggedInUser"] as User;

            if (!IsPostBack)
            {
                routines = user.Routines.ToList();

                fixedRoutines=new List<Routine>();
                oldRoutines = new List<Routine>();
                foreach (var routine in routines)
                {
                    var oldDate = routine.Dates.OrderByDescending(d => d.TheDate).FirstOrDefault();
                    var countAc = routine.Activities.Any(a => a.IsUsed == false);
                    if (!countAc || oldDate.TheDate.Date < DateTime.Now.Date)
                    {
                        oldRoutines.Add(routine);
                    }
                    else
                    {
                        if (!fixedRoutines.Any())
                        {
                            fixedRoutines.Add(routine);
                        }
                        else
                        {
                            var isInsert = false;
                            for (int i = 0; i < fixedRoutines.Count; i++)
                            {
                                if (!isInsert)
                                {
                                    var fromDate =
                                        fixedRoutines.ElementAt(i)
                                            .Dates.OrderByDescending(d => d.TheDate)
                                            .FirstOrDefault();
                                    var currentRoutineFromDate =
                                        routine.Dates.OrderByDescending(d => d.TheDate).FirstOrDefault();
                                    var ticks = fromDate.TheDate.Ticks - DateTime.Now.Ticks;
                                    var currentTicks = currentRoutineFromDate.TheDate.Ticks - DateTime.Now.Ticks;
                                    if (currentTicks < ticks)
                                    {
                                        fixedRoutines.Insert(i, routine);
                                        isInsert = true;
                                    }
                                }
                            }
                            if (fixedRoutines.Contains(routine) == false)
                            {
                                fixedRoutines.Add(routine);
                            }
                        }
                    }
                }
            }
            LabelRoutinesError.Text = string.Empty;
        }
        protected void ButtonHomeClick(object sender, EventArgs args)
        {
            Response.Redirect("Home.aspx");
        }
    }
}