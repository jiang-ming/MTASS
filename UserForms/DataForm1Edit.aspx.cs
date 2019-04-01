using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebMap.Web;
using System.Data.OleDb;
using System.Data;
using System.Configuration;

namespace MTASS.UserForms
{
    public partial class DataForm1Edit : System.Web.UI.Page
    {
        #region ***module variables
        protected String m_LookupTablesDBConnection = "_SiteAccess";
        private String m_catTable = "CategoryList";
        private String m_field_sectionName = "SectionName";
        private String m_field_mainCategory = "Main_Category";
        private String m_field_subCategory = "Sub_Category";
        
        private String AddNewKey = "AddNew";
        protected String m_FeatureClassTable = "Data1_DataEntry";
        private String m_InspectorField = "LoginInitial";
        private String m_FeatureClassTimeStampField = "[TIMESTAMP]";

        private String dataForm1 = "DataForm1.aspx";

        //Lookup Table Declaration
        DataTable m_ddSqlDomainIntegrity;
        DataTable m_ddSqlDomainCondition;
        DataTable m_ddSqlDomainClassification;
        DataTable m_ddSqlDomainResponsibleParty;
        DataTable m_ddSqlDomainPriority;

        //photo
        protected String m_AttachmentPath = ConfigurationManager.AppSettings["PhotoPath"];
        #endregion

        #region ***SqlDataSource query
        protected string sqlFeatureUpdateCommand()
        {
            //this sub-routine read field lists from sqlDataSource update parameters. 
            //Parameter names are case-sensitive and must match those used by Bind or Eval statement in GridView / DetailsView
            string sSql = "";
            sSql += string.Format("UPDATE [{0}] SET ", m_FeatureClassTable.ToUpper());
            bool bFirst = true;
            if (!string.IsNullOrEmpty(m_FeatureClassTimeStampField))
            {
                if (bFirst) bFirst = false; else sSql += ", ";
                //sSql += string.Format("{0} = GETDATE()", m_FeatureClassTimeStampField);
                sSql += string.Format("{0} = Now()", m_FeatureClassTimeStampField);
            }
            if (!string.IsNullOrEmpty(m_InspectorField))
            {
                if (bFirst) bFirst = false; else sSql += ", ";
                sSql += string.Format("{0} = '{1}'", m_InspectorField, User.Identity.Name.ToWSSafeString());
            }

            //if (bFirst) bFirst = false; else sSql += ", ";
            //String fld = "INSP" + ConfigurationManager.AppSettings["InspectionYear"].ToWSSafeString();
            //sSql += string.Format("[{0}]=Date()", fld);

            for (int i = 0; i <= SqlFacility.UpdateParameters.Count - 1; i++)
            {
                Parameter uPrm = SqlFacility.UpdateParameters[i];
                if (i < SqlFacility.UpdateParameters.Count - 1)
                {
                    if (bFirst) bFirst = false; else sSql += ", ";
                    sSql += string.Format("{0} = @{0}", uPrm.Name);
                }
                else
                {
                    //last parameter, this must be an update key
                    sSql += string.Format(" WHERE {0}=@{0}", uPrm.Name);
                }
            }
            return sSql;
        }
        protected string sqlFeatureInsertCommand()
        {
            //this sub-routine read field lists from sqlDataSource insert parameters. 
            //Parameter names are case-sensitive and must match those used by Bind or Eval statement in GridView / DetailsView

            String FieldList = String.Empty;
            String ValueList = String.Empty;
            bool bFirst = true;

            //time stamp
            if (!String.IsNullOrEmpty(m_FeatureClassTimeStampField))
            {
                if (bFirst) bFirst = false; else { FieldList += ","; ValueList += ","; }
                FieldList += m_FeatureClassTimeStampField;
                //ValueList += "GETDATE()";
                ValueList += "Now()";
            }

            //inspector
            if (!String.IsNullOrEmpty(m_InspectorField))
            {
                if (bFirst) bFirst = false; else { FieldList += ","; ValueList += ","; }
                FieldList += m_InspectorField;
                ValueList += String.Format("'{0}'", User.Identity.Name.ToWSSafeString());
            }

            //date inspected
            if (bFirst) bFirst = false; else { FieldList += ","; ValueList += ","; }
            String fld = "INSP" + ConfigurationManager.AppSettings["InspectionYear"].ToWSSafeString();
            FieldList += fld;
            ValueList += "Date()";

            //building id
            if (bFirst) bFirst = false; else { FieldList += ","; ValueList += ","; }
            FieldList += "Building_ID";
            String bldgID = String.Empty;
            if (txtBuildingID == null || String.IsNullOrWhiteSpace(txtBuildingID.Text)) bldgID = AESUtil.DecryptString(Request["BuildingID"].ToWSSafeString()); else bldgID = txtBuildingID.Text;
            ValueList += String.Format("{0}", bldgID);

            //sub-category id
            if (bFirst) bFirst = false; else { FieldList += ","; ValueList += ","; }
            FieldList += "Sub_Category_ID";
            String subCatID = String.Empty;
            if (txtSubCatID == null || String.IsNullOrWhiteSpace(txtSubCatID.Text)) subCatID = AESUtil.DecryptString(Request["SCID"].ToWSSafeString()); else subCatID = txtSubCatID.Text;
            ValueList += String.Format("{0}", subCatID);

            //all other fields
            for (int i = 0; i <= SqlFacility.InsertParameters.Count - 1; i++)
            {
                Parameter uPrm = SqlFacility.InsertParameters[i];
                if (bFirst) bFirst = false; else { FieldList += ","; ValueList += ","; }
                FieldList += uPrm.Name;
                ValueList += String.Format("@{0}", uPrm.Name);
            }

            string sSql = String.Format("INSERT INTO [{0}] ({1}) VALUES ({2})", m_FeatureClassTable.ToUpper(), FieldList, ValueList);
            return sSql;
        }
        #endregion

