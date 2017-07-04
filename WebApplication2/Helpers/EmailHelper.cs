using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using WebApplication2.Context;
using WebApplication2.Models;

namespace WebApplication2.Helpers
{
    public class EmailHelper
    {
        public static bool SendEmail(List<string> emailTo, string mailbody, string subject)
        {
            var useDefaultCredentials = false;
            var enableSsl = true;
            var method = SmtpDeliveryMethod.Network;

            var smtp_skip_email = ConstantDbContext.getInstance().findActiveByKeyNoTracking("SMTP_skip_email");
            if (smtp_skip_email != null && smtp_skip_email.Value == "1")
            {
                AuditLogDbContext.getInstance().createAuditLog(new AuditLog
                {
                    action = "[EMAIL SKIPPED]",
                    remarks = subject + " " + mailbody,
                    is_private = false,
                });
                return true;
            }

            var c_from = ConstantDbContext.getInstance().findActiveByKeyNoTracking("SMTP_from");
            var c_userName = ConstantDbContext.getInstance().findActiveByKeyNoTracking("SMTP_username");
            var c_password = ConstantDbContext.getInstance().findActiveByKeyNoTracking("SMTP_password");
            var c_port = ConstantDbContext.getInstance().findActiveByKeyNoTracking("SMTP_port");
            var c_host = ConstantDbContext.getInstance().findActiveByKeyNoTracking("SMTP_host");
            var c_domain = ConstantDbContext.getInstance().findActiveByKeyNoTracking("SMTP_domain");
            var c_smtpClientHost = ConstantDbContext.getInstance().findActiveByKeyNoTracking("SMTP_smtpClientHost");
            var c_smtpClientPort = ConstantDbContext.getInstance().findActiveByKeyNoTracking("SMTP_smtpClientPort");
            var c_smtpSSL = ConstantDbContext.getInstance().findActiveByKeyNoTracking("SMTP_ssl");

            /*
            var from = new MailAddress("uattest@geminisgroup.com", "Geminis");
            var userName = "uattest@geminisgroup.com";
            var password = "UAT@2016gem";
            var port = 587;
            var host = "mail.geminisgroup.com";
            var domain = "fbgemini";
            */

            var from = new MailAddress("geministest1@gmail.com", "Geminis");
            var userName = "geministest1@gmail.com";
            var password = "geministes";
            var port = 587;
            var host = "smtp.gmail.com";
            string domain = null;
            string smtpClientHost = null;
            var smtpClientPort = 465;
            var smtpSSL = 0;


            if (c_from != null && c_from.Value != null && c_from.Value != "") { from = new MailAddress(c_from.Value, "Geminis"); }
            if (c_userName != null && c_userName.Value != null && c_userName.Value != "") { userName = c_userName.Value; }
            if (c_password != null && c_password.Value != null && c_password.Value != "") { password = c_password.Value; }
            if (c_port != null && c_port.Value != null && c_port.Value != "") { port = int.Parse(c_port.Value); }
            if (c_host != null && c_host.Value != null && c_host.Value != "") { host = c_host.Value; }
            if (c_domain != null && c_domain.Value != null && c_domain.Value != "") { domain = c_domain.Value; }
            if (c_smtpClientHost != null && c_smtpClientHost.Value != null && c_smtpClientHost.Value != "") { smtpClientHost = c_smtpClientHost.Value; }
            if (c_smtpClientPort != null && c_smtpClientPort.Value != null && c_smtpClientPort.Value != "") { smtpClientPort = int.Parse(c_smtpClientPort.Value); }
            if (c_smtpSSL != null && c_smtpSSL.Value != null && c_smtpSSL.Value != "") { smtpSSL = int.Parse(c_smtpSSL.Value); }

            if (smtpSSL == 0)
            {
                enableSsl = false;
            }
            else
            {
                enableSsl = true;
            }

            using (var mail = new MailMessage())
            {
                mail.From = from;

                foreach (string to in emailTo)
                {
                    mail.To.Add(to);
                }

                mail.Subject = subject;
                mail.Body = mailbody;
                mail.IsBodyHtml = true;

                mail.DeliveryNotificationOptions = DeliveryNotificationOptions.Delay |
                                                   DeliveryNotificationOptions.OnFailure |
                                                   DeliveryNotificationOptions.OnSuccess;

                SmtpClient _client = null;
                if (smtpClientHost == null)
                {
                    _client = new SmtpClient();
                }
                else
                {
                    _client = new SmtpClient(smtpClientHost, smtpClientPort);
                }

                using (var client = _client)
                {
                    client.Host = host;
                    client.EnableSsl = enableSsl;
                    client.Port = port;
                    client.UseDefaultCredentials = useDefaultCredentials;

                    if (domain != null)
                    {
                        client.Credentials = new NetworkCredential(userName, password, domain);
                    }
                    else
                    {
                        client.Credentials = new NetworkCredential(userName, password);
                    }

                    client.DeliveryMethod = method;
                    try
                    {
                        client.Send(mail);
                    }
                    catch (Exception e)
                    {
                        AuditLogDbContext.getInstance().createAuditLog(new WebApplication2.Models.AuditLog
                        {
                            action = "[EMAIL API TEST]",
                            remarks = "Response Exception: " + e.Message + " " + userName + " " + port + " " + host,
                            is_private = false,
                        });
                    }
                }
            }

            return true;
        }


