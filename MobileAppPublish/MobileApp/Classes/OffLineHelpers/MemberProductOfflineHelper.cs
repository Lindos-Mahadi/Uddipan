using System.Collections.Generic;
using System.Linq;
using Android.Content;
using Android.Database.Sqlite;
using Android.Database;

namespace PMS.Droid.Classes.OffLineHelpers
{
    public class MemberProductOfflineHelper : SQLiteOpenHelper
    {
        private static string APP_DATABASENAME = OfflineDBConstants.APP_DATABASE_NAME;
        private static int APP_DATABASE_VERSION = OfflineDBConstants.APP_DATABASE_VERSION;
        private static string TABLE_NAME = OfflineDBConstants.TABLE_MEMBER_PRODUCT;

        public MemberProductOfflineHelper(Context ctx) :
            base(ctx, APP_DATABASENAME, null, APP_DATABASE_VERSION)
        {

        }

        public override void OnCreate(SQLiteDatabase db)
        {
        }

        public override void OnUpgrade(SQLiteDatabase db, int oldVersion, int newVersion)
        {
            db.ExecSQL(string.Format("DROP TABLE IF EXISTS {0}", TABLE_NAME));
            OnCreate(db);
        }

        public bool IsProductExist(long summaryId, int productType)
        {
            SQLiteDatabase db = this.ReadableDatabase;
            ICursor res = db.RawQuery(string.Format("select MemberID, ProductID, SummaryID from {0} where SummaryID={1} AND  ProductType ={2}", TABLE_NAME, summaryId, productType), null);
            return res.Count > 0;
        }
        public void AddNew(MemberProductModel model)
        {
            SQLiteDatabase db = this.WritableDatabase;
            ContentValues vals = new ContentValues();
            vals.Put("MemberID", model.MemberID);            
            vals.Put("ProductID", model.ProductID);
            vals.Put("ProductName", model.ProductName);
            vals.Put("ProductType", model.ProductType);
            vals.Put("LoanRecovery", model.LoanRecovery);
            vals.Put("Recoverable", model.Recoverable);
            vals.Put("Balance", model.Balance);
            vals.Put("PrinBalance", model.PrinBalance);
            vals.Put("SerBalance", model.SerBalance);
            vals.Put("InstallmentNo", model.InstallmentNo);
            vals.Put("TrxType", model.TrxType);
            vals.Put("SummaryID", model.SummaryID);
            vals.Put("InterestCalculationMethod", model.InterestCalculationMethod);
            vals.Put("Duration", model.Duration);
            vals.Put("DurationOverLoanDue", model.DurationOverLoanDue);
            vals.Put("DurationOverIntDue", model.DurationOverIntDue);
            vals.Put("LoanDue", model.LoanDue);
            vals.Put("IntDue", model.IntDue);
            vals.Put("CumIntCharge", model.CumIntCharge);
            vals.Put("CumInterestPaid", model.CumInterestPaid);
            vals.Put("PrincipalLoan", model.PrincipalLoan);
            vals.Put("LoanRepaid", model.LoanRepaid);
            vals.Put("IntCharge", model.IntCharge);
            vals.Put("NewDue", model.NewDue);
            vals.Put("MainProductCode", model.MainProductCode);
            vals.Put("CenterID", model.CenterID);
            vals.Put("MemberName", model.MemberName);
            vals.Put("Doc", model.Doc);
            vals.Put("OrgID", model.OrgID);
            vals.Put("accountNo", model.accountNo);
            vals.Put("fine", model.fine);
            vals.Put("PersonalSaving", model.PersonalSaving);
            vals.Put("PersonalWithdraw", model.PersonalWithdraw);
            db.Insert(TABLE_NAME, null, vals);
        }

