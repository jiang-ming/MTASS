using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebMap.Web;
using System.Data.OleDb;
using System.Data;
using System.Text;
using System.Configuration;
using Wendel;
using System.IO;
using CrystalDecisions.Shared;
using CrystalDecisions.CrystalReports.Engine;
namespace MTASS.UserForms
{
    public partial class PDFReport : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!PermissionHelper.bIsSessionOK())
            {
                return;
            }
            if (!PermissionHelper.IsUserDbAllowEdit())
            {
                Response.Redirect("~/Default.aspx");
                return;
            }
        }
        
        protected void btnPrint_Click(object sender, EventArgs e)
        {
            string strYear = txtYear.Text;
            String strLocation = ddLocation.SelectedValue.ToWSSafeString();
            string strReportType = ddReportType.SelectedValue.ToWSSafeString();
            //lbltest.Text = strLocation + strReportType + strYear;
            addJS("p1", String.Format("window.open('PrintPDFReport.aspx?yr={0}&loc={1}&type={2}','_blank');",strYear,strLocation,strReportType));
            //addJS("PrintPDFReport", String.Format("popNormalWindow('PrintPDFReport.aspx?yr={0}&loc={1}&type={2}','_blank');",strYear,strLocation,strReportType));
            //addJS("PrintPDFReport", String.Format("popNormalWindow('PrintPDFReport.aspx?yr={0}&loc={1}&type={2}','_blank2');", strYear, strLocation, "Full Report"));
        }
       

        protected void btnPrintAll_Click(object sender, EventArgs e)
        {
            string strYear = txtYear.Text;
            addJS("p1", String.Format("window.open('PrintPDFReport.aspx?yr={0}&loc=1&type=Full Report','_blank1');", strYear));
            addJS("p2", String.Format("window.open('PrintPDFReport.aspx?yr={0}&loc=2&type=Full Report','_blank2');", strYear));
            addJS("p3", String.Format("window.open('PrintPDFReport.aspx?yr={0}&loc=3&type=Full Report','_blank3');", strYear));
            addJS("p4", String.Format("window.open('PrintPDFReport.aspx?yr={0}&loc=4&type=Full Report','_blank4');", strYear));
            addJS("p5", String.Format("window.open('PrintPDFReport.aspx?yr={0}&loc=5&type=Full Report','_blank5');", strYear));
            addJS("p6", String.Format("window.open('PrintPDFReport.aspx?yr={0}&loc=6&type=Full Report','_blank6');", strYear));
            addJS("p7", String.Format("window.open('PrintPDFReport.aspx?yr={0}&loc=1&type=Open Item Report','_blank7');", strYear));
            addJS("p8", String.Format("window.open('PrintPDFReport.aspx?yr={0}&loc=2&type=Open Item Report','_blank8');", strYear));
            addJS("p9", String.Format("window.open('PrintPDFReport.aspx?yr={0}&loc=3&type=Open Item Report','_blank9');", strYear));
            addJS("p10", String.Format("window.open('PrintPDFReport.aspx?yr={0}&loc=4&type=Open Item Report','_blank10');", strYear));
            addJS("p11", String.Format("window.open('PrintPDFReport.aspx?yr={0}&loc=5&type=Open Item Report','_blank11');", strYear));
            addJS("p12", String.Format("window.open('PrintPDFReport.aspx?yr={0}&loc=6&type=Open Item Report','_blank12');", strYear));
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