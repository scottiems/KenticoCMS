using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.FormControls;
using CMS.FormEngine;
using CMS.GlobalHelper;
using CMS.CMSHelper;
using CMS.ExtendedControls;
using CMS.SettingsProvider;

public partial class CMSFormControls_Basic_MetaFileUploadControl : FormEngineUserControl
{
    #region "Variables"

    private string mValue = null;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets or sets the enabled state of the control.
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return fileUploader.Enabled;
        }
        set
        {
            fileUploader.Enabled = value;
        }
    }


    /// <summary>
    /// Gets or sets field value.
    /// </summary>
    public override object Value
    {
        get
        {
            return mValue;
        }
        set
        {
            mValue = (value != null ? value.ToString() : null);
        }
    }


    /// <summary>
    /// Field info object.
    /// </summary>
    public override FormFieldInfo FieldInfo
    {
        get
        {
            return base.FieldInfo;
        }
        set
        {
            base.FieldInfo = value;
            fileUploader.FieldInfo = value;
        }
    }


    /// <summary>
    /// Indicates if control is placed on live site.
    /// </summary>
    public override bool IsLiveSite
    {
        get
        {
            return fileUploader.IsLiveSite;
        }
        set
        {
            fileUploader.IsLiveSite = value;
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Set control properties from parent Form
        if (Form != null)
        {
            Form.OnBeforeRedirect += new EventHandler(Form_OnBeforeRedirect);

            BaseInfo info = Form.EditedObject as BaseInfo;

            if (info != null)
            {
                string category = ValidationHelper.GetString(GetValue("ObjectCategory"), CMSConstants.OBJECT_CATEGORY_THUMBNAIL);

                fileUploader.ObjectType = info.TypeInfo.ObjectType;
                fileUploader.Category = category;

                if (info.Generalized.ObjectSiteID > 0)
                {
                    fileUploader.SiteID = info.Generalized.ObjectSiteID;
                }

                if (Form.Mode == FormModeEnum.Insert)
                {
                    fileUploader.ObjectID = 0;
                }
                else
                {
                    fileUploader.ObjectID = info.Generalized.ObjectID;
                }
            }
        }

        // Set style properties of control
        if (!String.IsNullOrEmpty(ControlStyle))
        {
            fileUploader.Attributes.Add("style", ControlStyle);
            ControlStyle = null;
        }
        if (!String.IsNullOrEmpty(CssClass))
        {
            fileUploader.CssClass = CssClass;
            CssClass = null;
        }

        CheckFieldEmptiness = false;
    }


    private void Form_OnBeforeRedirect(object sender, EventArgs e)
    {
        if (Form.Mode == FormModeEnum.Insert)
        {
            BaseInfo info = ((UIForm)Form).EditedObject as BaseInfo;

            if (info != null)
            {
                // Upload new metafile
                fileUploader.ObjectID = info.Generalized.ObjectID;
                fileUploader.UploadFile();
            }
        }
    }

    #endregion

}