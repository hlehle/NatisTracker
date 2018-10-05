using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NatisTracker.ViewModels;
using System.IO;
using Oracle.DataAccess.Client;
using EnatisRepository.Repo;
using EnatisRepository.BarcodeReader;
using EnatisRepository.OracleDataRetrieval;
using System.Data;
using Aspose.BarCode.BarCodeRecognition;
using Aspose.Pdf.Facades;

namespace NatisTracker.ScanNatis
{
    public class LoadNew : IScanNatis
    {
        public bool Scan(NatisDataViewModel viewModel, string name, string department)
        {
            using (NatisTrackerDBEntities db = new NatisTrackerDBEntities())
            {
                if (viewModel.file != null)
                {
                    byte[] byteArr = new byte[viewModel.file.ContentLength];
                    viewModel.file.InputStream.Read(byteArr, 0, viewModel.file.ContentLength);
                    Stream stream = new MemoryStream(byteArr);
                    var reader = new BarcodeReader();
                    string[] natis = reader.readBarCode(stream);

                    if (natis != null)
                    {
                        // Filling Natis Doc Data
                        NatisData natisData = new NatisData();
                        var conn = new OracleDataConnections();

                        natisData.User = name;
                        natisData.DateLoaded = DateTime.Now;
                        natisData.VinNumber = natis[9];
                        var contractNumber = conn.getContractNo(natisData.VinNumber);
                        natisData.RegistrationNumber = natis[5];
                        natisData.EngineNumber = natis[10];
                        natisData.CarMake = natis[7];
                        natisData.SeriesNumber = conn.getDescription(contractNumber);
                        natisData.Description = natis[6];
                        natisData.RegistrationDate = Convert.ToDateTime(natis[12]);
                        natisData.VehicleStatus = natis[11];
                        natisData.OwnerName = natis[15];
                        natisData.OwnerIdentityNumber = natis[14];
                        natisData.NatisLocation = "Safe Vault";

                        // Filling Contract Data
                        string contractInfo = conn.GetContractStatus(contractNumber); 
                        ContractsData contractData = new ContractsData();
                        contractData.RecordNumber = natisData.RecordNumber;
                        contractData.VinNumber = natisData.VinNumber;
                        contractData.ContractNumber = contractNumber;
                        contractData.ContractStatus = contractInfo;
                        //contractData.StatusDescription = contractInfo[1];

                        ScanLogsData log = new ScanLogsData();

                        log.ContractNumber = contractNumber;
                        log.VinNumber = natisData.VinNumber;
                        log.DateScanned = DateTime.Now;
                        log.User = natisData.User;
                        log.Department = natisData.NatisLocation;
                        log.ContractStatus = contractData.ContractStatus;
                        log.ContractDescription = contractData.StatusDescription;

                        if (!isExist(db, natisData))
                        {
                            
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
                        else if (isExist(db, natisData) && !isInSafe(db, natisData))
                        {
                            log.Comment = "Back to Safe";

                            var vin = natisData.VinNumber;
                            var temp = db.NatisDatas.Where(a => a.VinNumber == vin).FirstOrDefault();
                            temp.NatisLocation = natisData.NatisLocation;
                            //db.NatisDatas.Add(natisData);
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

                        else if (isExist(db, natisData) && isInSafe(db, natisData))
                        {
                            return false;
                        }
                    }
                }
                return false;
            }
        }

        public bool isInSafe(NatisTrackerDBEntities db, NatisData data)
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

        public bool isExist(NatisTrackerDBEntities db, NatisData data)
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

        //public string[] readBarCode(Stream barcodeLocation)
        //{
        //    try
        //    {
        //        //PdfExtractor pdfExtractor = new PdfExtractor();
        //        //pdfExtractor.BindPdf(barcodeLocation);
        //        //pdfExtractor.StartPage = 1;

        //        //pdfExtractor.EndPage = 2;
        //        //pdfExtractor.ExtractImage();

        //        //MemoryStream imageStream = new MemoryStream();

        //        //pdfExtractor.GetNextImage(imageStream);

        //        //imageStream.Position = 0;

        //        BarCodeReader barcodeReader = new BarCodeReader(barcodeLocation, DecodeType.Pdf417);

        //        string[] codeText = null;
        //        while (barcodeReader.Read())
        //        {
        //            string str = barcodeReader.GetCodeText();
        //            str = barcodeReader.GetCodeText().Remove(str.Length - 1).Remove(0, 1);
        //            codeText = str.Split('%');
        //        }
        //        barcodeReader.Close();
        //        return codeText;
        //    }
        //    catch (Exception )
        //    {
        //        return null;
        //    }
        //}
    }
}