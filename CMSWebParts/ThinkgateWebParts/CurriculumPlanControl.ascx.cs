using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.PortalControls;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using Telerik.Web.UI;

public partial class CMSWebParts_ThinkgateWebParts_CurriculumPlanControl : CMSAbstractWebPart
{
    #region Private Variables

    private static string customTableClassName = "TG.DocumentPlanAssociation";
    private static readonly string ElementsConnectionString = ConfigurationManager.ConnectionStrings["root_application"].ConnectionString;
    private static readonly string CMSConnectionString = ConfigurationManager.ConnectionStrings["CMSConnectionString"].ConnectionString;

    #endregion

    #region Public Properties

    public int AssociationCount;

    #endregion

    #region Private Properties

    private int DocumentID { get; set; }
    private int ParentDocumentID { get; set; }
    private DocType DocumentType { get; set; }
    private string UserID { get; set; }
    private int Category { get; set; }
  
    public int docID;
    public DocType docType;
    public string uID, CPCurl, CPCAssociationDataListURL, CPCAssociationDataListDialogTitle; 

    #endregion

    #region Event Methods

    protected void Page_Init(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            //RadGrid1.AllowMultiRowSelection = true;
            //RadGrid1.AllowAutomaticInserts = true;
            //RadGrid1.ClientSettings.Selecting.AllowRowSelect = true;
            //RadGrid1.ClientSettings.Selecting.EnableDragToSelectRows = true;

            //GridClientSelectColumn checkColumn = new GridClientSelectColumn();
            //checkColumn.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
            //RadGrid1.MasterTableView.Columns.Add(checkColumn);

            BindGrade();
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        //int documentID = GetDocumentID();
        


        if (!IsPostBack)
        {
            FillParameters();
            BindGrade();
           
            Label1.Text = "Document ID: " + DocumentID.ToString();
            Label1.Visible = true; //Changing visibility to true will display the documentID on the top of the page
        }
        GetDocToCurriculums(DocumentID, DocumentType, Category);
        CPCurl = this.ResolveUrl("~/") + "CMSFormControls/Selectors/Thinkgate/CurriculumPlanControlSelector.aspx";
        CPCAssociationDataListURL = this.ResolveUrl("~/") + "CMSFormControls/Selectors/Thinkgate/AssociationDataList.aspx";
        CPCAssociationDataListDialogTitle="Associated Curricula";
        docID = DocumentID;
        docType = this.DocumentType;
        uID = this.UserID;
       
    }




    protected void cmbGrade_SelectedIndexChanged(object sender, EventArgs e)
    {
        cmbSubject.ClearSelection();
        BindSubject(cmbGrade.Text);
        ClearCourse();
    }

    protected void cmbSubject_SelectedIndexChanged(object sender, EventArgs e)
    {
        cmbCurriculum.ClearSelection();
        BindCourse(cmbGrade.Text, cmbSubject.Text);
    }

    protected void cmbCurriculum_SelectedIndexChanged(object sender, EventArgs e)
    {

        string queryString = BuildQueryString(cmbGrade.Text, cmbSubject.Text, cmbCurriculum.Text);
        btnAssociateCurricula.Enabled = true;
        //LoadData(queryString);
        
    }

    protected void btnAssociateCurricula_Click(object sender, EventArgs e)
    {
        HiddenField1.Value = "false";

        //int theDocumentID = GetDocumentID();
        FillParameters();
        if (DocumentID >= 0)
        {
            insertNewItems(DocumentID, ParentDocumentID, DocumentType, UserID, Category, int.Parse(cmbCurriculum.SelectedItem.Value));
            //insertItem(DocumentID, ParentDocumentID, DocumentType, UserID, Category, int.Parse(cmbCurriculum.SelectedItem.Value));
        }

        //Refresh Curriculums list
        GetDocToCurriculums(DocumentID, DocumentType, Category);
        //cmbGrade.ClearSelection();
        //ClearSubject();
        //ClearCourse();
      
        //ClientScriptManager client = Page.ClientScript;
        //client.RegisterClientScriptBlock(Page.GetType(),"executeJS", "<script type='text/javascript'>alert();</script>");
       
        //string popupScript = "<script type=\"text/javascript\">" +
        //  "alert();</" + "script>";
        //Page.ClientScript.RegisterStartupScript(this.GetType(), "popup", "alert();", true);


    }

    #endregion

    #region Private Methods

