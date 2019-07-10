using System.Collections.Generic;
using System.Data.SqlClient;
using System.Dynamic;

namespace Eze.AdminConsole.Database
{
    public class DatabaseRunner
    {
        static private string GetConnectionString()
        {
            return "Data Source=marston9020b.ezesoft.net;Initial Catalog=TC;"
                + "User Id=sa;Password=ezetc;";
        }
        public List<dynamic> RunDiagnostics()
        {
            string queryString = "exec sp_Blitz";
            var sqlResult = new List<dynamic>();
            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                try
                {
                    while (reader.Read())
                    {
                        if (reader[3].ToString() == "TC")
                        {
                            dynamic expando = new ExpandoObject();
                            expando.loginName = reader[3].ToString();
                            expando.programName = "";
                            expando.queryText = reader[5].ToString();
                            sqlResult.Add(expando);
                        }
                    }
                }
                finally
                {
                    reader.Close();
                }
            }
            return sqlResult;
        }
        public dynamic WhatsRunning()
        {
            string queryString = "exec sp_BlitzWho";
            dynamic sqlResult = new ExpandoObject();
            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                try
                {
                    while (reader.Read())
                    {
                        if (reader[3].ToString() == "TC")
                        {
                            sqlResult.loginName = reader[16].ToString();
                            sqlResult.programName = reader[18].ToString();
                            sqlResult.queryText = reader[4].ToString();
                        }
                    }
                }
                finally
                {
                    reader.Close();
                }
            }
            return sqlResult;
        }
    }
}