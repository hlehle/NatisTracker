using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NatisTracker.ViewModels
{
    public class NatisRequests
    {
        public string name { get; set; }
        public DateTime requestDate { get; set; }
        public string department { get; set; }
        public string vin { get; set; }
        [Required, StringLength(6, MinimumLength = 6) ]
        public string contractNo { get; set; }
        public string responder { get; set; }
        public string respondDate { get; set; }
    }
}