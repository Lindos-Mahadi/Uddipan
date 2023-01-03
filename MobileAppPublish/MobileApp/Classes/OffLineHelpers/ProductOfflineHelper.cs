using System.Collections.Generic;
using Android.Content;
using Android.Database.Sqlite;
using Android.Database;

namespace PMS.Droid.Classes.OffLineHelpers
{
    public class ProductOfflineHelper : SQLiteOpenHelper
    {
        private static string APP_DATABASENAME = OfflineDBConstants.APP_DATABASE_NAME;
        private static int APP_DATABASE_VERSION = OfflineDBConstants.APP_DATABASE_VERSION;
        private static string TABLE_NAME = OfflineDBConstants.TABLE_PRODUCT;

        public ProductOfflineHelper(Context ctx) :
            base(ctx, APP_DATABASENAME, null, APP_DATABASE_VERSION)
        {

        }

        public override void OnCreate(SQLiteDatabase db)
        {
            //db.ExecSQL(string.Format(@"CREATE TABLE IF NOT EXISTS {0}(
            //                ProductID INTEGER,
            //                ProductName TEXT NOT NULL)", TABLE_NAME));
        }

        public override void OnUpgrade(SQLiteDatabase db, int oldVersion, int newVersion)
        {
            db.ExecSQL(string.Format("DROP TABLE IF EXISTS {0}", TABLE_NAME));
            OnCreate(db);
        }

        public void AddNew(ProductModel model)
        {
            SQLiteDatabase db = this.WritableDatabase;
            ContentValues vals = new ContentValues();
            vals.Put("ProductID", model.ProductID);
            vals.Put("ProductName", model.ProductName);
            db.Insert(TABLE_NAME, null, vals);
        }
        public List<ProductModel> GetAll()
        {

            SQLiteDatabase db = this.ReadableDatabase;

            ICursor c = db.Query(TABLE_NAME, new string[] { "ProductID", "ProductName" }, null, null, null, null, null, null);

            var lst = new List<ProductModel>();

            while (c.MoveToNext())
            {
                lst.Add(new ProductModel
                {
                    ProductID = c.GetInt(0),
                    ProductName = c.GetString(1)
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