using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebMap.Web
{
    public class ErrorEmail
    {
        public static void sendEmail(string message, string additional_title = "")
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
            if (!String.IsNullOrWhiteSpace(additional_title))
            {
                additional_title = "(" + additional_title + ")";
            }
            mail.Subject = System.Configuration.ConfigurationManager.AppSettings["ApplicationName"] + " Error. " +  additional_title;
            mail.Body = message;

            //if we are using the IIS SMTP Service, we can write the message
            //directly to the PickupDirectory, and bypass the Network layer       
            System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient(System.Configuration.ConfigurationManager.AppSettings["errorEmailServer"]);
            //smtp.DeliveryMethod = Net.Mail.SmtpDeliveryMethod.PickupDirectoryFromIis
            smtp.Send(mail);
        }
    }
}