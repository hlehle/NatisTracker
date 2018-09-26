using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using EnatisRepository.Repo;
using Comet.Email;
using System.Net.Mail;

namespace EscalationEmailService
{
    public partial class EscalationEmail : ServiceBase
    {
        Timer timeDelay;
        public EscalationEmail()
        {
            InitializeComponent();
        }
        public void SendEscalationEmails(object sender, ElapsedEventArgs e)
        {
            // This is to notify a user(Origination) that natis document must be submitted back to the admin
            using (Intern_LeaveDBEntities db = new Intern_LeaveDBEntities())
            {
                var deliveries = db.SentIN_Delivery.Where(a => a.CourierStatus.Equals("Received")).ToList();

                foreach (var item in deliveries)
                {
                    if (DateTime.Now.Subtract(item.DateReceived.Value).TotalDays > 3)
                    {
                        string to = db.EmployeeDatas.Where(a => a.ContactName.Equals(item.PackageRecipient)).Select(a => a.Email).FirstOrDefault();
                        string subject = "Natis Document(s) Due";

                        string body = "Hi,\nPlease note that the natis document(s) for Package " + item.PackageNumber +
                                      " with " + item.Quantity + " Document(s) are due to be submitted to the admin.\n Thank you";

                        SystemEmailSender.SendMail(to, subject, body);
                    }
                }

                // This is to notify a user(Legal) that natis document must be submitted back to the admin
                var TickBoxData = db.TickBoxDatas.Where(a => a.ReceivedDate.HasValue && a.RecipientDepartment.Equals("Legal"));
                var CollectionDates = TickBoxData.Select(a => a.ReceivedDate).ToList();
                var requesterNames = TickBoxData.Select(a => a.RecipientName).ToList();

                for (int i = 0; i < CollectionDates.Count; i++)
                {
                    var id = TickBoxData.ToList()[i].TableId;
                    var requesterContractNumbers = db.ContractNumbers.Where(a => a.TableId == id).Select(a => a.ContractNumber1).ToList();

                    for (int j = 0; j < requesterContractNumbers.Count; j++)
                    {
                        var contractNumber = requesterContractNumbers[j];
                        var ContractData = db.ContractsDatas.Where(a => a.ContractNumber == contractNumber).FirstOrDefault();
                        var vin = ContractData.VinNumber;
                        var natis = db.NatisDatas.Where(a => a.VinNumber == vin).FirstOrDefault();
                        var name = requesterNames[i];
                        var requsterEmail = db.EmployeeDatas.Where(a => a.ContactName == name).Select(a => a.Email).FirstOrDefault();

                        var days = DateTime.Now.Subtract(CollectionDates[i].Value).Minutes;
                        if (days > 3 && natis.NatisLocation == "Legal")
                        {
                            // Send to user
                            string to_User = requsterEmail;
                            string subject_User = "Natis Document(s) Due";

                            string body_User = "Hi,\nPlease note that the natis document you requested " + days + " days ago is due to be submitted to the system admin." +
                                               "\nThank you.";

                            SystemEmailSender.SendMail(to_User, subject_User, body_User);

                            // Send to Admin
                            var Admins = db.EmployeeDatas.Where(a => a.User_Type.Equals("Admin")).Select(a => a.Email).ToArray();
                            string[] to_Admin = Admins;
                            string subject_Admin = "Escalation Notification";

                            string body_Admin = "Hi, \nPlease note that an Escalation email has been sent to " + name + ", the natis document you requested " + days +
                                                " days ago is due to be submitted to the system admin.\nThank you.";

                            SystemEmailSender.SendMail(to_Admin, subject_Admin, body_Admin);
                        }
                    }
                    
                }
            }  

        }
        protected override void OnStart(string[] args)
        {
            //LogService("Service Started");
            timeDelay = new Timer();
            timeDelay.Elapsed += new ElapsedEventHandler(SendEscalationEmails);
            timeDelay.Interval = 1000 * 60 * 60 * 24;

#if (DEBUG)
            timeDelay.Interval = 1000 * 60 * 60;
#endif

            timeDelay.Enabled = true;
            timeDelay.Start();

        }
        public void StartDebug()
        {
            OnStart(null);
        }
        public void StopDebug()
        {
            OnStop();
        }
        protected override void OnStop()
        {
            LogService("Service Stoped");
            timeDelay.Enabled = false;
        }
        private void LogService(string content)
        {
            FileStream fs = new FileStream(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\TestServiceLog.txt", FileMode.OpenOrCreate, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs);
            sw.BaseStream.Seek(0, SeekOrigin.End);
            sw.WriteLine(content + " at " + DateTime.Now.ToShortTimeString());
            sw.Flush();
            sw.Close();
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
                var systemParams = new Intern_LeaveDBEntities().SystemParams.FirstOrDefault();

                string defaultSenderAddress = systemParams.SenderAddress;

                string defaultSmtpServer = systemParams.SmtpServer;

                string defaultSmtpServerFallback = systemParams.SmtpServerFallback;


                //if (system == null)
                //{
                try
                {
                    var sender = new EmailSender(defaultSmtpServer, 25);
                    sender.SendMail(to, defaultSenderAddress, subject, body, attachments);
                }
                catch (EmailException)
                {
                    var sender = new EmailSender(defaultSmtpServerFallback, 25);
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
}
