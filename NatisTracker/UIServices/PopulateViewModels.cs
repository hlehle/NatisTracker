using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EnatisRepository.Repo;
using NatisTracker.ViewModels;
using NatisTracker.Models;

namespace NatisTracker.UIServices
{
    public class PopulateViewModels : IPopulateViewModels
    {
        public List<EmployeeDataViewModel> PopulateEmployees(Intern_LeaveDBEntities db)
        {
            var viewModel = new List<EmployeeDataViewModel>();

            foreach (var item in db.EmployeeDatas)
            {
                var emp = new EmployeeDataViewModel();
                emp.RecordNumber = item.RecordNumber;
                emp.UserId = item.UserId;
                emp.UserName = item.UserName;
                emp.Employee_Surname = item.Employee_Surname;
                emp.ContactName = item.ContactName;
                emp.Password = item.Password;
                emp.Department = item.Department;
                emp.User_Type = item.User_Type;
                emp.Email = item.Email;
                emp.Title = item.Title;
                emp.DCXKIM = item.DCXKIM;
                emp.DCXKIM = item.DCXKIM;
                emp.LocationCode = item.LocationCode;
                emp.IsChangePassword = (bool)item.IsChangePassword;

                viewModel.Add(emp);
            } 

            return viewModel;
        }
        public List<NatisDataViewModel> PopulateNatisData(Intern_LeaveDBEntities db)
        {
            var viewModel = new List<NatisDataViewModel>();

            foreach (var item in db.NatisDatas)
            {
                var natis = new NatisDataViewModel();
                natis.RecordNumber = item.RecordNumber;
                natis.ScanningUser = item.User;
                natis.DateScanned = item.DateLoaded;
                natis.vin = item.VinNumber;
                natis.registrationNo = item.RegistrationNumber;
                natis.engineNo = item.EngineNumber;
                natis.carMake = item.CarMake;
                natis.seriesNo = item.SeriesNumber;
                natis.description = item.Description;
                natis.registrationDate = item.RegistrationDate;
                natis.VehicleStatus = item.VehicleStatus;
                natis.OwnerID = item.OwnerIdentityNumber;
                natis.OwnerName = item.OwnerName;
                natis.natisLocation = item.NatisLocation;

                natis.contractInformation = new List<ContractData>();
                foreach (var contract in db.ContractsDatas)
                {
                    var con = new ContractData();
                    con.RecordNumber = contract.RecordNumber;
                    con.ContractNumber = contract.ContractNumber;
                    con.ContractStatus = contract.ContractStatus;
                    con.StatusDescription = contract.StatusDescription;
                    con.VinNumber = contract.VinNumber;

                    natis.contractInformation.Add(con);
                }

                viewModel.Add(natis);
            }

            return viewModel;
        }
        public List<ScanLogsDataViewModel> PopulateScanLogs(Intern_LeaveDBEntities db)
        {
            var viewModel = new List<ScanLogsDataViewModel>();

            foreach (var item in db.ScanLogsDatas)
            {
                var log = new ScanLogsDataViewModel();

                log.RecordNumber = item.RecordNumber;
                log.ContractNumber = item.ContractNumber;
                log.VinNumber = item.VinNumber;
                log.DateScanned = item.DateScanned;
                log.User = item.User;
                log.Department = item.Department;
                log.ContractStatus = item.ContractStatus;
                log.ContractDescription = item.ContractDescription;
                log.Comment = item.Comment;

                viewModel.Add(log);
            }

            return viewModel;
        }
        public List<RequestsDataViewModel> PopulateRequestsData(Intern_LeaveDBEntities db)
        {
            var viewModel = new List<RequestsDataViewModel>();

            foreach (var item in db.RequestsDatas)
            {
                var req = new RequestsDataViewModel();

                req.RecordNumber = item.RecordNumber;
                req.RequesterName = item.RequesterName;
                req.RequesterDepartment = item.RequesterDepartment;
                req.RequestStatus = item.RequestStatus;
                req.RequestDate = item.RequestDate;
                req.Vin = item.Vin;
                req.ContractNo = item.ContractNo;
                if (item.ReplyDate != null)
                {
                    req.ReplyDate = (DateTime)item.ReplyDate;
                }
                                
                req.Responder = item.Responder;

                viewModel.Add(req);
            }

            return viewModel;
        }
        public List<SentIN_DeliveryViewModel> PopulateSentIN_Deliveries(Intern_LeaveDBEntities db)
        {
            var viewModel = new List<SentIN_DeliveryViewModel>();

            foreach (var item in db.SentIN_Delivery)
            {
                var delivery = new SentIN_DeliveryViewModel();
                delivery.RecordNumber = item.RecordNumber;
                delivery.DealershipName = item.DealershipName;
                delivery.PackageNumber = item.PackageNumber;
                delivery.PackageSender = item.PackageSender;
                delivery.PackageRecipient = item.PackageRecipient;
                delivery.DateSent = item.DateSent;
                if (item.DateReceived.HasValue)
                {
                    delivery.DateReceived = (DateTime)item.DateReceived;
                }
                
                delivery.CourierStatus = item.CourierStatus;
                delivery.Comment = item.Comment;
                delivery.Quantity = (int)item.Quantity;
                delivery.DriverDetails = item.DriverDetails;
                delivery.DriverContact = item.DriverContact;
                delivery.DeliveryChoice = item.DeliveryChoice;
                delivery.SenderEmail = item.SenderEmail;

                delivery.ContractsList = new List<ContractNumbersViewModel>();
                foreach (var contractItem in db.ContractNumbers.Where(a => a.RecordNumber.HasValue))
                {
                    var con = new ContractNumbersViewModel();
                    con.ID = contractItem.ID;
                    con.RecordNumber = (int)contractItem.RecordNumber;
                    con.ContractNumber1 = contractItem.ContractNumber1;
                    con.IsReceived = (bool)contractItem.IsReceived;
                    con.DriverId = -1;

                    delivery.ContractsList.Add(con);
                }

                viewModel.Add(delivery);
            }

            return viewModel;
        }
        public List<SentOUT_DeliveryViewModel> PopulateSentOUT_Deliveries(Intern_LeaveDBEntities db)
        {
            var viewModel = new List<SentOUT_DeliveryViewModel>();

            foreach (var item in db.SentOUT_Delivery)
            {

            }

            return viewModel;
        }
        public List<ContractNumbersViewModel> PopulateContractNumber(Intern_LeaveDBEntities db)
        {
            var viewModel = new List<ContractNumbersViewModel>();
            return viewModel;
        }
        public TickBoxViewModelList PopulateTickBoxData(Intern_LeaveDBEntities db)
        {
            var viewModel = new TickBoxViewModelList();
            viewModel.TickBoxList = new List<TickBoxViewModel>();

            foreach (var item in db.TickBoxDatas)
            {
                var driver = new TickBoxViewModel();

                driver.SenderName = item.SenderName;
                driver.SentDate = item.SentDate;
                driver.ItemQuantity = item.ItemQuantity;
                driver.RecipientName = item.RecipientName;
                driver.RecipientType = item.RecipientType;
                driver.RecipientDepartment = item.RecipientDepartment;
                driver.TableId = item.TableId;

                if (item.IsConfirmed != null)
                {
                    driver.IsConfirmed = (bool)item.IsConfirmed;
                }
                else
                {
                    driver.IsConfirmed = false;
                }
                
                driver.ContractsList = new List<ContractNumbersViewModel>();
                foreach (var contractItem in item.ContractNumbers)
                {
                    var con = new ContractNumbersViewModel();
                    con.ID = contractItem.ID;
                    con.RecordNumber = -1;
                    con.ContractNumber1 = contractItem.ContractNumber1;
                    if (contractItem.IsReceived != null)
                    {
                        con.IsReceived = (bool)contractItem.IsReceived;
                    }
                    else
                    {
                        con.IsReceived = false;
                    }
                    
                    con.DriverId = (int)contractItem.TableId;

                    driver.ContractsList.Add(con);
                }

                if (!(bool)driver.IsConfirmed)
                {
                    viewModel.TickBoxList.Add(driver);
                }

        }

            return viewModel;
        }
    }
}