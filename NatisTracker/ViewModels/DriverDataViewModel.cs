using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NatisTracker.ViewModels
{
    public class TickBoxViewModel
    {
        public string SenderName { get; set; }
        public DateTime SentDate { get; set; }
        public int ItemQuantity { get; set; }
        public string DriverName { get; set; }
        public string RecipientType { get; set; }
        public string DriverDepartment { get; set; }
        public string DriverContacts { get; set; }
        public int DriverId { get; set; }
        public Nullable<bool> IsConfirmed { get; set; }
        public string Reply { get; set; }
        public List<ContractNumbersViewModel> ContractsList { get; set; }
    }

    public class TickBoxViewModelList
    {
        public int PackageId { get; set; }
        public bool IsConfirmed { get; set; }
        public List<TickBoxViewModel> TickBoxList { get; set; }
    }
}