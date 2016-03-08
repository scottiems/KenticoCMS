using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Web.UI.WebControls;
using CMS.CMSHelper;
using CMS.DocumentEngine;
using CMS.GlobalHelper;
using CMS.PortalControls;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using Telerik.Web.UI;

public partial class CMSWebParts_ThinkgateWebParts_LRMITag : CMSAbstractWebPart
{
    #region Private Variables

    private string rootConnectionString = ConfigurationManager.ConnectionStrings["root_application"].ConnectionString;
    private string documentTableClassName = "cms.document";
    private string userTableClassName = "cms.user";
    private string customTableClassLRMI = "thinkgate.LRMI";
    private string customTableClassLRMIDetail = "thinkgate.LRMIEducationalAlignment";
    private List<KeyValuePair<int, string>> deletedStandards = new List<KeyValuePair<int, string>>();
    public int DocumentID { get; set; }
    public int DocumentNodeID { get; set; }
    public bool createAllowed = false;
    public bool modifyAllowed = false;
    public string ParentDocID
    {
        get
        {
            return QueryHelper.GetString("parentnodeid", "");
        } 
    }
    #endregion

    #region Enums

    private enum LookupType
    {
        None = 0,
        EducationalAlignment = 1,
        Instruction = 2,
        Activity = 3,
        TargetAudience = 4,
        LearningResourceType = 5,
    };

    #endregion    
    
    #region Event Methods

    protected void Page_Load(object sender, EventArgs e)
    {
        DocID.Value = this.DocumentID.ToString();

        TreeProvider tree = new TreeProvider(CMSContext.CurrentUser);
        if (this.DocumentNodeID > 0)
        {
            CMS.DocumentEngine.TreeNode treenode = tree.SelectSingleNode(this.DocumentNodeID);

            createAllowed = treenode.CheckPermissions(CMS.SettingsProvider.PermissionsEnum.Create, CMSContext.CurrentSiteName, CMSContext.CurrentUser);
            modifyAllowed = treenode.CheckPermissions(CMS.SettingsProvider.PermissionsEnum.Modify, CMSContext.CurrentSiteName, CMSContext.CurrentUser);
            ApplyPermissions();
        }

        if (!IsPostBack)
        {
            BindLookups();              // Bind Lookup Values
            //LoadLRMITag(DocumentID);    // Load LRMI Tags in Edit Mode
			LoadLRMITag(DocumentNodeID);    // Load LRMI Tags in Edit Mode
        }
    }

    private void ApplyPermissions()
    {
        lnkSelectTypes.Visible = modifyAllowed;
        RadButtonSave.Visible = modifyAllowed;
    }

    protected void lnkSelectTypes_Click(object sender, EventArgs e)
    {
        UpdatEducationalAlignments();
        BindGrid();

        SetControlsActive(false);
        pnlSelectEducationalAlignment.Visible = true;
        pnlEducationalAlignmentGrid.Visible = false;
    }

    protected void RadButtonOK_Click(object sender, EventArgs e)
    {
        SelectEducationalAlignment();
        BindGrid();
        
        SetControlsActive(true);
    }

    protected void RadButtonSave_Click(object sender, EventArgs e)
    {
        if (createAllowed || modifyAllowed)
        {
			SaveLRMITag(DocumentNodeID);
			//SaveLRMITag(DocumentID);
        }
    }

