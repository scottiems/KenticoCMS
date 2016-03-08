using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.PortalControls;
using CMS.GlobalHelper;
using CMS.CMSHelper;

public partial class CMSWebParts_ThinkgateWebParts_AssociationFullControl : CMSAbstractWebPart
{
    #region "Properties"

    

    #endregion


    #region "Methods"

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        ScriptHelper.RegisterWOpenerScript(Page);
        ScriptHelper.RegisterClientScriptBlock(Page, Page.GetType(), "CloseDialog", ScriptHelper.CloseDialogScript);
        
        AssociationToolbar.DocumentID = (TreeHelper.SelectSingleNode(CMS.CMSHelper.CMSContext.CurrentDocument.NodeID).DocumentID).ToString();
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



