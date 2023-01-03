using API.ContextRepository;
using API.DataModel;
using API.Repository;
using API.ResponseModel.ResponseMessage;
using API.ResponseModel.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Data.SqlClient;
using System.Web.Http;

namespace BKashAPI.Controllers
{
    public class gBankerController : ApiController
    {
        APIContextUow UOW;
        IRepository<BKashAPIParking> bKashRepository;
        IRepository<BKashUserCheck> bKashUserCheckRepository;
        IDbHelper DbHelper;

        public gBankerController()
        {
            this.UOW = new APIContextUow();
            this.DbHelper = new MsSqlDbHelper();
            this.bKashRepository = new APIContextRepository<BKashAPIParking>(this.UOW);
            this.bKashUserCheckRepository = new APIContextRepository<BKashUserCheck>(this.UOW);
        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [System.Web.Http.HttpPost]
        public string Test()
        {
            var abc = bKashRepository.All().ToList();
            return "Test Done";
        }

        private BKashAPIParking GenerateAPIParking(APIViewModel model, int requestFor)
        {
            var data = new BKashAPIParking();
            data.AccountNo = model.Acc_No;
            data.BillDate = model.Bill_Date == string.Empty ? DateTime.UtcNow : Convert.ToDateTime(model.Bill_Date);
            data.Amount = model.Amount == string.Empty ? 0 : Convert.ToDecimal(model.Amount);
            data.MobileNo = model.Mobile_No;
            data.TrxId = model.Trxid;
            data.RequestFor = requestFor; // Check Bill
            data.CreateDate = DateTime.UtcNow;
            data.MessageResponse = string.Empty;
            data.bKashAccountNo = model.bKashAccountNo;
            bKashRepository.Add(data);
            UOW.Save();
            return data;
        }
        private BKashAPIParking GetAPIParking(APIViewModel model, int requestFor)
        {
            var data = new BKashAPIParking();
            data.AccountNo = model.Acc_No;
            data.BillDate = model.Bill_Date == string.Empty ? DateTime.UtcNow : Convert.ToDateTime(model.Bill_Date);
            data.Amount = model.Amount == string.Empty ? 0 : Convert.ToDecimal(model.Amount);
            data.MobileNo = model.Mobile_No;
            data.TrxId = model.Trxid;
            data.RequestFor = requestFor; // Check Bill
            data.CreateDate = DateTime.UtcNow;
            data.MessageResponse = string.Empty;
            data.bKashAccountNo = model.bKashAccountNo;
            
            return data;
        }



        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [System.Web.Http.HttpPost]
        public ResponseViewModel CheckBill(APIViewModel model)
        {
            try
            {                
                var checkValidUser = bKashUserCheckRepository.GetMany(p => p.UserId == model.Username && p.PasswordString == model.Password).ToList();

                if(checkValidUser.Count==0)
                {
                    return new GenerateResponseMessage().GetErrorExecutionMessage("Invalid user or password found");
                }

                var isValidDate = true;
                var isValidNumeric = true;

                if (model.Bill_Date != string.Empty) 
                {
                    isValidDate=  new ValidateInput().CheckValidDate(model.Bill_Date);                    
                }  
                if(isValidDate==false)
                {
                    return new GenerateResponseMessage().GetDateValidationError();
                }
                if(model.Amount!= string.Empty)
                {
                    isValidNumeric = new ValidateInput().CheckValidNumeric(model.Amount);                   
                }
                if(isValidNumeric==false)
                {
                    return new GenerateResponseMessage().GetNumericValidationError();
                }

                else
                {
                    var data = GenerateAPIParking(model, 1);
                    var paramValues = new Dictionary<string, object>();
                    paramValues.Add("MobileNo", data.MobileNo);
                    paramValues.Add("LoanAccountNo", data.AccountNo);
                    paramValues.Add("TrxId", data.TrxId);
                    paramValues.Add("bKashAccountNo", data.bKashAccountNo);
                    DataTable result = DbHelper.ExecuteQuery(UOW.Context.Database, System.Data.CommandType.StoredProcedure, "Rpt_MemberBalanceInfoApi", paramValues);
                    var spData = new ConvertDataTabletoList().ConvertToList<CheckBillViewModel>(result).ToList();
                    return new GenerateResponseMessage().GetCheckBalance(spData);
                }                      
               
            }
            catch (Exception ex)
            {
                return new GenerateResponseMessage().GetErrorExecutionMessage(ex.Message==null? ex.ToString() : ex.Message.ToString());
            }

        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [System.Web.Http.HttpPost]
        public ResponseViewModel BillPayment(APIViewModel model)
        {
            try
            {
                var billdata = Convert.ToDateTime(model.Bill_Date);

                var checkValidUser = bKashUserCheckRepository.GetMany(p => p.UserId == model.Username && p.PasswordString == model.Password).ToList();

                if (checkValidUser.Count == 0)
                {
                    return new GenerateResponseMessage().GetErrorExecutionMessage("Invalid user or password found");
                }

                var isValidDate = true;
                var isValidNumeric = true;

                if (model.Bill_Date != string.Empty)
                {
                    isValidDate = new ValidateInput().CheckValidDate(model.Bill_Date);
                }
                if (isValidDate == false)
                {
                    return new GenerateResponseMessage().GetDateValidationError();
                }
                if (model.Amount != string.Empty)
                {
                    isValidNumeric = new ValidateInput().CheckValidNumeric(model.Amount);
                }
                if (isValidNumeric == false)
                {
                    return new GenerateResponseMessage().GetNumericValidationError();
                }
                else
                {
                    var data = GenerateAPIParking(model, 2);
                    var paramValues = new Dictionary<string, object>();
                    paramValues.Add("MobileNo", data.MobileNo);
                    paramValues.Add("MemberCode", string.Empty);
                    paramValues.Add("AccountNo", data.AccountNo);
                    paramValues.Add("Amount", Convert.ToDecimal(data.Amount));
                    paramValues.Add("BillDate", Convert.ToDateTime(data.BillDate));
                    paramValues.Add("TrxId", data.TrxId);
                    paramValues.Add("bKashAccountNo", data.bKashAccountNo);
                    DataTable result = DbHelper.ExecuteQuery(UOW.Context.Database, System.Data.CommandType.StoredProcedure, "getAPiDetails", paramValues);


                    //string sqlSql = "select Count(TrxId)NoHit  from BKashAPIParking where TrxId='" + data.TrxId + "'";
                    //string server = "192.192.192.171\\MFI";
                    //string db = "gBankerBUroBkash";
                    //string userID = "bKashGC";
                    //string pwd = "bK@shGC#321";
                    //string connSting = "Server="+ server +";database="+ db  + ";user id="+userID +";password="+ pwd +""; 
                    //SqlDataAdapter sda = new SqlDataAdapter(sqlSql, connSting);
                    //DataSet ds = new DataSet();
                    //sda.Fill(ds, "BKashAPIParking");
                    //int noHit = Convert.ToInt16(ds.Tables[0].Rows[0]["NoHit"].ToString());
                    //if (noHit == 1)
                    //{

                    //    return new GenerateResponseMessage().GetSuccessfullEmptyCodeBillPayment();

                    //}
                    //else


                        return new GenerateResponseMessage().GetSuccessfullBillPayment();
                }               
            }
            catch (Exception ex)
            {
                //var data = GetAPIParking(model, 2);
                var message = ex.Message;
                //string sqlSql = "select Count(TrxId) NoHit   from BKashAPIParking where TrxId='" + data.TrxId + "'";
                //string server = "192.192.192.171\\MFI";
                //string db = "gBankerBUroBkash";
                //string userID = "bKashGC";
                //string pwd = "bK@shGC#321";
                //string connSting = "Server=" + server + ";database=" + db + ";User id=" + userID + ";password=" + pwd + "";
                //SqlDataAdapter sda = new SqlDataAdapter(sqlSql, connSting);
                //DataSet ds = new DataSet();
                //sda.Fill(ds, "BKashAPIParking");
                //int noHit = Convert.ToInt16(ds.Tables[0].Rows[0]["NoHit"].ToString());
                //if (noHit == 1)
                //{
                //    return new GenerateResponseMessage().GetErrorBillPaymentExecutionMessage(message);

                //}
                //else
                //    //return new GenerateResponseMessage().GetErrorBillPaymentExecutionMessage(message);

                return new GenerateResponseMessage().GetErrorExecutionMessage(message);
            }
        }
    }
}