    protected void rgEducationalAlignment_ItemDataBound(object sender, GridItemEventArgs e)
    {
        if (e.Item is GridDataItem)
        {
            GridDataItem _item = (GridDataItem)e.Item;

            int educationalAlignmentEnum = int.Parse(_item["EducationalAlignmentEnum"].Text);
            string educationalAlignmentValue = _item["EducationalAlignmentValues"].Text;

            TextBox txtEducationalAlignmentValue = (TextBox)_item.FindControl("txtEducationalAlignmentValue");
            LinkButton lblSelectedStandards = (LinkButton)_item.FindControl("lblSelectedStandards");
            Label btnAddStandards = (Label)_item.FindControl("btnAddStandards");
            
            // Assign Text of the Educational Alignment
            int cblItemIndex = cblEducationalAlignment.Items.IndexOf(cblEducationalAlignment.Items.FindByValue(_item["EducationalAlignmentEnum"].Text));
            string educationalAlignmentText = cblEducationalAlignment.Items[cblItemIndex].Text;
            _item["EducationalAlignmentText"].Text = educationalAlignmentText + ":";

	        if ((educationalAlignmentEnum != (int) LookupDetails.EducationalLevel) &&
	            (educationalAlignmentEnum != (int) LookupDetails.EducationalSubject))
	        {
		        // Asign the educational alignment value into textbox
		        txtEducationalAlignmentValue.Text = educationalAlignmentValue == "&nbsp;" ? "" : educationalAlignmentValue;
	        }
	        else
	        {
		        // for the lack of time to refactor the radgrid/template just hiding the rows here...
				_item["EducationalAlignmentGridTemplateColumn"].Visible = false;
				_item["EducationalAlignmentText"].Style["display"] = "none";

				//Now show the dropdowns
				if (educationalAlignmentEnum == (int)LookupDetails.EducationalLevel)
				{
					FindControl("EducationalLevelRow").Visible = true;
					((Telerik.Web.UI.RadComboBox)FindControl("rcbEducationalLevel")).SelectedValue = educationalAlignmentValue;
				}

				if (educationalAlignmentEnum == (int)LookupDetails.EducationalSubject)
				{
					FindControl("EducationalSubjectRow").Visible = true;
					((Telerik.Web.UI.RadComboBox)FindControl("rcbEducationalSubject")).SelectedValue = educationalAlignmentValue;
				}		        
	        }

			// Add onclick event on the image button
            if (createAllowed || modifyAllowed)
            {
                btnAddStandards.Attributes.Add("onclick", "openModalDialog('" + lblSelectedStandards.ClientID + "', '" + txtEducationalAlignmentValue.ClientID + "'); return false;");
            }
            else
            {
                btnAddStandards.Style.Add("cursor", "default");
                ((System.Web.UI.HtmlControls.HtmlControl)btnAddStandards.FindControl("addStandardIconDiv")).Style.Add("cursor", "default");
                ((System.Web.UI.HtmlControls.HtmlControl)btnAddStandards.FindControl("addStandardIconDiv")).Attributes.Remove("title");
                ((System.Web.UI.HtmlControls.HtmlControl)btnAddStandards.FindControl("addStandardIcon")).Style.Add("cursor", "default");
            }

            // Show/ Hide RadTextbox and image ImageButton on condition basis
            if (educationalAlignmentText.Contains(LookupDetails.Assessed.ToString()) || educationalAlignmentText.Contains(LookupDetails.Teaches.ToString()) || educationalAlignmentText.Contains(LookupDetails.Requires.ToString()))
            {
                txtEducationalAlignmentValue.Style["display"] = "none";
                txtEducationalAlignmentValue.Width = Unit.Pixel(0);

                if (txtEducationalAlignmentValue.Text.Length > 0)
                {
                    string educationalAlignments = txtEducationalAlignmentValue.Text.Substring(0, txtEducationalAlignmentValue.Text.Length - 1).Replace('|', ',');

                    if (educationalAlignments != "")
                    {
                        string selectSQL = "select StandardName from Standards where ID in(" + educationalAlignments + ")";

                        DataTable dtStandardsName = GetDataTable(GetLocalDbConnectionString(this.rootConnectionString), selectSQL);
                        string standardsName = string.Empty;
                        foreach (DataRow _row in dtStandardsName.Rows)
                        {
                            standardsName += _row["StandardName"].ToString() + "<br>";
                        }

                        lblSelectedStandards.Text = standardsName.Trim();
                    }
                }
            }
            else
            {
                btnAddStandards.Style["display"] = "none";
                btnAddStandards.Width = Unit.Pixel(0);
            }
        }
    }

    #endregion

    #region Private Methods

    #region Binding Methods

