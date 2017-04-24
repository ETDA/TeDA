using System.Collections.Generic;
using iTextSharp.text;
using iTextSharp.text.pdf.security;
using Org.BouncyCastle.Crypto;
using iTextSharp.text.pdf;
using System.IO;
using System;
using System.Security.Cryptography;
using System.Text;
using Org.BouncyCastle.Pkcs;
using System.Security.Cryptography.X509Certificates;
using LTVDigitalSignature;

namespace TedaLibrary
{
    class TedaObj
    {
        Rectangle sigLocation;
        Font font;
        private AsymmetricKeyParameter privateKey;
        private ICollection<Org.BouncyCastle.X509.X509Certificate> chain;
        public List<ICrlClient> crlList;
        private IExternalSignature signature;
        private bool tsa = false;
        private string tsaUrl = "";
        private string tsaUsername = "";
        private string tsaPassword = "";
        private string signatureField = "";

        public void setTimestamp(string tsaUrl, string tsaUsername, string tsaPassword)
        {
            this.tsaUrl = tsaUrl;
            this.tsaUsername = tsaUsername;
            this.tsaPassword = tsaPassword;
            this.tsa = true;
        }

        public void setKeyStore(Pkcs12Store keystore, string hashAlgorithm) //Getkey
        {
            //get name
            String alias = "";
            foreach (string al in keystore.Aliases)
            {
                if (keystore.IsKeyEntry(al) && keystore.GetKey(al).Key.IsPrivate) // ****  what this if do ?
                {
                    alias = al;
                    break;
                }
            }

            //get privatekey
            this.privateKey = keystore.GetKey(alias).Key;

            //create instance of Cretificate list for Long Time 
            this.chain = new List<Org.BouncyCastle.X509.X509Certificate>();
            foreach (X509CertificateEntry entry in keystore.GetCertificateChain(alias))
            {
                this.chain.Add(entry.Certificate);
            }

            this.signature = new PrivateKeySignature(privateKey, hashAlgorithm);
        }

        public void setKeyStore(X509Certificate2 cert, string hashAlgorithm) //Getkey
        {
            /*GET Certificate chain from Cert and translate info x509 Bouncycastle List*/
            Org.BouncyCastle.X509.X509Certificate bcCert = Org.BouncyCastle.Security.DotNetUtilities.FromX509Certificate(cert); // ไม่ได้เก็ต ของผู้ออก Certificate Chain มาด้วย 
            chain = new List<Org.BouncyCastle.X509.X509Certificate> { bcCert };

            // Initial .netx509 certchain and build chain
            X509Chain cert_chain = new X509Chain();
            cert_chain.Build(cert);

            int i = 0;
            //Add chain into bouncyCastle.chain
            foreach (X509ChainElement entry in cert_chain.ChainElements)
            {
                if (i != 0)//Skip first certchain due to cert_chain.Build provided first chain(entry.chain.[0]) 
                    this.chain.Add(Org.BouncyCastle.Security.DotNetUtilities.FromX509Certificate(entry.Certificate));
                i++;
            }

            this.signature = new RSAProviderPrivateKey(cert, hashAlgorithm);
        }

        public void getCRLList()
        {
            this.crlList = new List<ICrlClient>();
            ICrlClient crlOnline = new CrlClientOnline(this.chain);
            this.crlList.Add(crlOnline);
        }

        public byte[] importXml(byte[] inputPdf, Stream xmlData, Boolean isSign)
        {
            Console.WriteLine("Read PDF");
            PdfReader reader = new PdfReader(inputPdf);
            MemoryStream output = new MemoryStream();
            PdfStamper stamper = new PdfStamper(reader, output, '\0', false);

            AcroFields form = stamper.AcroFields;
            XfaForm xfa = form.Xfa;
            Console.WriteLine("Fill Data");
            xfa.FillXfaForm(xmlData);
            Console.WriteLine("Set fields to read only");
            if (!isSign)
            {
                foreach (string key in form.Fields.Keys)
                {
                    form.SetFieldProperty(key, "setfflags", PdfFormField.FF_READ_ONLY, null);
                }
            }

            stamper.Close();
            reader.Close();
            return output.ToArray();
        }

        public string findSignatureField(byte[] inputPdf, string fieldName)
        {
            string resultFieldName = "";
            Console.WriteLine("Read PDF");
            PdfReader reader = new PdfReader(inputPdf);
            AcroFields form = reader.AcroFields;
            foreach (string key in form.Fields.Keys)
            {
                if (key.IndexOf(fieldName) > 0)
                {
                    if (AcroFields.FIELD_TYPE_SIGNATURE == form.GetFieldType(key))
                    {
                        Console.WriteLine("Signature Field Found");
                        resultFieldName = key;
                        break;
                    }
                }
            }
            reader.Close();
            return resultFieldName;
        }

        public byte[] signPdf(byte[] inputPdf, byte[] sigImg, string signatureField)
        {
            this.getCRLList();
            Console.WriteLine("Read PDF");
            PdfReader reader = new PdfReader(inputPdf);
            MemoryStream output = new MemoryStream();

            PdfStamper stamper = PdfStamper.CreateSignature(reader, output, '\0', null, true);

            PdfSignatureAppearance sap = stamper.SignatureAppearance;
            sap.Reason = "test";
            sap.Location = "Bangkok";
            // Set Signature Image
            if (sigImg != null)
            {
                sap.SignatureGraphic = Image.GetInstance(sigImg);
                sap.ImageScale = -1;
                sap.SignatureRenderingMode = PdfSignatureAppearance.RenderingMode.GRAPHIC;
            }
            // Set Signature Field
            if (signatureField.Equals("") || signatureField == null)
            {
                Rectangle location = new Rectangle(10, 10, 300, 100);
                sap.SetVisibleSignature(location, 1, "signatureField");
            }
            else sap.SetVisibleSignature(signatureField);

            sap.CertificationLevel = PdfSignatureAppearance.NOT_CERTIFIED;

            //Create TSA server
            ITSAClient tsaClient = null;
            Boolean isTsaConnected = false;
            if (tsa)
            {
                tsaClient = new TSAClientBouncyCastle(tsaUrl, tsaUsername, tsaPassword);
                for (int retry = 0; retry < 5; retry++)
                {
                    try
                    {
                        //int hash = tsaClient.GetHashCode();
                        string testString = "test";
                        byte[] digest;
                        using (SHA256Managed sha256 = new SHA256Managed())
                        {
                            digest = sha256.ComputeHash(Encoding.UTF8.GetBytes(testString));
                        }
                        tsaClient.GetTimeStampToken(digest);
                        isTsaConnected = true;
                        break;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.StackTrace);
                    }
                    Console.WriteLine("retry " + (retry + 1));
                }
            }
            //Do Signing Check not null timestamp and crl
            if (tsaClient != null && crlList != null && isTsaConnected)
            {
                try
                {
                    MakeSignature.SignDetached(sap, this.signature, chain, this.crlList, null, tsaClient, 0, CryptoStandard.CADES);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.StackTrace);
                }
            }
            else
            {
                Console.WriteLine("Cannot sign the PDF file.");
                return null;
            }
            reader.Close();
            stamper.Close();
            signature = null;


            return output.ToArray();
        }
    }
}
