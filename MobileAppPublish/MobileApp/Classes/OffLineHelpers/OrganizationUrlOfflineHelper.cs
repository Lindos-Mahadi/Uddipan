using System.Collections.Generic;
using System.Linq;
using Android.Content;
using Android.Database.Sqlite;
using Android.Database;

namespace PMS.Droid.Classes.OffLineHelpers
{
    public class OrganizationUrlOfflineHelper : SQLiteOpenHelper
    {
        private static string APP_DATABASENAME = OfflineDBConstants.APP_DATABASE_NAME;
        private static int APP_DATABASE_VERSION = OfflineDBConstants.APP_DATABASE_VERSION;
        private static string TABLE_NAME = OfflineDBConstants.TABLE_ORGANIZATION;

        public OrganizationUrlOfflineHelper(Context ctx) :
            base(ctx, APP_DATABASENAME, null, APP_DATABASE_VERSION)
        {

        }

        public override void OnCreate(SQLiteDatabase db)
        {
            db.ExecSQL(string.Format(@"CREATE TABLE IF NOT EXISTS {0}(
                            OrganizationUrl  TEXT NOT NULL,
                            Name TEXT NOT NULL)", TABLE_NAME));
        }

        public override void OnUpgrade(SQLiteDatabase db, int oldVersion, int newVersion)
        {
            //db.ExecSQL(string.Format("DROP TABLE IF EXISTS {0}", TABLE_NAME));
            //OnCreate(db);
        }

        public void AddNew(OrganizationModel model)
        {
            var old = Get();
            if (old != null)
            {
                DeleteAll();
            }
            SQLiteDatabase db = this.WritableDatabase;
            ContentValues vals = new ContentValues();
            vals.Put("Name", model.Name);
            vals.Put("OrganizationUrl", model.OrganizationUrl);
            db.Insert(TABLE_NAME, null, vals);

        }
        public OrganizationModel Get()
        {

            SQLiteDatabase db = this.ReadableDatabase;

            ICursor c = db.Query(TABLE_NAME, new string[] { "Name", "OrganizationUrl" }, null, null, null, null, null, null);

            var lst = new List<OrganizationModel>();

            while (c.MoveToNext())
            {
                lst.Add(new OrganizationModel
                {
                    Name = c.GetString(0),
                    OrganizationUrl = c.GetString(1)
                });
            }

            c.Close();
            db.Close();

            return lst.FirstOrDefault();
        }
        public void DeleteAll()
        {
            SQLiteDatabase db = this.WritableDatabase;
            db.Delete(TABLE_NAME, null, null);
        }
    }
}