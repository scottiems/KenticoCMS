using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Web.UI.WebControls;
using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.PortalControls;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using Telerik.Web.UI;

public partial class CMSFormControls_Selectors_Thinkgate_AssociationDataList : System.Web.UI.Page
{
    #region Private Variables

    private static string customTableClassName = "TG.DocumentPlanAssociation";
    private static readonly string CMSConnectionString = ConfigurationManager.ConnectionStrings["CMSConnectionString"].ConnectionString;
    
    #endregion


    #region Public Variables
    public int AssociationCount;
    #endregion

    #region Public Properties

    public AssociationCategory Category { get; set; }
    public string RedirectURL { get; set; }
    
    
    #endregion

    #region Private Properties

    private string DocumentID { get; set; }
    private DocType DocumentType { get; set; }
    private string UserID { get; set; }

    #endregion

    #region Event Methods

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(Request.QueryString.Get("documentId")))
        { this.DocumentID = Request.QueryString.Get("documentId");}

        if (!string.IsNullOrWhiteSpace(Request.QueryString.Get("documentType")))
        { this.DocumentType = (DocType)Enum.Parse(typeof(DocType), Request.QueryString.Get("documentType"));}

        if (!string.IsNullOrWhiteSpace(Request.QueryString.Get("userId")))
        { this.UserID = Request.QueryString.Get("userId"); }

        if (!string.IsNullOrWhiteSpace(Request.QueryString.Get("category")))
        { this.Category = (AssociationCategory)Enum.Parse(typeof(AssociationCategory), Request.QueryString.Get("category"));}
                
        if (!string.IsNullOrWhiteSpace(Request.QueryString.Get("url")))
        { RedirectURL = Request.QueryString.Get("url"); }

        GetDocumentAssociations();
    }

    protected void lnkDeleteAssociation_Command(object sender, CommandEventArgs e)
    {
        switch (e.CommandName)
        {
            case "Delete":
                DeleteAssociationItem(int.Parse(e.CommandArgument.ToString())); 
                GetDocumentAssociations(); 
               
                break;
        }
    }

    #endregion

    #region Private Methods

        
    protected void GetDocumentAssociations()
    {
        QueryDataParameters queryDataParameters = new QueryDataParameters();

        queryDataParameters.Add("DocumentID", this.DocumentID);
        queryDataParameters.Add("DocumentType", (int)this.DocumentType);
        queryDataParameters.Add("UserId", null);
        queryDataParameters.Add("AssociationCategory", (int)this.Category);

        DataSet resultDataSet = CMS.DataEngine.ConnectionHelper.ExecuteQuery("Thinkgate_GetDocumentPlanAssociationDetails", queryDataParameters, QueryTypeEnum.StoredProcedure, false);
        DataTable resultDataTable = resultDataSet.Tables[0];

        CustomTableItemProvider customTableProvider = new CustomTableItemProvider(CMSContext.CurrentUser);
        AssociationCount = resultDataTable.Rows.Count;
        rptAssociationDetail.DataSource = resultDataTable;
        rptAssociationDetail.DataBind();

    }

    private void DeleteAssociationItem(int associationID)
    {
        if (associationID >= 0)
        {
            string query = string.Format("Delete from [dbo].[TG_DocumentPlanAssociation] WHERE DocumentID={0} AND DocumentTypeEnum={1} AND AssociationCategoryEnum={2} AND AssociationID={3} ", this.DocumentID, (int)this.DocumentType, (int)this.Category, associationID);
            GetDataTable(CMSConnectionString, query);
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

    #endregion

}