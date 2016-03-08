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
using System.Web.UI.HtmlControls;


using CMS.DocumentEngine;


using System.Collections.Generic;
using System.Web.Script.Serialization;
using System.Web.UI;


public partial class CMSWebParts_ThinkgateWebParts_StandardAssocList :System.Web.UI.Page
{
   
    public int Assoccount;


    private static string customTableClassName = "TG.DocumentPlanAssociation";
    private static readonly string ConnectionString = ConfigurationManager.ConnectionStrings["root_application"].ConnectionString;
    private static readonly string CMSConnectionString = ConfigurationManager.ConnectionStrings["CMSConnectionString"].ConnectionString;



    
          protected string DocumentType
		{
			get
			{
				return QueryHelper.GetString("doctype", "");
			}
		}

		 protected string DocumentID
		{
			get
			{
				return QueryHelper.GetString("parentnodeid", "");
			}
		}

   protected string ClientDatabaseName
		{
			get
			{
				return QueryHelper.GetString("client", "");
			}
		}

 protected void Page_Load(object sender, EventArgs e)
 {

     Assoccount = GetCurrentAssociationCount();
     GetStandardsAssoc();
     
 }

 protected void lnkStdDeleteAssociation_Command(object sender, CommandEventArgs e)
 {
     switch (e.CommandName)
     {
         case "Delete":
             DeleteStandardsAssocItem(int.Parse(e.CommandArgument.ToString()));
             GetStandardsAssoc();
             
             break;
     }
 }


 protected void GetStandardsAssoc()
 {
     QueryDataParameters queryDataParameters = new QueryDataParameters();

     queryDataParameters.Add("DocumentID", this.DocumentID.ToString());
     queryDataParameters.Add("DocumentType", (int)DocType.InstructionPlan);
     queryDataParameters.Add("UserId", null);
     queryDataParameters.Add("AssociationCategory", (int)AssociationCategory.Standard);
     queryDataParameters.Add("ClientDatabaseName", ClientDatabaseName);

     DataSet resultDataSet = CMS.DataEngine.ConnectionHelper.ExecuteQuery("Thinkgate_GetDocumentPlanAssociationDetails", queryDataParameters, QueryTypeEnum.StoredProcedure, false);
     DataTable resultDataTable = resultDataSet.Tables[0];


     CustomTableItemProvider customTableProvider = new CustomTableItemProvider(CMSContext.CurrentUser);
     Assoccount = resultDataTable.Rows.Count;
     rptStdAssocDetail.DataSource = resultDataTable;
     rptStdAssocDetail.DataBind();

 }

 private void DeleteStandardsAssocItem(int associationID)
 {
     if (associationID >= 0)
     {
         string query = string.Format("Delete from [dbo].[TG_DocumentPlanAssociation] WHERE DocumentID={0} AND DocumentTypeEnum={1} AND AssociationCategoryEnum={2} AND AssociationID={3} ", this.DocumentID.ToString(), (int)DocType.InstructionPlan, (int)AssociationCategory.Standard, associationID);
         GetDataTable(CMSConnectionString, query);
     }
 }
 private int GetCurrentAssociationCount()
 {
     string query = string.Format("SELECT * FROM  [dbo].[TG_DocumentPlanAssociation] where DocumentID = {0} and DocumentTypeEnum = {1} and AssociationCategoryEnum = {2}", this.DocumentID.ToString(), (int)DocType.InstructionPlan, (int)AssociationCategory.Standard);
     DataTable resultDataTable = GetDataTable(CMSConnectionString, query);
     return resultDataTable.Rows.Count;

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