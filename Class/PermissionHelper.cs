using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Data.SqlClient;
using System.Configuration;

namespace WebMap.Web
{
    public static class PermissionHelper
    {
        public static Boolean bIsDebuggingHost()
        {
            if (HttpContext.Current.Request.UserHostAddress == "192.168.132.105" || HttpContext.Current.Request.UserHostAddress == "127.0.0.1")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static Boolean bIsSessionOK(Boolean bRedirectToLoginPage = true)
        {
            //check any session variables - if null then redirect to login page
            try
            {
                if ((bool)HttpContext.Current.Session["AllowEdit"])
                {
                    return true;
                }
                return true;
            }
            catch (Exception)
            {
                //throw SessionExpire();
                if (bRedirectToLoginPage)
                {
                    ReLogin();
                }
                return false;
            }
        }
        public static Boolean IsUserDbAllowEdit()
        {
            try
            {
                if (!(bool)HttpContext.Current.Session["AllowEdit"])
                {
                    return false;
                }
                return true;
            }
            catch (Exception)
            {
                //throw SessionExpire();
                ReLogin();
                return false;
            }
        }
        public static Boolean IsUserDbAllowDeletePermit()
        {
            try
            {
                if (!(bool)HttpContext.Current.Session["UserDB_AllowDeletePermit"])
                {
                    return false;
                }
                return true;
            }
            catch (Exception)
            {
                //throw SessionExpire();
                ReLogin();
                return false;
            }
        }
        public static String GetUserName()
        {
            return HttpContext.Current.User.Identity.Name.ToUpper().Trim();
        }
        private static Exception SessionExpire()
        {
            return new Exception("Your session has expired due to extended period of inactivity. Please close your web browser and try logging in again.");
        }

        public static void ReLogin(String message = "Your session has expired.  Please log in to continue.")
        {
            FormsAuthentication.SignOut();
            HttpContext.Current.Session["CurrentEditor"] = null;
            FormsAuthentication.RedirectToLoginPage(string.Format("NoByPass=TRUE&MSG={0}", message));
        }

        #region auto-login
        public static void AutoLoginReadOnly(string username)
        {
            username = username.ToUpper();
            UpdateLogUser(username, "ReadOnly");
            HttpContext.Current.Session["CurrentEditor"] = ""; ;
            HttpContext.Current.Session["AllowEdit"] = false;
            ReadAdditionalUserData(username);
            FormsAuthentication.SetAuthCookie(username, false);
        }
        private static void UpdateLogUser(string strUser, string LogOnResult)
        {
            //Time_END
            SqlConnection myConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["_UserDatabase"].ConnectionString);
            string strCommand = string.Format("Update [LOG] set [USER]='{0}', [LOGON_SUCCESS]='{1}' WHERE [ID]={2}", strUser.ToUpper(), LogOnResult.ToUpper(), HttpContext.Current.Session["SID"]);
            SqlCommand myCommand = new SqlCommand(strCommand, myConnection);
            myCommand.Connection.Open();
            myCommand.ExecuteNonQuery();
            myCommand.Connection.Close();
            HttpContext.Current.Application["User" + HttpContext.Current.Session["SID"]] = strUser.ToUpper() + "|" + DateTime.Now.ToString();
        }
        private static void ReadAdditionalUserData(string loginUserName, bool RereadBasicData = false)
        {
            SqlConnection myConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["_UserDatabase"].ConnectionString);
            string strSelect = "select * from [USER] where [UserName] like '" + loginUserName.ToUpper() + "'";
            SqlDataAdapter dsCmd = new SqlDataAdapter(strSelect, myConnection);
            System.Data.DataSet myData = new System.Data.DataSet();
            dsCmd.Fill(myData, "User");
            System.Data.DataTable pTable = myData.Tables["User"];
            //User table should have first standard 6 columns - Username, Password, Comment, Enabled, CanViewLog, AllowEdit
            //we will add any extra field values after the first 6 columns to session variables
            if (pTable.Columns.Count > 6)
            {
                for (byte i = 7; i <= pTable.Columns.Count; i++)
                {
                    string sSessionVarName = string.Format("UserDB_{0}", pTable.Columns[i - 1].ColumnName);
                    if (pTable.Rows.Count > 0)
                    {
                        HttpContext.Current.Session[sSessionVarName] = pTable.Rows[0][i - 1];
                    }
                    else
                    {
                        HttpContext.Current.Session[sSessionVarName] = "";
                    }
                }
            }
            if (RereadBasicData)
            {
                HttpContext.Current.Session["user"] = loginUserName.ToUpper();
                try
                {
                    //some of the old login database does not have the AllowEdit field
                    HttpContext.Current.Session["AllowEdit"] = pTable.Rows[0]["AllowEdit"];
                }
                catch
                {
                    HttpContext.Current.Session["AllowEdit"] = false;
                }
            }
        }
        #endregion
    }
}
