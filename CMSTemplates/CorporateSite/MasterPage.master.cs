using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.UIControls;


public partial class CMSTemplates_CorporateSite_MasterPage : TemplateMasterPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        base.CreateChildControls();

        PageManager = manPortal;
    }

    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        ltlTags.Text = HeaderTags;
    }
}
