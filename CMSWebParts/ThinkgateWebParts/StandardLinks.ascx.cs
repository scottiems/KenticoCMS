
using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.PortalControls;
using CMS.GlobalHelper;
using CMS.CMSHelper;

public partial class CMSWebParts_ThinkgateWebParts_StandardLinks : CMSAbstractWebPart
{
    #region "Properties"

    public string Description { get; set; }

    #endregion


    #region "Methods"

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        try
        {
            if (CMSContext.CurrentDocument == null)
            {
                return;
            }
            CMS.DocumentEngine.TreeNode node = TreeHelper.SelectSingleNode(CMSContext.CurrentDocument.NodeID);
            if (node != null)
            {
                this.Description = node.GetValue("Description").ToString();
            }
        }
        catch { }
    }

    /// <summary>
    /// Content loaded event handler
    /// </summary>
    public override void OnContentLoaded()
    {
        base.OnContentLoaded();
        SetupControl();
    }


    /// <summary>
    /// Initializes the control properties
    /// </summary>
    protected void SetupControl()
    {
        if (this.StopProcessing)
        {
            // Do not process
        }
        else
        {

        }
    }


    /// <summary>
    /// Reloads the control data
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();

        SetupControl();
    }

    #endregion
}



