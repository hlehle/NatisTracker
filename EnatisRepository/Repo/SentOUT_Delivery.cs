//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace EnatisRepository.Repo
{
    using System;
    using System.Collections.Generic;
    
    public partial class SentOUT_Delivery
    {
        public int RecordNumber { get; set; }
        public string VinNumber { get; set; }
        public string DeliveryChoice { get; set; }
        public System.DateTime DateSent { get; set; }
        public string PackageSender { get; set; }
        public string PackageNumber { get; set; }
        public string DriverDetails { get; set; }
        public string DriverContacts { get; set; }
        public string RecipientDetails { get; set; }
        public string RecipientContacts { get; set; }
        public string Comments { get; set; }
    }
}
