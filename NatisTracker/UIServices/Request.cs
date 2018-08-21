using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NatisTracker.ViewModels;
using EnatisRepository.Repo;
using System.Web.Mvc;
using Comet.Email;
using Oracle.DataAccess.Client;
using System.Data;

namespace NatisTracker.Requests
{
    public class Request : INatisRequest
    {
        public NatisRequests request(NatisRequests viewModel, string name, string department, string email)
        {
            using (Intern_LeaveDBEntities db = new Intern_LeaveDBEntities())
            {
                RequestsData database = new RequestsData();
                database.RequesterName = name;
                database.RequestDate = DateTime.Now;
                database.RequesterDepartment = department;
                database.RequestStatus = "Pending";
                database.ContractNo = viewModel.contractNo;
                database.Vin = getVin(database.ContractNo);

                var requestedContract = database.ContractNo;
                var con = db.ContractsDatas.Where(a => a.ContractNumber.Equals(requestedContract)).Select(a => a.ContractNumber);

                if (con != null)
                {
                    db.RequestsDatas.Add(database);
                    db.SaveChanges();

                    string to = email;
                    string subject = "Request Notification";
                    string body = "Your request for the Natis document of the contract number "+database.ContractNo+" has been received";
                    SystemEmailSender.SendMail(to, subject, body);

                    string[] toArray = db.EmployeeDatas.Where(a => a.User_Type == "Admin").Select(a => a.Email).ToArray();
                    string subject1 = "Request Notification";
                    string body1 = "A request for a Natis Document of contract number "+database.ContractNo+" has been Submitted";
                    SystemEmailSender.SendMail(toArray, subject1, body1);

                    return viewModel;
                }

                // Validation for wrong requests will go here
                else
                {
                    return null;
                }
                
            }
        }

        public void respond(FormCollection form, string name, string email) { }

        public string getVin(string contractNo)
        {
            try
            {
                //using connection string attributes to connect to Oracle Database
                string connectionString = "Data Source=GALAXI;User ID=enatis_user;Password=Galaxi2017";


                OracleConnection conn = new OracleConnection(connectionString);
                conn.Open();

                string query = "select af.asset_chassis_no from prop.application a " +
                                                           "join prop.app_form af on af.id = a.current_form_id " +
                                                           "where a.application_no = :contractNo";

                OracleCommand cmd = new OracleCommand(query, conn);
                cmd.Parameters.Add("contractNo", contractNo);
                cmd.CommandType = CommandType.Text;

                OracleDataReader dr = cmd.ExecuteReader();
                dr.Read();

                string vin = dr.GetString(0);
                // Close and Dispose OracleConnection object
                conn.Close();
                conn.Dispose();
                //Console.WriteLine("Disconnected");
                return vin;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}