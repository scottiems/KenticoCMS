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
using System.Collections.Generic;
using CMS.SiteProvider;
using HtmlAgilityPack;
using CMS.IO;



public partial class CMSModules_Content_CMSDesk_Edit_MessageCenterEdit : CMSContentPage
{
    

    #region "Variables"

    protected bool newdocument = false;
    protected bool newculture = false;
    protected bool mShowToolbar = false;
    protected string clientName;
    protected string scrubbedClientName;
    protected string stateInitial;

    protected DataClassInfo ci = null;

    protected string CleintTargetRoleName = System.Configuration.ConfigurationManager.AppSettings["CMDClientTargetRoleName"].ToString();
    protected string UserAgreementtRoleName = System.Configuration.ConfigurationManager.AppSettings["CMDUserAgreementRoleName"].ToString();

    #endregion

    public string RoleAssigned = "0";

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
        if (!string.IsNullOrEmpty(Request.QueryString["clientName"]))
        {
        clientName = Request.QueryString["clientName"].ToString().Replace("'", "''");
        scrubbedClientName = clientName.Replace(" ", "");
        }
        else
        {
            throw new Exception("ClientID not found.");
        }
           
        if (!string.IsNullOrEmpty(Request.QueryString["State"]))
        {
            stateInitial = Request.QueryString["State"].ToString().Replace(" ", "");
            if (stateInitial.Length != 2)
            {
                throw new Exception("Invalid State Code.");
            }
        }

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
                        //SetTitle("CMSModules/CMS_Content/Menu/New.png", GetString("Content.NewTitle"), null, null);
                        SetTitle("CMSModules/CMS_Content/Menu/New.png", "Add Message", null, null);
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
                        //SetTitle("CMSModules/CMS_Content/Menu/New.png", GetString("content.newcultureversiontitle"), null, null);
                        SetTitle("CMSModules/CMS_Content/Menu/New.png", "Add Message", null, null);
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

                            //SetTitle("CMSModules/CMS_Content/Menu/New.png", GetString("Content.EditTitle") + " \"" + HTMLHelper.HTMLEncode(ResHelper.LocalizeString(nodeName)) + "\"", null, null);
                            SetTitle("CMSModules/CMS_Content/Menu/New.png", "Add Message", null, null);
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


