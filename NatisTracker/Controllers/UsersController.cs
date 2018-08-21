using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NatisTracker.ViewModels;
using NatisTracker.Deliveries;
using NatisTracker.Requests;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using Oracle.DataAccess.Client;
using NatisTracker.UIServices;
using EnatisRepository.Repo;
using System.Data;

namespace NatisTracker.Controllers
{
    public class UsersController : Controller
    {
        // GET: Users                
        public ActionResult AdminView()
        {
            using (Intern_LeaveDBEntities db = new Intern_LeaveDBEntities())
            {
                var viewModel = new AdminViewModel();

                viewModel.RequestsList = new PopulateViewModels().PopulateRequestsData(db);
                viewModel.ScanLogsList = new PopulateViewModels().PopulateScanLogs(db);
                viewModel.NatisDataList = new PopulateViewModels().PopulateNatisData(db);
                viewModel.DriverViewModel = new DriverDocsViewModel();
                viewModel.DriverViewModel.QuantityList = GetQuantity();
                viewModel.SendToUserView = new SendToUserViewModel();
                viewModel.SendToUserView.QuantityList = GetQuantity();
                viewModel.SendToUserView.PeopleList = viewModel.RequestsList.Where(a => a.RequestStatus == "Accepted").Select(a => a.RequesterName).Distinct().ToList();
                viewModel.SentIN_Delivery = new PopulateViewModels().PopulateSentIN_Deliveries(db);
                viewModel.Maturities = new PopulateViewModels().PopulateNatisData(db).Where(a => a.natisLocation.Equals("Contracts Maturities")).ToList();
                viewModel.Call_Centre = new PopulateViewModels().PopulateNatisData(db).Where(a => a.natisLocation.Contains("Call Centre")).ToList();
                viewModel.Legal = new PopulateViewModels().PopulateNatisData(db).Where(a => a.natisLocation.Equals("Legal")).ToList();
                viewModel.FleetServiceDriver = new PopulateViewModels().PopulateNatisData(db).Where(a => a.natisLocation.Contains("Fleet Service")).ToList();
                viewModel.Operations = new PopulateViewModels().PopulateNatisData(db).Where(a => a.natisLocation.Equals("Operations")).ToList();
                viewModel.Originations = new PopulateViewModels().PopulateNatisData(db).Where(a => a.natisLocation.Equals("Contracts Contracts Origination")).ToList();
                viewModel.Remarketing = new PopulateViewModels().PopulateNatisData(db).Where(a => a.natisLocation.Equals("Remarketing")).ToList();

                return View(viewModel);
            }
        }

        [HttpPost]
        public ActionResult AdminView(FormCollection form)
        {
            var viewModel = new AdminViewModel();

            TryUpdateModel(viewModel);
            Intern_LeaveDBEntities db = new Intern_LeaveDBEntities();

            if (ModelState.IsValid)
            {
                if (viewModel.DriverViewModel != null)
                {
                    new PopulateTickBoxData().PopulateSendToDriver(viewModel.DriverViewModel, Session["Name"].ToString(), Session["Department"].ToString());
                }

                else if (viewModel.SendToUserView != null)
                {
                    new PopulateTickBoxData().PopulateSendToUser(viewModel.SendToUserView, Session["Name"].ToString(), Session["Department"].ToString());
                }

                viewModel.RequestsList = new PopulateViewModels().PopulateRequestsData(db);
                viewModel.ScanLogsList = new PopulateViewModels().PopulateScanLogs(db);
                viewModel.NatisDataList = new PopulateViewModels().PopulateNatisData(db);
                viewModel.SentIN_Delivery = new PopulateViewModels().PopulateSentIN_Deliveries(db);
                viewModel.DriverViewModel = new DriverDocsViewModel();
                viewModel.DriverViewModel.QuantityList = GetQuantity();
                viewModel.SendToUserView = new SendToUserViewModel();
                viewModel.SendToUserView.QuantityList = GetQuantity();
                viewModel.SendToUserView.PeopleList = viewModel.RequestsList.Where(a => a.RequestStatus == "Accepted").Select(a => a.RequesterName).Distinct().ToList();
                viewModel.Maturities = new PopulateViewModels().PopulateNatisData(db).Where(a => a.natisLocation.Equals("Contracts Maturities")).ToList();
                viewModel.Call_Centre = new PopulateViewModels().PopulateNatisData(db).Where(a => a.natisLocation.Contains("Call Centre")).ToList();
                viewModel.Legal = new PopulateViewModels().PopulateNatisData(db).Where(a => a.natisLocation.Equals("Legal")).ToList();
                viewModel.FleetServiceDriver = new PopulateViewModels().PopulateNatisData(db).Where(a => a.natisLocation.Contains("Fleet Service")).ToList();
                viewModel.Operations = new PopulateViewModels().PopulateNatisData(db).Where(a => a.natisLocation.Equals("Operations")).ToList();
                viewModel.Originations = new PopulateViewModels().PopulateNatisData(db).Where(a => a.natisLocation.Equals("Contracts Contracts Origination")).ToList();
                viewModel.Remarketing = new PopulateViewModels().PopulateNatisData(db).Where(a => a.natisLocation.Equals("Remarketing")).ToList();
            }

            ModelState.Clear();
            return View(viewModel);
        }

