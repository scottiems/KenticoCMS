using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.PortalControls;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using Telerik.Web.UI;

public partial class CMSWebParts_ThinkgateWebParts_ATBCurricula : CMSAbstractWebPart
{
    private static string customTableClassName = "TG.DocumentPlanAssociation";
    private static readonly string ElementsConnectionString = ConfigurationManager.ConnectionStrings["root_application"].ConnectionString;
    private static readonly string CMSConnectionString = ConfigurationManager.ConnectionStrings["CMSConnectionString"].ConnectionString;

    private string docID;
    private DocType docType;
    private string databasetoHit;
    public int AssociationCount;
    private string ParentDocumentID { get; set; }
    
    /// <summary>
    /// The ATBCUrricula.ascx user control needs to be provided with two properties: DocumentID="DocID" and DocumentType="DocType".
    /// Eg: <uc1: ATBCurricula ID="someID" DocumentID="1000" DocumentType="InstructionPlan">
    /// 
    /// If the properties are not provided, the user control will take the DocumentID of the current page and Default DocumentType as InstructionPlan.
    /// </summary>
    /// 

    public string DocumentID
    {
        get
        {

            return docID;

        }

        set
        {
            docID = value;
        }
    }
    public DocType DocumentType
    {
        get
        {

            return docType;

        }

        set
        {
            docType = value;
        }
    }
    public string DB
    {
        get
        {

            return databasetoHit;

        }

        set
        {
            databasetoHit = value;
        }
    }
    private string UserID { get; set; }

