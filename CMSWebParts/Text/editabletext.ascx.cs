using System;
using System.ComponentModel;
using System.Data;
using System.Collections;
using System.Globalization;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;

using CMS.CMSHelper;
using CMS.Controls;
using CMS.ExtendedControls;
using CMS.FormEngine;
using CMS.GlobalHelper;
using CMS.PortalControls;
using CMS.PortalEngine;
using CMS.SettingsProvider;

public partial class CMSWebParts_Text_editabletext : CMSAbstractEditableWebPart
{
    #region "Public properties"

    /// <summary>
    /// Gets the url of the page which ensures editing of the web part's editable content in the On-Site editing mode.
    /// </summary>
    public override string EditPageUrl
    {
        get
        {
            return ucEditableText.EditPageUrl;
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Content loaded event handler.
    /// </summary>
    public override void OnContentLoaded()
    {
        base.OnContentLoaded();
        SetupControl();
    }


    /// <summary>
    /// Initializes the control properties.
    /// </summary>
    protected void SetupControl()
    {
        if (!StopProcessing)
        {
            ucEditableText.ContentID = this.WebPartID;
            ucEditableText.DataControl = this as ISimpleDataContainer;
            //ucEditableText.DesignPanel = pnlDesign;
            ucEditableText.PageManager = PageManager;

            ucEditableText.SetupControl();
        }
    }


    /// <summary>
    /// Overridden CreateChildControls method.
    /// </summary>
    protected override void CreateChildControls()
    {
        SetupControl();
        base.CreateChildControls();
    }


    /// <summary>
    /// Loads the control content.
    /// </summary>
    /// <param name="content">Content to load</param>
    /// <param name="forceReload">If true, the content is forced to reload</param>
    public override void LoadContent(string content, bool forceReload)
    {
        if (!StopProcessing)
        {
            ucEditableText.LoadContent(content, forceReload);

            if (!string.IsNullOrEmpty(ucEditableText.DefaultText))
            {
                // Default image defined => content is not empty
                EmptyContent = false;
            }
        }
    }


    /// <summary>
    /// Returns true if the control uses HTML editor.
    /// </summary>
    /// <param name="toolbarLocation">Toolbar location to check</param>
    public override bool UsesHtmlEditor(string toolbarLocation)
    {
        return ucEditableText.UsesHtmlEditor(toolbarLocation);
    }


    /// <summary>
    /// Returns true if entered data is valid. If data is invalid, it returns false and displays an error message.
    /// </summary>
    public override bool IsValid()
    {
        return ucEditableText.IsValid();
    }


    /// <summary>
    /// Gets the current control content.
    /// </summary>
    public override string GetContent()
    {
        if (!StopProcessing)
        {
            return ucEditableText.GetContent();
        }

        return null;
    }


    /// <summary>
    /// Returns the array list of the field IDs (Client IDs of the inner controls) that should be spell checked.
    /// </summary>
    public override List<string> GetSpellCheckFields()
    {
        return ucEditableText.GetSpellCheckFields();
    }


    /// <summary>
    /// Try initialize control
    /// </summary>
    protected override void OnInit(EventArgs e)
    {
        SetupControl();
        base.OnInit(e);
    }

    #endregion
}