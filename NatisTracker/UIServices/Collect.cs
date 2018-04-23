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

                    string contractNumber = getContractNo(natis[9]);

                    ScanLogsData database = new ScanLogsData();
                    NatisData data = new NatisData();

                    database.ContractNumber = contractNumber;
                    database.VinNumber = natis[9];
                    database.DateScanned = DateTime.Now;
                    database.User = name + " " + surname;
                    database.Department = department;
                    database.ContractStatus = "800";
                    database.Comment = viewModel.Comment;

                    data = db.NatisDatas.FirstOrDefault(u => u.VinNumber == database.VinNumber);
                    data.NatisLocation = department;

                    db.ScanLogsDatas.Add(database);
                    db.SaveChanges();

                    viewModel.contractNo = database.ContractNumber;
                    viewModel.vin = database.VinNumber;
                    viewModel.DateScanned = database.DateScanned;
                    viewModel.ScanningUser = name + " " + surname;
                    viewModel.Department = database.Department;
                    viewModel.ContractStatus = database.ContractStatus;
                    viewModel.Comment = database.Comment;

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
    }
}