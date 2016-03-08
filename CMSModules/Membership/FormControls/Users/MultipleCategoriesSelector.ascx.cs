using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.CMSHelper;
using CMS.FormControls;
using CMS.GlobalHelper;

public partial class CMSModules_Membership_FormControls_Users_MultipleCategoriesSelector : FormEngineUserControl
{
    /// <summary>
    /// Gets coma separated ID of selected categories.
    /// </summary>
    public override object Value
    {
        get
        {
            // Return string of categories ID
            return categorySelector.Value;
        }
        set
        {
            if (URLHelper.IsPostback())
            {
                categorySelector.Value = ValidationHelper.GetString(value, "");
            }
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
            base.IsLiveSite = value;
            categorySelector.IsLiveSite = value;
        }
    }


    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        Form.OnAfterDataLoad += Form_OnAfterDataLoad;
        Form.OnAfterSave += Form_OnAfterSave;
    }


    private void Form_OnAfterSave(object sender, EventArgs e)
    {
        // Set document ID - insert mode
        int documentID = ValidationHelper.GetInteger(Form.Data.GetValue("DocumentID"), 0);
        if (documentID > 0)
        {
            categorySelector.DocumentID = documentID;
        }

        categorySelector.Save();
    }


    private void Form_OnAfterDataLoad(object sender, EventArgs e)
    {
        // Set document ID - edit mode
        int documentID = ValidationHelper.GetInteger(Form.Data.GetValue("DocumentID"), 0);
        if (documentID > 0)
        {
            categorySelector.DocumentID = documentID;
        }
        // Set user ID
        if (CMSContext.CurrentUser != null)
        {
            categorySelector.UserID = CMSContext.CurrentUser.UserID;
        }
    }
}