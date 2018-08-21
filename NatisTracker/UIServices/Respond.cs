using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NatisTracker.ViewModels;
using EnatisRepository.Repo;
using Comet.Email;

namespace NatisTracker.Requests
{
    public class Respond : INatisRequest
    {
        public NatisRequests request(NatisRequests viewModel, string name,string department, string email) { return viewModel; }

        public void respond(FormCollection form, string name, string email)
        {
            string[] recordNumbers = form["RecordNumber"].Split(new char[] { ',' });
            string[] replies = form["Status"].Split(new char[] { ',' });

            using (Intern_LeaveDBEntities db = new Intern_LeaveDBEntities())
            {
                RequestsData response = new RequestsData();

                for (int i = 0; i < recordNumbers.Length; i++)
                {
                    int recordNumber = int.Parse(recordNumbers[i]);
                    string reply = replies[i];

                    if (!reply.Equals("Pending"))
                    {
                        response = db.RequestsDatas.Single(u => u.RecordNumber == recordNumber);
                        response.Responder = name;
                        response.ReplyDate = DateTime.Now;
                        response.RequestStatus = reply.ToString();

                        db.SaveChanges();

                        string to = email;
                        string subject = "Request Notification Update";
                        string body = "Your request for the Natis document of contract Number "+response.ContractNo+
                                      " has been Accepted and doucument is ready for collection";
                        SystemEmailSender.SendMail(to, subject, body);
                    }
                }
            }
        }
    }

}