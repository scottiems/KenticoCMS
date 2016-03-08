using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CMS.PortalControls;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using CMS.SettingsProvider;
using System.Text;

public partial class CMSWebParts_ThinkgateWebParts_StandardsBar : System.Web.UI.UserControl
{
    #region Public Properties

    public string DocumentType { get; set; }
    public string DocumentID { get; set; }
    public string DocumentTypeLabel { get; set; }
    public string ClassID { get; set; }
    public string ClientDatabaseName { get; set; }
    public int AssociationCount;
	public int CurriculaAssociatonCount;

    private static readonly string ConnectionString = ConfigurationManager.ConnectionStrings["root_application"].ConnectionString;
    private static readonly string CMSConnectionString = ConfigurationManager.ConnectionStrings["CMSConnectionString"].ConnectionString;


    #endregion


	//ctrlSearch objTestControl = (ctrlSearch)Page.FindControl("ctrlSearch"); 

    protected void Page_Load(object sender, EventArgs e)
    {
        if (this.DocumentType == null)
        {
            this.DocumentType = "InstructionPlan";


        }

		//this.Page


        if (this.DocumentID == null)
        {
            this.DocumentID = "1000";
            //this.DocumentID = (TreeHelper.SelectSingleNode(CMS.CMSHelper.CMSContext.CurrentDocument.NodeID).DocumentID).ToString();
        }
        if (this.ClientDatabaseName == null)
        {
            this.ClientDatabaseName = "Elements";

        }
        

      AssociationCount = GetCurrentAssociationCount();
	  CurriculaAssociatonCount = GetCurriculaAssociationCount();


    }


	private int GetCurriculaAssociationCount()
	{
		int result;
		string swrk = "";

		string sql = "select count (distinct curriculumID) as 'COUNT'from [dbo].[thinkgate_docToCurriculums] where (docID = '" + this.DocumentID.ToString() + "') ";
		DataTable dt = GetDataTable(ConfigurationManager.ConnectionStrings["CMSConnectionString"].ConnectionString, sql);
		if (dt.Rows.Count > 0)
		{
			StringBuilder sb = new StringBuilder();
			foreach (DataRow dr in dt.Rows)
			{
				string rowItem = dr["COUNT"].ToString();

				sb.Append(rowItem);

			}
			swrk = sb.ToString();
		}
		else
		{
			swrk = string.Empty;
		}

		if (Int32.TryParse(swrk, out result))
		{
			return result;
		}
		else
		{
			return 0;
		}

	}

	private int GetCurrentAssociationCount()
	{
		int result;
		string swrk = "";
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
			swrk = sb.ToString();
		}
		else
		{
			swrk = string.Empty;
		}

		if (Int32.TryParse(swrk, out result))
		{
			return result;
		}
		else
		{
			return 0;
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
                ConnectionStringToUse = ConnectionString;
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