using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NatisTracker.ViewModels
{
    public class EndUserViewModel
    {
        public IList<RequestsDataViewModel> RequestsList { get; set; }
        public IList<NatisDataViewModel> NatisDataList { get; set; }
        public DeliveryitemViewModel viewModel { get; set; }
    }
}