using System.Collections.Generic;
using PMS.Droid.Classes.OffLineHelpers;

namespace PMS.Droid.Helpers
{
    public class SessionHelper
    {
        public static string LoggedInUserID { get; set; }
        public static int OfficeID { get; set; }
        public static int OrgID { get; set; }
        public static string OfficeName { get; set; }
        public static List<ProductModel> ProductList { get; set; }
        public static List<PurposeModel> PurposeList { get; set; }
        public static List<CenterModel> CenterList { get; set; }
    }
}