using CMS.CMSHelper;
using CMS.PortalControls;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

public partial class CMSWebParts_ThinkgateWebParts_Resources : CMSAbstractWebPart
{     
    private string rootConnectionString = ConfigurationManager.ConnectionStrings["root_application"].ConnectionString;


    public int AssociationCount;   

    public string DocumentType;
    public AssociationCategory Category;

    public string E3RootURL
    {
        get
        {
            return ConfigurationManager.AppSettings["E3RootURL"] + ThinkgateKenticoHelper.getClientIDFromKenticoUserName(CMSContext.CurrentUser.UserName); 
        }
    }
    
    DataAccess dataAccess;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(Request.QueryString["documenttype"]))
        {
            this.DocumentType = Request.QueryString["documenttype"];
        }
        string CalledByToolBar = string.Empty;
        if(!string.IsNullOrEmpty(Request.QueryString["FromToolBar"]))
            CalledByToolBar = Request.QueryString["FromToolBar"].ToString();
        
        if (!IsPostBack)
        {
            if (!string.IsNullOrEmpty(CalledByToolBar)) //means calls from Association Toolbar.
            {
                btnAddAsHyperlink.Visible = false;
                btnAddAsContent.Visible = false;
                btnCloseCKEditor.Visible = false;
                btnClose.Visible = true;
                btnSave.Visible = true;             
            }
            else
            {
                btnAddAsHyperlink.Visible = true;
                btnAddAsContent.Visible = true;
                btnCloseCKEditor.Visible = true;
                btnClose.Visible = false;
                btnSave.Visible = false;              
            }
            CountTotalResources();
            BindControls();           
        }
    }
    protected void CountTotalResources()
    {        
        int docID = GetDocumentID();
        KenticoDBHelper kenticoDBHelper = new KenticoDBHelper();
        if (!string.IsNullOrWhiteSpace(Request.QueryString.Get("category")))
        {
            this.Category = (AssociationCategory)Enum.Parse(typeof(AssociationCategory), Request.QueryString.Get("category"));
            
        }
        DocType documentType = (DocType)Enum.Parse(typeof(DocType), this.DocumentType);
        DataSet dsKentico = kenticoDBHelper.GetDocToResources(docID, (int)Category, (int)documentType);
        if (dsKentico.Tables[0].Rows.Count != null)
        {
            AssociationCount = dsKentico.Tables[0].Rows.Count;
        }       
    }
    public void BindDataToGrid()
    {
        string category = rcbCategory.SelectedValue;
        string catType = rcbType.SelectedValue;
        string subType = rcbSubType.SelectedValue;
        string resourceName = rtbResourceName.Text.Trim();
        string textSearch = rtbTextSearch.Text.Trim();
        string textSearchOption = radSearchOption.SelectedValue;
        dataAccess = new DataAccess(rootConnectionString);
        DataSet Ds = dataAccess.GetResourceList("E3_Resources_Search_KenticoAdvanced", category, catType, subType, resourceName, textSearch, textSearchOption);              
        radGridResults.DataSource = GetResources(Ds);       
        radGridResults.DataBind();        
    }
    
    protected DataTable GetResources(DataSet e3DSResource)
    {       
        DataTable dtResult = new DataTable();
        if (e3DSResource != null)
        {            
            dtResult = e3DSResource.Tables[0];
            KenticoDBHelper kenticoDBHelper = new KenticoDBHelper();
            int docID = GetDocumentID();

            if (!string.IsNullOrWhiteSpace(Request.QueryString.Get("documentType")))
            {
                DocumentType = Request.QueryString.Get("documentType");
            }

            if (!string.IsNullOrWhiteSpace(Request.QueryString.Get("category")))
            {
                this.Category = (AssociationCategory)Enum.Parse(typeof(AssociationCategory), Request.QueryString.Get("category"));
            }
            //--Need to delete bellow two lines when this page integrates with Ad Resource custom dialog--used for testing pupose
            //Category = string.IsNullOrEmpty(Category) ? "0" : Category;
           // DocumentType = string.IsNullOrEmpty(DocumentType) ? "8" : DocumentType;
            DocType documentType = (DocType)Enum.Parse(typeof(DocType), this.DocumentType);
            DataSet dsKentico = kenticoDBHelper.GetDocToResources(docID, (int)Category, (int)documentType);

            //total resource/association count set from Kentico DB.
            AssociationCount = dsKentico.Tables[0].Rows.Count;
            
            if (dsKentico.Tables[0].Rows.Count >0)
            {
                for (int i = 0; i < e3DSResource.Tables[0].Rows.Count; i++)
                {
                    if (dsKentico.Tables[0].Select("AssociationID ='" + e3DSResource.Tables[0].Rows[i]["ID"].ToString() + "'").Count() > 0)
                    {
                        dtResult.Rows[i].Delete();
                    }
                }
            }
        }

        return dtResult;
    }

    protected DataTable GetResourceFromKenticoDB()
    {
        DataTable dt=null;        
        return dt;
    }

    protected void RadGrid_NeedDataSource(object sender, EventArgs e)
    {
    }
    protected void radGridResults_ItemDataBound(object sender, GridItemEventArgs e)
    {
        if (e.Item is GridDataItem)
        {
            var item = (GridDataItem)e.Item;
            var hlName = (System.Web.UI.HtmlControls.HtmlAnchor)item.FindControl("hlName");
            var lblName = (System.Web.UI.HtmlControls.HtmlGenericControl)item.FindControl("lblName");           
        }
    }
    protected void RadGridResults_PageIndexChanged(object sender, Telerik.Web.UI.GridPageChangedEventArgs e)
    {
        BindDataToGrid();
    }
    protected void RadButtonSearch_Click(object sender, EventArgs e)
    {
        BindDataToGrid();
    }   
   
    private void BindControls()
    {
        dataAccess = new DataAccess(rootConnectionString);

        string ProcName = "E3_GetResourceSelectionTypes";
        DataSet Ds =dataAccess.GetDataSet (ProcName);
        if (Ds.Tables[0].Rows.Count > 0)
        {
            // Bind Category          
            rcbCategory.DataTextField = "Category";
            rcbCategory.DataValueField = "Category";
            rcbCategory.DataSource = Ds.Tables[0];
            rcbCategory.DataBind();
        }      
        if (Ds.Tables[1].Rows.Count > 0)
        {
            // Bind Types           
            rcbType.DataTextField = "TYPE";
            rcbType.DataValueField = "TYPE";
            rcbType.DataSource = Ds.Tables[1];
            rcbType.DataBind();
        }
       
        if (Ds.Tables[2].Rows.Count > 0)
        {
            // Bind Sub Types           
            rcbSubType.DataTextField = "SubType";
            rcbSubType.DataValueField = "SubType";
            rcbSubType.DataSource = Ds.Tables[2];
            rcbSubType.DataBind();
        }
        //Bind Text search option
        radSearchOption.DataTextField = "Name";
        radSearchOption.DataValueField = "Value";
        radSearchOption.DataSource = DataAccess.TextSearchDropdownValues();
        radSearchOption.DataBind();        
    }

    //Similar code from newDTS.aspx.cs
    private int GetDocumentID()
    {
        int theDocumentID = 1000; //hard coded need to cahnge when integrate with instruction plan
        if (CMSContext.CurrentDocument != null)
        {
            theDocumentID = TreeHelper.SelectSingleNode(CMSContext.CurrentDocument.NodeID).DocumentID;
        }
        else
        {
            if (Session["CurrentTreeNodeID"] != null && Convert.ToInt64(Session["CurrentTreeNodeID"].ToString()) > 0)
            {
                theDocumentID = TreeHelper.SelectSingleNode(Convert.ToInt32(Session["CurrentTreeNodeID"])).DocumentID;
            }
        }
        return theDocumentID;
    }


    protected void radButtonSave_Click(object sender, EventArgs e)
    {
        string docId = string.Empty;
        string docName = string.Empty;
        KenticoDBHelper kenticoDbHelper = new KenticoDBHelper();

       
           DocType documentType = (DocType)Enum.Parse(typeof(DocType), this.DocumentType);
            int associationCategory= (int)AssociationCategory.Resource;
            int docType = (int)documentType ;
          
            List<ResourceDocument> documents = new List<ResourceDocument>();
            ResourceDocument doc;
            for (int i = 0; i < radGridResults.Items.Count; i++)
            {
                CheckBox chkItem = (CheckBox)radGridResults.Items[i].FindControl("chkSelect");
                if (chkItem.Checked)
                {
                    doc = new ResourceDocument();
                    HiddenField hdnDocID = (HiddenField)radGridResults.Items[i].FindControl("hdnDocID");
                    Label lblResourceName = (Label)radGridResults.Items[i].FindControl("lblResourceName");

                    //doc.UserID = new Guid(); ; // Which user need to assign here is not clear. may be logged in user. need to assign user ID
                    doc.DocumentTypeEnum = docType;
                    doc.AssociationCategoryEnum = associationCategory;

                    doc.ResourceID = hdnDocID.Value;
                    doc.DocName = lblResourceName.Text;
                    doc.DocID = Convert.ToString(GetDocumentID());
                    documents.Add(doc);
                }
            }
            if (documents != null)
            {
                foreach (var dc in documents)
                {
                    try
                    {
                        kenticoDbHelper.SaveResourceInfo(dc.DocID, dc.ResourceID,dc.UserID,dc.DocumentTypeEnum,dc.AssociationCategoryEnum);
                    }
                    catch (Exception ex)
                    { }
                }
                //Rebinding the data grid after saving into kentico DB.
                BindDataToGrid();
            }
        }
   // }
}
public class ResourceDocument
{
    public string ResourceID
    {get;set;}

