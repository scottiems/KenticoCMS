using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text;

public partial class CMSWebParts_ThinkgateWebParts_TGAssociationsToolbar_Standards : TGAssociationToobarBase
{

	public int AssociationCount;

	protected void Page_Load(object sender, EventArgs e)
	{
		if (DocumentID != null)
		{
			AssociationCount = GetCurrentAssociationCount();
		}
		else
		{
			AssociationCount = 0;
		}
	}

	private int GetCurrentAssociationCount()
	{
		int result;
		string swrk = "";
		string sql = "select count (*) as 'COUNT' from [dbo].[thinkgate_docToStandards] where (docID = '" + this.DocumentID.ToString() + "') ";
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