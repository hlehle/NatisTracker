using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NatisTracker.Models;
using NatisTracker.ViewModels;
using System.Net.Mail;

namespace NatisTracker.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(Employee_Table objEmp)
        {
            if (ModelState.IsValid)
            {
                using (Intern_LeaveDBEntities db = new Intern_LeaveDBEntities())
                {
                    var emp = db.Employee_Table.Where(a => a.UserName.Equals(objEmp.UserName)
                    && a.Password.Equals(objEmp.Password)).FirstOrDefault();

                    if (emp != null)
                    {
                        Session["ID"] = emp.Employee_ID.ToString();
                        Session["Name"] = emp.Employee_Name.ToString();
                        Session["Surname"] = emp.Employee_Surname.ToString();
                        Session["Department"] = emp.Department.ToString();

                        var viewModel = new ViewModels.UserDetailViewModel();
                        viewModel.Employee_Name = emp.Employee_Name;
                        viewModel.Employee_Name = emp.Employee_Surname;

                        if (emp.User_Type == "Admin")
                        {
                            return RedirectToAction("AdminView", "Users");
                        }

                        else if (emp.User_Type == "EndUser")
                        {
                            if (emp.Department.Equals("Origination"))
                            {
                                return RedirectToAction("OriginationView", "Users");
                            }
                            return RedirectToAction("UserView", "Users");
                        }

                        else if (emp.User_Type == "Dealership")
                        {
                            return RedirectToAction("DealershipView", "Users");
                        }

                        else
                        {

                        }

                    }

                    else
                    {
                        Response.Write("<script LANGUAGE='JavaScript' >alert('Incorrect Login Credentials')</script>");
                    }
                }
            }

            return View(objEmp);
        }
    }
}