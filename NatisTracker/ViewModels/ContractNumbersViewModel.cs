using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NatisTracker.ViewModels
{
    public class ContractNumbersViewModel
    {
        public int ID { get; set; }
        public int RecordNumber { get; set; }
        public string ContractNumber1 { get; set; }
        public bool IsReceived { get; set; }
        public int DriverId { get; set; }
    }
}