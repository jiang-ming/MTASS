using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;
using System.Data.OleDb;
using System.Data;
using System.Configuration;
using System.Text.RegularExpressions;
using WebMap.Web;

public partial class WDLogin : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        //Custom Client Names.
        string clientName = ConfigurationManager.AppSettings["ClientName"];
        Page.Title = clientName + " LOGIN PAGE";
        messageContainer.Visible = false;

        //PersistCookie.Checked = false;
        //PersistCookie.Visible = false;

        Session["INTERNAL_IP"] = false;
        System.Data.OleDb.OleDbConnection IPDBConnection = new System.Data.OleDb.OleDbConnection(ConfigurationManager.ConnectionStrings["_InternalIPDB"].ConnectionString);

        //check session to prevent logs from being created twice [1st on page load, 2nd when user click login button]
        if (Session["SID"] == null)
        {
            string sHost = "";
            try
            {
                sHost = System.Net.Dns.GetHostEntry(Request.Params["REMOTE_ADDR"]).HostName.ToString().ToUpper();
            }
            catch (Exception ex)
            {
            }
            finally
            {
                Session["sHOST"] = sHost.ToUpper();
            }
            System.Data.OleDb.OleDbConnection myConnection = new System.Data.OleDb.OleDbConnection(ConfigurationManager.ConnectionStrings["_UserDatabase"].ConnectionString);
            string strCommand = string.Format("Insert into [LOG] (IP_ADDRESS,COMPUTER_NAME, TIME_START) values ('{0}','{1}','{2}')", Request.Params["REMOTE_ADDR"], sHost, DateTime.Now.ToString());
            System.Data.OleDb.OleDbCommand myCommand = new System.Data.OleDb.OleDbCommand(strCommand, myConnection);
            myCommand.Connection.Open();
            myCommand.ExecuteNonQuery();
            System.Data.OleDb.OleDbCommand cmdGetidentity = new System.Data.OleDb.OleDbCommand("SELECT @@IDENTITY", myConnection);
            long lngNewID = Convert.ToInt64(cmdGetidentity.ExecuteScalar());
            Session["SID"] = lngNewID;
            myCommand.Connection.Close();
        }

        if (Page.Request["NoByPass"] == "TRUE")
        {
            if ((Session["CurrentEditor"] != null))
            //user has already authenticated through one of the ASP edit forms
            {
                FormsAuthentication.RedirectFromLoginPage(Session["CurrentEditor"].ToString(), false);
            }
            return;
        }

        if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["No_Internal_IP_L123_Bypass"]) && bool.Parse(ConfigurationManager.AppSettings["No_Internal_IP_L123_Bypass"]) && Request["Token"] == null)
        {
            return;
        }

        if ((Session["sHost"].ToString().IndexOf("GIS1") == 0 || Session["sHost"].ToString().IndexOf("PROD3") == 0) && Request["Token"] == null)
        {
            //logged in from WS PC
            DeveloperBypass();
            return;
        }
        else if (Request.UserHostAddress == "127.0.0.1" || Request.UserHostAddress == "::1")
        {
            //logged in from server using localhost in browser address
            DeveloperBypass_InitiallyReadOnly("ADMIN", "L3 Bypass (Debug)"); //will be prompted for password if trying to edit
            return;
        }
        else
        {
            //check ip address
            DataSet myData = new DataSet();

            //Level II bypass - GIS PC
            string strGISPCQuery = string.Format("select * from [IP_List_GIS] where [PCNAME] like '{0}'", Session["sHost"].ToString());
            OleDbDataAdapter dsGISCmd = new OleDbDataAdapter(strGISPCQuery, IPDBConnection);
            dsGISCmd.Fill(myData, "GIS_PC");
            DataTable pTableGIS = myData.Tables["GIS_PC"];
            if (pTableGIS.Rows.Count > 0)
            {
                DeveloperBypass_InitiallyReadOnly(pTableGIS.Rows[0]["LOGIN"].ToString(), string.Format("L2 Bypass ({0})", pTableGIS.Rows[0]["LOGIN"].ToString())); //will be prompted for password if trying to edit
                return;
            }

            //Level I bypass - Wendel Intranet
            bool bIpOk = false;
            string strIPQuery = string.Format("select * from [IP_List] where [IP] = '{0}'", Request.UserHostAddress);
            OleDbDataAdapter dsCmd = new OleDbDataAdapter(strIPQuery, IPDBConnection);
            dsCmd.Fill(myData, "IP_Exact");
            DataTable pTable = myData.Tables["IP_Exact"];
            if (pTable.Rows.Count > 0)
            {
                //Exact match to IP address
                bIpOk = true;
                //
            }
            else
            {
                strIPQuery = "select * from [IP_List] where instr(1,[IP],'*')";
                dsCmd = new OleDbDataAdapter(strIPQuery, IPDBConnection);
                dsCmd.Fill(myData, "IP_SubNet");
                pTable = myData.Tables["IP_SubNet"];
                System.Data.DataRow pRow = null;
                foreach (DataRow pRow_loopVariable in pTable.Rows)
                {
                    pRow = pRow_loopVariable;
                    Wildcard wildcard = new Wildcard(pRow["IP"].ToString(), RegexOptions.IgnoreCase);
                    if (wildcard.IsMatch(Request.UserHostAddress) || wildcard.IsMatch(Session["sHOST"].ToString()))
                    {
                        bIpOk = true;
                        break; // TODO: might not be correct. Was : Exit For
                    }
                }
            }

            //'Level 1 DNS lookup bypass
            //'disable DNS lookup. it's slow
            //If Not bIpOk Then
            //    strIPQuery = "select * from [IP_List] where [DNSLookup] = true"
            //    dsCmd = New OleDbDataAdapter(strIPQuery, IPDBConnection)
            //    dsCmd.Fill(myData, "DNS_SubNet")
            //    pTable = myData.Tables("DNS_SubNet")
            //    Dim pRow As System.Data.DataRow
            //    For Each pRow In pTable.Rows
            //        Try
            //            If Request.UserHostAddress = System.Net.Dns.GetHostEntry(pRow.Item("IP").ToString).AddressList(0).ToString Then
            //                bIpOk = True
            //                Exit For
            //            End If
            //        Catch ex As Exception

            //        End Try
            //    Next
            //End If

            //token in URL 
            if (Request["Token"] != null)
            {
                String[] userInfo = AESUtil.DecryptString(Request["Token"]).Split(new String[] { "|" }, StringSplitOptions.None);
                //if ip matched the token - allow regardless of ip address
                if (userInfo[0].ToUpper() == "WARIT" && userInfo[1] == Request.UserHostAddress)
                {
                    //token valid
                    DeveloperBypass();
                    return;
                }
                //developer bypass - allow if coming from any of the recognized ip and token contains any of the recognized ip
                else if (bIpOk)
                {
                    String strTokenIPQuery = String.Format("select * from [IP_List] where [IP] = '{0}'", userInfo[1]);
                    OleDbDataAdapter dsTokenIPCmd = new OleDbDataAdapter(strTokenIPQuery, IPDBConnection);
                    dsCmd.Fill(myData, "Token_IP_Exact");
                    DataTable pTokenIPTable = myData.Tables["Token_IP_Exact"];
                    if (pTable.Rows.Count > 0)
                    {
                        //token valid
                        DeveloperBypass();
                        return;
                    }
                }
            }

            if (bIpOk)
            {
                Session["INTERNAL_IP"] = true;
                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["No_Internal_IP_Bypass"]) && bool.Parse(ConfigurationManager.AppSettings["No_Internal_IP_Bypass"]))
                {
                    messageContainer.Visible = true;
                    lblResults.Text = "Internal IP bypass is disabled in this application. You need to login.";
                    lblResults.Font.Bold = true;
                }
                else
                {
                    UpdateLogUser("ADMIN", "L1 Bypass");
                    //Level I bypass
                    //Internal Access - bypass login screen
                    Session["AllowEdit"] = false;
                    FormsAuthentication.RedirectFromLoginPage("ADMIN", false);
                    return;
                }
            }
        }

        //Restrict_IP_Address
        if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["Restrict_IP_Address"]) && bool.Parse(ConfigurationManager.AppSettings["Restrict_IP_Address"]))
        {
            bool bUserIPOK = false;
            OleDbConnection myUserIPConnection = new OleDbConnection(ConfigurationManager.ConnectionStrings["_UserDatabase"].ConnectionString);
            string strUserIPQuery = "select * from [IP_Allow_List] where [IP] = '" + Request.UserHostAddress + "'";
            OleDbDataAdapter dsUserIPCmd = new OleDbDataAdapter(strUserIPQuery, myUserIPConnection);
            DataSet myUserIPData = new DataSet();
            dsUserIPCmd.Fill(myUserIPData, "IP_Exact");
            DataTable pUserIPTable = myUserIPData.Tables["IP_Exact"];
            if (pUserIPTable.Rows.Count > 0)
            {
                //Exact match to IP address
                bUserIPOK = true;
            }
            else
            {
                strUserIPQuery = "select * from [IP_Allow_List] where instr(1,[IP],'*')";
                dsUserIPCmd = new OleDbDataAdapter(strUserIPQuery, myUserIPConnection);
                dsUserIPCmd.Fill(myUserIPData, "IP_SubNet");
                pUserIPTable = myUserIPData.Tables["IP_SubNet"];
                System.Data.DataRow pRow = null;
                foreach (DataRow pRow_loopVariable in pUserIPTable.Rows)
                {
                    pRow = pRow_loopVariable;
                    if (Request.UserHostAddress.Length >= pRow["IP"].ToString().Length)
                    {
                        if (Request.UserHostAddress.Substring(0, pRow["IP"].ToString().Length - 1) == pRow["IP"].ToString().Substring(0, pRow["IP"].ToString().Length - 1))
                        {
                            bUserIPOK = true;
                            break; // TODO: might not be correct. Was : Exit For
                        }
                    }
                }
            }

            if (!bUserIPOK)
            {
                UserName.Disabled = true;
                UserPass.Disabled = true;
                cmdLogin.Enabled = false;
                messageContainer.Visible = true;
                lblResults.Text = "Sorry, access is not permitted from your IP Address. Please contact Wendel GIS department for assistance.";
                lblResults.Font.Bold = true;
            }
        }
    }

    private void DeveloperBypass()
    {
        UpdateLogUser("WARIT", "L3 Bypass (GIS1)");
        Session["CurrentEditor"] = "WARIT";
        //If user identity <> ADMIN, you must assign CurrentEditor otherwise, some ASPX codes will return error
        Session["AllowEdit"] = true;
        Session["INTERNAL_IP"] = true;
        ReadAdditionalUserData("WARIT");
        FormsAuthentication.RedirectFromLoginPage("WARIT", false);
    }

    private void DeveloperBypass_InitiallyReadOnly(String user, String description)
    {
        UpdateLogUser(user, description);
        Session["AllowEdit"] = false;
        Session["INTERNAL_IP"] = false;
        //localhost - testing / debugging
        ReadAdditionalUserData("WARIT");
        FormsAuthentication.RedirectFromLoginPage("ADMIN", false);
    }

    private void ReadAdditionalUserData(string loginUserName)
    {
        System.Data.OleDb.OleDbConnection myConnection = new System.Data.OleDb.OleDbConnection(ConfigurationManager.ConnectionStrings["_UserDatabase"].ConnectionString);
        string strSelect = "select * from [USER] where [UserName] like '" + loginUserName.ToUpper() + "'";
        System.Data.OleDb.OleDbDataAdapter dsCmd = new System.Data.OleDb.OleDbDataAdapter(strSelect, myConnection);
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
                    Session[sSessionVarName] = pTable.Rows[0][i - 1];
                }
                else
                {
                    Session[sSessionVarName] = "";
                }
            }
        }
    }

    protected void cmdLogin_Click(object sender, System.EventArgs e)
    {
        if ((validateUser(UserName.Value, UserPass.Value)))
        {
            Session["CurrentEditor"] = UserName.Value.Trim().ToUpper();
            ReadAdditionalUserData(UserName.Value);
            FormsAuthentication.RedirectFromLoginPage(UserName.Value, PersistCookie.Checked);
        }
        else
        {
            messageContainer.Visible = true;
            lblResults.Text = "Invalid Credentials: Please try again";
        }
        //FormsAuthentication.SignOut()
    }

    private bool validateUser(string strID, string strPass)
    {
        bool functionReturnValue = false;
        OleDbConnection myConnection = new OleDbConnection(ConfigurationManager.ConnectionStrings["_UserDatabase"].ConnectionString);
        string strSelect = "select * from [USER] where [UserName] like '" + GetSafeString(strID) + "' and ( " + " [Password] = '" + GetSafeString(strPass) + "'" + " and [Enabled] = true" + " )";
        OleDbDataAdapter dsCmd = new OleDbDataAdapter(strSelect, myConnection);
        DataSet myData = new DataSet();
        dsCmd.Fill(myData, "User");
        DataTable pTable = myData.Tables["User"];
        if (pTable.Rows.Count == 0)
        {
            UpdateLogUser(GetSafeString(strID), "NO");
            functionReturnValue = false;
        }
        else
        {
            UpdateLogUser(GetSafeString(strID), "YES");
            functionReturnValue = true;
            Session["user"] = strID.ToUpper();
            DataRow pDRow = pTable.Rows[0];
            try
            {
                //some of the old login database does not have the AllowEdit field
                Session["AllowEdit"] = pDRow["AllowEdit"];
            }
            catch
            {
                Session["AllowEdit"] = false;
            }
        }
        myConnection.Close();
        return functionReturnValue;
    }

    private string GetSafeString(string strInput)
    {
        //This is to protect SQL injection
        strInput = Regex.Replace(strInput, "\\(", "", RegexOptions.IgnoreCase);
        strInput = Regex.Replace(strInput, "\\)", "", RegexOptions.IgnoreCase);
        strInput = Regex.Replace(strInput, "'", "", RegexOptions.IgnoreCase);
        strInput = Regex.Replace(strInput, "( and)", "", RegexOptions.IgnoreCase);
        strInput = Regex.Replace(strInput, "( or)", "", RegexOptions.IgnoreCase);
        strInput = Regex.Replace(strInput, "=", "", RegexOptions.IgnoreCase);
        strInput = Regex.Replace(strInput, "(true)", "", RegexOptions.IgnoreCase);
        strInput = Regex.Replace(strInput, "(false)", "", RegexOptions.IgnoreCase);
        strInput = Regex.Replace(strInput, "(select)", "", RegexOptions.IgnoreCase);
        strInput = Regex.Replace(strInput, "(delete)", "", RegexOptions.IgnoreCase);
        strInput = Regex.Replace(strInput, "(insert)", "", RegexOptions.IgnoreCase);
        strInput = Regex.Replace(strInput, "(drop)", "", RegexOptions.IgnoreCase);
        return strInput;
    }

    public void UpdateLogUser(string strUser, string LogOnResult)
    {
        //Time_END
        System.Data.OleDb.OleDbConnection myConnection = new System.Data.OleDb.OleDbConnection(ConfigurationManager.ConnectionStrings["_UserDatabase"].ConnectionString);
        string strCommand = string.Format("Update [LOG] set [USER]='{0}', [LOGON_SUCCESS]='{1}' WHERE [ID]={2}", strUser.ToUpper(), LogOnResult.ToUpper(), Session["SID"]);
        System.Data.OleDb.OleDbCommand myCommand = new System.Data.OleDb.OleDbCommand(strCommand, myConnection);
        myCommand.Connection.Open();
        myCommand.ExecuteNonQuery();
        myCommand.Connection.Close();
        Application["User" + Session["SID"]] = strUser.ToUpper() + "|" + DateTime.Now.ToString() + "|" + DateTime.Now.ToString() + "|" + DateTime.Now.ToString() + "|" + DateTime.Now.ToString();
    }

}