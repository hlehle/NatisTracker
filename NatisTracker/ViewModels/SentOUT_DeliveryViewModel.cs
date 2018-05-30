using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NatisTracker.ViewModels
{
    public class SentOUT_DeliveryViewModel
    {
        public int RecordNumber { get; set; }
        public string VinNumber { get; set; }
        public string DeliveryChoice { get; set; }
        public DateTime DateSent { get; set; }
        public string PackageSender { get; set; }
        public string PackageNumber { get; set; }
        public string DriverDetails { get; set; }
        public string DriverContacts { get; set; }
        public string RecipientDetails { get; set; }
        public string RecipientContacts { get; set; }
        public string Comments { get; set; }
    }
}