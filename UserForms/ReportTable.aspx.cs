using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebMap.Web;
using System.Data.OleDb;
using System.Data;
using System.Text;
using System.Configuration;
using Wendel;

namespace MTASS.UserForms
{
    public partial class ReportTable : System.Web.UI.Page
    {
        #region ***module variables
        protected String m_LookupTablesDBConnection = "_SiteAccess";
        protected String m_FeatureClassTable = "Data1_DataEntry";
        //private String m_InspectorField = "LoginInitial";
        //private String m_FeatureClassTimeStampField = "[TIMESTAMP]";
        private String editpage = "DataForm1Edit.aspx";

        private long HitCount;
        private long lngTotalRecs;
        //Lookup Table Declaration
        #endregion

        #region Form events
        protected override void OnPreLoad(EventArgs e)
        {
            base.OnPreLoad(e);
            //Cache Lookup Table on page preload
            //***This must be inserted before you access the DetailsView including DetailsView.Rows.Count
            if (!IsPostBack)
            {
            }
            else
            {
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!PermissionHelper.bIsSessionOK())
            {
                return;
            }
            HttpContext.Current.Application["User" + HttpContext.Current.Session["SID"]] = HttpContext.Current.User.Identity.Name.ToString().ToUpper() + "|" + DateTime.Now.ToString();
            HttpContext.Current.Session["LastRequest"] = DateTime.Now;

            if (!Page.IsPostBack && !Page.IsCallback)
            {
                OneTimeInit();
            }

            EverytimeInit();

            HitCount = 0;
            SqlFacility.Selected += new SqlDataSourceStatusEventHandler(SqlFacility_Selected);
            ddCI.DataBound += new EventHandler(ddCI_DataBound);
            ddLocation.DataBound += new EventHandler(ddLocation_DataBound);
            checkColList();
        }

        private void OneTimeInit()
        {
            ViewState["gridmode"] = GridViewPageModeEnum.Paging;
            ViewState["filter"] = "";
            List<string> lstColHeaders= new List<string>();
            if (GridView1.Columns.Count > 0) {
                foreach (DataControlField dcf in GridView1.Columns)
                {
                    string strheader = dcf.HeaderText;
                    
                    ListItem lii = new ListItem(strheader,strheader,true);
                    if (dcf.Visible)
                    {
                        lstColHeaders.Add(strheader);
                        lii.Selected = true;
                    }
                    ckblCol.Items.Add(lii);
                }
                Session["selectedColHeaders"] = lstColHeaders;
            }
        }
        private void EverytimeInit()
        {
            //inject JS --> show loading animation when form is being processed
            //....we removed ShowLoading from OnClientClick events on the aspx page
            //....the code is added here (hijacking asp.net ValidatorOnSubmit) to make sure that ShowLoading() is called only if page is successfully submitted.
            string jsname = "OnSubmitScriptif (!Page.ClientScript.IsOnSubmitStatementRegistered(jstype, jsname))";
            string jstext = "";
            jstext += "if (typeof(ValidatorOnSubmit) == 'function' && ValidatorOnSubmit() == false)return false;" + Environment.NewLine;
            jstext += "else" + Environment.NewLine;
            jstext += "{" + Environment.NewLine;
            jstext += "  ShowLoading();" + Environment.NewLine;
            jstext += "}" + Environment.NewLine;
            jstext += "return true;" + Environment.NewLine;
            Page.ClientScript.RegisterOnSubmitStatement(Page.GetType(), jsname, jstext);

            SqlFacility.FilterExpression = ViewState["filter"].ToWSSafeString();            
        }
        public override void VerifyRenderingInServerForm(Control control)
        {
            /* Verifies that the control is rendered */
        }
        #endregion

        #region control events
        protected void ddCI_DataBound(object sender, EventArgs e)
        {
            if (!Page.IsPostBack && !Page.IsCallback)
            {
                //set default query
                ddCI.SelectedValue = "*"; // "-1";
                ProcessQF();
            }
        }
        protected void ddLocation_DataBound(object sender, EventArgs e)
        {
            if (!Page.IsPostBack && !Page.IsCallback)
            {
                //set default query
                ddLocation.SelectedIndex = 1;
                ProcessQF();
            }
        }
        protected void btnXLS_Click(object sender, System.EventArgs e)
        {
            ProcessQF();
            GridView1.ExportToXLS(HttpContext.Current, "DataReviewExport");
        }
        protected void btnSwitch_Click(object sender, System.EventArgs e)
        {
            ProcessQF();
        }
        #endregion

