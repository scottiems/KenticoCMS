using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Business;
using System.Data;
using Telerik.Web.UI;

public partial class CMSWebParts_ThinkgateWebParts_IMRating_SpecialPopulation : System.Web.UI.Page
{

    private RatingBusiness _ratingBusiness;
    private int _nodeId;
    private int _userId;
    private List<int> _specialPopulations;
    private DataTable existingReviewDataTable;
    private string _grade;


    protected void Page_Load(object sender, EventArgs e)
    {
        Initialize();

        if (!Page.IsPostBack)
        {
            GetData();
            BindData();
        }
    }
    private void Initialize()
    {
        _ratingBusiness = new RatingBusiness();
        _nodeId = Request.QueryString["nodeID"] == null
            ? default(int)
            : Convert.ToInt32(Request.QueryString["nodeID"]);
        _userId = Request.QueryString["userID"] == null
           ? default(int)
           : Convert.ToInt32(Request.QueryString["userID"]);

       
    }

    /// <summary>
    /// Get the data from database for questionid
    /// </summary>
    private void GetData()
    {
        existingReviewDataTable = _ratingBusiness.GetExistingReview(_nodeId, _userId);
    }

    private void BindData()
    {
        lblAgeValue.Text = existingReviewDataTable.Rows[0]["Age"] != null && existingReviewDataTable.Rows[0]["Age"].ToString() != "0" ? existingReviewDataTable.Rows[0]["Age"].ToString() : string.Empty;
        lblGradeValue.Text = existingReviewDataTable.Rows[0]["Grade"] != null ? existingReviewDataTable.Rows[0]["Grade"].ToString() : string.Empty;
        _specialPopulations = new List<int>();
        foreach (DataRow row in existingReviewDataTable.Rows)
        {
            if (!string.IsNullOrEmpty(row["SpecialPopulationID"].ToString()))
            {
                _specialPopulations.Add(Convert.ToInt32(row["SpecialPopulationID"]));
            }
         }

        PopulateSpecialPopulations();
    }
    /// <summary>
    /// Fires on item bound of special population list
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void specialPopulations_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        DataRowView specialPopulation = e.Item.DataItem as DataRowView;

        Label specialPopulationLabel = e.Item.FindControl("lblSpecialPopulation") as Label;
        if (specialPopulationLabel != null && specialPopulation.Row[1] != null)
            specialPopulationLabel.Text = specialPopulation.Row[1].ToString();

        if (specialPopulation != null && !string.IsNullOrEmpty(specialPopulation.Row[0].ToString()))
        {
            if (_specialPopulations != null
                && _specialPopulations.Exists(exists => exists == Convert.ToInt32(specialPopulation.Row[0])))
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
    /// Loads all the special populations from the database into the UI
    /// </summary>
    private void PopulateSpecialPopulations()
    {
        //Populate special population details
        var kenticoUsername = CMS.SiteProvider.UserInfoProvider.GetUserNameById(_userId);
        var clientID = ThinkgateKenticoHelper.getClientIDFromKenticoUserName(kenticoUsername);
        rptSpecialPopulation.DataSource = _ratingBusiness.GetActiveSpecialPopulations(clientID);
        rptSpecialPopulation.DataBind();
    }

}