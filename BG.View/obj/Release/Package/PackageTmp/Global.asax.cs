using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using BG.Controller;
using BG.Model;
using System.Diagnostics;

namespace BG.View
{
    public class Global : System.Web.HttpApplication
    {
        private static Timer timer;

        protected void Application_Start(object sender, EventArgs e)
        {
            //timer = new Timer(new TimerCallback(TimerTick), null, 60000, 60000);
        }

        private void TimerTick(object state)
        {
            //Service.Instance.RefreshCache(5);
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {


            if (Request.Cookies["ID"] != null)
            {

                var id = Request.Cookies["ID"] + "";
                var username = Request.Cookies["Username"] + "";
                var password = Request.Cookies["Password"] + "";

                if (Service.Instance.UserExists(username, password))
                {

                    var user = Service.Instance.GetUserFromCache(username, password);
                    if (user.Id + "" == id)
                    {

                        Session["LoggedInUser"] = user;

                    }

                }


            }
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}