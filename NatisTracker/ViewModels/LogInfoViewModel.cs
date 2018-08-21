using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NatisTracker.ViewModels
{
    public class LogInfoViewModel
    {
        public HttpPostedFileBase file { get; set; }
        public string contractNo { get; set; }
        public string VIN { get; set; }
        public DateTime Date { get; set; }
        public string user { get; set; }
        public string department { get; set; }
        public string status { get; set; }
        public string comment { get; set; }
    }
}