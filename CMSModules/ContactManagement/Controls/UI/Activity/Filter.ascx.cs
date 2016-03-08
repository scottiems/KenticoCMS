using System;
using System.Data;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.OnlineMarketing;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.Controls;
using CMS.UIControls;
using CMS.WebAnalytics;

public partial class CMSModules_ContactManagement_Controls_UI_Activity_Filter : CMSAbstractBaseFilterControl
{
    #region "Variables"

    private bool mShowContactSelector = true;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets the where condition created using filtered parameters.
    /// </summary>
    public override string WhereCondition
    {
        get
        {
            return GenerateWhereCondition();
        }
        set
        {
            base.WhereCondition = value;
        }
    }


    /// <summary>
    /// Gets or sets site ID. This value is used when site selector is hidden.
    /// </summary>
    public int SiteID
    {
        get;
        set;
    }


    /// <summary>
    /// Determines whether contact selector is visible.
    /// </summary>
    public bool ShowContactSelector
    {
        get
        {
            return mShowContactSelector;
        }
        set
        {
            mShowContactSelector = value;
        }
    }


    /// <summary>
    /// Determines whether IP text box (filter) is visible.
    /// </summary>
    public bool ShowIPFilter
    {
        get;
        set;
    }


    /// <summary>
    /// Determines whether site selector is visible.
    /// </summary>
    public bool ShowSiteFilter
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates if control is used on live site.
    /// </summary>
    public override bool IsLiveSite
    {
        get
        {
            return base.IsLiveSite;
        }
        set
        {
            base.IsLiveSite = value;
        }
    }

    #endregion


    #region "Page methods"

    protected override void OnInit(EventArgs e)
    {
        string currSiteName = null;
        int currSiteId = 0;

        // Get current site ID/name
        if (ContactHelper.IsSiteManager)
        {
            currSiteId = SiteID;
            currSiteName = SiteInfoProvider.GetSiteName(currSiteId);
        }
        else
        {
            currSiteName = CMSContext.CurrentSiteName;
            currSiteId = CMSContext.CurrentSiteID;
        }

        SiteID = currSiteId;
        ShowSiteFilter = (currSiteId == UniSelector.US_ALL_RECORDS);
        ShowIPFilter = ActivitySettingsHelper.IPLoggingEnabled(currSiteName);

        base.OnInit(e);
        plcCon.Visible = ShowContactSelector;
        plcIp.Visible = ShowIPFilter;
        plcSite.Visible = ShowSiteFilter;
        if (ShowSiteFilter)
        {
            siteSelector.DropDownCssClass = "DropDownFieldFilter";
        }
        btnReset.Text = GetString("general.reset");
        btnReset.Click += btnReset_Click;
        btnFilter.Click += btnSearch_Click;
    }


    /// <summary>
    /// Resets the associated UniGrid control.
    /// </summary>
    protected void btnReset_Click(object sender, EventArgs e)
    {
        UniGrid grid = FilteredControl as UniGrid;
        if (grid != null)
        {
            grid.Reset();
        }
    }


    /// <summary>
    /// Applies filter on associated UniGrid control.
    /// </summary>
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        UniGrid grid = FilteredControl as UniGrid;
        if (grid != null)
        {
            grid.ApplyFilter(sender, e);
        }
    }

    #endregion


    #region "State management"

    /// <summary>
    /// Resets filter to the default state.
    /// </summary>
    public override void ResetFilter()
    {
        drpType.Value = null;
        fltTimeBetween.Clear();
        fltContact.ResetFilter();
        fltName.ResetFilter();
        fltIP.ResetFilter();
    }


    /// <summary>
    /// Stores filter state to the specified object.
    /// </summary>
    /// <param name="state">The object that holds the filter state.</param>
    public override void StoreFilterState(FilterState state)
    {
        state.AddValue("FromTime", fltTimeBetween.ValueFromTime);
        state.AddValue("ToTime", fltTimeBetween.ValueToTime);
        base.StoreFilterState(state);
    }


    /// <summary>
    /// Restores filter state from the specified object.
    /// </summary>
    /// <param name="state">The object that holds the filter state.</param>
    public override void RestoreFilterState(FilterState state)
    {
        base.RestoreFilterState(state);
        fltTimeBetween.ValueFromTime = state.GetDateTime("FromTime");
        fltTimeBetween.ValueToTime = state.GetDateTime("ToTime");
    }

    #endregion


    #region "Search methods - where condition"

    /// <summary>
    /// Generates complete filter where condition.
    /// </summary>    
    private string GenerateWhereCondition()
    {
        string whereCond = string.Empty;
        if (!String.IsNullOrEmpty(drpType.SelectedValue))
        {
            whereCond = "ActivityType=N'" + drpType.SelectedValue.Replace("'", "''") + "'";
        }

        int siteId = SiteID;
        if (ShowSiteFilter)
        {
            siteId = ValidationHelper.GetInteger(siteSelector.Value, 0);
        }

        // Create condition based on site selector
        if (siteId > 0)
        {
            whereCond = SqlHelperClass.AddWhereCondition(whereCond, "ActivitySiteID=" + siteId);
        }
        else if (siteId == UniSelector.US_GLOBAL_RECORD)
        {
            whereCond = SqlHelperClass.AddWhereCondition(whereCond, "ActivitySiteID IS NULL");
        }

        if (ShowContactSelector)
        {
            // Filter by contact if contact selector is visible
            whereCond = SqlHelperClass.AddWhereCondition(whereCond, fltContact.GetCondition());
        }

        if (ShowIPFilter)
        {
            // Filter by IP if filter is visible
            whereCond = SqlHelperClass.AddWhereCondition(whereCond, fltIP.GetCondition());
        }

        whereCond = SqlHelperClass.AddWhereCondition(whereCond, fltName.GetCondition());
        whereCond = SqlHelperClass.AddWhereCondition(whereCond, fltTimeBetween.GetCondition());

        return whereCond;
    }

    #endregion
}