using EnatisRepository.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace NatisTracker.ViewModels
{
    public class SystemEmailSender
    {
        public static void SendMail(string to,
                                    string subject,
                                    string body,
                                    Attachment[] attachments = null)
        {
            SendMail(new[] { to }, subject, body, attachments);
        }


        public static void SendMail(string[] to,
                                    string subject,
                                    string body,
                                    Attachment[] attachments = null)
        {
            var systemParams = new Intern_LeaveDBEntities().SystemParams.FirstOrDefault();

            string defaultSenderAddress = systemParams.SenderAddress;

            string defaultSmtpServer = systemParams.SmtpServer;

            string defaultSmtpServerFallback = systemParams.SmtpServerFallback;


            //if (system == null)
            //{
            try
            {
                var sender = new Comet.Email.EmailSender(defaultSmtpServer, 25);
                sender.SendMail(to, defaultSenderAddress, subject, body, attachments);
            }
            catch (Comet.Email.EmailException)
            {

                var sender = new Comet.Email.EmailSender(defaultSmtpServerFallback, 25);
                sender.SendMail(to, defaultSenderAddress, subject, body, attachments);
            }
            //}
            //else
            //{
            //    try
            //    {
            //        var sender = new EmailSender(defaultSmtpServerFallback, 25);
            //        sender.SendMail(to, defaultSenderAddress, subject, body, attachments);

            //    }
            //    catch (EmailException)
            //    {

            //        var sender = new EmailSender(defaultSmtpServer, 25);
            //        sender.SendMail(to, defaultSenderAddress, subject, body, attachments);
            //    }
            //}

        }
    }
}