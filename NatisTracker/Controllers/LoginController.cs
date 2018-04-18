using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NatisTracker.Models;
using NatisTracker.ViewModels;
using System.Net.Mail;
using System.Web.Security;

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

                        if (emp.User_Type == "Admin")
                        {
                            return RedirectToAction("AdminView", "Users");
                        }

                        else if (emp.User_Type == "EndUser")
                        {
                            // Origination, Remarketing, Fines & Licensing and The Driver don't
                            // Request for Natis document and hence they have special pages as End Users

                            if (emp.Department.Equals("Origination"))
                            {
                                return RedirectToAction("OriginationView", "Users");
                            }

                            else if (emp.Department.Equals("Remarketing"))
                            {
                                return RedirectToAction("OriginationView", "Users");
                            }

                            else if (emp.Department.Equals("Fines & Licensing"))
                            {
                                return RedirectToAction("OriginationView", "Users");
                            }

                            else if (emp.Department.Equals("Driver"))
                            {
                                return RedirectToAction("OriginationView", "Users");
                            }

                            else
                            {
                                // Call Centre, Maturities and Legal Department catered here
                                // Departments above can make request for the Natis docs

                                return RedirectToAction("UserView", "Users");
                            }
                            
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
                        Response.Write("<script LANGUAGE='JavaScript' >alert('Incorrect User name or password')</script>");
                    }
                }
            }

            return View(objEmp);
        }

        public ActionResult LogOut()
        {

            FormsAuthentication.SignOut();

            Session.Abandon(); // it will clear the session at the end of request

            return RedirectToAction("Login", "Login");

        }
    }
}