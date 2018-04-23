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
        public void request(NatisRequests viewModel, string name, string surname, string department) { }

        public void respond(FormCollection form, string name, string surname)
        {
            string[] recordNumbers = form["RecordNumber"].Split(new char[] { ',' });
            string[] replies = form["Status"].Split(new char[] { ',' });

            using (Intern_LeaveDBEntities db = new Intern_LeaveDBEntities())
            {
                Models.RequestsData response = new RequestsData();

                for (int i = 0; i < recordNumbers.Length; i++)
                {
                    int recordNumber = int.Parse(recordNumbers[i]);
                    string reply = replies[i];

                    if (!reply.Equals("Pending"))
                    {
                        response = db.RequestsDatas.Single(u => u.RecordNumber == recordNumber);
                        response.Responder = name + " " + surname;
                        response.ReplyDate = DateTime.Now;
                        response.RequestStatus = reply.ToString();

                        db.SaveChanges();
                    }
                }
            }
        }
    }

}