        public ActionResult UserView()
        {
            Intern_LeaveDBEntities db = new Intern_LeaveDBEntities();

            var viewModel = new EndUserViewModel();
            string name = Session["Name"].ToString();

            viewModel.RequestsList = new PopulateViewModels().PopulateRequestsData(db).Where(a => a.RequesterName.Equals(name)).ToList();
            viewModel.NatisDataList = new PopulateViewModels().PopulateNatisData(db);
            viewModel.viewModel = new DeliveryitemViewModel();
            viewModel.viewModel.QuantityList = GetQuantity();

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult UserView(FormCollection form)
        {
            var viewModel = new EndUserViewModel();
            TryUpdateModel(viewModel, form);
            Intern_LeaveDBEntities db = new Intern_LeaveDBEntities();

            //if (ModelState.IsValid)
            //{

            if (viewModel.viewModel != null)
            {
                viewModel.viewModel.QuantityList = GetQuantity();
                new DeliveryServiceOUT().sendDelivery(viewModel.viewModel, Session["Name"].ToString());
            }

            //if (viewMaster._DriverPackage != null)
            //{
            //    new DeliveryServiceOUT().ReceivePackage(viewMaster._DriverPackage, Session["Name"].ToString(), Session["Department"].ToString());
            //    viewMaster._DriverPackage = PopulateDriverPackage1(db);
            //}

            string name = Session["Name"].ToString();

            viewModel.RequestsList = new PopulateViewModels().PopulateRequestsData(db).Where(a => a.RequesterName.Equals(name)).ToList();
            viewModel.viewModel = new DeliveryitemViewModel();
            viewModel.viewModel.QuantityList = GetQuantity();
            //}
            return View(viewModel);
        }

        public ActionResult DealershipView(string DealershipName, string FName, string LName, string EmailAddress)
        {
            Session["Name"] = FName;
            Session["Surname"] = LName;
            using (Intern_LeaveDBEntities db = new Intern_LeaveDBEntities())
            {
                DeliveryitemViewModel viewModel = new DeliveryitemViewModel();
                viewModel.CourierDeliveryDisplay = db.SentIN_Delivery.Where(a => a.DeliveryChoice.Equals("Courier")).ToList();
                viewModel.DriverDeliveryDisplay = db.SentIN_Delivery.Where(a => a.DeliveryChoice.Equals("Driver")).ToList();
                viewModel.ContractsDisplay = db.ContractNumbers.ToList();
                viewModel.DealerList = getList();
                viewModel.QuantityList = GetQuantity();
                viewModel.DealershipName = DealershipName;
                viewModel.PackageSender = FName + " " + LName;
                viewModel.SenderEmail = EmailAddress;

                return View(viewModel);
            }

        }

        [HttpPost]
        public ActionResult DealershipView(DeliveryitemViewModel viewModel)
        {
            using (Intern_LeaveDBEntities db = new Intern_LeaveDBEntities())
            {
                //viewModel.deliveryDisplay = db.SentIN_Delivery.ToList();
                var tryUpdate = TryUpdateModel(viewModel);
                //var modelState = ModelState.Values.ToList();

                if (ModelState.IsValid)
                {
                    new DeliveryServiceIN().sendDelivery(viewModel, Session["Name"].ToString());

                    if (viewModel.WaybillNumber != null || viewModel.DriverDetails != null)
                    {
                        ModelState.Clear();
                        viewModel = new DeliveryitemViewModel();
                        viewModel.CourierDeliveryDisplay = db.SentIN_Delivery.Where(a => a.DeliveryChoice.Equals("Courier")).ToList();
                        viewModel.DriverDeliveryDisplay = db.SentIN_Delivery.Where(a => a.DeliveryChoice.Equals("Driver")).ToList();
                        viewModel.ContractsDisplay = db.ContractNumbers.ToList();
                    }
                }
            }

            viewModel.DealerList = getList();
            viewModel.QuantityList = GetQuantity();
            return View(viewModel);

        }
        [HttpGet]
        public ActionResult OriginationView(string Firstname, string Surname, string EmailAddress, string Mobile)
        {
            Session["Name"] = Firstname;
            Session["Surname"] = Surname;
            using (Intern_LeaveDBEntities db = new Intern_LeaveDBEntities())
            {
                DeliveryViewModel viewModel = new DeliveryViewModel();
                viewModel = PopulateDeliveryViewModel(viewModel, db);
                return View(viewModel);
            }
        }

        [HttpPost]
        public ActionResult OriginationView(FormCollection form)
        {
            DeliveryViewModel viewModel = new DeliveryViewModel();
            using (Intern_LeaveDBEntities db = new Intern_LeaveDBEntities())
            {
                TryUpdateModel(viewModel);

                //if (ModelState.IsValid)
                //{
                //    new DeliveryServiceIN().receiveDelivery(viewModel, Session["Name"].ToString(), Session["Surname"].ToString());
                //    viewModel = PopulateDeliveryViewModel(viewModel, db);

                //    return View(viewModel);
                //}

                new DeliveryServiceIN().receiveDelivery(viewModel, Session["Name"].ToString(), Session["Email"].ToString());
                viewModel = PopulateDeliveryViewModel(viewModel, db);
                return View(viewModel);
            }

        }

        public ActionResult DriverView()
        {
            Intern_LeaveDBEntities db = new Intern_LeaveDBEntities();

            var driverPackage = new PopulateViewModels().PopulateTickBoxData(db);
            driverPackage.TickBoxList = driverPackage.TickBoxList.Where(a => a.RecipientType.Equals("Driver")).ToList();
            return View(driverPackage);
        }

        [HttpPost]
        public ActionResult DriverView(FormCollection f)
        {
            var viewModel = new TickBoxViewModelList();
            using (Intern_LeaveDBEntities db = new Intern_LeaveDBEntities())
            {
                TryUpdateModel(viewModel);

                //if (ModelState.IsValid)
                //{

                new DeliveryServiceOUT().SendNatis(viewModel, Session["Name"].ToString(), Session["Department"].ToString());
                viewModel = new PopulateViewModels().PopulateTickBoxData(db);
                viewModel.TickBoxList = viewModel.TickBoxList.Where(a => a.RecipientType.Equals("Driver")).ToList();
                ModelState.Clear();
                return View(viewModel);

                //}


            }
        }
        public List<string> getList()
        {
            List<string> list = new List<string>();
            list.Add("000000000 - MBFS user pre-approval dealership");
            list.Add("270300004 - Imperial Group Pty Ltd t/ a Mercedes - Benz M2 City");
            list.Add("270300008 - Imperial Group(Pty) Ltd T/ A Mercedes - Benz Bedfordview");
            list.Add("270300016 - ECU Investments(Pty) Ltd T/ A CBF Motors");
            list.Add("270300018 - Century Motors(Pty) Ltd");
            list.Add("270300022 - De Wit Motors, Ermelo");
            list.Add("270300023 - De Wit Motors CV, Ermelo");
            list.Add("270300025 - Sandown Motor Holdings (Pty)Ltd T / A Eikestad Motors");
            list.Add("270300030 - Trans - Vehicles CC T / A Eurocar Motors");
            list.Add("270300038 - Super Group Trading(Pty) Ltd T / A Grand Central Motors");
            list.Add("270300041 - Inyanga Motors(Pty) Ltd t / a McCarthy Inyanga Empangeni");
            list.Add("270300043 - Barloworld Motors(Pty) Ltd t / a John Williams Welkom");
            list.Add("270300044 - Barloworld Motors(Pty) Ltd t / a John Williams Bloemfontein");
            list.Add("270300049 - Metje & Ziegler Ltd T / A M & Z Motors");
            list.Add("270300054 - Maritime Motors(Pty) Ltd(Port Elizabeth");
            list.Add("270300061 - McCarthy Limited t / a Mercedes - Benz Centurion");
            list.Add("270300067 - McCarthy Limited T / A McCarthy Pretoria");
            list.Add("270300070 - McCarthy Limited T / A McCarthy Witbank");
            list.Add("270300076 - Imperial Group(Pty) Ltd T / A Mercedes - Benz East Rand Mall");
            list.Add("270300078 - Imperial Group(Pty) Ltd T / A Mercurius Motors Polokwane");
            list.Add("270300091 - Naledi Motors(Pty) Ltd T / A Naledi Motors Francistown CJD");
            list.Add("270300092 - Naledi Motors Pty Ltd t / a Naledi Motors Francistown CJD");
            list.Add("270300094 - Namaqua Garage(Pty) Ltd");
            list.Add("270300101 - Hallmark Motor Group(Pty) Ltd T / A New Vaal Motors");
            list.Add("270300106 - Sandown Motor Holdings t / a Mercedes Benz Century City");
            list.Add("270300111 - Stucky Motors(Pty) Ltd");
            list.Add("270300114 - M H Cloete Enterprises(Pty) Ltd T / A Rola Motors");
            list.Add("270300115 - Ronnies Motors Trust");
            list.Add("270300120 - Sandown Motor Holdings(Pty) Ltd T / A Mercedes - Benz Sandton");
            list.Add("270300122 - ANS Motors(Pty) Ltd T / A Shiraz Auto");
            list.Add("270300124 - Stanmar Motors(Pty) Ltd");
            list.Add("270300126 - Star Motors");
            list.Add("270300132 - JDG Trading(Pty) Ltd t / a United Motors(CV)");
            list.Add("270300135 - Zelco Motors CC");
            list.Add("270300139 - M and Z Freightliner");
            list.Add("270300144 - MBFSA HO Dealer");
            list.Add("270300190 - Mercedes Benz South Africa Ltd(Staff)");
            list.Add("270300208 - Sandown Motor Holdings(Pty) Ltd T / A Orbit Boland");
            list.Add("270300242 - Sandown Motor Holdings(Pty) Ltd T / A Mitsubishi Bryanston");
            list.Add("270300272 - Sandown Motor Holdings(Pty) Ltd T / A Sandown CV Belville");
            list.Add("270300275 - Sandown Motor Holdings(Pty) Ltd T / A Mercedes Benz Culemborg");
            list.Add("270300277 - Sandown Motor Holdings Pty t / a Mercedes - Benz CV Centurion");
            list.Add("270300278 - Sandown Motor Holdings(Pty) Ltd T / A Mercedes - Benz, CV CT");
            list.Add("270300279 - Sandown Motor Holdings(Pty) Ltd T / a Sandown CV Centurion");
            list.Add("270300280 - Sandown Motor Holdings(Pty) Ltd T / A Mercedes - Benz, Claremont");
            list.Add("270300283 - McCarthy Ltd t / a Mercedes - Benz Menlyn");
            list.Add("270300294 - Sandown Motor Holdings P / L t / a Mits Motors Paarden Eiland");
            list.Add("270300295 - Imperial Group(Pty) Ltd t / a Mercedes - Benz Springs");
            list.Add("270300296 - Sirius Motor Corporation Pty Ltd t / a Union Motors Lowveld CV");
            list.Add("270300298 - Mercedes - Benz Commercial Vehicles Durban");
            list.Add("270300300 - NMI - Durban South Motors(Pty) Ltd T / A NMI - DSM CV Pinetown");
            list.Add("270300301 - NMI - Durban South Motors(Pty) Ltd T / A NMI Umhlanga");
            list.Add("270300302 - NMI DSM(Pty) Ltd T / A Mercedes - Benz Durban");
            list.Add("270300304 - NMI - Durban South Motors(Pty) Ltd");
            list.Add("270300305 - Imperial Group(Pty) Ltd t / a Cargo CV Germiston");
            list.Add("270300307 - Gariep Motors(Pty) Ltd");
            list.Add("270300310 - Contract Changes Restructures");
            list.Add("270300312 - Sandown Motor Holding(Pty) Ltd T / A Mercedes - Benz Northcliff");
            list.Add("270300313 - Sandown Motor Holdings(Pty) Ltd T / A Mercedes - Benz Rosebank");
            list.Add("270300315 - NMI DSM(Pty) Ltd T / A Garden City Motors");
            list.Add("270300316 - NMI DSM(Pty) Ltd T / A Garden City Commercials");
            list.Add("270300325 - Inyanga Motors(Pty) Ltd T / A McCarthy Inyanga Vryheid");
            list.Add("270300326 - Imperial Motors T / A Mitsubishi Motors East Rand");
            list.Add("270300327 - Maemo Motors(Pty) Ltd");
            list.Add("270300331 - NMI DSM(Pty) Ltd T / A Mercedes - Benz CV, Durban");
            list.Add("270300332 - NMI Durban South Motors(Pty) Ltd T / A Mitsubishi Umhlanga");
            list.Add("270300333 - NMI DSM Pty Ltd t / a Mercedes - Benz Umhlanga");
            list.Add("270300334 - NMI Durban South Motors(Pty) Ltd t / a CJD Umhlanga");
            list.Add("270300335 - Sandown Motor Holdings T / A Mercedes - Benz Constantia Kloof");
            list.Add("270300342 - Sandown Motor Holdings(Pty) Ltd t / a CJD Fourways");
            list.Add("270300344 - Maturities");
            list.Add("270300347 - Imperial Group(Pty) Ltd t / a Mercurius Motors Tzaneen");
            list.Add("270300350 - Imperial Group(Pty) Ltd t / a Cargo Motors Klerksdorp");
            list.Add("270300358 - Sandown Motor Holdings t / a Chrysler Jeep Dodge Century City");
            list.Add("270300363 - Barloworld Motor Pty Ltd t / a John Williams CV Bloemfontein");
            list.Add("270300366 - Restructure Delinquent Account");
            list.Add("270300369 - Imperial Group(Pty) Ltd t / a Mercedes Benz CV East Rand");
            list.Add("270300378 - McCarthy Limited t / a Mitsubishi Motors Brooklyn");
            list.Add("270300382 - MH Cloete Enterprises Pty Ltd ta Rola Motors CV, Helderberg");
            list.Add("270300383 - Sandown Motor Holdings t / a Mitsubishi Motors Tygerberg");
            list.Add("270300384 - McCarthy Limited(Pty) Ltd t / a Chrysler Jeep Dodge Menlyn");
            list.Add("270300385 - McCarthy Limited t / a Mitsubishi Motors Midrand");
            list.Add("270300388 - McCarthy Ltd T / A McCarthy Call - a - Car Direct Menlyn");
            list.Add("270300402 - Sandown Motor Holdings(Pty) Ltd t / a Paarl Motors");
            list.Add("270300409 - Mccarthy Ltd t / a Chrysler Jeep Dodge Centurion");
            list.Add("270300413 - Mercedes - Benz South Africa Limited");
            list.Add("270300416 - Chrysler South Africa");
            list.Add("270300419 - Sandown Motor Holdings(Pty) Ltd T / a Sandown CV Roodepoort");
            list.Add("270300420 - MBCV Roodepoort");
            list.Add("270300421 - Barloworld Motors Pty Ltd t / a John Williams CJD Bloemfontein");
            list.Add("270300422 - Barloworld Motors Pty Ltd t / a John Williams PC Bloemfontein");
            list.Add("270300425 - McCarthy Limited Trading as Mercedes - Benz Wonderboom");
            list.Add("270300427 - Sandown Motor Holdings(Pty) Ltd T / a Mercedes - Benz Bryanston");
            list.Add("270300431 - John Williams Bloem CJD");
            list.Add("270300438 - Auto Lux(Pty) Ltd t / a Chrysler Jeep Dodge East Rand Mall");
            list.Add("270300442 - Sandown Motors Holdings t / a Chrysler Jeep Dodge Sandton");
            list.Add("270300443 - Sandown Motors Holdings Pty Ltd t / a CJD Constantia Kloof");
            list.Add("270300445 - JDG Trading(Pty) Ltd t / a United Motors PC");
            list.Add("270300447 - JDG Trading(Pty) Ltd t / a United Motors CJD");
            list.Add("270300448 - Sandown Motor Holdings Pty Ltd t / a Mercedes - Benz Cars(GRO)");
            list.Add("270300449 - Auction Finance(Pty) Ltd Gauteng");
            list.Add("270300450 - Auction Finance(Pty) Ltd Port Elizabeth");
            list.Add("270300451 - Auction Finance(Pty) Ltd Cape Town");
            list.Add("270300452 - Auction Finance(Pty) Ltd Free State");
            list.Add("270300453 - Sandown Gauteng Showroom Units");
            list.Add("270300455 - Restructures Debt Review");
            list.Add("270300456 - Mercedes - Benz Cars - Cape Town Regional Office(CRO)");
            list.Add("270300457 - Alexander Forbes Insurance Company Ltd");
            list.Add("270300458 - Shortfall Payment Agreement");
            list.Add("270300459 - Regent");
            list.Add("270300460 - Auction Finance(Pty) Ltd Durban");
            list.Add("270300461 - Regent Insurance Company Limited");
            list.Add("270300462 - Sandown Motor Holdings(Pty) Ltd t / a Sandown Pre - Owned");
            list.Add("270300463 - McCarthy Limited t / a Mitsubishi Motors Brooklyn");
            list.Add("270300464 - Maemo Motors - Chrysler");
            list.Add("270300465 - Maemo Motors PC");
            list.Add("270300466 - ECU Investments(Pty) Ltd T / A CBF Motors - Mitsubishi Motors");
            list.Add("270300467 - Rola Motors");
            list.Add("270300468 - Sandown Motor Holdings t / a Mitsubishi Motors Tygerberg");
            list.Add("270300469 - De Wit Motors, Ermelo - Mitsubishi");
            list.Add("270300470 - Sandown Motor Holdings P / L t / a Mits Motors Paarden Eiland");
            list.Add("270300471 - Gemini Moon Trading(Pty) Ltd t / a Malmesbury Motors - Mits");
            list.Add("270300472 - Namaqua Garage(Pty) Ltd - Mitsubishi");
            list.Add("270300473 - NMI Durban South Motors(Pty) Ltd T / A Mitsubishi Umhlanga");
            list.Add("270300474 - Imperial Motors T / A Mitsubishi Motors East Rand");
            list.Add("270300475 - McCarthy Limited t / a Mitsubishi Motors Brooklyn");
            list.Add("270300476 - McCarthy Limited t / a Mitsubishi Motors Midrand");
            list.Add("270300477 - Sandown Motor Holdings(Pty) Ltd T / A Mitsubishi Bryanston");
            list.Add("270300478 - ECU Investments(Pty) Ltd T / A CBF Motors - Mitsubishi Motors");
            list.Add("270300479 - Mitsubishi Motors Kimberley");
            list.Add("270300480 - Century Motors(Pty) Ltd - Mitsubishi Motors");
            list.Add("270300481 - Barloworld Motors(Pty) Ltd t / a John Williams Welkom Mits");
            list.Add("270300482 - De Wit Motors, Ermelo - Mitsubishi");
            list.Add("270300483 - Trans - Vehicles CC T / A Eurocar Motors - Mitsubishi");
            list.Add("270300484 - Gemini Moon Trading(Pty) Ltd t / a Malmesbury Motors - Mits");
            list.Add("270300485 - Namaqua Garage(Pty) Ltd - Mitsubishi");
            list.Add("270300486 - Stucky Motors Newcastle(Pty) Ltd T / A Stucky CJD and Mits");
            list.Add("270300487 - Power Projects(Pty) Ltd T / a Swazi Auto Truck Centre Mits");
            list.Add("270300488 - JDG Trading(Pty) Ltd t / a United Motors PC - Mits");
            list.Add("270300489 - Zelco Motors(Pty) Ltd - Mitsubishi");
            list.Add("270300490 - Imperial Group(Pty) Ltd t / a Cargo Motors Klerksdorp - Mits");
            list.Add("270300491 - Sandown Motor Holdings(Pty) Ltd T / A Eikestad Motors Mits");
            list.Add("270300492 - NMI DSM(Pty) Ltd T / A Garden City Motors - Mitsubishi");
            list.Add("270300493 - Mitsubishi Motors Kimberley");
            list.Add("270300494 - Barloworld Motors Pty Ltd t / a John Williams PC Bftn Mits");
            list.Add("270300495 - Barloworld Motors(Pty) Ltd t / a John Williams Welkom Mits");
            list.Add("270300496 - Metje & Ziegler Ltd T / A M & Z Motors - Mitsubishi");
            list.Add("270300497 - Maemo Motors PC - Mitsubishi");
            list.Add("270300498 - Maritime Motors(Pty) Ltd(Port Elizabeth) - Mitsubishi");
            list.Add("270300499 - Inyanga Motors(Pty) Ltd t / a McCarthy Inyanga Empangeni Mits");
            list.Add("270300500 - McCarthy Limited T / A McCarthy Witbank - Mitsubishi");
            list.Add("270300501 - Inyanga Motors(Pty) Ltd T / A McCarthy Inyanga Vryheid Mits");
            list.Add("270300502 - Imperial Group(Pty) Ltd T / A Mercurius Motors Polokwane Mits");
            list.Add("270300503 - Naledi Motors(Pty) Ltd - Mitsubishi");
            list.Add("270300504 - Hallmark Motor Group T / A New Vaal - Bethlehem - Mitsubishi");
            list.Add("270300505 - Mitsubishi Motors Vereeniging");
            list.Add("270300506 - Sandown Motor Holdings(Pty) Ltd T / A Orbit Boland Mitsubishi");
            list.Add("270300507 - Sandown Motor Holdings(Pty) Ltd t / a Paarl Motors Mitsubishi");
            list.Add("270300508 - M H Cloete Enterprises(Pty) Ltd T / A Rola Motors Mitsubishi");
            list.Add("270300509 - Ronnies Motors Trust - Mitsubishi");
            list.Add("270300510 - Imperial Ford & Mazda George");
            list.Add("270300511 - Sirius Mtr Corp Pty Ltd t / a Union Motors Lowveld PC Mits");
            list.Add("270300512 - Sirius Mtr Corp(Pty) Ltd t / a Union South Coast Mitsubishi");
            list.Add("270300513 - Star Motors - Mitsubishi");
            list.Add("270300516 - COURT ORDER SETTLEMENTS");
            list.Add("270300518 - Imperial Group(Pty)Ltd t / a Cargo Motors Chrysler Jeep Dodge");
            list.Add("270300519 - McCathy Ltd T / A Chrysler Jeep Dodge Wonderboom");
            list.Add("270300520 - J de Wit Motorbelange(Pty) Ltd T / A De Wit Motors Chrysler");
            list.Add("270300521 - Springs Car Wholesalers(pty)Ltd T / A Malmesbury Motors");
            list.Add("270300522 - Super Group Trading(Pty) Ltd T / A CJD East Rand");
            list.Add("270300524 - Zelco Motors(Pty) Ltd");
            list.Add("270300525 - Namaqua Garage(Pty) Ltd(Chrysle)");
            list.Add("270300526 - Sandown Motor Holdings(Pty) Ltd t / a Sandown Motors Tygerber");
            list.Add("270300527 - Ronnies Motors Trust East London CJD");
            list.Add("270300528 - Ronnies Motors Trust T / A Star Motors(Chrysler)");
            list.Add("270300529 - Sandown Motor Holdings(Pty) Ltd t / a CJD Newlands");
            list.Add("270300530 - Restructures - Business Rescue");
            list.Add("270300531 - NMI Durban South Motors(Pty)Ltd T / A NMI DSM Durban CJD");
            list.Add("270300532 - Stucky Motors Newcastle(Pty) Ltd T / A Stucky CJD");
            list.Add("270300533 - Super Group Trading(Pty) Ltd T / A Grand Central Motors CJD");
            list.Add("270300534 - Zelco Motors(Pty) Ltd(Mitsubishi)");
            list.Add("270300535 - Springs Car Wholesalers(Pty)Ltd T / A Malmesbury Motors - Mits");
            list.Add("270300537 - Cargo Motors Mitsubishi Bedfordview");
            list.Add("270300538 - New Vaal South T / A New Vaal(Pty)Ltd(Chrysler)");
            list.Add("270300541 - Mercurius Motors Commercial a Division of Imperial Group Ltd");
            list.Add("270300542 - Imperial Group(Pty) Ltd T / A Mercurius Motors CV Polokwane");
            list.Add("270300543 - Ronnies Motors Mthatha");
            list.Add("270300544 - NMI - DSM Pietermaritzburg - Mitsubishi Motors");
            list.Add("270300545 - Mitsubishi Motors Paarden Eiland");
            list.Add("270300546 - Mitsubishi Sandton");
            list.Add("270300547 - Imperial Group Ltd");
            list.Add("270300548 - Daimler Fleet Management SA Pty Ltd");
            list.Add("270300549 - Naledi Motors Gaborone CV");
            list.Add("270300550 - Naledi Motors Francistown CV");
            list.Add("270300551 - Molapo Motors(Pty) Ltd");
            list.Add("270300552 - KUNENE MOTOR HOLDINGS");
            list.Add("270300553 - Mercedes Benz SA(Staff Agility)");
            list.Add("270300554 - NMI Durban South Motors(Pty) Ltd T / A Union Motors South Coa");
            list.Add("270300555 - NMI Durban South Motors(Pty) Ltd T / A Union Motors Lowveld P");
            list.Add("270300556 - NMI Durban South Motors(Pty) Ltd T / A Union Motors Lowveld C");
            list.Add("270300557 - NMI Durban South Motors(Pty) Ltd T / A Union Motors South Coa");
            list.Add("270300558 - NMI Durban South Motors(Pty) Ltd T / A Union Motors Lowveld -");
            list.Add("270300559 - NMI Durban South Motors(Pty) Ltd T / A Union Motors Lowveld C");
            list.Add("270300560 - NMI Durban South Motors(Pty) Ltd T / A Union Motors South Coa");
            list.Add("270300561 - NMI Durban South Motors(Pty) Ltd - NMI DSM Pinetown CJD");
            list.Add("270300562 - Super Group Trading Pty Ltd - Bellville CV");
            list.Add("270300563 - Super Group Trading Pty Ltd - Mercedes Benz Century City");
            list.Add("270300564 - Super Group Trading Pty Ltd - Mercedes Benz Montague Gardens");
            list.Add("270300565 - Super Group Trading Pty Ltd - Orbit Motors Worcester");
            list.Add("270300566 - Super Group Trading Pty Ltd - Paarl Motors CV");
            list.Add("270300567 - Super Group Trading Pty Ltd - Mercedes Benz Claremont");
            list.Add("270300568 - Super Group Trading Pty Ltd - Mercedes Benz Culemborg");
            list.Add("270300569 - Super Group Trading Pty Ltd - Paarl Motors PC");
            list.Add("270300570 - Super Group Trading Pty Ltd - Mercedes Benz Stellenbosch");
            list.Add("270300572 - Super Group Trading Pty Ltd - CJD Century City");
            list.Add("270300573 - Mercedes Benz South Africa(Pty) Ltd T / A MBSA Online");
            list.Add("270300574 - McCarthy Limited t / a Mercedes - Benz Brooklyn");
            list.Add("270300577 - Nissan South Africa(Pty) Ltd");
            list.Add("270300582 - The Hollard Insurance Company Ltd");

            return list;
        }

        public ActionResult Details(int q)
        {
            return PartialView("_Details");
        }

        public List<int> GetQuantity()
        {
            List<int> list = new List<int>();
            for (int i = 1; i < 500; i++)
            {
                list.Add(i);
            }
            return list;
        }


        public DeliveryViewModel PopulateDeliveryViewModel(DeliveryViewModel viewModel, Intern_LeaveDBEntities db)
        {
            viewModel.CourierViewModel = new DeliveryCourierViewModel();
            viewModel.DriverViewModel = new DeliveryDriverViewModel();
            viewModel.CourierViewModel.DeliveryItems = new List<DeliveryitemViewModel>();
            viewModel.DriverViewModel.DeliveryItems = new List<DeliveryitemViewModel>();

            //viewModel.callCentreViewModel = db.NatisDatas.Where(a => a.NatisLocation == "Call Centre");
            //viewModel.legalViewModel = db.NatisDatas.Where(a => a.NatisLocation == "Legal");
            //viewModel.maturitiesViewModel = db.NatisDatas.Where(a => a.NatisLocation == "Maturities");

            var SentInDeliveries = db.SentIN_Delivery;
            var CourierSentInDeliveries = SentInDeliveries.Where(w => w.DeliveryChoice == "Courier" && w.CourierStatus == "Transit").ToList();
            foreach (var item in CourierSentInDeliveries)
            {
                DeliveryitemViewModel itemViewModel = new DeliveryitemViewModel();
                itemViewModel.RecordNumber = item.RecordNumber;
                itemViewModel.DealershipName = item.DealershipName;
                itemViewModel.WaybillNumber = item.PackageNumber;
                itemViewModel.PackageSender = item.PackageSender;
                itemViewModel.Recipient = item.PackageRecipient;
                itemViewModel.DateSent = item.DateSent;
                itemViewModel.DateRecieved = new DateTime();
                itemViewModel.DeliveryStatus = item.CourierStatus;
                itemViewModel.DeliveryChoice = item.DeliveryChoice;
                itemViewModel.Quantity = (int)item.Quantity;
                itemViewModel.DealerList = getList();
                itemViewModel.Comment = item.Comment;

                itemViewModel.ContractNumberItems = new List<DeliveryItemContractViewModel>();
                foreach (var contractNumber in item.ContractNumbers)
                {
                    DeliveryItemContractViewModel contractViewModel = new DeliveryItemContractViewModel();
                    contractViewModel.ContractNumber = contractNumber.ContractNumber1;
                    contractViewModel.RecordNumber = (int)contractNumber.RecordNumber;
                    contractViewModel.IsRecieved = (bool)contractNumber.IsReceived;

                    itemViewModel.ContractNumberItems.Add(contractViewModel);
                }

                viewModel.CourierViewModel.DeliveryItems.Add(itemViewModel);
            }

            var DriverSentInDeliveries = SentInDeliveries.Where(w => w.DeliveryChoice == "Driver" && w.CourierStatus == "Transit").ToList();
            foreach (var item in DriverSentInDeliveries)
            {
                DeliveryitemViewModel itemViewModel = new DeliveryitemViewModel();
                itemViewModel.RecordNumber = item.RecordNumber;
                itemViewModel.DealershipName = item.DealershipName;
                itemViewModel.WaybillNumber = item.PackageNumber;
                itemViewModel.PackageSender = item.PackageSender;
                itemViewModel.Recipient = item.PackageRecipient;
                itemViewModel.DateSent = item.DateSent;
                itemViewModel.DateRecieved = new DateTime();
                itemViewModel.DriverContacts = item.DriverContact;
                itemViewModel.DriverDetails = item.DriverDetails;
                itemViewModel.DeliveryStatus = item.CourierStatus;
                itemViewModel.DeliveryChoice = item.DeliveryChoice;
                itemViewModel.Quantity = (int)item.Quantity;
                itemViewModel.DealerList = getList();
                itemViewModel.Comment = item.Comment;

                itemViewModel.ContractNumberItems = new List<DeliveryItemContractViewModel>();
                foreach (var contractNumber in item.ContractNumbers)
                {
                    DeliveryItemContractViewModel contractViewModel = new DeliveryItemContractViewModel();
                    contractViewModel.ContractNumber = contractNumber.ContractNumber1;
                    contractViewModel.RecordNumber = (int)contractNumber.RecordNumber;
                    contractViewModel.IsRecieved = (bool)contractNumber.IsReceived;

                    itemViewModel.ContractNumberItems.Add(contractViewModel);
                }

                viewModel.DriverViewModel.DeliveryItems.Add(itemViewModel);
            }

            return viewModel;
        }

        public DriverPackage PopulateDriverPackage(Intern_LeaveDBEntities db)
        {
            DriverPackage driverPackage = new DriverPackage();
            var driver = db.TickBoxDatas.Where(a => a.RecipientName == null);
            driverPackage.PackageItems = new List<DriverPackageItems>();

            foreach (var item in driver)
            {
                DriverPackageItems a = new DriverPackageItems();
                a.DriverId = item.TableId;
                a.ItemQuantity = item.ItemQuantity;
                a.SentBy = item.SenderName;
                a.SentDate = item.SentDate;

                a.ContractNumbers = new List<DriverContracts>();
                foreach (var contractNumber in item.ContractNumbers)
                {
                    DriverContracts driverContract = new DriverContracts();
                    driverContract.DriverId = (int)contractNumber.TableId;
                    driverContract.ContractNumber = contractNumber.ContractNumber1;
                    a.ContractNumbers.Add(driverContract);
                }
                driverPackage.PackageItems.Add(a);
            }

            return driverPackage;
        }

        public DriverPackage PopulateDriverPackage1(Intern_LeaveDBEntities db)
        {
            DriverPackage driverPackage = new DriverPackage();
            var driver = db.TickBoxDatas.Where(a => a.RecipientName != null);
            driverPackage.PackageItems = new List<DriverPackageItems>();

            foreach (var item in driver)
            {
                DriverPackageItems a = new DriverPackageItems();
                a.DriverId = item.TableId;
                a.ItemQuantity = item.ItemQuantity;
                a.SentBy = item.SenderName;
                a.SentDate = item.SentDate;

                a.ContractNumbers = new List<DriverContracts>();
                foreach (var contractNumber in item.ContractNumbers)
                {
                    DriverContracts driverContract = new DriverContracts();
                    driverContract.DriverId = (int)contractNumber.TableId;
                    driverContract.ContractNumber = contractNumber.ContractNumber1;
                    driverContract.IsReceived = (bool)contractNumber.IsReceived;
                    a.ContractNumbers.Add(driverContract);
                }
                driverPackage.PackageItems.Add(a);
            }

            return driverPackage;
        }
    }
}