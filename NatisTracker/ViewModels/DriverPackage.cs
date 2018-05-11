using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NatisTracker.ViewModels
{
    public class DriverPackage
    {
        public int PackageId { get; set; }
        public IList<DriverPackageItems> PackageItems { get; set; }

        public bool IsConfirmed { get; set; }
    }

    public class DriverContracts
    {
        public int DriverId { get; set; }

        public bool IsReceived { get; set; }
        public string ContractNumber { get; set; }
    }

    public class DriverPackageItems
    {
        public int DriverId { get; set; }
        public int ItemQuantity { get; set; }
        public string SentBy { get; set; }
        public DateTime SentDate { get; set; }
        public string Confirmation { get; set; }
        public List<DriverContracts> ContractNumbers { get; set; }
        
    }
}