        public List<MemberProductModel> GetProductsByMember(long memberID)
        {

            SQLiteDatabase db = this.ReadableDatabase;

            ICursor c = db.Query(TABLE_NAME, new string[] { "MemberID", "ProductID", "ProductName","ProductType", "LoanRecovery", "Recoverable", "Balance", "InstallmentNo" ,"TrxType","SummaryID",
                "InterestCalculationMethod","Duration","DurationOverLoanDue","DurationOverIntDue","LoanDue","IntDue","CumIntCharge","CumInterestPaid","PrincipalLoan","LoanRepaid","NewDue","MainProductCode",
                "accountNo","fine"
            }, "MemberID=?", new string[] { memberID.ToString() }, null, null, null, null);

            var lst = new List<MemberProductModel>();

            while (c.MoveToNext())
            {
                lst.Add(new MemberProductModel
                {
                    MemberID = c.GetInt(0),
                    ProductID = c.GetInt(1),
                    ProductName = c.GetString(2),
                    ProductType = c.GetInt(3),
                    LoanRecovery = c.GetDouble(4),
                    Recoverable = c.GetDouble(5),
                    Balance = c.GetDouble(6),
                    InstallmentNo = c.GetInt(7),
                    TrxType = c.GetInt(8),
                    SummaryID = c.GetLong(9),
                    InterestCalculationMethod = c.GetString(10),
                    Duration = c.GetInt(11),
                    DurationOverLoanDue = c.GetDouble(12),
                    DurationOverIntDue = c.GetDouble(13),
                    LoanDue = c.GetDouble(14),
                    IntDue = c.GetDouble(15),
                    CumIntCharge = c.GetDouble(16),
                    CumInterestPaid = c.GetDouble(17),
                    PrincipalLoan = c.GetDouble(18),
                    LoanRepaid = c.GetDouble(19),
                    NewDue = c.GetDouble(20),
                    MainProductCode = c.GetString(21),
                    accountNo = c.GetString(c.GetColumnIndexOrThrow("accountNo")),
                    fine = c.GetColumnIndexOrThrow("fine")
                });
            }

            c.Close();
            db.Close();

            return lst;
        }

