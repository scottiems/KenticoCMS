using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Web.UI.WebControls;
using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.PortalControls;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using Telerik.Web.UI;

public partial class CMSWebParts_ThinkgateWebParts_newDTS : CMSAbstractWebPart
{
    #region  // Prepares the parameters

    private static string customTableClassName = "TG.DocumentPlanAssociation";
    private static readonly string ConnectionString = ConfigurationManager.ConnectionStrings["root_application"].ConnectionString;

    private string passPhrase = "Pas5pr@se";            // can be any string
    private string saltValue = "s@1tValue";             // can be any string
    private string hashAlgorithm = "SHA1";              // can also be "MD5"
    private int passwordIterations = 2;                 // can be any number
    private string initVector = "@1B2c3D4e5F6g7H8";     // must be 16 bytes
    private int keySize = 256;                          // can be 192 or 128
    public string ReturnURL { get; set; }
    private int documentType;
    private string CalledByToolBar = string.Empty;

    //Zero set for testing
    private Int32 DocumentID { get; set; }
    private DocType DocumentType { get; set; }
    private string UserID { get; set; }

    public int AssociationCount;

    #endregion

    /// <summary>
    /// E3RootURL
    /// </summary>
    public string E3RootURL
    {
        get
        {
            return ConfigurationManager.AppSettings["E3RootURL"] + ThinkgateKenticoHelper.getClientIDFromKenticoUserName(CMSContext.CurrentUser.UserName);
        }
    }

