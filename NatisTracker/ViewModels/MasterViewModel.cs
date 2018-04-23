using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NatisTracker.Models;

namespace NatisTracker.ViewModels
{        
    public class MasterViewModel
    {
        public IEnumerable<SentIN_Delivery> deliveryModel { get; set; }
        public IEnumerable<RequestsData> RequestViewModel { get; set; }
        public UserDetailViewModel viewModel { get; set; }
        public IEnumerable<NatisData> natisDataViewModel { get; set; }
        public IEnumerable<NatisData> maturitiesViewModel { get; set; }
        public IEnumerable<NatisData> legalViewModel { get; set; }
        public IEnumerable<NatisData> callCentreViewModel { get; set; }
        public IList<ContractsData> contractData { get; set; }
        public NatisAndContractViewModel natis { get; set; }
        public LogInfoViewModel logInfo { get; set; }
        public IList<ContractNumber> contracts { get; set; }
        public IEnumerable<ScanLogsData> logs { get; set; }

    }
    
}