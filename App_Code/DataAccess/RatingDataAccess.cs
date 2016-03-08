using System.Data;
using System.Data.SqlClient;
using System.Text;
using Classes.Rating;
using SQLDataAcess;

namespace DataAccess
{
    /// <summary>
    /// Summary description for RatingDataAccess
    /// </summary>
    public class RatingDataAccess
    {
        private readonly DataBaseAccess _dataBaseAccess;

        private const string E3_GET_USER_FULLNAME_FROM_KENTICO_USER_NAME = "E3_GetFullNameUsingKenticoUserName";
        private const string E3_GET_SPECIFIC_RATING_RANGE_VALUES = "E3_GetSpecificRatingRange";
        private const string E3_GET_RATING_RANGE = "E3_QuestionRatingFilterValues_GetAll";
        private const string E3_SPECIAL_POPULATION_GET_ALL = "E3_SpecialPopulation_GetAll";
        private const string E3_AGES_GET_ALL = "E3_Ages_GetAll";
        private const string E3_GRADES_GET_ALL = "E3_Grades_GetAll";
        private const string THINKGATE_GET_REVIEW_FOR_NODE_PER_USER = "Thinkgate_GetReviewForNodePerUser";
        private const string THINKGATE_SAVE_REVIEW_FOR_NODE_PER_USER = "Thinkgate_SaveReviewForNodePerUser";
        private const string THINKGATE_UPDATE_AVGRATING_AND_COUNT_FOR_NODE = "Thinkgate_UpdateRatingAverageAndCountInDocTypeTable";
        private const string THINKGATE_GET_ALL_REVIEW_RATING_SUMMARY = "Thinkgate_GetAllReviewRatingSummary";
        private const string THINKGATE_GET_REVIEWS_BY_RATINGVALUE = "Thinkgate_GetReviewsByRatingValue";
        private const string THINKGATE_DELETE_REVIEW_AND_UPDATE_AVERAGE = "Thinkgate_DeleteReviewAndUpdateAverage";
        
        

        public RatingDataAccess()
        {
            _dataBaseAccess = new DataBaseAccess();
        }

        public DataTable GetAllAgesFromClientDB()
        {

            DataTable dt = _dataBaseAccess.ExecuteStoreProcedureInClientDB(E3_AGES_GET_ALL);
            return dt;
        }

        public DataTable GetAllGradesFromClientDB()
        {
            DataTable dt = _dataBaseAccess.ExecuteStoreProcedureInClientDB(E3_GRADES_GET_ALL);
            return dt;
        }

        public DataTable GetActiveSpecialPopulationFromClientDB(string clientId = null)
        {
            SqlParameterCollection sqlParameterCollection = new SqlCommand().Parameters; 

            SqlParameter sqlParameter = new SqlParameter("@IsActive", SqlDbType.Bit) {Value = true};
            sqlParameterCollection.Add(sqlParameter);

            DataTable dt = _dataBaseAccess.ExecuteStoreProcedureInClientDB(E3_SPECIAL_POPULATION_GET_ALL, sqlParameterCollection, clientId);
            return dt;
        }

        public DataTable GetExistingReviewFromKenticoDB(int nodeid, int userid)
        {

            SqlParameterCollection sqlParameterCollection = new SqlCommand().Parameters;

            SqlParameter sqlParameter = new SqlParameter("@nodeid", SqlDbType.Int) {Value = nodeid};
            sqlParameterCollection.Add(sqlParameter);

            sqlParameter = new SqlParameter("@userid", SqlDbType.Int) {Value = userid};
            sqlParameterCollection.Add(sqlParameter);

            DataTable dt = _dataBaseAccess.ExecuteStoreProcedureInKenticoDB(THINKGATE_GET_REVIEW_FOR_NODE_PER_USER, sqlParameterCollection);
            return dt;
        }

