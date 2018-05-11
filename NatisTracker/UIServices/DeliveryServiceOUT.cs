using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NatisTracker.Models;
using System.Web.Mvc;
using NatisTracker.ViewModels;

namespace NatisTracker.Models
{
    public class DeliveryServiceOUT : IDeliveryService
    {
        public DeliveryitemViewModel sendDelivery(DeliveryitemViewModel viewModel, string name)
        {
            Intern_LeaveDBEntities db = new Intern_LeaveDBEntities();
            
            if (viewModel.WaybillNumber != null)// || view.DriverDetails != null)
            {
                SentOUT_Delivery database = new SentOUT_Delivery();
                NatisData data = new NatisData();

                database.VinNumber = viewModel.ContractStrings;
                database.PackageNumber = viewModel.WaybillNumber;
                database.PackageSender = name;
                database.DateSent = DateTime.Now;
                //database.CourierStatus = "Transit";
                database.DeliveryChoice = viewModel.DeliveryChoice;
                //database.DriverContacts = view.CourierViewModel.DeliveryItems[0].DriverContacts;
                database.RecipientDetails = viewModel.Recipient;
                database.RecipientContacts = viewModel.RecipientContacts;
                //database.DriverDetails = view.CourierViewModel.DeliveryItems[0].DriverDetails;
                database.Comments = viewModel.Comment;

                db.SentOUT_Delivery.Add(database);

                data = db.NatisDatas.Single(b => b.VinNumber == database.VinNumber);
                data.NatisLocation = "Away";

                db.SaveChanges();

            }

            return viewModel;

        }
        public DeliveryViewModel receiveDelivery(DeliveryViewModel view, string name) { return new DeliveryViewModel(); }

        public void SendToDriver(DriverPackage viewModel, string name)
        {
            using (Intern_LeaveDBEntities db = new Intern_LeaveDBEntities())
            {
                if (viewModel.PackageItems != null)
                {
                    for (int i = 0; i < viewModel.PackageItems.Count; i++)
                    {
                        if (!viewModel.PackageItems[i].Confirmation.Equals("Transit"))
                        {
                            var num = viewModel.PackageItems[i].DriverId;
                            var delivery = db.DriverDatas.First(r => r.DriverId == num);
                            delivery.DriverName = name;
                            //delivery.DateReceived = DateTime.Now;
                            //delivery.PackageRecipient = name;

                            for (int j = 0; j < viewModel.PackageItems[i].ContractNumbers.Count(); j++)
                            {
                                var ContractViewModel = viewModel.PackageItems[i].ContractNumbers[j];
                                var driverId = ContractViewModel.DriverId;
                                var deliverySentIn = db.DriverDatas.First(f => f.DriverId == driverId);
                                var contractNumber = ContractViewModel.ContractNumber;
                                var contractnumbers = deliverySentIn.ContractNumbers.First(f => f.ContractNumber1 == contractNumber);
                                contractnumbers.IsReceived = viewModel.PackageItems[i].ContractNumbers[j].IsReceived;

                                var natisContract = db.ContractsDatas.Where(a => a.ContractNumber == contractnumbers.ContractNumber1).FirstOrDefault();
                                var natis = db.NatisDatas.Where(a => a.VinNumber == natisContract.VinNumber).FirstOrDefault();

                                if (viewModel.IsConfirmed && (bool)contractnumbers.IsReceived && viewModel.PackageItems[i].Confirmation.Equals("Accepted"))
                                {
                                    natis.NatisLocation = "Driver: " + name;
                                }
                                //contractnumbers.ContractNumber1 = viewModel.CourierViewModel.DeliveryItems[i].ContractNumberItems[j].ContractNumber;
                            }
                        }

                    }
                }
                db.SaveChanges();
            }

        }

        public void ReceivePackage(DriverPackage viewModel, string name, string department)
        {
            using (Intern_LeaveDBEntities db = new Intern_LeaveDBEntities())
            {
                if (viewModel.PackageItems != null)
                {
                    for (int i = 0; i < viewModel.PackageItems.Count; i++)
                    {
                        if (!viewModel.PackageItems[i].Confirmation.Equals("Transit"))
                        {
                            var num = viewModel.PackageItems[i].DriverId;
                            var delivery = db.DriverDatas.First(r => r.DriverId == num);
                            delivery.DriverName = name;
                            //delivery.DateReceived = DateTime.Now;
                            //delivery.PackageRecipient = name;

                            for (int j = 0; j < viewModel.PackageItems[i].ContractNumbers.Count(); j++)
                            {
                                var ContractViewModel = viewModel.PackageItems[i].ContractNumbers[j];
                                var driverId = ContractViewModel.DriverId;
                                var deliverySentIn = db.DriverDatas.First(f => f.DriverId == driverId);
                                var contractNumber = ContractViewModel.ContractNumber;
                                var contractnumbers = deliverySentIn.ContractNumbers.First(f => f.ContractNumber1 == contractNumber);
                                contractnumbers.IsReceived = viewModel.PackageItems[i].ContractNumbers[j].IsReceived;

                                var natisContract = db.ContractsDatas.Where(a => a.ContractNumber == contractnumbers.ContractNumber1).FirstOrDefault();
                                var natis = db.NatisDatas.Where(a => a.VinNumber == natisContract.VinNumber).FirstOrDefault();

                                if (viewModel.IsConfirmed && (bool)contractnumbers.IsReceived && viewModel.PackageItems[i].Confirmation.Equals("Accepted"))
                                {
                                    natis.NatisLocation = department;
                                }
                                //contractnumbers.ContractNumber1 = viewModel.CourierViewModel.DeliveryItems[i].ContractNumberItems[j].ContractNumber;
                            }
                        }

                    }
                }
                db.SaveChanges();
            }

        }
    }
}