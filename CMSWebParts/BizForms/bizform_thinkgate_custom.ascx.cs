using System;
using System.Web;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.PortalControls;
using CMS.WebAnalytics;
using System.Web.UI.WebControls;

public partial class CMSWebParts_BizForms_bizform_thinkgate_custom : CMSAbstractWebPart
{
    #region "Properties"

    /// <summary>
    /// Gets or sets the form name of BizForm.
    /// </summary>
    public string BizFormName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("BizFormName"), "");
        }
        set
        {
            SetValue("BizFormName", value);
        }
    }


    /// <summary>
    /// Gets or sets the alternative form full name (ClassName.AlternativeFormName).
    /// </summary>
    public string AlternativeFormName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("AlternativeFormName"), "");
        }
        set
        {
            SetValue("AlternativeFormName", value);
        }
    }


    /// <summary>
    /// Gets or sets the site name.
    /// </summary>
    public string SiteName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("SiteName"), "");
        }
        set
        {
            SetValue("SiteName", value);
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether the WebPart use colon behind label.
    /// </summary>
    public bool UseColonBehindLabel
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("UseColonBehindLabel"), true);
        }
        set
        {
            SetValue("UseColonBehindLabel", value);
        }
    }


    /// <summary>
    /// Gets or sets the message which is displayed after validation failed.
    /// </summary>
    public string ValidationErrorMessage
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ValidationErrorMessage"), "");
        }
        set
        {
            SetValue("ValidationErrorMessage", value);
        }
    }


    /// <summary>
    /// Gets or sets the conversion track name used after successful registration.
    /// </summary>
    public string TrackConversionName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("TrackConversionName"), "");
        }
        set
        {
            if (value.Length > 400)
            {
                value = value.Substring(0, 400);
            }
            SetValue("TrackConversionName", value);
        }
    }


    /// <summary>
    /// Gets or sets the conversion value used after successful registration.
    /// </summary>
    public double ConversionValue
    {
        get
        {
            return ValidationHelper.GetDoubleSystem(GetValue("ConversionValue"), 0);
        }
        set
        {
            SetValue("ConversionValue", value);
        }
    }

    #endregion


    #region "Methods"

    protected override void OnLoad(EventArgs e)
    {
        viewBiz.OnAfterSave += viewBiz_OnAfterSave;
        base.OnLoad(e);
    }


    protected override void OnPreRender(EventArgs e)
    {
        //Override PreRender to Accept QueryString Data/Values from E3 to Kentico Forms
        if (this.viewBiz.BasicForm != null)
        {
            foreach (String key in Request.QueryString.AllKeys)
            {
                //See if form field matches QueryString Key
                if (viewBiz.BasicForm.FieldEditingControls[key] != null)
                {
                    if (!String.IsNullOrEmpty(Request.QueryString[key]))
                    {
                        viewBiz.BasicForm.FieldEditingControls[key].Value = Request.QueryString[key];
                    }
                }

            }

            if (Request.QueryString["viewmode"] == "3")
            {
                viewBiz_DisableSubmit();

                foreach (System.Collections.DictionaryEntry dEntry in viewBiz.BasicForm.FieldEditingControls)
                {
                    viewBiz.BasicForm.FieldEditingControls[dEntry.Key.ToString()].Enabled = false;
                }
            }
        }
    }


    /// <summary>
    /// Content loaded event handler.
    /// </summary>
    public override void OnContentLoaded()
    {
        base.OnContentLoaded();
        SetupControl();
    }


    /// <summary>
    /// Reloads data for partial caching.
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();
        SetupControl();
    }


    /// <summary>
    /// Initializes the control properties.
    /// </summary>
    protected void SetupControl()
    {
        if (StopProcessing)
        {
            // Do nothing
            viewBiz.StopProcessing = true;
        }
        else
        {
            // Set BizForm properties
            viewBiz.FormName = BizFormName;
            viewBiz.SiteName = SiteName;
            viewBiz.UseColonBehindLabel = UseColonBehindLabel;
            viewBiz.AlternativeFormFullName = AlternativeFormName;
            viewBiz.ValidationErrorMessage = ValidationErrorMessage;

            //Store the form name for E3 service call
            bizFormName.Value = BizFormName;

            // Set the live site context
            if (viewBiz.BasicForm != null)
            {
                viewBiz.BasicForm.ControlContext.ContextName = CMS.SiteProvider.ControlContext.LIVE_SITE;
            }
        }
    }


    private void viewBiz_OnAfterSave(object sender, EventArgs e)
    {
        if (TrackConversionName != String.Empty)
        {
            string siteName = CMSContext.CurrentSiteName;

            if (AnalyticsHelper.AnalyticsEnabled(siteName) && AnalyticsHelper.TrackConversionsEnabled(siteName) && !AnalyticsHelper.IsIPExcluded(siteName, HTTPHelper.UserHostAddress))
            {
                HitLogProvider.LogConversions(CMSContext.CurrentSiteName, CMSContext.PreferredCultureCode, TrackConversionName, 0, ConversionValue);
            }
        }
    }

    protected void viewBiz_DisableSubmit()
    {
        // Disable submit button if used
        if (viewBiz.BasicForm.SubmitButton != null)
        {
            viewBiz.BasicForm.SubmitButton.Visible = false;
        }

        // Disable image button if used
        if (viewBiz.BasicForm.SubmitImageButton != null)
        {
            viewBiz.BasicForm.SubmitImageButton.Visible = false;
        }

        // Stop bizform processing
        this.viewBiz.StopProcessing = true;

    }

    #endregion
}