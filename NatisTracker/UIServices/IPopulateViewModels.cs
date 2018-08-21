using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NatisTracker.ViewModels;
using EnatisRepository.Repo;

namespace NatisTracker.UIServices
{
    public interface IPopulateViewModels
    {
        
        List<EmployeeDataViewModel> PopulateEmployees(Intern_LeaveDBEntities db);
        List<NatisDataViewModel> PopulateNatisData(Intern_LeaveDBEntities db);
        List<ScanLogsDataViewModel> PopulateScanLogs(Intern_LeaveDBEntities db);
        List<RequestsDataViewModel> PopulateRequestsData(Intern_LeaveDBEntities db);
        List<SentIN_DeliveryViewModel> PopulateSentIN_Deliveries(Intern_LeaveDBEntities db);
        List<SentOUT_DeliveryViewModel> PopulateSentOUT_Deliveries(Intern_LeaveDBEntities db);
        List<ContractNumbersViewModel> PopulateContractNumber(Intern_LeaveDBEntities db);
        TickBoxViewModelList PopulateTickBoxData(Intern_LeaveDBEntities db);
    }
}