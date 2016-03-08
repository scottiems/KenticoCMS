using System.Data;
using System.Data.SqlClient;
using System.Text;
using Classes.Rating;
using SQLDataAcess;

namespace DataAccess
{
    /// <summary>
    /// Summary description for AssociationToolBarDataAccess
    /// </summary>
    public class AssociationToolBarDataAccess
    {
        private readonly DataBaseAccess _dataBaseAccess;

        private const string E3_GET_STANDARDSLIST_FOR_ASSOCIATIONS = "E3_GetStandardsForStandardAssociation";

        public AssociationToolBarDataAccess()
        {
            _dataBaseAccess = new DataBaseAccess();
        }
        public DataTable GetStandardsListFromClientDB(string grade, string standardset, string subject, string course, string searchText, string searchOption)
        {

            SqlParameterCollection sqlParameterCollection = new SqlCommand().Parameters;

            SqlParameter sqlParameter = new SqlParameter("@Grade", SqlDbType.VarChar) { Value = grade };
            sqlParameterCollection.Add(sqlParameter);

            sqlParameter = new SqlParameter("@StandardSet", SqlDbType.VarChar) { Value = standardset };
            sqlParameterCollection.Add(sqlParameter);

            sqlParameter = new SqlParameter("@Subject", SqlDbType.VarChar) { Value = subject };
            sqlParameterCollection.Add(sqlParameter);

            sqlParameter = new SqlParameter("@Course", SqlDbType.VarChar) { Value = course };
            sqlParameterCollection.Add(sqlParameter);

            sqlParameter = new SqlParameter("@Text", SqlDbType.VarChar) { Value = searchText };
            sqlParameterCollection.Add(sqlParameter);

            sqlParameter = new SqlParameter("@Option", SqlDbType.VarChar) { Value = searchOption };
            sqlParameterCollection.Add(sqlParameter);

            DataTable dt = _dataBaseAccess.ExecuteStoreProcedureInClientDB(E3_GET_STANDARDSLIST_FOR_ASSOCIATIONS, sqlParameterCollection);
            return dt;
        }
    }
}