﻿using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.GlobalHelper;
using CMS.UIControls;

public partial class CMSModules_Modules_Pages_Development_Module_Edit_PermissionName_Edit_Frameset : SiteManagerPage
{
    #region "Variables"

    private int mModuleId;
    private int mPermissionId;
    private int mTabIndex;
    private int mSaved;

    #endregion


    #region "Page Events"

    protected void Page_Load(object sender, EventArgs e)
    {
        mModuleId = QueryHelper.GetInteger("moduleId", 0);
        mPermissionId = QueryHelper.GetInteger("permissionId", 0);
        mTabIndex = QueryHelper.GetInteger("tabindex", 0);
        mSaved = QueryHelper.GetInteger("saved", 0);

        if (mTabIndex == 0)
        {
            editContent.Attributes["src"] = ResolveUrl("~/CMSModules/Modules/Pages/Development/Module_Edit_PermissionName_Edit.aspx") + "?hidebreadcrumbs=1&moduleId=" + mModuleId + "&permissionId=" + mPermissionId + "&saved=" + mSaved + "&tabIndex=" + mTabIndex;
        }
        else
        {
            editContent.Attributes["src"] = ResolveUrl("~/CMSModules/Modules/Pages/Development/Module_Edit_PermissionName_Roles.aspx") + "?moduleID=" + mModuleId + "&permissionId=" + mPermissionId + "&saved=" + mSaved + "&tabIndex=" + mTabIndex;
        }
    }

    #endregion
}