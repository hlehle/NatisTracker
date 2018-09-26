using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Oracle.DataAccess.Client;
using Oracle.DataAccess.Types;
using System.Web.Mvc;
using Aspose.BarCode.BarCodeRecognition;
using System.Data;
using System.Reflection;
using System.Text;
using EnatisRepository.Repo;
using NatisTracker.ViewModels;
using NatisTracker.Requests;
using NatisTracker.Deliveries;
using EnatisRepository.ScanNatis;
using NatisTracker.UIServices;
using NatisTracker.ScanNatis;

namespace NatisTracker.Controllers
{
    public class ProcessController : Controller
    {
        // GET: Process
        [HttpGet]
        public ActionResult LoadNewNatis()
        {
            var viewModel = new NatisDataViewModel();
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult LoadNewNatis(FormCollection form)
        {
            var viewModel = new NatisDataViewModel();
            TryUpdateModel(viewModel, form);

            if (ModelState.IsValid)
            {
                //var a = Request.Files.Count;
                viewModel.file = Request.Files[0];

                bool isPopulated = new LoadNew().Scan(viewModel, Session["Name"].ToString(), Session["Department"].ToString());

                if (isPopulated)
                { 

                    //viewModel.natis = new NatisDataViewModel();
                    return View(viewModel);
                }

                else
                {
                    NatisDataViewModel p = null;
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
            var requests = new PopulateViewModels().PopulateRequestsData(db);
            return View(requests);
        }

        [HttpPost]
        public ActionResult RequestResponse(FormCollection form)
        {
            

            if (ModelState.IsValid)
            {
                new Respond().respond(form, @Session["Name"].ToString(), Session["Email"].ToString());
            }

            Intern_LeaveDBEntities db = new Intern_LeaveDBEntities();
            var requests = new PopulateViewModels().PopulateRequestsData(db);
            return View(requests);

        }
        [HttpGet]
        public ActionResult Scan_Back_To_Safe()
        {
            //MasterViewModel view = new MasterViewModel();
            var viewModel = new NatisDataViewModel();
            return View(viewModel);
        }
        [HttpPost]
        public ActionResult Scan_Back_To_Safe(FormCollection form)
        {
            var viewModel = new NatisDataViewModel();
            TryUpdateModel(viewModel, form);

            if (ModelState.IsValid)
            {
                viewModel.ScannedString = viewModel.ScannedString.Remove(viewModel.ScannedString.Length - 1).Remove(0, 1);
                string[] arr = viewModel.ScannedString.Split('%');

                var load = new LoadNew();
                viewModel.contractNo = load.getContractNo(arr[9]);
                viewModel.vin = arr[9];
                var vin = viewModel.vin;
                viewModel.registrationNo = arr[5];
                viewModel.engineNo = arr[10];
                viewModel.carMake = arr[7];
                viewModel.seriesNo = load.getDescription(viewModel.contractNo);
                viewModel.description = arr[6];
                viewModel.registrationDate = Convert.ToDateTime(arr[12]);
                viewModel.VehicleStatus = arr[11];
                viewModel.OwnerName = arr[15];
                viewModel.OwnerID = arr[14];
                viewModel.natisLocation = "Safe Vault";

                using (var db = new Intern_LeaveDBEntities())
                {
                    var natis = db.NatisDatas.Where(a => a.VinNumber == vin).FirstOrDefault();
                    natis.NatisLocation = viewModel.natisLocation;
                    db.SaveChanges();
                }
            }

            return View(viewModel);
        }
        public ActionResult Scan_To_Collect()
        {
            var view = new NatisDataViewModel();
            return View(view);
        }

        [HttpPost]
        public ActionResult Scan_To_Collect(FormCollection form)
        {
            var viewModel = new NatisDataViewModel();
            TryUpdateModel(viewModel, form);

            if (ModelState.IsValid)
            {
                viewModel.ScannedString = viewModel.ScannedString.Remove(viewModel.ScannedString.Length - 1).Remove(0, 1);
                string[] arr = viewModel.ScannedString.Split('%');

                var load = new LoadNew();
                viewModel.contractNo = load.getContractNo(arr[9]);
                viewModel.vin = arr[9];
                var vin = viewModel.vin;
                viewModel.registrationNo = arr[5];
                viewModel.engineNo = arr[10];
                viewModel.carMake = arr[7];
                viewModel.seriesNo = load.getDescription(viewModel.contractNo);
                viewModel.description = arr[6];
                viewModel.registrationDate = Convert.ToDateTime(arr[12]);
                viewModel.VehicleStatus = arr[11];
                viewModel.OwnerName = arr[15];
                viewModel.OwnerID = arr[14];
                viewModel.natisLocation = Session["Department"].ToString();

                using (var db = new Intern_LeaveDBEntities())
                {
                    var natis = db.NatisDatas.Where(a => a.VinNumber == vin).FirstOrDefault();
                    natis.NatisLocation = viewModel.natisLocation;
                    db.SaveChanges();
                }

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
            TryUpdateModel(viewModel);

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

        public ActionResult TickToCollect()
        {
            using (Intern_LeaveDBEntities db = new Intern_LeaveDBEntities())
            {
                var viewModel = new PopulateViewModels().PopulateTickBoxData(db);
                var internalUser = Session["Name"].ToString();
                viewModel.TickBoxList = viewModel.TickBoxList.Where(a => a.RecipientType.Equals("Internal User") && a.RecipientName.Equals(internalUser)).ToList();
                return View(viewModel);
            }
        }

        [HttpPost]
        public ActionResult TickToCollect(FormCollection f)
        {
            var viewModel = new TickBoxViewModelList();
            using (Intern_LeaveDBEntities db = new Intern_LeaveDBEntities())
            {
                TryUpdateModel(viewModel);
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                if (ModelState.IsValid && viewModel.IsConfirmed)
                {
                    new DeliveryServiceOUT().SendNatis(viewModel, Session["Name"].ToString(), Session["Department"].ToString());
                    ModelState.Clear();
                }

                viewModel = new PopulateViewModels().PopulateTickBoxData(db);
                var internalUser = Session["Name"].ToString();
                viewModel.TickBoxList = viewModel.TickBoxList.Where(a => a.RecipientType.Equals("Internal User") && a.RecipientName.Equals(internalUser)).ToList();
               
                return View(viewModel);
            }
        }

        public ActionResult GenerateOneReports(FormCollection form)
        {
            var contractNumber = form["hidden"];
            var log = new Intern_LeaveDBEntities().ScanLogsDatas.Where(a => a.ContractNumber == contractNumber).ToList();
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
                var department = form["HiddenDepartment"];
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