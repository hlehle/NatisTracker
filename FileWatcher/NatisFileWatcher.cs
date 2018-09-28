using Aspose.BarCode.BarCodeRecognition;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using EnatisRepository.BarcodeReader;
using EnatisRepository.Repo;
using EnatisRepository.OracleDataRetrieval;
using Oracle.DataAccess.Client;
using EnatisRepository.HubbleService;
using System.Configuration;
using Aspose.Pdf;
using Aspose.Pdf.Facades;

namespace FileWatcher
{
    public partial class NatisFileWatcher : ServiceBase
    {
        public NatisFileWatcher()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            string path = ConfigurationManager.AppSettings["WatchedFolder"];
            MonitorDirectory(path);
        }

        private static void MonitorDirectory(string path)
        {
            FileSystemWatcher fileSystemWatcher = new FileSystemWatcher();
            fileSystemWatcher.Path = path;
            fileSystemWatcher.Created += FileSystemWatcher_Created;
            //fileSystemWatcher.Renamed += FileSystemWatcher_Renamed;
            //fileSystemWatcher.Deleted += FileSystemWatcher_Deleted;

            fileSystemWatcher.NotifyFilter = NotifyFilters.LastAccess |
                                             NotifyFilters.LastWrite |
                                             NotifyFilters.FileName |
                                             NotifyFilters.DirectoryName;

            fileSystemWatcher.Filter = "*.pdf";
            fileSystemWatcher.EnableRaisingEvents = true;
            fileSystemWatcher.IncludeSubdirectories = true;
        }

        private static void FileSystemWatcher_Created(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine("File is created");
            using (var db = new Intern_LeaveDBEntities())
            {
                try
                {
                    var pdfDocument = new Document(e.FullPath);

                    if (pdfDocument.Pages.Count == 1)
                    {
                        FileStream fileStream = new FileStream(pdfDocument.FileName, FileMode.Open);
                        DecryptBarcode(fileStream);
                        fileStream.Close();
                    }

                    else
                    {
                        for (int pageNr = 1; pageNr <= pdfDocument.Pages.Count; pageNr++)
                        {
                            using (var pageDocument = new Document())
                            {
                                pageDocument.Pages.Add(pdfDocument.Pages[pageNr]);
                                pageDocument.Save("new" + pageNr + ".pdf", SaveFormat.Pdf);

                                FileStream fileStream = new FileStream(Environment.CurrentDirectory + @"\new" + pageNr + ".pdf", FileMode.Open);
                                DecryptBarcode(fileStream);
                                fileStream.Close();
                                
                            }
                        }
                        string[] t = Directory.GetFiles(Environment.CurrentDirectory, "*.pdf");
                        Array.ForEach(t, File.Delete);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: "+ ex.Message);
                    return;
                }
            }

        }

        public static void DecryptBarcode(FileStream fileStream)
        {
            using (Intern_LeaveDBEntities db = new Intern_LeaveDBEntities())
            {
                BarcodeReader reader = new BarcodeReader();
                string[] natis = reader.readBarCode(fileStream);

                if (natis == null) // if No barcode, do nothing 
                {}

                else // (natis != null)
                {
                    // Filling Natis Doc Data
                    NatisData natisData = new NatisData();
                    var conn = new OracleDataConnections();

                    natisData.User = "Sihle Mdlalose";
                    natisData.DateLoaded = DateTime.Now;
                    natisData.VinNumber = natis[9];
                    string contractNumber = conn.getContractNo(natisData.VinNumber);

                    if (contractNumber == null)
                    {return;}

                    natisData.RegistrationNumber = natis[5];
                    natisData.EngineNumber = natis[10];
                    natisData.CarMake = natis[7];
                    natisData.SeriesNumber = conn.getDescription(contractNumber);
                    natisData.Description = natis[6];
                    natisData.RegistrationDate = Convert.ToDateTime(natis[12]);
                    natisData.VehicleStatus = natis[11];
                    natisData.OwnerName = natis[15];
                    natisData.OwnerIdentityNumber = natis[14];
                    natisData.NatisLocation = "Safe Vault";

                    using (MemoryStream ms = new MemoryStream())
                    {
                        fileStream.CopyTo(ms);
                        natisData.eNatisPDF = ms.ToArray();

                        // Filling Contract Data

                        string contractInfo = conn.GetContractStatus(contractNumber);
                        ContractsData contractData = new ContractsData();
                        contractData.RecordNumber = natisData.RecordNumber;
                        contractData.VinNumber = natisData.VinNumber;
                        contractData.ContractNumber = contractNumber;
                        contractData.ContractStatus = contractInfo;
                        //contractData.StatusDescription = contractInfo[1];

                        ScanLogsData log = new ScanLogsData();

                        log.ContractNumber = contractNumber;
                        log.VinNumber = natisData.VinNumber;
                        log.DateScanned = DateTime.Now;
                        log.User = natisData.User;
                        log.Department = natisData.NatisLocation;
                        log.ContractStatus = contractData.ContractStatus;
                        log.ContractDescription = contractData.StatusDescription;

                        if (!isExist(db, natisData))
                        {
                            log.Comment = "First time loaded to the Safe";

                            db.NatisDatas.Add(natisData);
                            db.ScanLogsDatas.Add(log);
                            db.ContractsDatas.Add(contractData);
                            db.SaveChanges();

                            //Saving To Hubble
                            DocumentUploadClient upload = new DocumentUploadClient();
                            var results = upload.UploadDocument(38, ms.ToArray(), contractNumber + ".pdf", contractNumber, natisData.VinNumber, natisData.RegistrationNumber, "eNatis Documents", contractNumber, 1);
                            
                        }


                        else if (isExist(db, natisData) && !isInSafe(db, natisData))
                        {
                            log.Comment = "Back to Safe";

                            var vin = natisData.VinNumber;
                            var temp = db.NatisDatas.Where(a => a.VinNumber == vin).FirstOrDefault();
                            temp.NatisLocation = natisData.NatisLocation;
                            //db.NatisDatas.Add(natisData);
                            db.ScanLogsDatas.Add(log);
                            db.ContractsDatas.Add(contractData);
                            db.SaveChanges();

                            //return true;
                        }

                        else if (isExist(db, natisData) && isInSafe(db, natisData))
                        {
                            //return false;
                        }
                    }
                }
            }
        }

        public static bool isInSafe(Intern_LeaveDBEntities db, NatisData data)
        {
            foreach (var item in db.NatisDatas)
            {
                if (item.VinNumber.Equals(data.VinNumber) && item.NatisLocation.Equals(data.NatisLocation))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool isExist(Intern_LeaveDBEntities db, NatisData data)
        {
            foreach (var item in db.NatisDatas)
            {
                if (item.VinNumber.Equals(data.VinNumber))
                {
                    return true;
                }
            }
            return false;
        }

        public void StartDebug()
        {
            OnStart(null);
        }

        protected override void OnStop()
        {
        }
    }
}
