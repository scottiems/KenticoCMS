using System;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
using System.Globalization;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;
using Business;
using Classes.Rating;
using System.Data;
using CMS.CMSHelper;
using Telerik.Web.UI;

public partial class CMSWebParts_ThinkgateWebParts_IMRating_RatingReviewSummary : Page
{
   

    private RatingBusiness _ratingBusiness;
    private ReviewRatingSummary _reviewRatingSummary;
        private ReviewsCountAverage _reviews;


    private int _nodeId;

        private const string RATING_VALUE = "RatingValue";
        private const string HTML_NEW_LINE = "<br>";

        protected void Page_Init(object sender, EventArgs e)
        {
            if (!this.ClientScript.IsClientScriptBlockRegistered(this.GetType(), "ApplicationVirtualPath"))
            {
                string script = "var applicationVirtualPath = '" + Request.ApplicationPath + "';";
                this.ClientScript.RegisterClientScriptBlock(this.GetType(), "ApplicationVirtualPath", script, true);
            }
        }
 
        protected void Page_Load(object sender, EventArgs e)
        {
            Initialize();

            if (!Page.IsPostBack)
            {
                GetData();
                BindData();
            }

         }
        /// <summary>
        /// Initialize the instances and get the respective itemId from query string
        /// </summary>
        private void Initialize()
        {
            _ratingBusiness = new RatingBusiness();
            _nodeId = Request.QueryString["nodeID"] == null
                ? default(int)
                : Convert.ToInt32(Request.QueryString["nodeID"]);
            nodeId.Value = _nodeId.ToString(CultureInfo.InvariantCulture);

            if (string.IsNullOrEmpty(hidActionId.Value) && !string.IsNullOrEmpty(Request.QueryString["ActionId"]))
                hidActionId.Value = Request.QueryString["ActionId"];
            else
                hidActionId.Value = btnShowAll.ClientID.ToString(CultureInfo.InvariantCulture);

            if (string.IsNullOrEmpty(needRefresh.Value) && !string.IsNullOrEmpty(Request.QueryString["needRefresh"]))
                needRefresh.Value = Request.QueryString["needRefresh"];
        }

        /// <summary>
        /// Get the data from database for questionid
        /// </summary>
        private void GetData()
        {
            GetAllReviewRatingSummary();
            GetIMReviewsByRatingValue();
        }

        /// <summary>
        /// Get review rating summary
        /// </summary>
        private void GetAllReviewRatingSummary()
        {
            _reviewRatingSummary = _ratingBusiness.GetAllReviewRatingSummary(_nodeId);
        }


        /// <summary>
        /// Bind the data to the control
        /// </summary>
        private void BindData()
        {
            if (_reviewRatingSummary != default(ReviewRatingSummary))
            {
                SetReviewAverageTotalCountAndNaReview();

                if (_reviewRatingSummary.RatingSummary != null)
                {
                    rptRating.DataSource = _reviewRatingSummary.RatingSummary.Where(filter => filter.Key != -1);
                    rptRating.DataBind();
                }
            }

            if (_reviews != default(ReviewsCountAverage))
            {
                rptReviewSummary.DataSource = _reviews.Reviews;
                rptReviewSummary.DataBind();
            }
        }

        /// <summary>
        /// Set the review average, total count and n/a review count
        /// </summary>
        private void SetReviewAverageTotalCountAndNaReview()
        {
            lblTotalRating.Text = string.Format("{0} Reviews ", _reviewRatingSummary.ReviewCount);
            SetAverageRating();
            SetShowAll();
            SetNAReviewCount();
        }