    private void BindLookups()
    {
        string selectSQL = "select LD.Enum, LD.Description, L.Name AS LookupType from LookupDetails LD inner join LookupType L on L.Enum=LD.LookupEnum";
        DataTable dataTable = GetDataTable(GetLocalDbConnectionString(this.rootConnectionString), selectSQL);

        if (dataTable.Rows.Count > 0)
        {
            // Bind Educational Alignment Types
            cblEducationalAlignment.DataSource = new DataView(dataTable, "LookupType='" + LookupType.EducationalAlignment + "'", "Enum", DataViewRowState.CurrentRows);
            cblEducationalAlignment.DataTextField = "Description";
            cblEducationalAlignment.DataValueField = "Enum";
            cblEducationalAlignment.DataBind();

            // Bind Instruction
            rcbInstructionType.DataSource = new DataView(dataTable, "LookupType='" + LookupType.Instruction + "'", "Enum", DataViewRowState.CurrentRows);
            rcbInstructionType.DataTextField = "Description";
            rcbInstructionType.DataValueField = "Enum";
            rcbInstructionType.DataBind();

            // Bind Activity
            rcbActivityType.DataSource = new DataView(dataTable, "LookupType='" + LookupType.Activity + "'", "Enum", DataViewRowState.CurrentRows);
            rcbActivityType.DataTextField = "Description";
            rcbActivityType.DataValueField = "Enum";
            rcbActivityType.DataBind();

            // Bind Learning Resource Type
            rcbLearningResourceType.DataSource = new DataView(dataTable, "LookupType='" + LookupType.LearningResourceType + "'", "Enum", DataViewRowState.CurrentRows);
            rcbLearningResourceType.DataTextField = "Description";
            rcbLearningResourceType.DataValueField = "Enum";
            rcbLearningResourceType.DataBind();

            // Bind Target Audience
            rcbTargetAudience.DataSource = new DataView(dataTable, "LookupType='" + LookupType.TargetAudience + "'", "Enum", DataViewRowState.CurrentRows);
            rcbTargetAudience.DataTextField = "Description";
            rcbTargetAudience.DataValueField = "Enum";
            rcbTargetAudience.DataBind();
        }

	    BindEducationalSubject();
		BindEducationalLevel();
    }

	private void BindEducationalSubject()
	{
		// Bind Educational Subject	
		string selectSql = string.Format("SELECT DISTINCT Subject FROM CurrCourses AS C WHERE C.Active='Yes'");
		DataTable dataTable = GetDataTable(GetLocalDbConnectionString(this.rootConnectionString), selectSql);
		
		if (dataTable.Rows.Count <= 0) return;

		rcbEducationalSubject.DataSource = new DataView(dataTable, null, "Subject", DataViewRowState.CurrentRows);
		rcbEducationalSubject.DataTextField = "Subject";
		rcbEducationalSubject.DataValueField = "Subject";
		rcbEducationalSubject.DataBind();
	}

	private void BindEducationalLevel() //Grade
	{
		// Bind Educational Level (AKA Grade)	
		string selectSql = string.Format("SELECT DISTINCT Grade FROM CurrCourses C WHERE C.Active = 'Yes'  ORDER BY Grade ASC");
		DataTable dataTable = GetDataTable(GetLocalDbConnectionString(this.rootConnectionString), selectSql);

		if (dataTable.Rows.Count <= 0) return;

		rcbEducationalLevel.DataSource = new DataView(dataTable, null, "Grade", DataViewRowState.CurrentRows);
		rcbEducationalLevel.DataTextField = "Grade";
		rcbEducationalLevel.DataValueField = "Grade";
		rcbEducationalLevel.DataBind();
	}

	private void BindPageInEditMode(DataTable dtLRMI)
    {
        if (dtLRMI.Rows.Count > 0)
        {
            hdnLRMIItemId.Value = dtLRMI.Rows[0]["ItemID"].ToString();
            rcbInstructionType.SelectedValue = dtLRMI.Rows[0]["InstructionEnum"].ToString();
            rcbActivityType.SelectedValue = dtLRMI.Rows[0]["ActivityEnum"].ToString();
            rcbLearningResourceType.SelectedValue = dtLRMI.Rows[0]["LearningResourceTypeEnum"].ToString();
            rtbDuration.Text = dtLRMI.Rows[0]["DurationMinutes"].ToString();
            rcbTargetAudience.SelectedValue = dtLRMI.Rows[0]["TargetAudienceEnum"].ToString();
            if (dtLRMI.Rows[0]["AgeAppropriateCriteria"].ToString().Contains("-"))
            {
                rtbAgeAppropriateCriteriaMin.Text = dtLRMI.Rows[0]["AgeAppropriateCriteria"].ToString().Split('-')[0];
                rtbAgeAppropriateCriteriaMax.Text = dtLRMI.Rows[0]["AgeAppropriateCriteria"].ToString().Split('-')[1];
            }
           
        }
    }

    private void BindEducationalAlignment(DataTable dtLRMIDetails)
    {
        if (dtLRMIDetails.Rows.Count > 0)
        {
            foreach (DataRow _row in dtLRMIDetails.Rows)
            {
                int _cblItemIndex = cblEducationalAlignment.Items.IndexOf(cblEducationalAlignment.Items.FindByValue(_row["EducationalAlignmentEnum"].ToString()));
                cblEducationalAlignment.Items[_cblItemIndex].Selected = true;
            }
        }
    }

