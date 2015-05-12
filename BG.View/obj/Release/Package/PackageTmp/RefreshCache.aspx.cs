using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BG.Controller;

namespace BG.View
{
    public partial class RefreshCache : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //Service.Instance.RecreateModelContainer();
            //Service.Instance.RefreshCache(24); //Sætter den tid der går imellem cachens opdateringer
            Response.Write(Service.Instance.Container.EconomicCacheSet.First().LastUpdated.ToLongTimeString());
        }
    }
}