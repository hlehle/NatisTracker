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
        public bool Scan(MasterViewModel appForm, string name, string surname, string department)
        {
            using (Intern_LeaveDBEntities db = new Intern_LeaveDBEntities())
            {
                if (appForm.natis.file != null)
                {
                    byte[] byteArr = new byte[appForm.natis.file.ContentLength];
                    appForm.natis.file.InputStream.Read(byteArr, 0, appForm.natis.file.ContentLength);
                    Stream stream = new MemoryStream(byteArr);
                    string[] natis = readBarCode(stream);

                    if (natis != null)
                    {
                        NatisData database = new NatisData();

                        database.ContractNumber = getContractNo(natis[9]);
                        database.User = name;
                        database.DateLoaded = DateTime.Now;
                        database.VinNumber = natis[9];
                        database.RegistrationNumber = natis[5];
                        database.EngineNumber = natis[10];
                        database.CarMake = natis[7];
                        database.SeriesNumber = getDescription(getContractNo(natis[9]));
                        database.Description = natis[6];
                        database.RegistrationDate = Convert.ToDateTime(natis[12]);
                        database.VehicleStatus = natis[11];
                        database.OwnerName = natis[15];
                        database.OwnerIdentityNumber = natis[14];
                        database.NatisLocation = "Safe Vault";

                        if (!isExist(db, database))
                        {
                            LogInfo log = new LogInfo();
                            log.ContractNumber = getContractNo(natis[9]);
                            log.VinNumber = natis[9];
                            log.DateScanned = DateTime.Now;
                            log.User = name + " " + surname;
                            log.Department = "Safe Vault";
                            log.ContractStatus = "800";
                            log.Comment = "First time loaded to the Safe";

                            db.NatisDatas.Add(database);
                            db.LogInfoes.Add(log);
                            db.SaveChanges();

                            appForm.natis.contractNo = database.ContractNumber;
                            appForm.natis.vin = database.VinNumber;
                            appForm.natis.registrationNo = database.RegistrationNumber;
                            appForm.natis.engineNo = database.EngineNumber;
                            appForm.natis.carMake = database.CarMake;
                            appForm.natis.seriesNo = database.SeriesNumber;
                            appForm.natis.description = database.Description;
                            appForm.natis.registrationDate = database.RegistrationDate;
                            appForm.natis.OwnerName = database.OwnerName;
                            appForm.natis.OwnerID = database.OwnerIdentityNumber;
                            appForm.natis.natisLocation = database.NatisLocation;

                            return true;
                        }


                    }
                }

            }
            return false;
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
                if (item.VinNumber.Equals(data.VinNumber))
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
            catch (Exception)
            {
                return null;
            }
        }
    }
}