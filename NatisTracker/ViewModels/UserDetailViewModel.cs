using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NatisTracker.ViewModels
{
    public class UserDetailViewModel
    {
        public string Employee_Name { get; set; }
        public string Employee_Surname { get; set; }
        public List<string> list { get; set; }
        public string PackageNumber { get; set; }
        public string deliveryChoice { get; set; }

        public string DriversName { get; set; }

        public int Quantity { get; set; }
        public string DriverContacts { get; set; }
        public DateTime DateReceived { get; set; }
        public string status { get; set; }
        public string Comment { get; set; }
        public int ItemQuantity { get; set; }
        public string DealershipName { get; set; }
        public HttpPostedFileBase File { get; set; }

        public string contractNo { get; set; }
    }
}