﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TedaLibrary.Object;
namespace TedaLibrary.Object
{
    public class EAttactment
    {
        public static string DATA = "Data";
        public static string SOURCE = "Source";
        public static string ALTERNATIVE = "Alternative";
        public static string SUPPLEMENT = "Supplement";
        public static string UNSPECIFIED = "Unspecified";

        public EAttactment(string attachmentName, string contentInformationXMLPath, string attachmentMIME, string attachType, string attachmentDescription)
        {
            this.contentInformationXMLPath = contentInformationXMLPath;
            this.attachmentName = attachmentName;
            this.attachmentMIME = attachmentMIME;
            this.attachmentDescription = attachmentDescription;
        }

        public EAttactment()
        {

        }
        // XML File
        //public string attachmentPath { get; set; }
        public string attachmentName { get; set; }
        public string attachmentMIME { get; set; }
        public string attachmentType { get; set; }
        public string attachmentDescription { get; set; }

        public string contentInformationXMLPath { get; set; }
        public string documentVersion { get; set; }
        public string documentID { get; set; }
        public string documentOID { get; set; }

        // Others File 
        public List<FileAttachment> fileAttachments { get; set; }

        // Icc Profile
        public OutputIntents outputIntents { get; set; }

        //public bool useDefaultIntent { get; set; }
    }
}