        public void SaveReviewInKenticoDB(Review review)
        {

            SqlParameterCollection sqlParameterCollection = new SqlCommand().Parameters;

            SqlParameter sqlParameter = new SqlParameter("@nodeid", SqlDbType.Int) {Value = review.NodeID};
            sqlParameterCollection.Add(sqlParameter);

            sqlParameter = new SqlParameter("@userid", SqlDbType.Int) {Value = review.UserID};
            sqlParameterCollection.Add(sqlParameter);


            sqlParameter = new SqlParameter("@age", SqlDbType.Int) {Value = review.Age};
            sqlParameterCollection.Add(sqlParameter);

            sqlParameter = new SqlParameter("@grade", SqlDbType.NVarChar) {Value = review.Grade};
            sqlParameterCollection.Add(sqlParameter);

            sqlParameter = new SqlParameter("@comment", SqlDbType.NVarChar) {Value = review.Comment};
            sqlParameterCollection.Add(sqlParameter);
            
            sqlParameter = new SqlParameter("@rolename", SqlDbType.NVarChar) {Value = review.RoleName};
            sqlParameterCollection.Add(sqlParameter);

            var specialPopulations = new dtGeneric_Int();
            foreach (int val in review.SpecialPopulationIDs)
            {
                specialPopulations.Add(val);
            }
            sqlParameter = new SqlParameter("@specialPopulations", SqlDbType.Structured)
            {
                Value = specialPopulations.ToSql().Data,
                TypeName = specialPopulations.ToSql().TypeName
            };
            sqlParameterCollection.Add(sqlParameter);

            sqlParameter = new SqlParameter("@rating", SqlDbType.Float) { Value = review.Rating };
            sqlParameterCollection.Add(sqlParameter);

            _dataBaseAccess.ExecuteStoreProcedureInKenticoDB(THINKGATE_SAVE_REVIEW_FOR_NODE_PER_USER, sqlParameterCollection);
        }

        public void UpdateRatingAvgNCountsInKenticoDB(int nodeId)
        {

            SqlParameterCollection sqlParameterCollection = new SqlCommand().Parameters;

            SqlParameter sqlParameter = new SqlParameter("@nodeid", SqlDbType.Int) {Value = nodeId};
            sqlParameterCollection.Add(sqlParameter);

            _dataBaseAccess.ExecuteStoreProcedureInKenticoDB(THINKGATE_UPDATE_AVGRATING_AND_COUNT_FOR_NODE,
                sqlParameterCollection);
        }

        public void DeleteReviewInKenticoDB(int reviewId)
        {

            SqlParameterCollection sqlParameterCollection = new SqlCommand().Parameters;

            SqlParameter sqlParameter = new SqlParameter("@reviewId", SqlDbType.Int) { Value = reviewId };
            sqlParameterCollection.Add(sqlParameter);

            _dataBaseAccess.ExecuteStoreProcedureInKenticoDB(THINKGATE_DELETE_REVIEW_AND_UPDATE_AVERAGE,
                sqlParameterCollection);
        }

        public DataTable GetReviewsByRatingValueFromKenticoDB(int nodeId, string ratingValue = null)
        {

            SqlParameterCollection sqlParameterCollection = new SqlCommand().Parameters;

            SqlParameter sqlParameter = new SqlParameter("@nodeid", SqlDbType.Int) { Value = nodeId };
            sqlParameterCollection.Add(sqlParameter);

            if (ratingValue != null)
            {
                sqlParameter = new SqlParameter("@ratingValue", SqlDbType.Float) {Value = ratingValue};
                sqlParameterCollection.Add(sqlParameter);
            }

            return _dataBaseAccess.ExecuteStoreProcedureInKenticoDB(THINKGATE_GET_REVIEWS_BY_RATINGVALUE,
                sqlParameterCollection);
        }

        public DataTable GetAllReviewRatingSummaryFromKenticoDB(int nodeId)
        {

            SqlParameterCollection sqlParameterCollection = new SqlCommand().Parameters;

            SqlParameter sqlParameter = new SqlParameter("@nodeid", SqlDbType.Int) {Value = nodeId};
            sqlParameterCollection.Add(sqlParameter);

            return _dataBaseAccess.ExecuteStoreProcedureInKenticoDB(THINKGATE_GET_ALL_REVIEW_RATING_SUMMARY,
                sqlParameterCollection);

        }


        public DataTable GetRatingRangeFromClientDB()
        {

            return _dataBaseAccess.ExecuteStoreProcedureInClientDB(E3_GET_RATING_RANGE);
        }
        public DataTable GetSpecificRatingRangeFromClientDB(int ratingRangeID)
        {

            SqlParameterCollection sqlParameterCollection = new SqlCommand().Parameters;

            SqlParameter sqlParameter = new SqlParameter("@ID", SqlDbType.Int) { Value = ratingRangeID };
            sqlParameterCollection.Add(sqlParameter);

            return _dataBaseAccess.ExecuteStoreProcedureInClientDB(E3_GET_SPECIFIC_RATING_RANGE_VALUES, sqlParameterCollection);
        }
        public DataTable GetFullNameFromKenticoUsernameFromSpecificClient(string kenticoUserName, string clientId)
        {

            SqlParameterCollection sqlParameterCollection = new SqlCommand().Parameters;

            SqlParameter sqlParameter = new SqlParameter("@KenticoUserName", SqlDbType.VarChar) { Value = kenticoUserName };
            sqlParameterCollection.Add(sqlParameter);

            return _dataBaseAccess.ExecuteStoreProcedureInClientDB(E3_GET_USER_FULLNAME_FROM_KENTICO_USER_NAME, sqlParameterCollection, clientId);
        }
        

    }
}