// C:\Project\LibraryManagementSystem\DBManager.cs
using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections.Generic;

namespace LibraryManagementSystem
{
    public class DBManager
    {
        private SqlConnection myConnection;

        public DBManager()
        {
            var builder = new SqlConnectionStringBuilder(ConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString);
            builder.TrustServerCertificate = true; // strongly-typed setter
            myConnection = new SqlConnection(builder.ConnectionString);

            try
            {
                myConnection.Open();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        // INSERT / UPDATE / DELETE (string query)
        public int ExecuteNonQuery(string query)
        {
            try
            {
                using (SqlCommand myCommand = new SqlCommand(query, myConnection))
                {
                    return myCommand.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 0;
            }
        }

        // INSERT / UPDATE / DELETE (parameterized command)
        public int ExecuteNonQuery(SqlCommand command)
        {
            try
            {
                command.Connection = myConnection;
                return command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 0;
            }
        }

        // SELECT that returns a single value (COUNT, MAX, etc.) - string
        public object ExecuteScalar(string query)
        {
            try
            {
                using (SqlCommand myCommand = new SqlCommand(query, myConnection))
                {
                    return myCommand.ExecuteScalar();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 0;
            }
        }

        // SELECT that returns a single value (parameterized)
        public object ExecuteScalar(SqlCommand command)
        {
            try
            {
                command.Connection = myConnection;
                return command.ExecuteScalar();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 0;
            }
        }

        // SELECT that returns a table of rows (string query)
        public DataTable ExecuteReader(string query)
        {
            try
            {
                using (SqlCommand myCommand = new SqlCommand(query, myConnection))
                using (SqlDataReader reader = myCommand.ExecuteReader())
                {
                    DataTable dt = new DataTable();
                    if (reader.HasRows)
                        dt.Load(reader);
                    return dt; // returns empty table if no rows
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new DataTable();
            }
        }

        // Overload: accepts a pre-configured SqlCommand (parameterized)
        public DataTable ExecuteReader(SqlCommand command)
        {
            try
            {
                command.Connection = myConnection;
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    DataTable dt = new DataTable();
                    if (reader.HasRows)
                        dt.Load(reader);
                    return dt;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new DataTable();
            }
        }

        // Execute multiple commands in a transaction. Commands must be configured (text + parameters).
        // Returns total affected rows (0 on failure).
        public int ExecuteTransaction(IEnumerable<SqlCommand> commands)
        {
            if (commands == null)
                return 0;

            SqlTransaction tx = null;
            try
            {
                tx = myConnection.BeginTransaction();
                int total = 0;
                foreach (var cmd in commands)
                {
                    cmd.Connection = myConnection;
                    cmd.Transaction = tx;
                    total += cmd.ExecuteNonQuery();
                }

                tx.Commit();
                return total;
            }
            catch (Exception ex)
            {
                try
                {
                    tx?.Rollback();
                }
                catch { /* ignore rollback errors */ }

                Console.WriteLine(ex.Message);
                return 0;
            }
        }

        public void CloseConnection()
        {
            try
            {
                myConnection.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}