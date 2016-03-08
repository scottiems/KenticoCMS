using CMS.CMSHelper;
using CMS.DocumentEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class CMSModules_Thinkgate_Controls_UpDownButton : System.Web.UI.UserControl
{
    // input parameters    
    //public int NodeID { get { Object obj = ViewState["NodeID"]; if (obj == null) { return 0; } else { return (int)obj; } } set { ViewState["NodeID"] = value; } }
    public int NodeID { get; set; }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            //NodeID = 1060;
        }
    }

    /// <summary>
    /// Move position up in list.
    /// </summary>
    protected void imgbtnUp_Click(object sender, ImageClickEventArgs e)
    {                        
        MoveDocumentUp();
        Page.Response.Redirect(HttpContext.Current.Request.Url.ToString(), true);
        
    }

    /// <summary>
    /// Move position down in list.
    /// </summary>
    protected void imgbtnDown_Click(object sender, ImageClickEventArgs e)
    {                        
        MoveDocumentDown();
        Page.Response.Redirect(HttpContext.Current.Request.Url.ToString(), true);
    }

    /// <summary>
    /// Moves a document up in the tree.
    /// </summary>
    private bool MoveDocumentUp()
    {
        bool result = false;

        // Create an instance of the Tree Provider
        TreeProvider tree = new TreeProvider(CMSContext.CurrentUser);

        // Select a node
        CMS.DocumentEngine.TreeNode node = tree.SelectSingleNode(this.NodeID);

        // if node is found, move it up
        if (node != null)
        {
            // move the node up
            tree.MoveNodeUp(node.NodeID);
            result = true;
        }

        return result;
    }

    /// <summary>
    /// Moves a document down in the tree.
    /// </summary>
    private bool MoveDocumentDown()
    {
        bool result = false;

        // Create an instance of the Tree provider
        TreeProvider tree = new TreeProvider(CMSContext.CurrentUser);

        // Select a node
        CMS.DocumentEngine.TreeNode node = tree.SelectSingleNode(this.NodeID);

        // if node is found, move it down
        if (node != null)
        {
            // Move the node up
            tree.MoveNodeDown(node.NodeID);
            result = true;
        }

        return result;
    }

}