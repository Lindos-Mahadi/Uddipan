using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.ResponseModel.ViewModel
{
    public class MemberBalanceInfo
    {
        public decimal Payable { get; set; }
        public decimal OutStanding { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
    }
}
