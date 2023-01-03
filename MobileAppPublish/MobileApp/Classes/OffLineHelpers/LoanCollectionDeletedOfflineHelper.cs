using System;
using System.Collections.Generic;
using System.Linq;
using Android.Content;
using Android.Database.Sqlite;
using Android.Database;

namespace PMS.Droid.Classes.OffLineHelpers
{
    public class LoanCollectionDeletedOfflineHelper : SQLiteOpenHelper
    {
        private static string APP_DATABASENAME = OfflineDBConstants.APP_DATABASE_NAME;
        private static int APP_DATABASE_VERSION = OfflineDBConstants.APP_DATABASE_VERSION;
        private static string TABLE_NAME = OfflineDBConstants.TABLE_LOAN_COLLECTION_Deleted;

        public LoanCollectionDeletedOfflineHelper(Context ctx) :
            base(ctx, APP_DATABASENAME, null, APP_DATABASE_VERSION)
        {

        }

        public override void OnCreate(SQLiteDatabase db)
        {
            //db.ExecSQL(string.Format(@"CREATE TABLE IF NOT EXISTS {0}(
            //                CollectionID INTEGER PRIMARY KEY AUTOINCREMENT,
            //                ProductName TEXT NOT NULL,
            //                CenterName  TEXT NOT NULL,
            //                OfficeName   TEXT NULL,
            //                MemberCode TEXT,
            //                Amount REAL,
            //                ProductID INTEGER,
            //                CenterID INTEGER,
            //                OfficeID INTEGER,
            //                MemberID INTEGER)", TABLE_NAME));          
        }

        public override void OnUpgrade(SQLiteDatabase db, int oldVersion, int newVersion)
        {
            db.ExecSQL(string.Format("DROP TABLE IF EXISTS {0}", TABLE_NAME));
            OnCreate(db);
        }

