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
            
            if (viewModel.WaybillNumber != null || viewModel.DriverDetails != null)
            {
                SentOUT_Delivery database = new SentOUT_Delivery();
                NatisData data = new NatisData();

                var con = viewModel.ContractStrings;
                var vin = db.ContractsDatas.Where(a => a.ContractNumber == con).Select(a => a.VinNumber).FirstOrDefault();

                database.VinNumber = vin;
                database.PackageNumber = viewModel.WaybillNumber;
                database.PackageSender = name;
                database.DateSent = DateTime.Now;
                //database.CourierStatus = "Transit";
                database.DeliveryChoice = viewModel.DeliveryChoice;
                database.DriverContacts = viewModel.DriverContacts;
                database.RecipientDetails = viewModel.Recipient;
                database.RecipientContacts = viewModel.RecipientContacts;
                database.DriverDetails = viewModel.DriverDetails;
                database.Comments = viewModel.Comment;

                db.SentOUT_Delivery.Add(database);

                data = db.NatisDatas.Single(b => b.VinNumber == database.VinNumber);
                data.NatisLocation = "Away";

                db.SaveChanges();

            }

            return viewModel;

        }
        public DeliveryViewModel receiveDelivery(DeliveryViewModel view, string name, string email) { return new DeliveryViewModel(); }

        public void SendNatis(TickBoxViewModelList viewModel, string name, string department)
        {
            using (Intern_LeaveDBEntities db = new Intern_LeaveDBEntities())
            {
                if (viewModel.TickBoxList != null)
                {
                    for (int i = 0; i < viewModel.TickBoxList.Count; i++)
                    {
                        if (!viewModel.TickBoxList[i].Reply.Equals("Transit"))
                        {
                            var num = viewModel.TickBoxList[i].DriverId;
                            var delivery = db.TickBoxDatas.Where(r => r.TableId == num).FirstOrDefault();
                            delivery.RecipientName = name;
                            delivery.IsConfirmed = viewModel.IsConfirmed;
                            delivery.RecipientDepartment = department;
                            delivery.ReceivedDate = DateTime.Now;

                            for (int j = 0; j < viewModel.TickBoxList[i].ContractsList.Count(); j++)
                            {
                                var ContractViewModel = viewModel.TickBoxList[i].ContractsList[j];
                                var driverId = ContractViewModel.DriverId;
                                var deliverySentIn = db.TickBoxDatas.First(f => f.TableId == driverId);
                                var contractNumber = ContractViewModel.ContractNumber1;
                                var contractnumbers = deliverySentIn.ContractNumbers.First(f => f.ContractNumber1 == contractNumber);
                                contractnumbers.IsReceived = viewModel.TickBoxList[i].ContractsList[j].IsReceived;

                                var natisContract = db.ContractsDatas.Where(a => a.ContractNumber == contractnumbers.ContractNumber1).FirstOrDefault();
                                var natis = db.NatisDatas.Where(a => a.VinNumber == natisContract.VinNumber).FirstOrDefault();

                                if (viewModel.IsConfirmed && (bool)contractnumbers.IsReceived && viewModel.TickBoxList[i].Reply.Equals("Accepted"))
                                {
                                    if (delivery.RecipientType.Equals("Driver"))
                                    {
                                        natis.NatisLocation = "Fleet Services";
                                    }
                                    else
                                    {
                                        natis.NatisLocation = department;
                                    }
                                    
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
                            var delivery = db.TickBoxDatas.First(r => r.TableId == num);
                            delivery.RecipientName = name;
                            //delivery.DateReceived = DateTime.Now;
                            //delivery.PackageRecipient = name;

                            for (int j = 0; j < viewModel.PackageItems[i].ContractNumbers.Count(); j++)
                            {
                                var ContractViewModel = viewModel.PackageItems[i].ContractNumbers[j];
                                var driverId = ContractViewModel.DriverId;
                                var deliverySentIn = db.TickBoxDatas.First(f => f.TableId == driverId);
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