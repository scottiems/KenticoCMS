using System;
using CMS.CMSHelper;
using CMS.DocumentEngine;
using CMS.SiteProvider;

public partial class CMSWebParts_ThinkgateWebParts_IMRating_RatingAverageUC : System.Web.UI.UserControl
{
    public int NodeID { get; set; }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (NodeID == default (int))
        {
            throw new Exception("Node ID not found");
        }

        TreeProvider tp = new TreeProvider(UserInfoProvider.GetFullUserInfo(CMSContext.CurrentUser.UserName));
        TreeNode doc = DocumentHelper.GetDocument(NodeID, "en-US", tp);
        decimal avgRating = doc.GetValue("AverageRating") != null ? Convert.ToDecimal( doc.GetValue("AverageRating").ToString() ) : 0;
        int ratingCount = doc.GetValue("RatingCount") != null ? Convert.ToInt32( doc.GetValue("RatingCount").ToString() ) : 0;

        if (avgRating != 0 && ratingCount != 0)
        {
            lblReviewFormat.Text =  ratingCount + " Reviews - Rating: " + avgRating.ToString("0.00") +"/5";
            rating.Value = avgRating;
            lblNoReviewMsg.Visible = false;
        }
        else
        {
            if (ratingCount != 0)
            {
                lblReviewFormat.Text = ratingCount + " Reviews - No Rating";
                rating.Value = avgRating;
                lblNoReviewMsg.Visible = false;
            }
            else
            {
                
            
            lblReviewFormat.Visible = false;
            lblNoReviewMsg.Text = "No Reviews/Not Rated";
            }
        }
    }
}