    protected void Page_Init(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            BindGrade();
        }
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            BindGrade();
        }
        if (this.DocumentID == null)
        {
            this.DocumentID = "1000";
            // this.DocumentID = (TreeHelper.SelectSingleNode(CMS.CMSHelper.CMSContext.CurrentDocument.NodeID).DocumentID).ToString();
            }
            
        if (this.DocumentType == 0)
        {
            this.DocumentType = DocType.InstructionPlan;


        }
        if (this.DocumentType == 0)
        {
            this.DB = "Elements";


        }

        GetCurriculaAssoc();

        badge_div.InnerHtml = AssociationCount.ToString();


        lblCurricula_AddNew_DocID.Text = "Document ID: " + DocumentID.ToString();
        lblCurricula_AddNew_DocID.Visible = true; //Changing visibility to true will display the documentID on the top of the page

        lblCurricula_AddNew_DocType.Text = "Document Type: " + DocumentType;
        lblCurricula_AddNew_DocType.Visible = true; //Changing visibility to true will display the documentID on the top of the page


    }

    protected void lnkDeleteAssociation_Command(object sender, CommandEventArgs e)
    {
        switch (e.CommandName)
        {
            case "Delete":
                DeleteCurriculaAssocItem(int.Parse(e.CommandArgument.ToString()));
                GetCurriculaAssoc();
                badge_div.InnerText = AssociationCount.ToString();
                UpdatePanel_Curricula.Update();
                mpeCurriculaAssoc.Show();

                break;
        }
    }


    protected void GetCurriculaAssoc()
    {
        QueryDataParameters queryDataParameters = new QueryDataParameters();

        queryDataParameters.Add("DocumentID", this.DocumentID.ToString());
        queryDataParameters.Add("DocumentType", (int)this.DocumentType);
        queryDataParameters.Add("UserId", null);
        queryDataParameters.Add("AssociationCategory", (int)AssociationCategory.Curricula);

        queryDataParameters.Add("ClientDatabaseName", this.DB);
        DataSet resultDataSet = CMS.DataEngine.ConnectionHelper.ExecuteQuery("Thinkgate_GetDocumentPlanAssociationDetails", queryDataParameters, QueryTypeEnum.StoredProcedure, false);
        DataTable resultDataTable = resultDataSet.Tables[0];

       // DataTable resultDataTable = GetDocToCurriculums(this.DocumentID, this.DocumentType, AssociationCategory.Curricula);

       CustomTableItemProvider customTableProvider = new CustomTableItemProvider(CMSContext.CurrentUser);
        AssociationCount = resultDataTable.Rows.Count;
        rptAssociationDetail.DataSource = resultDataTable;
        rptAssociationDetail.DataBind();
        

    }

    private void DeleteCurriculaAssocItem(int associationID)
    {
        if (associationID >= 0)
        {
            string query = string.Format("Delete from [dbo].[TG_DocumentPlanAssociation] WHERE DocumentID={0} AND DocumentTypeEnum={1} AND AssociationCategoryEnum={2} AND AssociationID={3} ", this.DocumentID.ToString(), (int)this.DocumentType, (int)AssociationCategory.Curricula, associationID);
            GetDataTable(CMSConnectionString, query);
        }
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
                ConnectionStringToUse = CMSConnectionString;
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
    protected void cmbGrade_SelectedIndexChanged(object sender, EventArgs e)
    {
        cmbSubject.ClearSelection();
        BindSubject(cmbGrade.Text);
        ClearCourse();
        btnCurriculaAddNewAssocOK.Enabled = false;
    }


    protected void cmbSubject_SelectedIndexChanged(object sender, EventArgs e)
    {
        cmbCurriculum.ClearSelection();
        BindCourse(cmbGrade.Text, cmbSubject.Text);
        btnCurriculaAddNewAssocOK.Enabled = false;
    }

    protected void cmbCurriculum_SelectedIndexChanged(object sender, EventArgs e)
    {

        btnCurriculaAddNewAssocOK.Enabled = true;

    }


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



    private void insertNewCurriculaItems(string documentID, string parentDocumentID, DocType documentType, string userID, AssociationCategory associationType, int curriculaID)
    {
        DataTable dt = GetDocToCurriculums(documentID, documentType, associationType);

        if (dt == null)
        {
            insertCurriculaItem(documentID, parentDocumentID, documentType, userID, associationType, curriculaID);
        }
        else
        {
            DataRow[] dr = dt.Select("AssociationID = " + curriculaID);
            if (dr != null && dr.Length <= 0)
            {
                insertCurriculaItem(documentID, parentDocumentID, documentType, userID, associationType, curriculaID);
            }
        }

    }



    private void insertCurriculaItem(string documentID, string parentDocumentID, DocType documentType, string userID, AssociationCategory associationType, int curriculaID)
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
            newCustomTableItem.SetValue("DocumentID", documentID.ToString());
            newCustomTableItem.SetValue("DocumentTypeEnum", (int)documentType);
            newCustomTableItem.SetValue("UserID", Guid.Empty);
            newCustomTableItem.SetValue("AssociationCategoryEnum", (int)associationType);
            newCustomTableItem.SetValue("AssociationID", curriculaID);
            newCustomTableItem.SetValue("ParentDocumentID", null);

            // Inserts the item into database
            newCustomTableItem.Insert();
        }
    }

    private DataTable GetDocToCurriculums(string documentID, DocType documentType, AssociationCategory associationType)
    {

        // Creates a new Custom table item provider
        CustomTableItemProvider customTableProvider = new CustomTableItemProvider(CMSContext.CurrentUser);

        // Checks if Custom table 'TG.DocumentPlanAssociation' exists
        DataClassInfo customTable = DataClassInfoProvider.GetDataClass(customTableClassName);
        if (customTable != null)
        {
            // Prepares the parameters
            string where = string.Format("DocumentID = {0} and DocumentTypeEnum = {1} and AssociationCategoryEnum = {2}", documentID.ToString(), (int)documentType, (int)associationType);
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


    protected void btnCurriculaAddNewAssocOK_Click(object sender, EventArgs e)
    {

        insertNewCurriculaItems(DocumentID, ParentDocumentID, DocumentType, UserID, AssociationCategory.Curricula, int.Parse(cmbCurriculum.SelectedItem.Value));
        GetCurriculaAssoc();
        badge_div.InnerHtml = AssociationCount.ToString();

        //     GetDocToCurriculums(DocumentID, DocumentType, Category);

        cmbGrade.ClearSelection();
        cmbSubject.ClearSelection();
        cmbCurriculum.ClearSelection();
        btnCurriculaAddNewAssocOK.Enabled = false;
        UpdatePanel_Curricula.Update();
        if (badge_div.InnerHtml == "1")
        {
            mpeCurriculaAddNewAssoc.Hide();
            mpeCurriculaAssoc.Hide();
            mpeCurriculaAssoc.PopupControlID = "pnlCurriculaAssoc";
           // mpeCurriculaAssoc.CancelControlID = "btnCurriculaAssocClose";
            mpeCurriculaAssoc.PopupDragHandleControlID = "pnlCurriculaAssocTitle";
            UpdatePanel_Curricula.Update();
            mpeCurriculaAssoc.Show();
        
        }
        else
        {
            mpeCurriculaAddNewAssoc.Hide();
            mpeCurriculaAssoc.Show();
        }

    }

    protected void btnCurriculaAddNewAssocClose_Click(object sender, EventArgs e)
    {


        cmbGrade.ClearSelection();
        cmbSubject.ClearSelection();
        cmbCurriculum.ClearSelection();
        btnCurriculaAddNewAssocOK.Enabled = false;


        mpeCurriculaAddNewAssoc.Hide();
        if (AssociationCount == 0)
        {
            mpeCurriculaAssoc.Hide();
            UpdatePanel_Curricula.Update();
        }
    }


    protected void UpdatePanel_Curricula_Load(object sender, EventArgs e)
    {
        if (badge_div.InnerHtml == "0")
        {
            mpeCurriculaAssoc.PopupControlID = "pnlCurriculaAddNewAssoc";
          //mpeCurriculaAssoc.CancelControlID = "btnCurriculaAddNewAssocClose";
            mpeCurriculaAssoc.PopupDragHandleControlID = "pnlCurriculaAddNewAssocTitle";

        
        }
        else
        {
            mpeCurriculaAssoc.PopupControlID = "pnlCurriculaAssoc";
          // mpeCurriculaAssoc.CancelControlID = "btnCurriculaAssocClose";
            mpeCurriculaAssoc.PopupDragHandleControlID = "pnlCurriculaAssocTitle";

        }
    }
    protected void btnCurriculaAssocClose_Click(object sender, EventArgs e)
    {
        GetCurriculaAssoc();
        if (AssociationCount == 0)
        {
            mpeCurriculaAssoc.Hide();
            mpeCurriculaAssoc.PopupControlID = "pnlCurriculaAddNewAssoc";
            mpeCurriculaAssoc.PopupDragHandleControlID = "pnlCurriculaAddNewAssocTitle";



        }
        else
            mpeCurriculaAssoc.Hide();
    }
   
}
