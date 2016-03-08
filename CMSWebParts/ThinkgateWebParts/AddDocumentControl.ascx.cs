using System.Activities.Statements;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using CMS.SettingsProvider;
using System;
using CMS.CMSHelper;
using System.Data;
using CMS.SiteProvider;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Spreadsheet;


public partial class CMSWebParts_ThinkgateWebParts_AddDocumentControl : System.Web.UI.UserControl
{
    #region Public Properties

    public string DocumentType { get; set; }
    public string DocumentID { get; set; }
    public string DocumentTypeLabel { get; set; }
    public string ClassID { get; set; }
    public string AddNewLevelLabel;

    #endregion

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            int nodeId = Convert.ToInt32(DocumentID);

            if (nodeId > 0)
            {
                CMS.DocumentEngine.TreeNode treenode = TreeHelper.SelectSingleNode(nodeId);

                bool createAllowed = treenode.CheckPermissions(PermissionsEnum.Create, CMSContext.CurrentSiteName, CMSContext.CurrentUser);
                bool modifyAllowed = treenode.CheckPermissions(PermissionsEnum.Modify, CMSContext.CurrentSiteName, CMSContext.CurrentUser);

                //check user permission for "Create" or "Modify" access, if allowed then show the "add new" area
                //if (TreeSecurityProvider.IsAuthorizedPerNode(treenode, NodePermissionsEnum.Create, currentUser) == AuthorizationResultEnum.Allowed || TreeSecurityProvider.IsAuthorizedPerNode(treenode, NodePermissionsEnum.Modify, currentUser) == AuthorizationResultEnum.Allowed)
                if (createAllowed || modifyAllowed)
                {
                    showAddNew.Visible = true;
                }
                else
                {
                    showAddNew.Visible = false;
                }
            }
            else
            {
                showAddNew.Visible = false;
            }

            DocumentTypeLabel = String.Empty;
            if (DocumentType != null)
            {
                if (DocumentType.ToLower() == "curriculumunit")
                {
                    AddNewLevelLabel = "Level";
                    DocumentTypeLabel = "Model Curriculum Level";
                }
                else
                {
                    int idx = GetDocumentTypeSuffixIndex();

                    if (idx > 0)
                    {

                        DocumentTypeLabel = DocumentType.Substring(0, (idx)) + " " + DocumentType.Substring(idx);
                    }
                    else
                    {

                        DocumentTypeLabel = DocumentType;
                    }
                    AddNewLevelLabel = DocumentTypeLabel;
                }
                DocumentTypeLabel = UppercaseFirst(DocumentTypeLabel);
                DocumentType = "thinkgate." + DocumentType;
                DataSet ds = ThinkgateKenticoHelper.GetTileMapLookupDataSet(DocumentType);

                if (ds.Tables[0].Rows.Count == 1)
                {
                    string resourceToShow = ds.Tables[0].Rows[0]["KenticoDocumentTypeToShow"].ToString();

                    DataClassInfo dci = new DataClassInfo();
                    dci = DataClassInfoProvider.GetDataClass(resourceToShow);
                    ClassID = dci.ClassID.ToString();
                    
                }
                else
                {
                    if (ds.Tables[0].Rows.Count > 1)
                    {
                        System.Text.StringBuilder sb = new System.Text.StringBuilder();
                        bool isFirstRow = true;
                        string buildRadioButtons = string.Empty;
                        foreach (DataRow dr in ds.Tables[0].Rows)
                        {
                            
                            if (isFirstRow)
                            {
                                isFirstRow = false;
                                buildRadioButtons = "<input type=\"radio\" name=\"" + ID + "-" +
                                                           DocumentID + "-formType\" value=\"" + dr["FriendlyName"] +
                                                           " \" checked /> " + dr["FriendlyName"] + "<br  />";
                            }
                            else
                            {
                                buildRadioButtons = "<input type=\"radio\" name=\"" + ID + "-" +
                                                          DocumentID + "-formType\" value=\"" + dr["FriendlyName"] +
                                                          " \"/> " + dr["FriendlyName"] + "<br  />";
                            }
                            sb.Append(buildRadioButtons);
                        }
                        divFormTypeSelectionRadioBtns.InnerHtml = sb.ToString();
                        ClassID = null;
                    }
                    else
                    {
                        throw new SystemException("Invalid Document Type");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            DocumentTypeLabel = "Unknown Document Type";
        }

    }

   

    private int GetDocumentTypeSuffixIndex()
    {
        int idx = DocumentType.IndexOf("Plan");
        if (idx < 2)
        {
            idx = DocumentType.IndexOf("Unit");
        }
        return idx;
    }

    private string UppercaseFirst(string s)
    {
        if (string.IsNullOrEmpty(s))
        {
            return string.Empty;
        }
        char[] a = s.ToCharArray();
        a[0] = char.ToUpper(a[0]);
        return new string(a);
    }


}