    private void BindDocumentDetails(DataTable dtDocument)
    {
        if (dtDocument.Rows.Count > 0)
        {
            lblTitle.Text = dtDocument.Rows[0]["DocumentPageTitle"].ToString();
            lblTopic.Text = dtDocument.Rows[0]["DocumentName"].ToString();
            lblCreated.Text = string.Format("{0:MM/dd/yyyy}", dtDocument.Rows[0]["DocumentCreatedWhen"]);
            lblCreator.Text = dtDocument.Rows[0]["DocumentCreatedByUserID"].ToString();
            lblCreator.Text = GetDocumentCreatedBy(ConvertNumeric(lblCreator.Text));
            //lblPublisher.Text = dtDocument.Rows[0][""].ToString();
            lblLanguage.Text = dtDocument.Rows[0]["DocumentCulture"].ToString();
            lblMediaType.Text = dtDocument.Rows[0]["DocumentType"].ToString();
        }
    }
        
    private void BindGrid()
    {
        DataTable dtEduAlignments = (DataTable)ViewState["dtEduAlignments"];
        rgEducationalAlignment.DataSource = dtEduAlignments;
        rgEducationalAlignment.DataBind();

        if (dtEduAlignments.Rows.Count > 0)
        {
            pnlSelectEducationalAlignment.Visible = false;
            pnlEducationalAlignmentGrid.Visible = true;
        }
        else
        {
            pnlSelectEducationalAlignment.Visible = false;
            pnlEducationalAlignmentGrid.Visible = false;
        }
    }

    private void UpdatEducationalAlignments()
    {
        // Set controls values into variables
        DataTable dtEduAlignments = (DataTable)ViewState["dtEduAlignments"];

        foreach (GridDataItem _item in rgEducationalAlignment.Items)
        {
            int _itemId = int.Parse(_item["ItemID"].Text);
            int educationalAlignmentEnum = int.Parse(_item["EducationalAlignmentEnum"].Text);
            string educationalAlignmentValue = ((TextBox)_item.FindControl("txtEducationalAlignmentValue")).Text;
            int lrmiItemID = int.Parse(_item["LRMIItemID"].Text);

            DataRow[] dr = dtEduAlignments.Select("EducationalAlignmentEnum = " + educationalAlignmentEnum);
            if (dr.Length > 0)
            {
                dr[0]["EducationalAlignmentValues"] = educationalAlignmentValue;
                dtEduAlignments.AcceptChanges();
            }
        }

        ViewState["dtEduAlignments"] = dtEduAlignments;
    }

    private void SelectEducationalAlignment()
    {
        DataTable dtLRMIDetails = (DataTable)ViewState["dtEduAlignments"];
        DataTable dtTemp = dtLRMIDetails.Copy();
        dtTemp.Clear();

        foreach (ListItem _item in cblEducationalAlignment.Items)
        {
            if (_item.Selected)
            {
                DataRow[] rows = dtLRMIDetails.Select("EducationalAlignmentEnum = " + _item.Value);
                DataRow newRow = dtTemp.NewRow();

                if (rows.Length > 0)
                {
                    newRow["ItemID"] = int.Parse(rows[0]["ItemID"].ToString());
                    newRow["EducationalAlignmentEnum"] = int.Parse(rows[0]["EducationalAlignmentEnum"].ToString());
                    newRow["EducationalAlignmentValues"] = rows[0]["EducationalAlignmentValues"].ToString();
                    newRow["LRMIItemID"] = int.Parse(rows[0]["LRMIItemID"].ToString());
                }
                else
                {
                    newRow["ItemID"] = 0;
                    newRow["EducationalAlignmentEnum"] = int.Parse(_item.Value);
                    newRow["EducationalAlignmentValues"] = "";
                    newRow["LRMIItemID"] = 0;
                }

                dtTemp.Rows.Add(newRow);
            }
        }

        ViewState["dtEduAlignments"] = dtTemp;
    }

    private void SetControlsActive(bool isActive)
    {
        trEducationalAlignment.Disabled = !isActive;
        tblLRMIDetails.Disabled = !isActive;
        tblDocDetails.Disabled = !isActive;

        lnkSelectTypes.Enabled = isActive;
        rcbInstructionType.Enabled = isActive;
        rcbActivityType.Enabled = isActive;
        rtbDuration.Enabled = isActive;
        rcbTargetAudience.Enabled = isActive;
        rtbAgeAppropriateCriteriaMin.Enabled = isActive;
        rtbAgeAppropriateCriteriaMax.Enabled = isActive;
        rcbLearningResourceType.Enabled = isActive;
        RadButtonSave.Enabled = isActive;
        RadButtonCancel.Disabled = !isActive;
    }

