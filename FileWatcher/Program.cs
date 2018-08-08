using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace FileWatcher
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
#if (DEBUG)
            Console.WriteLine("FileWatcher Service");
            NatisFileWatcher Watcher = new NatisFileWatcher();
            Watcher.StartDebug();
            ////Test.StopDebug();
            Console.ReadKey();
#else
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new FileWatcher()
            };
            ServiceBase.Run(ServicesToRun);
#endif
        }
    }
}
