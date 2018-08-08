using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using EnatisRepository.Repo;

namespace EscalationEmailService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
#if (DEBUG)
            Console.WriteLine("Escalation Emails Service");
            EscalationEmail Test = new EscalationEmail();
            Test.StartDebug();
            //Test.StopDebug();
            Console.ReadKey();
#else
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new EscalationEmail()
            };
            ServiceBase.Run(ServicesToRun);
#endif
        }
    }
}
