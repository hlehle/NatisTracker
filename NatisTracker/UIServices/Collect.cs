using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EnatisRepository.Repo;
using System.IO;
using Aspose.BarCode.BarCodeRecognition;
using NatisTracker.ViewModels;
using Oracle.DataAccess.Client;
using System.Data;
using NatisTracker.Models;

namespace EnatisRepository.Models
{
    public class Collect : IScanNatis
    {
        public bool Scan(NatisDataViewModel viewModel, string name, string department)
        {
            using (Intern_LeaveDBEntities db = new Intern_LeaveDBEntities())
            {

                if (viewModel.file != null)
                {
                    byte[] byteArr = new byte[viewModel.file.ContentLength];
                    viewModel.file.InputStream.Read(byteArr, 0, viewModel.file.ContentLength);
                    Stream stream = new MemoryStream(byteArr);

                    var load = new LoadNew();
                    string[] natis = load.readBarCode(stream);

                    string contractNumber = load.getContractNo(natis[9]);

                    ScanLogsData log = new ScanLogsData();
                    NatisData data = new NatisData();

                    string[] contactInfo = load.GetContractStatus(contractNumber);

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
    }
}