        #region ***Page events
        protected override void OnPreLoad(EventArgs e)
        {
            base.OnPreLoad(e);
            //Cache Lookup Table on page preload
            //***This must be inserted before you access the DetailsView including DetailsView.Rows.Count
            if (!IsPostBack)
            {
                m_ddSqlDomainIntegrity = DBUtil.GetOLEDataTable(m_LookupTablesDBConnection, SqlDomainIntegrity, "ddSqlDomainIntegrity");
                m_ddSqlDomainCondition = DBUtil.GetOLEDataTable(m_LookupTablesDBConnection, SqlDomainCondition, "ddSqlDomainCondition");
                m_ddSqlDomainResponsibleParty = DBUtil.GetOLEDataTable(m_LookupTablesDBConnection, SqlDomainResponsibleParty, "ddSqlDomainResponsibleParty");
                m_ddSqlDomainClassification = DBUtil.GetOLEDataTable(m_LookupTablesDBConnection, SqlDomainClassification, "ddSqlDomainClassification");
                m_ddSqlDomainPriority = DBUtil.GetOLEDataTable(m_LookupTablesDBConnection, SqlDomainPriority, "ddSqlDomainPriority");
            }
            else
            {
                m_ddSqlDomainIntegrity = Session["ddSqlDomainIntegrity"] as DataTable;
                m_ddSqlDomainCondition = Session["ddSqlDomainCondition"] as DataTable;
                m_ddSqlDomainResponsibleParty = Session["ddSqlDomainResponsibleParty"] as DataTable;
                m_ddSqlDomainClassification = Session["ddSqlDomainClassification"] as DataTable;
                m_ddSqlDomainPriority = Session["ddSqlDomainPriority"] as DataTable;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!PermissionHelper.bIsSessionOK())
            {
                return;
            }
            if (!Page.IsPostBack && !Page.IsCallback)
            {
                txtBuildingID.Text = AESUtil.DecryptString(Request["BuildingID"].ToWSSafeString());
                txtSubCatID.Text = AESUtil.DecryptString(Request["SCID"].ToWSSafeString());
                List<String> listInfo = DBUtil.DoLookupOLEDB(m_LookupTablesDBConnection, m_catTable, "ID", DBUtil.FieldTypeEnum2.Number, txtSubCatID.Text, new List<String>() { m_field_sectionName, m_field_mainCategory, m_field_subCategory });
                txtSection.Text = listInfo[0];
                txtCategory.Text = listInfo[1];
                txtSubCategory.Text = listInfo[2];
                String sEntryUid = AESUtil.DecryptString(Request["EntryUID"].ToWSSafeString());
                txtInspector.Text = User.Identity.Name.ToWSSafeString();

                lblBuildingName.Text = DBUtil.DoLookup(DBUtil.DBType.oledb, SqlFacility, "Building_ID", txtBuildingID.Text, DBUtil.FieldType.number, "LOCATION", "SELECT * FROM BUILDING");
                lblSection.Text = txtSection.Text;
                lblCategory.Text = txtCategory.Text;
                lblSubcategory.Text = txtSubCategory.Text.Replace("Fire Code","<a href='../PDF/FireCode.pdf' target='_blank' style='color:white;'>Fire Code</a>");
                txtEntryUID.Text = sEntryUid == AddNewKey ? "-1" : sEntryUid;
                lblMessage.Text = sEntryUid == AddNewKey ? "Adding New Record" : "Viewing Record"; 

            }
            HttpContext.Current.Application["User" + HttpContext.Current.Session["SID"]] = HttpContext.Current.User.Identity.Name.ToString().ToUpper() + "|" + DateTime.Now.ToString();
            HttpContext.Current.Session["LastRequest"] = DateTime.Now;

            if (txtEntryUID.Text == "-1")
            {
                if (IsEditEnabled())
                {
                    DetailsView1.ChangeMode(DetailsViewMode.Insert);
                }
                else
                {
                    lblMessage.Text = "Sorry, your user account does not have permission to edit the data.";
                }
            }

            if (!IsEditEnabled()) addJS("jsIsEditEnabled", "var jsIsEditEnabled = false;");
            else addJS("jsIsEditEnabled", "var jsIsEditEnabled = true;");
            lnkBack.DataBind();
        }
        #endregion

