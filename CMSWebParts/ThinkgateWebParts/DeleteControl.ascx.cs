using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.SiteProvider;
using CMS.TreeEngine;
using CMS.UIControls;
using CMS.DocumentEngine;


public partial class CMSWebParts_ThinkgateWebParts_DeleteControl : System.Web.UI.UserControl
{
	private int m_nodeId;
	private int _rootLevel = 0;
    private string _classname;
    public string ClassName
    {
      get { return _classname ;}
      set { _classname= value; }

    }

	public CMSWebParts_ThinkgateWebParts_DeleteControl()
	{
	   
	}

	protected void Page_Load(object sender, EventArgs e)
	{
		string target = NodeID.ToString();
        TreeProvider tree = new TreeProvider(CMSContext.CurrentUser);
        CMS.DocumentEngine.TreeNode nodeToRemove = tree.SelectSingleNode(this.NodeID);
        ClassName = nodeToRemove.ClassName.ToLower();
       
		if (Request.Form["__EVENTTARGET"] == "lnkDelete")
		{
			if (Request.Form["__EVENTARGUMENT"] != string.Empty)
			{
				string argument = Request.Form["__EVENTARGUMENT"].ToString();
				ViewState["argument"] = argument;

				string[] formatted = ViewState["argument"].ToString().Split(',');
			   
			   if(formatted[2] == target)
					// do postback to perform action
					lnkDelete_Click(this, new EventArgs());
			}
		}
	  
	}

	public int NodeID
	{
		get { return m_nodeId; }

		set { m_nodeId = value; }
	}
			
	protected void lnkDelete_Click(object sender, EventArgs args)
	{
		
		if (ViewState["argument"] != null)
		{
			string[] argument = ViewState["argument"].ToString().Split(',');
			if (argument[0] == "true")
			{
				if (argument[1] == "true")
				{
					TreeProvider tree = new TreeProvider(CMSContext.CurrentUser);
					CMS.DocumentEngine.TreeNode nodeToRemove = tree.SelectSingleNode(this.NodeID);
					DestroyDocument(nodeToRemove, true);
				}
				else
				{
					TreeProvider tree = new TreeProvider(CMSContext.CurrentUser);
					CMS.DocumentEngine.TreeNode nodeToRemove = tree.SelectSingleNode(this.NodeID);
					DestroyDocument(nodeToRemove, false);
				}
			}
			else
			{
				try
				{
					TreeProvider tree = new TreeProvider(CMSContext.CurrentUser);
					CMS.DocumentEngine.TreeNode nodeToRemove = tree.SelectSingleNode(this.NodeID);
					RemoveFromParentDocumentAndKeepLocalCopy(nodeToRemove);
				}
				catch (Exception ex)
				{
					
					throw new Exception(String.Format("lnkDelete_Click exception: {0}", ex.Message));
				}
			}

			ViewState["argument"] = null;
			Response.Redirect(Request.RawUrl);
		}
	}

	private string getState(CMS.SiteProvider.UserInfo user)
	{
		string envState = string.Empty;
		string[] clientID = user.UserName.Split('-');
		if (!string.IsNullOrEmpty(clientID[0]))
		{
			envState = ThinkgateKenticoHelper.getState(clientID[0]);
		}
		return envState;
	}

	private string getClientIDFromKenticoUserName(string kenticoUserName)
	{
		string[] clientID = kenticoUserName.Split('-');

		if (!string.IsNullOrEmpty(clientID[0]))
		{
			return clientID[0];
		}
		return string.Empty;
	}
  
   internal CMS.DocumentEngine.TreeNode GetCurrentUserNode(string pathLocation)
	{
		CMS.SiteProvider.UserInfo user = CMS.SiteProvider.UserInfoProvider.GetUserInfo(CMSContext.CurrentUser.UserName);
		string envState = getState(user);

		CurrentUserInfo adminUser = new CurrentUserInfo(UserInfoProvider.GetUserInfo("Administrator"), true);
		TreeProvider tree = new TreeProvider(adminUser);

		string aliasPath = string.Empty;

		switch (pathLocation)
		{
			case "Users":
				aliasPath = "/" + envState + "/" + pathLocation + "/" + user.UserName.Replace(".", "-");
				break;
			case "Documents":
				aliasPath = "/" + envState + "/" + pathLocation;
				break;
			case "Districts":
				string clientId = getClientIDFromKenticoUserName(CMSContext.CurrentUser.UserName);
				aliasPath = "/" + envState + "/" + pathLocation + "/" + clientId;
				break;
			default:
				break;
		}

	   return tree.SelectSingleNode(CMSContext.CurrentSiteName, aliasPath, "en-us");
	}

	private void RemoveFromParentDocumentAndKeepLocalCopy(CMS.DocumentEngine.TreeNode nodeToRemove)
	{
		TreeProvider treeProvider = new TreeProvider(CMSContext.CurrentUser);
		CMS.DocumentEngine.TreeNode rootNode = null;
		
		try
		{
			if (nodeToRemove.NodeAliasPath.Contains("/Users/"))
			{
				rootNode = GetCurrentUserNode("Users");
			}
			else if (nodeToRemove.NodeAliasPath.Contains("/Documents/"))
			{
				rootNode = GetCurrentUserNode("Documents");
			}
			else if (nodeToRemove.NodeAliasPath.Contains("/Districts/"))
			{
				rootNode = GetCurrentUserNode("Districts");
			}
            else if (nodeToRemove.NodeAliasPath.Contains("/Shared/"))
            {
                rootNode = GetCurrentUserNode("Shared");
            }
			treeProvider.MoveNode(nodeToRemove, rootNode.NodeID, true);
		}
		catch (Exception ex)
		{
			
			throw new Exception(String.Format("RemoveFromParentDocumentAndKeepLocalCopy exception: {0}", ex.Message));
		}
	}

	private void DestroyDocument(CMS.DocumentEngine.TreeNode nodeToDestroy, bool isCascadeDelete)
	{
		TreeProvider treeProvider = new TreeProvider(CMSContext.CurrentUser);

		if (isCascadeDelete)
		{
			// DESTROYS THIS AND ALL CHILD DOCUMENTS
			DocumentHelper.DeleteDocument(nodeToDestroy, treeProvider, true, false, false);
		}
		else
		{
			// Move the children to the user root and THEN Destroy the document
			// this iterates over the child nodes ONE level down from current Node
			foreach (CMS.DocumentEngine.TreeNode childNode in nodeToDestroy.Children)
			{
				RemoveFromParentDocumentAndKeepLocalCopy(childNode);
			}
			DocumentHelper.DeleteDocument(nodeToDestroy, treeProvider, true, false, false);
		}
	}

}
