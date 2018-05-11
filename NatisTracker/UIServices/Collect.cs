using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NatisTracker.Models;
using NatisTracker.ViewModels;
using System.IO;
using Aspose.BarCode.BarCodeRecognition;
using Oracle.DataAccess.Client;
using System.Data;

namespace NatisTracker.Models
{
    public class Collect : IScanNatis
    {
        public bool Scan(NatisAndContractViewModel viewModel, string name, string department)
        {
            using (Intern_LeaveDBEntities db = new Intern_LeaveDBEntities())
            {

                if (viewModel.file != null)
                {
                    byte[] byteArr = new byte[viewModel.file.ContentLength];
                    viewModel.file.InputStream.Read(byteArr, 0, viewModel.file.ContentLength);
                    Stream stream = new MemoryStream(byteArr);
                    string[] natis = readBarCode(stream);

                    string contractNumber = getContractNo(natis[9]);

                    ScanLogsData log = new ScanLogsData();
                    NatisData data = new NatisData();

                    string[] contactInfo = GetContractStatus(contractNumber);

                    log.ContractNumber = contractNumber;
                    log.VinNumber = natis[9];
                    log.DateScanned = DateTime.Now;
                    log.User = name;
                    log.Department = department;
                    log.ContractStatus = contactInfo[0];
                    log.ContractDescription = contactInfo[1];
                    log.Comment = viewModel.Comment;

                    data = db.NatisDatas.FirstOrDefault(u => u.VinNumber == log.VinNumber);
                    data.NatisLocation = department;

                    db.ScanLogsDatas.Add(log);
                    db.SaveChanges();

                    viewModel.contractNo = log.ContractNumber;
                    viewModel.vin = log.VinNumber;
                    viewModel.DateScanned = log.DateScanned;
                    viewModel.ScanningUser = name;
                    viewModel.Department = log.Department;
                    viewModel.ContractStatus = log.ContractStatus;
                    viewModel.StatusDescription = log.ContractDescription;
                    viewModel.Comment = log.Comment;

                }
            }
            return true;
        }

        public string[] readBarCode(Stream barcodeLocation)
        {
            try
            {
                BarCodeReader reader = new BarCodeReader(barcodeLocation, DecodeType.Pdf417);

                string[] codeText = null;
                while (reader.Read())
                {
                    string str = reader.GetCodeText();
                    str = reader.GetCodeText().Remove(str.Length - 1).Remove(0, 1);
                    codeText = str.Split('%');
                }
                reader.Close();
                return codeText;
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

                string query = "select a.application_no from prop.application a " +
                                                       "join prop.app_form af on a.current_form_id = af.id and af.asset_chassis_no = :VIN " +
                                                       " where rownum = 1 order by af.created_ts desc";

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

        public string[] GetContractStatus(string contractNumber)
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
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}