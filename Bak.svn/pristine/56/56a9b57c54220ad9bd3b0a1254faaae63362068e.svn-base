using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TedaLibrary.Interface;
using System.IO;
using Word = Microsoft.Office.Interop.Word;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.xml.xmp;
using iTextSharp.xmp;
using TedaLibrary.Object;
using CreateETDAInvoiceFromXml;

namespace TedaLibrary.Implement
{
    public class PdfCreator : IPdfCreator
    {
        public PdfCreator()
        {

        }

        public byte[] Pdf2PdfA(byte[] pdf, List<KeyValuePair<string, string>> properties, EAttactment attachment)
        {
            PdfReader document = new PdfReader(pdf);
            MemoryStream stream = new MemoryStream();
            Document pdfADocument = new Document();

            PdfAWriter writer = this.CreatePDFAInstance(pdfADocument, document, stream);
            writer.CreateXmpMetadata();
            XmpWriter xmpWriter = writer.XmpWriter;

            properties.ForEach(delegate (KeyValuePair<string, string> keyValuePairs)
                {
                    xmpWriter.AddDocInfoProperty(keyValuePairs.Key, keyValuePairs.Value);
                });

            GenerateEAttachmentDocument(writer, xmpWriter, attachment);

            pdfADocument.Close();
            document.Close();

            return stream.ToArray();
        }

        public byte[] Pdf2PdfA(byte[] pdf, List<KeyValuePair<string, string>> properties, OutputIntents outputIntents)
        {
            PdfReader document = new PdfReader(pdf);
            MemoryStream stream = new MemoryStream();
            Document pdfADocument = new Document();

            PdfAWriter writer = this.CreatePDFAInstance(pdfADocument, document, stream);
            writer.CreateXmpMetadata();
            XmpWriter xmpWriter = writer.XmpWriter;

            properties.ForEach(delegate (KeyValuePair<string, string> keyValuePairs)
            {
                xmpWriter.AddDocInfoProperty(keyValuePairs.Key, keyValuePairs.Value);
            });

            generatePDFA3Document(writer, outputIntents);

            pdfADocument.Close();
            document.Close();

            return stream.ToArray();
        }

        public void Word2Pdf(string path, string outputPath)
        {
            Word.Application wordApp = new Word.Application();
            Word.Document wordDocument = wordApp.Documents.Open(path);
            wordDocument.ExportAsFixedFormat(outputPath, Word.WdExportFormat.wdExportFormatPDF);
            wordDocument.Close();
        }

        public void ZUGFeRD2Pdf(string xmlPath, string xslPath,string outputFileName)
        {
            PdfInvoice app = new PdfInvoice();
            app.CreatePdf(xslPath, xmlPath, outputFileName);
        }

        public void VerifyPdf(byte[] pdf)
        {
            PdfReader document = new PdfReader(pdf);
            MemoryStream memory = new MemoryStream();
            try
            {
                PdfStamper stamper = PdfStamper.CreateSignature(document, memory, '\0');
            }
            catch (Exception ex)
            {
                throw new Exception("This pdf is can not be signed.");
            }
        }

        //public void VerifyPdfA3(byte[] pdf)
        //{

        //}

        private PdfAWriter CreatePDFAInstance(Document targetDocument, PdfReader originalDocument, Stream os)
        {
            PdfAWriter writer = PdfAWriter.GetInstance(targetDocument, os, PdfAConformanceLevel.PDF_A_3U);
            writer.CreateXmpMetadata();

            if (!targetDocument.IsOpen())
                targetDocument.Open();

            PdfContentByte cb = writer.DirectContent; // Holds the PDF data	
            PdfImportedPage page;
            int pageCount = originalDocument.NumberOfPages;
            for (int i = 0; i < pageCount; i++)
            {
                targetDocument.NewPage();
                page = writer.GetImportedPage(originalDocument, i + 1);
                cb.AddTemplate(page, 0, 0);
            }
            return writer;
        }

        private void generatePDFA3Document(PdfAWriter writer, OutputIntents outputIntents)
        {
            // Use default intent if output intent of this instance was not set
            if (outputIntents == null)
            {
                //byte[] iccProfile = File.ReadAllBytes("/Resources/sRGB Color Space Profile.icm");
                byte[] iccProfileByteArray = Properties.Resources.sRGB_Color_Space_Profile;
                ICC_Profile icc = ICC_Profile.GetInstance(iccProfileByteArray);
                writer.SetOutputIntents("sRGB IEC61966-2.1", "", "http://www.color.org", "sRGB IEC61966-2.1", icc);
            }
            else
            {
                byte[] iccProfileByteArray = File.ReadAllBytes(outputIntents.colorProfilePath);
                ICC_Profile icc = ICC_Profile.GetInstance(iccProfileByteArray);
                writer.SetOutputIntents(outputIntents.outputConditionIdentifier, outputIntents.outputCondition, outputIntents.registryName, outputIntents.info, icc);
            }
        }

