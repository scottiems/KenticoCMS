using System;
using System.Linq;
using System.Data;

using CMS.CMSHelper;
using CMS.FormEngine;
using CMS.GlobalHelper;
using CMS.LicenseProvider;
using CMS.PortalEngine;
using CMS.SettingsProvider;
using CMS.DocumentEngine;
using CMS.UIControls;
using CMS.WorkflowEngine;
using CMS.ExtendedControls;
using System.Configuration;
using System.Collections.Generic;
using CMS.SiteProvider;

public partial class CMSModules_Content_CMSDesk_Edit_Edit : CMSContentPage
{

    public enum LookupDetail
    {
        None = 0,
        All = 1,
        Administrator = 2, 
        Teacher = 3,
        Message = 4,
        UserAgreement = 5,
        Curricula = 37,
        CurriculumMap = 79,
        UnitPlan = 80,
        LessonPlan = 81,
        CurriculumUnitMA = 82,
        CurriculumUnitOH = 83,
        Resource = 84,
        CurriculumMapForm = 85,
        CurriculumMapAttachmentForm = 86,
        CurriculumMapWebBasedForm = 87,
        UnitPlanForm = 88,
        UnitPlanAttachmentForm = 89,
        UnitPlanWebBasedForm= 90,
        LessonPlanForm = 91,
        LessonPlanAttachmentForm = 92,
        LessonPlanWebBasedForm = 93,
        StateModelCurriculumUnitForm = 94,
        StateModelCurriculumForm = 95,
        ResourcePlanAttachmentForm = 96,
        ResourcePlanWebBasdForm = 97
    }

    #region "Constants"
    private const string THINKGATE_RESOURCE_PLAN = "Add New Resource";
    private const string THINKGATE_RESOURCE_PLAN_ATTACHMENT = "Add New Resource Attachment";
    private const string THINKGATE_RESOURCE_PLAN_WEBBASED = "Add New Resource Web-Based";
    private const string THINKGATE_UNIT_PLAN = "Add New Unit Plan";
    private const string THINKGATE_UNIT_PLAN_ATTACHMENT = "Add New Unit Plan Attachment";
    private const string THINKGATE_UNIT_PLAN_WEBBASED = "Add New Unit Plan Web-Based";
    private const string THINKGATE_CURRICULUM_UNIT = "Add New Curriculum Unit";
    private const string THINKGATE_INSTRUCTION_PLAN = "Add New Curriculum Map";
    private const string THINKGATE_INSTRUCTION_PLAN_ATTACHMENT = "Add New Curriculum Map Attachment";
    private const string THINKGATE_INSTRUCTION_PLAN_WEBBASED = "Add New Curriculum Map Web-Based";
    private const string THINKGATE_LESSON_PLAN = "Add New Lesson Plan";
    private const string THINKGATE_LESSON_PLAN_ATTACHMENT = "Add New Lesson Plan Attachment";
    private const string THINKGATE_LESSON_PLAN_WEBBASED = "Add New Lesson Plan Web-Based";

    private const string THINKGATE_COMPETENCYLIST_PLAN = "Add New Competency List";

    private string _title = string.Empty;
        
    private const string THINKGATE_RESOURCE = "thinkgate.resource";

    private const string THINKGATE_INSTRUCTIONPLAN = "thinkgate.InstructionalPlan";

    private const string THINKGATE_COMPETENCYLIST = "thinkgate.competencylist"; 



    #endregion

    public string SetFormTitle
    {
        get
        {
            return _title;
        }
        set
        {
            _title = value;
        }
    }


    public string ExpiryDateValue { set; get; }

    #region "Variables"

    protected bool newdocument = false;
    protected bool newculture = false;
    protected bool mShowToolbar = false;

    protected DataClassInfo ci = null;
    private DocType DocumentType { get; set; }
    private Int32 DocumentID { get; set; }
    protected string NodeClassName = string.Empty;

    #endregion


    #region "Protected variables"

    /// <summary>
    /// Local page message placeholder
    /// </summary>
    public override MessagesPlaceHolder MessagesPlaceHolder
    {
        get
        {
            // Ensure default offset in special case of edit page
            MessagesPlaceHolder plc = base.MessagesPlaceHolder;
            if (plc != null)
            {
                plc.WrapperControlID = "pnlContent";
                plc.OffsetY = 7;
            }
            return plc;
        }
    }


    /// <summary>
    /// Class identifier for new documents.
    /// </summary>
    protected int ClassID
    {
        get
        {
            return QueryHelper.GetInteger("classid", 0);
        }
    }


    /// <summary> 
    /// TemplateID, used when Use template selection is enabled for class of newly created document.
    /// </summary>
    protected int TemplateID
    {
        get
        {
            return QueryHelper.GetInteger("templateid", -1);
        }
    }


    /// <summary>
    /// Identifier of parent document. (For newly created documents.)
    /// </summary>
    protected int ParentNodeID
    {
        get
        {
            return QueryHelper.GetInteger("parentnodeid", 0);
        }
    }


    /// <summary>
    /// Culture of parent document. (For newly created documents.)
    /// </summary>
    protected string ParentCulture
    {
        get
        {
            return QueryHelper.GetString("parentculture", CMSContext.PreferredCultureCode);
        }
    }


    /// <summary>
    /// Indicates if e-commerce product section is edited.
    /// </summary>
    protected bool ProductSection
    {
        get
        {
            return (Mode != null) && (Mode.ToLowerCSafe() == "productssection") && (ci != null) && (ci.ClassIsProductSection);
        }
    }


    #endregion


    #region "Page events"

    protected override void OnPreInit(EventArgs e)
    {
        // Do not redirect for non-existing document if new culture version is being created
        DocumentManager.RedirectForNonExistingDocument = (Action != "newculture");

        base.OnPreInit(e);
    }


