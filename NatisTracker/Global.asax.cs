using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace NatisTracker
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            setLicenses();
        }

        public void setLicenses()
        {
            var license = new Aspose.BarCode.License();
            license.SetLicense("Aspose.Total.lic");
        }
    }
}