        public void AddNewCollection(LoanCollectionModel collection)
        {
            //  ProductType INTEGER,
            // SyncFlag INTEGER,
            SQLiteDatabase db = this.WritableDatabase;
            ContentValues vals = new ContentValues();
            vals.Put("ProductName", collection.ProductName);
            vals.Put("CenterName", collection.CenterName);
            vals.Put("OfficeName", collection.OfficeName);
            vals.Put("Amount", collection.Amount);
            vals.Put("MemberCode", collection.MemberCode);

            vals.Put("ProductType", collection.ProductType);
            vals.Put("SyncFlag", collection.SyncFlag);

            vals.Put("ProductID", collection.ProductID);
            vals.Put("CenterID", collection.CenterID);
            vals.Put("OfficeID", collection.OfficeID);
            vals.Put("MemberID", collection.MemberID);
            vals.Put("DueAmount", collection.DueAmount);
            vals.Put("TrxType", collection.TrxType);
            vals.Put("CollectionGUID", Guid.NewGuid().ToString());
            vals.Put("SummaryID", collection.SummaryID);

            vals.Put("IntCharge", collection.IntCharge);
            vals.Put("LoanInstallment", collection.LoanInstallment);
            vals.Put("IntInstallment", collection.IntInstallment);
            vals.Put("CollectionType", collection.CollectionType);
            //CollectionGUID
            db.Insert(TABLE_NAME, null, vals);
        }
        public List<LoanCollectionModel> GetAll()
        {
            SQLiteDatabase db = this.ReadableDatabase;
            //var sql = string.Format(@"SELECT LC.CollectionID, LC.MemberCode, LC.Amount, LC.OfficeName, LC.CenterName, LC.ProductName, LC.ProductID, LC.CenterID,
            //    LC.OfficeID, LC.MemberID, LC.DueAmount, LC.TrxType, LC.ProductType, LC.SyncFlag, LC.CollectionGUID, LC.SummaryID
            //,LC.IntCharge, LC.LoanInstallment, (CASE WHEN LC.ProductType=0 AND WD.LoanInstallment IS NOT NULL THEN WD.LoanInstallment ELSE LC.IntInstallment END) from {0} LC LEFT JOIN {1} WD ON LC.SummaryID = WD.SummaryID 
            //UNION
            //SELECT WD.CollectionID,WD.MemberCode,WD.Amount,WD.OfficeName,WD.CenterName,WD.ProductName,WD.ProductID,WD.CenterID,
            //   WD.OfficeID,WD.MemberID,WD.DueAmount,WD.TrxType,WD.ProductType,WD.SyncFlag,WD.CollectionGUID,WD.SummaryID
            //,WD.IntCharge, 0, WD.LoanInstallment from {1} WD LEFT JOIN {0} LC ON WD.SummaryID = LC.SummaryID WHERE LC.SummaryID IS NULL", TABLE_NAME, OfflineDBConstants.TABLE_WITHDRAWAL);
            ICursor c = db.Query(TABLE_NAME, new string[] { "CollectionID", "MemberCode", "Amount", "OfficeName", "CenterName", "ProductName", "ProductID", "CenterID",
                "OfficeID", "MemberID", "DueAmount", "TrxType", "ProductType", "SyncFlag", "CollectionGUID", "SummaryID"
            , "IntCharge", "LoanInstallment", "IntInstallment"}, null, null, null, null, null, null);

            var lst = new List<LoanCollectionModel>();

            while (c.MoveToNext())
            {
                lst.Add(new LoanCollectionModel
                {
                    CollectionID = c.GetInt(0),
                    MemberCode = c.GetString(1),
                    Amount = c.GetDouble(2),
                    OfficeName = c.GetString(3),
                    CenterName = c.GetString(4),
                    ProductName = c.GetString(5),
                    ProductID = c.GetInt(6),
                    CenterID = c.GetInt(7),
                    OfficeID = c.GetInt(8),
                    MemberID = c.GetLong(9),
                    DueAmount = c.GetDouble(10),
                    TrxType = c.GetInt(11),
                    ProductType = c.IsNull(12) ? 1 : c.GetInt(12),
                    SyncFlag = c.IsNull(13) ? 0 : c.GetInt(13),
                    Token = c.GetString(14),
                    SummaryID = c.GetLong(15),
                    IntCharge = c.GetDouble(16),
                    LoanInstallment = c.GetDouble(17),
                    IntInstallment = c.GetDouble(18)
                });
            }
            c.Close();
            db.Close();
            var withdrawals = GetAllWithdrawals();

            var newList = new List<LoanCollectionModel>();
            foreach (var wObj in withdrawals)
            {
                var collExists = lst.Where(w => w.SummaryID == wObj.SummaryID).FirstOrDefault();
                if (collExists != null)
                {
                    collExists.IntInstallment = wObj.LoanInstallment;
                }
                else
                {
                    wObj.IntInstallment = wObj.LoanInstallment;
                    wObj.LoanInstallment = 0;
                    newList.Add(wObj);
                }
            }
            lst.AddRange(newList);
            return lst.OrderBy(o => o.MemberCode).ToList();
        }

