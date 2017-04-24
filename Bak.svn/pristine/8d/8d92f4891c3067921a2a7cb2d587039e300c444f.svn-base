using System.Collections.Generic;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.X509;
using iTextSharp.text.pdf.security;
using TedaLibrary.Object;

namespace TedaLibrary.Interface
{
    interface IDigitalSig
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pdf">Pdf data in byte array</param>
        /// <param name="signInfo">data use for sign and do LTV</param>
        /// <returns>Signed pdf data in byte array</returns>
        byte[] Sign(byte[] pdf, SignInformation signInfo);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pdf">Pdf data in byte array</param>
        /// <param name="signInfo">data use for sign and do LTV</param>
        /// <returns>Signed pdf data in byte array with LTV Enable</returns>
        //byte[] SignWithLTVEnable(byte[] pdf, AsymmetricKeyParameter privateKey, ICollection<X509Certificate> chain, List<ICrlClient> crl, ITSAClient tsaClient);
        byte[] SignWithLTVEnable(byte[] pdf, SignInformation signInfo);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pdf">Pdf data in byte array</param>
        /// <param name="tsatsaClient">TimeStamp client object</param>
        /// <returns>Pdf data in byte array with LTV Enable</returns>
        byte[] LTVEnable(byte[] pdf,ITSAClient tsatsaClient);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pdf">Verified Pdf data in byte array</param>
        /// <param name="signatureName"></param>
        void VerifyDigitalSig(byte[] pdf,string signatureName);
    }
}
