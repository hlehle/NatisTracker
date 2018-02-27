using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NatisTracker.ViewModels;
using System.IO;
using NatisTracker.Models;
using Aspose.BarCode.BarCodeRecognition;
using Oracle.DataAccess.Client;
using System.Data;

namespace NatisTracker.UIServices
{
    public class NatisDataPopulate
    {
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

        public bool populate(NatisDataViewModel appForm)
        {
            using (Intern_LeaveDBEntities db = new Intern_LeaveDBEntities())
            {
                if (appForm.file != null)
                {
                    byte[] byteArr = new byte[appForm.file.ContentLength];
                    appForm.file.InputStream.Read(byteArr, 0, appForm.file.ContentLength);
                    Stream stream = new MemoryStream(byteArr);
                    string[] natis = readBarCode(stream);

                    if (natis != null)
                    {
                        NatisData database = new NatisData();

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
                            db.NatisDatas.Add(database);
                            db.SaveChanges();

                            appForm.vin = database.VinNumber;
                            appForm.registrationNo = database.RegistrationNumber;
                            appForm.engineNo = database.EngineNumber;
                            appForm.carMake = database.CarMake;
                            appForm.seriesNo = database.SeriesNumber;
                            appForm.description = database.Description;
                            appForm.registrationDate = database.RegistrationDate;
                            appForm.OwnerName = database.OwnerName;
                            appForm.OwnerID = database.OwnerIdentityNumber;
                            appForm.natisLocation = database.NatisLocation;

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
                if (item.Equals(data))
                {
                    return true;
                }
            }
            return false;
        }
    }
}