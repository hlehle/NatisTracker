using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NatisTracker.ViewModels;

namespace NatisTracker.Controllers
{
    public class OffPremiseTrackerController : Controller
    {
        // GET: OffPremiseTracker
        
        public ActionResult offByLegal()
        {
            return null;
        }

        [HttpPost]
        public ActionResult offByLegal(FormCollection form)
        {
            return null;
        }

        public ActionResult offByMaturities()
        {
            return null;
        }

        [HttpPost]
        public ActionResult offByMaturities(FormCollection form)
        {
            return null;
        }

        public ActionResult offByCallCentre()
        {
            return null;
        }

        [HttpPost]
        public ActionResult offByCallCentre(FormCollection form)
        {
            return null;
        }
    }
}