using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NatisTracker.ViewModels
{
    public class ScanLogsDataViewModel
    {
        public int RecordNumber { get; set; }
        public string ContractNumber { get; set; }
        public string VinNumber { get; set; }
        public DateTime DateScanned { get; set; }
        public string User { get; set; }
        public string Department { get; set; }
        public string ContractStatus { get; set; }
        public string ContractDescription { get; set; }
        public string Comment { get; set; }
    }
}