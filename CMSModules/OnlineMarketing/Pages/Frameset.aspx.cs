using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.GlobalHelper;
using CMS.UIControls;

public partial class CMSModules_OnlineMarketing_Pages_Frameset : CMSDeskPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        frameMenu.Attributes.Add("src", "Header.aspx" + URLHelper.Url.Query);
    }
}