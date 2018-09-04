using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mail;
using System.Configuration;
using System.Text;

namespace ICP.SP.IntranetWeb.Utils
{
    public class EmailHelper
    {
        public void SendEmail(string from, string to, string[] cc, string subject, string body)
        {
            using (SmtpClient smtpClient = new SmtpClient())
            {
                smtpClient.Port = Convert.ToInt32(ConfigurationManager.AppSettings["MailPort"]);
                smtpClient.Host = ConfigurationManager.AppSettings["MailServer"];
                smtpClient.EnableSsl = false;
                smtpClient.UseDefaultCredentials = true;

                MailMessage mail = new MailMessage()
                {
                    From = new MailAddress(from),
                    Subject = subject,
                    IsBodyHtml = true,
                    Body = body,
                    BodyEncoding = Encoding.UTF8,
                    Priority = MailPriority.Normal
                };
                try
                {
                    mail.To.Add(to);
                    if (cc != null)
                        for (var i = 0; i < cc.Length; i++) mail.CC.Add(new MailAddress(cc[i]));
                    try
                    {
                        smtpClient.Send(mail);
                    }
                    catch (Exception ex)
                    {
                        ex.ToString();
                    }
                }
                finally
                {
                    if (mail != null)
                        mail.Dispose();
                }
            }
        }
    }
}