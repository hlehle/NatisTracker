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

namespace EnatisRepository.BarcodeReader
{
    public class BarcodeReader
    {
        public BarcodeReader()
        {
            SetLicenses();
        }
        public string[] readBarCode(Stream barcodeLocation)
        {
            try
            {
                string dataDir = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\SampleTest.pdf";
                var pdfDocument = new Document(dataDir);

                for(int pageNr = 1; pageNr <= pdfDocument.Pages.Count; pageNr++)
                {
                    using (var pageDocument = new Document())
                    {
                        pageDocument.Pages.Add(pdfDocument.Pages[pageNr]);
                        pageDocument.Save(dataDir + " " + pageNr + ".pdf");
                    }
                }
                
                PdfExtractor pdfExtractor = new PdfExtractor();
                pdfExtractor.BindPdf(barcodeLocation);
                pdfExtractor.StartPage = 1;

                pdfExtractor.EndPage = 1;
                pdfExtractor.ExtractImage();

                MemoryStream imageStream = new MemoryStream();
                pdfExtractor.GetNextImage(imageStream);
                imageStream.Position = 0;
                BarCodeReader barcodeReader = new BarCodeReader(imageStream, DecodeType.Pdf417);

                //BarCodeReader reader = new BarCodeReader(barcodeLocation, DecodeType.Pdf417);

                string[] codeText = null;
                while (barcodeReader.Read())
                {
                    string str = barcodeReader.GetCodeText();
                    str = barcodeReader.GetCodeText().Remove(str.Length - 1).Remove(0, 1);
                    codeText = str.Split('%');
                }
                barcodeReader.Close();
                return codeText;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public void SetLicenses()
        {
            var BarCodeLicense = new Aspose.BarCode.License();
            BarCodeLicense.SetLicense("Aspose.Total.lic");
            var PdfLicence = new Aspose.BarCode.License();
            PdfLicence.SetLicense("Aspose.Total.lic");
        }
    }
}