    #endregion

    #region LRMI Handler Methods

    private string GetDocumentCreatedBy(int userID)
    {
        DataSet dsCreatedBy = new DataSet();

        // Creates a new Custom table item provider
        CustomTableItemProvider customTableProvider = new CustomTableItemProvider(CMSContext.CurrentUser);

        // Creates a new Data Class Info
        DataClassInfo customTable = DataClassInfoProvider.GetDataClass(userTableClassName);

        // Checks if Custom table 'Sample table' exists
        if (customTable != null)
        {
            // Prepares the parameters
            string where = "UserID = '" + userID + "'";
            int topN = 0;
            string orderBy = "";
            string columns = "FirstName, MiddleName, LastName, FullName";

            // Gets the data set according to the parameters
            dsCreatedBy = customTableProvider.GetItems(customTable.ClassName, where, orderBy, topN, columns);
        }

        if (dsCreatedBy.Tables.Count > 0 && dsCreatedBy.Tables[0].Rows.Count > 0)
        {
        return dsCreatedBy.Tables[0].Rows[0]["FullName"].ToString();
    }
        else
        {
            return string.Empty;
        }

    }

    private void LoadLRMITag(int documentID)
    {
        // Bind LRMI Tag Data For Specific Document ID
        DataTable dtLRMI = GetLRMITag(documentID);
        BindPageInEditMode(dtLRMI);

        // Bind LRMI Tag Details Data
        int lrmiItemID = int.Parse(hdnLRMIItemId.Value);
        DataTable dtLRMIDetails = GetLRMITagDetails(lrmiItemID);
        BindEducationalAlignment(dtLRMIDetails);

        // Bind LRMI Tag Data For Specific Document ID
        DataTable dtDocument = GetDocumentDetails(documentID);
        BindDocumentDetails(dtDocument);

        // Bind Grid
        BindGrid();
    }

