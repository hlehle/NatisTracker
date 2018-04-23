using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NatisTracker.Models;
using NatisTracker.ViewModels;
using System.IO;
using Oracle.DataAccess.Client;
using System.Data;
using Aspose.BarCode.BarCodeRecognition;

namespace NatisTracker.Models
{
    public class LoadNew : IScanNatis
    {
        public bool Scan(NatisAndContractViewModel viewModel, string name, string surname, string department)
        {
            using (Intern_LeaveDBEntities db = new Intern_LeaveDBEntities())
            {
                if (viewModel.file != null)
                {
                    byte[] byteArr = new byte[viewModel.file.ContentLength];
                    viewModel.file.InputStream.Read(byteArr, 0, viewModel.file.ContentLength);
                    Stream stream = new MemoryStream(byteArr);
                    string[] natis = readBarCode(stream);

                    if (natis != null)
                    {
                        // Filling Natis Doc Data
                        NatisData natisData = new NatisData();

                        natisData.User = name + " " + surname;
                        natisData.DateLoaded = DateTime.Now;
                        natisData.VinNumber = natis[9];
                        natisData.RegistrationNumber = natis[5];
                        natisData.EngineNumber = natis[10];
                        natisData.CarMake = natis[7];
                        natisData.SeriesNumber = getDescription(getContractNo(natis[9]));
                        natisData.Description = natis[6];
                        natisData.RegistrationDate = Convert.ToDateTime(natis[12]);
                        natisData.VehicleStatus = natis[11];
                        natisData.OwnerName = natis[15];
                        natisData.OwnerIdentityNumber = natis[14];
                        natisData.NatisLocation = "Safe Vault";

                        // Filling Contract Data
                        string contractNumber = getContractNo(natisData.VinNumber);
                        string[] contractInfo = GetContractStatus(contractNumber); 
                        ContractsData contractData = new ContractsData();
                        //contractData.RecordNumber = natisData.RecordNumber;
                        contractData.VinNumber = natisData.VinNumber;
                        contractData.ContractNumber = contractNumber;
                        contractData.ContractStatus = contractInfo[0];
                        contractData.StatusDescription = contractInfo[1];
                        
                        if (!isExist(db, natisData))
                        {
                            ScanLogsData log = new ScanLogsData();

                            log.ContractNumber = getContractNo(natis[9]);
                            log.VinNumber = natisData.VinNumber;
                            log.DateScanned = DateTime.Now;
                            log.User = natisData.User;
                            log.Department = natisData.NatisLocation;
                            log.ContractStatus = contractData.ContractStatus;
                            log.ContractDescription = contractData.StatusDescription;
                            log.Comment = "First time loaded to the Safe";

                            db.NatisDatas.Add(natisData);
                            db.ScanLogsDatas.Add(log);
                            db.ContractsDatas.Add(contractData);
                            db.SaveChanges();

                            viewModel.contractNo = contractNumber;
                            viewModel.vin = natisData.VinNumber;
                            viewModel.registrationNo = natisData.RegistrationNumber;
                            viewModel.engineNo = natisData.EngineNumber;
                            viewModel.carMake = natisData.CarMake;
                            viewModel.seriesNo = natisData.SeriesNumber;
                            viewModel.description = natisData.Description;
                            viewModel.registrationDate = natisData.RegistrationDate;
                            viewModel.OwnerName = natisData.OwnerName;
                            viewModel.OwnerID = natisData.OwnerIdentityNumber;
                            viewModel.natisLocation = natisData.NatisLocation;

                            return true;
                        }
                    }
                }
                return false;
            }
        }

        public string getDescription(string contractNo)
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
            catch (Exception ex)
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

        public bool isExist(Intern_LeaveDBEntities db, NatisData data)
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
            catch (Exception ex)
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
                               "from PMS.CM_CONTRACT_DETAIL_MV t "+
                               "inner join PMS.CS_STATUS_DESC sd "+
                               "on t.status_code = sd.status_code "+
                               "and sd.language_code = 'E' "+
                               "inner join PMS.CS_SUB_STAT_DESC ssd "+
                               "on t.sub_status_code = ssd.sub_status_code "+
                               "and ssd.language_code = 'E' "+
                               "where CONTRACT_NUMBER = :contractNumber "+
                               "order by t.version_number desc";

                OracleCommand cmd = new OracleCommand(query, conn);
                cmd.Parameters.Add("contractNumber", "0000000000"+contractNumber);
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