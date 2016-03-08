using System;
using System.Collections.Generic;
using System.Data;
using Classes.Rating;
using DataAccess;
using System.Linq;
using DocumentFormat.OpenXml.Drawing.Charts;
using Helpers;
using DataTable = System.Data.DataTable;


namespace Business
{
    /// <summary>
    /// Summary description for Rating
    /// </summary>
    public class RatingBusiness
    {
        private readonly RatingDataAccess _ratingDataAccess;

        public RatingBusiness()
        {
            _ratingDataAccess = new RatingDataAccess();
        }

        public DataTable GetAllAgesFromClient()
        {
            return _ratingDataAccess.GetAllAgesFromClientDB();
        }

        public DataTable GetAllGradesFromClient()
        {
            return _ratingDataAccess.GetAllGradesFromClientDB();
        }

        public DataTable GetActiveSpecialPopulations(string clientId = null)
        {
            return _ratingDataAccess.GetActiveSpecialPopulationFromClientDB(clientId);
        }

        public DataTable GetExistingReview(int nodeid, int userid)
        {
            return _ratingDataAccess.GetExistingReviewFromKenticoDB(nodeid, userid);
        }

        public void SaveReview(Review review)
        {
            if (review != null)
            {
                _ratingDataAccess.SaveReviewInKenticoDB(review);
                _ratingDataAccess.UpdateRatingAvgNCountsInKenticoDB(review.NodeID);
            }
        }
        public string GetFullNameFromKenticoUsername(string kenticoUserName, string clientId)
        {

           DataTable dt = _ratingDataAccess.GetFullNameFromKenticoUsernameFromSpecificClient(kenticoUserName, clientId);
            string userFullName = "Name not found";
            if (dt != null && dt.Rows.Count == 1)
            {
                userFullName = dt.Rows[0]["FullName"] != null ? dt.Rows[0]["FullName"].ToString() : "Name not found";
            }
            return userFullName;
        }
        

        public ReviewRatingSummary GetAllReviewRatingSummary(int nodeid)
        {
            DataTable dt = _ratingDataAccess.GetAllReviewRatingSummaryFromKenticoDB(nodeid);
            return ConvertDataTableToReviewSummaryDTO(dt);
        }

        public ReviewsCountAverage GetReviewsByRatingValue(int nodeid, string ratingValue = null)
        {
            DataTable dt = _ratingDataAccess.GetReviewsByRatingValueFromKenticoDB(nodeid, ratingValue);
            return ConvertDataTableToReviewsCountAverageDTO(dt);
        }

        public void DeleteReview(int reviewId, int nodeId)
        {
            _ratingDataAccess.DeleteReviewInKenticoDB(reviewId);
            
        }

        private ReviewRatingSummary ConvertDataTableToReviewSummaryDTO(DataTable dt)
        {
            ReviewRatingSummary reviewRatingSummary = new ReviewRatingSummary();

            foreach (DataRow dr in dt.Rows)
            {
                reviewRatingSummary.AverageRating = Convert.ToDecimal(dr.GetOrdinal<double>("AverageRating"));
                reviewRatingSummary.ReviewCount = dr.GetOrdinal<int>("ReviewCount");

                if (reviewRatingSummary.RatingSummary == null)
                    reviewRatingSummary.RatingSummary = new Dictionary<decimal, int>();

                reviewRatingSummary.RatingSummary.Add(Convert.ToDecimal(dr.GetOrdinal<double>("RatingValue")),
                    dr.GetOrdinal<int>("RatingCount"));
            }

            return reviewRatingSummary;
        }

        private ReviewsCountAverage ConvertDataTableToReviewsCountAverageDTO(DataTable dt)
        {
            ReviewsCountAverage reviews = new ReviewsCountAverage
            {
                Reviews = new List<Review>()
            };
            foreach (DataRow dr in dt.Rows)
            {
                reviews.AverageRating = Convert.ToDecimal(dr.GetOrdinal<double>("AverageRating"));
                reviews.ReviewCount = Convert.ToInt32(dr.GetOrdinal<double>("ReviewCount"));
                reviews.NodeId = dr.GetOrdinal<int>("NodeID");


                int questionReviewID = dr.GetOrdinal<int>("ReviewID");

                if (!reviews.Reviews.Exists(
                    exists => exists.ID == questionReviewID))
                {
                    DataTable dataTable = dt.Clone();
                    dataTable.LoadDataRow(dr.ItemArray, true);

                    reviews.Reviews.Add(convertDataTableToReviewDTO(dataTable));
                }
               

            }

            return reviews;
        }

        private Review convertDataTableToReviewDTO(DataTable dt)
        {
            Review review = default(Review);

            foreach (DataRow dr in dt.Rows)
            {
                if (review == default(Review))
                {
                    review = new Review
                    {
                        ID = dr.GetOrdinal<int>("ReviewID"),
                        NodeID = dr.GetOrdinal<int>("NodeID"),
                        UserID = dr.GetOrdinal<int>("UserID"),
                        Grade = dr.GetOrdinal<String>("Grade"),
                        Comment = dr.GetOrdinal<String>("Comment"),
                        Age = dr.GetOrdinal<int>("Age"),
                        Rating = Convert.ToDecimal(dr.GetOrdinal<double>("Rating")),
                        RoleName = dr.GetOrdinal<String>("RoleName"),
                        CreateDate = dr.GetOrdinal<DateTime>("CreateDate"),
                        SpecialPopulationIDs = new List<int>()

                    };

                }
                review.SpecialPopulationIDs.Add(dr.GetOrdinal<int>("SpecialPopulationIDs"));
             }
            return review;
        }
    }
}
