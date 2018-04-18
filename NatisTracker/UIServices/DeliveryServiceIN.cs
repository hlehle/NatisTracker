using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NatisTracker.ViewModels;
using System.Web.Mvc;

namespace NatisTracker.Models
{
    public class DeliveryServiceIN : IDeliveryService
    {
        public DeliveryitemViewModel sendDelivery(DeliveryitemViewModel viewModel, string name, string surname)
        {
            using (Intern_LeaveDBEntities db = new Intern_LeaveDBEntities())
            {

                if (viewModel.WaybillNumber != null || viewModel.DriverDetails != null)
                {
                    SentIN_Delivery database = new SentIN_Delivery();
                    ContractNumber c = new ContractNumber();

                    database.DealershipName = viewModel.DealershipName;
                    database.PackageNumber = viewModel.WaybillNumber;
                    database.PackageSender = name + " " + surname;
                    database.DateSent = DateTime.Now;
                    database.CourierStatus = "Transit";
                    database.DeliveryChoice = viewModel.DeliveryChoice;
                    database.DriverContact = viewModel.DriverContacts;
                    database.DriverDetails = viewModel.DriverDetails;
                    database.Quantity = viewModel.Quantity;
                    database.Comment = viewModel.Comment;
                    database.SenderEmail = viewModel.SenderEmail;

                    db.SentIN_Delivery.Add(database);

                    string[] contracts = getContractsNo(viewModel.ContractStrings);
                    for (int j = 0; j < contracts.Length; j++)
                    {
                        c.ContractNumber1 = contracts[j];
                        c.RecordNumber = database.RecordNumber;
                        c.IsReceived = false;
                        db.ContractNumbers.Add(c);
                        db.SaveChanges();
                    }
                }
                db.SaveChanges();
            }
            return viewModel;
        }

        public DeliveryViewModel receiveDelivery(DeliveryViewModel viewModel, string name, string surname)
        {
            using (Intern_LeaveDBEntities db = new Intern_LeaveDBEntities())
            {
                if (viewModel.CourierViewModel != null)
                {
                    for (int i = 0; i < viewModel.CourierViewModel.DeliveryItems.Count; i++)
                    {
                        if (!viewModel.CourierViewModel.DeliveryItems[i].DeliveryStatus.Equals("Transit"))
                        {
                            var num = viewModel.CourierViewModel.DeliveryItems[i].RecordNumber;
                            var delivery = db.SentIN_Delivery.First(r => r.RecordNumber == num);
                            delivery.CourierStatus = viewModel.CourierViewModel.DeliveryItems[i].DeliveryStatus;
                            delivery.DateReceived = DateTime.Now;
                            delivery.PackageRecipient = name+" "+surname;

                            for (int j = 0; j < viewModel.CourierViewModel.DeliveryItems[i].ContractNumberItems.Count(); j++)
                            {
                                var ContractViewModel = viewModel.CourierViewModel.DeliveryItems[i].ContractNumberItems[j];
                                var recordNumber = ContractViewModel.RecordNumber;
                                var deliverySentIn = db.SentIN_Delivery.First(f => f.RecordNumber == recordNumber);
                                var contractNumber = ContractViewModel.ContractNumber;
                                var contractnumbers = deliverySentIn.ContractNumbers.First(f => f.ContractNumber1 == contractNumber);
                                contractnumbers.IsReceived = viewModel.CourierViewModel.DeliveryItems[i].ContractNumberItems[j].IsRecieved;
                                //contractnumbers.ContractNumber1 = viewModel.CourierViewModel.DeliveryItems[i].ContractNumberItems[j].ContractNumber;
                            }
                        }

                    }
                }

                else if(viewModel.DriverViewModel != null)
                {


                    for (int i = 0; i < viewModel.DriverViewModel.DeliveryItems.Count; i++)
                    {
                        if (!viewModel.DriverViewModel.DeliveryItems[i].DeliveryStatus.Equals("Transit"))
                        {
                            var num = viewModel.DriverViewModel.DeliveryItems[i].RecordNumber;
                            var delivery = db.SentIN_Delivery.First(r => r.RecordNumber == num);
                            delivery.CourierStatus = viewModel.DriverViewModel.DeliveryItems[i].DeliveryStatus;
                            delivery.DateReceived = DateTime.Now;
                            delivery.PackageRecipient = name + " " + surname;

                            for (int j = 0; j < viewModel.DriverViewModel.DeliveryItems[i].ContractNumberItems.Count(); j++)
                            {
                                var ContractViewModel = viewModel.DriverViewModel.DeliveryItems[i].ContractNumberItems[j];
                                var recordNumber = ContractViewModel.RecordNumber;
                                var deliverySentIn = db.SentIN_Delivery.First(f => f.RecordNumber == recordNumber);
                                var contractNumber = ContractViewModel.ContractNumber;
                                var contractnumbers = deliverySentIn.ContractNumbers.First(f => f.ContractNumber1 == contractNumber);
                                contractnumbers.IsReceived = viewModel.DriverViewModel.DeliveryItems[i].ContractNumberItems[j].IsRecieved;
                                //contractnumbers.ContractNumber1 = viewModel.CourierViewModel.DeliveryItems[i].ContractNumberItems[j].ContractNumber;
                            }
                        }

                    }
                }

                else { }

                db.SaveChanges();
                return viewModel;
            }
        }

        public string[] getContractsNo(string contracts)
        {
            return contracts.Trim().Split(' ');
        }
    }
}