using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using NatisTracker.Models;
using System.Timers;
using System.Web.Configuration;

namespace NatisTracker
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private static double TimerIntervalInMilliseconds = Convert.ToDouble(WebConfigurationManager.AppSettings["TimerIntervalInMilliseconds"]);
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            SetLicenses();

            Timer timer = new Timer(TimerIntervalInMilliseconds);
            timer.Enabled = true;
            timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
            timer.Start();
            
        }

        public void SetLicenses()
        {
            var license = new Aspose.BarCode.License();
            license.SetLicense("Aspose.Total.lic");
        }

        static void SendEscalation()
        {
            using (Intern_LeaveDBEntities db = new Intern_LeaveDBEntities())
            {
                // This is to notify a user(Origination) that natis document must be submitted back to the admin
                var deliveries = db.SentIN_Delivery.Where(a => a.CourierStatus.Equals("Received")).ToList();

                foreach (var item in deliveries)
                {
                    if (DateTime.Now.Subtract(item.DateReceived.Value).TotalDays > 1)
                    {
                        string to = db.EmployeeDatas.Where(a => a.ContactName.Equals(item.PackageRecipient)).Select(a => a.Email).FirstOrDefault();
                        string subject = "Natis Document(s) Due";

                        string body = "Hi, \n Please note that the natis document(s) for Package "+item.PackageNumber+
                                      " with "+item.Quantity+" Document(s) are due to be submitted to the admin. \n Thank you";

                        SystemEmailSender.SendMail(to, subject, body);
                    }
                }

                // This is to notify a user(Legal) that natis document must be submitted back to the admin
                var requests = db.RequestsDatas.Where(a => a.ReplyDate.HasValue && a.RequesterDepartment.Equals("Legal"));
                var replyDates = requests.Select(a => a.ReplyDate).ToList();
                var requesterContractNumbers = requests.Select(a => a.ContractNo).ToList();
                var requesterNames = requests.Select(a => a.RequesterName).ToList();
                
                for (int i = 0; i < replyDates.Count; i++)
                {
                    var contractNumber = requesterContractNumbers[i];
                    var ContractData = db.ContractsDatas.Where(a => a.ContractNumber == contractNumber).FirstOrDefault();
                    var vin = ContractData.VinNumber;
                    var natis = db.NatisDatas.Where(a => a.VinNumber == vin).FirstOrDefault();
                    var name = requesterNames[i];
                    var requsterEmail = db.EmployeeDatas.Where(a => a.ContactName == name).Select(a => a.Email).FirstOrDefault();

                    var days = DateTime.Now.Subtract(replyDates[i].Value).TotalDays;
                    if (days > 1 && natis.NatisLocation == "Legal")
                    {
                        // Send to user
                        string to_User = requsterEmail;
                        string subject_User = "Natis Document(s) Due";

                        string body_User = "Hi, \n Please note that the natis document you requested "+days+" days ago is due to be submitted to the system admin."+
                                      "\n Thank you.";

                        SystemEmailSender.SendMail(to_User, subject_User, body_User);

                        // Send to Admin
                        var Admins = db.EmployeeDatas.Where(a => a.User_Type.Equals("Admin")).Select(a => a.Email).ToArray();
                        string[] to_Admin = Admins;
                        string subject_Admin = "Escalation Notification";

                        string body_Admin = "Hi, \n Please note that an Escalation email has been sent to "+name+" the natis document you requested " + days + 
                                            " days ago is due to be submitted to the system admin.\n Thank you.";

                        SystemEmailSender.SendMail(to_Admin, subject_Admin, body_Admin);
                    }
                }
            }
             
        }

        static void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            DateTime MyScheduledRunTime = DateTime.Parse(WebConfigurationManager.AppSettings["TimerStartTime"]);
            DateTime CurrentSystemTime = DateTime.Now;
            DateTime LatestRunTime = MyScheduledRunTime.AddMilliseconds(TimerIntervalInMilliseconds);
            if ((CurrentSystemTime.CompareTo(MyScheduledRunTime) >= 0) && (CurrentSystemTime.CompareTo(LatestRunTime) <= 0))
            {
                // RUN YOUR PROCESSES HERE
                SendEscalation();
            }
        }
    }
}
