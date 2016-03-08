using System;
using System.Web;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.PortalControls;
using CMS.DocumentEngine;

public partial class CMSWebParts_CommunityServices_Facebook_FacebookComments : CMSAbstractWebPart
{
    #region "Properties"

    /// <summary>
    /// Url to comment on.
    /// </summary>
    public string Url
    {
        get
        {
            return ValidationHelper.GetString(GetValue("URL"), string.Empty);
        }
        set
        {
            SetValue("URL", value);
        }
    }


    /// <summary>
    /// Number of posts.
    /// </summary>
    public int Posts
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("Posts"), 0);
        }
        set
        {
            SetValue("Posts", value);
        }
    }


    /// <summary>
    /// Width of the web part in pixels.
    /// </summary>
    public int Width
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("Width"), 500);
        }
        set
        {
            SetValue("Width", value);
        }
    }


    /// <summary>
    /// Color scheme of the web part.
    /// </summary>
    public string ColorScheme
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ColorScheme"), string.Empty);
        }
        set
        {
            SetValue("ColorScheme", value);
        }
    }


    /// <summary>
    /// Indicates if HTML 5 output should be generated.
    /// </summary>
    public bool UseHTML5
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("UseHTML5"), false);
        }
        set
        {
            SetValue("UseHTML5", value);
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
        if (StopProcessing)
        {
            // Do not process
        }
        else
        {
            // If paramater URL is empty, set URL of current document
            string url = Url;
            if (string.IsNullOrEmpty(url) && (CMSContext.CurrentDocument != null))
            {
                TreeNode node = CMSContext.CurrentDocument;
                url = CMSContext.GetUrl(node.NodeAliasPath, node.DocumentUrlPath, CMSContext.CurrentSiteName);
            }
            else
            {
                url = ResolveUrl(url);
            }

            // Register javascript SDK
            ScriptHelper.RegisterFacebookJavascriptSDK(Page, CMSContext.PreferredCultureCode);

            if (UseHTML5)
            {
                ltlComments.Text = "<div class=\"fb-comments\" data-href=\"" + URLHelper.GetAbsoluteUrl(url) + "\" data-num-posts=\"" + Posts + "\" data-width=\"" + Width + "\"" + (!string.IsNullOrEmpty(ColorScheme) ? " data-colorscheme=\"" + ColorScheme + "\"" : "") + "></div>";
            }
            else
            {
                ltlComments.Text = "<fb:comments href=\"" + URLHelper.GetAbsoluteUrl(url) + "\" num_posts=\"" + Posts + "\" width=\"" + Width + "\"" + (!string.IsNullOrEmpty(ColorScheme) ? " colorscheme=\"" + ColorScheme + "\"" : "") + "></fb:comments>";
            }
        }
    }


    /// <summary>
    /// Reloads the control data.
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();

        SetupControl();
    }

    #endregion
}