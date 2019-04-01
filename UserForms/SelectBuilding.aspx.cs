using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebMap.Web;

namespace MTASS.UserForms
{
    public partial class SelectBuilding : System.Web.UI.Page
    {
        private String selectcategorypage = "SelectCategory.aspx";
        
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!PermissionHelper.bIsSessionOK())
            {
                return;
            }
            HttpContext.Current.Application["User" + HttpContext.Current.Session["SID"]] = HttpContext.Current.User.Identity.Name.ToString().ToUpper() + "|" + DateTime.Now.ToString();
            HttpContext.Current.Session["LastRequest"] = DateTime.Now;

        }

        protected void btnGo_Click(object sender, EventArgs e)
        {
            String buildingid = ddBuilding.SelectedValue.ToWSSafeString();
            Response.Redirect(String.Format("{0}?BuildingID={1}", selectcategorypage, AESUtil.EncryptString(buildingid)));
        }
    }
}