        #region ***DetailsView event
        protected void DetailsView1_ItemCommand(object sender, DetailsViewCommandEventArgs e)
        {
            string cmd = e.CommandName.ToUpper();
            if (cmd=="CANCEL" && DetailsView1.CurrentMode == DetailsViewMode.Insert)
            {
                //cancel insert --> navigate back to previous page
                Response.Redirect(GetBackLink());
            }
            else if (cmd == "CANCEL")
            {
                lblMessage.Text = "Change Cancelled";
            }
            else if (cmd == "DELETE")
            {
                //move photo file
                List<String> info = DBUtil.DoLookupOLEDB(m_LookupTablesDBConnection, m_FeatureClassTable, "ENTRYUID", DBUtil.FieldTypeEnum2.Number, txtEntryUID.Text, new List<string>() { "Picture1", "Picture2", "Picture3" });
                movePhoto(info[0].ToWSSafeString());
                movePhoto(info[1].ToWSSafeString());
                movePhoto(info[2].ToWSSafeString());
                string originalPhoto = m_FeatureClassTable + "_" + txtEntryUID.Text + ".*";
                string[] photoset1 = System.IO.Directory.GetFiles(m_AttachmentPath, originalPhoto);
                foreach (string fi1 in photoset1)
                {
                    movePhoto(System.IO.Path.GetFileName(fi1));
                }
                string allOtherPhotos = m_FeatureClassTable + "_" + txtEntryUID.Text + "-*.*";
                string[] photoset2 = System.IO.Directory.GetFiles(m_AttachmentPath, allOtherPhotos);
                foreach (string fi1 in photoset2)
                {
                    movePhoto(System.IO.Path.GetFileName(fi1));
                }

                lblMessage.Text = "Record Deleted";
            }
        }
        protected void DetailsView1_ItemInserted(object sender, DetailsViewInsertedEventArgs e)
        {
            lblMessage.Text = "Record Added";
        }
        protected void DetailsView1_ItemUpdated(object sender, DetailsViewUpdatedEventArgs e)
        {
            lblMessage.Text = "Record Saved";
        }
        protected void DetailsView1_ModeChanging(object sender, DetailsViewModeEventArgs e)
        {
            if (!IsEditEnabled() && e.NewMode == DetailsViewMode.Edit)
            {
                e.Cancel = true;
                lblMessage.Text = "Sorry, your user account does not have permission to edit the data.";
            }
        }
        protected void DetailsView1_ModeChanged(object sender, EventArgs e)
        {
            if ((sender as DetailsView).CurrentMode == DetailsViewMode.Edit)
            {
                lblMessage.Text = "Editing Record";
            }
        }
        #endregion

