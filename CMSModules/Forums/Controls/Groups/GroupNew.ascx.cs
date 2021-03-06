using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.CMSHelper;
using CMS.CMSOutputFilter;
using CMS.Forums;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.UIControls;
using CMS.ExtendedControls;

public partial class CMSModules_Forums_Controls_Groups_GroupNew : CMSAdminEditControl
{
    #region "Variables"

    private int mGroupId = 0;
    private int mCommunityGroupId = 0;
    private Guid mCommunityGroupGUID = Guid.Empty;

    private string baseUrl = string.Empty;
    private string unsubscriptionUrl = string.Empty;

    #endregion


    #region "Public properties"

    /// <summary>
    /// Messages placeholder
    /// </summary>
    public override MessagesPlaceHolder MessagesPlaceHolder
    {
        get
        {
            return plcMess;
        }
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
            plcMess.IsLiveSite = value;
            base.IsLiveSite = value;
        }
    }


    /// <summary>
    /// Gets the ID of the group which has been created using the control.
    /// </summary>
    public int GroupID
    {
        get
        {
            return mGroupId;
        }
    }


    /// <summary>
    /// GUID of the community group current forum group is related to.
    /// </summary>
    public Guid CommunityGroupGUID
    {
        get
        {
            return mCommunityGroupGUID;
        }
        set
        {
            mCommunityGroupGUID = value;
        }
    }


    /// <summary>
    /// ID of the community group current forum group is related to.
    /// </summary>
    public int CommunityGroupID
    {
        get
        {
            return mCommunityGroupId;
        }
        set
        {
            mCommunityGroupId = value;
        }
    }

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        if (StopProcessing || !Visible)
        {
            EnableViewState = false;
        }

        // Do not edit code name in simple mode
        if (DisplayMode == ControlDisplayModeEnum.Simple)
        {
            plcCodeName.Visible = false;
        }

        // Control initializations				
        rfvGroupDisplayName.ErrorMessage = GetString("general.requiresdisplayname");
        rfvGroupName.ErrorMessage = GetString("general.requirescodename");

        lblGroupDisplayName.Text = GetString("Group_General.GroupDisplayNameLabel");
        lblGroupName.Text = GetString("Group_General.GroupNameLabel");

        lblForumBaseUrl.Text = GetString("Group_General.ForumBaseUrlLabel");
        lblUnsubscriptionUrl.Text = GetString("Group_General.UnsubscriptionUrlLabel");

        if (!IsLiveSite && !RequestHelper.IsPostBack())
        {
            if (CommunityGroupID > 0)
            {
                chkInheritBaseUrl.Checked = false;
                chkInheritUnsubUrl.Checked = false;
            }

            ReloadData();
        }

        // Show/hide URL textboxes
        plcBaseAndUnsubUrl.Visible = (DisplayMode != ControlDisplayModeEnum.Simple);

        if (plcBaseAndUnsubUrl.Visible)
        {
            SetUrls();
        }
    }


    /// <summary>
    /// Sets base and unsubscription URLs.
    /// </summary>
    private void SetUrls()
    {
        // Get base and unsubscription url from settings
        baseUrl = ValidationHelper.GetString(SettingsKeyProvider.GetStringValue(CMSContext.CurrentSiteName + ".CMSForumBaseUrl"), "");
        unsubscriptionUrl = ValidationHelper.GetString(SettingsKeyProvider.GetStringValue(CMSContext.CurrentSiteName + ".CMSForumUnsubscriptionUrl"), "");

        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "CheckBoxes", ScriptHelper.GetScript(@"
            function check(txtId,chk,inhV)  
            {
                txt = document.getElementById(txtId);
                if ((txt != null)&&(chk != null))
                {
                    if (chk.checked)
                    {
                        txt.disabled = 'disabled';
                        txt.value = inhV;
                    }
                    else
                    {
                        txt.disabled = '';
                    }
                }
            }
            "));


        // Force output filter to not resolve URLs in textboxes
        OutputFilter.CanResolveAllUrls = false;

        if (chkInheritBaseUrl.Checked)
        {
            txtForumBaseUrl.Text = baseUrl;
            txtForumBaseUrl.Enabled = false;
        }
        else
        {
            txtForumBaseUrl.Enabled = true;
        }

        if (chkInheritUnsubUrl.Checked)
        {
            txtUnsubscriptionUrl.Text = unsubscriptionUrl;
            txtUnsubscriptionUrl.Enabled = false;
        }
        else
        {
            txtUnsubscriptionUrl.Enabled = true;
        }


        chkInheritBaseUrl.Attributes.Add("onclick", "check('" + txtForumBaseUrl.ClientID + "', this,'" + baseUrl + "')");
        chkInheritUnsubUrl.Attributes.Add("onclick", "check('" + txtUnsubscriptionUrl.ClientID + "', this,'" + unsubscriptionUrl + "')");
    }


    /// <summary>
    /// Clears the form fields to default values.
    /// </summary>
    public override void ClearForm()
    {
        txtForumBaseUrl.Text = "";
        txtGroupDescription.Text = "";
        txtGroupDisplayName.Text = "";
        txtGroupName.Text = "";
        txtUnsubscriptionUrl.Text = "";
    }


    /// <summary>
    /// Reloads data.
    /// </summary>
    public override void ReloadData()
    {
        ClearForm();

        // Show/hide URL textboxes
        plcBaseAndUnsubUrl.Visible = (DisplayMode != ControlDisplayModeEnum.Simple);

        if (DisplayMode != ControlDisplayModeEnum.Simple)
        {
            SetUrls();
        }

        base.ReloadData();
    }


    /// <summary>
    /// Sets data to database.
    /// </summary>
    protected void btnOK_Click(object sender, EventArgs e)
    {
        if (!CheckPermissions("cms.forums", PERMISSION_MODIFY))
        {
            return;
        }

        // Generate code name in simple mode
        string codeName = txtGroupName.Text.Trim();
        if (DisplayMode == ControlDisplayModeEnum.Simple)
        {
            codeName = ValidationHelper.GetCodeName(txtGroupDisplayName.Text, 50) + "_group_" + CommunityGroupGUID;
        }

        string errorMessage = new Validator().NotEmpty(txtGroupDisplayName.Text, GetString("general.requiresdisplayname"))
            .NotEmpty(codeName, GetString("general.requirescodename"))
            .IsCodeName(codeName, GetString("general.errorcodenameinidentifierformat")).Result;

        if (!String.IsNullOrEmpty(errorMessage))
        {
            ShowError(errorMessage);
            return;
        }

        // If forum group with given name already exists show error message
        if (ForumGroupInfoProvider.GetForumGroupInfo(codeName, CMSContext.CurrentSiteID, CommunityGroupID) != null)
        {
            ShowError(GetString("Group_General.GroupAlreadyExists"));
            return;
        }

        ForumGroupInfo forumGroupObj = new ForumGroupInfo();
        forumGroupObj.GroupDescription = txtGroupDescription.Text.Trim();
        forumGroupObj.GroupDisplayName = txtGroupDisplayName.Text.Trim();
        forumGroupObj.GroupName = codeName;
        forumGroupObj.GroupOrder = 0;
        forumGroupObj.GroupEnableQuote = true;
        forumGroupObj.GroupLogActivity = true;
        if (DisplayMode != ControlDisplayModeEnum.Simple)
        {
            forumGroupObj.GroupBaseUrl = chkInheritBaseUrl.Checked ? null : txtForumBaseUrl.Text.Trim();
            forumGroupObj.GroupUnsubscriptionUrl = chkInheritUnsubUrl.Checked ? null : txtUnsubscriptionUrl.Text.Trim();
        }
        // Leave base url and unsubscription url empty for community forum group
        else if (CommunityGroupID > 0)
        {
            string baseUrl = String.Empty;
            // Get base url from automatically created forum group
            string forumGroupName = "Forums_group_" + CommunityGroupGUID;
            ForumGroupInfo autoGroup = ForumGroupInfoProvider.GetForumGroupInfo(forumGroupName, CMSContext.CurrentSite.SiteID, CommunityGroupID);
            if (autoGroup != null)
            {
                baseUrl = autoGroup.GroupBaseUrl;
            }

            forumGroupObj.GroupBaseUrl = baseUrl;
            forumGroupObj.GroupUnsubscriptionUrl = String.Empty;
        }

        if (CMSContext.CurrentSite != null)
        {
            forumGroupObj.GroupSiteID = CMSContext.CurrentSite.SiteID;
        }

        // Set the information on community group ID if available
        if (CommunityGroupID > 0)
        {
            forumGroupObj.GroupGroupID = CommunityGroupID;
        }

        ForumGroupInfoProvider.SetForumGroupInfo(forumGroupObj);

        mGroupId = forumGroupObj.GroupID;

        RaiseOnSaved();
    }
}