        #region gridview
        protected void GridView1_DataBound(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //fresh page loaded, possibly forwarding back from Edit.aspx, check if we should jump to a specific page
                if (!(Session["ViewingPage"] == null))
                {
                    if (GridView1.PageCount >= ((int)Session["ViewingPage"] + 1))
                    {
                        GridView1.PageIndex = (int)Session["ViewingPage"];
                    }
                    else
                    {
                        GridView1.PageIndex = (int)Session["ViewingPage"] - 1;
                        Session["ViewingPage"] = (int)Session["ViewingPage"] - 1;
                    }
                }
            }

            if (GridView1.Rows.Count > 0)
            {
                long lngFrom = GridView1.PageIndex * GridView1.PageSize + 1;
                long lngTo = lngFrom + GridView1.Rows.Count - 1;
                if (GridView1.AllowPaging)
                {
                    lngFrom = GridView1.PageIndex * GridView1.PageSize + 1;
                    lngTo = lngFrom + GridView1.Rows.Count - 1;
                    //get row count
                    DataView pDataView = (DataView)SqlFacility.Select(new DataSourceSelectArguments());
                    lngTotalRecs = pDataView.Count;
                    //this returns num of rows after filter
                    //Dim pDataTable As DataTable = pDataView.Table
                    //lngTotalRecs = pDataTable.Rows.Count   'This returns num of all rows before filter
                }
                else
                {
                    lngFrom = 1;
                    lngTo = lngFrom + GridView1.Rows.Count - 1;
                    lngTotalRecs = GridView1.Rows.Count;
                }
                lblRecordCount.Text = "Viewing Records " + lngFrom + "-" + lngTo + " of " + lngTotalRecs;
            }
        }
        protected void GridView1_PageIndexChanged(object sender, System.EventArgs e)
        {
            Session["ViewingPage"] = GridView1.PageIndex;
        }
        protected void setCellSize(TableCell tc)
        {
            tc.Style.Add("Width", "240");
            tc.Style.Add("Height", "180");
        }
        protected void GridView1_RowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
        {

            if ((e.Row.RowType == DataControlRowType.DataRow))
            {
                e.Row.Attributes.Add("onmouseover", "this.style.backgroundColor='Silver'");
                //This will be the back ground color of the GridView Control
                if (e.Row.RowState == DataControlRowState.Alternate)
                {
                    e.Row.Attributes.Add("onmouseout", "this.style.backgroundColor='#F0F0F0'");
                }
                else
                {
                    e.Row.Attributes.Add("onmouseout", "this.style.backgroundColor='White'");
                }
            }
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                //e.Row.Attributes.Add("onmouseover", "this.style.cursor='hand'");
                //e.Row.Attributes.Add("onmouseover", "this.style.backgroundcolor='Silver'");
                //if (e.Row.RowState == DataControlRowState.Alternate) {
                //    e.Row.Attributes.Add("onmouseout", "this.style.backgroundColor='#DCDCDC'");
                //} else {}
                //    e.Row.Attributes.Add("onmouseout", "this.style.backgroundColor='#EEEEEE'");
                //}

                //TableCell tc1 = e.Row.Cells[10];
                //Label picture1 = (Label)tc1.Controls[1];
                //if (picture1.HasSiteAccessImage()) setCellSize(tc1);

                //TableCell tc2 = e.Row.Cells[11];
                //Label picture2 = (Label)tc2.Controls[1];
                //if (picture2.HasSiteAccessImage()) setCellSize(tc2);

                //TableCell tc3 = e.Row.Cells[12];
                //Label picture3 = (Label)tc3.Controls[1];
                //if (picture3.HasSiteAccessImage()) setCellSize(tc3);

            }
        }
        protected void GridView1_Sorting(object sender, System.Web.UI.WebControls.GridViewSortEventArgs e)
        {
            //SqlFacility.SelectCommand = sqlPermitSelectCommand();
        }
        protected void GridView1_PageIndexChanging(object sender, System.Web.UI.WebControls.GridViewPageEventArgs e)
        {
            //SqlFacility.SelectCommand = sqlPermitSelectCommand();
        }

        protected void btnPB_Click(object sender, System.EventArgs e)
        {
            string strCols = txtCurrentCols.Text;
            List<string> lstVCol = strCols.Split('|').ToList();
            Session["selectedColHeaders"] = lstVCol;
            showHideColumns();
            checkColList();
        }
        private void checkColList()
        {
            List<string> lstChecked = new List<string>();
            lstChecked = (List<string>)Session["selectedColHeaders"];
            if (lstChecked.Count >= 0)
            {
                foreach (ListItem li3 in ckblCol.Items)
                {
                    if (lstChecked.Where(o => String.Equals(li3.Text, o, StringComparison.OrdinalIgnoreCase)).Any())
                    {
                        li3.Selected = true;
                    }
                    else
                    {
                        li3.Selected = false;
                    }
                }
            }
        }
        private void showHideColumns(){
            List<string> lstChecked = new List<string>();
            lstChecked = (List<string>)Session["selectedColHeaders"];
            if (lstChecked.Count>=0)
            {
                foreach (DataControlField dcf in GridView1.Columns)
                {
                    if (lstChecked.Where(o => string.Equals(dcf.HeaderText, o, StringComparison.OrdinalIgnoreCase)).Any())
                    {
                        dcf.Visible = true;
                    }
                    else
                    {
                        dcf.Visible = false;
                    }
                }
            }
        }
        protected void SqlFacility_Selected(object sender, SqlDataSourceStatusEventArgs e)
        {
            HitCount = HitCount + 1;
            lngTotalRecs = Convert.ToInt64(e.AffectedRows);
            //This always returns no of all records before applying the QF
        }
        protected void ddFitlers_SelectedIndexChanged(object sender, EventArgs e)
        {
            ProcessQF();
            GridView1.PageIndex = 0; 
        }
        protected void btnFilter_Click(object sender, ImageClickEventArgs e)
        {
            ProcessQF();
            GridView1.PageIndex = 0;
        }
        private String GridViewSortDirection
        {
            get { return (ViewState["SortDirection"] == null ? "ASC" : ViewState["SortDirection"].ToString()); }
            set { ViewState["SortDirection"] = value; }
        }
        private String GridViewSortExpression
        {
            get { return (ViewState["SortExpression"] == null ? string.Empty : ViewState["SortExpression"].ToString()); }
            set { ViewState["SortExpression"] = value; }
        }
        private String GetSortDirection()
        {
            if (ViewState["LastSortField"] != null && ViewState["SortExpression"].ToString() == ViewState["LastSortField"].ToString())
            {
                //sorting on same field, check ASC / DESC
                switch (GridViewSortDirection)
                {
                    case "ASC":
                        GridViewSortDirection = "DESC";
                        break;
                    case "DESC":
                        GridViewSortDirection = "ASC";
                        break;
                }
            }
            else
            {
                //sorting on new field, always use ASC
                GridViewSortDirection = "ASC";
            }
            return GridViewSortDirection;
        }
        protected void ProcessQF()
        {
            String sqlFilter = string.Empty;
            bool bFirst = true;

            if (ddCI.SelectedValue.ToWSSafeString() == "-1")
            {
                AddCriteriaIN("2,3,4", ref sqlFilter, "Component_Integrity_Code", ref bFirst);
            }
            else
            {
                AddCriteriaDD(ddCI, ref sqlFilter, "Component_Integrity_Code", FieldTypeEnum.number, ref bFirst);
            }
            AddCriteriaDD(ddLocation, ref sqlFilter, "BUILDING_ID", FieldTypeEnum.number, ref bFirst);
            AddCriteriaDDNull(ddCAR, ref sqlFilter, "CorrectiveActionsRequired", FieldTypeEnum.text, ref bFirst);
                
            SqlFacility.FilterExpression = sqlFilter;
            ViewState["filter"] = sqlFilter;
        }
        private void AddCriteriaDD(ExtendedDropDownList dd, ref String sql, String fieldName, FieldTypeEnum fieldtype, ref Boolean bFirst)
        {
            String fv = dd.SelectedValue.ToWSSafeString();
            if (fv != "*" && !String.IsNullOrWhiteSpace(fv) && fv != "-9999")
            {
                if (bFirst) bFirst = false; else sql += " AND ";
                sql += String.Format("{0}={2}{1}{2}", fieldName, fv, fieldtype == FieldTypeEnum.text ? "'" : String.Empty);
            }
        }
        private void AddCriteriaTB(TextBox tb, ref String sql, String fieldName, FieldTypeEnum fieldtype, ref Boolean bFirst)
        {
            String fv = tb.Text.ToWSSafeString(false, false, false, true, true);
            if (fv != "*" && !String.IsNullOrWhiteSpace(fv))
            {
                if (bFirst) bFirst = false; else sql += " AND ";
                if (fieldtype == FieldTypeEnum.text)
                {
                    sql += String.Format("{0} like '{1}%'", fieldName, fv);
                }
                else
                {
                    sql += String.Format("{0}={1}", fieldName, fv);
                }
            }
        }
        private void AddCriteriaIN(String commaDelimitedValues, ref String sql, String fieldName, ref Boolean bFirst)
        {
            String fv = commaDelimitedValues.ToWSSafeString(false, false, false, true, true);
            if (fv != "*" && !String.IsNullOrWhiteSpace(fv))
            {
                if (bFirst) bFirst = false; else sql += " AND ";
                sql += String.Format("{0} IN ({1})", fieldName, fv);
            }
        }
        private void AddCriteriaDT(TextBox tb, ref String sql, String fieldName, FieldTypeEnum fieldtype, ref Boolean bFirst)
        {
            String fv = tb.Text.ToWSSafeString();
            DateTime mydt;
            if (!String.IsNullOrWhiteSpace(fv) && DateTime.TryParse(fv, out mydt))
            {
                if (bFirst) bFirst = false; else sql += " AND ";
                if (fieldtype == FieldTypeEnum.dateFrom)
                {
                    sql += String.Format("{0} >= '{1}'", fieldName, fv);
                }
                else
                {
                    sql += String.Format("{0} < '{1}'", fieldName, mydt.AddDays(1).ToShortDateString());
                }
            }
        }
        private void AddCriteriaDDNull(ExtendedDropDownList dd, ref string sql, string fieldName, FieldTypeEnum fieldtype, ref Boolean bFirst)
        {
            string fv = dd.SelectedValue.ToWSSafeString();
            switch(fv)
            {
                case "Yes":
                    if (bFirst) bFirst = false; else sql += " AND ";
                    sql += String.Format(" ({0} IS NOT NULL AND {0}<>'') ", fieldName);
                    break;
                case "No":
                    if (bFirst) bFirst = false; else sql += " AND ";
                    sql += String.Format(" (({0} IS NULL) OR ({0}='')) ", fieldName);
                    break;
                case "View All":
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region database lookup / database display
        protected String GetFlagIcon(object statuscode)
        {
            String scc = statuscode.ToWSSafeString();
            if (scc == "1")
            {
                return "<img src='../Images/SmallMenuIcon/Check-32.png' alt='Pass' class='inline-icon' title='Pass' />";
            }
            else if (scc == "2")
            {
                return "<img src='../Images/SmallMenuIcon/Delete-32.png' alt='Failed' class='inline-icon' title='Failed' />";
            }
            else if (scc == "3")
            {
                return "<img src='../Images/SmallMenuIcon/Exclamation-32.png' alt='CodeViolation' class='inline-icon' title='Code Violation' />";
            }
            else if (scc == "4")
            {
                return "<img src='../Images/SmallMenuIcon/Warning-32.png' alt='NeedsAttention' class='inline-icon' title='Needs Immediate Attention' />";
            }
            else if (scc == "5")
            {
                return "<img src='../Images/SmallMenuIcon/Under-Construction-32.png' alt='Improvement' class='inline-icon' title='Opportunity for Improvement' />";
            }
            else
            {
                return "<img src='../Images/SmallMenuIcon/question32.png' alt='checked' class='inline-icon' title='N/A' />";
            }
        }
        protected String GetFormUrl(object euid, object bldgid, object scid)
        {
            String linkURL = String.Format("{0}?EntryUID={1}&BuildingID={2}&SCID={3}",
                    editpage, AESUtil.EncryptString(euid.ToWSSafeString()),
                    AESUtil.EncryptString(bldgid.ToWSSafeString()),
                    AESUtil.EncryptString(scid.ToWSSafeString()));
            return linkURL;
        }
        public string CreateLinkThumbnail(object strFileNameObj, object strCaptionObj)
        {
            return SiteAccessHelper.CreatePhotoThumbnail(strFileNameObj.ToWSSafeString(), strCaptionObj.ToWSSafeString(), ConfigurationManager.AppSettings["PhotoPath_Reduced"], "PhotoPath_reduced", false, "", "./TransmitImgSrc.ashx", "./TransmitFile.aspx", "../images");
        }
        #endregion

        #region **Util
        protected String gvstr(object s)
        {
            return s.ToWSSafeString().ToWSTitleCase();
        }
        private static Control FindControlIterative(Control root, string id)
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
        private enum GridViewPageModeEnum
        {
            Paging,
            ViewAll
        }
        private enum FieldTypeEnum
        {
            number,
            text,
            dateFrom,
            dateTo
        }
        #endregion
    }
}