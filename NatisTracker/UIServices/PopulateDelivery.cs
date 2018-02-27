using NatisTracker.Models;
using NatisTracker.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NatisTracker.UIServices
{
    public class PopulateDelivery
    {
        public MasterViewModel sendDelivery(MasterViewModel view, string name, string surname)
        {
            using (Intern_LeaveDBEntities db = new Intern_LeaveDBEntities())
            {
                if (view.viewModel.PackageNumber != null || view.viewModel.DriversName != null)
                {
                    try
                    {
                        NatisDelivery database = new NatisDelivery();

                        database.DealershipName = view.viewModel.DealershipName;
                        database.PackageNumber = view.viewModel.PackageNumber;
                        database.PackageSender = name + " " + surname;
                        database.DateSent = DateTime.Now;
                        database.CourierStatus = "Transit";
                        database.DeliveryChoice = view.viewModel.deliveryChoice;
                        database.DriverCantact = view.viewModel.DriverContacts;
                        database.DriverDetails = view.viewModel.DriversName;
                        database.Quantity = view.viewModel.Quantity;
                        database.Comment = view.viewModel.Comment;

                        db.NatisDeliveries.Add(database);
                        db.SaveChanges();

                        return view;
                    }
                    catch (Exception ex)
                    {

                        throw;
                    }
                }

                else
                {
                    return view;
                }
            }
        }

        public void receiveDelivery( FormCollection form, string name, string surname)
        {
            string[] recordNumbers = form["RecordNumber"].Split(new char[] { ',' });
            string[] replies = form["Delivery_Status"].Split(new char[] { ',' });

            using (Intern_LeaveDBEntities db = new Intern_LeaveDBEntities())
            {
                NatisDelivery deliveries = new NatisDelivery();

                for (int i = 0; i < recordNumbers.Length; i++)
                {
                    int recordNumber = int.Parse(recordNumbers[i]);
                    string reply = replies[i];

                    deliveries = db.NatisDeliveries.Single(u => u.RecordNumber == recordNumber);
                    deliveries.PackageRecipient = name + " " + surname;
                    deliveries.DateReceived = DateTime.Now;
                    deliveries.CourierStatus = reply.ToString();

                    db.SaveChanges();
                }
            }
        }
    }
}