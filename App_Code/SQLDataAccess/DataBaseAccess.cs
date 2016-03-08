using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;


namespace SQLDataAcess
{
    /// <summary>
    /// Summary description for DataBaseAccess
    /// </summary>
    public class DataBaseAccess
    {
        private static readonly string CMSConnectionString = ConfigurationManager.ConnectionStrings["CMSConnectionString"].ConnectionString;
        private static readonly string ThinkgateConfigConnectionString = ConfigurationManager.ConnectionStrings["root_application"].ConnectionString;

        
        public DataTable GetDataTableUsingQueryFromKenticoDB(string query)
        {
            DataTable myDataTable = new DataTable();

            if (query != null)
            {
                SqlConnection conn = new SqlConnection(CMSConnectionString);
                SqlDataAdapter adapter = new SqlDataAdapter {SelectCommand = new SqlCommand(query, conn)};

                try
                {
                    conn.Open();
                }
                catch (SqlException ex)
                {
                    //Debug.WriteLine("SqlException: " + ex.Message);
                    return myDataTable;
                }

                try
                {
                    adapter.Fill(myDataTable);
                }
                finally
                {
                    conn.Close();
                }
            }
            return myDataTable;
        }

        public DataTable GetDataTableUsingQueryFromClientDB(string query)
        {
            DataTable myDataTable = new DataTable();

            if (query != null)
            {
                SqlConnection conn = new SqlConnection(ThinkgateKenticoHelper.getLocalDbConnectionString(ThinkgateConfigConnectionString));
                SqlDataAdapter adapter = new SqlDataAdapter { SelectCommand = new SqlCommand(query, conn) };

                try
                {
                    conn.Open();
                }
                catch (SqlException ex)
                {
                    //Debug.WriteLine("SqlException: " + ex.Message);
                    return myDataTable;
                }

                try
                {
                    adapter.Fill(myDataTable);
                }
                finally
                {
                    conn.Close();
                }
            }
            return myDataTable;
        }

        public DataTable GetDataTableUsingQueryFromThinkgateConfigDB(string query)
        {
            DataTable myDataTable = new DataTable();

            if (query != null)
            {
                SqlConnection conn = new SqlConnection(ThinkgateConfigConnectionString);
                SqlDataAdapter adapter = new SqlDataAdapter { SelectCommand = new SqlCommand(query, conn) };

                try
                {
                    conn.Open();
                }
                catch (SqlException ex)
                {
                    //Debug.WriteLine("SqlException: " + ex.Message);
                    return myDataTable;
                }

                try
                {
                    adapter.Fill(myDataTable);
                }
                finally
                {
                    conn.Close();
                }
            }
            return myDataTable;
        }

        public DataTable GetDataTableUsingQuery(string connectionString, string query)
        {
            DataTable myDataTable = new DataTable();

            if (query != null)
            {
                if (connectionString != null)
                {

                    SqlConnection conn = new SqlConnection(connectionString);
                    SqlDataAdapter adapter = new SqlDataAdapter {SelectCommand = new SqlCommand(query, conn)};

                    try
                    {
                        conn.Open();
                    }
                    catch (SqlException ex)
                    {
                        return myDataTable;
                    }

                    try
                    {
                        adapter.Fill(myDataTable);
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
            return myDataTable;
        }

        public DataTable ExecuteStoreProcedureInClientDB(string storedProcedure, SqlParameterCollection sqlParamList = null, string clientId = null)
        {
            SqlConnection connection = null;

            try
            {
                connection = clientId == null ? new SqlConnection(ThinkgateKenticoHelper.getLocalDbConnectionString(ThinkgateConfigConnectionString)) 
                    : new SqlConnection(ThinkgateKenticoHelper.getLocalDbConnectionString(ThinkgateConfigConnectionString, clientId));
               
                SqlCommand sqlCommand = new SqlCommand(storedProcedure, connection){CommandType = CommandType.StoredProcedure};
                if (sqlParamList != null)
                    foreach (SqlParameter parm in sqlParamList)
                    {
                        SqlParameter sqlParm = new SqlParameter(parm.ParameterName, parm.SqlDbType)
                        {
                            Value = parm.Value,
                            TypeName = parm.TypeName
                        };
                        sqlCommand.Parameters.Add(sqlParm);
                    }

                SqlDataAdapter adapter = new SqlDataAdapter {SelectCommand = sqlCommand};

                DataTable table = new DataTable();
                adapter.Fill(table);

                return table;
            }
            catch (DataException ex)
            {
                Debug.WriteLine(ex.ToString());
                throw;
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
        }

        public DataTable ExecuteStoreProcedureInKenticoDB(string storedProcedure, SqlParameterCollection sqlParamList = null)
        {
            SqlConnection connection = null;

            try
            {
                connection = new SqlConnection(CMSConnectionString);

                SqlCommand sqlCommand = new SqlCommand(storedProcedure, connection){CommandType = CommandType.StoredProcedure};
                if (sqlParamList != null)
                    foreach (SqlParameter parm in sqlParamList)
                    {
                        SqlParameter sqlParm = new SqlParameter(parm.ParameterName, parm.SqlDbType)
                        {
                            Value = parm.Value,
                            TypeName = parm.TypeName
                        };
                        sqlCommand.Parameters.Add(sqlParm);
                    }

                SqlDataAdapter adapter = new SqlDataAdapter {SelectCommand = sqlCommand};

                DataTable table = new DataTable();
                adapter.Fill(table);

                return table;
            }
            catch (DataException ex)
            {
                Debug.WriteLine(ex.ToString());
                throw;
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
        }

        
        
    }
}