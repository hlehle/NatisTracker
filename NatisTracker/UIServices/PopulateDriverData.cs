using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NatisTracker.UIServices;
using NatisTracker.ViewModels;
using NatisTracker.Models;

namespace NatisTracker.UIServices
{
    public class PopulateDriverData : Populate
    {
        public void PopulateData(Driver ViewModel, string user)
        {
            using (Intern_LeaveDBEntities db = new Intern_LeaveDBEntities())
            {
                DriverData driverData = new DriverData();
                driverData.SentBy = user;
                driverData.SentDate = DateTime.Now;
                driverData.ItemQuantity = ViewModel.Quantity;
                db.DriverDatas.Add(driverData);
                db.SaveChanges();
                ContractNumber contractsData = new ContractNumber();
                string[] contracts = getContractsNo(ViewModel.ContractNumber);
                for (int i = 0; i < contracts.Length; i++)
                {
                    contractsData.ContractNumber1 = contracts[i];
                    contractsData.DriverId = driverData.DriverId;
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