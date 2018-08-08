using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NatisTracker.Models;
using NatisTracker.ViewModels;
using EnatisRepository.Repo;
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

                var requestedContract = database.ContractNo;
                var con = db.ContractsDatas.Where(a => a.ContractNumber.Equals(requestedContract)).Select(a => a.ContractNumber);

                if (con != null)
                {
                    db.RequestsDatas.Add(database);
                    db.SaveChanges();

                    string to = email;
                    string subject = "Request Notification";
                    string body = "Your request for the Natis document of the contract number "+database.ContractNo+" has been received";
                    SystemEmailSender.SendMail(to, subject, body);

                    string[] toArray = db.EmployeeDatas.Where(a => a.User_Type == "Admin").Select(a => a.Email).ToArray();
                    string subject1 = "Request Notification";
                    string body1 = "A request for a Natis Document of contract number "+database.ContractNo+" has been Submitted";
                    SystemEmailSender.SendMail(toArray, subject1, body1);

                    return viewModel;
                }

                // Validation for wrong requests will go here
                else
                {
                    return null;
                }
                
            }
        }

        public void respond(FormCollection form, string name, string email) { }
    }
}