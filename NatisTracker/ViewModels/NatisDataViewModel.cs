using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NatisTracker.ViewModels
{
    public class NatisDataViewModel
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
    }
}