        //RMessage Center Document
        if (System.Configuration.ConfigurationManager.AppSettings["ThinkgateMessageCenter"] != null)
        {
            if (System.Configuration.ConfigurationManager.AppSettings["ThinkgateMessageCenter"] == formElem.DocumentManager.ParentNode.DocumentName.ToString().ToUpper())
            {
                formElem.BasicForm.OnAfterValidate += BasicForm_OnAfterValidate;

                formElem.BasicForm.OnBeforeValidate += BasicForm_OnBeforeValidate;


                formElem.BasicForm.OnBeforeSave += BasicForm_OnBeforeSave;




                //ClientTarget Role validation
                if (User.IsInRole(CleintTargetRoleName))
                {
                    if (formElem.BasicForm.FieldControls != null)
                        formElem.BasicForm.FieldControls["ClientTargets"].FieldInfo.Visible = true;
                }
                else
                {
                    if (formElem.BasicForm.FieldControls != null)
                        formElem.BasicForm.FieldControls["ClientTargets"].FieldInfo.Visible = false;
                }



            }
        }
        Session["CurrentTreeNodeID"] = string.IsNullOrWhiteSpace(Request.QueryString.Get("nodeid")) ? 0 : Convert.ToInt64(Request.QueryString.Get("nodeid"));







    }



    protected override void OnPreRender(EventArgs e)
    {

        if (!RequestHelper.IsPostBack())
        {
            if (User.IsInRole(UserAgreementtRoleName) == false)
            {
                if (formElem.BasicForm.FieldControls != null)
                {
                    formElem.BasicForm.FieldControls["MessageCenterEnum"].FieldInfo.Settings["Query"] = "select 0, '<Select One>' union select Enum,Description from tg_lookupdetails where lookupenum=2 and enum!=5";
                    formElem.BasicForm.ReloadData();
                }
            }
            SetClientTargetsSQL();//TFS Task# 2469
        }

        if (this.Action == "" && newdocument == false && this.DocumentManager.CreateAnother == false)//TFS Task# 2469
        { SetClientTargetsSQL(); }//TFS Task# 2469

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
            CurrentMaster.Title.TitleText = "Add Message"; // GetString(isNew ? "com.productsection.new" : "com.productsection.properties");
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



        //Message Center Document to disable Title field in EditMode
        if (System.Configuration.ConfigurationManager.AppSettings["ThinkgateMessageCenter"] != null)
        {
            if (System.Configuration.ConfigurationManager.AppSettings["ThinkgateMessageCenter"] == formElem.DocumentManager.ParentNode.DocumentName.ToString().ToUpper())
            {

                if (CMSContext.ViewMode == ViewModeEnum.EditForm)
                {

                    if (Request.QueryString.Get("nodeid") == null)
                    {
                        if (formElem.BasicForm.FieldControls != null)
                        {
                            formElem.BasicForm.FieldControls["Title"].Enabled = true;
                        }
                    }
                    else
                    {
                        if (formElem.BasicForm.FieldControls != null)
                        {
                            formElem.BasicForm.FieldControls["Title"].Enabled = false;
                        }
                    }



                }
                if (formElem.BasicForm.FieldControls != null)
                {
                    if (formElem.BasicForm.FieldControls["MessageCenterEnum"].Value.ToString() != "0")
                    {
                        formElem.BasicForm.FieldControls["MessageCenterEnum"].Enabled = false;
                    }

                }

            }
        }
        if (Request.QueryString.Get("nodeid") == null)
        {
            CurrentMaster.Title.TitleText = "Add Message";
        }
        else
        {
            CurrentMaster.Title.TitleText = "Edit Message";
        }



        //if (User.IsInRole(UserAgreementtRoleName) == false)
        //{
        //    RoleAssigned = "1";



        //    if (formElem.BasicForm.FieldControls != null)
        //    {
        //      // CMSFormControls_Basic_DropDownListControl





        //        if ( formElem.BasicForm.FieldControls["MessageCenterEnum"].Value != "0")
        //        {

        //            string script = " function RemoveUserAgreement() { var ctrl = document.getElementById('m_c_f_f_MessageCenterEnum_dropDownList'); for (i = 0; i < ctrl.length; i++) {  ctrl.remove(i); } } }";

        //            ClientScript.RegisterClientScriptBlock(this.GetType(), "R", script, true);
        //        }
        //    }


        //}


    }


    #endregion

    #region " Standard Helper Methods "

    void insertItem(int theDocumentID, string theStandardID)
    {
        CustomTableItemProvider customTableProvider = new CustomTableItemProvider(CMSContext.CurrentUser);

        // Checks if Custom table exists
        DataClassInfo customTable = DataClassInfoProvider.GetDataClass("thinkgate.docToStandards");
        if (customTable != null)
        {
            CustomTableItem newCustomTableItem = CustomTableItem.New("thinkgate.docToStandards", customTableProvider);

            newCustomTableItem.SetValue("docID", theDocumentID.ToString());
            newCustomTableItem.SetValue("standardID", theStandardID);

            // Inserts the item into database
            newCustomTableItem.Insert();
        }
    }

    DataTable GetDocumentStandards(int documentID)
    {
        string query = "SELECT k.standardID FROM [VersaITBaseTM" + scrubbedClientName + "].[dbo].[Standards] as e, [Kentico7].[dbo].[thinkgate_docToStandards] as k where k.standardID = e.ID and k.docId = " + documentID;
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
            //LogContext.AppendLine("Error saving standards for document!");
            throw new ApplicationException("Error saving standards for document!", ex);
        }
    }

    #endregion

    #region "Document manager events"

    protected void DocumentManager_OnAfterAction(object sender, DocumentManagerEventArgs e)
    {
        TreeNode node = e.Node;

        /* Add custom code. */
        node.SetValue("DateUpdated", DateTime.Now);
        DocumentManager.UpdateDocument(false);

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

        /* Adds the selected standards to this document. This may raise exeption, so must be the last. */
        AddStandardsToDocument(node);

        //Redirect form to edit mode
        if (this.Action.Equals("new", StringComparison.InvariantCultureIgnoreCase) && newdocument == true && DocumentManager.CreateAnother == false)
        {
            Response.Redirect(Request.ApplicationPath + "/CMSModules/Thinkgate/MessageCenter/MessageCenterEdit.aspx?nodeid=" + node.NodeID + "&culture=" + CMSContext.PreferredCultureCode + "&clientName=" + scrubbedClientName +"&State="+stateInitial, true);
        }
    }


    protected void DocumentManager_OnLoadData(object sender, DocumentManagerEventArgs e)
    {
        formElem.BasicForm.LoadControlValues();
    }

    #endregion

    #region "Methods"

    protected void SetClientTargetsSQL()//TFS Task# 2469
    {
        CurrentUserInfo currentUser = CMSContext.CurrentUser;
        if (formElem.BasicForm.FieldControls != null)
        {
            if (currentUser.IsInRole(scrubbedClientName + "-Admin-Super_Admin", CMSContext.CurrentSiteName))
            {
                formElem.BasicForm.FieldControls["ClientTargets"].FieldInfo.Settings["Query"] = "select 'All' db, 'All' dbname UNION ALL select  REPLACE(name,'VersaITBaseTM','') as db, REPLACE(name,'VersaITBaseTM','') as dbname from master.sys.databases where name like 'VersaITBaseTM%' and name not like '%Content%' and name not like '%Demo'and name not like '%UAT'";
                formElem.BasicForm.ReloadData();
            }
            else if (!string.IsNullOrEmpty(stateInitial))
        {
                formElem.BasicForm.FieldControls["ClientTargets"].FieldInfo.Settings["Query"] = "select 'All' db, 'All' dbname UNION ALL select REPLACE(name,'VersaITBaseTM','') as db, REPLACE(name,'VersaITBaseTM','') as dbname from master.sys.databases where name like 'VersaITBaseTM" + stateInitial + "%' and name not like '%Content%' and name not like '%Demo'and name not like '%UAT'";
                formElem.BasicForm.ReloadData();
            }
            else
            {
                formElem.BasicForm.FieldControls["ClientTargets"].FieldInfo.Settings["Query"] = "select REPLACE(name,'VersaITBaseTM','') as db, REPLACE(name,'VersaITBaseTM','') as dbname from master.sys.databases where name = 'VersaITBaseTM" + scrubbedClientName + "'";
                formElem.BasicForm.ReloadData();
            }
        }
    }

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


    protected void BasicForm_OnBeforeSave(object sender, EventArgs e)
    {
        if (Session["CurrentTreeNodeID"] != null)
        {
            int documentNodeID = (Convert.ToInt32(Session["CurrentTreeNodeID"].ToString()));
            if (documentNodeID > 0)
            {
                TreeNode node = TreeHelper.SelectSingleNode(documentNodeID);
                if (node != null)
                {
                    node.SetValue("DateUpdated", DateTime.Now);
                }
            }
        }
    }

    protected void BasicForm_OnBeforeValidate(object sender, EventArgs e)
    {
        //Message Center Validation
        if (System.Configuration.ConfigurationManager.AppSettings["ThinkgateMessageCenter"] != null)
        {
            if (System.Configuration.ConfigurationManager.AppSettings["ThinkgateMessageCenter"] == formElem.DocumentManager.ParentNode.DocumentName.ToString().ToUpper())
            {
                if (formElem.BasicForm.FieldControls != null)
                {
                    if (formElem.BasicForm.FieldControls["MessageCenterEnum"].Value.ToString() == "0")
                    {
                        RaiseError("MessageCenterEnum", "<br/><br/><b>Type<span style='color: rgb(255, 0, 0);'>* </span>: </b>Please Select Type.", formElem.BasicForm);

                        formElem.BasicForm.StopProcessing = true;

                    }
                }
            }
        }
    }


    protected void BasicForm_OnAfterValidate(object sender, EventArgs e)
    {
        //Message Center Validation
        if (System.Configuration.ConfigurationManager.AppSettings["ThinkgateMessageCenter"] != null)
        {
            if (System.Configuration.ConfigurationManager.AppSettings["ThinkgateMessageCenter"] == formElem.DocumentManager.ParentNode.DocumentName.ToString().ToUpper())
            {

                //UserGroup Validation Check
                if (formElem.BasicForm.FieldControls["UserGroupEnum"].Value.ToString() == "0")
                {
                    RaiseError("UserGroupEnum", "<br/><br/><b>User Group<span style='color: rgb(255, 0, 0);'>* </span>: </b>Please Select UserGroup", formElem.BasicForm);

                    formElem.BasicForm.StopProcessing = true;
                }

                if (formElem.BasicForm.FieldControls["poston"].Value != null)
                {

                    if (DateTime.Parse(formElem.BasicForm.FieldControls["PostOn"].Value.ToString()).Date < DateTime.Today)
                    {
                        RaiseError("PostOn", "<br/><br/><b>Post On: </b> Post On date should be greater than or equal to current date.", formElem.BasicForm);

                        formElem.BasicForm.StopProcessing = true;
                    }
                }
                bool iserror = false;
                if (formElem.BasicForm.FieldControls["removeon"].Value != null)
                {
                    if (formElem.BasicForm.FieldControls["poston"].Value != null)
                    {

                        if (DateTime.Parse(formElem.BasicForm.FieldControls["PostOn"].Value.ToString()).Date >= DateTime.Parse(formElem.BasicForm.FieldControls["RemoveOn"].Value.ToString()).Date)
                        {
                            RaiseError("poston", "<br/><br/><b>Remove On: </b> Remove On date should be greater than Post On date.", formElem.BasicForm);
                            iserror = true;
                            formElem.BasicForm.StopProcessing = true;

                        }


                    }
                }
                if (!iserror)
                {
                    if (formElem.BasicForm.FieldControls["removeon"].Value != null)
                    {
                        if (DateTime.Parse(formElem.BasicForm.FieldControls["Removeon"].Value.ToString()).Date <= DateTime.Today)
                        {
                            RaiseError("Removeon", "<br/><br/><b>Remove On: </b> Remove On date should be greater than current date.", formElem.BasicForm);

                            formElem.BasicForm.StopProcessing = true;
                        }
                    }
                }


                if (formElem.BasicForm.FieldControls != null)
                {
                    // formElem.BasicForm.FieldControls["ClientTargets"].FieldInfo.Visible = true;

                    if (formElem.BasicForm.FieldControls["ClientTargets"].Value.ToString() == "")
                    {
                        RaiseError("ClientTargets", "<br/><br/><b>Client Targets<span style='color: rgb(255, 0, 0);'>* </span>: </b>At least one target must be selected before the message can be saved", formElem.BasicForm);

                        formElem.BasicForm.StopProcessing = true;
                    }

                }




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

}