    /// <summary>
    /// Page_Init
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Init(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            standardsSearchGrid.AllowMultiRowSelection = true;
            standardsSearchGrid.AllowAutomaticInserts = true;
            standardsSearchGrid.ClientSettings.Selecting.AllowRowSelect = true;
            standardsSearchGrid.ClientSettings.Selecting.EnableDragToSelectRows = true;

            GridClientSelectColumn checkColumn = new GridClientSelectColumn();
            checkColumn.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
            standardsSearchGrid.MasterTableView.Columns.Add(checkColumn);

            BindStandardSet();
            if (Request.QueryString["ReturnURL"] != null)
                ReturnURL = Request.QueryString["ReturnURL"].ToString();

            if (!string.IsNullOrEmpty(Request.QueryString["FromToolBar"]))
                CalledByToolBar = Request.QueryString["FromToolBar"].ToString();


            if (!string.IsNullOrEmpty(CalledByToolBar)) //means calls from Association Toolbar.
            {
                btnAddAsHyperlink.Visible = false;
                btnAddAsContent.Visible = false;
                btnClose.Visible = true;
                btnCloseWnd.Visible = false;
                btnAddToLRMI.Visible = false;
                btnAddStandards.Visible = true;
            }
            else if (ReturnURL != null && ReturnURL == "LRMI")
            {
                btnAddAsHyperlink.Visible = false;
                btnAddAsContent.Visible = false;
                btnClose.Visible = false;
                btnAddToLRMI.Visible = true;
                btnCloseWnd.Visible = true;
                btnAddToLRMI.Enabled = false;
            }
            else
            {
                btnAddAsHyperlink.Visible = true;
                btnAddAsContent.Visible = true;
                btnClose.Visible = true;
                btnAddToLRMI.Visible = false;
                btnCloseWnd.Visible = false;
            }
        }
    }

    /// <summary>
    /// Page_Load
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(Request.QueryString.Get("documentId")))
        { this.DocumentID = Convert.ToInt32(Request.QueryString.Get("documentId")); }

        if (!string.IsNullOrWhiteSpace(Request.QueryString.Get("documentType")))
        { this.DocumentType = (DocType)Enum.Parse(typeof(DocType), Request.QueryString.Get("documentType")); }

        if (!string.IsNullOrWhiteSpace(Request.QueryString.Get("userId")))
        { this.UserID = Request.QueryString.Get("userId"); }

        
        //this.DocumentID = "0";
        if (!IsPostBack)
        {
            this.DocumentID = GetDocumentID();
          
            BindStandardSet();
            Label1.Text = "Document ID: " + this.DocumentID.ToString();
        }
        Refresh();
       
    }

    /// <summary>
    /// Get Document Id
    /// </summary>
    /// <returns></returns>
    private int GetDocumentID()
    {
        int theDocumentID = 0;
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
        if (theDocumentID == 0)
        {
            theDocumentID = this.DocumentID;
        }
        return theDocumentID;
    }

    /// <summary>
    /// Get Standard document
    /// </summary>
    /// <param name="theDocumentID"></param>
    /// <returns></returns>
    private string GetDocToStandards(int theDocumentID)
    {
        string retVal = string.Empty;

        // Creates a new Custom table item provider
        CustomTableItemProvider customTableProvider = new CustomTableItemProvider(CMSContext.CurrentUser);

        // Checks if Custom table 'Sample table' exists
        DataClassInfo customTable = DataClassInfoProvider.GetDataClass(customTableClassName);
        if (customTable != null)
        {
            // Prepares the parameters
            string where = "docID = '" + theDocumentID + "'";
            int topN = 0;
            string orderBy = "";
            string columns = "docID, standardID";

            // Gets the data set according to the parameters
            DataSet dataSet = customTableProvider.GetItems(customTableClassName, where, orderBy, topN, columns);

            if (!DataHelper.DataSourceIsEmpty(dataSet))
            {
                return dataSet.ToJSON(false);
            }
        }

        return "No records found";
    }

    /// <summary>
    /// Insert records into Custom table
    /// </summary>
    /// <param name="theDocumentID"></param>
    /// <param name="theStandardID"></param>
    private void InsertItem(int theDocumentID, string theStandardID)
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

    /// <summary>
    /// Call InsertItem method  and insert records one by one in cumstom table for selected item into grid.
    /// </summary>
    /// <param name="gridItemCollection"></param>
    /// <param name="documentID"></param>
    private void InsertNewItems(RadGrid gridItemCollection, int documentID)
    {
        DataTable dt = GetDocStandards(documentID);
        foreach (Telerik.Web.UI.GridDataItem gridDataItem in gridItemCollection.SelectedItems)
        {
            string theStandardID = gridDataItem["ID"].Text;

            DataRow[] dr = dt.Select("AssociationID = " + theStandardID);

            if (!string.IsNullOrEmpty(theStandardID) && dr.Length <= 0)
            {
                InsertItem(documentID, theStandardID);
            }
        }
    }

    /// <summary>
    /// GetDocStandards
    /// </summary>
    /// <param name="documentID"></param>
    /// <returns></returns>
    private DataTable GetDocStandards(int documentID)
    {
        string query = "SELECT k.AssociationID FROM [Elements].[dbo].[Standards] as e, [Kentico7].[dbo].[TG_DocumentPlanAssociation] as k where k.AssociationID = e.ID and k.DocumentID  = " + documentID;
        return GetDataTable(null, query);
    }

    /// <summary>
    /// LoadData
    /// </summary>
    /// <param name="queryString"></param>
    private void LoadData(string queryString)
    {
        DataTable dt = GetDataTable(null, queryString);
       /* dt.Columns.Add("xID", typeof(string));

        if (dt.Rows.Count > 0)
        {
            foreach (DataRow row in dt.Rows)
            {
                row["xID"] = EncryptInt(Convert.ToInt32(row["ID"].ToString()));
            }
        }*/

        standardsSearchGrid.DataSource = dt;
        standardsSearchGrid.DataBind();
    }

    /// <summary>
    /// GetFormData
    /// </summary>
    private void GetFormData()
    {
        if (StandardSetDdl.Text != string.Empty
            && GradeDbl.Text != string.Empty
            && SubjectDdl.Text != string.Empty
            && CourseDdl.Text != string.Empty)
        {
            string queryString = BuildQueryString(GradeDbl.Text, SubjectDdl.Text, CourseDdl.Text, StandardSetDdl.Text);
        }
    }

    /// <summary>
    /// BuildQueryString
    /// </summary>
    /// <param name="grade"></param>
    /// <param name="subject"></param>
    /// <param name="course"></param>
    /// <param name="standardSet"></param>
    /// <returns></returns>
    private string BuildQueryString(string grade, string subject, string course, string standardSet)
    {
        StringBuilder sb = new StringBuilder(string.Empty);

        //this.DocumentID = (this.DocumentID!=null) ? this.DocumentID : "0";

        if (!string.IsNullOrEmpty(grade) && !string.IsNullOrEmpty(subject))
        {
            string district = "22936"; //this would come from the parms
            sb.Append("select ID, Standard_Set as 'Set', Grade, Subject, Course, Level, StandardName as 'Standard Name', \"Desc\" as Description from Standards s ");
            sb.Append("where s.District =" + district + " ");
            sb.Append("and s.Grade = '" + grade + "' ");
            sb.Append("and s.Subject = '" + subject + "' ");
            sb.Append("and s.Course = '" + course + "' ");
            sb.Append("and s.Standard_Set = '" + standardSet + "' ");
            sb.Append("and s.ID not in(select AssociationID from [Kentico7].dbo.TG_DocumentPlanAssociation where documentid =" + this.DocumentID + " and DocumentTypeEnum = " + (int)this.DocumentType + " and AssociationCategoryEnum = " + (int)AssociationCategory.Standard + ")");
            sb.Append("order by s.StandardName");

        }
        return sb.ToString();
    }

    /// <summary>
    /// Standard_Set_SelectedIndexChanged
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Standard_Set_SelectedIndexChanged(object sender, EventArgs e)
    {
        BindGrade(StandardSetDdl.Text);
    }

    /// <summary>
    /// BindSubject
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void GradeDbl_SelectedIndexChanged(object sender, EventArgs e)
    {
        BindSubject(StandardSetDdl.Text, GradeDbl.Text);
    }

    /// <summary>
    /// BindCourse
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void SubjectDdl_SelectedIndexChanged(object sender, EventArgs e)
    {
        BindCourse(StandardSetDdl.Text, GradeDbl.Text, SubjectDdl.Text);
    }

    /// <summary>
    /// CourseDdl_SelectedIndexChanged
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void CourseDdl_SelectedIndexChanged(object sender, EventArgs e)
    {
        SetDefult();
        string queryString = BuildQueryString(GradeDbl.Text, SubjectDdl.Text, CourseDdl.Text, StandardSetDdl.Text);
        LoadData(queryString);

        if (btnAddToLRMI.Visible)
        { btnAddToLRMI.Enabled = true; }
    }

    /// <summary>
    /// BindStandardSet
    /// </summary>
    private void BindStandardSet()
    {
        string sql = "select distinct Standard_Set from Standards";
        DataTable dt = GetDataTable(null, sql);
        if (dt.Rows.Count > 0)
        {
            StandardSetDdl.DataSource = dt;
            StandardSetDdl.DataTextField = "Standard_Set";
            StandardSetDdl.DataBind();
        }
    }

    /// <summary>
    /// BindGrade
    /// </summary>
    /// <param name="standardSet"></param>
    private void BindGrade(string standardSet)
    {

        string sql = string.Format("select distinct Grade from Standards as s where s.standard_set='{0}'", standardSet);

        SubjectDdl.DataSource = null;
        SubjectDdl.DataBind();
        SubjectDdl.EmptyMessage = "Select One";

        CourseDdl.DataSource = null;
        CourseDdl.DataBind();
        CourseDdl.EmptyMessage = "Select One";
        SetDefult();

        DataTable dt = GetDataTable(null, sql);
        if (dt.Rows.Count > 0)
        {
            GradeDbl.DataSource = dt;
            GradeDbl.DataTextField = "Grade";
            GradeDbl.DataBind();
        }
    }

    /// <summary>
    /// BindSubject
    /// </summary>
    /// <param name="standardSet"></param>
    /// <param name="grade"></param>
    private void BindSubject(string standardSet, string grade)
    {
   
        CourseDdl.DataSource = null;
        CourseDdl.DataBind();
        CourseDdl.EmptyMessage = "Select One";
        SetDefult();
        string sql = string.Format("select distinct Subject from Standards as s where s.standard_set='{0}' and s.grade='{1}'", standardSet, grade);
        DataTable dt = GetDataTable(null, sql);
        if (dt.Rows.Count > 0)
        {
            SubjectDdl.DataSource = dt;
            SubjectDdl.DataTextField = "Subject";
            SubjectDdl.DataBind();
        }
    }

    /// <summary>
    /// BindCourse
    /// </summary>
    /// <param name="standardSet"></param>
    /// <param name="grade"></param>
    /// <param name="subject"></param>
    private void BindCourse(string standardSet, string grade, string subject)
    {
  
        SetDefult();
        string sql = string.Format("select distinct Course from Standards as s where s.standard_set='{0}' and s.grade='{1}' and s.subject='{2}'", standardSet, grade, subject);
        DataTable dt = GetDataTable(null, sql);
        if (dt.Rows.Count > 0)
        {
            CourseDdl.DataSource = dt;
            CourseDdl.DataTextField = "Course";
            CourseDdl.DataBind();
        }
    }

    /// <summary>
    /// SetDefult
    /// </summary>
    private void SetDefult()
    {
        standardsSearchGrid.DataSource = null;
        standardsSearchGrid.DataBind();
    }

    /// <summary>
    /// GetDataTable
    /// </summary>
    /// <param name="connectionString"></param>
    /// <param name="query"></param>
    /// <returns></returns>
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
                ConnectionStringToUse = ConnectionString;
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

    /// <summary>
    /// GetCurrentDocID
    /// </summary>
    /// <returns></returns>
    private int GetCurrentDocID()
    {
        return GetDocumentID();
    }

    /// <summary>
    /// EncryptInt
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public string EncryptInt(int id)
    {
        return EncodeTo64(Encrypt(id.ToString()));
    }

    /// <summary>
    /// EncodeTo64
    /// </summary>
    /// <param name="toEncode"></param>
    /// <returns></returns>
    public string EncodeTo64(string toEncode)
    {
        byte[] toEncodeAsBytes = Encoding.ASCII.GetBytes(toEncode);
        string returnValue = Convert.ToBase64String(toEncodeAsBytes);
        return returnValue;
    }

    /// <summary>
    /// Encrypt
    /// </summary>
    /// <param name="plainText"></param>
    /// <returns></returns>
    public string Encrypt(string plainText)
    {
        // Convert strings into byte arrays.
        // Let us assume that strings only contain ASCII codes.
        // If strings include Unicode characters, use Unicode, UTF7, or UTF8 
        // encoding.
        byte[] initVectorBytes = Encoding.ASCII.GetBytes(initVector);
        byte[] saltValueBytes = Encoding.ASCII.GetBytes(saltValue);

        // Convert our plaintext into a byte array.
        // Let us assume that plaintext contains UTF8-encoded characters.
        byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);

        // First, we must create a password, from which the key will be derived.
        // This password will be generated from the specified passphrase and 
        // salt value. The password will be created using the specified hash 
        // algorithm. Password creation can be done in several iterations.
        PasswordDeriveBytes password = new PasswordDeriveBytes(passPhrase, saltValueBytes, hashAlgorithm, passwordIterations);

        // Use the password to generate pseudo-random bytes for the encryption
        // key. Specify the size of the key in bytes (instead of bits).
        byte[] keyBytes = password.GetBytes(keySize / 8);

        // Create uninitialized Rijndael encryption object.
        RijndaelManaged symmetricKey = new RijndaelManaged();

        // It is reasonable to set encryption mode to Cipher Block Chaining
        // (CBC). Use default options for other symmetric key parameters.
        symmetricKey.Mode = CipherMode.CBC;

        // Generate encryptor from the existing key bytes and initialization 
        // vector. Key size will be defined based on the number of the key 
        // bytes.
        ICryptoTransform encryptor = symmetricKey.CreateEncryptor(keyBytes, initVectorBytes);

        // Define memory stream which will be used to hold encrypted data.
        MemoryStream memoryStream = new MemoryStream();

        // Define cryptographic stream (always use Write mode for encryption).
        CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
        // Start encrypting.
        cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);

        // Finish encrypting.
        cryptoStream.FlushFinalBlock();

        // Convert our encrypted data from a memory stream into a byte array.
        byte[] cipherTextBytes = memoryStream.ToArray();

        // Close both streams.
        memoryStream.Close();
        cryptoStream.Close();

        // Convert encrypted data into a base64-encoded string.
        string cipherText = Convert.ToBase64String(cipherTextBytes);

        // Return encrypted string.
        return cipherText;
    }

    /// <summary>
    /// btnAddStandards_Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnAddStandards_Click(object sender, EventArgs e)
    {
        int theDocumentID = GetDocumentID();
        InsertNewItems(standardsSearchGrid, theDocumentID);

       
        string queryString = BuildQueryString(GradeDbl.Text, SubjectDdl.Text, CourseDdl.Text, StandardSetDdl.Text);
        
        LoadData(queryString);
        Refresh();
    }

    /// <summary>
    /// Refresh
    /// </summary>
    private void Refresh()
    {
        string documenttype = Request.QueryString["documenttype"];
        DocType documentTypename = (DocType)Enum.Parse(typeof(DocType), documenttype);

        string query = string.Format("SELECT * FROM  [Kentico7].[dbo].[TG_DocumentPlanAssociation] where DocumentID = {0} and DocumentTypeEnum = {1} and AssociationCategoryEnum = {2}", this.DocumentID, (int)documentTypename, (int)AssociationCategory.Standard);
        DataTable resultDataTable = GetDataTable(null, query);
        if (resultDataTable.Rows.Count != null)
        {
            AssociationCount = resultDataTable.Rows.Count;
        }
    }
}