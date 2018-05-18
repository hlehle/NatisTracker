using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using NatisTracker.Models;

namespace NatisTracker
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            SetLicenses();
        }

        public void SetLicenses()
        {
            var license = new Aspose.BarCode.License();
            license.SetLicense("Aspose.Total.lic");

            SendEscalation();
        }

        public void SendEscalation()
        {
            using (Intern_LeaveDBEntities db = new Intern_LeaveDBEntities())
            {
                var deliveries = db.SentIN_Delivery.Where(a => a.CourierStatus.Equals("Received")).ToList();

                foreach (var item in deliveries)
                {
                    if (DateTime.Now.Subtract(item.DateReceived.Value).TotalMinutes > 5)
                    {
                        string to = db.EmployeeDatas.Where(a => a.ContactName.Equals(item.PackageRecipient)).Select(a => a.Email).FirstOrDefault();
                        string subject = "Please Ignore";
                        string body = "Notification Test email for eNatis escalations";
                        SystemEmailSender.SendMail(to, subject, body);
                    }
                }

                var replyDates = db.RequestsDatas.Where(a => a.RequesterDepartment == "Legal").Select(a => a.ReplyDate).ToList();
                //var requesterDepartment = db.RequestsDatas.Where(a => a.RequesterDepartment == "Legal").Select(a => a.RequesterDepartment).ToList();
                var requesterContractNumbers = db.RequestsDatas.Where(a => a.RequesterDepartment == "Legal").Select(a => a.ContractNo).ToList();
                var requesterNames = db.RequestsDatas.Where(a => a.RequesterDepartment == "Legal").Select(a => a.RequesterName).ToList();
                
                for (int i = 0; i < replyDates.Count; i++)
                {
                    var contractNumber = requesterContractNumbers[i];
                    var ContractData = db.ContractsDatas.Where(a => a.ContractNumber == contractNumber).FirstOrDefault();
                    var vin = ContractData.VinNumber;
                    var natis = db.NatisDatas.Where(a => a.VinNumber == vin).FirstOrDefault();
                    var name = requesterNames[i];
                    var requsterEmail = db.EmployeeDatas.Where(a => a.ContactName == name).Select(a => a.Email).FirstOrDefault();

                    if (DateTime.Now.Subtract(replyDates[i].Value).TotalMinutes > 5 && natis.NatisLocation == "Legal")
                    {
                        string to = requsterEmail;
                        string subject = "Natis Doc Due";
                        string body = "Natis document is due";
                        SystemEmailSender.SendMail(to, subject, body);
                    }
                }
            }
             
        }
    }
}
