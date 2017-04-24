using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iTextSharp.text.pdf.security;
using iTextSharp.text.pdf;
using System.Collections;
using System.Security.Cryptography;
using TedaLibrary.Object;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using System.IO;

namespace TedaLibrary.Implement
{
    class DigitalSigRemote
    {
        PdfPKCS7 pdfP7;
        byte[] pdfHashValue;
        byte[] encodedPKCS7;
        private const int ESTIMATE_SIZE = 51200;
        private X509Certificate2Signature[] TRUSTED_ROOT_CERTIFICATE;
        private X509Certificate2Signature[] certificateChain;
        private List<ICrlClient> crlList;
        private PdfSignatureAppearance sap;
        public long INSTANCE;

        private PdfReader document;
        private PdfStamper pdfStamper;
        private MemoryStream stream = new MemoryStream();
        private static PdfPKCS7 sgn = null;
        private static byte[] hashValue = null;

        private static Dictionary<long, DigitalSigRemote> MEMORY_DB = new Dictionary<long, DigitalSigRemote>();

        private void CreateInstance(byte[] pdf)
        {
            DigitalSigRemote instance = new DigitalSigRemote();
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                // Buffer storage.
                byte[] data = new byte[4];

                // Ten iterations.
                for (int i = 0; i < 10; i++)
                {
                    // Fill buffer.
                    rng.GetBytes(data);

                    // Convert to int 32.
                    int value = BitConverter.ToInt32(data, 0);
                    this.INSTANCE = value;
                    MEMORY_DB.Add(this.INSTANCE, this);
                    //Console.WriteLine(value);
                }
            }
        }

        private DigitalSigRemote getInstance(long id)
        {
            return MEMORY_DB[id];
        }

        private void CreateNewSignatureField(SignInformation signInfo)
        {
            try
            {

                pdfStamper = PdfStamper.CreateSignature(document, stream, '\0', null, true);
                sap = pdfStamper.SignatureAppearance;
                sap.Reason = signInfo.reason;
                sap.Location = signInfo.location;
                sap.CertificationLevel = signInfo.certifyLevel;

                PdfSignature sig = new PdfSignature(PdfName.ADOBE_PPKLITE, PdfName.ADBE_PKCS7_DETACHED);
                sig.Reason = signInfo.reason;
                sap.CryptoDictionary = sig;

                Dictionary<PdfName, int> exc = new Dictionary<PdfName, int>();
                exc[PdfName.CONTENTS] = ESTIMATE_SIZE * 2 + 2;

                sap.PreClose(exc);
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }

        private String calculateSigningContent(X509Certificate2 cert)
        {
            byte[] sh = null;
            try
            {
                Org.BouncyCastle.X509.X509Certificate[] chain = GetCertChain(cert);
                sgn = new PdfPKCS7(null, chain, "SHA-256", true);
                hashValue = HashFile(sap.GetRangeStream());
                List<ICrlClient> crlList = new List<ICrlClient>();
                crlList.Add(new CrlClientOnline(chain));

                ICollection<byte[]> crlBytes = null;
                int i = 0;
                while (crlBytes == null && i < chain.Length)
                    crlBytes = MakeSignature.ProcessCrl(chain[i++], null);

                sh = sgn.getAuthenticatedAttributeBytes(hashValue, null, crlBytes/*crlbyte*/, CryptoStandard.CADES);
            }
            catch (Exception ex)
            {
                if (document != null)
                {
                    document.Close();
                }
                if (pdfStamper != null)
                {
                    pdfStamper.Close();
                }
                throw new Exception("getHash : " + ex.Message, ex);
            }
            return System.Convert.ToBase64String(sh);
        }

        private void Sign(string hash)
        {
            try
            {

                byte[] result = System.Convert.FromBase64String(hash);
                sgn.SetExternalDigest(result, null, "RSA");
                byte[] encodesig = sgn.GetEncodedPKCS7(hashValue, null /* tsa client */, null, null, CryptoStandard.CADES);

                sap.Close(this.AttachSignedContent(encodesig));
            }
            catch (Exception ex)
            {
                if (document != null)
                {
                    document.Close();
                }
                if (pdfStamper != null)
                {
                    pdfStamper.Close();
                }
                throw ex;
            }
        }

        private PdfDictionary AttachSignedContent(byte[] sigValue)
        {
            if (ESTIMATE_SIZE < sigValue.Length)
                throw new IOException("Not enough space");

            byte[] paddedSig = new byte[ESTIMATE_SIZE];
            Array.Copy(sigValue, paddedSig, sigValue.Length);
            PdfDictionary dic = new PdfDictionary();
            dic.Put(PdfName.CONTENTS, new PdfString(paddedSig).SetHexWriting(true));
            return dic;
        }


        private string verifyExistingSignatureField(String sigField)
        {
            AcroFields form = this.document.AcroFields;
            foreach (string key in form.Fields.Keys)
            {
                if (key.IndexOf(sigField) > 0)
                {
                    if (form.GetFieldType(key) == AcroFields.FIELD_TYPE_SIGNATURE)
                    {
                        return key;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            return null;
        }

        private Org.BouncyCastle.X509.X509Certificate[] GetCertChain(X509Certificate2 cert)
        {
            Org.BouncyCastle.X509.X509CertificateParser cp = new Org.BouncyCastle.X509.X509CertificateParser();
            X509Chain ch = new X509Chain();

            //ch.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck; 
            ch.Build(cert);
            Org.BouncyCastle.X509.X509Certificate[] chain = new Org.BouncyCastle.X509.X509Certificate[ch.ChainElements.Count];
            for (int idx = 0; idx < ch.ChainElements.Count; idx++)
            {
                X509ChainElement chElem = ch.ChainElements[idx];
                chain[idx] = cp.ReadCertificate(chElem.Certificate.RawData);
            }
            return chain;
        }

        private byte[] HashFile(Stream stream)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            byte[] hash = null;
            if (stream != null)
            {
                stream.Seek(0, SeekOrigin.Begin);

                SHA256 sha = SHA256.Create();
                hash = sha.ComputeHash(stream);
            }
            return hash;
        }
    }
}
