﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NatisTracker.ViewModels;
using System.Web.Mvc;

namespace NatisTracker.Models
{
    public interface IDeliveryService
    {
        DeliveryitemViewModel sendDelivery(DeliveryitemViewModel view, string name);
        DeliveryViewModel receiveDelivery(DeliveryViewModel view, string name, string email);

        void SendToDriver(DriverPackage viewModel, string name);
    }
}