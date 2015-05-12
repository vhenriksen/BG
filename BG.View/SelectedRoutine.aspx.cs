using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.ServiceModel;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BG.Controller;
using BG.Model;

namespace BG.View
{
    public partial class SelectedRoutine : System.Web.UI.Page
    {
        protected Routine routine;
        protected User user;
        protected List<Activity> Activities;
        protected List<Activity> AllActivities;
        private int routineId;
        protected void Page_Load(object sender, EventArgs e)
        {
            user = Session["LoggedInUser"] as User;
            var request = Convert.ToInt32(Request["routineId"]);
            if (request >-1)
            {
                routineId = Convert.ToInt32(request);
            }
            else if (Session["routineId"] != null)
            {
                routineId = Convert.ToInt32(Session["routineId"]);
            }
            routine = Service.Instance.GetRoutineFromCacheById(Convert.ToInt32(routineId));
            HeaderText.InnerText = routine.Name;
            
            if (Session["activities"] != null)
            {
                Activities = Session["activities"] as List<Activity>;
            }
            else
            {
                Activities = new List<Activity>();
            }


            GenreateCheckBoxes();
        }

        protected void ButtonAcceptRoutineClick(object sender, EventArgs args)
        {
            foreach (var control in divCheckbox.Controls)
            {
                if (control is CheckBox)
                {
                    var checkbox = control as CheckBox;
                    if (checkbox.Checked)
                    {
                        var id = (control as CheckBox).ID;

                        Activities.Add(routine.Activities.Single(a => a.Id == int.Parse(id)));
                    }
                }
            }
            if (Activities.Any())
            {
                Session["activities"] = Activities;
                Session["routineId"] = routine.Id;
                Response.Redirect("PerformedActivities.aspx");
            }
        }

        protected void ButtonDeclineRoutineClick(object sender, EventArgs args)
        {
            Session["LoggedInUser"] = user;
            Session["routineId"] = null;
            Session["activities"] = null;
            Response.Redirect("Routines.aspx");
        }

        public void GenreateCheckBoxes()
        {
            var availableActivities = routine.Activities.Where(activity => activity.IsUsed == false).ToList();
            Service.Instance.Container.SaveChanges();

            foreach (var activity in availableActivities)
            {
                var checkbox = new CheckBox();
                checkbox.ID = activity.Id + "";
                checkbox.Text = activity.EconomicProject.Name + "<br/>" + activity.EconomicTaskType.Name;

                divCheckbox.Controls.Add(checkbox);
            }
        }

        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);
            if (!IsPostBack)
            {
                var request = Request["routineId"];
                routine = Service.Instance.GetRoutineFromCacheById(Convert.ToInt32(request));
            }
        }

    }
}