        private void SetAverageRating()
        {
            lblAverageRating.Text = string.Format("{0:0.##} Stars", _reviewRatingSummary.AverageRating);
            hidAverageRating.Value = _reviewRatingSummary.AverageRating.ToString(CultureInfo.InvariantCulture);
            averageRating.Value = _reviewRatingSummary.AverageRating;
        }
        /// <summary>
        /// Set Show All 
        /// </summary>
        private void SetShowAll()
        {
            if (_reviewRatingSummary.RatingSummary.Any(any => any.Value != 0))
            {
                btnShowAll.Attributes.Add(RATING_VALUE, null);
                lblShowAll.Visible = false;
            }
            else
            {
                btnShowAll.Visible = false;
            }


        }
        /// <summary>
        /// Set No review given count
        /// </summary>
        private void SetNAReviewCount()
        {
            if (_reviewRatingSummary.RatingSummary != null && _reviewRatingSummary.RatingSummary.Any(any => any.Key == -1))
            {
                int reviewCount = _reviewRatingSummary.RatingSummary.FirstOrDefault(first => first.Key == -1).Value;

                if (reviewCount != 0)
                {
                    lblNAReviewCount.Visible = false;

                    btnNAReviewCount.Text = reviewCount.ToString(CultureInfo.InvariantCulture);
                    btnNAReviewCount.Attributes.Add(
                        RATING_VALUE,
                        _reviewRatingSummary.RatingSummary.FirstOrDefault(
                            first => first.Key == -1)
                                            .Key.ToString(CultureInfo.InvariantCulture));
                }
                else
                {
                    btnNAReviewCount.Visible = false;
                }
            }
        }
        /// <summary>
        /// Fires when rating count datasource are available
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void rptRating_OnItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            KeyValuePair<decimal, int> rating = (KeyValuePair<decimal, int>)e.Item.DataItem;

            RadRating ctrlRadRating = e.Item.FindControl("rating") as RadRating;
            if (ctrlRadRating != null)
            {
                ctrlRadRating.Value = rating.Key;
            }

