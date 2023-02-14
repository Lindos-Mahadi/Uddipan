using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.HtmlControls;

namespace gBanker.Web.ViewModels
{
    public class SupportingDocumentUploadModel
    {
        public string FileName { get; set; }
        public string FileType { get; set; }
        public string File { get; set; }
        public string PropertyName { get; set; }
    }
}