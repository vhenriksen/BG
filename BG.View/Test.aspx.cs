using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BG.Controller;
using Economic.Api;

namespace BG.View
{
    public partial class Test : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Write("Test loaded\r\n<br/>");
            var economicSession = new EconomicSession();
            economicSession.Connect(227765, "sol", "u2rmbt2v");
            var allProjects = Service.Instance.GetTaskTypesFromEconomic();
            foreach(var project in allProjects)
            {
                Response.Write(project.Name + "\r\n<br/>");
            }
            Response.Write("Test done\r\n<br/>");
        }
    }
}