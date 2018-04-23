using NatisTracker.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NatisTracker.Models
{
    public interface INatisRequest
    {
        void request(NatisRequests viewModel, string name, string surname, string department);

        void respond(FormCollection form, string name, string surname);
    }
}