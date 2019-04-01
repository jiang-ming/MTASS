using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebMap.Web;

namespace MTASS.UserForms
{
    public partial class TransmitFile : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //DownloadFile();
        }

        protected void DownloadFile()
        {
            try
            {
                String FileNum = String.Empty;
                try { FileNum = AESUtil.DecryptString(Request["FILE"].ToWSSafeString()); } catch { FileNum = Request["FILE"].ToWSSafeString(); }
                String sFileToSend = Session["FILETOSEND" + FileNum].ToString();
                if (String.IsNullOrEmpty(sFileToSend) || !System.IO.File.Exists(sFileToSend))
                {
                    String js = "alert('Error. The specified file cannot be downloaded because it does not exist.');window.close();";
                    Page.ClientScript.RegisterClientScriptBlock(Page.GetType(), "FILESENDERROR", js, true);
                    return;
                }
                Response.ContentType = WebMap.Web.StringUtil.GetMimetype(System.IO.Path.GetExtension(sFileToSend));
                Response.Clear();
                Response.ClearContent();
                Response.ClearHeaders();
                Response.Buffer = true;
                Response.AppendHeader("Content-Disposition", "attachment; filename=" + System.IO.Path.GetFileName(sFileToSend));
                Response.AppendHeader("Content-Length", (new System.IO.FileInfo(sFileToSend)).Length.ToString());
                Response.TransmitFile(sFileToSend);
                Response.Flush();
                Context.ApplicationInstance.CompleteRequest();
            }
            catch (Exception)
            {
            }
        }

        protected void LinkButton1_Click(object sender, EventArgs e)
        {
            DownloadFile();
        }
    }
}