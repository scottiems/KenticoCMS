using CMS.GlobalHelper;
using CMS.SettingsProvider;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Text;

public partial class CMSWebParts_ThinkgateWebParts_StdAddNewAssoc : System.Web.UI.Page
{
	public int Assoccount;

	//private static string customTableClassName = "TG.DocumentPlanAssociation";
	private readonly string ConnectionString = ConfigurationManager.ConnectionStrings["root_application"].ConnectionString;
	private readonly string CMSConnectionString = ConfigurationManager.ConnectionStrings["CMSConnectionString"].ConnectionString;

	public int AssociationCount;
    public bool FromLRMI = false;

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
        FromLRMI = QueryHelper.GetString("ReturnURL", string.Empty).Equals("LRMI", StringComparison.InvariantCultureIgnoreCase);
        if (FromLRMI == true)
        {
            btnAddNewStandards.Visible = false;
            btnAddNewStandardsFromLRMI.Visible = true;
        }
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
        string docid = this.DocumentID;

        for (var i = 0; i < selitems.Length; i++)
        {
            int nodeid;
            string docEntry = selitems[i];
            if (Int32.TryParse(docEntry, out nodeid))
            {
                string retQuery = string.Format("select count(distinct standardID) count from [dbo].[thinkgate_docToStandards] where docID='{0}' AND standardID = '{1}' ", docid, nodeid);
                DataTable dt = GetDataTable(CMSConnectionString, retQuery);
                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        if (Convert.ToInt32(dt.Rows[0]["count"]) == 0)
                        {
                            string query = string.Format("Insert into [dbo].[thinkgate_docToStandards] (docID, standardID) values ('{0}', {1})", docid, nodeid);
                            GetDataTable(CMSConnectionString, query);
                        }
                    }
                }
                else//May Not required.
                {
                    string query = string.Format("Insert into [dbo].[thinkgate_docToStandards] (docID, standardID) values ('{0}', {1})", docid, nodeid);
                    GetDataTable(CMSConnectionString, query);
                }
            }
        }
        SelectedItems.Value = string.Empty;
    }


	protected void DelSelectedItems_Click(object sender, EventArgs e)
	{
		//string cnt = GetCurrentAssociationCount();

		string[] selitems = (SelectedItems.Value).Split(',');
		for (var i = 0; i < selitems.Length; i++)
		{
			int nodeid;
			string docEntry = selitems[i];
			if (Int32.TryParse(docEntry, out nodeid))
			{
                ThinkgateKenticoHelper.deleteStandard(Convert.ToInt32(this.DocumentID), nodeid, true);
			}
		}
        SelectedItems.Value = string.Empty;
	}

	private string GetCurrentAssociationCount()
	{
		string sql = "select count (*) as 'COUNT' from [dbo].[thinkgate_docToStandards] where (docID = '" + this.DocumentID.ToString() + "') ";
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