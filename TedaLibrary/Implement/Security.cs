using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iTextSharp.text;
using TedaLibrary.Interface;

namespace TedaLibrary.Implement
{
    class Security : ISecurity
    {
        public byte[] AddMicroText(byte[] pdf, string text, Rectangle location, Font font, int fontSize)
        {
            throw new NotImplementedException();
        }

        public byte[] AddWatermark(byte[] pdf, string text, Rectangle location, Font font, int fontSize)
        {
            throw new NotImplementedException();
        }
    }
}
