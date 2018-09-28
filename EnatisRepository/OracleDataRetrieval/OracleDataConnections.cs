using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Oracle.DataAccess.Client;
using System.IO;
using EnatisRepository.Repo;
using Aspose.BarCode.BarCodeRecognition;
using Aspose.Pdf.Facades;
using System.Configuration;

namespace EnatisRepository.OracleDataRetrieval
{
    public class OracleDataConnections
    {
        public OracleDataConnections() { }
        public string getDescription(string contractNo)
        {
            try
            {
                //using connection string attributes to connect to Oracle Database
                string connectionString = ConfigurationManager.AppSettings["GALAXI_ConnectionString"];


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
        public string getContractNo(string vin)
        {
            try
            {
                //using connection string attributes to connect to Oracle Database
                string connectionString = ConfigurationManager.AppSettings["GALAXI_ConnectionString"];


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
        public string GetContractStatus(string contractNumber)
        {
            try
            {
                //using connection string attributes to connect to Oracle Database
                string connectionString = ConfigurationManager.AppSettings["WEBDB_ConnectionString"];

                OracleConnection conn = new OracleConnection(connectionString);
                conn.Open();

                string query = "select t.sub_status_code from ascent.contract_detail t " +
                               "where t.contract_number = :contractNumber " +
                               "order by t.version_number desc";

                OracleCommand cmd = new OracleCommand(query, conn);
                cmd.Parameters.Add("contractNumber", "0000000000" + contractNumber);
                cmd.CommandType = CommandType.Text;

                string contractInfo = null;
                OracleDataReader results = cmd.ExecuteReader();

                if (results.HasRows)
                {
                    results.Read();
                    contractInfo = results.GetValue(0).ToString();
                    
                    return contractInfo;
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
