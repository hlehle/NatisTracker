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
using Excel = Microsoft.Office.Interop.Excel;
using System.Reflection;

namespace NatisTracker.Controllers
{
    public class ProcessController : Controller
    {
        // GET: Process
        [HttpGet]
        public ActionResult LoadNewNatis()
        {
            MasterViewModel view = new MasterViewModel();
            view.natis = new NatisDataViewModel();
            return View(view);
        }

        [HttpPost]
        public ActionResult LoadNewNatis(FormCollection form)
        {
            MasterViewModel appForm = new MasterViewModel();
            appForm.natis = new NatisDataViewModel();
            TryUpdateModel<MasterViewModel>(appForm, form);

            if (ModelState.IsValid)
            {
                var a = Request.Files.Count;
                appForm.natis.file = Request.Files[0];

                //NatisDataPopulate n = new NatisDataPopulate();
                bool isPopulated = new LoadNew().Scan(appForm, Session["Name"].ToString(), Session["Surname"].ToString(), Session["Department"].ToString());

                if (isPopulated)
                {
                    //appForm.natis = new NatisDataViewModel();
                    return View(appForm);
                }

                else
                {
                    MasterViewModel p = null;
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
                new Respond().respond(form, @Session["Name"].ToString(), Session["Surname"].ToString());
            }

            Intern_LeaveDBEntities database = new Intern_LeaveDBEntities();
            return View(from requests in database.NatisRequests select requests);

        }
        [HttpGet]
        public ActionResult Scan_Back_To_Safe()
        {
            MasterViewModel view = new MasterViewModel();
            view.logInfo = new LogInfoViewModel();
            return View(view);
        }

        [HttpPost]
        public ActionResult Scan_Back_To_Safe(FormCollection form)
        {
            var appForm = new MasterViewModel();
            appForm.logInfo = new LogInfoViewModel();
            TryUpdateModel<MasterViewModel>(appForm, form);

            if (ModelState.IsValid)
            {
                var a = Request.Files.Count;
                appForm.logInfo.file = Request.Files[0];

                //ScanNatis s = new ScanNatis();
                new BackToSafe().Scan(appForm, @Session["Name"].ToString(), @Session["Surname"].ToString(), Session["Department"].ToString());
            }

            return View(appForm);
        }

        public ActionResult Scan_To_Collect()
        {
            MasterViewModel view = new MasterViewModel();
            view.logInfo = new LogInfoViewModel();
            return View(view);
        }

        [HttpPost]
        public ActionResult Scan_To_Collect(FormCollection form)
        {
            var appForm = new MasterViewModel();
            appForm.logInfo = new LogInfoViewModel();
            TryUpdateModel<MasterViewModel>(appForm, form);

            if (ModelState.IsValid)
            {
                var a = Request.Files.Count;
                appForm.logInfo.file = Request.Files[0];

                //ScanNatis c = new ScanNatis();
                new Collect().Scan(appForm, Session["Name"].ToString(), Session["Surname"].ToString(), Session["Department"].ToString());
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
                new Request().request(appForm, Session["Name"].ToString(), Session["Surname"].ToString(), Session["Department"].ToString());
                ViewBag.Stored = "Yes";
            }
            else
            {
                ViewBag.Stored = "No";
            }

            return View(appForm);
        }

        public ActionResult GenerateReports()
        {
            var log = new Intern_LeaveDBEntities().LogInfoes.ToList();

            //this represents the entire Microsoft Office Excel application
            Excel.Application xlApp;

            //create an Excel workbook
            Excel._Workbook xlwookbook;

            //create an Excel worksheet
            Excel._Worksheet xlSheet;

            //represents a range, a cell, a row , a selection of cells etc
            Excel.Range xlrange;

            try
            {
                //create a new Excel window
                xlApp = new Excel.Application();
                xlApp.Visible = true;

                //create a new Workbook
                xlwookbook = xlApp.Workbooks.Add(Missing.Value);

                //create a new Worksheet
                xlSheet = (Excel._Worksheet)xlwookbook.ActiveSheet;

                //create table headers
                xlSheet.Cells[1, 1] = "Contract Number";
                xlSheet.Cells[1, 2] = "VIN NUmber";
                xlSheet.Cells[1, 3] = "Date Scanned";
                xlSheet.Cells[1, 4] = "Scanned By";
                xlSheet.Cells[1, 5] = "Location";
                xlSheet.Cells[1, 6] = "Contract Status";
                xlSheet.Cells[1, 7] = "Comment";

                //format A1 : D1
                xlSheet.get_Range("A1", "G1").Font.Bold = true;

                //set the vertical alignment
                xlSheet.get_Range("A1", "G1").VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;

                for (int i = 2; i < log.Count+2; i++)
                {
                    xlSheet.Cells[i, 1] = log[i-2].ContractNumber;
                    xlSheet.Cells[i, 2] = log[i-2].VinNumber;
                    xlSheet.Cells[i, 3] = log[i-2].DateScanned;
                    xlSheet.Cells[i, 4] = log[i-2].User;
                    xlSheet.Cells[i, 5] = log[i-2].Department;
                    xlSheet.Cells[i, 6] = log[i-2].ContractStatus;
                    xlSheet.Cells[i, 7] = log[i-2].Comment;
                }

                //AutoFit columns A:D
                xlrange = xlSheet.get_Range("A1", "G1");
                xlrange.EntireColumn.AutoFit();

                xlApp.Visible = true;

                //UserControl = true: if the application is visible or if it
                //was created or started by the user
                xlApp.UserControl = true;

                xlwookbook.SaveAs("Audit Report.xls");
            }
            catch (Exception ex)
            {

                throw;
            }

            return RedirectToAction("AdminView", "Users");
        }
    }
}