using NatisTracker.Models;
using NatisTracker.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NatisTracker.UIServices
{
    public class NatisRequest
    {
        public void request(NatisRequests appForm, string name, string surname, string department)
        {
            using (Intern_LeaveDBEntities db = new Intern_LeaveDBEntities())
            {
                Models.NatisRequest database = new Models.NatisRequest();
                database.RequesterName = name + " " + surname;
                database.RequestDate = DateTime.Now;
                database.RequesterDepartment = department;
                database.RequestStatus = "Pending";
                database.ContractNo = appForm.contractNo;

                db.NatisRequests.Add(database);
                db.SaveChanges();
            }
        }

        public void respond(FormCollection form, string name, string surname)
        {
            string[] recordNumbers = form["RecordNumber"].Split(new char[] { ',' });
            string[] replies = form["Status"].Split(new char[] { ',' });

            using (Intern_LeaveDBEntities db = new Intern_LeaveDBEntities())
            {
                Models.NatisRequest response = new Models.NatisRequest();

                for (int i = 0; i < recordNumbers.Length; i++)
                {
                    int recordNumber = int.Parse(recordNumbers[i]);
                    string reply = replies[i];

                    response = db.NatisRequests.Single(u => u.RecordNumber == recordNumber);
                    response.Responder = name + " " + surname;
                    response.ReplyDate = DateTime.Now;
                    response.RequestStatus = reply.ToString();

                    db.SaveChanges();
                }
            }
        }
    }
}