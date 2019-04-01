using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;
using WebMap.Web;
using System.IO;
using System.Data.OleDb;
using System.Data;
using System.Configuration;
using CrystalDecisions.Shared;
using CrystalDecisions.CrystalReports.Engine;
namespace MTASS
{
    public partial class Default : System.Web.UI.Page
    {
        private ReportDocument m_RPT;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!PermissionHelper.bIsSessionOK())
            {
                return;
            }
            if (PermissionHelper.IsUserDbAllowEdit())
            {
                addJS("jsPrBtn", "var jsHidePrBtn = false;", false);
            }
            else
            {
                addJS("jsPrBtn", "var jsHidePrBtn = true;",false);
            }
            HttpContext.Current.Application["User" + HttpContext.Current.Session["SID"]] = HttpContext.Current.User.Identity.Name.ToString().ToUpper() + "|" + DateTime.Now.ToString();
            HttpContext.Current.Session["LastRequest"] = DateTime.Now;
        }

        protected void lnkLogout_Click(object sender, EventArgs e)
        {
            FormsAuthentication.SignOut();
            Response.Redirect(".");
        }

        protected void lnkReport_Click(object sender, EventArgs e)
        {

            MemoryStream oStream = default(MemoryStream);
            OleDbConnection conn = new OleDbConnection(ConfigurationManager.ConnectionStrings["_SiteAccess"].ConnectionString);
            string SQL = string.Format("Select * from R_Control_SubCategory_List Where Building_ID=1");
            OleDbDataAdapter da = new OleDbDataAdapter(SQL, conn);
            DataSet ds = new DataSet();
            da.Fill(ds, "R_Control_SubCategory_List");
            m_RPT = new ReportDocument();
            m_RPT.Load(Server.MapPath("./UserForms/FormSource/") + "SurveyReport01.rpt");
            m_RPT.SetDataSource(ds);
            //m_RPT.SetParameterValue("R_Control_SubCategory_List.Building_ID", Convert.ToInt32("1"));
            m_RPT.SetParameterValue("R_Control_SubCategory_List.Building_ID", Convert.ToInt32("1"));
            oStream = (MemoryStream)m_RPT.ExportToStream(ExportFormatType.PortableDocFormat);
            Response.Clear();
            Response.Buffer = true;
            Response.ContentType = "application/pdf";
            Response.BinaryWrite(oStream.ToArray());
            Response.End();
        }
        private void addJS(String key, String js, Boolean waitJQueryReady = false)
        {
            if (!waitJQueryReady)
            {
                ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), key, js, true);
            }
            else
            {
                String jsJQ = String.Format(" $(document).ready(function () {{ {0} }});", js);
                ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), key, jsJQ, true);
            }
        }
    }
}