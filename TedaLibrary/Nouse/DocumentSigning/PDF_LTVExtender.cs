using iTextSharp.text.pdf;
using iTextSharp.text.pdf.security;
using System;
using System.Collections.Generic;
using System.IO;

namespace TedaLibrary.DocumentSigning
{
    /// <summary>
    /// Make the signature in PDF LTV (Long Term Validation) enabled
    /// </summary>
    class PDF_LTVExtender
    {

        private MemoryStream outputStream = null;
        private PdfReader reader = null;
        private PdfStamper stamper = null;
        private String signatureName = null;
        private AcroFields fields = null;


        /// <summary>
        /// Prepare the pdf to make LTV-enabled
        /// </summary>
        /// <param name="byte_pdfData"></param>
        public PDF_LTVExtender(byte[] byte_pdfData)
        {
            this.outputStream = new MemoryStream();

            this.reader = new PdfReader(byte_pdfData);
            this.stamper = new PdfStamper(reader, outputStream, '\0', true);
            this.fields = stamper.AcroFields;
            List<string> _fieldNames = this.fields.GetSignatureNames();
            foreach (string _fieldName in _fieldNames)
            {
                signatureName = _fieldName;
            }
        }


        /// <summary>
        /// Get back the LTV-enabled PDF file
        /// </summary>
        /// <returns></returns>
        public byte[] getLTVPDF()
        {
            this.enableLTV();
            this.stamper.Close();
            return outputStream.ToArray();
        }


        /// <summary>
        /// Perform LTV
        /// </summary>
        private void enableLTV()
        {
            LtvVerification v = this.stamper.LtvVerification;
            PdfPKCS7 pkcs7 = this.fields.VerifySignature(this.signatureName);
            CrlClientOnline crl = new CrlClientOnline(pkcs7.SignCertificateChain);
            if (pkcs7.IsTsp)
            {
                v.AddVerification(signatureName, null, crl,
                    LtvVerification.CertificateOption.SIGNING_CERTIFICATE,
                    LtvVerification.Level.CRL,
                    LtvVerification.CertificateInclusion.NO);
            }
            else
            {
                v.AddVerification(signatureName, null, crl,
                    LtvVerification.CertificateOption.WHOLE_CHAIN,
                    LtvVerification.Level.CRL,
                    LtvVerification.CertificateInclusion.NO);
            }
        }
    }
}
