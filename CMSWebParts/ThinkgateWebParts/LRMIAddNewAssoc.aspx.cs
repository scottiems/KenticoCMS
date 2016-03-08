using System.Configuration;
using System.Linq;
using CMS.CMSHelper;
using CMS.DocumentEngine;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using System;
using System.Data;
using CMS.SiteProvider;
using CMSWebParts.ThinkgateWebParts;
using CultureInfo = System.Globalization.CultureInfo;

public partial class CMSWebParts_ThinkgateWebParts_LRMIAddNewAssoc : System.Web.UI.Page
{
    private bool _createAllowed;
    private bool _modifyAllowed;
    private const string DocumentTableClassName = "cms.document";
    private const string UserTableClassName = "cms.user";
    private const string CustomTableClassLrmiDetail = "thinkgate.LRMIEducationalAlignment";
    private static readonly string CmsConnectionString = ConfigurationManager.ConnectionStrings["CMSConnectionString"].ConnectionString;

    private int _lrmiItemId;

	private static string DocumentNodeId
	{
		get
		{
			return QueryHelper.GetString("parentnodeid", "");
		}
	}

	protected void Page_Load(object sender, EventArgs e)
	{
        TreeProvider tree = new TreeProvider(CMSContext.CurrentUser);
        LRMITag.ObjectId = int.Parse(DocumentNodeId);
        if (int.Parse(DocumentNodeId) > 0)
        {
            TreeNode treenode = tree.SelectSingleNode(int.Parse(DocumentNodeId));

            _createAllowed = treenode.CheckPermissions(PermissionsEnum.Create, CMSContext.CurrentSiteName, CMSContext.CurrentUser);
            _modifyAllowed = treenode.CheckPermissions(PermissionsEnum.Modify, CMSContext.CurrentSiteName, CMSContext.CurrentUser);
            ApplyPermissions();
        }

        if (!IsPostBack)
        {
            LoadLrmiTag(int.Parse(DocumentNodeId));    // Load LRMI Tags in Edit Mode
        }
        LRMITag.SaveCancelButtonClick += new EventHandler(User_Clicked_Button);
	}
    private void ApplyPermissions()
    {
         LRMITag.SaveButtonVisible = _modifyAllowed;
    }
    private void LoadLrmiTag(int documentId)
    {
        // Bind LRMI Tag Data For Specific Document ID
        _lrmiItemId = Convert.ToInt32(int.Parse(DocumentNodeId));
        // Bind LRMI Tag Data For Specific Document ID
        DataTable dtDocument = GetDocumentDetails(documentId);
        BindDocumentDetails(dtDocument);
        // Bind LRMI Tag Details Data
        DataTable dtLrmiDetails = GetLrmiTagDetails(_lrmiItemId);
        BindEducationalAlignment(dtLrmiDetails);       
    }
   private DataTable GetLrmiTagDetails(int lrmiItemId)
    {
        DataSet dsLrmiTagDetails = new DataSet();

        // Creates a new Custom table item provider
        CustomTableItemProvider customTableProvider = new CustomTableItemProvider(CMSContext.CurrentUser);

        // Creates a new Data Class Info
        DataClassInfo customTable = DataClassInfoProvider.GetDataClass(CustomTableClassLrmiDetail);

        // Checks if Custom table 'Sample table' exists
        if (customTable != null)
        {
            // Prepares the parameters
            string where = "LRMIItemID = '" + lrmiItemId + "'";
            const int topN = 0;
            const string orderBy = "";
            const string columns = "ItemID, EducationalAlignmentEnum, EducationalAlignmentValues, LRMIItemID";

            // Gets the data set according to the parameters
            dsLrmiTagDetails = customTableProvider.GetItems(customTable.ClassName, where, orderBy, topN, columns);
        }

       return dsLrmiTagDetails.Tables[0];
    }
    private void BindEducationalAlignment(DataTable dtLrmiDetails)
    {
        if (dtLrmiDetails.Rows.Count > 0)
        {
            foreach (DataRow row in dtLrmiDetails.Rows)
            {
                string educationalAlignmentValues = row["EducationalAlignmentValues"].ToString();
                string[] listValues;

                switch ((int)row["EducationalAlignmentEnum"])
                {
                    case (int)LookupDetails.Assessed :
                        listValues = educationalAlignmentValues.Split('|');
                        foreach (var listValue in listValues)
                        {
                            LRMITag.TagCriteriaSelectionParameters.AssessedStandardIds.Add(Convert.ToInt32(listValue));    
                        }
                        break;
                    case (int)LookupDetails.Teaches:
                        listValues = educationalAlignmentValues.Split('|');
                        foreach (var listValue in listValues)
                        {
                            LRMITag.TagCriteriaSelectionParameters.TeachesStandardIds.Add(Convert.ToInt32(listValue));
                        }
                        break;
                    case (int)LookupDetails.Requires:
                        listValues = educationalAlignmentValues.Split('|');
                        foreach (var listValue in listValues)
                        {
                            LRMITag.TagCriteriaSelectionParameters.RequiresStandardIds.Add(Convert.ToInt32(listValue));
                        }
                        break;
                    case (int)LookupDetails.EducationalSubject:
                        listValues = educationalAlignmentValues.Split('|');
                        foreach (var listValue in listValues)
                        {
                            LRMITag.TagCriteriaSelectionParameters.EducationalSubject.Add(listValue);
                        }
                        break;
                    case (int)LookupDetails.EducationalLevel:
                        listValues = educationalAlignmentValues.Split('|');
                        foreach (var listValue in listValues)
                        {
                            LRMITag.TagCriteriaSelectionParameters.GradeLevel.Add(listValue);
                        }
                        break;
                    case (int)LookupDetails.LearningResourceType:
                        listValues = educationalAlignmentValues.Split('|');
                        foreach (var listValue in listValues)
                        {
                            LRMITag.TagCriteriaSelectionParameters.LearningResourceType.Add(Convert.ToInt32(listValue));
                        }
                        break;
                    case (int)LookupDetails.EducationalUse:
                        listValues = educationalAlignmentValues.Split('|');
                        foreach (var listValue in listValues)
                        {
                            LRMITag.TagCriteriaSelectionParameters.EducationalUse.Add(Convert.ToInt32(listValue));
                        }
                        break;
                    case (int)LookupDetails.EndUser:
                        listValues = educationalAlignmentValues.Split('|');
                        foreach (var listValue in listValues)
                        {
                            LRMITag.TagCriteriaSelectionParameters.EndUser.Add(Convert.ToInt32(listValue));
                        }
                        break;
                    case (int)LookupDetails.MediaType:
                        listValues = educationalAlignmentValues.Split('|');
                        foreach (var listValue in listValues)
                        {
                            LRMITag.TagCriteriaSelectionParameters.MediaType.Add(Convert.ToInt32(listValue));
                        }
                        break;
                    case (int)LookupDetails.Activity:
                        listValues = educationalAlignmentValues.Split('|');
                        foreach (var listValue in listValues)
                        {
                            LRMITag.TagCriteriaSelectionParameters.InteractivityType.Add(Convert.ToInt32(listValue));
                        }
                        break;
                    case (int)LookupDetails.ReadingLevel:
                        LRMITag.TagCriteriaSelectionParameters.ReadingLevel = educationalAlignmentValues;
                        break;
                    case (int)LookupDetails.TextComplexity:
                        LRMITag.TagCriteriaSelectionParameters.TextComplexity = educationalAlignmentValues;
                        break;
                    case (int)LookupDetails.AgeAppropriate:
                        LRMITag.TagCriteriaSelectionParameters.AgeAppropriate = Convert.ToInt32(educationalAlignmentValues);
                        break;
                    case (int)LookupDetails.UsageRights:
                        LRMITag.TagCriteriaSelectionParameters.UseRightUrl = Convert.ToInt32(educationalAlignmentValues);
                        break;
                    case (int)LookupDetails.UsageRightsUrl:
                        LRMITag.TagCriteriaSelectionParameters.UseRightUrlTxt = educationalAlignmentValues;
                        break;
                    case (int)LookupDetails.Original3RdParty:
                        LRMITag.TagCriteriaSelectionParameters.OriginalThirdPartyUrl = educationalAlignmentValues;
                        break;
                    case (int)LookupDetails.Creator:
                        LRMITag.TagCriteriaSelectionParameters.Creator = educationalAlignmentValues;
                        break;
                    case (int)LookupDetails.Publisher:
                        LRMITag.TagCriteriaSelectionParameters.Publisher = educationalAlignmentValues;
                        break;
                    case (int)LookupDetails.Language:
                        LRMITag.TagCriteriaSelectionParameters.Language = Convert.ToInt32(educationalAlignmentValues);
                        break;
                    case (int)LookupDetails.DateCreated:
                        LRMITag.TagCriteriaSelectionParameters.DateCreated = Convert.ToDateTime(educationalAlignmentValues);
                        break;
                    case (int)LookupDetails.TimeRequired:
                        listValues = educationalAlignmentValues.Split(':');
                        LRMITag.TagCriteriaSelectionParameters.TimeRequiredDays = Convert.ToInt32(listValues[0]);
                        LRMITag.TagCriteriaSelectionParameters.TimeRequiredHours = Convert.ToInt32(listValues[1]);
                        LRMITag.TagCriteriaSelectionParameters.TimeRequiredMinutes = Convert.ToInt32(listValues[2]);
                        break;
                }
            }
        }
    }
    private DataTable GetDocumentDetails(int documentId)
    {
        DataSet dsDocumentDetail = new DataSet();

        // Creates a new Custom table item provider
        CustomTableItemProvider customTableProvider = new CustomTableItemProvider(CMSContext.CurrentUser);

        // Creates a new Data Class Info
        DataClassInfo customTable = DataClassInfoProvider.GetDataClass(DocumentTableClassName);

        // Checks if Custom table 'Sample table' exists
        if (customTable != null)
        {
            // Prepares the parameters
            string where = "DocumentNodeID = '" + documentId + "'";
            const int topN = 0;
            const string orderBy = "";
            const string columns = "DocumentPageTitle, DocumentName, DocumentCreatedWhen, DocumentCreatedByUserID, DocumentCulture, DocumentType";

            // Gets the data set according to the parameters
            dsDocumentDetail = customTableProvider.GetItems(customTable.ClassName, where, orderBy, topN, columns);
        }

        return dsDocumentDetail.Tables[0];
    }
    private void BindDocumentDetails(DataTable dtDocument)
    {
        if (dtDocument.Rows.Count > 0)
        {
            LRMITag.TagCriteriaSelectionParameters.Creator = GetDocumentCreatedBy((int)dtDocument.Rows[0]["DocumentCreatedByUserID"]);
            LRMITag.TagCriteriaSelectionParameters.DateCreated = Convert.ToDateTime(string.Format("{0:MM/dd/yyyy}", dtDocument.Rows[0]["DocumentCreatedWhen"]));
          }
    }
    private string GetDocumentCreatedBy(int userId)
    {
        DataSet dsCreatedBy = new DataSet();

        // Creates a new Custom table item provider
        CustomTableItemProvider customTableProvider = new CustomTableItemProvider(CMSContext.CurrentUser);

        // Creates a new Data Class Info
        DataClassInfo customTable = DataClassInfoProvider.GetDataClass(UserTableClassName);

        // Checks if Custom table 'Sample table' exists
        if (customTable != null)
        {
            // Prepares the parameters
            string where = "UserID = '" + userId + "'";
            const int topN = 0;
            const string orderBy = "";
            const string columns = "FirstName, MiddleName, LastName, FullName";

            // Gets the data set according to the parameters
            dsCreatedBy = customTableProvider.GetItems(customTable.ClassName, where, orderBy, topN, columns);
        }

        if (dsCreatedBy.Tables.Count > 0 && dsCreatedBy.Tables[0].Rows.Count > 0)
        {
            return dsCreatedBy.Tables[0].Rows[0]["FullName"].ToString();
        }

        return string.Empty;
    }
    protected void User_Clicked_Button(object sender, EventArgs e)
    {
        string script = "closeDialog();";
        TagCriteriaSelectionParameters tcsp = LRMITag.TagCriteriaSelectionParameters;
        if (!TagCriteriaSelectionParameters.IsEmpty(tcsp))
        {
            DeleteLrmiTag();
            InsertLrmiTag();

            script = "showmessage();";
         }
        ScriptHelper.RegisterStartupScript(Page, typeof(string), "showsavemessage", ScriptHelper.GetScript(script));
    }
    private void DeleteLrmiTag()
    {
        // Creates Custom Table Provider
        CustomTableItemProvider customTableProvider = new CustomTableItemProvider(CMSContext.CurrentUser);

        // Creates Custom Table 
        DataClassInfo customTable = DataClassInfoProvider.GetDataClass(CustomTableClassLrmiDetail);

        // Checks if Custom table exists
        if (customTable != null)
        {
            // Prepares the parameters
            string where = "LRMIItemID = '" + DocumentNodeId + "'";
            const int topN = 0;
            const string orderBy = "";
            const string columns = "ItemID,EducationalAlignmentValues,EducationalAlignmentEnum";

            // Gets the data set according to the parameters
            DataSet ds = customTableProvider.GetItems(customTable.ClassName, where, orderBy, topN, columns);

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                // Create Custom Table Item
                int itemId = int.Parse(row["ItemID"].ToString());
                int itemEnumId = (int)row["EducationalAlignmentEnum"];
                string rowStandards = row["EducationalAlignmentValues"].ToString();
                if ((itemEnumId == (int)LookupDetails.Assessed || itemEnumId == (int)LookupDetails.Requires || itemEnumId == (int)LookupDetails.Teaches) && !string.IsNullOrEmpty(rowStandards))
                {
                    string[] standards = rowStandards.Split('|');
                    foreach (string st in standards)
                    {
                        ThinkgateKenticoHelper.deleteStandard(Convert.ToInt32(DocumentNodeId), Convert.ToInt32(st), false);
                    }
                }

                CustomTableItem customTableItem = customTableProvider.GetItem(itemId, customTable.ClassName);
                customTableItem.Delete();
            }
        }
    }
    private void InsertLrmiTag()
    {
        if (LRMITag.TagCriteriaSelectionParameters.AssessedStandardIds.Count > 0)
        {
            string assessedStandardSets = string.Join("|",
                LRMITag.TagCriteriaSelectionParameters.AssessedStandardIds.Distinct()
                    .Select(x => x.ToString(CultureInfo.InvariantCulture))
                    .ToArray());
            AddSelectedItems(DocumentNodeId, assessedStandardSets);
            InsertLrmiTagDetails((int) LookupDetails.Assessed, assessedStandardSets);
        }
        if (LRMITag.TagCriteriaSelectionParameters.RequiresStandardIds.Count > 0)
        {
            string requiresStandardSets = string.Join("|",
                LRMITag.TagCriteriaSelectionParameters.RequiresStandardIds.Distinct()
                    .Select(x => x.ToString(CultureInfo.InvariantCulture))
                    .ToArray());
            AddSelectedItems(DocumentNodeId, requiresStandardSets);
            InsertLrmiTagDetails((int) LookupDetails.Requires, requiresStandardSets);
        }
        if (LRMITag.TagCriteriaSelectionParameters.TeachesStandardIds.Count > 0)
        {
            string teachesStandardSets = string.Join("|",
                LRMITag.TagCriteriaSelectionParameters.TeachesStandardIds.Distinct()
                    .Select(x => x.ToString(CultureInfo.InvariantCulture))
                    .ToArray());
            AddSelectedItems(DocumentNodeId, teachesStandardSets);
            InsertLrmiTagDetails((int) LookupDetails.Teaches, teachesStandardSets);
        }
        if (LRMITag.TagCriteriaSelectionParameters.EducationalSubject.Count > 0)
            InsertLrmiTagDetails((int)LookupDetails.EducationalSubject, string.Join("|", LRMITag.TagCriteriaSelectionParameters.EducationalSubject.Distinct().Select(x => x.ToString(CultureInfo.InvariantCulture)).ToArray()));
        if (LRMITag.TagCriteriaSelectionParameters.GradeLevel.Count > 0)
            InsertLrmiTagDetails((int)LookupDetails.EducationalLevel, string.Join("|", LRMITag.TagCriteriaSelectionParameters.GradeLevel.Distinct().Select(x => x.ToString(CultureInfo.InvariantCulture)).ToArray()));
        if (LRMITag.TagCriteriaSelectionParameters.MediaType.Count > 0)
            InsertLrmiTagDetails((int)LookupDetails.MediaType, string.Join("|", LRMITag.TagCriteriaSelectionParameters.MediaType.Distinct().Select(x => x.ToString(CultureInfo.InvariantCulture)).ToArray()));
        if (LRMITag.TagCriteriaSelectionParameters.LearningResourceType.Count > 0)
            InsertLrmiTagDetails((int)LookupDetails.LearningResourceType, string.Join("|", LRMITag.TagCriteriaSelectionParameters.LearningResourceType.Distinct().Select(x => x.ToString(CultureInfo.InvariantCulture)).ToArray()));
        if (LRMITag.TagCriteriaSelectionParameters.EducationalUse.Count > 0)
            InsertLrmiTagDetails((int)LookupDetails.EducationalUse, string.Join("|", LRMITag.TagCriteriaSelectionParameters.EducationalUse.Distinct().Select(x => x.ToString(CultureInfo.InvariantCulture)).ToArray()));
        if (LRMITag.TagCriteriaSelectionParameters.EndUser.Count > 0)
            InsertLrmiTagDetails((int)LookupDetails.EndUser, string.Join("|", LRMITag.TagCriteriaSelectionParameters.EndUser.Distinct().Select(x => x.ToString(CultureInfo.InvariantCulture)).ToArray()));
        if (LRMITag.TagCriteriaSelectionParameters.InteractivityType.Count > 0)
            InsertLrmiTagDetails((int)LookupDetails.Activity, string.Join("|", LRMITag.TagCriteriaSelectionParameters.InteractivityType.Distinct().Select(x => x.ToString(CultureInfo.InvariantCulture)).ToArray()));

        if (LRMITag.TagCriteriaSelectionParameters.TimeRequiredDays > 0 ||
            LRMITag.TagCriteriaSelectionParameters.TimeRequiredHours > 0 ||
            LRMITag.TagCriteriaSelectionParameters.TimeRequiredMinutes > 0)
            InsertLrmiTagDetails((int)LookupDetails.TimeRequired, LRMITag.TagCriteriaSelectionParameters.TimeRequiredDays + ":" + LRMITag.TagCriteriaSelectionParameters.TimeRequiredHours + ":" + LRMITag.TagCriteriaSelectionParameters.TimeRequiredMinutes);
        if (LRMITag.TagCriteriaSelectionParameters.ReadingLevel.Length > 0)
            InsertLrmiTagDetails((int)LookupDetails.ReadingLevel, LRMITag.TagCriteriaSelectionParameters.ReadingLevel);
        if (LRMITag.TagCriteriaSelectionParameters.TextComplexity.Length > 0)
            InsertLrmiTagDetails((int)LookupDetails.TextComplexity, LRMITag.TagCriteriaSelectionParameters.TextComplexity);
        if (LRMITag.TagCriteriaSelectionParameters.AgeAppropriate > 0)
            InsertLrmiTagDetails((int)LookupDetails.AgeAppropriate, LRMITag.TagCriteriaSelectionParameters.AgeAppropriate.ToString());
        if (LRMITag.TagCriteriaSelectionParameters.UseRightUrl > 0)
            InsertLrmiTagDetails((int)LookupDetails.UsageRights, LRMITag.TagCriteriaSelectionParameters.UseRightUrl.ToString());
        if (LRMITag.TagCriteriaSelectionParameters.OriginalThirdPartyUrl.Length > 0)
            InsertLrmiTagDetails((int)LookupDetails.Original3RdParty, LRMITag.TagCriteriaSelectionParameters.OriginalThirdPartyUrl);
        if (LRMITag.TagCriteriaSelectionParameters.Creator.Length > 0)
            InsertLrmiTagDetails((int)LookupDetails.Creator, LRMITag.TagCriteriaSelectionParameters.Creator);
        if (LRMITag.TagCriteriaSelectionParameters.UseRightUrlTxt.Length > 0)
            InsertLrmiTagDetails((int)LookupDetails.UsageRightsUrl, LRMITag.TagCriteriaSelectionParameters.UseRightUrlTxt);
            InsertLrmiTagDetails((int)LookupDetails.DateCreated, LRMITag.TagCriteriaSelectionParameters.DateCreated.ToString());
            if (LRMITag.TagCriteriaSelectionParameters.Publisher.Length > 0)
            InsertLrmiTagDetails((int)LookupDetails.Publisher, LRMITag.TagCriteriaSelectionParameters.Publisher);
            if (LRMITag.TagCriteriaSelectionParameters.Language > 0)
            InsertLrmiTagDetails((int)LookupDetails.Language, LRMITag.TagCriteriaSelectionParameters.Language.ToString());

    }
    private void InsertLrmiTagDetails(int educationalAlignmentEnum, string educationalAlignmentValue)
    {
        // Creates Custom Table Provider
        CustomTableItemProvider customTableProvider = new CustomTableItemProvider(CMSContext.CurrentUser);

        // Creates Custom Table 
        DataClassInfo customTable = DataClassInfoProvider.GetDataClass(CustomTableClassLrmiDetail);

        // Checks if Custom table exists
        if (customTable != null)
        {
            // Create Custom Table Item
            CustomTableItem customTableItem = CustomTableItem.New(customTable.ClassName, customTableProvider);

            // Sets the row data
            customTableItem.SetValue("EducationalAlignmentEnum", educationalAlignmentEnum);
            customTableItem.SetValue("EducationalAlignmentValues", educationalAlignmentValue);
            customTableItem.SetValue("LRMIItemID", Convert.ToInt32(DocumentNodeId));

            customTableItem.Insert();
        }
    }
    public static void AddSelectedItems(string docid, string selectedItems)
    {
        string[] selitems = selectedItems.Split('|');

        for (var i = 0; i < selitems.Length; i++)
        {
            int nodeid;
            string docEntry = selitems[i];
            if (Int32.TryParse(docEntry, out nodeid))
            {

                string retQuery = string.Format("select count(distinct standardID) count from [dbo].[thinkgate_docToStandards] where docID='{0}' AND standardID = '{1}' ", docid, nodeid);
                DataTable dt = ThinkgateKenticoHelper.GetDataTable(CmsConnectionString, retQuery);
                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        if (Convert.ToInt32(dt.Rows[0]["count"]) == 0)
                        {
                            string query = string.Format("Insert into [dbo].[thinkgate_docToStandards] (docID, standardID) values ('{0}', {1})", docid, nodeid);
                            ThinkgateKenticoHelper.GetDataTable(CmsConnectionString, query);
                        }
                    }
                }
                else//May Not required.
                {
                    string query = string.Format("Insert into [dbo].[thinkgate_docToStandards] (docID, standardID) values ('{0}', {1})", docid, nodeid);
                    ThinkgateKenticoHelper.GetDataTable(CmsConnectionString, query);
                }
            }
        }
    }
}