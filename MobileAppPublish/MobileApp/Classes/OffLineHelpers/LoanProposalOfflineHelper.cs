using System;
using System.Collections.Generic;
using Android.Content;
using Android.Database.Sqlite;
using Android.Database;

namespace PMS.Droid.Classes.OffLineHelpers
{
    public class LoanProposalOfflineHelper : SQLiteOpenHelper
    {
        private static string APP_DATABASENAME = OfflineDBConstants.APP_DATABASE_NAME;
        private static int APP_DATABASE_VERSION = OfflineDBConstants.APP_DATABASE_VERSION;
        private static string TABLE_NAME = OfflineDBConstants.TABLE_LOAN_PROPOSAL;

        public LoanProposalOfflineHelper(Context ctx) :
            base(ctx, APP_DATABASENAME, null, APP_DATABASE_VERSION)
        {

        }

        public override void OnCreate(SQLiteDatabase db)
        {
            //db.ExecSQL(string.Format(@"CREATE TABLE IF NOT EXISTS {0}(
            //                ProposalID INTEGER PRIMARY KEY AUTOINCREMENT,
            //                ProductName TEXT NOT NULL,
            //                CenterName  TEXT NOT NULL,
            //                OfficeName   TEXT NULL,
            //                PurposeName   TEXT NULL,
            //                MemberCode TEXT,
            //                Amount REAL,
            //                ProductID INTEGER,
            //                PurposeID INTEGER,
            //                CenterID INTEGER,
            //                OfficeID INTEGER)", TABLE_NAME));
        }

        public override void OnUpgrade(SQLiteDatabase db, int oldVersion, int newVersion)
        {
            db.ExecSQL(string.Format("DROP TABLE IF EXISTS {0}", TABLE_NAME));
            OnCreate(db);
        }

        public void AddNewProposal(LoanProposalModel collection)
        {
            SQLiteDatabase db = this.WritableDatabase;
            ContentValues vals = new ContentValues();
            vals.Put("ProductName", collection.ProductName);
            vals.Put("CenterName", collection.CenterName);
            vals.Put("OfficeName", collection.OfficeName);
            vals.Put("PurposeName", collection.PurposeName);
            vals.Put("Amount", collection.Amount);
            vals.Put("MemberCode", collection.MemberCode);

            vals.Put("ProductID", collection.ProductID);
            vals.Put("CenterID", collection.CenterID);
            vals.Put("OfficeID", collection.OfficeID);
            vals.Put("PurposeID", collection.PurposeID);

            db.Insert(TABLE_NAME, null, vals);
        }
        public List<LoanProposalModel> GetAll()
        {

            SQLiteDatabase db = this.ReadableDatabase;

            ICursor c = db.Query(TABLE_NAME, new string[] { "ProposalID", "MemberCode", "Amount", "OfficeName", "CenterName", "ProductName", "ProductID", "CenterID", "OfficeID", "PurposeID", "PurposeName" }, null, null, null, null, null, null);

            var lst = new List<LoanProposalModel>();

            while (c.MoveToNext())
            {
                lst.Add(new LoanProposalModel
                {
                    ProposalID = c.GetInt(0),
                    MemberCode = c.GetString(1),
                    Amount = c.GetDouble(2),
                    OfficeName = c.GetString(3),
                    CenterName = c.GetString(4),
                    ProductName = c.GetString(5),
                    ProductID = c.GetInt(6),
                    CenterID = c.GetInt(7),
                    OfficeID = c.GetInt(8),
                    PurposeID = c.GetInt(9),
                    PurposeName = c.GetString(10),
                });
            }
            c.Close();
            db.Close();

            return lst;
        }
        public void DeleteProposal(int ID)
        {
            SQLiteDatabase db = this.WritableDatabase;
            db.Delete(TABLE_NAME, "ProposalID=?", new String[] { ID.ToString() });
        }
    }
}