using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Data.OleDb;
using System.Configuration;
using WebMap.Web;
using System.Web.Security;

    partial class WDUserStatus : System.Web.UI.Page
    {

        private void Page_Load(object sender, System.EventArgs e)
        {
            if (!PermissionHelper.bIsSessionOK())
            {
                return;
            }
            try
            {
                //number of users online
                NumUserOnline.Text = string.Format("{0} user(s) online.", Application["UserOnline"].ToString());

                //details
                System.Data.OleDb.OleDbConnection myConnection = new System.Data.OleDb.OleDbConnection(ConfigurationManager.ConnectionStrings["_UserDatabase"].ConnectionString);
                string strCommand = string.Format("Select [ID],[IP_ADDRESS],[COMPUTER_NAME],[TIME_START] from [LOG] where [TIME_END] is null and [TIME_START] >= #{0}# ORDER BY [TIME_START] DESC", DateTime.Now.AddDays(-1));
                OleDbDataAdapter pDA = new OleDbDataAdapter(strCommand, myConnection);
                DataSet pDS = new DataSet();
                pDA.Fill(pDS, "ActiveUsers");

                DataTable dt = pDS.Tables["ActiveUsers"];
                DataColumn dcUser = null;
                dcUser = new DataColumn("User");
                dcUser.DataType = System.Type.GetType("System.String");
                dt.Columns.Add(dcUser);
                dcUser = new DataColumn("LastRequest");
                dcUser.DataType = System.Type.GetType("System.String");
                dt.Columns.Add(dcUser);
                dcUser = new DataColumn("SinceLastRequest");
                dcUser.DataType = System.Type.GetType("System.String");
                dt.Columns.Add(dcUser);

                DataTable dt2 = dt.Clone();
                dt2.Columns["User"].SetOrdinal(1);

                //loop throigh table and update user name
                DataRow[] drs = dt.Select();
                foreach (DataRow dr in drs)
                {
                    string SID = dr["ID"].ToString();
                    if (Application["User" + SID] != null)
                    {
                        String[] UserInfo = Application["User" + SID].ToString().Split("|".ToCharArray());
                        dr["User"] = UserInfo[0];
                        dr["LastRequest"] = DateTime.Parse(UserInfo[1]).ToLongTimeString();
                        dr["SinceLastRequest"] = FormatTimeSpan((DateTime.Now - DateTime.Parse(UserInfo[1])));
                        dt2.ImportRow(dr);
                    }
                }
                GridView1.DataSource = dt2;
                dt2.Columns["ID"].ColumnName = "Session ID";
                dt2.Columns["IP_ADDRESS"].ColumnName = "IP Address";
                dt2.Columns["COMPUTER_NAME"].ColumnName = "Host Name";
                dt2.Columns["TIME_START"].ColumnName = "Time Started";
                dt2.Columns["LastRequest"].ColumnName = "Last Request";
                dt2.Columns["SinceLastRequest"].ColumnName = "Time Since Last Request";
                GridView1.DataBind();

            }
            catch
            {

            }

        }

        private string FormatTimeSpan(TimeSpan span)
        {
            string st = "";
            if (span.Days > 0)
                st += span.Days.ToString("00") + " day ";
            st += span.Hours.ToString("00") + ":" + span.Minutes.ToString("00") + ":" + span.Seconds.ToString("00");
            return st;
        }

    }