        private void GenerateEAttachmentDocument(PdfAWriter writer, XmpWriter xmpWriter, EAttactment attachment)
        {
            // Use default intent if output intent of this instance was not set
            if (attachment.outputIntents == null)
            {
                //byte[] iccProfile = File.ReadAllBytes("/Resources/sRGB Color Space Profile.icm");
                byte[] iccProfile = Properties.Resources.sRGB_Color_Space_Profile;
                ICC_Profile icc = ICC_Profile.GetInstance(iccProfile);
                writer.SetOutputIntents("sRGB IEC61966-2.1", "", "http://www.color.org", "sRGB IEC61966-2.1", icc);
            }
            else
            {
                OutputIntents outputIntents = attachment.outputIntents;
                byte[] iccProfileByteArray = File.ReadAllBytes(outputIntents.colorProfilePath);
                ICC_Profile icc = ICC_Profile.GetInstance(iccProfileByteArray);
                writer.SetOutputIntents(outputIntents.outputConditionIdentifier, outputIntents.outputCondition, outputIntents.registryName, outputIntents.info, icc);
            }

            //============= Create Exchange ECertificate =================
            // 1 add ContentInformation.xml to document
            PdfArray attachmentArray = new PdfArray();
            writer.ExtraCatalog.Put(new PdfName("AF"), attachmentArray);
            PdfFileSpecification contentSpec = this.EmbeddedAttachment(attachment.contentInformationXMLPath, attachment.attachmentName,
                    attachment.attachmentMIME, new PdfName(attachment.attachmentType), writer, attachment.attachmentDescription);
            attachmentArray.Add(contentSpec.Reference);

            foreach (var item in attachment.fileAttachments)
            {
                contentSpec = this.EmbeddedAttachment(item.attachmentPath, item.attachmentName,
                    item.attachmentMIME, new PdfName(item.attachmentType), writer, item.attachmentDescription);
                attachmentArray.Add(contentSpec.Reference);
            }

            // 2 add Electronic Document XMP Metadata
            ElectronicDocumentSchema ed = ElectronicDocumentSchema.generateED(attachment.attachmentName, attachment.documentVersion, attachment.documentID, attachment.documentOID);
            xmpWriter.AddRdfDescription(ed);

            string pdfaSchema = Properties.Resources.EDocument_PDFAExtensionSchema;
            // convert string to stream
            byte[] byteArray = Encoding.UTF8.GetBytes(pdfaSchema);


            //byte[] byteArray = Encoding.ASCII.GetBytes(contents);
            MemoryStream stream = new MemoryStream(byteArray);
            IXmpMeta edPDFAextension = XmpMetaFactory.Parse(stream);
            IXmpMeta originalXMP = xmpWriter.XmpMeta;

            XmpUtils.AppendProperties(edPDFAextension, originalXMP, true, true);

        }

        private PdfFileSpecification EmbeddedAttachment(String filePath, String fileName, String mimeType,
            PdfName afRelationship, PdfAWriter writer, String description)
        {
            PdfDictionary parameters = new PdfDictionary();
            parameters.Put(PdfName.MODDATE, new PdfDate(File.GetLastWriteTime(filePath)));
            PdfFileSpecification fileSpec = PdfFileSpecification.FileEmbedded(writer, filePath, fileName, null, mimeType,
                    parameters, 0);
            fileSpec.Put(new PdfName("AFRelationship"), afRelationship);
            writer.AddFileAttachment(description, fileSpec);
            return fileSpec;
        }

        private static MemoryStream CreatePDF(string html)
        {
            MemoryStream msOutput = new MemoryStream();
            TextReader reader = new StringReader(html);

            // step 1: creation of a document-object
            Document document = new Document(PageSize.A4, 30, 30, 30, 30);

            // step 2:
            // we create a writer that listens to the document
            // and directs a XML-stream to a file
            PdfWriter writer = PdfWriter.GetInstance(document, msOutput);

            // step 3: we create a worker parse the document

            //  var 1 = new XMLWorker(document);

            /**************************************************
             * Example #2                                     *
             *                                                *
             * Use the XMLWorker to parse the HTML.           *
             * Only inline CSS and absolutely linked          *
             * CSS is supported                               *
             * ************************************************/
            document.Open();
            //XMLWorker also reads from a TextReader and not directly from a string

            //Parse the HTML
            iTextSharp.tool.xml.XMLWorkerHelper.GetInstance().ParseXHtml(writer, document, reader);


            // step 4: we open document and start the worker on the document

            //worker.StartDocument();

            //// step 5: parse the html into the document
            //worker.Parse(reader);

            //// step 6: close the document and the worker
            //worker.EndDocument();
            //  worker.Close();
            document.Close();

            return msOutput;
        }
    }
}
