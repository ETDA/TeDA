using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using TedaLibrary.Implement;
using TedaLibrary.Object;

namespace TedaLibrary.Test
{
    class TestCreator
    {
        static void Main(string[] args)
        {
            TestZUGFeRD2Pdf();

        }

        private static void TestWord2PDF()
        {
            string pdfPath = @"";
            string outputPath = @"";
            PdfCreator pdfCreater = new PdfCreator();
            pdfCreater.Word2Pdf(pdfPath, outputPath);
        }

        private static void TestPdf2PdfA()
        {
            string pdfPath = @"";
            string outputPath = @"";
            PdfCreator pdfCreator = new PdfCreator();

            List<KeyValuePair<string, string>> properties = new List<KeyValuePair<string, string>>();
            properties.Add(new KeyValuePair<string, string>("Title", "ETDA Recommendation PDF/A-3U Example"));
            properties.Add(new KeyValuePair<string, string>("Author", "Author 1"));
            properties.Add(new KeyValuePair<string, string>("Author", "Author 2"));
            properties.Add(new KeyValuePair<string, string>("Subject", "Example Document"));
            properties.Add(new KeyValuePair<string, string>("Keywords", "ETDA Recommendation Basic Level Document"));
            
            // use default if not define
            OutputIntents outputIntents = new OutputIntents();
            outputIntents.colorProfilePath = @"";
            outputIntents.outputConditionIdentifier = "sRGB IEC61966-2.1";
            outputIntents.outputCondition = "";
            outputIntents.registryName = "http://www.color.org";
            outputIntents.info = "sRGB IEC61966-2.1";
            
            byte[] pdfByte = File.ReadAllBytes(pdfPath);
            byte[] result = pdfCreator.Pdf2PdfA(pdfByte, properties,outputIntents);
            //byte[] result = pdfCreator.Pdf2PdfA(pdfByte, properties, null);
            File.WriteAllBytes(outputPath, result);
        }

        private static void TestPdf2PdfAWithAttachment()
        {
            string pdfPath = @"";
            string outputPath = @"";
            PdfCreator pdfCreator = new PdfCreator();

            List<KeyValuePair<string, string>> properties = new List<KeyValuePair<string, string>>();
            properties.Add(new KeyValuePair<string, string>("Title", "ETDA Recommendation PDF/A-3U Example"));
            properties.Add(new KeyValuePair<string, string>("Author", "Author 1"));
            properties.Add(new KeyValuePair<string, string>("Author", "Author 2"));
            properties.Add(new KeyValuePair<string, string>("Subject", "Example Document"));
            properties.Add(new KeyValuePair<string, string>("Keywords", "ETDA Recommendation Basic Level Document"));

            EAttactment eAttach = new EAttactment();
            //EAttactment eAttach = new EAttactment("ContentInformation.xml", "resource/ContentInformation.xml", "Text/XML",EAttactment.ALTERNATIVE, "Document Content in XML format");
            eAttach.attachmentName = "ContentInformation.xml";
            eAttach.contentInformationXMLPath = "resource/ContentInformation.xml";
            eAttach.attachmentMIME = "Text/XML";
            eAttach.attachmentType = EAttactment.ALTERNATIVE;
            eAttach.attachmentDescription = "Document Content in XML format";
            eAttach.documentID = "C# example 01/2559";
            eAttach.documentOID = "2.16.764.1.4.100.0.0.0.0";
            eAttach.documentVersion = "1.0";

            FileAttachment fileAttach = new FileAttachment();
            fileAttach.attachmentName = @"FileAttach.docx";
            fileAttach.attachmentPath = @"D:/specification.docx";
            fileAttach.attachmentMIME = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
            fileAttach.attachmentType = EAttactment.SOURCE;
            fileAttach.attachmentDescription = "Specification Document";

            eAttach.fileAttachments = new List<FileAttachment>();
            eAttach.fileAttachments.Add(fileAttach);

            // use default if not define
            OutputIntents outputIntents = new OutputIntents();
            outputIntents.colorProfilePath = @"";
            outputIntents.outputConditionIdentifier = "sRGB IEC61966-2.1";
            outputIntents.outputCondition = "";
            outputIntents.registryName = "http://www.color.org";
            outputIntents.info = "sRGB IEC61966-2.1";
            eAttach.outputIntents = outputIntents;

            byte[] pdfByte = File.ReadAllBytes(pdfPath);
            byte[] result = pdfCreator.Pdf2PdfA(pdfByte, properties, eAttach);
            File.WriteAllBytes(outputPath, result);
        }

        private static void TestZUGFeRD2Pdf()
        {
            //string xmlPath = @"";
            //string xslPath = @"";
            //string outputFileName = @"";
            //PdfCreator pdfCreator = new PdfCreator();
            //pdfCreator.ZUGFeRD2Pdf(xmlPath, xslPath, outputFileName);

            string main_dir = "C:\\Users\\admin\\Desktop\\src\\";

            string xmlPath = main_dir + "ZUGFeRD-invoice.xml";
            string xslPath = main_dir + "ZUGFeRD-invoice.xslt";
            string outputFileName = main_dir + "result.pdf";
            PdfCreator pdfCreator = new PdfCreator();
            pdfCreator.ZUGFeRD2Pdf(xmlPath, xslPath, outputFileName);

        }

        private void TestVerifyPdf()
        {
            PdfCreator pdfCreator = new PdfCreator();
            string pdfPath = @"";
            byte[] pdfByte = File.ReadAllBytes(pdfPath);
            try
            {
                pdfCreator.VerifyPdf(pdfByte);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
