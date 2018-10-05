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
using log4net;
using Aspose.Pdf.Facades;

namespace FileWatcher
{
    public partial class NatisFileWatcher : ServiceBase
    {
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static Dictionary<string, int> stillPossiblyWriting = new Dictionary<string, int>();
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
            fileSystemWatcher.Changed += FileSystemWatcher_Changed;
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
            logger.Info("A new file has been created");
            try
            {
                ProcessFile(e.FullPath);
            }
            catch (System.IO.IOException)
            {
                DetermineAction(e.FullPath);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                //throw new Exception("SITHLE LOG THIS!!!", ex);
            }
        }

        private static void ProcessFile(string filePath)
        {
           
                using (FileStream fs = System.IO.File.OpenRead(filePath))
                {
                    using (Document originalPDFDocument = new Document(fs))
                    {
                        if (originalPDFDocument == null || originalPDFDocument.Pages.Count == 0) return;

                        for (int pageNr = 1; pageNr <= originalPDFDocument.Pages.Count; pageNr++)
                        {
                            string outputFileName = String.Format("{0}.pdf", Guid.NewGuid());

                            using (Document separateDocumentForPage = new Document())
                            {
                                separateDocumentForPage.Pages.Add(originalPDFDocument.Pages[pageNr]);
                                separateDocumentForPage.Save(outputFileName, SaveFormat.Pdf);
                            }
                            DecryptBarcode(outputFileName);
                        }
                        string[] t = Directory.GetFiles(Environment.CurrentDirectory, "*.pdf");
                        Array.ForEach(t, File.Delete);
                        logger.Info("End of Document");
                    }
                }
           
        }

        private static void FileSystemWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            if(stillPossiblyWriting.ContainsKey(e.FullPath))
            {
                try
                {
                    ProcessFile(e.FullPath);
                    stillPossiblyWriting.Remove(e.FullPath);
                }
                catch
                {
                    DetermineAction(e.FullPath);
                }
            }
        }

        private static void DetermineAction(string filePath)
        {
            if (stillPossiblyWriting.ContainsKey(filePath))
            {
                var maxAttempts = Convert.ToInt32(ConfigurationManager.AppSettings["FileOpenRetryMaxAttempts"]);
                if (stillPossiblyWriting[filePath] == maxAttempts)
                {
                    //throw new Exception(string.Format("Could not open file {0} after {1} attempts", filePath, maxAttempts));
                    logger.Error(string.Format("Could not open file {0} after {1} attempts", filePath, maxAttempts));
                }
                stillPossiblyWriting[filePath]++;
            }
            else
                stillPossiblyWriting.Add(filePath, 1);
        }

        public static void DecryptBarcode(string inputFileName)
        {
            string[] natis = null;
            byte[] pdfFile = null;
            using (FileStream fileStream = File.OpenRead(inputFileName))
            {

                BarcodeReader reader = new BarcodeReader();
                natis = reader.readBarCode(fileStream);

                using (MemoryStream ms = new MemoryStream())
                {
                    fileStream.CopyTo(ms);
                    fileStream.Flush();
                    pdfFile = ms.ToArray();
                }
            }
            //byte[] data = null;

            if (natis == null || natis.Length < 1) // if No barcode, do nothing 
            {
                logger.Info("No Barcode found in this page");
                //data = Encoding.Default.GetBytes("Log Error: No Barcode found in this page");
                //logfile.Write(data, 0, data.Length);
            }

            else // (natis != null)
            {
                var conn = new OracleDataConnections();

                // Filling Contract Data

                ContractsData contractData = new ContractsData();
                contractData.VinNumber = natis[9];
                string contractNumber = conn.getContractNo(contractData.VinNumber);

                if (contractNumber == null)
                {
                    logger.Info("The VIN " + contractData.VinNumber + " has no contract Number linked to it.");
                    //data = Encoding.Default.GetBytes("Log Error: The VIN "+ natisData.VinNumber+" has no contract Number linked to it.");
                    //logfile.Write(data, 0, data.Length);
                    return;
                }

                contractData.ContractNumber = contractNumber;
                contractData.ContractStatus = conn.GetContractStatus(contractNumber);


                // Filling Natis Doc Data
                NatisData natisData = new NatisData();

                natisData.ContractsDataID = contractData.RecordNumber;
                natisData.User = "Sihle Mdlalose";
                natisData.DateLoaded = DateTime.Now;
                natisData.VinNumber = contractData.VinNumber;
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
                natisData.eNatisPDF = pdfFile;

                ScanLogsData log = new ScanLogsData();

                log.ContractNumber = contractNumber;
                log.VinNumber = natisData.VinNumber;
                log.DateScanned = DateTime.Now;
                log.User = natisData.User;
                log.Department = natisData.NatisLocation;
                log.ContractStatus = contractData.ContractStatus;
                log.ContractDescription = contractData.StatusDescription;
                using (NatisTrackerDBEntities db = new NatisTrackerDBEntities())
                {
                    bool exists = IsExist(db, contractData);
                    bool isInSafe = IsInSafe(db, contractData);

                    if (!exists)
                    {
                        //Saving To Hubble
                        DocumentUploadClient upload = new DocumentUploadClient();
                        int collectionId = Convert.ToInt32(ConfigurationManager.AppSettings["CollectionId"]);
                        var results = upload.UploadDocument(collectionId,
                                                           pdfFile,
                                                            contractNumber + ".pdf",
                                                            contractNumber,
                                                            natisData.VinNumber,
                                                            natisData.RegistrationNumber,
                                                            "eNatis Documents",
                                                            contractNumber,
                                                            1);

                        if (results.Success)
                        {
                            // Saving to FinfMy Natis db
                            log.Comment = "First time loaded to the Safe";

                            db.ContractsDatas.Add(contractData);
                            db.NatisDatas.Add(natisData);
                            db.ScanLogsDatas.Add(log);
                            db.SaveChanges();

                            logger.Info(contractData.ContractNumber + " Successfully Saved");
                        }
                    }


                    else if (exists && !isInSafe)
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

                    else if (exists && isInSafe)
                    {
                        logger.Info("The Contract Number " + log.ContractNumber + " already exists.");
                    }
                }
            }
        }

        public static bool IsInSafe(NatisTrackerDBEntities db, ContractsData data)
        {
            var vinNo = data.VinNumber;
            return db.NatisDatas.Any(x => x.VinNumber == vinNo && x.NatisLocation == "Safe Vault");

        }

        public static bool IsExist(NatisTrackerDBEntities db, ContractsData data)
        {
            var contractNo = data.ContractNumber;
            return db.ContractsDatas.Any(x => x.ContractNumber == contractNo);
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
