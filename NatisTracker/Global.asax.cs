using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using NatisTracker.Models;
using System.Timers;
using System.Web.Configuration;

namespace NatisTracker
{
    public class MvcApplication : System.Web.HttpApplication
    {
        //private static double TimerIntervalInMilliseconds = Convert.ToDouble(WebConfigurationManager.AppSettings["TimerIntervalInMilliseconds"]);
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            SetLicenses();
            SendEscalation();

            //Timer timer = new Timer(TimerIntervalInMilliseconds);
            //timer.Enabled = true;
            //timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
            //timer.Start();
            
        }

        public void SetLicenses()
        {
            var license = new Aspose.BarCode.License();
            license.SetLicense("Aspose.Total.lic");
        }

        static void SendEscalation()
        {
            

        }

        //static void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        //{
        //    DateTime MyScheduledRunTime = DateTime.Parse(WebConfigurationManager.AppSettings["TimerStartTime"]);
        //    DateTime CurrentSystemTime = DateTime.Now;
        //    DateTime LatestRunTime = MyScheduledRunTime.AddMilliseconds(TimerIntervalInMilliseconds);
        //    if ((CurrentSystemTime.CompareTo(MyScheduledRunTime) >= 0) && (CurrentSystemTime.CompareTo(LatestRunTime) <= 0))
        //    {
        //        // RUN YOUR PROCESSES HERE
        //        SendEscalation();
        //    }
        //}
    }
}
