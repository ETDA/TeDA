using iTextSharp.text.pdf;
using iTextSharp.text.pdf.security;
using Org.BouncyCastle.Cms;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509.Store;
using System;
using System.Collections;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace TedaLibrary.DocumentSigning
{
    class PDF_Validator
    {
        public void verify(byte[] byte_pdfData, X509Certificate2 trustCert, string signatureName)
        {
            PdfReader pdfReader = new PdfReader(byte_pdfData);

            AcroFields acroField = pdfReader.AcroFields;

            if (signatureName == null || "".CompareTo(signatureName) != 0)
            {
                signatureName = acroField.GetSignatureNames().Last();
            }

            PdfPKCS7 pdfP7 = acroField.VerifySignature(signatureName);

            if (pdfP7 == null) throw new NullReferenceException("Invalid signatureName:" + signatureName);

            if (!pdfP7.Verify()) throw new PdfException("Unable to verify specified signature field, specify signature invalid");

            byte[] pkcs7Signatue = pdfP7.GetEncodedPKCS7();
            CmsSignedData signedData = new CmsSignedData(pkcs7Signatue);

            // Get signer certificate from CMSSignedData
            IX509Store x509Certs = signedData.GetCertificates("Collection");
            ICollection cerlist = x509Certs.GetMatches(null);
            IEnumerator cEnum = cerlist.GetEnumerator();
            ArrayList _chain = new ArrayList();
            while (cEnum.MoveNext())
            {
                Org.BouncyCastle.X509.X509Certificate cer = (Org.BouncyCastle.X509.X509Certificate)cEnum.Current;
                X509Certificate2 cer2 = new X509Certificate2(cer.GetEncoded());
                _chain.Add(cer2);
            }

            X509Certificate2[] certChain = (X509Certificate2[])_chain.ToArray(typeof(X509Certificate2));
            validateSignature(signedData, certChain[0]);
        }

        /// <summary>
        /// Validate the signature that valid or not
        /// </summary>
        /// <param name="signedData"></param>
        /// <param name="certificate"></param>
        private void validateSignature(CmsSignedData signedData, X509Certificate2 certificate)
        {
            bool isValid = false;

            SignerInformationStore signers = signedData.GetSignerInfos();
            IEnumerator it = signers.GetSigners().GetEnumerator();
            if (it.MoveNext())
            {
                SignerInformation signer = (SignerInformation)it.Current;
                Org.BouncyCastle.X509.X509Certificate cer = DotNetUtilities.FromX509Certificate(certificate);
                isValid = signer.Verify(cer);
            }

            if (!isValid)
            {
                throw new Exception("Signature is not valid");
            }

        }
    }
}
