using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using NatisTracker.ViewModels;
using NatisTracker.Models;
using Oracle.DataAccess.Client;
using Oracle.DataAccess.Types;
using System.Web.Mvc;
using Aspose.BarCode.BarCodeRecognition;
using System.Data;
using NatisTracker.UIServices;

namespace NatisTracker.Controllers
{
    public class ProcessController : Controller
    {
        // GET: Process
        [HttpGet]
        public ActionResult LoadNewNatis()
        {
            return View(new NatisDataViewModel());
        }

        [HttpPost]
        public ActionResult LoadNewNatis(FormCollection form)
        {
            var appForm = new NatisDataViewModel();
            TryUpdateModel<NatisDataViewModel>(appForm, form);

            if (ModelState.IsValid)
            {
                var a = Request.Files.Count;
                appForm.file = Request.Files[0];

                NatisDataPopulate n = new NatisDataPopulate();
                bool isPopulated = n.populate(appForm);

                if (isPopulated)
                {
                    return View(appForm);
                }

                else
                {
                    NatisDataViewModel p = null;
                    return View(p);
                }
            }

            return View(appForm);
        }

        public ActionResult GenerateReport()
        {
            return View();
        }

        [HttpGet]
        public ActionResult RequestResponse()
        {
            Intern_LeaveDBEntities db = new Intern_LeaveDBEntities();
            return View(from requests in db.NatisRequests select requests);
        }

        [HttpPost]
        public ActionResult RequestResponse(FormCollection form)
        {
            if (ModelState.IsValid)
            {
                var r = new UIServices.NatisRequest();
                r.respond(form, @Session["Name"].ToString(),Session["Surname"].ToString());
            }
            Intern_LeaveDBEntities database = new Intern_LeaveDBEntities();
            return View(from requests in database.NatisRequests select requests);

        }
        [HttpGet]
        public ActionResult Scan_Back_To_Safe()
        {
            return View(new LogInfoViewModel());
        }

        [HttpPost]
        public ActionResult Scan_Back_To_Safe(FormCollection form)
        {
            var appForm = new LogInfoViewModel();
            TryUpdateModel<LogInfoViewModel>(appForm, form);

            if (ModelState.IsValid)
            {
                var a = Request.Files.Count;
                appForm.file = Request.Files[0];

                ScanNatis s = new ScanNatis();
                s.toSafe(appForm, @Session["Name"].ToString(), @Session["Surname"].ToString());
            }
            
            return View(appForm);
        }

        public ActionResult Scan_To_Collect()
        {
            return View(new LogInfoViewModel());
        }

        [HttpPost]
        public ActionResult Scan_To_Collect(FormCollection form)
        {
            var appForm = new LogInfoViewModel();
            TryUpdateModel<LogInfoViewModel>(appForm, form);

            if (ModelState.IsValid)
            {
                var a = Request.Files.Count;
                appForm.file = Request.Files[0];

                ScanNatis c = new ScanNatis();
                c.Collect(appForm, Session["Name"].ToString(), Session["Surname"].ToString(), Session["Department"].ToString());
            }

                return View(appForm);
        }

        public ActionResult RequestNatis()
        {

            return View(new NatisRequests());
        }

        [HttpPost]
        public ActionResult RequestNatis(FormCollection form)
        {
            var appForm = new NatisRequests();
            TryUpdateModel<NatisRequests>(appForm, form);

            if (ModelState.IsValid)
            {
                var r = new UIServices.NatisRequest();
                r.request(appForm, Session["Name"].ToString(), Session["Surname"].ToString(), Session["Department"].ToString());
            }

                return View(appForm);
        }

    }
}