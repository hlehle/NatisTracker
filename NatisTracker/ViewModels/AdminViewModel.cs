using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NatisTracker.ViewModels;

namespace NatisTracker.ViewModels
{
    public class AdminViewModel
    {
        public IList<RequestsDataViewModel> RequestsList { get; set; }
        public IList<ScanLogsDataViewModel> ScanLogsList { get; set; }
        public IList<NatisDataViewModel> NatisDataList { get; set; }
        public IList<SentIN_DeliveryViewModel> SentIN_Delivery { get; set; }
        public IList<NatisDataViewModel> Maturities { get; set; }
        public IList<NatisDataViewModel> Call_Centre { get; set; }
        public IList<NatisDataViewModel> Legal { get; set; }
        public IList<NatisDataViewModel> FleetServiceDriver { get; set; }
        public IList<NatisDataViewModel> Operations { get; set; }
        public IList<NatisDataViewModel> Originations { get; set; }
        public IList<NatisDataViewModel> Remarketing { get; set; }
        public DriverDocsViewModel DriverViewModel { get; set; }
        public SendToUserViewModel SendToUserView { get; set; }
    }

    public class DriverDocsViewModel
    {
        public int Quantity { get; set; }
        public List<int> QuantityList { get; set; }
        public string ContractNumber { get; set; }
        public List<string> ContractList { get; set; }
        public string Comment { get; set; }
    }

    public class SendToUserViewModel
    {
        public int Quantity { get; set; }
        public List<int> QuantityList { get; set; }
        public string ContractNumber { get; set; }
        public List<string> ContractList { get; set; }
        public string Comment { get; set; }
        public List<string> PeopleList { get; set; }
        public string RecipientName { get; set; }
    }
}