    public string DocumentID
    { get; set; }

    public int AssociationCategoryEnum
    {get;set;}

    public Guid UserID
    { get; set; }

    public int DocumentTypeEnum
    { get; set; }

    
    public string DocID
    { get; set; }

    public string DocName
    { get; set; }
}
public class KenticoDBHelper
{   
    private readonly string customTable_Class_DocumentPlanAssociation_Name = "TG.DocumentPlanAssociation";
    private readonly string customTable_Class_LookupDetails_Name = "TG.LookupDetails";

    public void SaveResourceInfo(string theDocumentID, string theResourceID, Guid userID, int DocumentTypeEnum, int AssociationCategoryEnum)
    {
        try
        {
            // Creates Custom table item provider
            CustomTableItemProvider customTableProvider = new CustomTableItemProvider(CMSContext.CurrentUser);

            // Checks if Custom table exists
            DataClassInfo customTable = DataClassInfoProvider.GetDataClass(customTable_Class_DocumentPlanAssociation_Name);
            if (customTable != null)
            {
                // Create table item
                CustomTableItem newCustomTableItem = CustomTableItem.New(customTable_Class_DocumentPlanAssociation_Name, customTableProvider);

                // Sets the row data
                newCustomTableItem.SetValue("DocumentTypeEnum", DocumentTypeEnum);
                newCustomTableItem.SetValue("AssociationCategoryEnum", AssociationCategoryEnum);
                newCustomTableItem.SetValue("UserID", userID);

                newCustomTableItem.SetValue("DocumentID", theDocumentID.ToString());
                newCustomTableItem.SetValue("AssociationID", theResourceID);

                // Inserts the item into database
                newCustomTableItem.Insert();
            }
        }
        catch (Exception ex)
        {
        }
    }    