        #region ***sql datasource event
        protected void SqlFacility_Inserted(object sender, SqlDataSourceStatusEventArgs e)
        {
            String query = "SELECT @@IDENTITY";
            OleDbCommand cmd = new OleDbCommand(query, (OleDbConnection)e.Command.Connection);
            Int32 newid = (Int32)cmd.ExecuteScalar();
            cmd.Connection.Close();
            txtEntryUID.Text = newid.ToWSSafeString();
        }
        #endregion

        #region ***Supporting routines / utilities
        protected String DoLookupSqlDomainIntegrity(object objKey)
        {
            try
            {
                DataView DV = new DataView(m_ddSqlDomainIntegrity, String.Format("CODE={0}", objKey), "CODE", DataViewRowState.CurrentRows);
                if (DV.Count > 0)
                {
                    return DV[0]["DESCRIP"].ToString();
                }
                else
                {
                    return "";
                }
            }
            catch (Exception)
            {
                return "";
            }
        }
        protected String DoLookupSqlDomainCondition(object objKey)
        {
            try
            {
                DataView DV = new DataView(m_ddSqlDomainCondition, String.Format("CODE={0}", objKey), "CODE", DataViewRowState.CurrentRows);
                if (DV.Count > 0)
                {
                    return DV[0]["DESCRIP"].ToString();
                }
                else
                {
                    return "";
                }
            }
            catch (Exception)
            {
                return "";
            }
        }
        protected String DoLookupSqlDomainResponsibleParty(object objKey)
        {
            try
            {
                DataView DV = new DataView(m_ddSqlDomainResponsibleParty, String.Format("CODE={0}", objKey), "CODE", DataViewRowState.CurrentRows);
                if (DV.Count > 0)
                {
                    return DV[0]["DESCRIP"].ToString();
                }
                else
                {
                    return "";
                }
            }
            catch (Exception)
            {
                return "";
            }
        }
        protected String DoLookupSqlDomainClassification(object objKey)
        {
            try
            {
                DataView DV = new DataView(m_ddSqlDomainClassification, String.Format("CODE={0}", objKey), "CODE", DataViewRowState.CurrentRows);
                if (DV.Count > 0)
                {
                    return DV[0]["DESCRIP"].ToString();
                }
                else
                {
                    return "";
                }
            }
            catch (Exception)
            {
                return "";
            }
        }
        protected String DoLookupSqlDomainPriority(object objKey)
        {
            try
            {
                DataView DV = new DataView(m_ddSqlDomainPriority, String.Format("CODE={0}", objKey), "CODE", DataViewRowState.CurrentRows);
                if (DV.Count > 0)
                {
                    return DV[0]["DESCRIP"].ToString();
                }
                else
                {
                    return "";
                }
            }
            catch (Exception)
            {
                return "";
            }
        }
        protected String GetBackLink()
        {
            return String.Format("{0}?BuildingID={1}&SCID={2}",
                 dataForm1, AESUtil.EncryptString(txtBuildingID.Text), AESUtil.EncryptString(txtSubCatID.Text));
        }
        protected String GetBackLinkCaption()
        {
            //return txtSubCategory.Text.ToWSTitleCase().ToWSShortenText(13,2);
            return "Item List";
        }
        protected String GetHideFieldInjector()
        {
            //custom code
            //use javascript to hide some input fields depending on category and sub-category
            //inject the required javascript and return blank string

            //String cat = txtCategory.Text;
            //String subcat = txtSubCategory.Text;
            //String subtype = DBUtil.DoLookup(DBUtil.DBType.oledb, "_SiteAccess", "Main_Category", cat, DBUtil.FieldType.text, "Sub_type", "SELECT * FROM DomainBuildingFeatures");
            //if (subtype.ToUpper() == "N/A")
            //{
            //    String jsHideInput = "$('#hideFieldsContainer').hide();";
            //    ScriptManager.RegisterStartupScript(this, this.GetType(), "hideinputfields", jsHideInput, true);
            //}
            //if (cat.ToUpper() == "ROOF")
            //{
            //    String jsShowInstallDate = "$('#installed_date_container').show();";
            //    ScriptManager.RegisterStartupScript(this, this.GetType(), "showinstalldate", jsShowInstallDate, true);
            //}

            return "";
        }
        public string CreateLinkThumbnail(object strFileNameObj, object strCaptionObj)
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
            else if (m_AttachmentPath.StartsWith("\\\\"))
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
                sFullLinkPathName = m_AttachmentPath + strFileName;
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
                strDisplayText = String.Format("<img src=\"{0}\" alt=\"{1}\" class=\"thumb120\" border=\"0\" onMouseOver=\"window.status='{2}';return true;\" onMouseOut=\"window.status='';return true;\">", contentPreviewUrl, strCaption, strCaption.Replace(" ", ""));
            }
            else if (strFileName.IndexOf(".WMA", StringComparison.CurrentCultureIgnoreCase) > 0 || strFileName.IndexOf(".MP3", StringComparison.CurrentCultureIgnoreCase) > 0 || strFileName.IndexOf(".WAV", StringComparison.CurrentCultureIgnoreCase) > 0)
            {
                strDisplayText = string.Format("<img alt=\"{0}\" src=\"../images/Wendel/Sound.png\" border=\"0\" onMouseOver=\"window.status='{1}';return true;\" onMouseOut=\"window.status='';return true;\">", strCaption, strCaption.Replace(" ", ""));
            }
            else
            {
                strDisplayText = "View...";
            }

