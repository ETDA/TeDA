using iTextSharp.text.pdf;
using iTextSharp.text.pdf.security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace TedaLibrary.DocumentSigning
{
    class PDF_Signer
    {
        private CryptoStandard sigType;

        public PDF_Signer()
        {
            //Default Crypto standard
            this.sigType = CryptoStandard.CADES;
        }

        public PDF_Signer(CryptoStandard sigType)
        {
            this.sigType = sigType;
        }

        public MemoryStream sign(
            byte[] byte_pdfData, X509Certificate2 cert, Org.BouncyCastle.X509.X509Certificate[] chain,
            string hashAlgorithm, string reason, string location, int certifyLevel,
            byte[] sigImg, bool isShowDescription
            )
        {
            //Open source PDF
            PdfReader pdfReader = new PdfReader(byte_pdfData);

            MemoryStream outputStream = new MemoryStream();

            //Create PDF Stamper
            PdfStamper pdfStamper = PdfStamper.CreateSignature(pdfReader, outputStream, '\0');

            //Create PDF Signature Appearance
            PdfSignatureAppearance signatureAppearance = pdfStamper.SignatureAppearance;
            signatureAppearance.Reason = reason; //Reason
            signatureAppearance.Location = location; //Location
            signatureAppearance.CertificationLevel = certifyLevel;
            signatureAppearance.SignatureRenderingMode = PdfSignatureAppearance.RenderingMode.DESCRIPTION; //Rendering mode


            IExternalSignature signature = new X509Certificate2Signature(cert, hashAlgorithm);

            try
            {
                //Do signing
                MakeSignature.SignDetached(signatureAppearance, signature, chain, null, null, null, 0, this.sigType);
            }
            catch (Exception e)
            {
                throw new Exception("Cannot sign the PDF file.", e);
            }

            return outputStream;
        }
    }
}
