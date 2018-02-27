using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NatisTracker.Models;

namespace NatisTracker.ViewModels
{
    public class MasterViewModel
    {
        public IEnumerable<NatisDelivery> deliveryModel { get; set; }
        public IEnumerable<NatisRequest> RequestViewModel { get; set; }
        public UserDetailViewModel viewModel { get; set; }
        public IEnumerable<NatisData> natisDataViewModel { get; set; }
        public IEnumerable<NatisData> maturitiesViewModel { get; set; }
        public IEnumerable<NatisData> legalViewModel { get; set; }
        public IEnumerable<NatisData> callCentreViewModel { get; set; }

    }
}