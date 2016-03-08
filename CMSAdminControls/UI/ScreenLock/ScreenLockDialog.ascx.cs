using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;

using CMS.UIControls;
using CMS.GlobalHelper;
using CMS.CMSHelper;
using CMS.SettingsProvider;
using CMS.SiteProvider;

public partial class CMSAdminControls_UI_ScreenLock_ScreenLockDialog : CMSUserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        bool enabled = SecurityHelper.IsScreenLockEnabled(CMSContext.CurrentSiteName);

        if (enabled)
        {
            // Set title text and image
            screenLockTitle.TitleText = "Screen lock";
            screenLockTitle.TitleImage = GetImageUrl("/Design/Controls/Dialogs/lock_time.png");
            imgWarning.ImageUrl = GetImageUrl("/Design/Controls/Dialogs/lock_time.png");

            txtUserName.Text = CMSContext.CurrentUser.UserName;

            ScriptHelper.RegisterJQueryDialog(Page);
        }
        
        Visible = enabled;
    }
}