        public List<LoanCollectionModel> GetAllWithdrawals()
        {
            SQLiteDatabase db = this.ReadableDatabase;
            ICursor c = db.Query(OfflineDBConstants.TABLE_WITHDRAWAL, new string[] { "CollectionID", "MemberCode", "Amount", "OfficeName", "CenterName", "ProductName", "ProductID", "CenterID",
                "OfficeID", "MemberID", "DueAmount", "TrxType", "ProductType", "SyncFlag", "CollectionGUID", "SummaryID"
            , "IntCharge", "LoanInstallment", "IntInstallment"}, null, null, null, null, null, null);

            var lst = new List<LoanCollectionModel>();

            while (c.MoveToNext())
            {
                lst.Add(new LoanCollectionModel
                {
                    CollectionID = c.GetInt(0),
                    MemberCode = c.GetString(1),
                    Amount = c.GetDouble(2),
                    OfficeName = c.GetString(3),
                    CenterName = c.GetString(4),
                    ProductName = c.GetString(5),
                    ProductID = c.GetInt(6),
                    CenterID = c.GetInt(7),
                    OfficeID = c.GetInt(8),
                    MemberID = c.GetLong(9),
                    DueAmount = c.GetDouble(10),
                    TrxType = c.GetInt(11),
                    ProductType = c.IsNull(12) ? 1 : c.GetInt(12),
                    SyncFlag = c.IsNull(13) ? 0 : c.GetInt(13),
                    Token = c.GetString(14),
                    SummaryID = c.GetLong(15),
                    IntCharge = c.GetDouble(16),
                    LoanInstallment = c.GetDouble(17),
                    IntInstallment = c.GetDouble(18)
                });
            }
            c.Close();
            db.Close();

            return lst.OrderBy(o => o.MemberCode).ToList();
        }
        public List<LoanCollectionSummaryModel> GetCollectionSummary()
        {

            SQLiteDatabase db = this.ReadableDatabase;

            ICursor c = db.Query(TABLE_NAME, new string[] { "CollectionID", "MemberCode", "Amount", "OfficeName", "CenterName", "ProductName", "ProductID", "CenterID", "OfficeID", "MemberID", "DueAmount", "TrxType", "ProductType" }, null, null, null, null, null, null);

            var lst = new List<LoanCollectionModel>();

            while (c.MoveToNext())
            {
                lst.Add(new LoanCollectionModel
                {
                    CollectionID = c.GetInt(0),
                    MemberCode = c.GetString(1),
                    Amount = c.GetDouble(2),
                    OfficeName = c.GetString(3),
                    CenterName = c.GetString(4),
                    ProductName = c.GetString(5),
                    ProductID = c.GetInt(6),
                    CenterID = c.GetInt(7),
                    OfficeID = c.GetInt(8),
                    MemberID = c.GetLong(9),
                    DueAmount = c.GetDouble(10),
                    TrxType = c.GetInt(11),
                    ProductType = c.IsNull(12) ? 1 : c.GetInt(12)
                });
            }
            c.Close();
            db.Close();
            //======Product summary
            var prods = from l in lst
                        group l by new { l.ProductID, l.ProductName, l.CenterName }
                        into g
                        orderby g.Key.CenterName ascending
                        select new { g.Key.ProductID, g.Key.ProductName, g.Key.CenterName, Collection = g.Sum(s => s.Amount), DueAmount = g.Sum(s => s.DueAmount) };
            //========center summary...
            var centerSummaryList = from l in lst
                                    group l by new { l.CenterName }
                        into g
                                    orderby g.Key.CenterName ascending
                                    select new { g.Key.CenterName, Collection = g.Sum(s => s.Amount), DueAmount = g.Sum(s => s.DueAmount) };

            var summaryList = new List<LoanCollectionSummaryModel>() { new LoanCollectionSummaryModel() { CenterName = "Samity", ProductName = "Product", Receivable = "Receivable", Collection = "Collection", Due = "Due", DueAmount = 0, RecordSequence = 1, RecordType = "H" } };

            // var currentGroupSummary = new LoanCollectionSummaryModel() { CenterName = "", DueAmount = 0, CollectionAmount = 0, ReceivableAmount = 0 };

            int recordSequence = 2;
            foreach (var center in centerSummaryList)
            {
                var centerName = center.CenterName;
                var centerCollections = prods.Where(w => w.CenterName == centerName);
                var due = 0.00;
                foreach (var v in centerCollections)
                {
                    due = v.DueAmount - v.Collection;
                    if (due < 0)
                        due = 0;
                    var summaryObj = new LoanCollectionSummaryModel()
                    {
                        CenterName = v.CenterName,
                        ProductName = v.ProductName,
                        Receivable = string.Format("{0:C2}", v.DueAmount).Replace("$", "").Replace("€", ""),
                        Collection = string.Format("{0:C2}", v.Collection).Replace("$", "").Replace("€", ""),
                        Due = string.Format("{0:C2}", due).Replace("$", "").Replace("€", ""),
                        DueAmount = due,
                        RecordSequence = recordSequence,
                        RecordType = "D"
                    };
                    summaryList.Add(summaryObj);
                    recordSequence++;
                }
                due = center.DueAmount - center.Collection;
                if (due < 0)
                    due = 0;
                var centerSummaryObj = new LoanCollectionSummaryModel()
                {
                    CenterName = "",
                    ProductName = "Samity Total:",
                    Receivable = string.Format("{0:C2}", center.DueAmount).Replace("$", "").Replace("€", ""),
                    Collection = string.Format("{0:C2}", center.Collection).Replace("$", "").Replace("€", ""),
                    Due = string.Format("{0:C2}", due).Replace("$", "").Replace("€", ""),
                    DueAmount = 0,
                    RecordSequence = recordSequence,
                    RecordType = "S"
                };
                summaryList.Add(centerSummaryObj);
                recordSequence++;
            }


            var totalReceivable = prods.Sum(s => s.DueAmount);
            var totalCollection = prods.Sum(s => s.Collection);
            var totalDue = summaryList.Sum(s => s.DueAmount);
            summaryList.Add(new LoanCollectionSummaryModel() { CenterName = "", ProductName = "Grand Total:", Collection = string.Format("{0:C2}", totalCollection).Replace("$", "").Replace("€", ""), Receivable = string.Format("{0:C2}", totalReceivable).Replace("$", "").Replace("€", ""), Due = string.Format("{0:C2}", totalDue).Replace("$", "").Replace("€", ""), DueAmount = 0, RecordSequence = recordSequence, RecordType = "G" });
            return summaryList;
        }

