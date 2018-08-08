using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net.Mail;

namespace NatisTrackerServiceLib
{
    public class ServiceLib
    {
        
        static void DoThis()
        {
            SystemEmailSender.SendMail("","","",null);
        }


    }

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
            //string defaultSenderAddress = GetSenderAddress(systemParams);

            //string defaultSmtpServer = GetSmtpServer(systemParams);

            //string defaultSmtpServerFallback = GetSmtpServerFallback(systemParams);


            //if (system == null)
            //{
            try
            {
                var sender = new Comet.Email.EmailSender("141.113.103.104", 25);
                sender.SendMail(to, "mbfs_systems@daimler.com", subject, body, attachments);
            }
            catch (Comet.Email.EmailException)
            {

                var sender = new Comet.Email.EmailSender("53.151.100.102", 25);
                sender.SendMail(to, "mbfs_systems@daimler.com", subject, body, attachments);
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
