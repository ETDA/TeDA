using Org.BouncyCastle.Pkcs;
using System.Security.Cryptography.X509Certificates;
using iTextSharp.text.pdf.security;
using System.Collections.Generic;

namespace TedaLibrary.Interface
{
    interface IDigitalSigUtil
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="keyStorePath">Path to keystore file</param>
        /// <param name="keyStorePassword">Password use for open keystore file</param>
        /// <returns>Pkcs12 Store</returns>
        Pkcs12Store GetKeyStore(string keyStorePath,string keyStorePassword);

        /// <summary>
        /// 
        /// </summary>
        /// <returns>X509 Certificate</returns>
        X509Certificate2 GetX509Certificate2();

        /// <summary>
        /// to get CrlList from certificate chain
        /// </summary>
        /// <returns>Crl List</returns>
        List<ICrlClient> GetCrlList(ICollection<Org.BouncyCastle.X509.X509Certificate> chain);
    }
}
