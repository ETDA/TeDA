using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Org.BouncyCastle.Pkcs;
using TedaLibrary.Interface;
using iTextSharp.text.pdf.security;
using System.IO;

namespace TedaLibrary.Implement
{
    public class DigitalSigUtil : IDigitalSigUtil
    {
        /// <summary>
        ///     To get X509Certificate
        /// </summary>
        /// <returns>X509 key store object </returns>
        public X509Certificate2 GetX509Certificate2()
        {
            X509Store store = new X509Store(StoreName.My, StoreLocation.CurrentUser);

            store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly); //Openstore with mode
            X509Certificate2Collection collection = (X509Certificate2Collection)store.Certificates; // Get collection from storname.my
            X509Certificate2Collection fcollection = (X509Certificate2Collection)collection.Find(X509FindType.FindByTimeValid, DateTime.Now, false);//show only cert which still timevalid
            X509Certificate2Collection scollection = X509Certificate2UI.SelectFromCollection
                (fcollection
                , "Certificate Selection"
                , "Select a certificate from the following list to get information on that certificate"
                , X509SelectionFlag.SingleSelection);//Opendialog selection
            X509Certificate2 cert = scollection[0];// Select first certificate from root 
            return cert;
        }

        /// <summary>
        ///     To
        /// </summary>
        /// <param name="keyStorePath">Path to key store file</param>
        /// <param name="keyStorePassword">Key store's password</param>
        /// <returns>Pkcs12 key store object</returns>
        public Pkcs12Store GetKeyStore(string keyStorePath, string keyStorePassword)
        {
            Pkcs12Store keystore = new Pkcs12Store(); // Create Key store
            keystore.Load(new FileStream(keyStorePath, FileMode.Open), keyStorePassword.ToCharArray()); // Load key using filestream via path and password
            return keystore;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="chain"></param>
        /// <returns></returns>
        public List<ICrlClient> GetCrlList(ICollection<Org.BouncyCastle.X509.X509Certificate> chain)
        {
            List<ICrlClient> crlList = new List<ICrlClient>();
            ICrlClient crlOnline = new CrlClientOnline(chain);
            crlList.Add(crlOnline);
            return crlList;
        }
    }
}