    private void SaveLRMITag(int documentID)
    {
        // Set controls values into variables for LRMI Tag
        int itemId = ConvertNumeric(hdnLRMIItemId.Value);
        int instructionEnum = ConvertNumeric(rcbInstructionType.SelectedValue);
        int activityEnum = ConvertNumeric(rcbActivityType.SelectedValue);
        int learningResourceTypeEnum = ConvertNumeric(rcbLearningResourceType.SelectedValue);
        int durationMinutes = ConvertNumeric(rtbDuration.Text);
        int targetAudienceEnum = ConvertNumeric(rcbTargetAudience.SelectedValue);
        string ageAppropriateCriteria = rtbAgeAppropriateCriteriaMin.Text + "-" + rtbAgeAppropriateCriteriaMax.Text;
	    string educationalSubjectValue = rcbEducationalSubject.SelectedValue;
	    string educationalLevelValue = rcbEducationalLevel.SelectedValue;


        deletedStandards.Clear();
        // Check the item id of the LRMI data for given document id
        // This is used to fix the insertion of duplicate records on F5(Refresh)
        DataTable dtLRMICheck = GetLRMITag(documentID);
        if (dtLRMICheck.Rows.Count > 0)
        {
            itemId = ConvertNumeric(dtLRMICheck.Rows[0]["ItemID"].ToString());
            hdnLRMIItemId.Value = dtLRMICheck.Rows[0]["ItemID"].ToString();
        }

        // Insert LRMI Tag Data
        InsertLRMITag(itemId, instructionEnum, activityEnum, durationMinutes, targetAudienceEnum, ageAppropriateCriteria, documentID, learningResourceTypeEnum);

        if (ConvertNumeric(hdnLRMIItemId.Value) == 0)
        {
            // Get the LRMI Id of the newly inserted record
            DataTable dtLRMI = GetLRMITag(documentID);
            hdnLRMIItemId.Value = dtLRMI.Rows[0]["ItemID"].ToString();
        }
        else
        {
            // Delete all educational alignments data for given LRMI Id
            DeleteLRMITagDetails(ConvertNumeric(hdnLRMIItemId.Value), documentID);
        }

        // Set controls values into variables for LRMI Tag Details
        int counter = 1;
        DataTable dtEduAlignments = (DataTable)ViewState["dtEduAlignments"];

        foreach (GridDataItem _item in rgEducationalAlignment.Items)
        {
            int educationalAlignmentEnum = int.Parse(_item["EducationalAlignmentEnum"].Text);
            string educationalAlignmentValue = ((TextBox)_item.FindControl("txtEducationalAlignmentValue")).Text;
            if ((educationalAlignmentEnum == (int)LookupDetails.Assessed || educationalAlignmentEnum == (int)LookupDetails.Requires || educationalAlignmentEnum == (int)LookupDetails.Teaches) && !string.IsNullOrEmpty(educationalAlignmentValue))
            {
                string rowStandards = educationalAlignmentValue.Remove(educationalAlignmentValue.Length - 1, 1);
                string[] standards = rowStandards.Split('|');
                foreach (string st in standards)
                {
                    deletedStandards.Remove(new KeyValuePair<int, string>(educationalAlignmentEnum, st));
                }
            }
        }

        foreach (GridDataItem _item in rgEducationalAlignment.Items)
        {
            int _itemId = int.Parse(_item["ItemID"].Text);
            int educationalAlignmentEnum = int.Parse(_item["EducationalAlignmentEnum"].Text);
            string educationalAlignmentValue = ((TextBox)_item.FindControl("txtEducationalAlignmentValue")).Text;
            int lrmiItemID = int.Parse(hdnLRMIItemId.Value);
            
            string newstandards = "";
            if ((educationalAlignmentEnum == (int)LookupDetails.Assessed || educationalAlignmentEnum == (int)LookupDetails.Requires || educationalAlignmentEnum == (int)LookupDetails.Teaches) && !string.IsNullOrEmpty(educationalAlignmentValue) && deletedStandards.Count > 0)
            {
                string rowStandards = educationalAlignmentValue.Remove(educationalAlignmentValue.Length - 1, 1);
                string[] standards = rowStandards.Split('|');
                foreach (string st in standards)
                {
                    if (!deletedStandards.Contains(new KeyValuePair<int, string>((int)LookupDetails.Assessed, st)) && !deletedStandards.Contains(new KeyValuePair<int, string>((int)LookupDetails.Requires, st)) && !deletedStandards.Contains(new KeyValuePair<int, string>((int)LookupDetails.Teaches, st)))
                    {
                        newstandards += st + "|";
                    }
                }
                educationalAlignmentValue = newstandards;
            }

			//Get the dropdown values
			//string theEducationalSubject = string.Empty;
			//string theEducationalLevel = string.Empty;

	        if (educationalAlignmentEnum == (int) LookupDetails.EducationalLevel &&
	            !string.IsNullOrEmpty(((Telerik.Web.UI.RadComboBox) FindControl("rcbEducationalLevel")).Text))
	        {
				educationalAlignmentValue = ((Telerik.Web.UI.RadComboBox)FindControl("rcbEducationalLevel")).Text;
	        }

	        if (educationalAlignmentEnum == (int)LookupDetails.EducationalSubject &&
	            !string.IsNullOrEmpty(((Telerik.Web.UI.RadComboBox) FindControl("rcbEducationalSubject")).Text))
	        {
				educationalAlignmentValue = ((Telerik.Web.UI.RadComboBox)FindControl("rcbEducationalSubject")).Text;
	        }

            // Insert LRMI Tag Details Data
            InsertLRMITagDetails(_itemId, educationalAlignmentEnum, educationalAlignmentValue, lrmiItemID);
            if (((TextBox)_item.FindControl("txtEducationalAlignmentValue")).Style["display"] == "none")
            {
                string stdscript = "SaveStandards('" + ParentDocID + "','" + educationalAlignmentValue.Replace("|", ",") + "');";

                ScriptHelper.RegisterClientScriptBlock(Page, typeof(string), "savestd_" + counter, ScriptHelper.GetScript(stdscript));
                counter++;
            }
            
            DataRow[] dr = dtEduAlignments.Select("EducationalAlignmentEnum = " + educationalAlignmentEnum);
            if (dr.Length > 0)
            {
                dr[0]["EducationalAlignmentValues"] = educationalAlignmentValue;
                dtEduAlignments.AcceptChanges();
            }
        }

        ViewState["dtEduAlignments"] = dtEduAlignments;
        BindGrid();

       //ScriptHelper.Alert(Page, "Data has been saved.");

       string script = "showmessage();";

        ScriptHelper.RegisterClientScriptBlock(Page, typeof(string), "showsavemessage", ScriptHelper.GetScript(script));
       
    }

