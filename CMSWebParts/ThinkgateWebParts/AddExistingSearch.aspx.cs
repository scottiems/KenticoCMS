using Business;
using CMS.CMSHelper;
using CMS.DocumentEngine;
using CMS.GlobalHelper;
using CMS.GlobalHelper.UniGraphConfig;
using CMS.SiteProvider;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using DataAccess;


public partial class CMSWebParts_ThinkgateWebParts_AddExistingSearch : System.Web.UI.Page
	{

		TreeProvider treeProvider = null;
	    private bool filterExpiredMaterials = true;

		/// <summary>
		/// Gets the type of the document.
		/// </summary>
		/// <value>
		/// The type of the document.
		/// </value>
		protected string DocumentType
		{
			get
			{
				return QueryHelper.GetString("doctype", "");
			}
		}

		/// <summary>
		/// Identifier of parent document. (For newly created documents.)
		/// </summary>
		protected string ParentNodeID
		{
			get
			{
				return QueryHelper.GetString("parentnodeid", "0");
			}
		}


		/// <summary>
		/// Gets the document action.
		/// </summary>
		/// <value>
		/// The document action. Default= "3"
		/// 
		/// Copy/Move:
		/// Value    Child Nodes      Associations
		/// =====    ===========      ============
		///  "0"          N                 N  
		///  "1"          N                 Y  
		///  "2"          Y                 N  
		///  "3"          Y                 Y
		/// 
		/// </value>
		/// 
		protected string DocumentAction2
		{
			get
			{
				return QueryHelper.GetString("action", "3");
			}
		}


		protected void Page_Load(object sender, EventArgs e)
		{
			CMSContext.Init();
			DropDownList1_SelectedIndexChanged(sender, e);
		} 


		protected void CopySelectedItems_Click(object sender, EventArgs e)
		{
			TreeProvider tp = new TreeProvider(UserInfoProvider.GetFullUserInfo(CMSContext.CurrentUser.UserName));

			string[] selitems =  (SelectedItems.Value).Split(',');
			int parentNode;
			if (Int32.TryParse(ParentNodeID, out parentNode))
			{
                int expiredChildCount = 0;
                int expiredResourceAssociationCount = 0;
                CMS.DocumentEngine.TreeNode toNode = tp.SelectSingleNode(parentNode);
			    foreach (string t in selitems)
			    {
			        int nodeid;
			       
			        string docEntry = t;
			        string[] docEntryData = (docEntry).Split('|');
			        if (Int32.TryParse(docEntryData[0], out nodeid))
			        {
			            CMS.DocumentEngine.TreeNode fromNode = tp.SelectSingleNode(nodeid);
                        if (IsIncludeChildNodeRequested(DocumentAction.Value))
                        expiredChildCount += GetExpiredChildCountforNode(tp, fromNode, toNode, DocumentAction.Value);
                        if (IsIncludeAssociationRequested(DocumentAction.Value))
                            expiredResourceAssociationCount += GetExpiredAssociationCount(tp, fromNode);
                       
			        }
			    }
                CopySelectedItemsConfirmed();
			    if (expiredChildCount != 0)
			    {
			        AddExistingMessage.Text = AddExistingMessage.Text + "\n" + expiredChildCount +
			                                  " Usage right expired child nodes were not copied.";
			    }
                if (expiredResourceAssociationCount != 0)
                {
                    AddExistingMessage.Text = AddExistingMessage.Text + "\n" + expiredResourceAssociationCount +
                                              " Usage right expired resource associations were not copied.";
                }
			}

		   
		}

	    
	    internal static Boolean IsIncludeChildNodeRequested(string action)
	    {
	        if (action == null) action = "0";
	        return (action == "1" || action == "3");
	    }

        internal static Boolean IsIncludeAssociationRequested(string action)
        {
            if (action == null) action = "0";
            return (action == "2" || action == "3");
        }

	    internal static int GetExpiredChildCountforNode(TreeProvider tp, CMS.DocumentEngine.TreeNode nodeToClone, CMS.DocumentEngine.TreeNode destinationNode, string action)
        {
                   int i = 0;
                   TreeNodeDataSet nodeToClone_DS = tp.SelectNodes(CMSContext.CurrentSiteName, nodeToClone.NodeAliasPath + "/%", "en-us", false);
                    if (nodeToClone_DS != null && nodeToClone_DS.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow row in nodeToClone_DS.Tables[0].Rows)
                        {
                            CMS.DocumentEngine.TreeNode childDoc = DocumentHelper.GetDocument((int) row["NodeID"], "en-us", tp);
                            if(childDoc["ExpirationDate"] != null)
                            {
                                DateTime expirationDate = (DateTime)(childDoc["ExpirationDate"]);
                                if (expirationDate.Date < DateTime.Today)
                                    i++;
                            }

                        }
                    }
	        return i;
        }

        internal int GetExpiredAssociationCount(TreeProvider tp, CMS.DocumentEngine.TreeNode nodeToClone)
        {
            int count = 0;
            count = GetExpiredAssociationCountforNode(nodeToClone.NodeID);
            if (IsIncludeChildNodeRequested(DocumentAction.Value))
            {
                    TreeNodeDataSet nodeToClone_DS = tp.SelectNodes(CMSContext.CurrentSiteName, nodeToClone.NodeAliasPath + "/%", "en-us", false);
                    if (nodeToClone_DS != null && nodeToClone_DS.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow row in nodeToClone_DS.Tables[0].Rows)
                        {
                            if(!string.IsNullOrEmpty(row["NodeID"].ToString()))
                            {
                                count += GetExpiredAssociationCountforNode((int)row["NodeId"]);
                            }

                        }
                    }
            }
            return count;
        }

        internal static int GetExpiredAssociationCountforNode(int nodeID)
        {
            int rowCount = 0;
            using (SqlConnection sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["CMSConnectionString"].ToString()))
            {
                sqlConnection.Open();
                using (SqlCommand sqlCommand = new SqlCommand("Thinkgate_GetExpiredAssociationCountforNode", sqlConnection))
                {
                    sqlCommand.Parameters.AddWithValue("@NodeId", nodeID);
                    sqlCommand.CommandType = CommandType.StoredProcedure;

                    rowCount = sqlCommand.ExecuteNonQuery();
                }
            }
            return rowCount < 0 ? 0 : rowCount;
        }

        protected void CopySelectedItemsConfirmed()
        {
            TreeProvider tp = new TreeProvider(UserInfoProvider.GetFullUserInfo(CMSContext.CurrentUser.UserName));

            string[] selitems = (SelectedItems.Value).Split(',');
            int parentNode;
            if (Int32.TryParse(ParentNodeID, out parentNode))
            {
                CMS.DocumentEngine.TreeNode toNode = tp.SelectSingleNode(parentNode);
                for (var i = 0; i < selitems.Length; i++)
                {
                    int nodeid;
                    string docEntry = selitems[i];
                    string[] docEntryData = (docEntry).Split('|');
                    if (Int32.TryParse(docEntryData[0], out nodeid))
                    {
                        CMS.DocumentEngine.TreeNode fromNode = tp.SelectSingleNode(nodeid);
                        cloneNode(tp, fromNode, toNode, DocumentAction.Value);
                    }

                }
            }
            AddExistingMessage.Text = selitems.Length + " document(s) successfully copied.";
        }


		protected void DropDownList1_SelectedIndexChanged(object sender, EventArgs e)
		{
			string ddtype = DropDownList1.SelectedValue;
			UserInfo userInfo = UserInfoProvider.GetUserInfo(CMSContext.CurrentUser.UserName);
			treeProvider = new TreeProvider(userInfo);
			GetDocuments(userInfo, ddtype, DocumentType, filterExpiredMaterials);
		}
		
		internal static Boolean cloneNode(TreeProvider tp, CMS.DocumentEngine.TreeNode nodeToClone, CMS.DocumentEngine.TreeNode destinationNode, string action)
		{
			if (action == null) action = "0";
			bool includeChildNodes = (action == "1" || action == "3");
			bool includeAssociations = (action == "2" || action == "3");
			if ((tp != null) && (nodeToClone != null) && (destinationNode != null))
			{
                CMS.DocumentEngine.TreeNode newTreeNode = DocumentHelper.CopyDocument(nodeToClone, destinationNode.NodeID, false, tp);

				if (newTreeNode != null)
				{
					// Remove ReviewCount and RatingAverage
                    RatingDataAccess _ratingDataAccess = new RatingDataAccess();
                    _ratingDataAccess.UpdateRatingAvgNCountsInKenticoDB(newTreeNode.NodeID);

                    //Copy the parent node associations
                    if(includeAssociations)
					CopyDocumentPlanAssociationDetails(nodeToClone.NodeID, newTreeNode.NodeID);

				    if (includeChildNodes)
				    {
				        DataSet nodeToClone_DS = tp.SelectNodes(CMSContext.CurrentSiteName, nodeToClone.NodeAliasPath + "/%", "en-us", false);
				        if (nodeToClone_DS != null && nodeToClone_DS.Tables[0].Rows.Count > 0)
				        {
				            foreach (DataRow row in nodeToClone_DS.Tables[0].Rows)
				            {
				                int src = (int) row["NodeId"];
				                CMS.DocumentEngine.TreeNode childNode = DocumentHelper.GetDocument(src, "en-us", tp);
				                if (childNode["ExpirationDate"] != null)
				                {
                                    DateTime expirationDate = Convert.ToDateTime(childNode["ExpirationDate"]);
				                    if (expirationDate.Date < DateTime.Today) continue;
				                }
				              
                                cloneNode(tp, childNode, newTreeNode, action);

				            }
				        }
				    }
				}		

				return true;
			}
			return false;
		}


		/// <summary>
		/// Copy Document Plan Association Details
		/// </summary>
		/// <param name="sourceDocumentID"></param>
		/// <param name="destinationDocumnetID"></param>
		/// <returns></returns>
		public static int CopyDocumentPlanAssociationDetails(int sourceDocumentId, int destinationDocumentId)
		{
			int rowCount = 0;
			using (SqlConnection sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["CMSConnectionString"].ToString()))
			{
				sqlConnection.Open();
				using (SqlCommand sqlCommand = new SqlCommand("Thinkgate_DocumentPlanAssociationDetails_Copy", sqlConnection))
				{
					sqlCommand.Parameters.AddWithValue("@SourceDocumentId", sourceDocumentId);
					sqlCommand.Parameters.AddWithValue("@DestinationDocumentId", destinationDocumentId);
					sqlCommand.CommandType = CommandType.StoredProcedure;

					rowCount = sqlCommand.ExecuteNonQuery();
				}
			}
			return rowCount;
		}

		/// <summary>
		/// Return Document Type References from Kentico DB for the tileParm _resourceToShow.
		/// </summary>
		private void GetDocuments(UserInfo ui, string filterType, string docType, bool filterExpiredContent)
		{

            DataSet userNodeList = new DataSet();

		    DataSet resourceToShow = ThinkgateKenticoHelper.GetTileMapLookupDataSet(docType);

            foreach (DataRow dr in resourceToShow.Tables[0].Rows)
            {
                DataSet tempUserNodeList = ThinkgateKenticoHelper.SearchDocumentTypeReferences(ui, dr["KenticoDocumentTypeToShow"].ToString(), treeProvider, filterType, filterExpiredContent);
                DataColumn newColumn = new DataColumn("docDocumentType", typeof(System.String));
                newColumn.DefaultValue = dr["FriendlyName"].ToString();
                if (tempUserNodeList != null)
                {
                tempUserNodeList.Tables[0].Columns.Add(newColumn);
                if (userNodeList.Tables.Count > 0)
                userNodeList.Tables[0].Merge(tempUserNodeList.Tables[0]);
                else
                  userNodeList.Merge(tempUserNodeList);
                
                }

            }

            
     
		    if (userNodeList.Tables.Count > 0 && userNodeList.Tables[0].Rows.Count > 0)
			{
				rptNames.DataSource = userNodeList.Tables[0];
				rptNames.DataBind();
			}
			else
			{
				rptNames.DataSource = returnEmptyDataSet(docType); //build empty dataset, probably a better way to do this...
				rptNames.DataBind();
			}
		}


		private static DataSet returnEmptyDataSet(string docType)
		{
			DataTable table1 = new DataTable();
			DataSet set = new DataSet(docType);

			string colName = string.Empty;
			switch (docType)
			{
				case "thinkgate.LessonPlan":
					colName = "LessonPlanOverview";
					break;
				case "thinkgate.UnitPlan":
					colName = "UnitPlanOverview";
					break;
				case "thinkgate.InstructionalPlan":
					colName = "InstructionalPlanOverview";
					break;
				default:
					break;
			}

			table1.Columns.Add("NodeID");
			table1.Columns.Add(colName);
			table1.Columns.Add("DocumentName");
			table1.Columns.Add("NodeParentID");

			set.Tables.Add(table1);

			return set;
		}



}