    public DataSet GetDocToResources(int theDocumentID,int associationCategory,int documentType)
    {
        //string retVal = string.Empty;
        DataSet retDs=null;
        // Creates a new Custom table item provider
        CustomTableItemProvider customTableProvider = new CustomTableItemProvider(CMSContext.CurrentUser);

        // Checks if Custom table 'Sample table' exists
        DataClassInfo customTable = DataClassInfoProvider.GetDataClass(customTable_Class_DocumentPlanAssociation_Name);
        if (customTable != null)
        {
            // Prepares the parameters
            string where = "AssociationCategoryEnum=" + associationCategory + " and DocumentTypeEnum=" + documentType; //"resourceID >= 220 && resourceID<=225";
            int topN = 0;
            string orderBy = "";
            string columns = "DocumentID, AssociationID";

            // Gets the data set according to the parameters
            retDs = customTableProvider.GetItems(customTable_Class_DocumentPlanAssociation_Name, where, orderBy, topN, columns);           
        }
        return retDs;
    }
   
    
}
public class DataAccess
{
    SqlConnection sqlConnection;
    SqlDataAdapter sqlDataAdapter;
    SqlCommand cmd;
    DataSet dataSet;
    public DataAccess(string connectionString)
    {
        sqlConnection = new SqlConnection(connectionString);
        sqlDataAdapter = new SqlDataAdapter();
        cmd = new SqlCommand();
    }    
    public DataSet GetResourceList(string ProcName,string category,string Type, string subType,string resourceName, string textSearch,string textSearchOption)
    {
        dataSet = new DataSet();
        if(!string.IsNullOrEmpty(ProcName))
        {
        cmd.CommandText = ProcName;
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Connection = sqlConnection;

        cmd.Parameters.AddWithValue("@Category", category);
        cmd.Parameters.AddWithValue("@Type", Type);
        cmd.Parameters.AddWithValue("@SubType", subType);
        cmd.Parameters.AddWithValue("@ResourceName", resourceName);
        cmd.Parameters.AddWithValue("@TextSearch", textSearch);
        cmd.Parameters.AddWithValue("@TextSearchOpt", textSearchOption);

        sqlDataAdapter.SelectCommand = cmd;

        try
        {
            sqlConnection.Open();
            sqlDataAdapter.Fill(dataSet);
        }
        catch (SqlException ex)
        {
            Debug.WriteLine("SqlException: " + ex.Message);
        }
        finally
        {
            sqlConnection.Close();
        }
            }
        return dataSet;
    }

    public DataSet GetDataSet(string ProcName)
    {
        dataSet = new DataSet();
        if (ProcName != null)
        {                    
            cmd.CommandText = ProcName;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = sqlConnection;
            sqlDataAdapter.SelectCommand = cmd;
            try
            {
                sqlConnection.Open();
                sqlDataAdapter.Fill(dataSet);
            }
            catch (SqlException ex)
            {
                Debug.WriteLine("SqlException: " + ex.Message);                
            }           
            finally
            {
                sqlConnection.Close();
            }
        }
        return dataSet;
    }
    public static List<NameValue> TextSearchDropdownValues()
    {
        return new List<NameValue>
                {
                    new NameValue("Any Words", "any"),
                    new NameValue("All Words","all"),
                    new NameValue("Exact Phrase","exact"),
                    new NameValue("Keywords","key")
                    //, new NameValue("Author","author"),
                    //new NameValue("Addendum Name","addendum name"),
                    //new NameValue("Standard State nbr","standardnbr"),
                    //new NameValue("Standard Name","standardname"),
                    //new NameValue("Standard Text","standardtext"),
                    //new NameValue("Author","author")
                };
    }

}
public class NameValue
{
    public string Name { get; set; }
    public string Value { get; set; }

    public NameValue(string name, string value)
    {
        Name = name;
        Value = value;
    }
}