using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebMap.Web;
using System.Data;

namespace MTASS.UserForms
{
    public partial class DataForm1 : System.Web.UI.Page
    {
        private String m_LookupTablesDBConnection = "_SiteAccess";
        private String m_catTable = "CategoryList";
        private String m_field_sectionName = "SectionName";
        private String m_field_mainCategory = "Main_Category";
        private String m_field_subCategory = "Sub_Category";
        
        //private String selectSubCategorypage = "SelectSubCategory.aspx";
        private String selectSubCategorypage = "SelectCategory.aspx";
        private String editpage = "DataForm1Edit.aspx";
        private String AddNewKey = "AddNew";

        //Lookup Table Declaration
        DataTable m_ddSqlDomainIntegrity;
        DataTable m_ddSqlDomainCondition;

        protected override void OnPreLoad(EventArgs e)
        {
            base.OnPreLoad(e);
            //Cache Lookup Table on page preload
            //***This must be inserted before you access the DetailsView including DetailsView.Rows.Count
            if (!IsPostBack)
            {
                m_ddSqlDomainIntegrity = DBUtil.GetOLEDataTable(m_LookupTablesDBConnection, SqlDomainIntegrity, "ddSqlDomainIntegrity");
                m_ddSqlDomainCondition = DBUtil.GetOLEDataTable(m_LookupTablesDBConnection, SqlDomainCondition, "ddSqlDomainCondition");
            }
            else
            {
                m_ddSqlDomainIntegrity = Session["ddSqlDomainIntegrity"] as DataTable;
                m_ddSqlDomainCondition = Session["ddSqlDomainCondition"] as DataTable;
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

                lblBuildingName.Text = DBUtil.DoLookup(DBUtil.DBType.oledb, SqlFacility, "Building_ID", txtBuildingID.Text, DBUtil.FieldType.number, "LOCATION", "SELECT * FROM BUILDING");
                lblSection.Text = txtSection.Text;
                lblCategory.Text = txtCategory.Text;
            }
            HttpContext.Current.Application["User" + HttpContext.Current.Session["SID"]] = HttpContext.Current.User.Identity.Name.ToString().ToUpper() + "|" + DateTime.Now.ToString();
            HttpContext.Current.Session["LastRequest"] = DateTime.Now;
            lnkSelectSubCategory.DataBind();
            ltNoData.Visible = true;
            ltNoData.Text = String.Format("<p class=\"ui-body ui-body-c message\"><strong>No data entered for {0}. Please use the Add New Record button below to add a new record.</strong></p>", txtSubCategory.Text.ToWSTitleCase().Replace("Fire Code","<a href='../PDF/FireCode.pdf' target='_blank'>Fire Code</a>"));
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
            //btnAddNew.Visible = false;
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

        protected String GetSubtypeDisplay(object subtypedesc, object _customtype, object _datecreated)
        {
            String subtype = subtypedesc.ToWSSafeString();
            String customtype = _customtype.ToWSSafeString();
            DateTime datecreated = DateTime.Now;
            try { datecreated = Convert.ToDateTime(_datecreated); } catch (Exception) { }
            //if (subtype.ToUpper() == "N/A")
            //{
            //    return String.Format("{0} ({1})",txtSubCategory.Text, datecreated.Year.ToString());
            //}
            //else if (subtype.ToUpper() == "CUSTOM TYPE" && !String.IsNullOrWhiteSpace(customtype))
            //{
            //    return String.Format("{0} ({1})", customtype, datecreated.Year.ToString());
            //}
            //else return String.Format("{0} ({1})", subtype, datecreated.Year.ToString());
            return String.Format("{0}", subtype, datecreated.Year.ToString());
        }

        protected String GetShortNote(object note)
        {
            return note.ToWSSafeString().GetTextMaxLength(75);
        }

        protected String DoLookupSqlDomainIntegrity(object objKey)
        {
            try
            {
                DataView DV = new DataView(m_ddSqlDomainIntegrity, String.Format("CODE={0}", objKey), "CODE", DataViewRowState.CurrentRows);
                if (DV.Count > 0)
                {
                    if (objKey.ToWSSafeString() == "2" || objKey.ToWSSafeString() == "3" || objKey.ToWSSafeString() == "4")
                    {
                        return DV[0]["DESCRIP"].ToString() + "<img class='ul-li-image-bubble' src='../Images/SmallMenuIcon/Exclamation-32.png' alt='exclamation' />";

                    }
                    else
                    {
                        return DV[0]["DESCRIP"].ToString();
                    }
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
    }
}