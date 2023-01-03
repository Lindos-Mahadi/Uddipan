using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.ResponseModel.ResponseMessage
{
    public class ValidateInput
    {
        public bool CheckValidDate(string date)
        {
            DateTime temp;
            if (DateTime.TryParse(date, out temp))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool CheckValidNumeric (string amount)
        {
            decimal n;
            bool isNumeric = decimal.TryParse(amount, out n);
            if(isNumeric==true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