    protected override void OnInit(EventArgs e)
    {
        CurrentUserInfo currentUser = CMSContext.CurrentUser;

        // Check UIProfile
        if (!currentUser.IsAuthorizedPerUIElement("CMS.Content", "EditForm"))
        {
            RedirectToCMSDeskUIElementAccessDenied("CMS.Content", "EditForm");
        }

        base.OnInit(e);

        DocumentManager.OnAfterAction += DocumentManager_OnAfterAction;
        DocumentManager.OnLoadData += DocumentManager_OnLoadData;

        // Register scripts
        string script = "function " + formElem.ClientID + "_RefreshForm(){" + Page.ClientScript.GetPostBackEventReference(btnRefresh, "") + " }";
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), formElem.ClientID + "_RefreshForm", ScriptHelper.GetScript(script));

        ScriptHelper.RegisterCompletePageScript(this);
        ScriptHelper.RegisterProgress(this);
        ScriptHelper.RegisterDialogScript(this);

        formElem.OnAfterDataLoad += formElem_OnAfterDataLoad;

        // Analyze the action parameter
        switch (Action)
        {
            case "new":
            case "convert":
                {
                    newdocument = true;

                    // Check if document type is allowed under parent node
                    if ((ParentNodeID > 0) && (ClassID > 0))
                    {
                        // Check class
                        ci = DataClassInfoProvider.GetDataClass(ClassID);
                        if (ci == null)
                        {
                            throw new Exception("[Content/Edit.aspx]: Class ID '" + ClassID + "' not found.");
                        }

                        if (ci.ClassName.ToLowerCSafe() == "cms.blog")
                        {
                            if (!LicenseHelper.LicenseVersionCheck(URLHelper.GetCurrentDomain(), FeatureEnum.Blogs, VersionActionEnum.Insert))
                            {
                                RedirectToAccessDenied(String.Format(GetString("cmsdesk.bloglicenselimits"), ""));
                            }
                        }

                        if (!LicenseHelper.LicenseVersionCheck(URLHelper.GetCurrentDomain(), FeatureEnum.Documents, VersionActionEnum.Insert))
                        {
                            RedirectToAccessDenied(String.Format(GetString("cmsdesk.documentslicenselimits"), ""));
                        }

                        // Check if need template selection, if so, then redirect to template selection page
                        int templateId = TemplateID;
                        if (!ProductSection && ci.ClassShowTemplateSelection && (templateId < 0) && (ci.ClassName.ToLowerCSafe() != "cms.menuitem"))
                        {
                            URLHelper.Redirect("~/CMSModules/Content/CMSDesk/TemplateSelection.aspx" + URLHelper.Url.Query);
                        }

                        // Set default template ID
                        formElem.DefaultPageTemplateID = (templateId > 0) ? templateId : ci.ClassDefaultPageTemplateID;

                        string newClassName = ci.ClassName;
                        formElem.FormName = newClassName + ".default";
                        NodeClassName = newClassName;
                        DocumentManager.Mode = FormModeEnum.Insert;
                        DocumentManager.ParentNodeID = ParentNodeID;
                        DocumentManager.NewNodeCultureCode = ParentCulture;

                        // Set up the document conversion
                        int convertDocumentId = QueryHelper.GetInteger("convertdocumentid", 0);
                        if (convertDocumentId > 0)
                        {
                            DocumentManager.SourceDocumentID = convertDocumentId;
                            DocumentManager.Mode = FormModeEnum.Convert;
                        }

                        DocumentManager.NewNodeClassID = ClassID;

                        // Check allowed document type
                        TreeNode parentNode = DocumentManager.ParentNode;
                        if ((parentNode == null) || !DataClassInfoProvider.IsChildClassAllowed(parentNode.GetIntegerValue("NodeClassID", 0), ClassID))
                        {
                            AddNotAllowedScript("child");
                        }

                        if (!currentUser.IsAuthorizedToCreateNewDocument(DocumentManager.ParentNode, DocumentManager.NewNodeClassName))
                        {
                            AddNotAllowedScript("new");
                        }
                    }

                    if (RequiresDialog)
                    {
                        SetTitle("CMSModules/CMS_Content/Menu/New.png", GetString("Content.NewTitle"), null, null);
                    }
                }
                break;

            case "newculture":
                {
                    newculture = true;
                    int nodeId = QueryHelper.GetInteger("nodeid", 0);
                    DocumentManager.Mode = FormModeEnum.InsertNewCultureVersion;
                    formElem.NodeID = nodeId;

                    // Check permissions
                    bool authorized = false;
                    if (nodeId > 0)
                    {
                        // Get the node                    
                        TreeNode treeNode = DocumentManager.Tree.SelectSingleNode(nodeId);
                        if (treeNode != null)
                        {
                            DocumentManager.NewNodeClassID = treeNode.GetIntegerValue("NodeClassID", 0);
                            DocumentManager.ParentNodeID = ParentNodeID;
                            DocumentManager.NewNodeCultureCode = ParentCulture;
                            authorized = currentUser.IsAuthorizedToCreateNewDocument(treeNode.NodeParentID, treeNode.NodeClassName);

                            if (authorized)
                            {
                                string className = DocumentManager.NewNodeClassName;
                                NodeClassName = DocumentManager.NewNodeClassName;
                                if (className.ToLowerCSafe() == "cms.blog")
                                {
                                    if (!LicenseHelper.LicenseVersionCheck(URLHelper.GetCurrentDomain(), FeatureEnum.Blogs, VersionActionEnum.Insert))
                                    {
                                        RedirectToAccessDenied(String.Format(GetString("cmsdesk.bloglicenselimits"), ""));
                                    }
                                }

                                if (!LicenseHelper.LicenseVersionCheck(URLHelper.GetCurrentDomain(), FeatureEnum.Documents, VersionActionEnum.Insert))
                                {
                                    RedirectToAccessDenied(String.Format(GetString("cmsdesk.documentslicenselimits"), ""));
                                }

                                // Default data document ID
                                formElem.CopyDefaultDataFromDocumentId = ValidationHelper.GetInteger(Request.QueryString["sourcedocumentid"], 0);

                                ci = DataClassInfoProvider.GetDataClass(className);
                                formElem.FormName = className + ".default";
                            }
                        }
                    }

                    if (!authorized)
                    {
                        AddNotAllowedScript("newculture");
                    }

                    if (RequiresDialog)
                    {
                        SetTitle("CMSModules/CMS_Content/Menu/New.png", GetString("content.newcultureversiontitle"), null, null);
                    }
                }
                break;

            default:
                {
                    TreeNode node = Node;
                    if (node == null)
                    {
                        RedirectToNewCultureVersionPage();
                    }
                    else
                    {
                        // Update view mode
                        if (!RequiresDialog)
                        {
                            CMSContext.ViewMode = ViewModeEnum.EditForm;
                        }

                        EnableSplitMode = true;
                        DocumentManager.Mode = FormModeEnum.Update;
                        NodeClassName = node.NodeClassName.ToString();
                        ci = DataClassInfoProvider.GetDataClass(node.NodeClassName);
                        if (RequiresDialog)
                        {
                            menuElem.ShowSaveAndClose = true;

                            // Add the document name to the properties header title
                            string nodeName = node.GetDocumentName();
                            // Get name for root document
                            if (node.NodeClassName.ToLowerCSafe() == "cms.root")
                            {
                                nodeName = CMSContext.CurrentSite.DisplayName;
                            }

                            SetTitle("CMSModules/CMS_Content/Menu/New.png", GetString("Content.EditTitle") + " \"" + HTMLHelper.HTMLEncode(ResHelper.LocalizeString(nodeName)) + "\"", null, null);
                        }
                    }
                }
                break;
        }

        formElem.Visible = true;

        // Display / hide the CK editor toolbar area
        FormInfo fi = FormHelper.GetFormInfo(ci.ClassName, false);

        if (fi.UsesHtmlArea())
        {
            // Add script to display toolbar
            if (formElem.HtmlAreaToolbarLocation.ToLowerCSafe() == "out:cktoolbar")
            {
                mShowToolbar = true;
            }
        }

        // Init form for product section edit
        if (ProductSection)
        {
            // Form prefix
            formElem.FormPrefix = "ecommerce";
        }

        if (RequiresDialog)
        {
            plcCKFooter.Visible = false;
        }
    }


    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        this.DocumentID = 0;
        if (!string.IsNullOrWhiteSpace(Request.QueryString.Get("documentType")))
        { this.DocumentType = (DocType)Enum.Parse(typeof(DocType), Request.QueryString.Get("documentType")); }

        if (!string.IsNullOrWhiteSpace(Request.QueryString.Get("documentId")))
        { this.DocumentID = Convert.ToInt32(Request.QueryString.Get("documentId")); }

        // Register script files
        ScriptHelper.RegisterScriptFile(this, "cmsedit.js");
        ScriptHelper.RegisterTooltip(this);

        formElem.MessagesPlaceHolder.ClearLabels();

        bool hasCategories = (formElem.BasicForm != null) && formElem.BasicForm.FormInformation.ItemsList.OfType<FormCategoryInfo>().Any();
        if (hasCategories)
        {
            formElem.DefaultFormLayout = FormLayoutEnum.FieldSets;
            formElem.DefaultCategoryName = ResHelper.GetString("general.general");
        }

        InitBindSkuAction();

        if (NodeClassName == "thinkgate.curriculumUnit")
        {
            formElem.BasicForm.OnAfterValidate += BasicForm_OnAfterValidate;
            formElem.BasicForm.OnBeforeValidate += BasicForm_OnBeforeValidate;
        }
        if (NodeClassName == THINKGATE_INSTRUCTIONPLAN)
        {
            formElem.BasicForm.OnAfterValidate += BasicFormInstructionalPlanSuggestedBeginDate_OnAfterValidate;
            formElem.BasicForm.OnBeforeValidate += BasicFormInstructionalPlanSuggestedBeginDate_OnBeforeValidate;
        }
        if (NodeClassName == THINKGATE_RESOURCE)
        {
            formElem.BasicForm.OnAfterValidate += ResourceWebLink_OnValidate;
            formElem.BasicForm.OnBeforeValidate += ResourceWebLink_OnValidate;
            SetClientTargetsSQL();
        }
        Session["CurrentTreeNodeID"] = string.IsNullOrWhiteSpace(Request.QueryString.Get("nodeid")) ? 0 : Convert.ToInt64(Request.QueryString.Get("nodeid"));
    }

    private void BasicFormInstructionalPlanSuggestedBeginDate_OnAfterValidate(object sender, EventArgs e)
    {
        if(formElem.BasicForm.FieldControls["InstructionalPlanSuggestedBeginDate"].Value != null && formElem.BasicForm.FieldControls["ExpirationDate"].Value != null)
        {
            if (DateTime.Parse(Convert.ToString(formElem.BasicForm.FieldControls["ExpirationDate"].Value)) < DateTime.Parse(Convert.ToString(formElem.BasicForm.FieldControls["InstructionalPlanSuggestedBeginDate"].Value)))
            {
                RaiseError("InstructionalPlanSuggestedBeginDate", "<br/><br/><b>Suggested Begin Date should not be greater than expiration date</b>", formElem.BasicForm);
                
                formElem.BasicForm.StopProcessing = true;

                return;                

            }
        }        
    }

    private void BasicFormInstructionalPlanSuggestedBeginDate_OnBeforeValidate(object sender, EventArgs e)
    {
        if (formElem.BasicForm.FieldControls["InstructionalPlanSuggestedBeginDate"].Value != null && formElem.BasicForm.FieldControls["ExpirationDate"].Value != null)
        {
            ExpiryDateValue =Convert.ToString(formElem.BasicForm.FieldControls["ExpirationDate"].Value);

            if (DateTime.Parse(Convert.ToString(formElem.BasicForm.FieldControls["ExpirationDate"].Value)) < DateTime.Parse(Convert.ToString(formElem.BasicForm.FieldControls["InstructionalPlanSuggestedBeginDate"].Value)))
            {
                
                RaiseError("InstructionalPlanSuggestedBeginDate", "<br/><br/><b>Suggested Begin Date should not be greater than expiration date</b>", formElem.BasicForm);
                formElem.BasicForm.StopProcessing = true;
                return;
            }
        }          
    }

    protected static bool IsValidUrl(string urlString)
    {
        Uri uri;
        urlString = urlString.ToLower();
        if (urlString.StartsWith("www."))
            urlString = "http://" + urlString;

        return Uri.TryCreate(urlString, UriKind.Absolute, out uri)
            && (uri.Scheme == Uri.UriSchemeHttp
             || uri.Scheme == Uri.UriSchemeHttps
             || uri.Scheme == Uri.UriSchemeFtp
             || uri.Scheme == Uri.UriSchemeMailto
             || uri.Scheme == Uri.UriSchemeFile
             || uri.Scheme == Uri.UriSchemeGopher
             || uri.Scheme == Uri.UriSchemeNetPipe
             || uri.Scheme == Uri.UriSchemeNetTcp
             || uri.Scheme == Uri.UriSchemeNews
             || uri.Scheme == Uri.UriSchemeNntp
             );
    }

    protected void ResourceWebLink_OnValidate(object sender, EventArgs e)
    {
        string webLinkString = formElem.BasicForm.FieldControls["Weblink"].Value.ToString();
        if (webLinkString.Length > 0)
        {
            bool isValidWebLinkUrl = IsValidUrl(webLinkString);
            if (!isValidWebLinkUrl)
            {
                RaiseError("Weblink", "<br/><br/><b>Please enter a valid Web URL.</b>",
                    formElem.BasicForm);
                formElem.BasicForm.StopProcessing = true;
            }
        }
    }


    protected void SetClientTargetsSQL()
    {
        string dbName = ThinkgateKenticoHelper.FindDBName();

        //if (formElem.BasicForm.FieldControls != null && formElem.BasicForm.FieldControls["Grade"].Value.ToString() != "0" && formElem.BasicForm.FieldControls["Subject"].Value.ToString() == "0")
        //{
        //    formElem.BasicForm.FieldControls["Subject"].FieldInfo.Settings["Query"] = "Select '0', '<Select One>' union select distinct Subject,Subject from [" + dbName + "]..CurrCourses where Active='Yes' and grade='" + formElem.BasicForm.FieldControls["Grade"].Value + "'";
        //}
        //if (formElem.BasicForm.FieldControls != null && formElem.BasicForm.FieldControls["Subject"].Value.ToString() != "0" && formElem.BasicForm.FieldControls["Courses"].Value.ToString() == "0")
        //{
        //    formElem.BasicForm.FieldControls["Courses"].FieldInfo.Settings["Query"] = "Select '0', '<Select One>' union select distinct Course,Course  from [" + dbName + "]..CurrCourses where Active = 'Yes' and  grade='" + formElem.BasicForm.FieldControls["Grade"].Value + "' and Subject='" + formElem.BasicForm.FieldControls["Subject"].Value + "'";
        //}
        if (formElem.BasicForm.FieldControls != null && formElem.BasicForm.FieldControls["Type"].Value.ToString() != "0" && formElem.BasicForm.FieldControls["SubType"].Value.ToString() == "0")
        {
            formElem.BasicForm.FieldControls["SubType"].FieldInfo.Settings["Query"] = " Select 0, '<Select One>' union select Enum, description from dbo.TG_LookupDetails where lookupenum=" + Convert.ToInt32(formElem.BasicForm.FieldControls["Type"].Value);
        }

        if (formElem.BasicForm.FieldControls != null)
        {

            string typeSelected = Convert.ToString(formElem.BasicForm.FieldControls["Type"].Value);
            string category = formElem.BasicForm.FieldControls["Category"] != null ? Convert.ToString(formElem.BasicForm.FieldControls["Category"].Value) : "";            
            string Name = Convert.ToString(formElem.BasicForm.FieldControls["Name"].Value);
            string Description = Convert.ToString(formElem.BasicForm.FieldControls["Description"].Value);
            DateTime ExpirationDate = Convert.ToDateTime(formElem.BasicForm.FieldControls["ExpirationDate"].Value);
            //string Grade = Convert.ToString(formElem.BasicForm.FieldControls["Grade"].Value);
            //string Subject = Convert.ToString(formElem.BasicForm.FieldControls["Subject"].Value);
            //string Courses = Convert.ToString(formElem.BasicForm.FieldControls["Courses"].Value);
            string SubType = Convert.ToString(formElem.BasicForm.FieldControls["SubType"].Value);
            string Weblink = Convert.ToString(formElem.BasicForm.FieldControls["Weblink"].Value);            

            formElem.BasicForm.ReloadData();

            //formElem.BasicForm.FieldControls["Location"].Value = Location;
            formElem.BasicForm.FieldControls["SubType"].Value = SubType;
            //formElem.BasicForm.FieldControls["Subject"].Value = Subject;
            //formElem.BasicForm.FieldControls["Courses"].Value = Courses;
            //formElem.BasicForm.FieldControls["Grade"].Value = Grade;
            formElem.BasicForm.FieldControls["Name"].Value = Name;
            formElem.BasicForm.FieldControls["ExpirationDate"].Value = ExpirationDate;
            formElem.BasicForm.FieldControls["Description"].Value = Description;           
            if (formElem.BasicForm.FieldControls["Category"] != null)
                formElem.BasicForm.FieldControls["Category"].Value = category;            
            formElem.BasicForm.FieldControls["Type"].Value = typeSelected;
            formElem.BasicForm.FieldControls["Weblink"].Value = Weblink;
                       
            if (Request.QueryString["Type"] != null)
            {
                formElem.BasicForm.FieldControls["Type"].Value = Request.QueryString["Type"].ToString();
            }
            if (Request.QueryString["SubType"] != null)
            {
                formElem.BasicForm.FieldControls["SubType"].Value = Request.QueryString["SubType"].ToString();
            }
        }
    }

    protected void BasicForm_OnBeforeValidate(object sender, EventArgs e)
    {
        if (formElem.BasicForm.FieldControls["DocumentPublishFrom"].Value != null && formElem.BasicForm.FieldControls["DocumentPublishTo"].Value != null)
        {
            if (DateTime.Parse(Convert.ToString(formElem.BasicForm.FieldControls["DocumentPublishFrom"].Value)) >= DateTime.Parse(Convert.ToString(formElem.BasicForm.FieldControls["DocumentPublishTo"].Value)))
            {
                RaiseError("DocumentPublishFrom", "<br/><br/><b>PublishTo date should be greater than PublishFrom date</b>", formElem.BasicForm);
                formElem.BasicForm.StopProcessing = true;
            }
            else if (DateTime.Parse(Convert.ToString(System.DateTime.Now.Date)) > DateTime.Parse(Convert.ToString(formElem.BasicForm.FieldControls["DocumentPublishTo"].Value)))
            {
                RaiseError("DocumentPublishFrom", "<br/><br/><b>PublishTo date should be greater than Current date</b>", formElem.BasicForm);
                formElem.BasicForm.StopProcessing = true;
            }
        }
        else if (formElem.BasicForm.FieldControls["DocumentPublishTo"].Value != null)
        {
            if (DateTime.Parse(Convert.ToString(System.DateTime.Now.Date)) > DateTime.Parse(Convert.ToString(formElem.BasicForm.FieldControls["DocumentPublishTo"].Value)))
            {
                RaiseError("DocumentPublishFrom", "<br/><br/><b>PublishTo date should be greater than Current date</b>", formElem.BasicForm);
                formElem.BasicForm.StopProcessing = true;                
            }
        }
    }

    private void RaiseError(string sFieldControl, string sErrorMessage, CMS.FormControls.BasicForm form)
    {
        LocalizedLabel errorLabel = (LocalizedLabel)form.FieldErrorLabels[sFieldControl];
        errorLabel.Text = sErrorMessage;
        errorLabel.Visible = false;
        form.StopProcessing = true;
        formElem.ValidationErrorMessage = formElem.ValidationErrorMessage + sErrorMessage;        
        
    }

    protected void BasicForm_OnAfterValidate(object sender, EventArgs e)
    {
        //Condition Component and Type validation
        if (formElem.BasicForm.FieldControls["DocumentPublishFrom"].Value != null && formElem.BasicForm.FieldControls["DocumentPublishTo"].Value != null)
        {
            if (DateTime.Parse(Convert.ToString(formElem.BasicForm.FieldControls["DocumentPublishFrom"].Value)) >= DateTime.Parse(Convert.ToString(formElem.BasicForm.FieldControls["DocumentPublishTo"].Value)))
            {
                RaiseError("DocumentPublishFrom", "<br/><br/><b>PublishTo date should be greater than PublishFrom date</b>", formElem.BasicForm);
                formElem.BasicForm.StopProcessing = true;
            }
            else if (DateTime.Parse(Convert.ToString(System.DateTime.Now.Date)) > DateTime.Parse(Convert.ToString(formElem.BasicForm.FieldControls["DocumentPublishTo"].Value)))
            {
                RaiseError("DocumentPublishFrom", "<br/><br/><b>PublishTo date should be greater than Current date</b>", formElem.BasicForm);
                formElem.BasicForm.StopProcessing = true;
            }
        }
        else if (formElem.BasicForm.FieldControls["DocumentPublishTo"].Value != null)
        {
            if (DateTime.Parse(Convert.ToString(System.DateTime.Now.Date)) > DateTime.Parse(Convert.ToString(formElem.BasicForm.FieldControls["DocumentPublishTo"].Value)))
            {
                RaiseError("DocumentPublishFrom", "<br/><br/><b>PublishTo date should be greater than Current date</b>", formElem.BasicForm);
                formElem.BasicForm.StopProcessing = true;                
            }
        }       
    }

    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        ScriptHelper.RegisterStartupScript(this, typeof(string), "InitializePage", ScriptHelper.GetScript("InitializePage()"));

        // Title and breadcrumbs for product section edit
        if (ProductSection)
        {
            bool isNew = Action == "new";
            TreeNode node = DocumentManager.Node;
            string className = (node != null) ? node.ClassName : ci.ClassName;

            // Title
            CurrentMaster.Title.TitleImage = GetDocumentTypeIconUrl(className, "48x48");
            CurrentMaster.Title.TitleText = GetString(isNew ? "com.productsection.new" : "com.productsection.properties");
            CurrentMaster.Title.HelpTopicName = "new_edit_product_section";

            // Get products starting path
            string key = SettingsKeyProvider.GetFullKeyName(CurrentSiteName, "CMSStoreProductsStartingPath");
            string productsStartingPath = SettingsKeyProvider.GetStringValue(key);

            // Create and set breadcrumbs
            TreeNode breadcrumbNode = isNew ? DocumentManager.ParentNode : node;
            if (breadcrumbNode != null)
            {
                CurrentMaster.Title.Breadcrumbs = CreateBreadcrumbs(breadcrumbNode, "/" + productsStartingPath.Trim('%').Trim('/') + "%");
            }
        }

        if (!newdocument && !newculture)
        {
            formElem.Enabled = DocumentManager.AllowSave;

            imgBindSku.ImageUrl = DocumentManager.AllowSave ? GetImageUrl("Objects/Ecommerce_SKU/bind.png") : GetImageUrl("Objects/Ecommerce_SKU/binddisabled.png");
            btnBindSku.Enabled = DocumentManager.AllowSave;
        }
        if (!RequiresDialog)
        {
            // Set dialog title
            ScriptHelper.RegisterTitleScript(this, GetString("content.ui.form"));
        }

        LookupDetail enumlkpDetail;
        if (Request.QueryString["SubType"] != null)
        {
            string subType = Request.QueryString["SubType"].ToString() == "" ? "0" : Request.QueryString["SubType"].ToString();
            if (Enum.TryParse(subType, true, out enumlkpDetail))
            {
                switch (enumlkpDetail)
                {
                    case LookupDetail.CurriculumMapForm:
                        SetFormTitle = THINKGATE_INSTRUCTION_PLAN;
                          formElem.BasicForm.FieldControls["ExpirationDate"].Value = ExpiryDateValue;
                          formElem.BasicForm.FieldControls["ExpirationDate"].Enabled = true;
                        break;
                    case LookupDetail.CurriculumMapAttachmentForm:
                        SetFormTitle = THINKGATE_INSTRUCTION_PLAN_ATTACHMENT;
                        break;
                    case LookupDetail.CurriculumMapWebBasedForm:
                        SetFormTitle = THINKGATE_INSTRUCTION_PLAN_WEBBASED;
                        break;
                    case LookupDetail.UnitPlanForm:
                        SetFormTitle = THINKGATE_UNIT_PLAN;
                        break;
                    case LookupDetail.UnitPlanAttachmentForm:
                        SetFormTitle = THINKGATE_UNIT_PLAN_ATTACHMENT;
                        break;
                    case LookupDetail.UnitPlanWebBasedForm:
                        SetFormTitle = THINKGATE_UNIT_PLAN_WEBBASED;
                        break;
                    case LookupDetail.StateModelCurriculumForm:
                        SetFormTitle = THINKGATE_CURRICULUM_UNIT;
                        break;
                    case LookupDetail.StateModelCurriculumUnitForm:
                        SetFormTitle = THINKGATE_UNIT_PLAN;
                        break;
                    case LookupDetail.LessonPlanForm:
                        SetFormTitle = THINKGATE_LESSON_PLAN;
                        break;
                    case LookupDetail.LessonPlanAttachmentForm:
                        SetFormTitle = THINKGATE_LESSON_PLAN_ATTACHMENT;
                        break;
                    case LookupDetail.LessonPlanWebBasedForm:
                        SetFormTitle = THINKGATE_LESSON_PLAN_WEBBASED;
                        break;
                    case LookupDetail.ResourcePlanAttachmentForm:
                        SetFormTitle = THINKGATE_RESOURCE_PLAN_ATTACHMENT;
                        break;
                    case LookupDetail.ResourcePlanWebBasdForm:
                        SetFormTitle = THINKGATE_RESOURCE_PLAN_WEBBASED;
                        break;

                }
            }
        }

       
        if (NodeClassName == THINKGATE_RESOURCE && formElem.BasicForm.FieldControls != null)
        {


            if (Request.QueryString["Type"] != null)
            {
                formElem.BasicForm.FieldControls["Type"].Value = Request.QueryString["Type"].ToString();
                if (Convert.ToInt32(formElem.BasicForm.FieldControls["Type"].Value) > 0)
                    formElem.BasicForm.FieldControls["Type"].Enabled = false;
            }
            if (Request.QueryString["SubType"] != null)
            {
                formElem.BasicForm.FieldControls["SubType"].Value = Request.QueryString["SubType"].ToString() == "" ? "0" : Request.QueryString["SubType"].ToString();
                if (Convert.ToInt32(formElem.BasicForm.FieldControls["SubType"].Value) > 0)
                    formElem.BasicForm.FieldControls["SubType"].Enabled = false;
            }

            if (formElem.BasicForm.FieldControls["SubType"].ValueDisplayName.Contains("Attachment"))
            {
                formElem.BasicForm.FieldControls["AttachmentName"].Visible = true;
                formElem.BasicForm.FieldControls["Weblink"].Visible = false;
            }
            else if (formElem.BasicForm.FieldControls["SubType"].ValueDisplayName.Contains("Web-Based"))
            {
                formElem.BasicForm.FieldControls["Weblink"].Visible = true;
                formElem.BasicForm.FieldControls["AttachmentName"].Visible = false;
            }
            else
            {
                formElem.BasicForm.FieldControls["Weblink"].Visible = false;
                formElem.BasicForm.FieldControls["AttachmentName"].Visible = false;
            }
        }

        if (!RequestHelper.IsPostBack() && NodeClassName == THINKGATE_RESOURCE)
        {
            //SetGradeSubjectCoursesTargetsSQL();
        }

        if (NodeClassName.ToLower() == THINKGATE_COMPETENCYLIST)
        {
            SetFormTitle = THINKGATE_COMPETENCYLIST_PLAN;
        }
    }

    protected void SetGradeSubjectCoursesTargetsSQL()
    {
        string dbName = ThinkgateKenticoHelper.FindDBName();
        formElem.BasicForm.FieldControls["Courses"].FieldInfo.Settings["Query"] = "Select '0', '<Select One>' union select distinct Course,Course  from [" + dbName + "]..CurrCourses where Active = 'Yes'";
    }

    #endregion


    #region " Standard Helper Methods "

    void insertItem(int theDocumentID, string theStandardID)
    {
        CustomTableItemProvider customTableProvider = new CustomTableItemProvider(CMSContext.CurrentUser);

        // Checks if Custom table exists
        DataClassInfo customTable = DataClassInfoProvider.GetDataClass("TG.DocumentPlanAssociation");
        if (customTable != null)
        {
            CustomTableItem newCustomTableItem = CustomTableItem.New("TG.DocumentPlanAssociation", customTableProvider);

            // Sets the row data
            newCustomTableItem.SetValue("DocumentID", this.DocumentID);
            newCustomTableItem.SetValue("DocumentTypeEnum", (int)this.DocumentType);
            newCustomTableItem.SetValue("UserID", Guid.Empty);
            newCustomTableItem.SetValue("AssociationCategoryEnum", (int)AssociationCategory.Standard);
            newCustomTableItem.SetValue("AssociationID", theStandardID);
            newCustomTableItem.SetValue("ParentDocumentID", string.Empty);

            // Inserts the item into database
            newCustomTableItem.Insert();
        }
    }

    DataTable GetDocumentStandards(int documentID)
    {
        string query = "SELECT k.AssociationID FROM [Elements].[dbo].[Standards] as e, [Kentico7].[dbo].[TG_DocumentPlanAssociation] as k where k.AssociationID = e.ID and k.DocumentID  = " + documentID;
        DataSet dataSet = CMS.DataEngine.ConnectionHelper.ExecuteQuery(query, (new QueryDataParameters()), QueryTypeEnum.SQLQuery, false);
        return dataSet.Tables[0];
    }

    /// <summary>
    /// Adds and maps the standards to the current document. Standards are parsed out from the Description field (Html) of the document.
    /// </summary>
    /// <param name="savedDocumentTreeNode"></param>
    void AddStandardsToDocument(TreeNode savedDocumentTreeNode)
    {
        if (savedDocumentTreeNode == null)
            return;
        try
        {
            /* Retrieve the html of the description field of current saved document. */
            string documentHtml = savedDocumentTreeNode.GetValue("Description").ToString();
            if (string.IsNullOrWhiteSpace(documentHtml))
            {
                return;
            }
            else if (!documentHtml.Contains("standardid"))
            {
                return;
            }
            int documentID = TreeHelper.SelectSingleNode(savedDocumentTreeNode.NodeID).DocumentID;

            /* Do a string parse of the Html and split out standards from it. */
            string[] splits = System.Text.RegularExpressions.Regex.Split(documentHtml, "standardid=");
            if (splits != null && splits.Length > 0)
            {
                List<int> standards = new List<int>();
                for (int i = 0; i < splits.Length; i++)
                {
                    int standardid = 0;
                    string id = splits[i].Substring(1, splits[i].IndexOf("\"", 1) - 1);
                    int.TryParse(id, out standardid);
                    if (standardid > 0)
                    {
                        standards.Add(standardid);
                    }
                }
                /* We now have standards that have been put into description. Start adding now. */
                if (standards.Count > 0)
                {
                    DataTable alreadySavedStandards = GetDocumentStandards(documentID);
                    foreach (int standard in standards)
                    {
                        /* Check if standard is already added/ mapped for current document. */
                        DataRow[] dr = alreadySavedStandards.Select("standardID = " + standard);
                        if (!string.IsNullOrEmpty(standard.ToString()) && dr.Length <= 0)
                        {
                            /* Not added, this should be added. */
                            insertItem(documentID, standard.ToString());
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            throw new ApplicationException("Error saving standards for document!", ex);
        }
    }

    #endregion

    #region "Document manager events"

    protected void DocumentManager_OnAfterAction(object sender, DocumentManagerEventArgs e)
    {
        TreeNode node = e.Node;

        /* Adds the selected standards to this document */
        //AddStandardsToDocument(node);

        int newNodeId = node.NodeID;
        if (newdocument || newculture)
        {
            // Store error text
            if (!string.IsNullOrEmpty(formElem.MessagesPlaceHolder.ErrorText))
            {
                SessionHelper.SetValue("FormErrorText|" + newNodeId, formElem.MessagesPlaceHolder.ErrorText);
            }
        }
        else
        {
            // Reload the form
            formElem.LoadForm(true);
        }

        // If not menu item type nor EditLive view mode, switch to form mode to keep editing the form
        if (!TreePathUtils.IsMenuItemType(node.NodeClassName) && (CMSContext.ViewMode != ViewModeEnum.EditLive))
        {
            CMSContext.ViewMode = ViewModeEnum.EditForm;
        }

        if (NodeClassName.ToLower() == THINKGATE_COMPETENCYLIST)
        {
            ThinkgateKenticoHelper.UpdateDocumentTypeItem(newNodeId);
        }
        if (NodeClassName.ToLower() == THINKGATE_INSTRUCTIONPLAN)
        {
            ThinkgateKenticoHelper.UpdateDocumentTypeItem(newNodeId);
        }
    }

    protected void DocumentManager_OnLoadData(object sender, DocumentManagerEventArgs e)
    {
        formElem.BasicForm.LoadControlValues();
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Adds script for redirecting to NotAllowed page.
    /// </summary>
    /// <param name="action">Action string</param>
    private void AddNotAllowedScript(string action)
    {
        AddScript("window.location.replace('../NotAllowed.aspx?action=" + action + "')");
    }


    /// <summary>
    /// Adds the script to the output request window.
    /// </summary>
    /// <param name="script">Script to add</param>
    public override void AddScript(string script)
    {
        ScriptHelper.RegisterStartupScript(this, typeof(string), script.GetHashCode().ToString(), ScriptHelper.GetScript(script));
    }


    protected void btnRefresh_Click(object sender, EventArgs e)
    {
        TreeNode node = DocumentManager.Node;

        // Check permission to modify document
        if (CMSContext.CurrentUser.IsAuthorizedPerDocument(node, NodePermissionsEnum.Modify) == AuthorizationResultEnum.Allowed)
        {
            // Ensure version for later detection whether node is published
            node.VersionManager.EnsureVersion(node, node.IsPublished);

            // Tree refresh is needed only if node was archived or published
            WorkflowStepInfo currentStep = node.WorkflowStep;
            bool refreshTree = (currentStep != null) && (currentStep.StepIsArchived || currentStep.StepIsPublished);

            // Move to edit step
            node.MoveToFirstStep(null);

            // Refresh tree
            if (refreshTree)
            {
                ScriptHelper.RefreshTree(this, node.NodeID, node.NodeID);
            }

            // Reload form
            formElem.LoadForm(true);

            if (DocumentManager.SaveChanges)
            {
                ScriptHelper.RegisterStartupScript(this, typeof(string), "moveToEditStepChange", ScriptHelper.GetScript("Changed();"));
            }
        }
    }


    protected void formElem_OnAfterDataLoad(object sender, EventArgs e)
    {
        TreeNode node = DocumentManager.Node;
        if (node != null)
        {
            // Show stored error message
            string frmError = SessionHelper.GetValue("FormErrorText|" + node.NodeID) as string;
            if (!string.IsNullOrEmpty(frmError))
            {
                formElem.ShowError(frmError);
                // Remove error message
                SessionHelper.Remove("FormErrorText|" + node.NodeID);
            }
        }
    }


    private void InitBindSkuAction()
    {
        if (RequiresDialog)
        {
            return;
        }

        if ((DocumentManager.NodeID > 0) && (DocumentManager.Node != null) && (Action != "newculture"))
        {
            var dataClass = DataClassInfoProvider.GetDataClass(DocumentManager.Node.ClassName);
            if ((dataClass != null) && ModuleEntry.IsModuleLoaded(ModuleEntry.ECOMMERCE))
            {
                plcSkuBinding.Visible = dataClass.ClassIsProduct;

                btnBindSku.Click += (sender, args) =>
                {
                    string url = "~/CMSModules/Ecommerce/Pages/Content/Product/Product_Edit_General.aspx";
                    url = URLHelper.AddParameterToUrl(url, "nodeid", DocumentManager.NodeID.ToString());
                    url = URLHelper.AddParameterToUrl(url, "action", "bindsku");
                    if (RequiresDialog)
                    {
                        url = URLHelper.AddParameterToUrl(url, "dialog", "1");
                    }
                    Response.Redirect(url);
                };
            }
        }
    }


    /// <summary>
    /// Creates breadcrumbs array leading to specified node using given tree provider and starting at given starting path.
    /// </summary>
    /// <param name="node">Tree node to generate breadcrumbs for.</param>
    /// <param name="startingPath">Path where to start to generate.</param>
    protected virtual string[,] CreateBreadcrumbs(TreeNode node, string startingPath)
    {
        string[,] breadcrumbs = null;

        if (node != null)
        {
            const string columns = "NodeID, NodeAliasPath, NodeSiteID, NodeOwner, DocumentName, DocumentLastVersionName, DocumentMenuCaption, DocumentCulture, SiteName, ClassName, NodeACLID";

            // Prepare the where condition
            string where = SqlHelperClass.AddWhereCondition(null, TreeProvider.GetNodesOnPathWhereCondition(node.NodeAliasPath, true, true));
            DataSet ds = DocumentHelper.GetDocuments(CurrentSiteName, startingPath, TreeProvider.ALL_CULTURES, true, null, where, "NodeAliasPath ASC", -1, false, 0, columns, node.TreeProvider);

            ds = TreeSecurityProvider.FilterDataSetByPermissions(ds, NodePermissionsEnum.Read, CMSContext.CurrentUser);

            if (!DataHelper.DataSourceIsEmpty(ds))
            {
                // Initialize resolver
                ContextResolver currentResolver = CMSContext.CurrentResolver.CreateContextChild();
                currentResolver.EncodeResolvedValues = true;

                DataRow[] resolverData = new DataRow[1];
                DataTable table = ds.Tables[0];

                // Prepare breadcrumbs
                breadcrumbs = new string[table.Rows.Count + 1, 4];

                int i = 0;

                foreach (DataRow dr in table.Rows)
                {
                    // Add current datarow to the resolver
                    resolverData[0] = dr;
                    currentResolver.SourceData = resolverData;

                    // Prepare the item name. Disable encoding.
                    currentResolver.EncodeResolvedValues = false;
                    string linkName = currentResolver.ResolveMacros(TreePathUtils.GetMenuCaption(ValidationHelper.GetString(dr["DocumentMenuCaption"], String.Empty), ValidationHelper.GetString(dr["DocumentName"], String.Empty)));
                    currentResolver.EncodeResolvedValues = true;

                    // Use site name for root node
                    linkName = string.IsNullOrEmpty(linkName) ? CMSContext.CurrentSite.DisplayName : linkName;

                    // Create breadcrumb
                    breadcrumbs[i, 0] = HTMLHelper.HTMLEncode(linkName);
                    breadcrumbs[i, 1] = "~/CMSModules/Ecommerce/Pages/Tools/Products/Product_List.aspx?nodeid=" + dr["NodeID"];
                    breadcrumbs[i, 3] = string.Format("EditDocument({0}); RefreshTree({0},{0});", dr["NodeID"]);

                    // Increment index
                    i++;
                }

                // Add 'properties' breadcrumb
                breadcrumbs[i, 0] = HTMLHelper.HTMLEncode(GetString((Action == "new") ? "com.productsection.new" : "com.productsection.properties"));
                breadcrumbs[i, 1] = "";
                breadcrumbs[i, 2] = "";
            }
        }

        return breadcrumbs;
    }

    #endregion
}