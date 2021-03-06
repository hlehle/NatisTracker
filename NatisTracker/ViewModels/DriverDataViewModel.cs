﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NatisTracker.ViewModels
{
    public class TickBoxViewModel
    {
        public string SenderName { get; set; }
        public DateTime SentDate { get; set; }
        public int ItemQuantity { get; set; }
        public string RecipientName { get; set; }
        public string RecipientType { get; set; }
        public string RecipientDepartment { get; set; }
        public string DriverContacts { get; set; }
        public int TableId { get; set; }
        public string Comment { get; set; }
        public Nullable<bool> IsConfirmed { get; set; }
        public string Reply { get; set; }
        public List<ContractNumbersViewModel> ContractsList { get; set; }
    }

    public class TickBoxViewModelList
    {
        public int PackageId { get; set; }
        [Required, Range(typeof(bool), "true", "true", ErrorMessage = "The field Is Confirmed must be checked.")]
        public bool IsConfirmed { get; set; }
        public List<TickBoxViewModel> TickBoxList { get; set; }
    }
}