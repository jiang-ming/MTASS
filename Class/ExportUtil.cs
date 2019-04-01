using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.IO;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;

namespace WebMap.Web
{
    public static class ExportUtil
    {
        public static void ExportToWord(this GridView GV, HttpContext context, String filenameWithoutExtension)
        {
            HttpResponse Response = context.Response;
            Response.Clear();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", String.Format("attachment;filename={0}.doc", filenameWithoutExtension));
            Response.Charset = "";
            Response.ContentType = "application/vnd.ms-word ";
            StringWriter sw = new StringWriter();
            HtmlTextWriter hw = new HtmlTextWriter(sw);
            GV.AllowPaging = false;
            GV.DataBind();
            GV.RenderControl(hw);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();
        }

        public static void ExportToXLS(this GridView GV, HttpContext context, String filenameWithoutExtension)
        {
            String attachment = String.Format("attachment; filename={0}.xls", filenameWithoutExtension.Replace("#","%23"));
            context.Response.ClearContent();
            context.Response.AddHeader("content-disposition", attachment);
            context.Response.ContentType = "application/vnd.ms-excel";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            GV.AllowPaging = false;
            GV.DataBind();
            GV.RenderControl(htw);
            //remove html img tags
            String htmlcontent = sw.ToString();
            //htmlcontent = Regex.Replace(htmlcontent, @"(<img \/?[^>]+>)", @"", RegexOptions.IgnoreCase); //remove <img ...>
            htmlcontent = Regex.Replace(htmlcontent, @"(<img src='\.\.\/Images\/SmallMenuIcon\/?[^>]+>)", @"", RegexOptions.IgnoreCase); //remove <img src="Images/SmallMenuIcon...>
            htmlcontent = Regex.Replace(htmlcontent, @"(<img src=""\.\.\/Images\/SmallMenuIcon\/?[^>]+>)", @"", RegexOptions.IgnoreCase); //remove <img src='Images/SmallMenuIcon...>
            htmlcontent = Regex.Replace(htmlcontent, @"(<a \/?[^>]+>)", @"", RegexOptions.IgnoreCase); //remove <a ....>
            htmlcontent = Regex.Replace(htmlcontent, @"(</a\/?[^>]*>)", @"", RegexOptions.IgnoreCase); //remove </a>
            htmlcontent = htmlcontent.Replace("src=\"./TransmitImgSrc.ashx", "src=\"https://www.wd-gis.com/MTASS/UserForms/TransmitImgSrc.ashx");
            //write output
            context.Response.Write(htmlcontent);
            context.Response.End();
        }

        public static void ExportToCSV(this DataTable dt, HttpContext context, String filenameWithoutExtension)
        {
            String attachment = String.Format("attachment; filename={0}.csv", filenameWithoutExtension);
            context.Response.ClearContent();
            context.Response.AddHeader("content-disposition", attachment);
            context.Response.ContentType = "application/text";
            StringBuilder sb = new StringBuilder();
            for (int k = 0; k < dt.Columns.Count; k++)
            {
                //add separator
                sb.Append(dt.Columns[k].Caption.ToWSSafeString() + ',');
            }
            //append new line
            sb.Append("\r\n");
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                for (int k = 0; k < dt.Columns.Count; k++)
                {
                    //add separator
                    sb.Append(dt.Rows[i][k].ToWSSafeString() + ',');
                }
                //append new line
                sb.Append("\r\n");
            }
            context.Response.Output.Write(sb.ToString());
            context.Response.Flush();
            context.Response.End();
        }
    }
}
