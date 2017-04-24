using iTextSharp.text.xml.xmp;
using System;
using System.Collections.Generic;
using System.Text;

namespace TedaLibrary
{
    class ElectronicDocumentSchema : XmpSchema
    {
        static string XMLNS = "xmlns:ed=\"urn:etda:pdfa:ElectronicDocument:1p0:standard#\"";

        static string DocumentFileName = "ed:DocumentFileName";
        static string DocumentVersion = "ed:DocumentVersion";
        static string DocumentReferenceID = "ed:DocumentReferenceID";
        static string DocumentOID = "ed:DocumentOID";

        public ElectronicDocumentSchema(string xmlns) : base(ElectronicDocumentSchema.XMLNS)
        {
            ;
        }

        public static ElectronicDocumentSchema generateED(string documentFilename, string documentVersion, string documentReferenceID,
        string documentOID)
        {
            ElectronicDocumentSchema ed = new ElectronicDocumentSchema("ed");
            ed.AddProperty(DocumentFileName, documentFilename);
            ed.AddProperty(DocumentVersion, documentVersion);
            ed.AddProperty(DocumentReferenceID, documentReferenceID);
            ed.AddProperty(DocumentOID, documentOID);
            return ed;
        }
    }
}