    private void InsertLRMITag(int itemId, int instructionEnum, int activityEnum, int durationMinutes, int targetAudienceEnum, string ageAppropriateCriteria, int documentID, int learningResourceTypeEnum,string useRightURL = null, string originalThirdPartyURL = null)
    {
        // Creates Custom Table Provider
        CustomTableItemProvider customTableProvider = new CustomTableItemProvider(CMSContext.CurrentUser);

        // Creates Custom Table 
        DataClassInfo customTable = DataClassInfoProvider.GetDataClass(customTableClassLRMI);

        // Checks if Custom table exists
        if (customTable != null)
        {
            // Create Custom Table Item
            CustomTableItem customTableItem = null;

            if (itemId == 0)
                customTableItem = CustomTableItem.New(customTable.ClassName, customTableProvider);
            else
                customTableItem = customTableProvider.GetItem(itemId, customTable.ClassName);

            // Sets the row data
            customTableItem.SetValue("InstructionEnum", instructionEnum);
            customTableItem.SetValue("ActivityEnum", activityEnum);
            customTableItem.SetValue("LearningResourceTypeEnum", learningResourceTypeEnum);
            customTableItem.SetValue("DurationMinutes", durationMinutes);
            customTableItem.SetValue("TargetAudienceEnum", targetAudienceEnum);
            customTableItem.SetValue("AgeAppropriateCriteria", ageAppropriateCriteria);
            customTableItem.SetValue("UseRightURL", useRightURL);
            customTableItem.SetValue("OriginalThirdPartyURL", originalThirdPartyURL);
            customTableItem.SetValue("DocumentID", documentID);

            if (itemId == 0)
                customTableItem.Insert();
            else
                customTableItem.Update();
        }
    }

    private void DeleteLRMITagDetails(int lrmiItemID, int docID)
    {
        // Creates Custom Table Provider
        CustomTableItemProvider customTableProvider = new CustomTableItemProvider(CMSContext.CurrentUser);

        // Creates Custom Table 
        DataClassInfo customTable = DataClassInfoProvider.GetDataClass(customTableClassLRMIDetail);

        // Checks if Custom table exists
        if (customTable != null)
        {
            // Prepares the parameters
            string where = "LRMIItemID = '" + lrmiItemID + "'";
            int topN = 0;
            string orderBy = "";
            string columns = "ItemID,EducationalAlignmentValues,EducationalAlignmentEnum";

            // Gets the data set according to the parameters
            DataSet ds = customTableProvider.GetItems(customTable.ClassName, where, orderBy, topN, columns);
            CustomTableItem customTableItem = null;

            foreach (DataRow _row in ds.Tables[0].Rows)
            {
                // Create Custom Table Item
                int _itemId = int.Parse(_row["ItemID"].ToString());
                int itemEnumID = (int)_row["EducationalAlignmentEnum"];
                string rowStandards = _row["EducationalAlignmentValues"].ToString();
                if ((itemEnumID == (int)LookupDetails.Assessed || itemEnumID == (int)LookupDetails.Requires || itemEnumID == (int)LookupDetails.Teaches) && !string.IsNullOrEmpty(rowStandards))
                {
                    rowStandards = rowStandards.Remove(rowStandards.Length - 1, 1);
                    string[] standards = rowStandards.Split('|');
                    foreach (string st in standards)
                    {
                        deletedStandards.Add(new KeyValuePair<int, string>(itemEnumID, st));
                        ThinkgateKenticoHelper.deleteStandard(docID, Convert.ToInt32(st), false);
                    }
                }

                customTableItem = customTableProvider.GetItem(_itemId, customTable.ClassName);
                customTableItem.Delete();
            }
        }
    }

    private void InsertLRMITagDetails(int itemId, int educationalAlignmentEnum, string educationalAlignmentValue, int lrmiItemID)
    {
        // Creates Custom Table Provider
        CustomTableItemProvider customTableProvider = new CustomTableItemProvider(CMSContext.CurrentUser);

        // Creates Custom Table 
        DataClassInfo customTable = DataClassInfoProvider.GetDataClass(customTableClassLRMIDetail);

        // Checks if Custom table exists
        if (customTable != null)
        {
            // Create Custom Table Item
            CustomTableItem customTableItem = CustomTableItem.New(customTable.ClassName, customTableProvider);

            // Sets the row data
            customTableItem.SetValue("EducationalAlignmentEnum", educationalAlignmentEnum);
            customTableItem.SetValue("EducationalAlignmentValues", educationalAlignmentValue);
            customTableItem.SetValue("LRMIItemID", lrmiItemID);

            customTableItem.Insert();
        }
    }

