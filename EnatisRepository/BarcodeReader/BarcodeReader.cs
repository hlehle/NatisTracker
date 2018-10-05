using Aspose.Pdf.Facades;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aspose.BarCode.BarCodeRecognition;
using Aspose.BarCode;
using Aspose.Pdf.Devices;
using Aspose.Pdf;
using EnatisRepository.HubbleService;
using log4net;

namespace EnatisRepository.BarcodeReader
{
    public class BarcodeReader
    {
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public BarcodeReader()
        {
            //SetLicenses();
        }
        public string[] readBarCode(Stream barcodeLocation)
        {
            string[] codeText = null;
            try
            {
                PdfExtractor pdfExtractor = new PdfExtractor();
                pdfExtractor.BindPdf(barcodeLocation);
                pdfExtractor.StartPage = 1;

                pdfExtractor.EndPage = 1;
                pdfExtractor.ExtractImage();

                using (MemoryStream imageStream = new MemoryStream())
                {
                    pdfExtractor.GetNextImage(imageStream);
                    imageStream.Position = 0;
                    using (BarCodeReader barcodeReader = new BarCodeReader(imageStream, DecodeType.Pdf417))
                    {
                        while (barcodeReader.Read())
                        {
                            string str = barcodeReader.GetCodeText();
                            str = barcodeReader.GetCodeText().Remove(str.Length - 1).Remove(0, 1);
                            codeText = str.Split('%');
                        }
                    }
                }
                return codeText;
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                return null;
                //throw new Exception("SITHLE LOG THIS!!!");
            }
        }
        public void SetLicenses()
        {
            var BarCodeLicense = new Aspose.BarCode.License();
            BarCodeLicense.SetLicense("Aspose.Total.lic");
            var PdfLicence = new Aspose.Pdf.License();
            PdfLicence.SetLicense("Aspose.Total.lic");
        }
    }
}
