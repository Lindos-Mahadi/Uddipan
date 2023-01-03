using Android.Content;
using Android.Database.Sqlite;
using Android.Database;

namespace PMS.Droid.Classes.OffLineHelpers
{
    public class UserOfflineHelper : SQLiteOpenHelper
    {
        private const string APP_DATABASENAME = OfflineDBConstants.APP_DATABASE_NAME;
        private const int APP_DATABASE_VERSION = OfflineDBConstants.APP_DATABASE_VERSION;
        private static string TABLE_NAME = OfflineDBConstants.TABLE_USER;

        public UserOfflineHelper(Context ctx) :
            base(ctx, APP_DATABASENAME, null, APP_DATABASE_VERSION)
        {

        }

        public override void OnCreate(SQLiteDatabase db)
        {
            //db.ExecSQL(string.Format(@"CREATE TABLE IF NOT EXISTS {0}(
            //                LoginID TEXT NOT NULL)", TABLE_NAME));
        }

        public override void OnUpgrade(SQLiteDatabase db, int oldVersion, int newVersion)
        {
            db.ExecSQL(string.Format("DROP TABLE IF EXISTS {0}", TABLE_NAME));
            OnCreate(db);
        }
        public bool IsUserExist(string userid)
        {
            SQLiteDatabase db = this.ReadableDatabase;

            ICursor c = db.Query(TABLE_NAME, new string[] { "LoginID", "Password" }, "LoginID=?", new string[] { userid }, null, null, null, null);

            var user = "";

            while (c.MoveToNext())
            {
                user = c.GetString(0);
            }

            c.Close();
            db.Close();

            return !string.IsNullOrEmpty(user) && user.Length > 0;
        }

        public void AddNew(string loginID, string password, string name, string installmentDate)
        {
            SQLiteDatabase db = this.WritableDatabase;
            ContentValues vals = new ContentValues();
            vals.Put("LoginID", loginID);
            vals.Put("Password", password);
            vals.Put("Name", name);
            vals.Put("InstallmentDate", installmentDate);
            db.Insert(TABLE_NAME, null, vals);
        }

        public void Update(string installmentDate, string loginID, string employeeName)
        {
            SQLiteDatabase db = this.WritableDatabase;
            ContentValues vals = new ContentValues();           
            vals.Put("InstallmentDate", installmentDate);
            vals.Put("Name", employeeName);
            db.Update(TABLE_NAME, vals, null, null);
        }
        public string GetUserID()
        {

            SQLiteDatabase db = this.ReadableDatabase;

            ICursor c = db.Query(TABLE_NAME, new string[] { "LoginID" }, null, null, null, null, null, null);

            var user = "";

            while (c.MoveToNext())
            {
                user = c.GetString(0);              
            }

            c.Close();
            db.Close();

            return user;
        }

        public LoginUserModel GetUser()
        {

            SQLiteDatabase db = this.ReadableDatabase;

            ICursor c = db.Query(TABLE_NAME, new string[] { "LoginID", "Password","Name", "InstallmentDate" }, null, null, null, null, null, null);

            LoginUserModel user =null;

            while (c.MoveToNext())
            {
                user = new LoginUserModel()
                {
                    LoginID = c.GetString(0),
                    Password = c.GetString(1),
                    Name = c.GetString(2),
                    InstallmentDate = c.GetString(3)
                };
            }

            c.Close();
            db.Close();

            return user;
        }



        public bool IsValidLogin(string user, string password)
        {

            SQLiteDatabase db = this.ReadableDatabase;

            ICursor c = db.Query(TABLE_NAME, new string[] { "LoginID", "Password" }, "LoginID=?", new string[] { user }, null, null, null, null);
            var pwd = "";
            while (c.MoveToNext())
            {
                pwd = c.GetString(1);
            }
            c.Close();
            db.Close();

            return password == pwd;
        }
        public void DeleteAll()
        {
            SQLiteDatabase db = this.WritableDatabase;
            db.Delete(TABLE_NAME, null, null);
        }


    }

    public class LoginUserModel
    {
        public string LoginID { get; set; }
        public string Password { get; set; }
        public string InstallmentDate { get; set; }
        public string Name { get; set; }
    }
}