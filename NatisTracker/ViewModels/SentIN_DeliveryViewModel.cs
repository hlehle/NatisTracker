using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NatisTracker.ViewModels
{
    public class SentIN_DeliveryViewModel
    {
        public int RecordNumber { get; set; }
        public string DealershipName { get; set; }
        public string PackageNumber { get; set; }
        public string PackageSender { get; set; }
        public string PackageRecipient { get; set; }
        public DateTime DateSent { get; set; }
        public DateTime DateReceived { get; set; }
        public string CourierStatus { get; set; }
        public string Comment { get; set; }
        public int Quantity { get; set; }
        public string DriverDetails { get; set; }
        public string DriverContact { get; set; }
        public string DeliveryChoice { get; set; }
        public string SenderEmail { get; set; }
        public List<ContractNumbersViewModel> ContractsList { get; set; }
    }
}