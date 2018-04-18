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
        public DeliveryitemViewModel sendDelivery(DeliveryitemViewModel viewModel, string name, string surname)
        {
            Intern_LeaveDBEntities db = new Intern_LeaveDBEntities();
            
            if (viewModel.WaybillNumber != null)// || view.DriverDetails != null)
            {
                SentOUT_Delivery database = new SentOUT_Delivery();
                NatisData data = new NatisData();

                database.VinNumber = viewModel.ContractStrings;
                database.PackageNumber = viewModel.WaybillNumber;
                database.PackageSender = name + " " + surname;
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
        public DeliveryViewModel receiveDelivery(DeliveryViewModel view, string name, string surname) { return new DeliveryViewModel(); }

    }
}