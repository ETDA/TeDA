﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using TedaLibrary.Implement;
using TedaLibrary.Object;
using System.Security.Cryptography.X509Certificates;
using iTextSharp.text.pdf.security;

namespace TedaLibrary.Test
{
    class TestDigitalSig
    {
        static void Main(string[] args)
        {
            Sign();
        }

        private static SignInformation setSignInformation()
        {
            string sigImgPath = @"";
            string tsaUrl = "";
            string tsaUsername = "";
            string tsaPassword = "";

            SignInformation signInfo = new SignInformation();
            signInfo.hashAlgorithm = "SHA256";
            signInfo.reason = "";
            signInfo.location = "";
            signInfo.certifyLevel = 1;
            signInfo.sigImg = File.ReadAllBytes(sigImgPath);
            signInfo.sigType = CryptoStandard.CADES; // 1
            signInfo.cert = new DigitalSigUtil().GetX509Certificate2();

            X509Chain cert_chain = new X509Chain();
            cert_chain.Build(signInfo.cert);

            Org.BouncyCastle.X509.X509Certificate bcCert = Org.BouncyCastle.Security.DotNetUtilities.FromX509Certificate(signInfo.cert); // ไม่ได้เก็ต ของผู้ออก Certificate Chain มาด้วย 
            ICollection<Org.BouncyCastle.X509.X509Certificate> chain = new List<Org.BouncyCastle.X509.X509Certificate> { bcCert };
            int i = 0;
            //Add chain into bouncyCastle.chain
            foreach (X509ChainElement entry in cert_chain.ChainElements)
            {
                if (i != 0)//Skip first certchain due to cert_chain.Build provided first chain(entry.chain.[0]) 
                    chain.Add(Org.BouncyCastle.Security.DotNetUtilities.FromX509Certificate(entry.Certificate));
                    //chain[i] = Org.BouncyCastle.Security.DotNetUtilities.FromX509Certificate(entry.Certificate);
                i++;
            }
            signInfo.chain = chain;
            signInfo.crlList = new DigitalSigUtil().GetCrlList(signInfo.chain);
            signInfo.tsaClient = new TSAClientBouncyCastle(tsaUrl, tsaUsername, tsaPassword);
            return signInfo;
        }

        private static void Sign()
        {
            DigitalSig dSig = new DigitalSig();
            string pdfPath = @"";

            byte[] pdf = File.ReadAllBytes(pdfPath);

            SignInformation signInfo = setSignInformation();
            byte[] result = dSig.Sign(pdf, signInfo);

            string outputPath = @"";
            File.WriteAllBytes(outputPath, result);
        }

        private static void SignWithLTVEnable()
        {
            DigitalSig dSig = new DigitalSig();
            string pdfPath = @"";

            byte[] pdf = File.ReadAllBytes(pdfPath);
            SignInformation signInfo = setSignInformation();

            string tsaUrl = "";
            signInfo.tsaClient = new TSAClientBouncyCastle(tsaUrl);
            byte[] result = dSig.SignWithLTVEnable(pdf, signInfo);

            string outputPath = @"";
            File.WriteAllBytes(outputPath, result);
        }

        private static void LTVEnable()
        {
            DigitalSig dSig = new DigitalSig();
            string pdfPath = @"";
            byte[] pdf = File.ReadAllBytes(pdfPath);

            string tsaUrl = "";
            ITSAClient tsaClient = new TSAClientBouncyCastle(tsaUrl);
            byte[] result = dSig.LTVEnable(pdf,tsaClient);

            string outputPath = @"";
            File.WriteAllBytes(outputPath, result);
        }

        private static void VerifyDigitalSig()
        {
            DigitalSig dSig = new DigitalSig();
            string pdfPath = @"";
            byte[] pdf = File.ReadAllBytes(pdfPath);
            string signatureName = "";

            try
            {
                dSig.VerifyDigitalSig(pdf, signatureName);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }
    }
}
