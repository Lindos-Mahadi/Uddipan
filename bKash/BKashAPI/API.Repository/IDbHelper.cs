using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Repository
{
    public interface IDbHelper
    {
        int ExecuteNonQuery(Database db, CommandType cmdType, string sqlText, Dictionary<string, object> parameters);
        DataTable ExecuteQuery(Database db, CommandType cmdType, string sqlText, Dictionary<string, object> parameters);
    }
}
