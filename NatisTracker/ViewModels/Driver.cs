using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NatisTracker.ViewModels
{
    public class Driver
    {
        public int Quantity { get; set; }
        public List<int> QuantityList { get; set; }
        public string ContractNumber { get; set; }
        public List<string> ContractList { get; set; }
        public string Comment { get; set; }
    }
}