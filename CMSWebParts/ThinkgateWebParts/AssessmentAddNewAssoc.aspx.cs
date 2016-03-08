using CMS.GlobalHelper;
using CMS.SettingsProvider;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Text;

public partial class CMSWebParts_ThinkgateWebParts_AssessmentAddNewAssoc : System.Web.UI.Page
{
    private readonly string ConnectionString = ConfigurationManager.ConnectionStrings["root_application"].ConnectionString;
    private readonly string CMSConnectionString = ConfigurationManager.ConnectionStrings["CMSConnectionString"].ConnectionString;

    public string DocumentID
    {
        get
        {
            return QueryHelper.GetString("parentnodeid", "");
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        DocID.Value = this.DocumentID.ToString();
    }

    private DataTable GetDataTable(string connectionString, string query)
    {
        DataTable dataTable = new DataTable();

        if (query != null)
        {
            connectionString = connectionString == null ? CMSConnectionString : connectionString;

            SqlConnection sqlConnection = new SqlConnection(connectionString);
            SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
            sqlDataAdapter.SelectCommand = new SqlCommand(query, sqlConnection);

            try
            {
                sqlConnection.Open();
            }
            catch (SqlException sqlException)
            {
                Debug.WriteLine("SqlException: " + sqlException.Message);
                return dataTable;
            }

            try
            {
                sqlDataAdapter.Fill(dataTable);
            }
            finally
            {
                sqlConnection.Close();
            }
        }
        return dataTable;
    }

    protected void DelSelectedItems_Click(object sender, EventArgs e)
    {
        ModifyAssessmentAssociations("Thinkgate_DeleteAssessmentsValues");
    }

    protected void AddSelectedItems_Click(object sender, EventArgs e)
    {
        ModifyAssessmentAssociations("Thinkgate_AddAssessmentsValues");
    }

    private void ModifyAssessmentAssociations(string storedProcedureName)
    {
        string[] selectedItems = (SelectedItems.Value).Split(',');
        var assessments = new dtGeneric_Int();
        for (var i = 0; i < selectedItems.Length; i++)
        {
            int nodeId;
            string docEntry = selectedItems[i];
            if (Int32.TryParse(docEntry, out nodeId))
            {
                assessments.Add(nodeId);
            }
        }
        if (assessments.Count > 0)
        {
            SqlParameterCollection sqlParameterCollection = new SqlCommand().Parameters;
            SqlParameter sqlParameter = new SqlParameter("@DocumentId", SqlDbType.Int);
            sqlParameter.Value = this.DocumentID;
            sqlParameterCollection.Add(sqlParameter);

            sqlParameter = new SqlParameter("@AssessmentIDs", SqlDbType.Structured);
            sqlParameter.Value = assessments.ToSql().Data;
            sqlParameter.TypeName = assessments.ToSql().TypeName;
            sqlParameterCollection.Add(sqlParameter);

            ThinkgateKenticoHelper.Connection_StoredProc_DataTable(storedProcedureName, sqlParameterCollection, CMSConnectionString);
        }
        SelectedItems.Value = null;
    }
}