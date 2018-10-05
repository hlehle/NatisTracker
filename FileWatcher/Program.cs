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
            SetLicenses();



#if (DEBUG)
            Console.WriteLine("FileWatcher Service Window");
            NatisFileWatcher Watcher = new NatisFileWatcher();
            Watcher.StartDebug();
            ////Test.StopDebug();
            Console.ReadKey();
#else
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new NatisFileWatcher()
            };
            ServiceBase.Run(ServicesToRun);
#endif
        }

        static void SetLicenses()
        {
            var BarCodeLicense = new Aspose.BarCode.License();
            BarCodeLicense.SetLicense("Aspose.Total.lic");
            var PdfLicence = new Aspose.Pdf.License();
            PdfLicence.SetLicense("Aspose.Total.lic");
        }
    }
}
