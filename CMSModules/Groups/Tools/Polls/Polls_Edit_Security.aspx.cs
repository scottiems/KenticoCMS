using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.Polls;
using CMS.UIControls;

public partial class CMSModules_Groups_Tools_Polls_Polls_Edit_Security : CMSGroupPollsPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        PollSecurity.IsLiveSite = false;
        PollSecurity.ItemID = QueryHelper.GetInteger("pollid", 0);
        PollSecurity.OnCheckPermissions += new CMSAdminControl.CheckPermissionsEventHandler(PollSecurity_OnCheckPermissions);
        PollSecurity.Enabled = CheckGroupPermissions(CMSContext.CurrentSiteID, CMSAdminControl.PERMISSION_MANAGE, false);
    }


    private void PollSecurity_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        PollInfo pi = PollInfoProvider.GetPollInfo(PollSecurity.ItemID);
        int groupId = 0;

        if (pi != null)
        {
            groupId = pi.PollGroupID;
        }

        // Check permissions
        if (!CMSContext.CurrentUser.IsGroupAdministrator(groupId) || !CMSContext.CurrentUser.IsAuthorizedPerResource("CMS.Groups", CMSAdminControl.PERMISSION_MANAGE))
        {
            sender.StopProcessing = true;
        }
    }
}