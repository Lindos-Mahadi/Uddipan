using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using PMS.Droid.Classes.OffLineHelpers;

namespace PMS.Droid.Classes.Helpers
{
    class FileHelper
    {
        public static void SqliteToCsv(List<LoanCollectionModel> dataList, string path, bool initialRow = false, string[] columns = null)
        {

            if (dataList.Any())
            {
                StringBuilder sb = new StringBuilder();

                string rows = (columns.Length > 0) ? string.Join(",", columns) : "";
                StreamWriter sw;
                if (initialRow)
                {
                    sw = File.AppendText(path);
                }
                else
                {
                    sw = File.AppendText(path);
                }

                sw.WriteLine(rows);
                sw.Flush();

                dataList.ForEach((LoanCollectionModel loanCollection) =>
                {
                    string line = loanCollection.CollectionID + "," +
                         loanCollection.OfficeID + "," +
                         loanCollection.OfficeName + "," +
                         loanCollection.CenterID + "," +
                         loanCollection.CenterName + "," +
                         loanCollection.ProductID + "," +
                         loanCollection.ProductName.Replace(",", "-") + "," +
                         loanCollection.MemberCode + "," +
                         loanCollection.MemberID + "," +
                         loanCollection.Amount + "," +
                         loanCollection.DueAmount + "," +
                         loanCollection.TrxType + "," +
                         loanCollection.ProductType + "," +
                         loanCollection.SyncFlag + "," +
                         loanCollection.SummaryID + "," +
                         loanCollection.LoanInstallment + "," +
                         loanCollection.IntInstallment + "," +
                         loanCollection.IntCharge + "," +
                         loanCollection.Created + "," +
                         loanCollection.Token;
                    sw.WriteLine(line);
                    sw.Flush();
                });

                sw.Close();

                MediaScannerConnection.ScanFile(Android.App.Application.Context, new string[] { path }, null, null);
            }
        }
    }
}