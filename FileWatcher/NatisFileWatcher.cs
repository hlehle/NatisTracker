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
            fileSystemWatcher.EnableRaisingEvents = true;
        }

        private static void FileSystemWatcher_Created(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine("File is created");
            using (Intern_LeaveDBEntities db = new Intern_LeaveDBEntities())
            {
                var pdfDocument = new Document(e.FullPath);

                try
                {
                    if (pdfDocument.Pages.Count == 1)
                    {
                        FileStream fileStream = new FileStream(pdfDocument.FileName, FileMode.Open);
                        DecryptBarcode(fileStream);
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
                                string[] t = Directory.GetFiles(Environment.CurrentDirectory, "*.pdf");
                                Array.ForEach(t, File.Delete);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                    throw;
                }
                
                
            }

        }

        public static void DecryptBarcode(FileStream fileStream)
        {
            using (Intern_LeaveDBEntities db = new Intern_LeaveDBEntities())
            {
                BarcodeReader reader = new BarcodeReader();
                string[] natis = reader.readBarCode(fileStream);

                
            }

        }

        public static string getDescription(string contractNo)
        {
            try
            {
                //using connection string attributes to connect to Oracle Database
                string connectionString = "Data Source=GALAXI;User ID=enatis_user;Password=Galaxi2017";


                OracleConnection conn = new OracleConnection(connectionString);
                conn.Open();

                string query = "select af.asset_description from prop.application a " +
                                                           "join prop.app_form af on af.id = a.current_form_id " +
                                                           "where a.application_no = :contractNo";

                OracleCommand cmd = new OracleCommand(query, conn);
                cmd.Parameters.Add("contractNo", contractNo);
                cmd.CommandType = CommandType.Text;

                OracleDataReader dr = cmd.ExecuteReader();
                dr.Read();

                string description = dr.GetString(0);
                // Close and Dispose OracleConnection object
                conn.Close();
                conn.Dispose();
                //Console.WriteLine("Disconnected");
                return description;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string getContractNo(string vin)
        {
            try
            {
                //using connection string attributes to connect to Oracle Database
                string connectionString = "Data Source=GALAXI;User ID=enatis_user;Password=Galaxi2017";


                OracleConnection conn = new OracleConnection(connectionString);
                conn.Open();

                string query = "select max(a.application_no) from prop.application a " +
                                                       "join prop.app_form af on a.current_form_id = af.id and af.asset_chassis_no = :VIN ";

                OracleCommand cmd = new OracleCommand(query, conn);
                cmd.Parameters.Add("VIN", vin);
                cmd.CommandType = CommandType.Text;

                OracleDataReader dr = cmd.ExecuteReader();
                dr.Read();

                string contractNumber = dr.GetString(0);
                // Close and Dispose OracleConnection object
                conn.Close();
                conn.Dispose();
                //Console.WriteLine("Disconnected");
                return contractNumber;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string[] GetContractStatus(string contractNumber)
        {
            try
            {
                //using connection string attributes to connect to Oracle Database
                string connectionString = "Data Source=WEBDBQA;User ID=enatis_user;Password=welcome1";


                OracleConnection conn = new OracleConnection(connectionString);
                conn.Open();

                string query = "select ssd.sub_status_code, ssd.sub_status_description " +
                               "from PMS.CM_CONTRACT_DETAIL_MV t " +
                               "inner join PMS.CS_STATUS_DESC sd " +
                               "on t.status_code = sd.status_code " +
                               "and sd.language_code = 'E' " +
                               "inner join PMS.CS_SUB_STAT_DESC ssd " +
                               "on t.sub_status_code = ssd.sub_status_code " +
                               "and ssd.language_code = 'E' " +
                               "where CONTRACT_NUMBER = :contractNumber " +
                               "order by t.version_number desc";

                OracleCommand cmd = new OracleCommand(query, conn);
                cmd.Parameters.Add("contractNumber", "0000000000" + contractNumber);
                cmd.CommandType = CommandType.Text;

                string[] contractInfo = new string[2];
                OracleDataReader results = cmd.ExecuteReader();
                if (results.HasRows)
                {
                    results.Read();
                    contractInfo[0] = results.GetValue(0).ToString();
                    contractInfo[1] = results.GetValue(1).ToString();
                    return contractInfo;
                }
                else
                {
                    contractInfo[0] = "";
                    contractInfo[1] = "";

                }

                // Close and Dispose OracleConnection object
                conn.Close();
                conn.Dispose();
                //Console.WriteLine("Disconnected");
                return contractInfo;
            }
            catch (Exception)
            {
                return null;
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
