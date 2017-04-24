using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iTextSharp.text;

namespace TedaLibrary.Interface
{
    interface ISecurity
    {
        /// <summary>
        /// Add Micro Text to pdf file
        /// </summary>
        /// <param name="pdf"></param>
        /// <param name="text"></param>
        /// <param name="location"></param>
        /// <param name="font"></param>
        /// <param name="fontSize"></param>
        /// <returns></returns>
        byte[] AddMicroText(byte[] pdf,string text, Rectangle location,Font font,int fontSize);

        /// <summary>
        /// Add Wattermark to pdf file
        /// </summary>
        /// <param name="pdf"></param>
        /// <param name="text"></param>
        /// <param name="location"></param>
        /// <param name="font"></param>
        /// <param name="fontSize"></param>
        /// <returns></returns>
        byte[] AddWatermark(byte[] pdf,string text,Rectangle location,Font font,int fontSize);
    }
}