    private void FillParameters()
    {
        if (!string.IsNullOrWhiteSpace(Request.QueryString.Get("documentId")))
        {
            this.DocumentID = int.Parse(Request.QueryString.Get("documentId"));
        }
        if (!string.IsNullOrWhiteSpace(Request.QueryString.Get("documentType")))
        {
            //DocType currentDocumentType = DocType.InstructionPlan;
            //this.DocumentType = currentDocumentType;
            DocType currentDocumentType;

            if (Enum.TryParse(Request.QueryString.Get("documentType").ToString(), out currentDocumentType))
            {
                if (Enum.IsDefined(typeof(DocType), Request.QueryString.Get("documentType")))
                {
                    this.DocumentType = (DocType)Enum.Parse(typeof(DocType), Request.QueryString.Get("documentType"));
                }
            }
        }
        if (!string.IsNullOrWhiteSpace(Request.QueryString.Get("userId")))
        {
            this.UserID = Request.QueryString.Get("userId");
        }
        if (!string.IsNullOrWhiteSpace(Request.QueryString.Get("category")))
        {
            AssociationCategory currentAssociationType;
            if (Enum.TryParse(Request.QueryString.Get("category").ToString(), out currentAssociationType))
            {
                if (Enum.IsDefined(typeof(AssociationCategory), Request.QueryString.Get("category")))
                {
                    this.Category = Enum.Parse(typeof(AssociationCategory), Request.QueryString.Get("category")).GetHashCode();
                }
                else
                {
                    this.Category = int.Parse(Request.QueryString.Get("category"));
                }
            }
        }
    }

    //private int GetDocumentID()
    //{
    //    int theDocumentID = 0;
    //    if (CMSContext.CurrentDocument != null)
    //    {
    //        theDocumentID = TreeHelper.SelectSingleNode(CMSContext.CurrentDocument.NodeID).DocumentID;
    //    }
    //    else
    //    {
    //        if (Session["CurrentTreeNodeID"] != null && Convert.ToInt64(Session["CurrentTreeNodeID"].ToString()) > 0)
    //        {
    //            theDocumentID = TreeHelper.SelectSingleNode(Convert.ToInt32(Session["CurrentTreeNodeID"])).DocumentID;
    //        }
    //    }
    //    return theDocumentID;
    //}

    //private void GetDocToCurriculums2(int theDocumentID)
    //{
    //    //To Do: Use the newly created View to cross check the already added Curricula in the Elements table.
    //    string query = "SELECT * FROM  [dbo].[vCurriculumActiveDocs] where docId = " + theDocumentID;

    //    DataTable resultDataTable = GetDataTable(ElementsConnectionString, query);

    //    CustomTableItemProvider customTableProvider = new CustomTableItemProvider(CMSContext.CurrentUser);

    //    DataSet dataSet = customTableProvider.GetItems(customTableClassName, string.Empty, string.Empty, 0, "docId");

    //    dataSet.Tables.Add(resultDataTable);

    //    //Repeater1.DataSource = resultDataTable;
    //    //Repeater1.DataBind();
    //}

    private DataTable GetDocToCurriculums(int documentID, DocType documentType, int associationType)
    {
        string retVal = string.Empty;

        // Creates a new Custom table item provider
        CustomTableItemProvider customTableProvider = new CustomTableItemProvider(CMSContext.CurrentUser);

        // Checks if Custom table 'TG.DocumentPlanAssociation' exists
        DataClassInfo customTable = DataClassInfoProvider.GetDataClass(customTableClassName);
        if (customTable != null)
        {
            // Prepares the parameters
            string where = string.Format("DocumentID = {0} and DocumentTypeEnum = {1} and AssociationCategoryEnum = {2}", documentID, documentType.GetHashCode(), associationType);
            int topN = 0;
            string orderBy = "";
            string columns = "DocumentID, AssociationID";

            // Gets the data set according to the parameters
            DataSet dataSet = customTableProvider.GetItems(customTableClassName, where, orderBy, topN, columns);

            if (!DataHelper.DataSourceIsEmpty(dataSet))
            {
                AssociationCount = dataSet.Tables[0].Rows.Count;
                return dataSet.Tables[0];
            }
        }
        return null;
    }

    private void insertItem(int documentID, int parentDocumentID, DocType documentType, string userID, int associationType, int curriculaID)
    {
        // Creates Custom table item provider
        CustomTableItemProvider customTableProvider = new CustomTableItemProvider(CMSContext.CurrentUser);

        // Checks if Custom table exists
        DataClassInfo customTable = DataClassInfoProvider.GetDataClass(customTableClassName);
        if (customTable != null)
        {
            // Create table item
            CustomTableItem newCustomTableItem = CustomTableItem.New(customTableClassName, customTableProvider);

            // Sets the row data
            newCustomTableItem.SetValue("DocumentID", documentID);
            newCustomTableItem.SetValue("DocumentTypeEnum", documentType);
            newCustomTableItem.SetValue("UserID", Guid.Empty);
            newCustomTableItem.SetValue("AssociationCategoryEnum", associationType);
            newCustomTableItem.SetValue("AssociationID", curriculaID);
            newCustomTableItem.SetValue("ParentDocumentID", parentDocumentID);

            // Inserts the item into database
            newCustomTableItem.Insert();
        }
    }


