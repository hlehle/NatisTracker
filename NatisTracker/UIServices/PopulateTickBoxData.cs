using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NatisTracker.UIServices;
using NatisTracker.ViewModels;
using EnatisRepository.Repo;

namespace NatisTracker.UIServices
{
    public class PopulateTickBoxData : Populate
    {
        public void PopulateSendToDriver(DriverDocsViewModel ViewModel, string user, string department)
        {
            using (NatisTrackerDBEntities db = new NatisTrackerDBEntities())
            {
                TickBoxData driverData = new TickBoxData();
                driverData.SenderName = user;
                driverData.SenderDepartment = department;
                driverData.SentDate = DateTime.Now;
                driverData.ItemQuantity = ViewModel.Quantity;
                driverData.RecipientType = "Driver";
                driverData.IsConfirmed = false;
                db.TickBoxDatas.Add(driverData);
                db.SaveChanges();
                ContractNumber contractsData = new ContractNumber();
                string[] contracts = getContractsNo(ViewModel.ContractNumber);
                for (int i = 0; i < contracts.Length; i++)
                {
                    contractsData.ContractNumber1 = contracts[i];
                    contractsData.TableId = driverData.TableId;
                    db.ContractNumbers.Add(contractsData);
                    db.SaveChanges();
                }
                
            }
            
        }
        public void PopulateSendToUser(SendToUserViewModel ViewModel, string user, string department)
        {
            using (NatisTrackerDBEntities db = new NatisTrackerDBEntities())
            {
                TickBoxData driverData = new TickBoxData();
                driverData.SenderName = user;
                driverData.SenderDepartment = department;
                driverData.SentDate = DateTime.Now;
                driverData.RecipientName = ViewModel.RecipientName;
                driverData.ItemQuantity = ViewModel.Quantity;
                driverData.RecipientType = "Internal User";
                driverData.IsConfirmed = false;
                db.TickBoxDatas.Add(driverData);
                db.SaveChanges();
                ContractNumber contractsData = new ContractNumber();
                string[] contracts = getContractsNo(ViewModel.ContractNumber);
                for (int i = 0; i < contracts.Length; i++)
                {
                    contractsData.ContractNumber1 = contracts[i];
                    contractsData.TableId = driverData.TableId;
                    db.ContractNumbers.Add(contractsData);
                    db.SaveChanges();
                }

            }

        }
        public string[] getContractsNo(string contracts)
        {
            return contracts.Trim().Split(' ');
        }
    }
}