using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebMap.Web;
using System.Data;
using System.Configuration;

namespace MTASS.UserForms
{
    public partial class SelectCategory : System.Web.UI.Page
    {
        #region module variables
        private static String m_LookupTablesDBConnection = "_SiteAccess";
        private static String m_catTable = "Q_AFull_Item_List"; //"CategoryList";
        private static String m_field_sectionName = "SectionName";
        private static String m_field_mainCategory = "Main_Category";
        private static String m_field_subCategory = "Sub_Category";
        private static String selectsubcategorypage = "SelectSubCategory.aspx";
        private static String dataForm0 = "DataForm0.aspx";
        private static String dataForm1 = "DataForm1.aspx";
        private static Boolean collapseSection = true;
        private String previousSection = "***FIRST";

        protected String m_FeatureClassTable = "Data1_DataEntry";
        #endregion

        #region web method
        [System.Web.Services.WebMethod]
        public static String GenerateSubMenu(String buildingid, String section, String category)
        {
            String sm = "";
            String sql = String.Format("SELECT Q_SubCategory_List.SectionName, Q_SubCategory_List.Main_Category, Q_SubCategory_List.Sub_Category, IIf([Q_Data1_L2_Count].ct Is Null,0,[Q_Data1_L2_Count].ct) AS ct, IIf([Q_Data1_L2_Count_Failed].ct Is Null,0,[Q_Data1_L2_Count_Failed].ct) AS ctfailed, Q_SubCategory_List.DisplayOrder, QBDG.Building_ID, Q_SubCategory_List.DataForm, Q_SubCategory_List.ID FROM (Q_SubCategory_List LEFT JOIN (SELECT * FROM Q_Data1_L2_Count WHERE BUILDING_ID={0}) AS QBDG ON (Q_SubCategory_List.ID = QBDG.ID)) LEFT JOIN (SELECT * FROM Q_Data1_L2_Count_Failed WHERE BUILDING_ID={0}) AS QBDG2  ON (Q_SubCategory_List.ID = QBDG2.ID) WHERE Q_SubCategory_List.Main_Category='{2}' ORDER BY Q_SubCategory_List.DisplayOrder", buildingid, section.ToWSSafeString(), category.ToWSSafeString());
            DataTable dtItemList = DBUtil.GetOLEDataTable("_SiteAccess", sql, "itemlist");
            sm += "<ul data-role='listview' data-inset='true' data-filter='false' data-theme='f' >";
            foreach (DataRow dr in dtItemList.Rows)
            {
                String menulabel = GetCategoryLabel(dr[m_field_subCategory].ToWSSafeString());
                Int16 dataform = Convert.ToInt16(dr["DataForm"]);
                String scid = dr["ID"].ToWSSafeString();
                String linkurl = "";
                Int16 dataCount = Convert.ToInt16(dr["ct"]);
                Int16 failedCount = Convert.ToInt16(dr["ctfailed"]);
                switch (dataform)
                {
                    case 0:
                        linkurl = String.Format("{0}?BuildingID={1}&SCID={2}",
                            dataForm0, AESUtil.EncryptString(buildingid), AESUtil.EncryptString(scid));
                        string Form0Count = DBUtil.DoLookup(DBUtil.DBType.oledb, "_SiteAccess", "Building_ID", buildingid, DBUtil.FieldType.number, "ct", String.Format("SELECT * FROM [Q_Data0_L2_count] WHERE ID={0}", dr["ID"].ToWSSafeString()));
                        if (String.IsNullOrWhiteSpace(Form0Count)) dataCount = 0; else dataCount = Convert.ToInt16(Form0Count);
                        failedCount = 0;
                        break;
                    case 1:
                        linkurl = String.Format("{0}?BuildingID={1}&SCID={2}",
                            dataForm1, AESUtil.EncryptString(buildingid), AESUtil.EncryptString(scid));
                        break;
                    default:
                        linkurl = String.Format("{0}?BuildingID={1}&SCID={2}",
                            dataForm1, AESUtil.EncryptString(buildingid), AESUtil.EncryptString(scid));
                        break;
                }

                sm += "<li>";
                sm += String.Format("<a class='listbutton-no-ellipsis' href='{1}'>{0}</a>", menulabel, linkurl);
                if (failedCount > 0)
                {
                    sm += String.Format("<span class='ul-li-image-bubble-container' ><span class='ul-li-image-bubble-caption'>{0}</span></span>", failedCount);
                }
                sm += String.Format("<span class='ui-li-count'>{0}</span>", dataCount);
                sm += "</li>";
            }
            sm += "</ul>";
            return sm;
        }

