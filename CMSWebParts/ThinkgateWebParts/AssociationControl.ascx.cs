
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CMS.PortalControls;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

public partial class CMSTemplates_CorporateSite_AssociationControl : AssociationControlBase, IAssociationParameters
{
    #region Private Variables

    private static string customTableClassName = "TG.DocumentPlanAssociation";
    private static readonly string ConnectionString = ConfigurationManager.ConnectionStrings["root_application"].ConnectionString;
    private static readonly string CMSConnectionString = ConfigurationManager.ConnectionStrings["CMSConnectionString"].ConnectionString;
    int AssociationCount;

    #endregion

    #region Public Properties

    public string DocumentID { get; set; }
    public string DocumentType { get; set; }
    public string UserID { get; set; }
  
    #endregion

    #region Event Methods

    protected void Page_Load(object sender, EventArgs e)
    {
        IAssociationParameters parameters = base.GetParentAssociationParameters();

        if (string.IsNullOrWhiteSpace(this.DocumentID))
        { this.DocumentID = parameters.DocumentID; }

        if (string.IsNullOrWhiteSpace(this.DocumentType))
        { this.DocumentType = parameters.DocumentType; }

        if (string.IsNullOrWhiteSpace(this.UserID))
        { this.UserID = parameters.UserID; }

        lnkAC.Text = this.Text;
        AssociationCount = GetCurrentAssociationCount();

        if (AssociationCount == 0)
        { this.AssociationDataListURL = null; }
        DocType documentType = (DocType)Enum.Parse(typeof(DocType), this.DocumentType);
        //DocType documentType = DocType.InstructionPlan;
        badge_div.InnerHtml = AssociationCount.ToString();
		lnkIAC.Attributes.Add("class", GetImagePath());
        toolBarItem_div.Attributes.Add("onclick", "openModalAssocToolBarDialog(event, '" + this.DocumentID + "', '" + documentType + "', '" + this.UserID + "', '" + AssociationCount.ToString() + "', '" + this.Category.ToString() + "', '" + this.URL + "', '" + this.AssociationDataListURL + "','" + this.AssociationDataListDialogTitle + "','" + badge_div.ClientID + "')");
    }

    #endregion

  

    #region Private Methods

    private string GetImagePath()
    {
        //int associationCount = GetCurrentAssociationCount();
        //div11.InnerHtml = associationCount.ToString();
        string imageurl = string.Empty;
        switch (this.Category)
        {
            case AssociationCategory.Curricula:
                {
                    return "defaultIcon curriculaIcon";
                }
            case AssociationCategory.Standard:
                {
                    return "defaultIcon standardsIcon";
                }
            case AssociationCategory.LRMI:
                {
                    return "defaultIcon tagsIcon";
                }
            case AssociationCategory.Resource:
                {
                    return "defaultIcon curriculaIcon";
                }
            case AssociationCategory.Schedule:
                {
                    return "defaultIcon curriculaIcon";
                }
            case AssociationCategory.Assessment:
                {
                    return "defaultIcon curriculaIcon";
                }
            default:
                {
                    return "defaultIcon";
                }
        }
    }

    private int GetCurrentAssociationCount()
    {
        DocType documentType = (DocType)Enum.Parse(typeof(DocType), this.DocumentType);

        //DocType documentType = DocType.InstructionPlan;

        switch (this.Category)
        {
            case AssociationCategory.Curricula:
                {
                    string query = string.Format("SELECT * FROM  [dbo].[TG_DocumentPlanAssociation] where DocumentID = {0} and DocumentTypeEnum = {1} and AssociationCategoryEnum = {2}", int.Parse(this.DocumentID), (int)documentType, (int)AssociationCategory.Curricula);
                    DataTable resultDataTable = GetDataTable(CMSConnectionString, query);
                    return resultDataTable.Rows.Count;
                }
            case AssociationCategory.LRMI:
                {
                    return 5;
                }
            case AssociationCategory.Resource:
                {
                    string query = string.Format("SELECT * FROM  [dbo].[TG_DocumentPlanAssociation] where DocumentID = {0} and DocumentTypeEnum = {1} and AssociationCategoryEnum = {2}", int.Parse(this.DocumentID), (int)documentType, (int)AssociationCategory.Resource);
                    DataTable resultDataTable = GetDataTable(CMSConnectionString, query);
                   return resultDataTable.Rows.Count;
                }
            case AssociationCategory.Standard:
                {
                    string query = string.Format("SELECT * FROM  [dbo].[TG_DocumentPlanAssociation] where DocumentID = {0} and DocumentTypeEnum = {1} and AssociationCategoryEnum = {2}", int.Parse(this.DocumentID), (int)documentType, (int)AssociationCategory.Standard);
                    DataTable resultDataTable = GetDataTable(CMSConnectionString, query);
                    return resultDataTable.Rows.Count;
                }
            case AssociationCategory.Schedule:
                {
                    return 4;
                }
            default:
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
    #endregion