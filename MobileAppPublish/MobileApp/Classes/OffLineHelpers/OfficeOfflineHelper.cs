using System.Collections.Generic;
using Android.Content;
using Android.Database.Sqlite;
using Android.Database;

namespace PMS.Droid.Classes.OffLineHelpers
{
    public class OfficeOfflineHelper : SQLiteOpenHelper
    {
        private static string APP_DATABASENAME = OfflineDBConstants.APP_DATABASE_NAME;
        private static int APP_DATABASE_VERSION = OfflineDBConstants.APP_DATABASE_VERSION;
        private static string TABLE_NAME = OfflineDBConstants.TABLE_OFFICE;

        public OfficeOfflineHelper(Context ctx) :
            base(ctx, APP_DATABASENAME, null, APP_DATABASE_VERSION)
        {

        }

        public override void OnCreate(SQLiteDatabase db)
        {
            //db.ExecSQL(string.Format(@"CREATE TABLE IF NOT EXISTS {0}(
            //                OfficeID INTEGER,
            //                OfficeName TEXT NOT NULL)", TABLE_NAME));
        }

        public override void OnUpgrade(SQLiteDatabase db, int oldVersion, int newVersion)
        {
            db.ExecSQL(string.Format("DROP TABLE IF EXISTS {0}", TABLE_NAME));
            OnCreate(db);
        }

        public void AddNew(OfficeModel model)
        {
            SQLiteDatabase db = this.WritableDatabase;
            ContentValues vals = new ContentValues();
            vals.Put("OfficeID", model.OfficeID);
            vals.Put("OfficeName", model.OfficeName);
             
            db.Insert(TABLE_NAME, null, vals);
        }

        public List<OfficeModel> GetAll()
        {

            SQLiteDatabase db = this.ReadableDatabase;

            ICursor c = db.Query(TABLE_NAME, new string[] { "OfficeID", "OfficeName" }, null, null, null, null, null, null);

            var contacts = new List<OfficeModel>();

            while (c.MoveToNext())
            {
                contacts.Add(new OfficeModel
                {
                    OfficeID = c.GetInt(0),
                    OfficeName = c.GetString(1)
                   
                });
            }

            c.Close();
            db.Close();

            return contacts;
        }
        public void DeleteAll()
        {
            SQLiteDatabase db = this.WritableDatabase;
            db.Delete(TABLE_NAME, null, null);
        }

        public int GetDatabaseVerion()
        {
            return this.WritableDatabase.Version;
            
        }
    }
}