    private void insertNewItems(int documentID, int parentDocumentID, DocType documentType, string userID, int associationType, int curriculaID)
    {
        DataTable dt = GetDocToCurriculums(documentID, documentType, associationType);
        //foreach (GridDataItem gridDataItem in gridItemCollection)
        //{
        //string curriculumID = gridDataItem["ID"].Text;

        if (dt == null)
        {
            insertItem(documentID, parentDocumentID, documentType, userID, associationType, curriculaID);
        }
        else
        {
            DataRow[] dr = dt.Select("AssociationID = " + curriculaID);
            if (dr != null && dr.Length <= 0)
            {
                insertItem(documentID, parentDocumentID, documentType, userID, associationType, curriculaID);
            }
        }

        AssociationCount++;
        //}
    }

    //private DataTable GetDocCurriculums(int documentID)
    //{
    //    //To Do: Use the newly created View to cross check the already added Curricula in the Elements table.
    //    string query = "SELECT ID  from [dbo].[vCurriculumActiveDocs]  where docId = " + documentID;

    //    return GetDataTable(ElementsConnectionString, query);
    //}

    //private void LoadData(string queryString)
    //{
    //    DataTable dt = GetDataTable(ElementsConnectionString, queryString);
    //    //dt.Columns.Add("xID", typeof(string));

    //    //if (dt.Rows.Count > 0)
    //    //{
    //    //    foreach (DataRow row in dt.Rows)
    //    //    {
    //    //        row["xID"] = EncryptInt(Convert.ToInt32(row["ID"].ToString()));
    //    //    }
    //    //}

    //    RadGrid1.DataSource = dt;
    //    RadGrid1.DataBind();
    //}

    //private void GetFormData()
    //{
    //    if (cmbGrade.Text != string.Empty
    //        && cmbSubject.Text != string.Empty
    //        && cmbCurriculum.Text != string.Empty)
    //    {
    //        string queryString = BuildQueryString(cmbGrade.Text, cmbSubject.Text, cmbCurriculum.Text);
    //        //LoadData(queryString);
    //    }
    //}

    private void BindGrade()
    {
        string sql = string.Format("SELECT DISTINCT Grade FROM CurrCourses C WHERE C.Active = 'Yes'");
        DataTable dt = GetDataTable(ElementsConnectionString, sql);
        if (dt.Rows.Count > 0)
        {
            cmbGrade.DataSource = dt;
            cmbGrade.DataTextField = "Grade";
            cmbGrade.DataBind();
        }
    }

    private void BindSubject(string grade)
    {
        string sql = string.Format("SELECT DISTINCT Subject FROM CurrCourses as C WHERE C.Grade='{0}' and C.Active='{1}'", grade, "Yes");
        DataTable dt = GetDataTable(ElementsConnectionString, sql);
        if (dt.Rows.Count > 0)
        {
            cmbSubject.DataSource = dt;
            cmbSubject.DataTextField = "Subject";
            cmbSubject.DataBind();
        }
    }

    private void ClearSubject()
    {
        cmbSubject.ClearSelection();
        cmbSubject.DataSource = null;
    }

    private void ClearCourse()
    {
        cmbCurriculum.ClearSelection();
        cmbCurriculum.DataSource = null;
    }

    private void BindCourse(string grade, string subject)
    {
        string sql = string.Format("SELECT DISTINCT ID, Course FROM CurrCourses as C WHERE C.Grade='{0}' and C.Subject='{1}' and C.Active='{2}'", grade, subject, "Yes");
        DataTable dt = GetDataTable(ElementsConnectionString, sql);
        if (dt.Rows.Count > 0)
        {
            cmbCurriculum.DataSource = dt;
            cmbCurriculum.DataValueField = "ID";
            cmbCurriculum.DataTextField = "Course";
            cmbCurriculum.DataBind();
        }
    }

    private string BuildQueryString(string grade, string subject, string course)
    {
        StringBuilder sb = new StringBuilder(string.Empty);

        if (!string.IsNullOrEmpty(grade) && !string.IsNullOrEmpty(subject))
        {
            sb.Append("SELECT ID, District, Grade, Subject, Course from CurrCourses C ");
            sb.Append("WHERE C.Active = 'Yes' AND C.Grade ='" + grade + "' ");
            sb.Append("AND C.Subject = '" + subject + "' ");
            sb.Append("AND C.Course = '" + course + "' ");
            sb.Append("ORDER BY C.ID");

        }
        return sb.ToString();
    }

    private DataTable GetDataTable(string connectionString, string query)
    {
        DataTable myDataTable = new DataTable();

        if (query != null)
        {
            string ConnectionStringToUse = string.Empty;

            if (connectionString != null)
            {
                ConnectionStringToUse = connectionString;
            }
            else
            {
                ConnectionStringToUse = ElementsConnectionString;
            }

            SqlConnection conn = new SqlConnection(ConnectionStringToUse);
            SqlDataAdapter adapter = new SqlDataAdapter();
            adapter.SelectCommand = new SqlCommand(query, conn);

            try
            {
                conn.Open();
            }
            catch (SqlException ex)
            {
                Debug.WriteLine("SqlException: " + ex.Message);
                return myDataTable;
            }

            try
            {
                adapter.Fill(myDataTable);
            }
            finally
            {
                conn.Close();
            }
        }
        return myDataTable;
    }

    #endregion

}