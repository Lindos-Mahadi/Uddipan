using Android.Content;
using Android.Database.Sqlite;


namespace PMS.Droid.Classes.OffLineHelpers
{
    public class OfflineDbUpgradeHelper : SQLiteOpenHelper
    {
        private const string APP_DATABASENAME = OfflineDBConstants.APP_DATABASE_NAME;
        private const int APP_DATABASE_VERSION = OfflineDBConstants.APP_DATABASE_VERSION;
        private static string TABLE_NAME = OfflineDBConstants.TABLE_UPGRADE_HELPER;

        public OfflineDbUpgradeHelper(Context ctx) :
            base(ctx, APP_DATABASENAME, null, APP_DATABASE_VERSION)
        {

        }

        public override void OnCreate(SQLiteDatabase db)
        {
            db.ExecSQL(string.Format(@"CREATE TABLE IF NOT EXISTS {0}(
                            LoginID TEXT NOT NULL)", TABLE_NAME));

            db.ExecSQL(string.Format(@"CREATE TABLE IF NOT EXISTS {0}(
                            CenterID INTEGER,
                            CenterName TEXT NOT NULL,
                            OfficeID INTEGER,
                            OfficeName TEXT NOT NULL)", OfflineDBConstants.TABLE_CENTER));

            db.ExecSQL(string.Format(@"CREATE TABLE IF NOT EXISTS {0}(
                            CollectionID INTEGER PRIMARY KEY AUTOINCREMENT,
                            ProductName TEXT NOT NULL,
                            CenterName  TEXT NOT NULL,
                            OfficeName   TEXT NULL,
                            MemberCode TEXT,
                            Amount REAL,
                            ProductID INTEGER,
                            CenterID INTEGER,
                            OfficeID INTEGER,
                            MemberID INTEGER,
                            DueAmount REAL,
                            ProductType INTEGER,
                            SyncFlag INTEGER,
                            TrxType INTEGER, 
                            CollectionGUID TEXT,
                            SummaryID INTEGER,
                            IntCharge INTEGER,
                            LoanInstallment INTEGER,
                            IntInstallment INTEGER,
                            Created Text,
                            fine REAL)", OfflineDBConstants.TABLE_LOAN_COLLECTION));

            db.ExecSQL(string.Format(@"CREATE TABLE IF NOT EXISTS {0}(
                            CollectionID INTEGER PRIMARY KEY AUTOINCREMENT,
                            ProductName TEXT NOT NULL,
                            CenterName  TEXT NOT NULL,
                            OfficeName   TEXT NULL,
                            MemberCode TEXT,
                            Amount REAL,
                            ProductID INTEGER,
                            CenterID INTEGER,
                            OfficeID INTEGER,
                            MemberID INTEGER,
                            DueAmount REAL,
                            ProductType INTEGER,
                            SyncFlag INTEGER,
                            TrxType INTEGER, 
                            CollectionGUID TEXT,
                            SummaryID INTEGER,
                            IntCharge INTEGER,
                            LoanInstallment INTEGER,
                            IntInstallment INTEGER,
                            CollectionType  INTEGER   )", OfflineDBConstants.TABLE_LOAN_COLLECTION_Deleted));



            db.ExecSQL(string.Format(@"CREATE TABLE IF NOT EXISTS {0}(
                            CollectionID INTEGER PRIMARY KEY AUTOINCREMENT,
                            ProductName TEXT NOT NULL,
                            CenterName  TEXT NOT NULL,
                            OfficeName   TEXT NULL,
                            MemberCode TEXT,
                            Amount REAL,
                            ProductID INTEGER,
                            CenterID INTEGER,
                            OfficeID INTEGER,
                            MemberID INTEGER,
                            DueAmount REAL,
                            ProductType INTEGER,
                            SyncFlag INTEGER,
                            TrxType INTEGER, 
                            CollectionGUID TEXT,
                            SummaryID INTEGER,
                            IntCharge INTEGER,
                            LoanInstallment INTEGER,
                            IntInstallment INTEGER)", OfflineDBConstants.TABLE_WITHDRAWAL));

            db.ExecSQL(string.Format(@"CREATE TABLE IF NOT EXISTS {0}(
                            ProposalID INTEGER PRIMARY KEY AUTOINCREMENT,
                            ProductName TEXT NOT NULL,
                            CenterName  TEXT NOT NULL,
                            OfficeName   TEXT NULL,
                            PurposeName   TEXT NULL,
                            MemberCode TEXT,
                            Amount REAL,
                            ProductID INTEGER,
                            PurposeID INTEGER,
                            CenterID INTEGER,
                            OfficeID INTEGER)", OfflineDBConstants.TABLE_LOAN_PROPOSAL));

            db.ExecSQL(string.Format(@"CREATE TABLE IF NOT EXISTS {0}(                    
                            MemberID INTEGER,
                            MemberName TEXT NOT NULL,
                            MemberCode TEXT NOT NULL,
                            CenterID INTEGER,
                            CenterName TEXT NOT NULL,
                            OfficeID INTEGER,
                            OfficeName TEXT NOT NULL)", OfflineDBConstants.TABLE_MEMBER));
           
            db.ExecSQL(string.Format(@"CREATE TABLE IF NOT EXISTS {0}(
                            MemberID  INTEGER,
                            ProductID INTEGER,
                            ProductName TEXT NOT NULL,
                            ProductType INTEGER,
                            LoanRecovery INTEGER,
                            Recoverable INTEGER,
                            Balance INTEGER,
                            PrinBalance INTEGER,
                            SerBalance INTEGER,
                            InstallmentNo INTEGER,
                            TrxType INTEGER, 
                            SummaryID INTEGER,                            
                            InterestCalculationMethod TEXT,
                            Duration INTEGER,
                            DurationOverLoanDue INTEGER,
                            DurationOverIntDue INTEGER,
                            LoanDue INTEGER,
                            IntDue INTEGER,
                            CumIntCharge INTEGER,
                            CumInterestPaid INTEGER,
                            PrincipalLoan INTEGER,
                            LoanRepaid INTEGER,
                            IntCharge integer,
                            NewDue integer,
                            MainProductCode TEXT,
                            CenterID integer,
                            MemberName text,
                            Doc integer,
                            OrgID INTEGER,
                            accountNo TEXT,
                            fine REAL,
                            PersonalSaving REAL,
                            PersonalWithdraw REAL)", OfflineDBConstants.TABLE_MEMBER_PRODUCT));
            db.ExecSQL(string.Format(@"CREATE TABLE IF NOT EXISTS {0}(
                            OfficeID INTEGER,
                            OfficeName TEXT NOT NULL)", OfflineDBConstants.TABLE_OFFICE));
            db.ExecSQL(string.Format(@"CREATE TABLE IF NOT EXISTS {0}(
                            ProductID INTEGER,
                            ProductName TEXT NOT NULL)", OfflineDBConstants.TABLE_PRODUCT));

            db.ExecSQL(string.Format(@"CREATE TABLE IF NOT EXISTS {0}(
                            PurposeID INTEGER,
                            PurposeName TEXT NOT NULL)", OfflineDBConstants.TABLE_PURPOSE));

            db.ExecSQL(string.Format(@"CREATE TABLE IF NOT EXISTS {0}(
                            LoginID TEXT NOT NULL,
                            Password TEXT NOT NULL,
                            Name TEXT NOT NULL,
                            InstallmentDate TEXT NOT NULL)", OfflineDBConstants.TABLE_USER));

            db.ExecSQL(string.Format(@"CREATE TABLE IF NOT EXISTS {0}(
                            OrganizationUrl  TEXT NOT NULL,
                            Name TEXT NOT NULL)", OfflineDBConstants.TABLE_ORGANIZATION));

            db.ExecSQL(string.Format(@"CREATE TABLE IF NOT EXISTS {0}(
                            ID INTEGER PRIMARY KEY AUTOINCREMENT,
                            ActionName TEXT NOT NULL,
                            ErrorText  TEXT NOT NULL,
                            InputParameters   TEXT NULL,
                            UserID TEXT NOT NULL,
                            CreateDate TEXT NOT NULL)", OfflineDBConstants.TABLE_SYSTEM_ERROR));
        }

        public override void OnUpgrade(SQLiteDatabase db, int oldVersion, int newVersion)
        {
            db.ExecSQL(string.Format("DROP TABLE IF EXISTS {0}", TABLE_NAME));
            db.ExecSQL(string.Format("DROP TABLE IF EXISTS {0}", OfflineDBConstants.TABLE_CENTER));
            db.ExecSQL(string.Format("DROP TABLE IF EXISTS {0}", OfflineDBConstants.TABLE_OFFICE));
            db.ExecSQL(string.Format("DROP TABLE IF EXISTS {0}", OfflineDBConstants.TABLE_MEMBER));
            db.ExecSQL(string.Format("DROP TABLE IF EXISTS {0}", OfflineDBConstants.TABLE_MEMBER_PRODUCT));
            db.ExecSQL(string.Format("DROP TABLE IF EXISTS {0}", OfflineDBConstants.TABLE_LOAN_COLLECTION));
            db.ExecSQL(string.Format("DROP TABLE IF EXISTS {0}", OfflineDBConstants.TABLE_LOAN_COLLECTION_Deleted)); //KHALID
            db.ExecSQL(string.Format("DROP TABLE IF EXISTS {0}", OfflineDBConstants.TABLE_LOAN_PROPOSAL));
            db.ExecSQL(string.Format("DROP TABLE IF EXISTS {0}", OfflineDBConstants.TABLE_PRODUCT));
            db.ExecSQL(string.Format("DROP TABLE IF EXISTS {0}", OfflineDBConstants.TABLE_PURPOSE));
            db.ExecSQL(string.Format("DROP TABLE IF EXISTS {0}", OfflineDBConstants.TABLE_USER));
            db.ExecSQL(string.Format("DROP TABLE IF EXISTS {0}", OfflineDBConstants.TABLE_WITHDRAWAL));

            OnCreate(db);
        }       
    }
}