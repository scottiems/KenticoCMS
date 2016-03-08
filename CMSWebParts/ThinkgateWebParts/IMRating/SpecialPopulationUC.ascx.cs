using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using Business;
using System.Data;
using Telerik.Web.UI;

public partial class CMSWebParts_ThinkgateWebParts_IMRating_SpecialPopulationUC : UserControl
{
    #region Public Variables

    public int Age { get; set; }
    public string Grade { get; set; }
    public List<int> SpecialPopulations { get; set; }

    #endregion Public Variables


    #region Private Variables

    private RatingBusiness _ratingBusiness;

    #endregion Private Variables
   


    protected void Page_Load(object sender, EventArgs e)
    {
        _ratingBusiness = new RatingBusiness();

        if (!Page.IsPostBack)
        {
            PopulateData();
        }
    }

    /// <summary>
    /// Fires on item bound of special population list
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void specialPopulations_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        DataRowView specialPopulation = e.Item.DataItem as DataRowView;

        if (specialPopulation != null && !string.IsNullOrEmpty(specialPopulation.Row[0].ToString()))
        {
            if (SpecialPopulations != null
                && SpecialPopulations.Exists(exists => exists == Convert.ToInt32(specialPopulation.Row[0])))
            {
                RadButton checkBox = e.Item.FindControl("chkSpecialPopulation") as RadButton;
                if (checkBox != null)
                {
                    checkBox.Checked = true;
                }

            }
        }
    }

    /// <summary>
    /// Populate the data from review facade and bind the dropdowns.
    /// </summary>
    private void PopulateData()
    {
        PopulateAgeDropDown();
        SetAgeSelection();

        PopulateGradeDropDown();
        SetGradeSelection();
        
        PopulateSpecialPopulations();
    }


    /// <summary>
    /// Populates the Age drop down list with all the Ages (5-18) from the Database
    /// </summary>
    private void PopulateAgeDropDown()
    {
        //Populate age details

        ddlAge.DataSource = _ratingBusiness.GetAllAgesFromClient();
        ddlAge.DataTextField = "Age";
        ddlAge.DataValueField = "Age";
        ddlAge.DataBind();
        ddlAge.Items.Insert(0, "Select Age");
    }

    /// <summary>
    /// On an edit to an existing review this method will display the last saved age.  On a new review it
    /// will default to the first age in the list.                                                                               
    /// </summary>
    private void SetAgeSelection()
    {
        ddlAge.SelectedValue = Age.ToString();
    }


    /// <summary>
    /// Populates the Grades drop down list with all the Grades from the Database
    /// </summary>
    private void PopulateGradeDropDown()
    {
        //Populate grade details
        ddlGrade.DataSource = _ratingBusiness.GetAllGradesFromClient();
        ddlGrade.DataTextField = "Grade";
        ddlGrade.DataValueField = "Grade";
        ddlGrade.DataBind();
        ddlGrade.Items.Insert(0, "Select Grade");
    }

    /// <summary>
    /// On an edit to an existing review this method will display the last saved Grade.  On a new review it
    /// will default to the first grade in the list.
    /// </summary>
    private void SetGradeSelection()
    {
        ddlGrade.SelectedValue = Grade;
    }

  
    /// <summary>
    /// Loads all the special populations from the database into the UI
    /// </summary>
    private void PopulateSpecialPopulations()
    {
        //Populate special population details
        rptSpecialPopulation.DataSource = _ratingBusiness.GetActiveSpecialPopulations();
        rptSpecialPopulation.DataBind();
    }

   

}