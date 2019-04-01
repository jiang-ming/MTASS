using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using CrystalDecisions.Shared;
using CrystalDecisions.ReportSource;
using CrystalDecisions.CrystalReports.Engine;
using System.IO;
using System.Data;
using System.Data.OleDb;
using System.Configuration;

namespace MTASS.UserForms
{
    public partial class SurveyReport1 : System.Web.UI.Page
    {
        private ReportDocument m_RPT;
        private String m_rptFileName = "SurveyReport01.rpt";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsCallback && !Page.IsPostBack)
            {
                ExportPDF();
            }
        }
        protected void ExportPDF()
        {
            MemoryStream oStream = default(MemoryStream);
            try
            {

                //create report object
                m_RPT = new ReportDocument();
                m_RPT.Load(Server.MapPath("./" + m_rptFileName));
                DataSet ds = new DataSet();
                OleDbConnection conn = new OleDbConnection(ConfigurationManager.ConnectionStrings["_SiteAccess"].ConnectionString);
                OleDbDataAdapter pda1 = new OleDbDataAdapter("SELECT * FROM R_CONTROL_SUBCATEGORY_LIST", conn);
                pda1.Fill(ds, "R_CONTROL_SUBCATEGORY_LIST");
                OleDbDataAdapter pda2 = new OleDbDataAdapter("SELECT * FROM R_DATA0_REPORT", conn);
                pda2.Fill(ds, "R_DATA0_REPORT");
                OleDbDataAdapter pda3 = new OleDbDataAdapter("SELECT * FROM R_DATA1_REPORT", conn);
                pda3.Fill(ds, "R_DATA1_REPORT");
                m_RPT.SetDataSource(ds);

                m_RPT.SetParameterValue("ShowEquipmentInfo", true);
                m_RPT.SetParameterValue("ShowEquipmentPicture", false);
                m_RPT.SetParameterValue("ShowSafetySurvey", true);
                m_RPT.SetParameterValue("ShowSafetySurveyPicture", false);

                // Export the report
                oStream = (MemoryStream)m_RPT.ExportToStream(ExportFormatType.PortableDocFormat);
                Response.Clear();
                Response.Buffer = true;
                //Response.ContentType = "application/pdf";
                Response.AddHeader("Content-Disposition", "attachment; filename=Report.pdf");
                Response.BinaryWrite(oStream.ToArray());
                Response.Flush();
                Response.End();
            }
            catch (Exception err)
            {
                Response.Write("<BR>");
                Response.Write(err.Message.ToString());
            }
        }
    }
}