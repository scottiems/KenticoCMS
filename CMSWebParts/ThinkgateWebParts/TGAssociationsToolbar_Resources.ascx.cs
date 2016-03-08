using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text;

public partial class CMSWebParts_ThinkgateWebParts_TGAssociationsToolbar_Resources : TGAssociationToobarBase
{

	public int AssociationCount;

	protected void Page_Load(object sender, EventArgs e)
	{
	    AssociationCount = DocumentID != null ? GetCurrentAssociationCount() : 0;
	}

    private int GetCurrentAssociationCount()
	{
		int result;
		string swrk = "";
        string sql = "select count (Distinct res.ResourceID) as 'COUNT' from [dbo].[thinkgate_docToResources] DR Join [dbo].[thinkgate_resource] res ON DR.ResourceID = res.ResourceID where (CONVERT(date,res.ExpirationDate) >= CONVERT(date,CURRENT_TIMESTAMP) OR res.ExpirationDate IS NULL) AND (DR.docID = '" + DocumentID + "') ";
		DataTable dt = GetDataTable(CMSConnectionString, sql);
		if (dt.Rows.Count > 0)
		{
			StringBuilder sb = new StringBuilder();
			foreach (DataRow dr in dt.Rows)
			{
				string rowItem = dr["COUNT"].ToString();

				sb.Append(rowItem);

			}
			swrk = sb.ToString();
		}
		else
		{
			swrk = string.Empty;
		}

		if (Int32.TryParse(swrk, out result))
		{
			return result;
		}
		else
		{
			return 0;
		}
	}

}