            SetRatingReviewCountLink(e, rating);

        }


        /// <summary>
        /// Fires when review details are available
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void rptReviewSummary_OnItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            Review review = e.Item.DataItem as Review;

            if (review != default(Review))
            {
                SetCommentControl(e, review);
                SetRatingCreatedByAndDateControls(e, review);
                SetEditAndDeleteButtonRules(e, review);
            }
        }

    /// <summary>
    /// Filter the review details based on user selection
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void showReviews_OnClick(object sender, EventArgs e)
    {
        LinkButton btnLink = sender as LinkButton;
        if (btnLink != null )
        {
            GetIMReviewsByRatingValue(btnLink.Attributes[RATING_VALUE]);
            GetAllReviewRatingSummary();
            BindData();
            hidActionId.Value = (btnLink.Parent as RepeaterItem) != null ? string.Format("rptRating_{0}",btnLink.ClientID.ToString(CultureInfo.InvariantCulture)) : btnLink.ClientID.ToString(CultureInfo.InvariantCulture);

        }
     }

 
        /// <summary>
        /// Filter rating for question Id
        /// </summary>
        /// <param name="ratingValue"></param>
        private void GetIMReviewsByRatingValue(string ratingValue = null)
        {
            _reviews = _ratingBusiness.GetReviewsByRatingValue(_nodeId, ratingValue);
        }

        /// <summary>
        /// Set the rating review count 
        /// </summary>
        /// <param name="e"></param>
        /// <param name="rating"></param>
        private void SetRatingReviewCountLink(RepeaterItemEventArgs e, KeyValuePair<decimal, int> rating)
        {
            Label ctrlReviewCountLabel = e.Item.FindControl("lblReviewCount") as Label;
            LinkButton ctrlReviewCount = e.Item.FindControl("btnReviewCount") as LinkButton;

            if (rating.Value == default(int))
            {
                if (ctrlReviewCountLabel != null)
                {
                    ctrlReviewCountLabel.Text = rating.Value.ToString(CultureInfo.InvariantCulture);
                }
                if (ctrlReviewCount != null)
                {
                    ctrlReviewCount.Visible = false;
                }
            }
            else
            {
                if (ctrlReviewCountLabel != null)
                {
                    ctrlReviewCountLabel.Visible = false;
                }
                if (ctrlReviewCount != null)
                {
                    ctrlReviewCount.Attributes.Add(RATING_VALUE, rating.Key.ToString(CultureInfo.InvariantCulture));
                    ctrlReviewCount.Text = rating.Value.ToString(CultureInfo.InvariantCulture);
                }
            }
        }

        /// <summary>
        /// Set the comment control based on questionreview
        /// </summary>
        /// <param name="e"></param>
        /// <param name="review"></param>
        private void SetCommentControl(RepeaterItemEventArgs e, Review review)
        {
            Literal ctrlComment = e.Item.FindControl("ltrReviewLimited") as Literal;

            if (ctrlComment != null && !string.IsNullOrEmpty(review.Comment))
            {
                ctrlComment.Text = review.Comment.Length > 100 ?
                                   review.Comment.Substring(0, 100) :
                                   review.Comment;

                if (review.Comment.Length > 100)
                {
                    LinkButton moreLink = e.Item.FindControl("btnComment") as LinkButton;
                    if (moreLink != null)
                    {
                        moreLink.Visible = true;
                    }

                    Literal ctrlCommentFull = e.Item.FindControl("ltrReviewFull") as Literal;
                    if (ctrlCommentFull != null)
                    {
                        ctrlCommentFull.Text = review.Comment;
                    }
                }
            }
        }

    protected void btnDelete_OnClick(object sender, EventArgs e)
    {
        Button btnDelete = sender as Button;

        if (btnDelete != null)
        {
            _ratingBusiness.DeleteReview(Convert.ToInt32(btnDelete.Attributes["ReviewId"]), _nodeId);

            SetRefreshFlag();

            if (!string.IsNullOrEmpty(hidActionId.Value))
            {
                LinkButton btnLink = (LinkButton)FindControl(hidActionId.Value);

                if (btnLink != null)
                {
                    GetIMReviewsByRatingValue(btnLink.Attributes[RATING_VALUE]);
                }
            }
            needRefresh.Value = "Yes";
            GetAllReviewRatingSummary();
            BindData();
        }
    }

    /// <summary>
    /// Set the Refresh Flag on parent window to refresh the review if there is change in review.
    /// </summary>
    private void SetRefreshFlag()
    {
        ScriptManager.RegisterStartupScript(Page, typeof(Page), "setRefreshOnDialogClose", "setRefreshOnDialogClose();", true);
        ScriptManager.RegisterStartupScript(Page, typeof(Page), "setParentRefreshFlag", "setParentRefreshFlag();", true);
    }


    private void SetRatingCreatedByAndDateControls(RepeaterItemEventArgs e, Review review)
    {
        RadRating ctrlRating = e.Item.FindControl("rating") as RadRating;
        
        if (ctrlRating != null)
        {
            ctrlRating.Value = review.Rating;
        }

        Label ctrlReviewer = e.Item.FindControl("lblReviewer") as Label;

        if (ctrlReviewer != null)
        {
            var kenticoUsername = CMS.SiteProvider.UserInfoProvider.GetUserNameById(review.UserID);
            var clientID = ThinkgateKenticoHelper.getClientIDFromKenticoUserName(kenticoUsername);
            var username = _ratingBusiness.GetFullNameFromKenticoUsername(kenticoUsername, clientID);
            ctrlReviewer.Text = String.Format("{0}{1}{2}",
                username, HTML_NEW_LINE, review.RoleName);
        }

        Label ctrlDate = e.Item.FindControl("lblDate") as Label;

        if (ctrlDate != null)
        {
            ctrlDate.Text = review.CreateDate.ToShortDateString();
        }
    }


    private void SetEditAndDeleteButtonRules(RepeaterItemEventArgs e, Review review)
    {
        Button btnEdit = e.Item.FindControl("btnEdit") as Button;
        if (btnEdit != null)
        {
            btnEdit.Visible = (review.UserID == CMSContext.CurrentUser.UserID);
        }

        Button btnDelete = e.Item.FindControl("btnDelete") as Button;
        if (btnDelete != null)
        {
            btnDelete.Visible = ((review.UserID == CMSContext.CurrentUser.UserID)
                                 || ThinkgateKenticoHelper.GetUsersE3RoleList(CMSContext.CurrentUser).Contains("District Administrator"));
        }

    }
}
