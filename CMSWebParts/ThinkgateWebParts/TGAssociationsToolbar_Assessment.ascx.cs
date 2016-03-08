using System;
using System.Configuration;
using System.Data;
using System.Text;

public partial class CMSWebParts_ThinkgateWebParts_TGAssociationsToolbar_Assessment : TGAssociationToobarBase
{
    public int AssessmentAssociatonCount;

    #region Page Events
    protected void Page_Load(object sender, EventArgs e)
    {
        if (DocumentID != null)
        { AssessmentAssociatonCount = GetAssessmentAssociationCount(); }
        else
        { AssessmentAssociatonCount = 0;}
    }
    #endregion

    #region Private Methods
    
    private int GetAssessmentAssociationCount()
    {
        string sql = "Select Count(distinct AssessmentID) as 'COUNT' from [dbo].[thinkgate_docToAssessments] where (docID = '" + DocumentID + "') ";
        DataTable dataTable = GetDataTable(CMSConnectionString, sql);

        if (dataTable.Rows.Count > 0)
        {
            int assessmentAssociationCount;
            if (Int32.TryParse(dataTable.Rows[0]["COUNT"].ToString(), out assessmentAssociationCount))
            { return assessmentAssociationCount; }
        }
        return 0;
    }
    
    #endregion
}