            sR = string.Format("<a alt=\"{0}\" href=\"{1}\" target=\"_new\" onMouseOver=\"window.status='{3}';return true;\" onMouseOut=\"window.status='';return true;\">{2}</a>", strCaption, sFullLinkPathName, strDisplayText, strCaption.Replace(" ", ""));
            return sR;
        }
        protected String gvstr(Object val)
        {
            return val.ToWSSafeString();
        }
        protected void movePhoto(string filename)
        {
            string movefrom1 = m_AttachmentPath.CheckAppendEnd(@"\") + filename;
            string moveto1 = m_AttachmentPath.CheckAppendEnd(@"\") + @"_Deleted\" + filename;
            try { System.IO.File.Move(movefrom1, moveto1); } catch (Exception) { }
            string movefrom2 = m_AttachmentPath.CheckAppendEnd(@"\") + @"reduced\" + filename;
            string moveto2 = m_AttachmentPath.CheckAppendEnd(@"\") + @"reduced\_Deleted\" + filename;
            try { System.IO.File.Move(movefrom2, moveto2); } catch (Exception) { }
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
        protected static Control FindControlIterative(Control root, string id)
        {
            Control ctl = root;
            LinkedList<Control> ctls = new LinkedList<Control>();


            while (ctl != null)
            {
                if (ctl.ID == id)
                    return ctl;
                foreach (Control child in ctl.Controls)
                {
                    if (child.ID == id)
                        return child;
                    if (child.HasControls())
                        ctls.AddLast(child);
                }
                ctl = ctls.First.Value;
                ctls.Remove(ctl);
            }
            return null;
        }
        private void JSInitVar(String varName, String varValue)
        {
            ScriptManager.RegisterClientScriptBlock(this, Page.GetType(), varName, String.Format("var {0} = '{1}';", varName, varValue), true);
        }
        private void JSInitVar(String varName, Int32 varValue)
        {
            ScriptManager.RegisterClientScriptBlock(this, Page.GetType(), varName, String.Format("var {0} = {1};", varName, varValue), true);
        }
        private void JSInitVar(String varName, Double varValue)
        {
            ScriptManager.RegisterClientScriptBlock(this, Page.GetType(), varName, String.Format("var {0} = {1};", varName, varValue), true);
        }
        private void JSAssignVar(String varName, String varValue)
        {
            ScriptManager.RegisterClientScriptBlock(this, Page.GetType(), varName, String.Format("{0} = '{1}';", varName, varValue), true);
        }
        private void JSAssignVar(String varName, Int32 varValue)
        {
            ScriptManager.RegisterClientScriptBlock(this, Page.GetType(), varName, String.Format("{0} = {1};", varName, varValue), true);
        }
        private void JSAssignVar(String varName, Double varValue)
        {
            ScriptManager.RegisterClientScriptBlock(this, Page.GetType(), varName, String.Format("{0} = {1};", varName, varValue), true);
        }
        #endregion

        //========================================================================================================================
        #region user account / permission
        //========================================================================================================================
        protected bool CheckEditPermissionOK()
        {
            //If Session("UserDB_EditHydrant") Then
            try
            {
                if ((bool)Session["AllowEdit"])
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
        protected bool IsEditEnabled()
        {
            return CheckEditPermissionOK();
        }
        #endregion
    }
}