        #endregion

        #region page events
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!PermissionHelper.bIsSessionOK())
            {
                return;
            }
            if (!Page.IsPostBack && !Page.IsCallback)
            {
                txtBuildingID.Text = AESUtil.DecryptString(Request["BuildingID"].ToWSSafeString());
                txtSectionExpanded.Text = AESUtil.DecryptString(Request["CFID"].ToWSSafeString()); //coming back from SelectSubCategory this is CategoryFirstOfID but if navigate through to edit form, when you come back this is actually SCID
                if (!String.IsNullOrWhiteSpace(txtSectionExpanded.Text))
                {
                    List<String> listInfo = DBUtil.DoLookupOLEDB(m_LookupTablesDBConnection, m_catTable, "ID", DBUtil.FieldTypeEnum2.Number, txtSectionExpanded.Text, new List<String>() { m_field_sectionName, m_field_mainCategory, "FirstOfID" });
                    txtSectionExpanded.Text = listInfo[0];
                    String sCatName = listInfo[1];
                    String sCFID = listInfo[2];
                    //function loadSubMenu(divid, firstid, _buildingid, _sectionname, _maincategory) {
                    String js = String.Format("loadSubMenu('{0}','{1}','{2}','{3}','{4}');scroolToDivID('{0}');", GetCategorySubMenuID(sCFID), sCFID, txtBuildingID.Text, txtSectionExpanded.Text, sCatName);
                    addJS("init-cat-expand", js, true);
                }

                lblBuildingName.Text = DBUtil.DoLookup("OLEDB", SqlFacility, "Building_ID", txtBuildingID.Text, "NUMBER", "LOCATION", "SELECT * FROM BUILDING");
                if (collapseSection)
                {
                    ltCollapseULBegin.Text = "<div data-role=\"collapsible-set\" data-theme=\"b\" data-content-theme=\"c\" >";
                    ltCollapseULEnd.Text = "</div>";
                }
                else
                {
                    ltCollapseULBegin.Text = "<ul data-role=\"listview\" data-inset=\"true\" data-filter=\"false\" data-filter-theme=\"false\">";
                    ltCollapseULEnd.Text = "</ul>";
                }
            }
            HttpContext.Current.Application["User" + HttpContext.Current.Session["SID"]] = HttpContext.Current.User.Identity.Name.ToString().ToUpper() + "|" + DateTime.Now.ToString();
            HttpContext.Current.Session["LastRequest"] = DateTime.Now;
        }
        #endregion

        #region control events
        protected void OnCategoryClick(Object sender, EventArgs e)
        {
            LinkButton btn = (LinkButton)sender;
            String sc = btn.Text.Trim();
            //String section = btn.CommandArgument;
            //Response.Redirect(String.Format("{0}?BuildingID={1}&Section={2}&Category={3}", selectsubcategorypage, AESUtil.EncryptString(txtBuildingID.Text), AESUtil.EncryptString(section), AESUtil.EncryptString(sc)));
            String CategoryFirstID = btn.CommandArgument;
            Response.Redirect(String.Format("{0}?BuildingID={1}&CFID={2}", selectsubcategorypage, AESUtil.EncryptString(txtBuildingID.Text), AESUtil.EncryptString(CategoryFirstID)));
        }
        protected void DateButton_Click(object sender, EventArgs e)
        {
            //update database
            txtBuildingID.Text = AESUtil.DecryptString(Request["BuildingID"].ToWSSafeString()); //viewstate is disabled, need to update this on postback
            String fld = "INSP" + ConfigurationManager.AppSettings["InspectionYear"].ToWSSafeString();
            String sSql = String.Format("UPDATE [{0}] SET [{1}] = DATE() WHERE [BUILDING_ID]={2} AND [{1}] IS NULL", m_FeatureClassTable, fld, txtBuildingID.Text);
            DBUtil.ConnExecute(DBUtil.DBConnType.Oledb, m_LookupTablesDBConnection, sSql);
            Response.Redirect(Request.RawUrl);
        }
        #endregion

        #region supporting routines
        protected String GetDivider(object section)
        {
            String sc = section.ToWSSafeString();
            if (sc.Equals(previousSection))
            {
                //same section as previous record - do nothing
                return String.Empty;
            }

            if (collapseSection)
            {
                String closeSection = String.Empty;
                if (!previousSection.Equals("***FIRST"))
                {
                    //close previous section
                    closeSection = "\n\t\t\t\t\t</ul>\n\t\t\t\t</div>";
                }
                previousSection = sc;

                String defaultExpand = string.Empty;
                if (txtSectionExpanded.Text.ToUpper().Trim().Equals(sc.ToUpper().Trim()))
                {
                    defaultExpand = " data-collapsed=\"false\" ";
                }

                //start new section
                return String.Format("{0}\n\t\t\t\t<div data-role=\"collapsible\" {2}>\n\t\t\t\t\t<h3>{1}</h3>\n\t\t\t\t\t<ul data-role=\"listview\" data-filter=\"false\" data-filter-theme=\"c\" >", closeSection, sc, defaultExpand);
            }
            else
            {
                previousSection = sc;
                return String.Format("\n\t\t<li data-role=\"list-divider\">{0}</li>", sc);
            }
        }
        protected static String GetCategoryLabel(object category)
        {
            String sc = category.ToWSSafeString();
            if (String.IsNullOrWhiteSpace(sc))
            {
                return "**Undefined**";
            }
            //if (txtBuildingID.Text == "2" && sc =="Maintenance Equipment")
            //{
            //    return "Communications and Systems Equipment";
            //}
            //else
            //{
            //    return sc;
            //}
            return sc;
        }
        protected String GetCategorySubMenuID(object FirstOfID)
        {
            String sc = FirstOfID.ToWSSafeString();
            return "divSubMenu" + sc;
        }
        protected String GetRecordCount(object category, int count)
        {
            return count.ToString();
            //String ct = String.Empty;
            //switch (category.ToWSSafeString().ToUpper())
            //{
            //    case "ELEVATORS":
            //        ct = DBUtil.DoLookup(DBUtil.DBType.oledb, "_SiteAccess", "Building_ID", txtBuildingID.Text, DBUtil.FieldType.number, "ct", "SELECT * FROM [Q_Bldg_Elevator_count]");
            //        if (String.IsNullOrWhiteSpace(ct)) ct = "0";
            //        return ct;
            //    case "MAINTENANCE EQUIPMENT":
            //        ct = DBUtil.DoLookup(DBUtil.DBType.oledb, "_SiteAccess", "Building_ID", txtBuildingID.Text, DBUtil.FieldType.number, "ct", "SELECT * FROM [Q_Bldg_meq_count]");
            //        if (String.IsNullOrWhiteSpace(ct)) ct = "0";
            //        return ct;
            //    default:
            //        return count.ToString();
            //}
        }
        protected String GetErrorBubble(int count)
        {
            if (count == 0)
            {
                return "";
            }
            else
            {
                return String.Format("<span class='ul-li-image-bubble-container'><span class='ul-li-image-bubble-caption'>{0}</span></span>", count);
            }

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
        #endregion
    }
}