    #endregion

    #region GetData Handler Methods

    private DataTable GetDocumentDetails(int documentID)
    {
        DataSet dsDocumentDetail = new DataSet();

        // Creates a new Custom table item provider
        CustomTableItemProvider customTableProvider = new CustomTableItemProvider(CMSContext.CurrentUser);

        // Creates a new Data Class Info
        DataClassInfo customTable = DataClassInfoProvider.GetDataClass(documentTableClassName);

        // Checks if Custom table 'Sample table' exists
        if (customTable != null)
        {
            // Prepares the parameters
            string where = "DocumentNodeID = '" + documentID + "'";
            int topN = 0;
            string orderBy = "";
            string columns = "DocumentPageTitle, DocumentName, DocumentCreatedWhen, DocumentCreatedByUserID, DocumentCulture, DocumentType";

            // Gets the data set according to the parameters
            dsDocumentDetail = customTableProvider.GetItems(customTable.ClassName, where, orderBy, topN, columns);
        }

        return dsDocumentDetail.Tables[0];
    }

    private DataTable GetLRMITag(int documentID)
    {
        DataSet dsLRMITag = new DataSet();

        // Creates a new Custom table item provider
        CustomTableItemProvider customTableProvider = new CustomTableItemProvider(CMSContext.CurrentUser);

        // Creates a new Data Class Info
        DataClassInfo customTable = DataClassInfoProvider.GetDataClass(customTableClassLRMI);

        // Checks if Custom table 'Sample table' exists
        if (customTable != null)
        {
            // Prepares the parameters
            string where = "DocumentID = '" + documentID + "'";
            int topN = 0;
            string orderBy = "";
            string columns = "ItemID, InstructionEnum, ActivityEnum, LearningResourceTypeEnum, DurationMinutes, TargetAudienceEnum, AgeAppropriateCriteria, UseRightURL, OriginalThirdPartyURL, DocumentID";

            // Gets the data set according to the parameters
            dsLRMITag = customTableProvider.GetItems(customTable.ClassName, where, orderBy, topN, columns);
        }

        return dsLRMITag.Tables[0];
    }

    private DataTable GetLRMITagDetails(int lrmiItemID)
    {
        DataSet dsLRMITagDetails = new DataSet();

        // Creates a new Custom table item provider
        CustomTableItemProvider customTableProvider = new CustomTableItemProvider(CMSContext.CurrentUser);

        // Creates a new Data Class Info
        DataClassInfo customTable = DataClassInfoProvider.GetDataClass(customTableClassLRMIDetail);

        // Checks if Custom table 'Sample table' exists
        if (customTable != null)
        {
            // Prepares the parameters
            string where = "LRMIItemID = '" + lrmiItemID + "'";
            int topN = 0;
            string orderBy = "";
            string columns = "ItemID, EducationalAlignmentEnum, EducationalAlignmentValues, LRMIItemID";

            // Gets the data set according to the parameters
            dsLRMITagDetails = customTableProvider.GetItems(customTable.ClassName, where, orderBy, topN, columns);
        }

        ViewState["dtEduAlignments"] = dsLRMITagDetails.Tables[0];
        return dsLRMITagDetails.Tables[0];
    }
    
    private DataTable GetDataTable(string connectionString, string selectSQL)
    {
        DataTable dataTable = new DataTable();

        if (selectSQL != null)
        {
            string connectionStringToUse = string.Empty;

            if (connectionString != null)
            { connectionStringToUse = connectionString; }
            else
            { connectionStringToUse = rootConnectionString; }

            SqlConnection sqlConnection = new SqlConnection(connectionStringToUse);
            SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
            sqlDataAdapter.SelectCommand = new SqlCommand(selectSQL, sqlConnection);

            try
            {
                sqlConnection.Open();
            }
            catch (SqlException ex)
            {
                Debug.WriteLine("SqlException: " + ex.Message);
                return dataTable;
            }

            try
            {
                sqlDataAdapter.Fill(dataTable);
            }
            finally
            {
                sqlConnection.Close();
            }
        }

        return dataTable;
    }

    private static string GetLocalDbConnectionString(string connString)
    {
		return connString.Replace("ThinkgateConfig", ThinkgateKenticoHelper.FindDBName());
    }

	#endregion

    #region Helper Methods

    private int ConvertNumeric(string value)
    {
        return value == "" ? 0 : int.Parse(value);
    }
    
    #endregion

    #endregion
}