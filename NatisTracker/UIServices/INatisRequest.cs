using NatisTracker.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NatisTracker.Requests
{
    public interface INatisRequest
    {
        NatisRequests request(NatisRequests viewModel, string name, string department, string email);

        void respond(FormCollection form, string name, string email);
    }
}