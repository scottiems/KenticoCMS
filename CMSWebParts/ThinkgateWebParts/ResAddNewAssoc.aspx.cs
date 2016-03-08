using CMS.GlobalHelper;
using CMS.SettingsProvider;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Text;

public partial class CMSWebParts_ThinkgateWebParts_ResAddNewAssoc : System.Web.UI.Page
{
	public int Assoccount;

	private readonly string ConnectionString = ConfigurationManager.ConnectionStrings["root_application"].ConnectionString;
	private readonly string CMSConnectionString = ConfigurationManager.ConnectionStrings["CMSConnectionString"].ConnectionString;

	public int AssociationCount;

    const string RESOURCE_TABLE = "thinkgate_docToResources";

	public string DocumentType
	{
		get
		{
			return QueryHelper.GetString("doctype", "");
		}
	}

	public string DocumentID
	{
		get
		{
			return QueryHelper.GetString("parentnodeid", "");
		}
	}

	public string ClientDatabaseName
	{
		get
		{
			return QueryHelper.GetString("clientid", "");
		}
	}

	protected void Page_Load(object sender, EventArgs e)
	{
		DocID.Value = this.DocumentID.ToString();
	}


	private DataTable GetDataTable(string connectionString, string query)
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
				ConnectionStringToUse = CMSConnectionString;
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
				Debug.WriteLine("SqlException: " + ex.Message);
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


	protected void AddSelectedItems_Click(object sender, EventArgs e)
	{
		string[] selitems = (SelectedItems.Value).Split(',');
		for (var i = 0; i < selitems.Length; i++)
		{
			int nodeid;
			string docEntry = selitems[i];
			if (Int32.TryParse(docEntry, out nodeid))
			{
                if (!SelectedRowExists(nodeid))
                {
                    string query = string.Format("Insert into [dbo].[{0}] (docID, ResourceID) values ('{1}', {2})", RESOURCE_TABLE, this.DocumentID, nodeid);
                    GetDataTable(CMSConnectionString, query);
                }
			}
		}
        SelectedItems.Value = string.Empty;
	}

    /// <summary>
    /// To check duplicate row before inserting any record into DB Table. Bug#24790 fix.
    /// </summary>
    /// <returns></returns>
    protected bool SelectedRowExists(int nodeID)
    {
        string sqlQuery = string.Format("Select DocID,ResourceID from [dbo].[{0}] where docID='{1}' and ResourceID={2}", RESOURCE_TABLE, this.DocumentID, nodeID);
        DataTable dt = GetDataTable(ConfigurationManager.ConnectionStrings["CMSConnectionString"].ConnectionString, sqlQuery);
        return dt.Rows.Count > 0 ? true : false;
    }

	protected void DelSelectedItems_Click(object sender, EventArgs e)
	{
		string[] selitems = (SelectedItems.Value).Split(',');
		for (var i = 0; i < selitems.Length; i++)
		{
			int nodeid;
			string docEntry = selitems[i];
			if (Int32.TryParse(docEntry, out nodeid))
			{
                string query = string.Format("Delete from [dbo].[{0}] WHERE (docID={1} AND resourceID={2}) ", RESOURCE_TABLE, this.DocumentID, nodeid);
				GetDataTable(CMSConnectionString, query);
			}
		}
        SelectedItems.Value = string.Empty;
	}

	private string GetCurrentAssociationCount()
	{
        string sql = string.Format("select count (*) as 'COUNT' from [dbo].[{0}] where (docID = '{1}'", RESOURCE_TABLE, this.DocumentID.ToString());
		DataTable dt = GetDataTable(ConfigurationManager.ConnectionStrings["CMSConnectionString"].ConnectionString, sql);
		if (dt.Rows.Count > 0)
		{
			StringBuilder sb = new StringBuilder();
			foreach (DataRow dr in dt.Rows)
			{
				string rowItem = dr["COUNT"].ToString();

				sb.Append(rowItem);

			}
			return sb.ToString();
		}
		else
		{
			return string.Empty;
		}
	}
}
