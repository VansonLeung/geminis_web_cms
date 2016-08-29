using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using WebApplication2.Models;

namespace WebApplication2.Helpers
{
    public class EmailHelper
    {
        public static bool SendEmail(string emailTo, string mailbody, string subject)
        {
            var from = new MailAddress("geministest1@gmail.com");
            var to = new MailAddress(emailTo);

            var useDefaultCredentials = false;
            var enableSsl = true;
            var replyto = ""; // set here your email; 
            var userName = string.Empty;
            var password = string.Empty;
            var port = 587;
            var host = "smtp.gmail.com";

            userName = "geministest1@gmail.com"; // setup here the username; 
            password = "geministes"; // setup here the password; 
            bool.TryParse("false", out useDefaultCredentials); //setup here if it uses defaault credentials 
            bool.TryParse("true", out enableSsl); //setup here if it uses ssl 
            int.TryParse("587", out port); //setup here the port 
            host = "smtp.gmail.com"; //setup here the host 

            using (var mail = new MailMessage(from, to))
            {
                mail.Subject = subject;
                mail.Body = mailbody;
                mail.IsBodyHtml = true;

                mail.DeliveryNotificationOptions = DeliveryNotificationOptions.Delay |
                                                   DeliveryNotificationOptions.OnFailure |
                                                   DeliveryNotificationOptions.OnSuccess;

                using (var client = new SmtpClient())
                {
                    client.Host = host;
                    client.EnableSsl = true;
                    client.Port = port;
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential(userName, password);
                    client.Send(mail);
                }
            }

            return true;
        }


        public static bool SendEmailToAccount(Account account, string mailbody, string subject)
        {
            return SendEmail(account.Email, mailbody, subject);
        }


        public static bool SendEmailToAccountOnPasswordReset(Account account, String newPassword)
        {
            var mailbody = string.Format(
                "Dear {0} {1}, <br/><br/>" +
                "<p>Superadmin has reset your Geminis CMS login password. Here is your temporary password: {2}</p>" +
                "<p>Login by clicking <a href='http://localhost:51042/Account/Login'>HERE</a></p>" +
                "<p>Upon logging in, you will be prompted to assign your account a new password.</p>" +
                "<p>Geminis CMS Team</p>",
                account.Firstname,
                account.Lastname,
                newPassword
            );

            var subject = string.Format(
                "[Geminis] Superadmin has reset your Geminis CMS login password"
            );

            return SendEmail(account.Email, mailbody, subject);
        }





        public static bool SendEmailToAccountOnNewActivity(Account account, BaseArticle article, String action)
        {
            return false;
        }

    }
}