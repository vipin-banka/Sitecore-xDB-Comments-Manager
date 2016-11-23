#region

using System;
using System.Net;
using System.Net.Mail;
using Sitecore.Configuration;
using Sitecore.Diagnostics;
using Sitecore.StringExtensions;

#endregion

namespace xDBCommentsManager
{
    public static class Utility
    {
        /// <summary>
        /// Send mail function for sending mail to user and admin.
        /// </summary>
        /// <param name="emailFrom"></param>
        /// <param name="emailTo"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        public static void SendMail(string emailFrom, string emailTo, string subject, string body)
        {
            try
            {
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(emailFrom);
                foreach (var address in emailTo.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries))
                {                    
                    mail.To.Add(address);
                }
                //MailMessage mail = new MailMessage(emailFrom, emailTo);
                SmtpClient client = new SmtpClient();
                if (!Settings.GetSetting("MailServerUserName").IsNullOrEmpty() &&
                    !Settings.GetSetting("MailServerPassword").IsNullOrEmpty())
                {
                    client.UseDefaultCredentials = false;
                    NetworkCredential basicCredential = new NetworkCredential(
                        Settings.GetSetting("MailServerUserName"), Settings.GetSetting("MailServerPassword"));
                    client.Credentials = basicCredential;
                }
                else
                {
                    client.UseDefaultCredentials = false;
                }


                // Checking for setting of smtp port
                if (!Settings.GetSetting("MailServerPort").IsNullOrEmpty())
                {
                    // Assigning smtp port 
                    client.Port = int.Parse(Settings.GetSetting("MailServerPort"));
                }
                client.DeliveryMethod = SmtpDeliveryMethod.Network;


                // Checking for setting of smtp host address.
                if (!Settings.GetSetting("MailServer").IsNullOrEmpty())
                {
                    // Assign smtp address.
                    client.Host = Settings.GetSetting("MailServer");
                }
                mail.IsBodyHtml = true;
                // Assign mail subject text.
                mail.Subject = subject;

                // Assign body text.
                mail.Body = body;
                if (client.Port.ToString() != "" && !client.Host.IsNullOrEmpty())
                {
                    // Send mail
                    client.Send(mail);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, new Exception());
            }
        }
    }
}