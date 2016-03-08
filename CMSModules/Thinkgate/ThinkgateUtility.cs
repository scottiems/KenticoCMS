using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

/// <summary>
/// Summary description for ThinkgateUtility
/// </summary>
public class ThinkgateUtility
{
	public ThinkgateUtility()
	{
		//
		// TODO: Add constructor logic here
		//
	}

    /// <summary>
    /// Copy Document Plan Association Details
    /// </summary>
    /// <param name="sourceDocumentID"></param>
    /// <param name="destinationDocumnetID"></param>
    /// <returns></returns>
    internal static int CopyDocumentPlanAssociationDetails(int sourceDocumentId, int destinationDocumentId)
    {
        int rowCount = 0;
        using (SqlConnection sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["CMSConnectionString"].ToString()))
        {
            sqlConnection.Open();
            using (SqlCommand sqlCommand = new SqlCommand("Thinkgate_DocumentPlanAssociationDetails_Copy", sqlConnection))
            {
                sqlCommand.Parameters.AddWithValue("@SourceDocumentId", sourceDocumentId);
                sqlCommand.Parameters.AddWithValue("@DestinationDocumentId", destinationDocumentId);
                sqlCommand.CommandType = CommandType.StoredProcedure;

                rowCount = sqlCommand.ExecuteNonQuery();
            }
        }
        return rowCount;
    }

}