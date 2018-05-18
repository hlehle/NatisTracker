using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NatisTracker.Models;
using System.Web.Mvc;
using NatisTracker.ViewModels;

namespace NatisTracker.Models
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
                        string subject = "To whom it may concern";
                        string body = "Your request has been Accepted";
                        SystemEmailSender.SendMail(to, subject, body);
                    }
                }
            }
        }
    }

}