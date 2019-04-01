using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebMap.Web;

namespace MTASS.UserForms
{
    public partial class DataForm0 : System.Web.UI.Page
    {
        private String m_LookupTablesDBConnection = "_SiteAccess";
        private String m_catTable = "CategoryList";
        private String m_field_sectionName = "SectionName";
        private String m_field_mainCategory = "Main_Category";
        private String m_field_subCategory = "Sub_Category";

        //private String selectSubCategorypage = "SelectSubCategory.aspx";
        private String selectSubCategorypage = "SelectCategory.aspx";
        private String editpage = "DataForm0Edit.aspx";
        private String AddNewKey = "AddNew";

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

                lblBuildingName.Text = DBUtil.DoLookup(DBUtil.DBType.oledb, SqlFacility, "Building_ID", txtBuildingID.Text, DBUtil.FieldType.number, "LOCATION", "SELECT * FROM BUILDING");
                lblSection.Text = txtSection.Text;
                lblCategory.Text = txtCategory.Text;
            }
            HttpContext.Current.Application["User" + HttpContext.Current.Session["SID"]] = HttpContext.Current.User.Identity.Name.ToString().ToUpper() + "|" + DateTime.Now.ToString();
            HttpContext.Current.Session["LastRequest"] = DateTime.Now;
            lnkSelectSubCategory.DataBind();
            ltNoData.Visible = true;
            ltNoData.Text = String.Format("<p class=\"ui-body ui-body-c message\"><strong>No data entered for {0}. Please use the Add New Record button below to add a new record.</strong></p>", txtSubCategory.Text.ToWSTitleCase());
        }

        protected void btnAddNew_Click(object sender, EventArgs e)
        {
            Response.Redirect(String.Format("{0}?EntryUID={1}&BuildingID={2}&SCID={3}", 
                editpage, AESUtil.EncryptString(AddNewKey),
                    AESUtil.EncryptString(txtBuildingID.Text), AESUtil.EncryptString(txtSubCatID.Text)));
        }

        protected void Repeater1_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            ltNoData.Visible = false;
        }

        protected void Repeater1_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            String cname = e.CommandName.ToWSSafeString();
            String carg = e.CommandArgument.ToWSSafeString();
            if (cname.ToUpper() == "ViewEdit".ToUpper())
            {
                Response.Redirect(String.Format("{0}?EntryUID={1}&BuildingID={2}&SCID={3}", 
                    editpage, AESUtil.EncryptString(carg),
                    AESUtil.EncryptString(txtBuildingID.Text), AESUtil.EncryptString(txtSubCatID.Text)));
            }
        }

        protected String GetSelectSubCatLink()
        {
            return String.Format("{0}?BuildingID={1}&CFID={2}", selectSubCategorypage, AESUtil.EncryptString(txtBuildingID.Text), AESUtil.EncryptString(txtSubCatID.Text));
        }

        protected String GetSubtypeDisplay(object _subtype, object _datecreated)
        {
            String subtype = _subtype.ToWSSafeString();
            DateTime datecreated = DateTime.Now;
            try { datecreated = Convert.ToDateTime(_datecreated); } catch (Exception) { }
            return String.Format("{0}", subtype, datecreated.Year.ToString());
        }
    }
}