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
    
    public partial class ContractNumber
    {
        public int ID { get; set; }
        public Nullable<int> RecordNumber { get; set; }
        public string ContractNumber1 { get; set; }
        public Nullable<bool> IsReceived { get; set; }
        public Nullable<int> TableId { get; set; }
    
        public virtual SentIN_Delivery SentIN_Delivery { get; set; }
        public virtual TickBoxData TickBoxData { get; set; }
    }
}
