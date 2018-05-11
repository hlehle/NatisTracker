using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NatisTracker.Models;
using NatisTracker.ViewModels;
using System.Web.Mvc;

namespace NatisTracker.Models
{
    public class Request : INatisRequest
    {
        public NatisRequests request(NatisRequests viewModel, string name, string department)
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


                return viewModel;
            }
        }

        public void respond(FormCollection form, string name) { }
    }
}