using System.Collections.Generic;
using Android.Content;
using Android.Database.Sqlite;
using Android.Database;

namespace PMS.Droid.Classes.OffLineHelpers
{
    public class SystemErrorOfflineHelper : SQLiteOpenHelper
    {
        private static string APP_DATABASENAME = OfflineDBConstants.APP_DATABASE_NAME;
        private static int APP_DATABASE_VERSION = OfflineDBConstants.APP_DATABASE_VERSION;
        private static string TABLE_NAME = OfflineDBConstants.TABLE_SYSTEM_ERROR;

        public SystemErrorOfflineHelper(Context ctx) :
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

        public void AddNew(SystemErrorModel model)
        {
            SQLiteDatabase db = this.WritableDatabase;
            ContentValues vals = new ContentValues();
            vals.Put("ActionName", model.ActionName);
            vals.Put("ErrorText", model.ErrorText);
            vals.Put("InputParameters", model.InputParameters);
            vals.Put("UserID", model.UserID);
            vals.Put("CreateDate", model.CreateDate);
            db.Insert(TABLE_NAME, null, vals);
        }
        public List<SystemErrorModel> GetAll()
        {

            SQLiteDatabase db = this.ReadableDatabase;

            ICursor c = db.Query(TABLE_NAME, new string[] { "ID", "ActionName", "ErrorText", "InputParameters", "UserID", "CreateDate" }, null, null, null, null, null, null);

            var lst = new List<SystemErrorModel>();

            while (c.MoveToNext())
            {
                lst.Add(new SystemErrorModel
                {
                    ID = c.GetInt(0),
                    ActionName = c.GetString(1),
                    ErrorText = c.GetString(2),
                    InputParameters = c.GetString(3),
                    UserID = c.GetString(4),
                    CreateDate = c.GetString(5)
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