using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebMap.Web;
using System.Web.Security;
using System.Configuration;
using Wendel;
using System.IO;
using CrystalDecisions.Shared;
using CrystalDecisions.CrystalReports.Engine;
using System.Data;
using System.Data.SqlClient;
using System.Data.OleDb;


    public partial class PrintPDFReport : System.Web.UI.Page
    {
        private ReportDocument m_RPT;
        
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
            if (!Page.IsPostBack && !Page.IsCallback)
            {
                OneTimeInit();
            }
        }
        private void OneTimeInit()
        {
            //anything that get saved into session or viewstate can be put here.
            //all javascript initialization needs to be re-applied after every postback
            //Get query
            txtYr.Text = Request["yr"].ToWSSafeString();
            txtLoc.Text = Request["loc"].ToWSSafeString();
            txtType.Text = Request["type"].ToWSSafeString();
            
            //txtWDSurveyWorkSheet.Text = "17476";       
            CreateReport(txtYr.Text,txtLoc.Text,txtType.Text);
        }
        protected void CreateReport(string strYr, string strLoc, string strType)
        {

            MemoryStream oStream = default(MemoryStream);
            //OleDbConnection conn = new OleDbConnection(ConfigurationManager.ConnectionStrings["_SiteAccess"].ConnectionString);
            //string SQL = string.Format("Select * from R_Control_SubCategory_List Where Building_ID={0} ", strLoc);
            //OleDbDataAdapter da = new OleDbDataAdapter(SQL, conn);
            //DataSet ds = new DataSet();
            //da.Fill(ds, "R_Control_SubCategory_List");
            m_RPT = new ReportDocument();
            if (strType == "Full Report")
            {
                m_RPT.Load(Server.MapPath("./FormSource/") + "SurveyReport01.rpt");
            }
            else if (strType == "Open Item Report")
            {
                m_RPT.Load(Server.MapPath("./FormSource/") + "SurveyReport02.rpt");
            }
            //m_RPT.SetDataSource(ds);
            //m_RPT.SetParameterValue("ShowEquipmentInfo", true);
            m_RPT.SetParameterValue("ShowEquipmentPicture", true);
            m_RPT.SetParameterValue("ShowSafetySurvey", true);
            m_RPT.SetParameterValue("ShowSafetySurveyPicture", true);
            m_RPT.SetParameterValue("RptYear", strYr);
            m_RPT.RecordSelectionFormula =String.Format( "{{R_Control_SubCategory_List.Building_ID}} = {0}",strLoc);
            oStream = (MemoryStream)m_RPT.ExportToStream(ExportFormatType.PortableDocFormat);
            Response.Clear();
            Response.Buffer = true;
            Response.ContentType = "application/pdf;filename=\"Newfile.pdf\"";
            Response.BinaryWrite(oStream.ToArray());
            m_RPT.Close();
            Response.End();
            
        }
    }
