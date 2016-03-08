using System;
using System.Web;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Collections;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.FormControls;
using CMS.FormEngine;
using CMS.SettingsProvider;
using CMS.UIControls;

public partial class CMSModules_AdminControls_Controls_Class_FieldEditor_ControlSettings : CMSUserControl
{
    #region "Variables"

    private FormInfo fi = null;
    private static Hashtable mSettings = null;

    #endregion


    #region "Properties"

    /// <summary>
    /// FormInfo for specific control.
    /// </summary>
    public FormInfo FormInfo
    {
        get
        {
            return fi;
        }
        set
        {
            fi = value;
        }
    }


    /// <summary>
    /// Shows in what control is this form used.
    /// </summary>
    public FormTypeEnum FormType
    {
        get
        {
            return form.FormType;
        }
        set
        {
            form.FormType = value;
        }
    }


    /// <summary>
    /// Field settings hashtable.
    /// </summary>
    public Hashtable Settings
    {
        get
        {
            return mSettings;
        }
        set
        {
            mSettings = new Hashtable(value, StringComparer.InvariantCultureIgnoreCase);
        }
    }


    /// <summary>
    /// Basic form data.
    /// </summary>
    public DataRow FormData
    {
        get
        {
            return form.DataRow;
        }
    }


    /// <summary>
    /// Sets basicform to simple or advanced mode.
    /// </summary>
    public bool SimpleMode
    {
        get
        {
            return fms.SimpleMode;
        }
        set
        {
            fms.SimpleMode = value;
        }
    }


    /// <summary>
    /// Determines whether to allow mode switching (simple <-> advanced).
    /// </summary>
    public bool AllowModeSwitch
    {
        get
        {
            return form.AllowModeSwitch;
        }
        set
        {
            form.AllowModeSwitch = value;
        }
    }


    /// <summary>
    /// Defines minimum of items needed to be visible to display mode switch
    /// </summary>
    public int MinItemsToAllowSwitch
    {
        get
        {
            return fms.MinItemsToAllowSwitch;
        }
        set
        {
            fms.MinItemsToAllowSwitch = value;
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        fms.AddForm(form);
        pnlSettings.GroupingText = GetString("templatedesigner.section.settings");
    }


    /// <summary>
    /// Checks if form is loaded with any controls and returns appropriate value.
    /// </summary>
    public bool CheckVisibility()
    {
        Visible = true;

        if (!form.IsAnyFieldVisible() && !AllowModeSwitch)
        {
            Visible = false;
        }

        if (form.FormInformation != null && !form.FormInformation.ItemsList.Any())
        {
            Visible = false;
        }

        return Visible;
    }


    /// <summary>
    /// Reloads control.
    /// </summary>
    public void Reload(bool forceReloadCategories)
    {
        if (fi != null)
        {
            form.SubmitButton.Visible = false;
            form.SiteName = CMSContext.CurrentSiteName;
            form.FormInformation = FormInfo;
            form.Data = GetData();
            form.ShowPrivateFields = true;
            form.ForceReloadCategories = forceReloadCategories;
        }
        else
        {
            form.DataRow = null;
            form.FormInformation = null;
        }

        fms.ReloadForms();
    }


    /// <summary>
    /// Saves basic form data.
    /// </summary>
    public bool SaveData()
    {
        if (form.Visible && form.FieldControls != null && form.FieldControls.Count != 0)
        {
            return form.SaveData(null);
        }

        return true;
    }


    /// <summary>
    /// Loads DataRow for BasicForm with data from FormFieldInfo settings.
    /// </summary>
    private DataRowContainer GetData()
    {
        DataRowContainer result = new DataRowContainer(FormInfo.GetDataRow());

        if (Settings != null)
        {
            foreach (string columnName in result.ColumnNames)
            {
                if (Settings.ContainsKey(columnName) && !String.IsNullOrEmpty(Convert.ToString(Settings[columnName])))
                {
                    result[columnName] = Settings[columnName];
                }
            }
        }
        return result;
    }

    #endregion
}