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
using System.Text;

namespace NatisTracker.Controllers
{
    public class ProcessController : Controller
    {
        // GET: Process
        [HttpGet]
        public ActionResult LoadNewNatis()
        {
            var viewModel = new NatisAndContractViewModel();
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult LoadNewNatis(FormCollection form)
        {
            var viewModel = new NatisAndContractViewModel();
            TryUpdateModel(viewModel, form);

            if (ModelState.IsValid)
            {
                var a = Request.Files.Count;
                viewModel.file = Request.Files[0];

                bool isPopulated = new LoadNew().Scan(viewModel, Session["Name"].ToString(), Session["Department"].ToString());

                if (isPopulated)
                {
                    //viewModel.natis = new NatisDataViewModel();
                    return View(viewModel);
                }

                else
                {
                    NatisAndContractViewModel p = null;
                    return View(p);
                }
            }

            return View(viewModel);
        }

        public ActionResult GenerateReport()
        {
            return View();
        }

        [HttpGet]
        public ActionResult RequestResponse()
        {
            Intern_LeaveDBEntities db = new Intern_LeaveDBEntities();
            return View(from requests in db.RequestsDatas select requests);
        }

        [HttpPost]
        public ActionResult RequestResponse(FormCollection form)
        {
            if (ModelState.IsValid)
            {
                new Respond().respond(form, @Session["Name"].ToString(), Session["Email"].ToString());
            }

            Intern_LeaveDBEntities database = new Intern_LeaveDBEntities();
            return View(from requests in database.RequestsDatas select requests);

        }
        [HttpGet]
        public ActionResult Scan_Back_To_Safe()
        {
            MasterViewModel view = new MasterViewModel();
            view.logInfo = new LogInfoViewModel();
            return View(view);
        }

        public ActionResult Scan_To_Collect()
        {
            var view = new NatisAndContractViewModel();
            return View(view);
        }

        [HttpPost]
        public ActionResult Scan_To_Collect(FormCollection form)
        {
            var viewModel = new NatisAndContractViewModel();
            TryUpdateModel(viewModel, form);

            if (ModelState.IsValid)
            {
                var a = Request.Files.Count;
                viewModel.file = Request.Files[0];

                new Collect().Scan(viewModel, Session["Name"].ToString(), Session["Department"].ToString());
            }

            return View(viewModel);
        }

        public ActionResult RequestNatis()
        {
            //ViewBag.Stored = "No";
            return View(new NatisRequests());
        }

        [HttpPost]
        public ActionResult RequestNatis(FormCollection form)
        {
            var viewModel = new NatisRequests();
            TryUpdateModel<NatisRequests>(viewModel, form);

            if (ModelState.IsValid)
            {
                viewModel = new Request().request(viewModel, Session["Name"].ToString(), Session["Department"].ToString(), Session["Email"].ToString());
                ViewBag.Stored = "Yes";
                ModelState.Clear();
            }
            else
            {
                ViewBag.Stored = "No";
            }

            return View(viewModel);

        }

        public ActionResult GenerateOneReports(FormCollection form)
        {
            var contractNumber = form["a"];
            var log = new Intern_LeaveDBEntities().ScanLogsDatas.Where(a => a.ContractNumber == contractNumber).ToList();
            var path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)+@"\"+contractNumber+".csv";
            StringBuilder line = new StringBuilder();
            FileContentResult file = null;

            //using (FileStream streamWriter = new FileStream(path, FileMode.Create))
            //{
            var header = string.Format("{0},{1},{2},{3},{4},{5},{6}",
                                           "Contract Number",
                                           "VIN",
                                           "Date Scanned",
                                           "Scanned By",
                                           "Natis Location",
                                           "Contract Status",
                                           "Comment"
                                          );
                line.AppendLine(header);

                for (int i = 0; i < log.Count; i++)
                {
                    var listResults = string.Format("{0},{1},{2},{3},{4},{5},{6}",
                                                      log[i].ContractNumber,
                                                      log[i].VinNumber,
                                                      log[i].DateScanned,
                                                      log[i].User,
                                                      log[i].Department,
                                                      log[i].ContractStatus,
                                                      log[i].Comment
                                                     );
                    line.AppendLine(listResults);


                //}
                byte[] data = Encoding.Default.GetBytes(line.ToString());
                file = File(data, "application/csv", contractNumber+" Report.csv");
                //streamWriter.Write(data,0,data.Length);                
            }

            //return RedirectToAction("AdminView", "Users");
            return file;
        }

        public ActionResult GenerateAuditReports()
        {
            var log = new Intern_LeaveDBEntities().ScanLogsDatas.ToList();
            var path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\AuditReport.csv";
            StringBuilder line = new StringBuilder();
            FileContentResult file = null;


            //using (FileStream streamWriter = new FileStream(path, FileMode.Create))
            //{
                var header = string.Format("{0},{1},{2},{3},{4},{5},{6}",
                                           "Contract Number",
                                           "VIN",
                                           "Date Scanned",
                                           "Scanned By",
                                           "Natis Location",
                                           "Contract Status",
                                           "Comment"
                                          );
                line.AppendLine(header);

                for (int i = 0; i < log.Count; i++)
                {
                    var listResults = string.Format("{0},{1},{2},{3},{4},{5},{6}",
                                                      log[i].ContractNumber,
                                                      log[i].VinNumber,
                                                      log[i].DateScanned,
                                                      log[i].User,
                                                      log[i].Department,
                                                      log[i].ContractStatus,
                                                      log[i].Comment
                                                     );
                    line.AppendLine(listResults);


                //}
                byte[] data = Encoding.Default.GetBytes(line.ToString());
                
                file = File(data, "application/csv", "Audit Report.csv");

                //streamWriter.Write(data, 0, data.Length);
            }

            return file;
        }

        public ActionResult GenerateDepartmentReports(FormCollection form)
        {
            using (Intern_LeaveDBEntities db = new Intern_LeaveDBEntities())
            {
                var department = form["b"];
                var natis = db.NatisDatas.Where(a => a.NatisLocation == department).ToList();
                var log = new List<ScanLogsData>();

                foreach (var item in natis)
                {
                    var vin = item.VinNumber;
                    var temp = db.ScanLogsDatas.Where(a => a.VinNumber == vin).OrderBy(a => a.DateScanned).FirstOrDefault();
                    log.Add(temp);
                }

                StringBuilder line = new StringBuilder();
                FileContentResult file = null;


                //using (FileStream streamWriter = new FileStream(path, FileMode.Create))
                //{
                var header = string.Format("{0},{1},{2},{3},{4},{5},{6}",
                                           "Contract Number",
                                           "VIN",
                                           "Date Scanned",
                                           "Scanned By",
                                           "Natis Location",
                                           "Contract Status",
                                           "Comment"
                                          );
                line.AppendLine(header);

                for (int i = 0; i < log.Count; i++)
                {
                    var listResults = string.Format("{0},{1},{2},{3},{4},{5},{6}",
                                                      log[i].ContractNumber,
                                                      log[i].VinNumber,
                                                      log[i].DateScanned,
                                                      log[i].User,
                                                      log[i].Department,
                                                      log[i].ContractStatus,
                                                      log[i].Comment
                                                     );
                    line.AppendLine(listResults);


                    //}
                    byte[] data = Encoding.Default.GetBytes(line.ToString());
                    file = File(data, "application/csv", department + " Report.csv");
                    //streamWriter.Write(data, 0, data.Length);
                }

                return file;
            }
            
        }
    }
}