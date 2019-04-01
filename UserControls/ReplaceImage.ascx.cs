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

namespace NFTACS.UserControls
{
    public partial class ReplaceImage : System.Web.UI.UserControl
    {
        #region clr public property
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
                ViewState["KeyValue"] = value.ToWSSafeString();;
            }
        }
        public String ImageField { get; set; }
        public String ServerImagePath {get; set;}
        public String HttpImagePath {get;set;}
        public String CurrentImageFile
        {
            get { return (String)ViewState["CurrentImageFile"] ?? String.Empty; }
            set {
                ViewState["CurrentImageFile"] = value.ToWSSafeString();
                //display current image
                if (String.IsNullOrWhiteSpace(value.ToWSSafeString()))
                {
                    lblCurrentPhoto.Text = "";
                }
                else
                {
                    if (!String.IsNullOrEmpty(HttpImagePath)) lblCurrentPhoto.Text = getImgTag(HttpImagePath, CurrentImageFile); else lblCurrentPhoto.Text = "";
                    imgCurrentPhoto.ImageUrl = getImgTag(HttpImagePath, CurrentImageFile, true);
                    
                }
            }
        }
        #endregion

        #region event
        public delegate void UploadCompletedDelegate(Object sender, String newFile);
        public event UploadCompletedDelegate UploadCompleted;
        #endregion

        
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack && !Page.IsCallback)
            {
                //set titel text
                if (!String.IsNullOrEmpty(TitleText)) lblPopupHeader.Text = TitleText;
            }
            lblError.Text = "";
            lblMessage.Text = "";
        }

        private String getImgTag(String basePath, String ImageFile, bool linkOnly = false) {
            String imgFullPath;
            if (ImageFile.StartsWith("http://", StringComparison.OrdinalIgnoreCase) || ImageFile.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                imgFullPath = ImageFile;
            }
            else
            {
                if (!basePath.EndsWith("/")) basePath += "/";
                imgFullPath = basePath + ImageFile;
            }
            String imgTag = String.Format("<img src=\"{0}\" alt=\"{1}\" class=\"thumb120\" border=\"0\">", imgFullPath, ImageFile);
            if (linkOnly) imgTag = imgFullPath;
            return imgTag;
        }

        protected void MyFile_UploadedComplete(object sender, AjaxControlToolkit.AsyncFileUploadEventArgs e)
        {
            //validate file
            if (!MyFile.HasFile)
            {
                lblError.Text = "Please select a picture file to upload.";
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
                    lblError.Text = "Sorry, the file you selected is not a known image type.";
                    return;
            }
            if (strFileName.IndexOf("'") >= 0 | strFileName.IndexOf("\"") >= 0)
            {
                lblError.Text = "Sorry, the file name is not valid. Please remove spacial characters (e.g. qoutes, double quotes) from the file name and try again.";
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
            String strUpdateCommand = String.Format("UPDATE [{0}] SET [{1}] = '{2}' WHERE [{3}] = {4}{5}{4}", TableName , ImageField, System.IO.Path.GetFileName(saveFileName), KeyField, KeyFieldType == KeyFieldTypeEnum.String ? "'" : "", KeyValue );
            OleDbCommand objCommand = new OleDbCommand(strUpdateCommand, Conn);
            objCommand.Connection.Open();
            objCommand.ExecuteNonQuery();
            System.Diagnostics.Debug.WriteLine("Record updated");

            //Save uploaded file to server
            try
            {
                //Upload the file
                MyFile.SaveAs(saveFileNameFullPath);
                lblMessage.Text = "Your File was Uploaded Sucessfully.";
                CurrentImageFile = System.IO.Path.GetFileName(saveFileName);
            }
            catch (Exception Exp)
            {
                lblError.Text = "OOPS! The server has encountered an error. Please check your file and try again.";
            }
            String js = String.Format("$(\"{0}\").attr(\"src\",\"{1}\")", imgCurrentPhoto.ClientID, getImgTag(HttpImagePath, CurrentImageFile, true));
            ScriptManager.RegisterClientScriptBlock(UpdatePanel1 ,UpdatePanel1.GetType(), "newimg", js, true);
            ajaxlabel.Text = js;
            UpdatePanel1.Update();
        }

    }
}
