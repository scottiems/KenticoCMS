
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CMS.GlobalHelper;
using Telerik.Web.UI;

public partial class CMSFormControls_Selectors_Thinkgate_StandardSelector : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        ScriptHelper.RegisterWOpenerScript(Page);
        ScriptHelper.RegisterClientScriptBlock(Page, Page.GetType(), "CloseDialog", ScriptHelper.CloseDialogScript);
    }
}