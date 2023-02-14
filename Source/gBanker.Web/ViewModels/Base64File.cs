using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace gBanker.Web.ViewModels
{
    public class Base64File
    {
        public string MimeType { get; set; }
        public string Data { get; set; }
        public byte[] DataBytes { get; set; }
    }
}