        public MemberProductModel GetProductByMemberAndName(string productName, long membreID)
        {
            SQLiteDatabase db = this.ReadableDatabase;
            ICursor res = db.RawQuery(string.Format("select MemberID, ProductID, ProductName,ProductType, LoanRecovery, Recoverable, Balance, InstallmentNo, TrxType, SummaryID,accountNo,fine from {0} where ProductName='{1}' AND  MemberID ={2}", TABLE_NAME, productName, membreID), null);
            if (res.MoveToFirst())
            {
                return new MemberProductModel
                {
                    MemberID = res.GetInt(0),
                    ProductID = res.GetInt(1),
                    ProductName = res.GetString(2),
                    ProductType = res.GetInt(3),
                    LoanRecovery = res.GetDouble(4),
                    Recoverable = res.GetDouble(5),
                    Balance = res.GetDouble(6),
                    InstallmentNo = res.GetInt(7),
                    TrxType = res.GetInt(8),
                    SummaryID = res.GetLong(9),
                    accountNo = res.GetString(res.GetColumnIndexOrThrow("accountNo")),
                    fine = res.GetColumnIndexOrThrow("fine")
                };
            }
            return null;
        }
        public List<MemberProductModel> GetMemberProducts(long membreID, int trxType = 1)
        {
            SQLiteDatabase db = this.ReadableDatabase;
            ICursor res = db.RawQuery(string.Format(@"select MemberID, ProductID, ProductName,ProductType, LoanRecovery, Recoverable, Balance, InstallmentNo,TrxType, SummaryID,
                 InterestCalculationMethod,Duration,DurationOverLoanDue,DurationOverIntDue,LoanDue,IntDue,CumIntCharge,CumInterestPaid,PrincipalLoan,LoanRepaid, PrinBalance,
                 SerBalance, IntCharge, NewDue, MainProductCode, Doc, OrgID, accountNo, fine, PersonalSaving, PersonalWithdraw  
                from {0} where MemberID ={1} AND TrxType = {2}", TABLE_NAME, membreID, trxType), null);
            var lst = new List<MemberProductModel>();

            while (res.MoveToNext())
            {
                lst.Add(new MemberProductModel
                {
                    MemberID = res.GetInt(0),
                    ProductID = res.GetInt(1),
                    ProductName = res.GetString(2),
                    ProductType = res.GetInt(3),
                    LoanRecovery = res.GetDouble(4),
                    Recoverable = res.GetDouble(5),
                    Balance = res.GetDouble(6),
                    InstallmentNo = res.GetInt(7),
                    TodayCollectionAmount = res.GetDouble(4),
                    TrxType = res.GetInt(8),
                    SummaryID = res.GetLong(9),
                    InterestCalculationMethod = res.GetString(10),
                    Duration = res.GetInt(11),
                    DurationOverLoanDue = res.GetDouble(12),
                    DurationOverIntDue = res.GetDouble(13),
                    LoanDue = res.GetDouble(14),
                    IntDue = res.GetDouble(15),
                    CumIntCharge = res.GetDouble(16),
                    CumInterestPaid = res.GetDouble(17),
                    PrincipalLoan = res.GetDouble(18),
                    LoanRepaid = res.GetDouble(19),
                    PrinBalance = res.GetDouble(20),
                    SerBalance = res.GetDouble(21),
                    IntCharge = res.GetDouble(22),
                    NewDue = res.GetDouble(23),
                    MainProductCode = res.GetString(24),
                    Doc = res.GetInt(25),
                    OrgID = res.GetInt(26),
                    accountNo = res.GetString(res.GetColumnIndexOrThrow("accountNo")),
                    fine = res.GetDouble(28),
                    PersonalSaving = res.GetDouble(29),
                    PersonalWithdraw = res.GetDouble(30)
                });
            }
            res.Close();
            db.Close();
            return lst;
        }
        public List<MemberProductModel> GetCollectionRecords(int centerId, int trxType = 1)
        {
            SQLiteDatabase db = this.ReadableDatabase;
            ICursor res = db.RawQuery(string.Format(@"select MemberID, ProductID, ProductName,ProductType, LoanRecovery, Recoverable, Balance, InstallmentNo,TrxType, SummaryID,
                 InterestCalculationMethod,Duration,DurationOverLoanDue,DurationOverIntDue,LoanDue,IntDue,CumIntCharge,CumInterestPaid,PrincipalLoan,LoanRepaid, PrinBalance, SerBalance, IntCharge, NewDue, MainProductCode, MemberName, accountNo, fine 
                from {0} where TrxType = {1} AND CenterID = {2}", TABLE_NAME, trxType, centerId), null);
            var lst = new List<MemberProductModel>();

            while (res.MoveToNext())
            {
                var obj = new MemberProductModel
                {
                    MemberID = res.GetInt(0),
                    ProductID = res.GetInt(1),
                    ProductName = res.GetString(2),
                    ProductType = res.GetInt(3),
                    LoanRecovery = res.GetDouble(4),
                    Recoverable = res.GetDouble(5),
                    Balance = res.GetDouble(6),
                    InstallmentNo = res.GetInt(7),
                    TodayCollectionAmount = res.GetDouble(4),
                    TrxType = res.GetInt(8),
                    SummaryID = res.GetLong(9),
                    InterestCalculationMethod = res.GetString(10),
                    Duration = res.GetInt(11),
                    DurationOverLoanDue = res.GetDouble(12),
                    DurationOverIntDue = res.GetDouble(13),
                    LoanDue = res.GetDouble(14),
                    IntDue = res.GetDouble(15),
                    CumIntCharge = res.GetDouble(16),
                    CumInterestPaid = res.GetDouble(17),
                    PrincipalLoan = res.GetDouble(18),
                    LoanRepaid = res.GetDouble(19),
                    PrinBalance = res.GetDouble(20),
                    SerBalance = res.GetDouble(21),
                    IntCharge = res.GetDouble(22),
                    NewDue = res.GetDouble(23),
                    MainProductCode = res.GetString(24),
                    MemberName = res.GetString(25),
                    accountNo = res.GetString(res.GetColumnIndexOrThrow("accountNo")),
                    fine = res.GetColumnIndexOrThrow("fine")
                };
                lst.Add(obj);
            }
            res.Close();
            db.Close();

            var newList = new List<MemberProductModel>();
            var prods = lst.Select(d => new { d.ProductID, d.ProductName }).Distinct();
            foreach (var pr in prods)
            {
                var prodItems = lst.Where(w => w.ProductID == pr.ProductID).ToList();
                newList.Add(new OffLineHelpers.MemberProductModel() { MemberID = 0, ProductName = pr.ProductName });
                newList.AddRange(prodItems);
            }

            return newList;
        }

        public List<CollectionNewModel> GetCollectionRecordsGrouped(int centerId, int trxType = 1)
        {
            SQLiteDatabase db = this.ReadableDatabase;
            ICursor res = db.RawQuery(string.Format(@"select MemberID, ProductID, ProductName,ProductType, LoanRecovery, Recoverable, Balance, InstallmentNo,TrxType, SummaryID,
                 InterestCalculationMethod,Duration,DurationOverLoanDue,DurationOverIntDue,LoanDue,IntDue,CumIntCharge,CumInterestPaid,PrincipalLoan,LoanRepaid, PrinBalance, SerBalance, IntCharge, NewDue, MainProductCode, MemberName, accountNo, fine 
                from {0} where TrxType = {1} AND CenterID = {2}", TABLE_NAME, trxType, centerId), null);
            var lst = new List<MemberProductModel>();

            while (res.MoveToNext())
            {
                var obj = new MemberProductModel
                {
                    MemberID = res.GetInt(0),
                    ProductID = res.GetInt(1),
                    ProductName = res.GetString(2),
                    ProductType = res.GetInt(3),
                    LoanRecovery = res.GetDouble(4),
                    Recoverable = res.GetDouble(5),
                    Balance = res.GetDouble(6),
                    InstallmentNo = res.GetInt(7),
                    TodayCollectionAmount = res.GetDouble(4),
                    TrxType = res.GetInt(8),
                    SummaryID = res.GetLong(9),
                    InterestCalculationMethod = res.GetString(10),
                    Duration = res.GetInt(11),
                    DurationOverLoanDue = res.GetDouble(12),
                    DurationOverIntDue = res.GetDouble(13),
                    LoanDue = res.GetDouble(14),
                    IntDue = res.GetDouble(15),
                    CumIntCharge = res.GetDouble(16),
                    CumInterestPaid = res.GetDouble(17),
                    PrincipalLoan = res.GetDouble(18),
                    LoanRepaid = res.GetDouble(19),
                    PrinBalance = res.GetDouble(20),
                    SerBalance = res.GetDouble(21),
                    IntCharge = res.GetDouble(22),
                    NewDue = res.GetDouble(23),
                    MainProductCode = res.GetString(24),
                    MemberName = res.GetString(25),
                    accountNo = res.GetString(res.GetColumnIndexOrThrow("accountNo")),
                    fine = res.GetColumnIndexOrThrow("fine")
                };
                lst.Add(obj);
            }
            res.Close();
            db.Close();

            var newList = new List<CollectionNewModel>();
            var prods = lst.Select(d => new { d.ProductID, d.ProductName }).Distinct();
            foreach (var pr in prods)
            {
                var prodItems = lst.Where(w => w.ProductID == pr.ProductID).ToList();
                var colNew = new CollectionNewModel() { ProductName = pr.ProductName, MemberProducts = prodItems };
                newList.Add(colNew);
            }
            return newList;
        }

        public List<CollectionNewModel> GetCollectionRecordsAll(int centerId, int trxType = 1)
        {
            SQLiteDatabase db = this.ReadableDatabase;

            var sql = string.Format(@"select M.MemberID, M.ProductID, ProductName,ProductType, LoanRecovery, Recoverable, Balance, InstallmentNo, M.TrxType, SummaryID,
                 InterestCalculationMethod,Duration,DurationOverLoanDue,DurationOverIntDue,LoanDue,IntDue,CumIntCharge,CumInterestPaid,PrincipalLoan,LoanRepaid, PrinBalance, 
                    SerBalance, IntCharge, NewDue, MainProductCode, MemberName,
                   ifnull(LP.Amount, 0) Amount,  ifnull(LP.MemberID, 0) IsCollected, Doc
                from {0} M  LEFT JOIN (SELECT MemberID, ProductID, TrxType, Amount FROM {3}  WHERE TrxType = {1} AND CenterID = {2}) LP
                    ON M.MemberID = LP.MemberID AND M.ProductID = LP.ProductID
                    AND M.TrxType = LP.TrxType
                    where M.TrxType = {1} AND M.CenterID = {2}", TABLE_NAME, trxType, centerId, OfflineDBConstants.TABLE_LOAN_COLLECTION);

            ICursor res = db.RawQuery(sql, null);
            var lst = new List<MemberProductModel>();

            while (res.MoveToNext())
            {
                var obj = new MemberProductModel
                {
                    MemberID = res.GetInt(0),
                    ProductID = res.GetInt(1),
                    ProductName = res.GetString(2),
                    ProductType = res.GetInt(3),
                    LoanRecovery = res.GetDouble(4),
                    Recoverable = res.GetDouble(5),
                    Balance = res.GetDouble(6),
                    InstallmentNo = res.GetInt(7),
                    TrxType = res.GetInt(8),
                    SummaryID = res.GetLong(9),
                    InterestCalculationMethod = res.GetString(10),
                    Duration = res.GetInt(11),
                    DurationOverLoanDue = res.GetDouble(12),
                    DurationOverIntDue = res.GetDouble(13),
                    LoanDue = res.GetDouble(14),
                    IntDue = res.GetDouble(15),
                    CumIntCharge = res.GetDouble(16),
                    CumInterestPaid = res.GetDouble(17),
                    PrincipalLoan = res.GetDouble(18),
                    LoanRepaid = res.GetDouble(19),
                    PrinBalance = res.GetDouble(20),
                    SerBalance = res.GetDouble(21),
                    IntCharge = res.GetDouble(22),
                    NewDue = res.GetDouble(23),
                    MainProductCode = res.GetString(24),
                    MemberName = res.GetString(25),
                    TodayCollectionAmount = res.GetDouble(26),
                    IsCollected = (res.GetInt(27) != 0),
                    Doc = res.GetInt(28)
                };
                lst.Add(obj);
            }
            res.Close();
            db.Close();
            var newList = new List<CollectionNewModel>();
            var prods = lst.Select(d => new { d.ProductID, d.ProductName, d.MainProductCode, d.ProductType, isAdded = false }).Distinct();
            foreach (var pr in prods)
            {
                if (pr.ProductType == 1)
                {
                    var prodItems = lst.Where(w => w.ProductID == pr.ProductID).ToList();
                    var colNew = new CollectionNewModel() { ProductName = pr.ProductName, MemberProducts = new List<MemberProductModel>() };
                    foreach (var item in prodItems)
                    {
                        var gsItem = lst.Where(w => w.MainProductCode == pr.MainProductCode && w.MemberID == item.MemberID && w.ProductName.StartsWith("21")).FirstOrDefault();
                        if (gsItem != null)
                        {
                            item.GSID = gsItem;
                            item.GSCollected = gsItem.TodayCollectionAmount;
                            item.GSAmount = gsItem.LoanRecovery;
                        }
                        var vsItem = lst.Where(w => w.MainProductCode == pr.MainProductCode && w.MemberID == item.MemberID && w.ProductName.StartsWith("22")).FirstOrDefault();
                        if (vsItem != null)
                        {
                            item.VSID = vsItem;
                            item.VSCollected = vsItem.TodayCollectionAmount;
                            item.VSAmount = vsItem.LoanRecovery;
                        }
                        var ltsItem = lst.Where(w => w.MainProductCode == pr.MainProductCode && w.MemberID == item.MemberID && w.ProductName.StartsWith("23")).FirstOrDefault();
                        if (ltsItem != null)
                        {
                            item.LTSID = ltsItem;
                            item.LTSCollected = ltsItem.TodayCollectionAmount;
                            item.LTSAmount = ltsItem.LoanRecovery;
                        }
                        colNew.MemberProducts.Add(item);
                    }
                    newList.Add(colNew);
                }
            }
            var allAdded = from s in newList
                           from m in s.MemberProducts
                           select new { m.SummaryID };
            var alladdedCount = allAdded.Count();
            var missingSavingCollections = lst.Where(w => allAdded.Count(c => c.SummaryID == w.SummaryID) == 0); //Find only collection items that is not added in prevous step.
            var misingCount = missingSavingCollections.Count();
            var savingsItem = new CollectionNewModel() { ProductName = "Savings", MemberProducts = new List<MemberProductModel>() };
            var missingMembers = missingSavingCollections.Select(s => new { s.MemberID, s.MemberName }).Distinct(); //Find members and create gs vs lts based on member.
            foreach (var member in missingMembers)
            {
                var memberItems = missingSavingCollections.Where(w => w.MemberID == member.MemberID);
                var dummyMemberItem = new MemberProductModel() { ProductType = 99, ProductID = 0, MemberName = member.MemberName, MemberID = member.MemberID };
                foreach (var item in memberItems)
                {
                    if (item.ProductName.StartsWith("21")) //gs item.
                    {
                        dummyMemberItem.GSID = item;
                        dummyMemberItem.GSCollected = item.TodayCollectionAmount;
                        dummyMemberItem.GSAmount = item.LoanRecovery;                       
                    }
                    else if (item.ProductName.StartsWith("22")) //vs item.
                    {
                        dummyMemberItem.VSID = item;
                        dummyMemberItem.VSCollected = item.TodayCollectionAmount;
                        dummyMemberItem.VSAmount = item.LoanRecovery;                       
                    }
                    else if (item.ProductName.StartsWith("23")) //lts item.
                    {
                        dummyMemberItem.LTSID = item;
                        dummyMemberItem.LTSCollected = item.TodayCollectionAmount;
                        dummyMemberItem.LTSAmount = item.LoanRecovery;
                    }
                    if (item.TodayCollectionAmount > 0)
                        dummyMemberItem.IsCollected = true;
                }
                savingsItem.MemberProducts.Add(dummyMemberItem);
            }
            //If there is any savings collection, add this to collection list.
            if (savingsItem.MemberProducts.Count > 0)
                newList.Add(savingsItem);
            return newList;
        }
        public List<MemberProductModel> GetProducts(int trxType = 1)
        {
            SQLiteDatabase db = this.ReadableDatabase;
            ICursor res = db.RawQuery(string.Format(@"select DISTINCT ProductID, ProductName from {0} where ProductType = 1 AND TrxType = {1}", TABLE_NAME, trxType), null);
            var lst = new List<MemberProductModel>() { new MemberProductModel { ProductID = 0, ProductName = "All" } };

            while (res.MoveToNext())
            {
                lst.Add(new MemberProductModel
                {

                    ProductID = res.GetInt(0),
                    ProductName = res.GetString(1)
                });
            }
            return lst;
        }
        public List<MemberProductModel> GetMemberProductsWithdrawal(long membreID)
        {
            SQLiteDatabase db = this.ReadableDatabase;
            ICursor res = db.RawQuery(string.Format(@"select MemberID, ProductID, ProductName,ProductType, LoanRecovery, Recoverable, Balance, InstallmentNo,TrxType, SummaryID,
                 InterestCalculationMethod,Duration,DurationOverLoanDue,DurationOverIntDue,LoanDue,IntDue,CumIntCharge,CumInterestPaid,PrincipalLoan,LoanRepaid, PrinBalance, SerBalance, IntCharge, accountNo, fine 
                from {0} where MemberID ={1} AND ProductType = {2}", TABLE_NAME, membreID, 0), null);
            var lst = new List<MemberProductModel>();

            while (res.MoveToNext())
            {
                lst.Add(new MemberProductModel
                {
                    MemberID = res.GetInt(0),
                    ProductID = res.GetInt(1),
                    ProductName = res.GetString(2),
                    ProductType = res.GetInt(3),
                    LoanRecovery = 0,
                    Recoverable = res.GetDouble(5),
                    Balance = res.GetDouble(6),
                    InstallmentNo = res.GetInt(7),
                    TodayCollectionAmount = res.GetDouble(4),
                    TrxType = res.GetInt(8),
                    SummaryID = res.GetLong(9),
                    InterestCalculationMethod = res.GetString(10),
                    Duration = res.GetInt(11),
                    DurationOverLoanDue = res.GetDouble(12),
                    DurationOverIntDue = res.GetDouble(13),
                    LoanDue = res.GetDouble(14),
                    IntDue = res.GetDouble(15),
                    CumIntCharge = res.GetDouble(16),
                    CumInterestPaid = res.GetDouble(17),
                    PrincipalLoan = res.GetDouble(18),
                    LoanRepaid = res.GetDouble(19),
                    PrinBalance = res.GetDouble(20),
                    SerBalance = res.GetDouble(21),
                    IntCharge = res.GetDouble(22),
                    accountNo = res.GetString(res.GetColumnIndexOrThrow("accountNo")),
                    fine = res.GetColumnIndexOrThrow("fine")
                });
            }
            return lst;
        }
        public List<MemberProductModel> GetAll()
        {

            SQLiteDatabase db = this.ReadableDatabase;

            ICursor c = db.Query(TABLE_NAME, new string[] { "MemberID", "ProductID", "ProductName", "ProductType", "LoanRecovery", "Recoverable", "Balance", "InstallmentNo", "TrxType","SummaryID","accountNo","fine" }, null, null, null, null, null, null);

            var lst = new List<MemberProductModel>();

            while (c.MoveToNext())
            {
                lst.Add(new MemberProductModel
                {
                    MemberID = c.GetInt(0),
                    ProductID = c.GetInt(1),
                    ProductName = c.GetString(2),
                    ProductType = c.GetInt(3),
                    LoanRecovery = c.GetDouble(4),
                    Recoverable = c.GetDouble(5),
                    Balance = c.GetDouble(6),
                    InstallmentNo = c.GetInt(7),
                    TrxType = c.GetInt(8),
                    SummaryID = c.GetLong(9),
                    accountNo = c.GetString(c.GetColumnIndexOrThrow("accountNo")),
                    fine = c.GetColumnIndexOrThrow("fine")
                });
            }

            c.Close();
            db.Close();

            return lst;
        }
        public void DeleteAll()
        {
            SQLiteDatabase db = this.WritableDatabase;
            db.Delete(TABLE_NAME,null, null);
        }
    }
    public class CollectionNewModel
    {
        public string ProductName { get; set; }
        public List<MemberProductModel> MemberProducts { get; set; }
    }
}