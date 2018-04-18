using NatisTracker.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NatisTracker.ViewModels
{
    public class DeliveryViewModel
    {
        public DeliveryCourierViewModel CourierViewModel { get; set; }
        public DeliveryDriverViewModel DriverViewModel { get; set; }
 
    }
    public class DeliveryCourierViewModel
    {
        public IList<DeliveryitemViewModel> DeliveryItems { get; set; }
    }
    public class DeliveryDriverViewModel
    {
        public IList<DeliveryitemViewModel> DeliveryItems { get; set; }
    }
    public class DeliveryitemViewModel
    {
        public int RecordNumber { get; set; }
        [Required]
        public string DealershipName { get; set; }
        public string WaybillNumber { get; set; }
        public string PackageSender { get; set; }
        public string SenderEmail { get; set; }
        public string FName { get; set; }
        public string LName { get; set; }
        public string Recipient { get; set; }
        public string RecipientContacts { get; set; }
        public DateTime DateSent { get; set; }
        public DateTime DateRecieved { get; set; }
        public string DeliveryStatus { get; set; }
        [Required]
        public string DeliveryChoice { get; set; }
        [Required]
        public int Quantity { get; set; }
        public string DriverDetails { get; set; }
        public string DriverContacts { get; set; }
        public string Comment { get; set; }
        public string ContractStrings { get; set; }
        public IList<string> DealerList { get; set; }
        public IList<int> QuantityList { get; set; }
        public IList<DeliveryItemContractViewModel> ContractNumberItems { get; set; }
        
        //Dilivery Display view model is just to display the Deliveries from the db  
        public IList<SentIN_Delivery> CourierDeliveryDisplay { get; set; }

        public IList<SentIN_Delivery> DriverDeliveryDisplay { get; set; }
        //Contract Display view model is just to display the Deliveries from the db
        public IList<ContractNumber> ContractsDisplay { get; set; }
    }
    public class DeliveryItemContractViewModel
    {
        public int RecordNumber { get; set; }
        public string ContractNumber { get; set; }
        public bool IsRecieved { get; set; }
    }
}