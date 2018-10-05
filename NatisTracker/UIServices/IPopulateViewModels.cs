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
        List<EmployeeDataViewModel> PopulateEmployees(NatisTrackerDBEntities db);
        List<NatisDataViewModel> PopulateNatisData(NatisTrackerDBEntities db);
        List<ScanLogsDataViewModel> PopulateScanLogs(NatisTrackerDBEntities db);
        List<RequestsDataViewModel> PopulateRequestsData(NatisTrackerDBEntities db);
        List<SentIN_DeliveryViewModel> PopulateSentIN_Deliveries(NatisTrackerDBEntities db);
        List<SentOUT_DeliveryViewModel> PopulateSentOUT_Deliveries(NatisTrackerDBEntities db);
        List<ContractNumbersViewModel> PopulateContractNumber(NatisTrackerDBEntities db);
        TickBoxViewModelList PopulateTickBoxData(NatisTrackerDBEntities db);
    }
}