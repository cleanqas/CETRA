using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using MailMessage = System.Net.Mail.MailMessage;

namespace CETRA
{
    public class EmailSender
    {
        string MailSmtpHost { get; set; }
        int MailSmtpPort { get; set; }
        string MailSmtpUsername { get; set; }
        string MailSmtpPassword { get; set; }
        string MailFrom { get; set; }
        string MailSubject { get; set; }

        public EmailSender()
        {
            MailSmtpHost = ConfigurationManager.AppSettings["MailSmtpHost"];
            MailSmtpPort = Convert.ToInt32(ConfigurationManager.AppSettings["MailSmtpPort"]);
            MailSmtpUsername = ConfigurationManager.AppSettings["MailSmtpUsername"];
            MailSmtpPassword = ConfigurationManager.AppSettings["MailSmtpPassword"];
            MailFrom = ConfigurationManager.AppSettings["MailFrom"];
            MailSubject = ConfigurationManager.AppSettings["MailSubject"];
        }

        private bool SendEmail(string to, string body, string action = "")
        {
            if (!string.IsNullOrEmpty(action))
                MailSubject = string.Format("{0}:{1}", MailSubject, action);
            MailMessage mail = new MailMessage(MailFrom, to, MailSubject, body);
            var alternameView = AlternateView.CreateAlternateViewFromString(body, new ContentType("text/html"));
            mail.AlternateViews.Add(alternameView);

            var smtpClient = new SmtpClient(MailSmtpHost, MailSmtpPort);
            smtpClient.Credentials = new NetworkCredential(MailSmtpUsername, MailSmtpPassword);
            try
            {
                smtpClient.Send(mail);
            }
            catch (Exception e)
            {
                //Log error here
                return false;
            }

            return true;
        }

        public bool SendToBranchOperator(string branchCode)
        {
            string toAddress = string.Format("{0}_{1}@{2}", ConfigurationManager.AppSettings["branchOperatorEmailPrefix"], branchCode, ConfigurationManager.AppSettings["emailDomail"]);
            string messageBody = ConfigurationManager.AppSettings["BranchOperatorMessage"];
            return SendEmail(toAddress, messageBody);
        }

        public bool SendToBranchVerifier(string branchCode)
        {
            string toAddress = string.Format("{0}_{1}@{2}", ConfigurationManager.AppSettings["branchVerifierEmailPrefix"], branchCode, ConfigurationManager.AppSettings["emailDomail"]);
            string messageBody = ConfigurationManager.AppSettings["BranchVerifierMessage"];
            return SendEmail(toAddress, messageBody);
        }

        public bool SendToHOOperator(string branchCode, string action)
        {
            string toAddress = string.Format("{0}@{1}", ConfigurationManager.AppSettings["hoOperatorEmailPrefix"], ConfigurationManager.AppSettings["emailDomail"]);
            string messageBody = string.Format(ConfigurationManager.AppSettings["HOOperatorMessage"], branchCode);
            return SendEmail(toAddress, messageBody, action);
        }
    }
}