        public List<LoanCollectionListModel> GetMemberWiseCollectionSummary(int centeId)
        {

            SQLiteDatabase db = this.ReadableDatabase;

            var sql = string.Format(@"SELECT MemberID, MemberCode, 
                                    SUM(CASE WHEN ProductType=0 THEN 0 ELSE Amount END) LoanTotal,  
                                    SUM(CASE WHEN ProductType=0 THEN Amount ELSE 0 END) SavingsTotal, 
                                    SUM(DueAmount-Amount) DueTotal
                                    FROM {0}
                                WHERE CenterID = {1} GROUP BY MemberID, MemberCode", TABLE_NAME, centeId);
            ICursor c = db.RawQuery(sql, null);


            //ICursor c = db.Query(TABLE_NAME, new string[] { "MemberCode", "Amount",  "MemberID", "DueAmount", "TrxType" }, null, null, null, null, null, null);

            var lst = new List<LoanCollectionListModel>()
            {
                new LoanCollectionListModel() { MemberID = -1, MemberName = "Member", SavingsAmount = "Savings", LoanAmount = "Loan", DueAmount = "Due", TotalAmount = "Total", RecordType = "H", IsSynced = true, DueAmountDbl = 0, LoanAmountDbl = 0, SavingsAmountDbl = 0, TotalAmountDbl = 0, HasDue = false }
            };

            while (c.MoveToNext())
            {
                var obj = new LoanCollectionListModel
                {
                    MemberID = c.GetInt(0),
                    MemberName = c.GetString(1),
                    LoanAmount = string.Format("{0:C2}", c.GetDouble(2)).Replace("$", "").Replace("€", ""),
                    SavingsAmount = string.Format("{0:C2}", c.GetDouble(3)).Replace("$", "").Replace("€", ""),
                    DueAmount = c.GetDouble(4) < 0 ? "0.00" : string.Format("{0:C2}", c.GetDouble(4)).Replace("$", "").Replace("€", ""),
                    TotalAmount = string.Format("{0:C2}", (c.GetDouble(2) + c.GetDouble(3))).Replace("$", "").Replace("€", ""),
                    HasDue = (c.GetDouble(4) > 0),
                    RecordType = "T",
                    LoanAmountDbl = c.GetDouble(2),
                    SavingsAmountDbl = c.GetDouble(3),
                    DueAmountDbl = c.GetDouble(4) < 0 ? 0.00 : c.GetDouble(4),
                    TotalAmountDbl = (c.GetDouble(2) + c.GetDouble(3)),
                };
                lst.Add(obj);
            }

            var totalRow = (from l in lst
                            where l.RecordType == "T"
                            group l by new { l.RecordType }
                     into g
                            orderby g.Key.RecordType ascending
                            select new { g.Key.RecordType, LoanSum = g.Sum(s => s.LoanAmountDbl), SavingsSum = g.Sum(s => s.SavingsAmountDbl), DueSum = g.Sum(s => s.DueAmountDbl), TotalSum = g.Sum(s => s.TotalAmountDbl) }).FirstOrDefault();

            var item = new LoanCollectionListModel()
            {
                MemberID = -1,
                MemberName = "Total",
                SavingsAmount = totalRow == null ? "" : string.Format("{0:C2}", totalRow.SavingsSum).Replace("$", ""),
                LoanAmount = totalRow == null ? "" : string.Format("{0:C2}", totalRow.LoanSum).Replace("$", ""),
                DueAmount = totalRow == null ? "" : string.Format("{0:C2}", totalRow.DueSum).Replace("$", ""),
                TotalAmount = totalRow == null ? "" : string.Format("{0:C2}", totalRow.TotalSum).Replace("$", ""),
                RecordType = "Z", //Summary record
                IsSynced = true,
                DueAmountDbl = 0,
                LoanAmountDbl = 0,
                SavingsAmountDbl = 0,
                TotalAmountDbl = 0,
                HasDue = totalRow == null ? false : (totalRow.DueSum > 0)
            };
            lst.Add(item);

            c.Close();
            db.Close();

            return lst;
        }
        public bool CollectionSummaryExist(long summaryId)
        {

            SQLiteDatabase db = this.ReadableDatabase;

            ICursor c = db.Query(TABLE_NAME, new string[] { "SummaryID" }, "SummaryID=?", new string[] { summaryId.ToString() }, null, null, null, null);

            var sid = 0L;

            while (c.MoveToNext())
            {

                sid = c.GetLong(0);

            }

            c.Close();
            db.Close();

            return sid != 0;
        }
        public void DeleteCollection(long ID)
        {
            SQLiteDatabase db = this.WritableDatabase;
            db.Delete(TABLE_NAME, "CollectionID=?", new String[] { ID.ToString() });
        }
        public void DeleteCollectionBySummaryId(long ID)
        {
            SQLiteDatabase db = this.WritableDatabase;
            db.Delete(TABLE_NAME, "SummaryID=?", new String[] { ID.ToString() });
        }
        public void DeleteMemberCollection(long MemberId)
        {
            SQLiteDatabase db = this.WritableDatabase;
            db.Delete(TABLE_NAME, "MemberID=?", new String[] { MemberId.ToString() });
        }
        public void DeleteAll()
        {
            SQLiteDatabase db = this.WritableDatabase;
            db.Delete(TABLE_NAME, null, null);
        }
        public int GetCollectionRecordCount()
        {
            SQLiteDatabase db = this.WritableDatabase;
            var sql = string.Format(@"SELECT count(1)
                                    FROM {0}
                                ", TABLE_NAME);
            ICursor c = db.RawQuery(sql, null);
            var cnt = 0;
            while (c.MoveToNext())
            {
                cnt = c.GetInt(0);
            }
            c.Close();
            db.Close();
            return cnt;
        }
    }
}