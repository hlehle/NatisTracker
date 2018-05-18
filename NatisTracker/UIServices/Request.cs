using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NatisTracker.Models;
using NatisTracker.ViewModels;
using System.Web.Mvc;
using Comet.Email;

namespace NatisTracker.Models
{
    public class Request : INatisRequest
    {
        public NatisRequests request(NatisRequests viewModel, string name, string department, string email)
        {
            using (Intern_LeaveDBEntities db = new Intern_LeaveDBEntities())
            {
                RequestsData database = new RequestsData();
                database.RequesterName = name;
                database.RequestDate = DateTime.Now;
                database.RequesterDepartment = department;
                database.RequestStatus = "Pending";
                database.ContractNo = viewModel.contractNo;

                db.RequestsDatas.Add(database);
                db.SaveChanges();

                string to = email;
                string subject = "To whom it may concern";
                string body = "Your request has been received";
                SystemEmailSender.SendMail(to, subject, body);

                string[] toArray = db.EmployeeDatas.Where(a => a.User_Type == "Admin").Select(a => a.Email).ToArray();
                string subject1 = "To whom it may concern";
                string body1 = "A request for a Natis Document has been Sent";
                SystemEmailSender.SendMail(toArray, subject1, body1);

                return viewModel;
            }
        }

        public void respond(FormCollection form, string name, string email) { }
    }
}