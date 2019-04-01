using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebMap.Web;

namespace MTASS.UserForms
{
    public partial class SelectSubCategory : System.Web.UI.Page
    {
        private String m_LookupTablesDBConnection = "_SiteAccess";
        private String m_catTable = "CategoryList";
        private String m_field_sectionName = "SectionName";
        private String m_field_mainCategory = "Main_Category";
        private String m_field_subCategory = "Sub_Category";
        private String selectCategoryPage = "SelectCategory.aspx";
        private String dataForm0 = "DataForm0.aspx";
        private String dataForm1 = "DataForm1.aspx";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!PermissionHelper.bIsSessionOK())
            {
                return;
            }
            if (!Page.IsPostBack && !Page.IsCallback)
            {
                txtBuildingID.Text = AESUtil.DecryptString(Request["BuildingID"].ToWSSafeString());
                txtCategoryFirstID.Text = AESUtil.DecryptString(Request["CFID"].ToWSSafeString());
                List<String> listInfo = DBUtil.DoLookupOLEDB(m_LookupTablesDBConnection, m_catTable, "ID", DBUtil.FieldTypeEnum2.Number, txtCategoryFirstID.Text, new List<String>() { m_field_sectionName, m_field_mainCategory });
                txtSection.Text = listInfo[0];
                txtCategory.Text = listInfo[1];

                lblBuildingName.Text = DBUtil.DoLookup("OLEDB", SqlFacility, "Building_ID", txtBuildingID.Text, "NUMBER", "LOCATION", "SELECT * FROM BUILDING");
                lblSection.Text = txtSection.Text;
                lblCategory.Text = txtCategory.Text;
            }
            HttpContext.Current.Application["User" + HttpContext.Current.Session["SID"]] = HttpContext.Current.User.Identity.Name.ToString().ToUpper() + "|" + DateTime.Now.ToString();
            HttpContext.Current.Session["LastRequest"] = DateTime.Now;
            lnkSelectCategory.DataBind();
            SqlFacility.Selected += new SqlDataSourceStatusEventHandler(SqlFacility_Selected);
        }

        void SqlFacility_Selected(object sender, SqlDataSourceStatusEventArgs e)
        {
            string sql = SqlFacility.SelectCommand;
        }

        protected String GetSubCatetoryLabel(object subCategory)
        {
            String sc = subCategory.ToWSSafeString();
            if (String.IsNullOrWhiteSpace(sc))
            {
                return "**Undefined**";
            }
            //custom handler
            //else if (txtBuildingID.Text == "2" && sc =="Maintenance Equipment")
            //{
            //    return "Communications and Systems Equipment";
            //}
            else
            {
                return sc;
                //return String.Format("<p>{0}</p>", sc);
            }

        }

        protected String GetRecordCount(object subCategory, int count, int dataform)
        {
            String sc = subCategory.ToWSSafeString();
            String ct = String.Empty;
            switch (dataform)
            {
                case 0:
                    ct = DBUtil.DoLookup(DBUtil.DBType.oledb, "_SiteAccess", "Building_ID", txtBuildingID.Text, DBUtil.FieldType.number, "ct", String.Format("SELECT * FROM [Q_Data0_L2_count] WHERE Main_Category='{0}' AND Sub_Category='{1}'",txtCategory.Text,sc));
                    if (String.IsNullOrWhiteSpace(ct)) ct = "0";
                    return ct;
                case 1:
                    return count.ToString();
                default:
                    return count.ToString();
            }
        }

        protected String getButtonParameter(int subCatID, int dataform)
        {
            return subCatID.ToWSSafeString() + "|" + dataform.ToWSSafeString();
        }

        protected void OnCategoryClick(Object sender, EventArgs e) 
        {
            LinkButton btn = (LinkButton)sender;
            String sc = btn.Text.Trim();
            String[] argarray = btn.CommandArgument.Split('|');
            String subCatID = argarray[0];
            int dataform = Convert.ToInt32(argarray[1]);
            switch (dataform)
            {
                case 0:
                    Response.Redirect(String.Format("{0}?BuildingID={1}&SCID={2}",
                        dataForm0, AESUtil.EncryptString(txtBuildingID.Text), AESUtil.EncryptString(subCatID)));
                    break;
                case 1:
                    Response.Redirect(String.Format("{0}?BuildingID={1}&SCID={2}",
                        dataForm1, AESUtil.EncryptString(txtBuildingID.Text), AESUtil.EncryptString(subCatID)));
                    break;
                default:
                    Response.Redirect(String.Format("{0}?BuildingID={1}&SCID={2}",
                        dataForm1, AESUtil.EncryptString(txtBuildingID.Text), AESUtil.EncryptString(subCatID)));
                    break;
            }
        }

        protected String GetSelectCatLink()
        {
            return String.Format("{0}?BuildingID={1}&CFID={2}", selectCategoryPage, AESUtil.EncryptString(txtBuildingID.Text), AESUtil.EncryptString(txtCategoryFirstID.Text));
        }
    }
}