        public static bool SendEmailToAccount(Account account, string mailbody, string subject)
        {
            return SendEmail(new List<string> { account.Email }, mailbody, subject);
        }


        public static bool SendEmailToAccounts(List<Account> accounts, string mailbody, string subject)
        {
            List<string> emails = new List<string>();
            foreach (Account account in accounts)
            {
                emails.Add(account.Email);
            }
            return SendEmail(emails, mailbody, subject);
        }


        public static bool SendEmailToAccountOnPasswordReset(Account account, String newPassword)
        {
            var mailbody = string.Format(
                "Dear {0} {1}, <br/><br/>" +
                "<p>Superadmin has reset your Geminis CMS login password. Here is your temporary password: {2}</p>" +
                "<p>Login by clicking <a href='" + ServerHelper.GetSiteRoot() + "/Account/Login'>HERE</a></p>" +
                "<p>Upon logging in, you will be prompted to assign your account a new password.</p>" +
                "<p>Geminis CMS Team</p>",
                account.Firstname,
                account.Lastname,
                newPassword
            );

            var subject = string.Format(
                "[Geminis] Superadmin has reset your Geminis CMS login password"
            );

            return SendEmail(new List<string> { account.Email }, mailbody, subject);
        }



        public static bool SendEmailToAccountOnPasswordCreate(Account account, String newPassword)
        {
            var mailbody = string.Format(
                "Dear {0} {1}, <br/><br/>" +
                "<p>Superadmin has created your Geminis CMS login account. Here is your temporary password: {2}</p>" +
                "<p>Login by clicking <a href='" + ServerHelper.GetSiteRoot() + "/Account/Login'>HERE</a></p>" +
                "<p>Upon logging in, you will be prompted to assign your account a new password.</p>" +
                "<p>Geminis CMS Team</p>",
                account.Firstname,
                account.Lastname,
                newPassword
            );

            var subject = string.Format(
                "[Geminis] Superadmin has created your Geminis CMS login account"
            );

            return SendEmail(new List<string> { account.Email }, mailbody, subject);
        }




        public static bool SendEmailToSuperadminAccountsOnPasswordForget(Account account)
        {
            var mailbody = string.Format(
                "Dear Superadmins, <br/><br/>" +
                "<p>Account: "+account.Username+" has requested a password reset.</p>" +
                "<p>Login your superadmin account by clicking <a href='" + ServerHelper.GetSiteRoot() + "/Account/Login'>HERE</a></p>" +
                "<p>Upon logging in, you can change "+account.Username+"'s password.</p>" +
                "<p>Geminis CMS Team</p>"
            );

            var subject = string.Format(
                "[Geminis] Account: " + account.Username + " has requested a password reset."
            );

            var superadmins = AccountDbContext.getInstance().findAccountsByRole("superadmin");

            return SendEmailToAccounts(superadmins, mailbody, subject);
        }


        public static bool SendEmailToAccountOnNewActivity(Account account, BaseArticle article, String action)
        {
            return false;
        }

        

    }
}