using System;
using CMS.Controls;
using CMS.FormControls;

public partial class CMSFormControls_DaysToMinutesConverter : FormEngineUserControl
{
    public override object Value
    {
        get
        {
            // convert to minutes
            var cmsTransformation = new CMSTransformation();
            return cmsTransformation.ConvertDaysToMinutes(txtMinutes.Text);
        }
        set
        {
            // convert to days
            var cmsTransformation = new CMSTransformation();
            txtMinutes.Text = cmsTransformation.ConvertMinutesToDays(value);
        }
    }

    /// <summary>
    /// Validates for numeric inputs
    /// </summary>
    /// <returns>If a value is supplied then return true for valid numeric numbers</returns>
    public override bool IsValid()
    {
        if (base.FieldInfo.AllowEmpty && string.IsNullOrWhiteSpace(txtMinutes.Text)) return true;
        double notUsed;
        return double.TryParse(txtMinutes.Text, out notUsed);
    }
}