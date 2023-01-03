using API.ResponseModel.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.ResponseModel.ResponseMessage
{
    public class GenerateResponseMessage
    {
        public ResponseViewModel GetSuccessfullExecutionMessage()
        {
            var model = new ResponseViewModel();
            model.Processed_Code = "200";
            model.Processed_Msg = "Process Successfull";
            return model;
        }

        public ResponseViewModel GetSuccessfullBillPayment()
        {
            var model = new ResponseViewModel();
           // model.Processed_Code = "";
            model.Processed_Code = "200";
            model.Processed_Msg = "Your Bill Payment is Successfull";
            return model;
        }
        public ResponseViewModel GetSuccessfullEmptyCodeBillPayment()
        {
            var model = new ResponseViewModel();
             model.Processed_Code = "";
            //model.Processed_Code = "200";
            model.Processed_Msg = "Your Bill Payment is Successfull";
            return model;
        }

        public ResponseViewModel GetInProgressExecutionMessage()
        {
            var model = new ResponseViewModel();
            model.Processed_Code = "201";
            model.Processed_Msg = "Working";
            return model;
        }

        public ResponseViewModel GetErrorExecutionMessage(string exception)
        {
            var model = new ResponseViewModel();
            if (exception.Contains('~'))
            {
                var exceptionMessage = exception.Split('~');
                //model.Processed_Code = "";
                 model.Processed_Code = exceptionMessage[1].ToString();
                model.Processed_Msg = exceptionMessage[0].ToString();
            }            
            else
            {
                //model.Processed_Code = "";
                model.Processed_Code = "503";
                model.Processed_Msg = "Unspecified Error, Please contact office for details";
            }
            return model;
        }
        public ResponseViewModel GetErrorBillPaymentExecutionMessage(string exception)
        {
            var model = new ResponseViewModel();
            if (exception.Contains('~'))
            {
                var exceptionMessage = exception.Split('~');
                model.Processed_Code = "";
                // model.Processed_Code = exceptionMessage[1].ToString();
                model.Processed_Msg = exceptionMessage[0].ToString();
            }
            else
            {
                model.Processed_Code = "";
               // model.Processed_Code = "503";
                model.Processed_Msg = "Unspecified Error, Please contact office for details";
            }
            return model;
        }
        public ResponseViewModel GetCheckBalance(List<CheckBillViewModel> data)
        {           
            var model = new ResponseViewModel();
            model.Processed_Code = data[0].ErrorCode;
            model.Processed_Msg = data[0].ErrorMessage;
            return model;
        }

        public ResponseViewModel GetDateValidationError()
        {
            var model = new ResponseViewModel();
            model.Processed_Code = "300";
            model.Processed_Msg = "Please Validate Billing Date";
            return model;
        }

        public ResponseViewModel GetNumericValidationError()
        {
            var model = new ResponseViewModel();
            model.Processed_Code = "400";
            model.Processed_Msg = "Please Validate Amount";
            return model;
        }
    }
}
