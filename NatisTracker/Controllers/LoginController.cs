using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EnatisRepository.Repo;
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
        public ActionResult Login(EmployeeDataViewModel objEmp)
        {
            if (ModelState.IsValid)
            {
                using (Intern_LeaveDBEntities db = new Intern_LeaveDBEntities())
                {
                    var emp = db.EmployeeDatas.Where(a => a.UserName.Equals(objEmp.UserName)
                    && a.Password.Equals(objEmp.Password)).FirstOrDefault();

                    if (emp != null)
                    {
                        Session["ID"] = emp.UserId.ToString();
                        Session["Name"] = emp.ContactName.ToString();
                        Session["Email"] = emp.Email.ToString();
                        Session["Department"] = emp.Department.ToString();

                        if ((bool)emp.IsChangePassword)
                        {
                            return RedirectToAction("ChangePassword");
                        }

                        if (emp.User_Type == "Admin")
                        {
                            return RedirectToAction("AdminView", "Users");
                        }

                        else if (emp.User_Type == "EndUser")
                        {
                            //Origination is a special case and soon will be moved to SinBin

                            if (emp.Department.Equals("Contracts Origination"))
                            {
                                return RedirectToAction("OriginationView", "Users");
                            }

                            else if (emp.Department.Equals("Fleet Services"))
                            {
                                return RedirectToAction("DriverView", "Users");
                            }

                            else
                            {
                                return RedirectToAction("UserView", "Users");
                            }
                            
                        }

                        else
                        {

                        }

                    }

                    else
                    {
                        Response.Write("<script LANGUAGE='JavaScript' >alert('Incorrect User name or password')</script>");
                        return RedirectToAction("Login", "Login");
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

        public ActionResult ChangePassword()
        {
            var id = Session["ID"].ToString();
            var view = new Intern_LeaveDBEntities().EmployeeDatas.Where(a => a.UserId == id).Select(a => a.UserId);
            var changePassword = new ChangePasswordViewModel();
            changePassword.UserId = id;
            return View(changePassword);
        }
        [HttpPost]
        public ActionResult ChangePassword(int? i)
        {
            var changePassword = new ChangePasswordViewModel();
            TryUpdateModel(changePassword);
            using (Intern_LeaveDBEntities db = new Intern_LeaveDBEntities())
            {
                if (changePassword.Password.Equals(changePassword.ConfirmPassword))
                {
                    var id = changePassword.UserId;
                    var empData = db.EmployeeDatas.Where(a => a.UserId == id).FirstOrDefault();
                    empData.Password = changePassword.Password;
                    empData.IsChangePassword = false;
                    db.SaveChanges();

                    return RedirectToAction("Login", "Login");
                }
                else
                {
                    return View(changePassword);
                }
                
            }
            
        }
    }
}