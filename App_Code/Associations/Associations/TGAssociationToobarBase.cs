using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

public class TGAssociationToobarBase : System.Web.UI.UserControl
{

	#region "Public Properties"
	public static readonly string CMSConnectionString = ConfigurationManager.ConnectionStrings["CMSConnectionString"].ConnectionString;

	public string DocumentType { get; set; }
	public string DocumentID { get; set; }
	#endregion

	public DataTable GetDataTable(string connectionString, string query)
	{
		DataTable myDataTable = new DataTable();

		if (query != null)
		{
			string ConnectionStringToUse = string.Empty;

			if (connectionString != null)
			{
				ConnectionStringToUse = connectionString;
			}
			else
			{
				return myDataTable;
			}

			SqlConnection conn = new SqlConnection(ConnectionStringToUse);
			SqlDataAdapter adapter = new SqlDataAdapter();
			adapter.SelectCommand = new SqlCommand(query, conn);

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
		return myDataTable;
	}

}
