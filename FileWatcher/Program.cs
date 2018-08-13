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

            var BarCodeLicense = new Aspose.BarCode.License();
            BarCodeLicense.SetLicense("Aspose.Total.lic");
            var PdfLicence = new Aspose.Pdf.License();
            PdfLicence.SetLicense("Aspose.Total.lic");

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
