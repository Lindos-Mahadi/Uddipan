using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Repository
{
    public class MsSqlDbHelper : IDbHelper
    {
        public int ExecuteNonQuery(Database db, CommandType cmdType, string sqlText, Dictionary<string, object> parameters)
        {
            var cmd = new SqlCommand(sqlText, (SqlConnection)db.Connection) { CommandType = cmdType };
            if (parameters != null)
            {
                foreach (KeyValuePair<string, object> parameter in parameters)
                {
                    cmd.Parameters.Add(new SqlParameter(parameter.Key, parameter.Value));
                }
            }

            if (db.Connection.State != ConnectionState.Open)
            {
                cmd.Connection.Open();
            }
            var queryReturnValue = cmd.ExecuteNonQuery();
            cmd.Connection.Close();
            cmd.Dispose();
            return queryReturnValue;
        }

        public DataTable ExecuteQuery(Database db, CommandType cmdType, string sqlText, Dictionary<string, object> parameters)
        {
            var cmd = new SqlCommand(sqlText, (SqlConnection)db.Connection) { CommandType = cmdType };
            if (parameters != null)
            {
                foreach (KeyValuePair<string, object> parameter in parameters)
                {
                    cmd.Parameters.Add(new SqlParameter(parameter.Key, parameter.Value));
                }
            }

            if (db.Connection.State != ConnectionState.Open)
            {
                cmd.Connection.Open();
            }

            //var queryReturnValue = cmd.ExecuteNonQuery();
            var dataAdapter = new SqlDataAdapter(cmd);
            var dt = new DataTable();
            dataAdapter.Fill(dt);
            cmd.Connection.Close();
            dataAdapter.Dispose();
            cmd.Dispose();
            return dt;
        }
    }
}
