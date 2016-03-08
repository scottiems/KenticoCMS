using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;


using CMS.FormControls;
using CMS.FormEngine;
using CMS.GlobalHelper;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

public partial class CMSFormControls_Thinkgate_GreadCustomDDL : FormEngineUserControl
{
    public static int AssociationCount;
    private static readonly string CMSConnectionString = ConfigurationManager.ConnectionStrings["root_application"].ConnectionString;
     

    private static DataTable GetDataTable(string connectionString, string query)
    {
        DataTable myDataTable = new DataTable();

        if (query != null)
        {
            string ConnectionStringToUse = string.Empty;

            if (connectionString != null)
            {
                ConnectionStringToUse = connectionString;
            }
            else
            {
                ConnectionStringToUse = CMSConnectionString;
            }

            SqlConnection conn = new SqlConnection(ConnectionStringToUse);
            if (!(ConnectionStringToUse.Contains("Kentico")))
            {
                conn.Open();
                conn.ChangeDatabase(ThinkgateKenticoHelper.FindDBName());
            }

            SqlDataAdapter adapter = new SqlDataAdapter();
            adapter.SelectCommand = new SqlCommand(query, conn);

            try
            {
                //conn.Open();
            }
            catch (SqlException ex)
            {
                //Debug.WriteLine("SqlException: " + ex.Message);
                return myDataTable;
            }

            try
            {
                adapter.Fill(myDataTable);
            }
            finally
            {
                conn.Close();
            }
        }
        return myDataTable;
    }
    
    private static string getLocalDbConnectionString(string connString)
    {
        return connString.Replace("ThinkgateConfig", ThinkgateKenticoHelper.FindDBName());
    }

    /// <summary>
    /// Property used to access the Width parameter of the form control.
    /// </summary>
    public int SelectorWidth
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("SelectorWidth"), 0);
        }
        set
        {
            SetValue("SelectorWidth", value);
        }
    }

    /// <summary>
    /// Returns true if a color is selected. Otherwise, it returns false and displays an error message.
    /// </summary>
    public override bool IsValid()
    {
        if ((string)Value != "")
        {
            return true;
        }
        else
        {
            // Set form control validation error message.
            this.ValidationError = "Please choose a grade.";
            return false;
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    { 
        // Apply CSS class
        if (!String.IsNullOrEmpty(CssClass))
        {
            DropDownList1.CssClass = CssClass;
            CssClass = null;
        }
        else if (String.IsNullOrEmpty(DropDownList1.CssClass))
        {
            DropDownList1.CssClass = "DropDownField";
        }
        if (!String.IsNullOrEmpty(ControlStyle))
        {
            DropDownList1.Attributes.Add("style", ControlStyle);
            ControlStyle = null;
        }
        if (!IsPostBack)
        { GradeItems(); }
    }

    /// <summary>
    /// Sets up the internal DropDownList control.
    /// </summary>
    protected void GradeItems()
    {

        // Applies the width specified through the parameter of the form control if it is valid.

        if (SelectorWidth > 0)
        {
            DropDownList1.Width = SelectorWidth;
        }

        if (DropDownList1.Items.Count == 0)
        {
            string sql = string.Format("Select '0' as GradeValueField, '<Select One>'  as gradeTextField union  SELECT DISTINCT grade as GradeValueField ,grade as gradeTextField FROM CurrCourses C WHERE C.Active = 'Yes'");
            DataTable dt = GetDataTable(CMSConnectionString, sql);

            DropDownList1.DataSource = dt;
            DropDownList1.DataTextField ="gradeTextField";
            DropDownList1.DataValueField = "GradeValueField";
            DropDownList1.DataBind();
        }
    }

    /// <summary>
    /// Gets or sets form control value.
    /// </summary>
    public override object Value
    {
        get
        {
            return DropDownList1.SelectedValue;
        }
        set
        {
            // Ensure drop down list options
            GradeItems();
            DropDownList1.SelectedValue = System.Convert.ToString(value);
        }
    }
}