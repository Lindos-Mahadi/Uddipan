using System.Collections.Generic;
using System.Linq;
using Android.Content;
using Android.Database.Sqlite;
using Android.Database;

namespace PMS.Droid.Classes.OffLineHelpers
{
    public class MemberOfflineHelper : SQLiteOpenHelper
    {
        private static string APP_DATABASENAME = OfflineDBConstants.APP_DATABASE_NAME;
        private static int APP_DATABASE_VERSION = OfflineDBConstants.APP_DATABASE_VERSION;
        private static string TABLE_NAME = OfflineDBConstants.TABLE_MEMBER;

        public MemberOfflineHelper(Context ctx):
            base(ctx, APP_DATABASENAME, null, APP_DATABASE_VERSION)
        {

        }

        public override void OnCreate(SQLiteDatabase db)
        {
        //    db.ExecSQL(string.Format(@"CREATE TABLE IF NOT EXISTS {0}(                    
        //                    MemberID INTEGER,
        //                    MemberName TEXT NOT NULL,
        //                    MemberCode TEXT NOT NULL,
        //                    CenterID INTEGER,
        //                    CenterName TEXT NOT NULL,
        //                    OfficeID INTEGER,
        //                    OfficeName TEXT NOT NULL)", TABLE_NAME));
        }

        public override void OnUpgrade(SQLiteDatabase db, int oldVersion, int newVersion)
        {
            db.ExecSQL(string.Format("DROP TABLE IF EXISTS {0}", TABLE_NAME));
            OnCreate(db);
        }

