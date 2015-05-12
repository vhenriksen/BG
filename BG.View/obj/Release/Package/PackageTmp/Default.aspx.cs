using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BG.Controller;
using System.Diagnostics;

namespace BG.View
{
    public partial class Default : System.Web.UI.Page
    {
        private IService service;
        protected int routineId;

        protected void Page_Load(object sender, EventArgs e)
        {
            service = Service.Instance;
            
            //service.CreateTestData();
           

            if(Session["LoggedInUser"] != null)
            {
                Response.Redirect("Home.aspx");
            }

        }


        protected void ButtonLogin_Click(object sender, EventArgs e)
        {
            var username = TextBoxUsername.Text;
            var password = TextBoxPassword.Text;

            if (service.UserExists(username, password))
            {
                var user = Service.Instance.GetUserFromCache(username, password);

                Session["LoggedInUser"] = user;

                var cookie = new HttpCookie("Username");
                cookie.Value = username;
                cookie.Expires = DateTime.Now.AddDays(30);
                Response.Cookies.Add(cookie);

                cookie = new HttpCookie("Password");
                cookie.Value = password;
                cookie.Expires = DateTime.Now.AddDays(30);
                Response.Cookies.Add(cookie);

                cookie = new HttpCookie("ID");
                cookie.Value = user.Id + "";
                cookie.Expires = DateTime.Now.AddDays(30);
                Response.Cookies.Add(cookie);
                
                Response.Redirect("Home.aspx");
            
            }
            else
            {

                LabelLoginError.Text = "Your username or password is invalid!";
                LabelLoginError.Visible = true;

            }

        }
        
    }
}