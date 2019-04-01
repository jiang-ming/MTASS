using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.Data;
using WebMap.Web;
using System.Configuration;
using System.Text.RegularExpressions;

namespace MTASS
{
    public class Global : System.Web.HttpApplication
    {

        void Application_Start(object sender, EventArgs e)
        {
            // Fires when the application is started
            Application["UserOnline"] = 0;
        }

        void Application_End(object sender, EventArgs e)
        {

        }

        void Application_Error(object sender, EventArgs e)
        {
            // Save exection error message and details to session VAR for email notification.
            Exception exception = Server.GetLastError().InnerException;
            if (exception == null)
            {
                //the error does not have inner exception
                //most like this is http error (e.g. 404) and will be handled by IIS
                return;
            }

            string errorHeader = string.Empty;
            string errorDetail = string.Empty;
            string errorUserMessage = string.Empty;
            errorHeader = "Page: " + Request.Url.ToString() + Environment.NewLine;

            try
            {
                //get URL referrer
                string sRef = string.Empty;
                try
                {
                    sRef = Request.UrlReferrer.ToString();
                }
                catch (Exception ex)
                {
                    sRef = "unknown";
                }
                finally
                {
                    errorHeader += "Referrer: " + sRef + Environment.NewLine;
                }
                //Get Client IP address
                string sIP = string.Empty;
                string sTR = string.Empty;
                sIP = Request.UserHostAddress.ToString();
                sTR = System.Net.Dns.GetHostEntry(sIP).HostName.ToString();
                errorHeader += string.Format("Client: {0} [{1}]", sIP, sTR) + Environment.NewLine;
                //get user name
                string sUserName = string.Empty;
                try
                {
                    sUserName = User.Identity.Name.ToString().ToUpper();
                }
                catch (Exception ex)
                {
                    sUserName = "unknown";
                }
                finally
                {
                    errorHeader += "User: " + sUserName + Environment.NewLine;
                }
                //Generate other error details
                errorHeader += "Error Message : " + exception.Message + Environment.NewLine;
                errorHeader += "Source: " + exception.Source.ToString() + Environment.NewLine;
                errorHeader += "Target Site: " + exception.TargetSite.ToString() + Environment.NewLine;
                if (exception.GetType() == typeof(System.UnauthorizedAccessException))
                {
                    UnauthorizedAccessException errorAccess = exception as UnauthorizedAccessException;
                    //
                    if (errorAccess.StackTrace.ToUpper().IndexOf("SERVERCONNECTION.CONNECT") > 0)
                    {
                        errorHeader += "Unable to connect to server." + Environment.NewLine;
                    }
                }

                errorDetail = Environment.NewLine + Environment.NewLine + "--" + Environment.NewLine + exception.StackTrace;

                //special handling for ArcGIS SOC UsageTimeout (default 600 seconds, WD 14400 seconds)
                //   this error is not related to ping server failed but the SOC server has released the context
                //   when the usage timeout is reached regardless of the activity between the browser and web server
                if (exception.Message.IndexOf("The remote procedure call failed and did not execute") != -1 && exception.Source.ToString().IndexOf("ESRI.ArcGIS.Server()") != -1 && exception.TargetSite.ToString().IndexOf("ESRI.ArcGIS.Server.IServerObject(get_ServerObject())") != -1 && string.IsNullOrEmpty(errorDetail))
                {
                    errorUserMessage = "The application can not connect to the map server. You may have been inactive for a long period of time or may have reached the usage timeout on the server. Please close all your web browser windows and try logging in again.";
                    errorDetail = errorUserMessage + Environment.NewLine + errorDetail;
                    //errorHeader = "" 'do this to disable the email
                }
                else if (exception.Message.IndexOf("Conversion from type 'DBNull' to type") >= 0)
                {
                    errorUserMessage = "At least one of the fields contain null values. The data entry form cannot be displayed at this time.";
                    errorDetail = errorUserMessage + Environment.NewLine + errorDetail;
                    //errorHeader = "" 'do this to disable the email
                }
                else if (exception.Message.IndexOf("The field is too small to accept the amount of data you attempted to add.") >= 0)
                {
                    errorUserMessage = "At least one of the field value has larger amount of data than the database can accept. Please shorten your input and try again.";
                    errorDetail = errorUserMessage + Environment.NewLine + errorDetail;
                    //errorHeader = "" 'do this to disable the email
                }
                else if (exception.Message.IndexOf("Value was either too large or too small for an Int16.") >= 0)
                {
                    errorUserMessage = "At least one of the number fields has larger value than the database can accept. Please check your input and try again.";
                    errorDetail = errorUserMessage + Environment.NewLine + errorDetail;
                    //errorHeader = "" 'do this to disable the email
                }
                else if (exception.Message.IndexOf("has a SelectedValue which is invalid because it does not exist in the list of items") >= 0)
                {
                    string sMsg = exception.Message;
                    errorUserMessage = "The data entry form cannot be displayed at this time. At least one field has a value that does not exist in the drop-down list." + Environment.NewLine + "Please check field value for " + sMsg.Substring(0, sMsg.IndexOf("has a SelectedValue", StringComparison.CurrentCultureIgnoreCase) - 1);
                    errorDetail = errorUserMessage + Environment.NewLine + errorDetail;
                    //errorHeader = "" 'do this to disable the email
                }

            }
            catch (Exception ex)
            {

            }
            finally
            {
                if (!string.IsNullOrEmpty(errorHeader) && !(Request.UserHostAddress.ToString() == "127.0.0.1" || Request.UserHostAddress.ToString() == "::1"))
                {
                    string sEmailContent = errorHeader + Environment.NewLine + errorDetail;
                    sendEmail(sEmailContent);
                }
            }

            try
            {
                //save error message in session for displaying in error page
                Session["ErrorHeader"] = errorHeader;
                Session["ErrorDetail"] = errorDetail;
                Session["Error_UserMessage"] = errorUserMessage;
                //do not comment this line as this is used on the error page to inform user of the custom error message
                Session["Error"] = exception;
            }
            catch
            {
            }
        }