        public void AddNew(MemberModel model)
        {
            SQLiteDatabase db = this.WritableDatabase;
            ContentValues vals = new ContentValues();
            vals.Put("MemberID", model.MemberID);
            vals.Put("MemberName", model.MemberName);
            vals.Put("MemberCode", model.MemberCode);
            vals.Put("CenterID", model.CenterID);
            vals.Put("CenterName", model.CenterName);
            vals.Put("OfficeID", model.OfficeID);
            vals.Put("OfficeName", model.OfficeName);
            db.Insert(TABLE_NAME, null, vals);
        }
        public List<MemberModel> GetMembersByCenter(int centerID)
        {

            SQLiteDatabase db = this.ReadableDatabase;

            ICursor c = db.Query(TABLE_NAME, new string[] { "MemberID", "MemberName", "MemberCode" }, "CenterID=?", new string[] { centerID.ToString() }, null, null, null, null);

            var lst = new List<MemberModel>();

            while (c.MoveToNext())
            {
                lst.Add(new MemberModel
                {
                    MemberID = c.GetInt(0),
                    MemberName = c.GetString(1),
                    MemberCode = c.GetString(2)
                });
            }

            c.Close();
            db.Close();

            return lst;
        }
        public List<MemberModel> GetMembersByCenterForCollection(int centerID, int trxType = 1)
        {
            SQLiteDatabase db = this.ReadableDatabase;
            //var sql = @"SELECT A.MemberID MemberID,A.MemberName MemberName,A.MemberCode MemberCode, P.TrxType
            //        from {0} A 
            //    INNER JOIN {1} P ON A.MemberID = P.MemberID
            //    LEFT JOIN  {2} B
            //        ON A.MemberID = B.MemberID AND P.ProductID = B.P
            //    WHERE B.MemberID IS NULL AND A.CenterID = {3}";
            var sql = string.Format(@"SELECT M.MemberID, M.MemberName, M.MemberCode FROM 
                (SELECT A.MemberID MemberID,A.MemberName MemberName,A.MemberCode MemberCode, A.CenterID CenterID, P.ProductID, P.TrxType
                    from {0} A 
                INNER JOIN {1} P ON A.MemberID = P.MemberID WHERE P.TrxType = {4} AND A.CenterID = {3}) M
                LEFT JOIN (SELECT MemberID, ProductID, TrxType FROM {2}  WHERE TrxType = {4} AND CenterID = {3}) LP
                    ON M.MemberID = LP.MemberID AND M.ProductID = LP.ProductID
                    AND M.TrxType = LP.TrxType
                WHERE LP.MemberID IS NULL AND M.CenterID = {3}", TABLE_NAME, OfflineDBConstants.TABLE_MEMBER_PRODUCT, OfflineDBConstants.TABLE_LOAN_COLLECTION, centerID, trxType);
            ICursor c = db.RawQuery(sql, null);

            var lst = new List<MemberModel>();

            while (c.MoveToNext())
            {
                var member = new MemberModel
                {
                    MemberID = c.GetLong(0),
                    MemberName = c.GetString(1),
                    MemberCode = c.GetString(2)
                };
                var exist = lst.Where(w => w.MemberID == member.MemberID).Count();
                if (exist == 0)
                    lst.Add(member);
            }
            c.Close();
            db.Close();

            return lst;
        }

        public List<MemberModel> GetMembersByCenterForWithdrawal(int centerID)
        {
            SQLiteDatabase db = this.ReadableDatabase;
            var sql = string.Format(@"SELECT M.MemberID, M.MemberName, M.MemberCode FROM 
                (SELECT A.MemberID MemberID,A.MemberName MemberName,A.MemberCode MemberCode, A.CenterID CenterID, P.ProductID, P.TrxType
                    from {0} A 
                INNER JOIN {1} P ON A.MemberID = P.MemberID WHERE P.ProductType = 0 AND A.CenterID = {3}) M
                LEFT JOIN (SELECT MemberID, ProductID, TrxType FROM {2}  WHERE CenterID = {3}) LP
                    ON M.MemberID = LP.MemberID
                    AND M.ProductID = LP.ProductID
                WHERE LP.MemberID IS NULL AND M.CenterID = {3}", TABLE_NAME, OfflineDBConstants.TABLE_MEMBER_PRODUCT, OfflineDBConstants.TABLE_WITHDRAWAL, centerID);
            ICursor c = db.RawQuery(sql, null);

            var lst = new List<MemberModel>();

            while (c.MoveToNext())
            {
                var member = new MemberModel
                {
                    MemberID = c.GetLong(0),
                    MemberName = c.GetString(1),
                    MemberCode = c.GetString(2)
                };
                var exist = lst.Where(w => w.MemberID == member.MemberID).Count();
                if (exist == 0)
                    lst.Add(member);
            }
            c.Close();
            db.Close();

            return lst;
        }
        public List<MemberModel> GetAll()
        {

            SQLiteDatabase db = this.ReadableDatabase;

            ICursor c = db.RawQuery(string.Format("select MemberID,MemberName,MemberCode,OfficeID,OfficeName,CenterID,CenterName from {0} ", TABLE_NAME), null);

            var lst = new List<MemberModel>();

            while (c.MoveToNext())
            {
                lst.Add(new MemberModel
                {
                    MemberID = c.GetLong(0),
                    MemberName = c.GetString(1),
                    MemberCode = c.GetString(2),
                    OfficeID= c.GetInt(3),
                    OfficeName = c.GetString(4),
                    CenterID = c.GetInt(5),
                    CenterName = c.GetString(6)
                });
            }

            c.Close();
            db.Close();

            return lst;
        }
        public MemberModel GetById(long id)
        {
            SQLiteDatabase db = this.ReadableDatabase;
            ICursor res = db.RawQuery(string.Format("select * from {0} where MemberID={1}", TABLE_NAME, id), null);
            if (res.MoveToFirst())
            {
                return new MemberModel
                {
                    MemberID = res.GetInt(0),
                    MemberName = res.GetString(1),
                    MemberCode = res.GetString(2),                    
                    CenterID = res.GetInt(3),
                    CenterName = res.GetString(4),
                    OfficeID = res.GetInt(5),
                    OfficeName = res.GetString(6)
                };
            }
            return null;
        }
        public MemberModel GetByName(string name)
        {
            SQLiteDatabase db = this.ReadableDatabase;
            ICursor res = db.RawQuery(string.Format("select * from {0} where MemberName='{1}'", TABLE_NAME, name), null);
            if (res.MoveToFirst())
            {
                return new MemberModel
                {
                    MemberID = res.GetInt(0),
                    MemberName = res.GetString(1),
                    MemberCode = res.GetString(2),
                    CenterID = res.GetInt(3),
                    CenterName = res.GetString(4),
                    OfficeID = res.GetInt(5),
                    OfficeName = res.GetString(6)
                };
            }
            return null;
        }
        public void DeleteAll()
        {
            SQLiteDatabase db = this.WritableDatabase;
            db.Delete(TABLE_NAME, null, null);
        }
    }
}