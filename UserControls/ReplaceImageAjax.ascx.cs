//The asp page containing this control cannot be hosted in a greyout iframe as ReplaceImageAjax control is interfering with the iframe and will cause the top level page to be replaced with this page after post-back.
//This page needs to be a pop-up window on its own
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Configuration;
using WebMap.Web;

public partial class ReplaceImageAjax : System.Web.UI.UserControl
{
    #region ***clr public property
    public String ButtonText { get; set; }
    public String TitleText { get; set; }
    public String ConnectionName { get; set; }
    public String TableName { get; set; }
    public String KeyField { get; set; }
    public enum KeyFieldTypeEnum
    {
        Numeric,
        String
    }
    public KeyFieldTypeEnum KeyFieldType { get; set; }
    public String KeyValue
    {
        get { return (String)ViewState["KeyValue"] ?? String.Empty; }
        set
        {
            ViewState["KeyValue"] = value.ToWSSafeString(); ;
        }
    }
    public String ImageField { get; set; }
    public String ServerImagePath { get; set; }
    public String WebConfigImagePath { get; set; }
    public String WebConfigReducedImagePath { get; set; }
    public int reducedImageMaxWidth { get; set; }
    public int reducedImageMaxHeight { get; set; }
    public String HttpImagePath { get; set; }
    public String CurrentImageFile
    {
        //ajax file upload is executed in an iFrame created by .NET - the only way to communicate with MyFile_UploadedComplete is through session state. ViewState and hidden field on page will not work
        get { return (String)Session[lblCurrentPhoto.ClientID] ?? String.Empty; }
        set
        {
            Session[lblCurrentPhoto.ClientID] = value.ToWSSafeString();
            string btnDeletePhotoID = UpdatePanel1.ClientID + "_deletephoto";
            //display current image
            if (String.IsNullOrWhiteSpace(value.ToWSSafeString()))
            {
                lblCurrentPhoto.Text = "";
                String js = String.Format("$(\"#{0}\").hide();", btnDeletePhotoID);
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), UpdatePanel1.ClientID + "_btn_delete", js, true);
            }
            else
            {
                if (!String.IsNullOrEmpty(ServerImagePath)) lblCurrentPhoto.Text = getImgTag(ServerImagePath, CurrentImageFile); else lblCurrentPhoto.Text = CurrentImageFile + " (cannot locate file)";
                String js = String.Format("$(\"#{0}\").show();", btnDeletePhotoID);
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), UpdatePanel1.ClientID + "_btn_delete", js, true);
            }
        }
    }
    public String LabelIDContainingPicture { get; set; }
    #endregion

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!String.IsNullOrEmpty(TitleText)) lblPopupHeader.Text = TitleText;
        lblError.Text = "";
        lblMessage.Text = "";
    }

    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);
        DisableImageUpload();
    }

    private void DisableImageUpload()
    {
        if (String.IsNullOrWhiteSpace(KeyValue))
        {
            /* 
                *  if KeyValue is null (or blank) it means we are still adding the record.
                *  at this stage the record does not have MEQUID so we cannot upload the picture 
                *  we need to disable the upload function while user is still adding a new record
                */
            String btnPopupID = UpdatePanel1.ClientID + "_btn_popup";
            String btnDisableID = UpdatePanel1.ClientID + "_disabled_msg";
            String js = String.Format("$(\"#{0}\").hide();$(\"#{1}\").show();", btnPopupID, btnDisableID);
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), UpdatePanel1.ClientID + "_btn_popup_disable", js, true);
        }
    }

    private String getImgTag(String basePath, String ImageFile, bool linkOnly = false)
    {
        String imgFullPath;
        if (ImageFile.StartsWith("http://", StringComparison.OrdinalIgnoreCase) || ImageFile.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
        {
            imgFullPath = ImageFile;
        }
        else
        {
            //if (!basePath.EndsWith("/")) basePath += "/";
            //imgFullPath = basePath + ImageFile;
            imgFullPath = String.Format("./TransmitImgSrc.ashx?wcFolder={2}&imageID={0}&orgFileName={1}&thumb=true", ImageFile, ImageFile, WebConfigImagePath);
        }
        String imgTag = String.Format("<img src=\"{0}\" alt=\"{1}\" class=\"thumb120\" border=\"0\">", imgFullPath, ImageFile);
        if (linkOnly) imgTag = imgFullPath;
        return imgTag;
    }

    protected void MyFile_UploadedComplete(object sender, AjaxControlToolkit.AsyncFileUploadEventArgs e)
    {
        Session["uploadError"] = String.Empty;
        Session["uploadMessage"] = String.Empty;
        //validate file
        if (e.State != AjaxControlToolkit.AsyncFileUploadState.Success || !MyFile.HasFile)
        {
            //lblError.Text = "Please select a picture file to upload.";
            Session["uploadError"] = "Please select a picture file to upload.";
            return;
        }

        //Grab the file name from its fully qualified path at client 
        String strFileFullPathName = MyFile.PostedFile.FileName;

        // only the attched file name not its path
        String strFileName = System.IO.Path.GetFileName(strFileFullPathName);
        String strFileExt = System.IO.Path.GetExtension(strFileName);
        switch (strFileExt.ToUpper())
        {
            case ".PNG":
            case ".JPG":
            case ".JPEG":
            case ".JP2":
            case ".TIF":
            case ".TIFF":
            case ".GIF":
            case ".BMP":
                break;
            default:
                //lblError.Text = "Sorry, the file you selected is not a known image type.";
                Session["uploadError"] = "Sorry, the file you selected is not a known image type.";

                return;
        }
        if (strFileName.IndexOf("'") >= 0 | strFileName.IndexOf("\"") >= 0)
        {
            //lblError.Text = "Sorry, the file name is not valid. Please remove spacial characters (e.g. qoutes, double quotes) from the file name and try again.";
            Session["uploadError"] = "Sorry, the file name is not valid. Please remove spacial characters (e.g. qoutes, double quotes) from the file name and try again.";
            return;
        }

        System.Diagnostics.Debug.WriteLine("Document ext: " + strFileExt);

        //Calculating Save File name
        //Tablename_ObjectID_RunningNumber
        String baseFileName = TableName + "_" + KeyValue;
        if (!ServerImagePath.EndsWith(@"\")) ServerImagePath += @"\";
        if (!HttpImagePath.EndsWith("/")) HttpImagePath += "/";
        String saveFileName = baseFileName + strFileExt;
        String saveFileNameFullPath = ServerImagePath + saveFileName;
        int fileRunning = 0;
        while (System.IO.File.Exists(saveFileNameFullPath))
        {
            fileRunning++;
            saveFileName = baseFileName + "-" + fileRunning.ToString() + strFileExt;
            saveFileNameFullPath = ServerImagePath + saveFileName;
        }

        //Update data table
        OleDbConnection Conn = new OleDbConnection(ConfigurationManager.ConnectionStrings[ConnectionName].ConnectionString);
        String strUpdateCommand = String.Format("UPDATE [{0}] SET [{1}] = '{2}' WHERE [{3}] = {4}{5}{4}", TableName, ImageField, System.IO.Path.GetFileName(saveFileName), KeyField, KeyFieldType == KeyFieldTypeEnum.String ? "'" : "", KeyValue);
        OleDbCommand objCommand = new OleDbCommand(strUpdateCommand, Conn);
        objCommand.Connection.Open();
        objCommand.ExecuteNonQuery();
        System.Diagnostics.Debug.WriteLine("Record updated");

        //Save uploaded file to server
        try
        {
            //Upload the file
            MyFile.SaveAs(saveFileNameFullPath);
            Session["uploadMessage"] = "Your File was Uploaded Successfully.";
            CurrentImageFile = System.IO.Path.GetFileName(saveFileName);
            if (!String.IsNullOrWhiteSpace(WebConfigReducedImagePath))
            {
                //saveFileNameFullPath = ServerImagePath + saveFileName;
                ImageUtil.resizeImage_caller(saveFileName, WebConfigImagePath, WebConfigReducedImagePath, reducedImageMaxWidth, reducedImageMaxHeight);
            }
        }
        catch (Exception Exp)
        {
            //lblError.Text = "OOPS! The server has encountered an error. Please check your file and try again.";
            Session["uploadError"] = "OOPS! The server has encountered an error. Please check your file and try again.";
        }

        /*
        *   update to controls on the page will not work as MyFile_UploadedComplete event is fired in an iframe. 
        *   
        *   put any update you want to do in the Button1_click below
        * 
        */

    }

    protected void btnRefresh_Click(object sender, EventArgs e)
    {
        lblError.Text = Session["uploadError"].ToWSSafeString();
        lblMessage.Text = Session["uploadMessage"].ToWSSafeString();
        //update photo on pop-up page
        CurrentImageFile = CurrentImageFile.ToString(); // get/set will force lblCurrentPhoto to update
        //update photo on detailsview
        Label lblpic = (Label)FindControlRecursive(Parent, LabelIDContainingPicture);
        String js = String.Format("$('#{0}')[0].innerHTML = '{1}'; {2}_reposition();", lblpic.ClientID, CreateLinkThumbnail(CurrentImageFile, CurrentImageFile), UpdatePanel1.ClientID);
        ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "parentdetailsviewimg", js, true);
    }
    protected void btnDelete_Click(object sender, EventArgs e)
    {
        //though we don't have to use session to send info over to btn_refresh_click but we are doing it to accommodate ajax file-upload workflow 
        OleDbConnection Conn = new OleDbConnection(ConfigurationManager.ConnectionStrings[ConnectionName].ConnectionString);
        String strUpdateCommand = String.Format("UPDATE [{0}] SET [{1}] = '{2}' WHERE [{3}] = {4}{5}{4}", TableName, ImageField, String.Empty, KeyField, KeyFieldType == KeyFieldTypeEnum.String ? "'" : "", KeyValue);
        OleDbCommand objCommand = new OleDbCommand(strUpdateCommand, Conn);
        objCommand.Connection.Open();
        objCommand.ExecuteNonQuery();
        System.Diagnostics.Debug.WriteLine("Record updated");
        CurrentImageFile = ""; //this remove photo in pop-up upload panel
        //remove photo in main detailsview
        Label lblpic = (Label)FindControlRecursive(Parent, LabelIDContainingPicture);
        String js = String.Format("$('#{0}')[0].innerHTML = '{1}';", lblpic.ClientID, CreateLinkThumbnail(CurrentImageFile, CurrentImageFile));
        ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "parentdetailsviewimg", js, true);
    }

    #region ***parent detailsview update
    //photo
    protected String m_AttachmentPath = ConfigurationManager.AppSettings["PhotoPath"];
    protected String m_AttachmentVPath = ConfigurationManager.AppSettings["PhotoPath"];
    public static Control FindControlRecursive(Control Root, string Id)
    {
        if (Root.ID == Id)
            return Root;
        foreach (Control Ctl in Root.Controls)
        {
            Control FoundCtl = FindControlRecursive(Ctl, Id);
            if (FoundCtl != null)
                return FoundCtl;
        }
        return null;
    }
    public string CreateLinkThumbnail(object strFileNameObj, object strCaptionObj) //if copying this code from userforms, replace ' with \\' for all javascript content output
    {
        string strFileName = strFileNameObj.ToWSSafeString();
        if (string.IsNullOrWhiteSpace(strFileName))
        {
            return "";
        }
        string strCaption = strCaptionObj.ToWSSafeString();
        string sFullLinkPathName = string.Empty;
        if (strFileName.StartsWith("http://", StringComparison.CurrentCultureIgnoreCase) || strFileName.StartsWith("https://", StringComparison.CurrentCultureIgnoreCase))
        {
            //file name contain complete hyperlink, discard virtual directory
            sFullLinkPathName = strFileName;
        }
        else if (strFileName.Contains(":\\") || strFileName.StartsWith("\\\\"))
        {
            //full path to document file is specified in the database (either local or UNC path, use TransmitFile)
            sFullLinkPathName = strFileName;
            int iFileCount = (new WendelWMA.WMA_Utility.SetupTransmitFile()).SetupNextSendFile(sFullLinkPathName);
            sFullLinkPathName = string.Format("../UserForms/TransmitFile.aspx?FILE={0}", iFileCount);
        }
        else if (m_AttachmentVPath.StartsWith("\\\\"))
        {
            if (strFileName.IndexOf(".JPG", StringComparison.CurrentCultureIgnoreCase) > 0 ||
                strFileName.IndexOf(".JPEG", StringComparison.CurrentCultureIgnoreCase) > 0 ||
                strFileName.IndexOf(".GIF", StringComparison.CurrentCultureIgnoreCase) > 0 ||
                strFileName.IndexOf(".BMP", StringComparison.CurrentCultureIgnoreCase) > 0 ||
                strFileName.IndexOf(".PNG", StringComparison.CurrentCultureIgnoreCase) > 0 ||
                strFileName.IndexOf(".JP2", StringComparison.CurrentCultureIgnoreCase) > 0)
            {
                sFullLinkPathName = String.Format("./TransmitImgSrc.ashx?wcFolder=PhotoPath&imageID={0}&orgFileName={1}", strFileName, strCaption);
            }
            else
            {
                sFullLinkPathName = System.IO.Path.Combine(m_AttachmentPath, strFileName);
                int iFileCount = (new WendelWMA.WMA_Utility.SetupTransmitFile()).SetupNextSendFile(sFullLinkPathName);
                sFullLinkPathName = string.Format("../UserForms/TransmitFile.aspx?FILE={0}", iFileCount);
            }
        }
        else
        {
            sFullLinkPathName = m_AttachmentVPath + strFileName;
        }

        string sR = "";
        string strDisplayText = "";
        if (strFileName.IndexOf(".JPG", StringComparison.CurrentCultureIgnoreCase) > 0 ||
            strFileName.IndexOf(".JPEG", StringComparison.CurrentCultureIgnoreCase) > 0 ||
            strFileName.IndexOf(".GIF", StringComparison.CurrentCultureIgnoreCase) > 0 ||
            strFileName.IndexOf(".BMP", StringComparison.CurrentCultureIgnoreCase) > 0 ||
            strFileName.IndexOf(".PNG", StringComparison.CurrentCultureIgnoreCase) > 0 ||
            strFileName.IndexOf(".JP2", StringComparison.CurrentCultureIgnoreCase) > 0)
        {
            String contentPreviewUrl = String.Format("./TransmitImgSrc.ashx?wcFolder=PhotoPath&imageID={0}&orgFileName={1}&thumb=true", strFileName, strCaption);
            strDisplayText = String.Format("<img src=\"{0}\" alt=\"{1}\" class=\"thumb120\" border=\"0\" onMouseOver=\"window.status=\\'{2}\\';return true;\" onMouseOut=\"window.status=\\'\\';return true;\">", contentPreviewUrl, strCaption, strCaption.Replace(" ", ""));
        }
        else if (strFileName.IndexOf(".WMA", StringComparison.CurrentCultureIgnoreCase) > 0 || strFileName.IndexOf(".MP3", StringComparison.CurrentCultureIgnoreCase) > 0 || strFileName.IndexOf(".WAV", StringComparison.CurrentCultureIgnoreCase) > 0)
        {
            strDisplayText = string.Format("<img alt=\"{0}\" src=\"../images/Wendel/Sound.png\" border=\"0\" onMouseOver=\"window.status=\\'{1}\\';return true;\" onMouseOut=\"window.status=\\'\\';return true;\">", strCaption, strCaption.Replace(" ", ""));
        }
        else
        {
            strDisplayText = "View...";
        }

        sR = string.Format("<a alt=\"{0}\" href=\"{1}\" target=\"_new\" onMouseOver=\"window.status=\\'{3}\\';return true;\" onMouseOut=\"window.status=\\'\\';return true;\">{2}</a>", strCaption, sFullLinkPathName, strDisplayText, strCaption.Replace(" ", ""));
        return sR;
    }
    #endregion

}
