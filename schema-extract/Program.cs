using Newtonsoft.Json;
using System;
using System.Data.SqlClient;
using System.IO;
using System.Net;

namespace eze.schema.extract
{
    class Program
    {
        static void Main(string[] args)
        {
            //set the connection string
            string connString = @"Server=.; Database=TC; Trusted_Connection=True;";
            // connString = @"Server=ultron;Database=TC2;User Id=sa; Password=redsox;";

            try
            {
                //sql connection object
                using (SqlConnection conn = new SqlConnection(connString))
                {

                    //set stored procedure
                    SqlCommand cmd = new SqlCommand(SqlConstants.SqlQuery, conn);
                    conn.Open();
                    SqlDataReader dr = cmd.ExecuteReader();

                    Console.WriteLine(Environment.NewLine + "Retrieving data from database..." + Environment.NewLine);

                    var schemaResult = new SchemaData();
                    if (dr.HasRows)
                    {
                        while (dr.Read()) // && schemaResult.Objects.Count < 1)
                        {
                            DbObject obj = new DbObject()
                            {
                                Name = dr.GetString(0),
                                Type = dr.GetString(1),
                                CheckSum = dr.IsDBNull(2) ? 0 : dr.GetInt32(2),
                                Definition = dr.IsDBNull(3) ? "" : dr.GetString(3)
                            };
                            schemaResult.Objects.Add(obj);
                        }
                        PostResults(schemaResult);
                    }
                    else
                    {
                        Console.WriteLine("No data found.");
                    }
                    dr.Close();
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                //display error message
                Console.WriteLine("Exception: " + ex.Message);
            }


        }

        static void PostResults(SchemaData schemaResult)
        {
            schemaResult.Client = "*2019.4";
            schemaResult.Version = "2019.4";

            string json = JsonConvert.SerializeObject(schemaResult);
            Console.WriteLine("POSTing results...");
            File.WriteAllText("output.json", json);

            var httpWebRequest = (HttpWebRequest)WebRequest.Create("http://localhost:5000/api/schema/" + schemaResult.Client);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                streamWriter.Write(json);
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
            }
        }
    }
}