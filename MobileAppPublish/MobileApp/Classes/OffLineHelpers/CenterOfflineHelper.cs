using System.Collections.Generic;
using Android.Content;
using Android.Database.Sqlite;
using Android.Database;

namespace PMS.Droid.Classes.OffLineHelpers
{
    public class CenterOfflineHelper : SQLiteOpenHelper
    {
        private static string APP_DATABASENAME = OfflineDBConstants.APP_DATABASE_NAME;
        private static int APP_DATABASE_VERSION = OfflineDBConstants.APP_DATABASE_VERSION;
        private static string TABLE_NAME = OfflineDBConstants.TABLE_CENTER;

        public CenterOfflineHelper(Context ctx) :
            base(ctx, APP_DATABASENAME, null, APP_DATABASE_VERSION)
        {

        }

        public override void OnCreate(SQLiteDatabase db)
        {
            //db.ExecSQL(string.Format(@"CREATE TABLE IF NOT EXISTS {0}(
            //                CenterID INTEGER,
            //                CenterName TEXT NOT NULL,
            //                OfficeID INTEGER,
            //                OfficeName TEXT NOT NULL)", TABLE_NAME));
        }

        public override void OnUpgrade(SQLiteDatabase db, int oldVersion, int newVersion)
        {
            db.ExecSQL(string.Format("DROP TABLE IF EXISTS {0}", TABLE_NAME));
            OnCreate(db);
        }

        public void AddNew(CenterModel model)
        {
            SQLiteDatabase db = this.WritableDatabase;
            ContentValues vals = new ContentValues();
            vals.Put("CenterID", model.CenterID);
            vals.Put("CenterName", model.CenterName);
            vals.Put("OfficeID", model.OfficeID);
            vals.Put("OfficeName", model.OfficeName);
            db.Insert(TABLE_NAME, null, vals);
        }
        public List<CenterModel> GetCentersByOffice(int officeID, int transactionType = 0)
        {
            SQLiteDatabase db = this.ReadableDatabase;
            var lcFilter = transactionType > 0  ? "" : string.Format(" INNER JOIN {0} LC ON MP.MemberID= LC.MemberID AND MP.ProductID = LC.ProductID ", OfflineDBConstants.TABLE_LOAN_COLLECTION);
            var trTypeFilter = transactionType == 0 || transactionType == 99 ? "" : string.Format("AND MP.TrxType = {0}", transactionType);
            var sql = string.Format(@"SELECT DISTINCT C.CenterID, C.CenterName 
                                FROM {0} C INNER JOIN {1} M ON C.CenterID = M.CenterID  
                                   INNER JOIN {2} MP ON M.MemberID = MP.MemberID {5}
                                    WHERE C.OfficeID={3} {4}", TABLE_NAME, OfflineDBConstants.TABLE_MEMBER, OfflineDBConstants.TABLE_MEMBER_PRODUCT, officeID, trTypeFilter, lcFilter);

            ICursor c = db.RawQuery(sql, null);

            // ICursor c = db.Query(TABLE_NAME, new string[] { "CenterID", "CenterName" }, "OfficeID=?", new string[] { officeID.ToString() }, null, null, null, null);

            var lst = new List<CenterModel>();

            while (c.MoveToNext())
            {
                lst.Add(new CenterModel
                {
                    CenterID = c.GetInt(0),
                    CenterName = c.GetString(1)
                });
            }

            c.Close();
            db.Close();

            return lst;
        }

        public List<CenterModel> GetCentersByOfficeForWithdrawal(int officeID, bool txnOnly = false)
        {
            SQLiteDatabase db = this.ReadableDatabase;
            var lcFilter = !txnOnly? "" : string.Format(" INNER JOIN {0} LC ON MP.MemberID= LC.MemberID AND MP.ProductID = LC.ProductID ", OfflineDBConstants.TABLE_WITHDRAWAL);
            var sql = string.Format(@"SELECT DISTINCT C.CenterID, C.CenterName 
                                FROM {0} C INNER JOIN {1} M ON C.CenterID = M.CenterID  
                                   INNER JOIN {2} MP ON M.MemberID = MP.MemberID {4}
                                    WHERE C.OfficeID={3} AND MP.ProductType = 0", TABLE_NAME, OfflineDBConstants.TABLE_MEMBER, OfflineDBConstants.TABLE_MEMBER_PRODUCT, officeID, lcFilter);

            ICursor c = db.RawQuery(sql, null);


            var lst = new List<CenterModel>();

            while (c.MoveToNext())
            {
                lst.Add(new CenterModel
                {
                    CenterID = c.GetInt(0),
                    CenterName = c.GetString(1)
                });
            }

            c.Close();
            db.Close();

            return lst;
        }
        public CenterModel GetById(int id, int officeID)
        {
            SQLiteDatabase db = this.ReadableDatabase;
            ICursor res = db.RawQuery(string.Format("select CenterID,CenterName,OfficeID,OfficeName from {0} where CenterID={1} And OfficeID={2}", TABLE_NAME, id, officeID), null);
            if (res.MoveToFirst())
            {
                return new CenterModel
                {
                    CenterID = res.GetInt(0),
                    CenterName = res.GetString(1)
                };
            }
            return null;
        }
        public List<CenterModel> GetAll()
        {
            SQLiteDatabase db = this.ReadableDatabase;
            ICursor c = db.RawQuery(string.Format("select CenterID,CenterName,OfficeID,OfficeName from {0} ", TABLE_NAME), null);

            var lst = new List<CenterModel>();
            while (c.MoveToNext())
            {
                lst.Add(new CenterModel
                {
                    CenterID = c.GetInt(0),
                    CenterName = c.GetString(1),
                    OfficeID = c.GetInt(2),
                    OfficeName = c.GetString(3)
                });
            }

            c.Close();
            db.Close();
            return lst;
        }
        public void DeleteAll()
        {
            SQLiteDatabase db = this.WritableDatabase;
            db.Delete(TABLE_NAME, null, null);
        }
    }
}