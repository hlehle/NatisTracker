using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NatisTracker.ViewModels;
using System.Web.Mvc;

namespace NatisTracker.Deliveries
{
    public interface IDeliveryService
    {
        DeliveryitemViewModel sendDelivery(DeliveryitemViewModel view, string name);
        DeliveryViewModel receiveDelivery(DeliveryViewModel view, string name, string email);

        void SendNatis(TickBoxViewModelList viewModel, string name, string department);
    }
}