        void sendEmail(string message)
        {
            //create the mail message
            System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
            //set the addresses
            mail.From = new System.Net.Mail.MailAddress(System.Configuration.ConfigurationManager.AppSettings["errorEmailFrom"], System.Configuration.ConfigurationManager.AppSettings["ApplicationName"]);
            String[] strTo = System.Configuration.ConfigurationManager.AppSettings["errorEmailTo"].Split(",".ToCharArray());
            int k = 0;
            for (k = strTo.GetLowerBound(0); k <= strTo.GetUpperBound(0); k++)
            {
                mail.To.Add(strTo[k].Trim());
            }
            //set the content
            mail.Subject = System.Configuration.ConfigurationManager.AppSettings["ApplicationName"] + " Error Report";
            mail.Body = message;

            //if we are using the IIS SMTP Service, we can write the message
            //directly to the PickupDirectory, and bypass the Network layer       
            System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient(System.Configuration.ConfigurationManager.AppSettings["errorEmailServer"]);
            //smtp.DeliveryMethod = Net.Mail.SmtpDeliveryMethod.PickupDirectoryFromIis
            smtp.Send(mail);
        }

        void Session_Start(object sender, EventArgs e)
        {
            // Fires when the session is started
            Application["UserOnline"] = (int)Application["UserOnline"] + 1;

            //Session("VPath_Setting") = "DEBUG" ' Either 'DEBUG' OR 'PRODUCTION'
            // Fires when the session is started        
            string sHost = "";
            try
            {
                sHost = System.Net.Dns.GetHostEntry(Request.Params["REMOTE_ADDR"]).HostName.ToString().ToUpper();
            }
            catch
            {
            }
            finally
            {
                Session["sHOST"] = sHost.ToUpper();
            }

            if (ConfigurationManager.ConnectionStrings["_UserDatabase"] != null)
            {
                //create log entry in the log database
                System.Data.OleDb.OleDbConnection myConnection = new System.Data.OleDb.OleDbConnection(ConfigurationManager.ConnectionStrings["_UserDatabase"].ConnectionString);
                string strCommand = string.Format("Insert into [LOG] (IP_ADDRESS,COMPUTER_NAME, TIME_START) values ('{0}','{1}','{2}')", Request.Params["REMOTE_ADDR"], sHost, DateTime.Now.ToString());
                System.Data.OleDb.OleDbCommand myCommand = new System.Data.OleDb.OleDbCommand(strCommand, myConnection);
                myCommand.Connection.Open();
                myCommand.ExecuteNonQuery();
                System.Data.OleDb.OleDbCommand cmdGetidentity = new System.Data.OleDb.OleDbCommand("SELECT @@IDENTITY", myConnection);
                long lngNewID = Convert.ToInt64(cmdGetidentity.ExecuteScalar());
                Session["SID"] = lngNewID;
                myCommand.Connection.Close();

                //check if client is a known search engine crawler
                bool bIpMatched = false;
                if (!String.IsNullOrEmpty(ConfigurationManager.AppSettings["SkipCrawlerCheck"]) && !bool.Parse(ConfigurationManager.AppSettings["SkipCrawlerCheck"]))
                {
                    System.Data.OleDb.OleDbConnection IPDBConnection = new System.Data.OleDb.OleDbConnection(ConfigurationManager.ConnectionStrings["_InternalIPDB"].ConnectionString);
                    string strIPQuery = "select * from [IP_Crawler] where [IP] = '" + Request.UserHostAddress + "'";
                    System.Data.OleDb.OleDbDataAdapter dsCmd = new System.Data.OleDb.OleDbDataAdapter(strIPQuery, IPDBConnection);
                    System.Data.DataSet myData = new System.Data.DataSet();
                    dsCmd.Fill(myData, "IP_Exact");
                    System.Data.DataTable pTable = myData.Tables["IP_Exact"];
                    if (pTable.Rows.Count > 0)
                    {
                        //Exact match to IP address
                        bIpMatched = true;
                        //
                    }
                    else
                    {
                        //this is a small table, just loop through all the record, if the table gets bigger we need to change the logic here
                        strIPQuery = "select * from [IP_Crawler] where instr(1,[IP],'*')";
                        dsCmd = new System.Data.OleDb.OleDbDataAdapter(strIPQuery, IPDBConnection);
                        dsCmd.Fill(myData, "IP_SubNet");
                        pTable = myData.Tables["IP_SubNet"];
                        System.Data.DataRow pRow = null;
                        foreach (DataRow pRow_loopVariable in pTable.Rows)
                        {
                            pRow = pRow_loopVariable;
                            Wildcard wildcard = new Wildcard(pRow["IP"].ToString(), RegexOptions.IgnoreCase);
                            if (wildcard.IsMatch(Request.UserHostAddress) || wildcard.IsMatch(Session["sHOST"].ToString()))
                            {
                                bIpMatched = true;
                                break; // TODO: might not be correct. Was : Exit For
                            }
                        }
                    }
                }

                if (bIpMatched)
                {
                    Session["IsCrawler"] = true;
                }
                else
                {
                    Session["IsCrawler"] = false;
                }
            }

            String userName = "Unknown";
            if (HttpContext.Current.User.Identity.Name != null) userName = HttpContext.Current.User.Identity.Name;
            Application["User" + Session["SID"]] = userName + "|" + DateTime.Now.ToString();
            Session["LastRequest"] = DateTime.Now;
        }

        void Session_End(object sender, EventArgs e)
        {
            //// Code that runs when a session ends. 
            //// Note: The Session_End event is raised only when the sessionstate mode
            //// is set to InProc in the Web.config file. If session mode is set to StateServer 
            //// or SQLServer, the event is not raised.
            //// Close out session and quit application
            Application["UserOnline"] = (int)Application["UserOnline"] - 1;
            Application["User" + Session["SID"]] = null;

            try
            {
                System.Data.OleDb.OleDbConnection myConnection = new System.Data.OleDb.OleDbConnection(ConfigurationManager.ConnectionStrings["_UserDatabase"].ConnectionString);
                string strCommand = string.Format("UPDATE [LOG] SET [TIME_END]='{0}' WHERE  [ID]={1}", DateTime.Now.ToString(), Session["SID"]);
                System.Data.OleDb.OleDbCommand myCommand = new System.Data.OleDb.OleDbCommand(strCommand, myConnection);
                myCommand.Connection.Open();
                myCommand.ExecuteNonQuery();
                myCommand.Connection.Close();
            }
            catch (Exception ex)
            {

            }
        }

    }
}
