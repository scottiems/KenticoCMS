using CMS.GlobalHelper;
using CMS.SettingsProvider;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Text;

public partial class CMSWebParts_ThinkgateWebParts_CurriculaAddNewAssoc : System.Web.UI.Page
{
	public int Assoccount;

	private readonly string ConnectionString = ConfigurationManager.ConnectionStrings["root_application"].ConnectionString;
	private readonly string CMSConnectionString = ConfigurationManager.ConnectionStrings["CMSConnectionString"].ConnectionString;

	public int AssociationCount;

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
        hdnDocType.Value = this.DocumentType.ToString()!=""?this.DocumentType.ToString().ToLower():this.DocumentType.ToString();

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
				string query = string.Format("Insert into [dbo].[thinkgate_docToCurriculums] (docID, curriculumID) values ('{0}', {1})", this.DocumentID, nodeid);
				GetDataTable(CMSConnectionString, query);
			}
		}
		SelectedItems.Value = null;
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
				string query = string.Format("Delete from [dbo].[thinkgate_docToCurriculums] WHERE (docID={0} AND curriculumID={1}) ", this.DocumentID, nodeid);
				GetDataTable(CMSConnectionString, query);
			}
		}
		SelectedItems.Value = null;
	}


	private string GetCurrentAssociationCount()
	{
        string sql = "select distinct count (distinct curriculumid) as 'COUNT' from [dbo].[thinkgate_docToCurriculums] where (docID = '" + this.DocumentID.ToString() + "') ";
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


	private int getDocType()
	{
		int doctype = 0;
		if (DocumentType == "InstructionPlan" || DocumentType == "InstructionalPlan")
		{
			doctype = (int)DocType.InstructionPlan;
		}
		if (DocumentType == "UnitPlan")
		{
			doctype = (int)DocType.UnitPlan;
		}
		if (DocumentType == "LessonPlan")
		{
			doctype = (int)DocType.LessonPlan;
		}

		return doctype;
	}
}