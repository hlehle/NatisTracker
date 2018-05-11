using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NatisTracker.ViewModels
{
    public class NatisAndContractViewModel
    {
        public HttpPostedFileBase file { get; set; }
        public string vin { get; set; }
        public string contractNo { get; set; }
        public string registrationNo { get; set; }
        public string engineNo { get; set; }
        public string carMake { get; set; }
        public string seriesNo { get; set; }
        public string description { get; set; }
        public DateTime registrationDate { get; set; }
        public string OwnerName { get; set; }
        public string OwnerID { get; set; }
        public string natisLocation { get; set; }
        public DateTime DateScanned { get; set; }
        public string Department { get; set; }
        public string ContractStatus { get; set; }
        public string StatusDescription { get; set; }
        public IList<ContractInformation> contractInformation { get; set; }
        public string ScanningUser { get; set; }
        public string Comment { get; set; }
    }

    public class ContractInformation
    {
        public int RecordNumber { get; set; }
        public string ContractNumber { get; set; }
        public string ContractStatus { get; set; }
        public string StatusDescription { get; set; }
        public string VinNumber { get; set; }
    }
}