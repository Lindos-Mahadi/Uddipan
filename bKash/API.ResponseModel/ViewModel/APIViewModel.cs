using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.ResponseModel.ViewModel
{
    public class APIViewModel 
    {
        public APIViewModel()
        {
            Username = string.Empty;
            Password = string.Empty;
            Acc_No = string.Empty;
            Bill_Date = string.Empty;
            Amount = string.Empty;
            Mobile_No = string.Empty;
            Trxid = string.Empty;
            bKashAccountNo = string.Empty;
    }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Acc_No { get; set; }
        public string Bill_Date { get; set; } //
        public string Amount { get; set; }
        public string Mobile_No { get; set; } //To Identify which branch transaction
        public string Trxid { get; set; } //BKash TransactionID, so that gbanker can send response Asynchronous
        public string bKashAccountNo { get; set; }
    }
}
