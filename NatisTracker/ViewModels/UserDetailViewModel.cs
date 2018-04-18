using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NatisTracker.ViewModels
{
    public class UserDetailViewModel
    {
        //public string username { get; set; }
        public string Recipient_Name { get; set; }
        public string Recipient_Contacts { get; set; }
        public List<string> list { get; set; }
        public string PackageNumber { get; set; }
        [Required]
        public string deliveryChoice { get; set; }

        public string DriversName { get; set; }
        public string DriverContacts { get; set; }
        public DateTime DateReceived { get; set; }
        public string status { get; set; }
        public string Comment { get; set; }
        [Required]
        public int ItemQuantity { get; set; }
        [Required]
        public string DealershipName { get; set; }
        //public HttpPostedFileBase File { get; set; }

        [Required]
        public string contractNo { get; set; }
        public IList<ContractViewModel> con { get; set; }
        //public string VINnumber { get; set; }
    }

    public class ContractViewModel
    {
        public bool IsRecieved { get; set; }
        public string ContractNumber { get; set; }
        //public int RecordNumber { get; set; }
    }
}