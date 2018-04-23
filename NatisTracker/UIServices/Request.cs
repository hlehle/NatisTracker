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
        public void request(NatisRequests viewModel, string name, string surname, string department)
        {
            using (Intern_LeaveDBEntities db = new Intern_LeaveDBEntities())
            {
                RequestsData database = new RequestsData();
                database.RequesterName = name + " " + surname;
                database.RequestDate = DateTime.Now;
                database.RequesterDepartment = department;
                database.RequestStatus = "Pending";
                database.ContractNo = viewModel.contractNo;

                db.RequestsDatas.Add(database);
                db.SaveChanges();


                //Response.Write("<script LANGUAGE='JavaScript' >alert('Confirmation Password and New Password do not Match')</script>");
                //BootstrapAlert(lblMsg, "Congrats! You've won a dismissable booty message.", BootstrapAlertType.Success, True);
            }
        }

        public void respond(FormCollection form, string name, string surname) { }
    }
}