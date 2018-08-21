using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NatisTracker.ViewModels
{
    public class RequestsDataViewModel
    {
        public int RecordNumber { get; set; }
        public string RequesterName { get; set; }
        public string RequesterDepartment { get; set; }
        public string RequestStatus { get; set; }
        public DateTime RequestDate { get; set; }
        public string Vin { get; set; }
        public string ContractNo { get; set; }
        public Nullable<DateTime> ReplyDate { get; set; }
        public string Responder { get; set; }
        public Nullable<DateTime> CollectionDate { get; set; }

    }
}