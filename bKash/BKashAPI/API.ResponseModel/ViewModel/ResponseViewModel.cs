using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.ResponseModel.ViewModel
{
    public class ResponseViewModel
    {
        public ResponseViewModel()
        {
            Processed_Code = string.Empty;
            Processed_Msg = string.Empty;
            Consumer_Name = string.Empty;
            Trxid = string.Empty;

        }
        public string Processed_Code { get; set; }
        public string Processed_Msg { get; set; }
        public string Consumer_Name { get; set; }
        public string Trxid { get; set; } // TransactionId to send message to end user from BKash.
    }
}
