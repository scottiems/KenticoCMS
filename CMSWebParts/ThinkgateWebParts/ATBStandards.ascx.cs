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


public partial class CMSWebParts_ThinkgateWebParts_ATBStandards : CMSAbstractWebPart
{
    private static string customTableClassName = "TG.DocumentPlanAssociation";
    private static readonly string ElementsConnectionString = ConfigurationManager.ConnectionStrings["root_application"].ConnectionString;
    private static readonly string CMSConnectionString = ConfigurationManager.ConnectionStrings["CMSConnectionString"].ConnectionString;

    private string docID;
    private DocType docType;
    private string databasetoHit;
    public int AssociationCountSTD;
    private string ParentDocumentID { get; set; }

   

    /// <summary>
    /// The ATBStandards.ascx user control needs to be provided with two properties: DocumentID="DocID" and DocumentType="DocType".
    /// Eg: <uc1: ATBStandards ID="someID" DocumentID="1000" DocumentType="InstructionPlan">
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

            standardsGrid.AllowMultiRowSelection = true;
            standardsGrid.AllowAutomaticInserts = true;
            standardsGrid.ClientSettings.Selecting.AllowRowSelect = true;
            standardsGrid.ClientSettings.Selecting.EnableDragToSelectRows = true;


            GridClientSelectColumn checkColumn = new GridClientSelectColumn();
            checkColumn.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
            standardsGrid.MasterTableView.Columns.Add(checkColumn);

            BindStandardSet();
        }
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            BindStandardSet();
        }
        if (this.DocumentID == null)
        {
          this.DocumentID = "1000";
          //this.DocumentID = (TreeHelper.SelectSingleNode(CMS.CMSHelper.CMSContext.CurrentDocument.NodeID).DocumentID).ToString();
        }
        if (this.DocumentType == 0)
        {
            this.DocumentType = DocType.InstructionPlan;


        }
        if (this.DocumentType == 0)
        {
            this.DB = "Elements";


        }


        GetStandardsAssoc();
        //Refresh();

        badge_div_std.InnerHtml = AssociationCountSTD.ToString();


        lblStandards_AddNew_DocID.Text = "Document ID: " + DocumentID.ToString();
        lblStandards_AddNew_DocID.Visible = true; //Changing visibility to true will display the documentID on the top of the page

        lblStandards_AddNew_DocType.Text = "Document Type: " + DocumentType;
        lblStandards_AddNew_DocType.Visible = true; //Changing visibility to true will display the documentID on the top of the page


    }

   
    protected void lnkDeleteAssociation_Command(object sender, CommandEventArgs e)
    {
        switch (e.CommandName)
        {
            case "Delete":
                DeleteStandardsAssocItem(int.Parse(e.CommandArgument.ToString()));
                GetStandardsAssoc();
                badge_div_std.InnerText = AssociationCountSTD.ToString();
                UpdatePanel_Standards.Update();
                mpeStandardsAssoc.Show();

                break;
        }
    }


    protected void GetStandardsAssoc()
    {
        QueryDataParameters queryDataParameters = new QueryDataParameters();

        queryDataParameters.Add("DocumentID", this.DocumentID.ToString());
        queryDataParameters.Add("DocumentType", (int)this.DocumentType);
        queryDataParameters.Add("UserId", null);
        queryDataParameters.Add("AssociationCategory", (int)AssociationCategory.Standard);
        queryDataParameters.Add("ClientDatabaseName", this.DB);

        DataSet resultDataSet = CMS.DataEngine.ConnectionHelper.ExecuteQuery("Thinkgate_GetDocumentPlanAssociationDetails", queryDataParameters, QueryTypeEnum.StoredProcedure, false);
        DataTable resultDataTable = resultDataSet.Tables[0];


        CustomTableItemProvider customTableProvider = new CustomTableItemProvider(CMSContext.CurrentUser);
        AssociationCountSTD = resultDataTable.Rows.Count;
        rptAssociationDetail.DataSource = resultDataTable;
        rptAssociationDetail.DataBind();

    }

    private void DeleteStandardsAssocItem(int associationID)
    {
        if (associationID >= 0)
        {
            string query = string.Format("Delete from [dbo].[TG_DocumentPlanAssociation] WHERE DocumentID={0} AND DocumentTypeEnum={1} AND AssociationCategoryEnum={2} AND AssociationID={3} ", this.DocumentID.ToString(), (int)this.DocumentType, (int)AssociationCategory.Standard, associationID);
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


    protected void Standard_Set_SelectedIndexChanged(object sender, EventArgs e)
    {
        BindSTDGrade(StandardSetDdl.Text);
        btnAddNewStandardsOK.Enabled = false;
    }


    protected void GradeDbl_SelectedIndexChanged(object sender, EventArgs e)
    {
        BindSTDSubject(StandardSetDdl.Text, GradeDdl.Text);
        btnAddNewStandardsOK.Enabled = false;
    }


    protected void SubjectDdl_SelectedIndexChanged(object sender, EventArgs e)
    {
        BindSTDCourse(StandardSetDdl.Text, GradeDdl.Text, SubjectDdl.Text);
        btnAddNewStandardsOK.Enabled = false;
    }


    protected void CourseDdl_SelectedIndexChanged(object sender, EventArgs e)
    {
        SetDefault();
        string queryString = BuildQueryString(GradeDdl.Text, SubjectDdl.Text, CourseDdl.Text, StandardSetDdl.Text);
        LoadData(queryString);
        btnAddNewStandardsOK.Enabled = true;
        pnlstandardSelectorPanel.Visible = true;


    }

    private string BuildQueryString(string grade, string subject, string course, string standardSet)
    {
        StringBuilder sb = new StringBuilder(string.Empty);

        /// TODO: District parameter is hardcoded. Need to get it from the params table.
        /// 
        if (!string.IsNullOrEmpty(grade) && !string.IsNullOrEmpty(subject))
        {
            string district = "22936";
            sb.Append("select ID, Standard_Set, Grade, Subject, Course, Level, StandardName, \"Desc\" as Description from Standards s ");
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
    public void LoadData(string queryString)
    {
         DataTable dt = GetDataTable(ElementsConnectionString, queryString);


        // dt.Columns.Add("chkSelect", typeof(System.Boolean));

        // foreach (DataRow dr in dt.Rows)
        // {

        //     dr["ChkSelect"] = 0;
        // }
         ViewState["StandardsList"] = dt;
        
        //GridView1.DataSource = griddt;
        //GridView1.DataBind();

         standardsGrid.DataSource = (DataTable)ViewState["StandardsList"];
         standardsGrid.DataBind();
         //rptSTDlist.DataSource = dt;
         //rptSTDlist.DataBind();
       
    }


    private void BindStandardSet()
    {
        string sql = "select distinct Standard_Set from Standards";
        DataTable dt = GetDataTable(ElementsConnectionString, sql);
        if (dt.Rows.Count > 0)
        {
            StandardSetDdl.DataSource = dt;
            //StandardSetDdl.DataTextField = "Standard_Set";
            StandardSetDdl.DataBind();
        }
    }
    private void BindSTDGrade(string standardSet)
    {

        string sql = string.Format("select distinct Grade from Standards as s where s.standard_set='{0}'", standardSet);

        SubjectDdl.DataSource = null;
        SubjectDdl.DataBind();
        //SubjectDdl.EmptyMessage = "Select One";

        CourseDdl.DataSource = null;
        CourseDdl.DataBind();
        //CourseDdl.EmptyMessage = "Select One";
        SetDefault();

        DataTable dt = GetDataTable(ElementsConnectionString, sql);
        if (dt.Rows.Count > 0)
        {
            GradeDdl.DataSource = dt;
            //GradeDbl.DataTextField = "Grade";
            GradeDdl.DataBind();
        }
    }
    private void SetDefault()
    {
        pnlstandardSelectorPanel.Visible = false;
        standardsGrid.DataSource = null;
        standardsGrid.DataBind();
        standardsGrid.SelectedIndexes.Clear();
        ViewState["StandardsList"] = null;
        //griddt = null;
        //GridView1.DataSource = null;
        //GridView1.DataBind();
        //rptSTDlist.DataSource = null;
        //rptSTDlist.DataBind();
    }

    private void BindSTDSubject(string standardSet, string grade)
    {

        CourseDdl.DataSource = null;
        CourseDdl.DataBind();
        //CourseDdl.EmptyMessage = "Select One";
        SetDefault();
        string sql = string.Format("select distinct Subject from Standards as s where s.standard_set='{0}' and s.grade='{1}'", standardSet, grade);
        DataTable dt = GetDataTable(ElementsConnectionString, sql);
        if (dt.Rows.Count > 0)
        {
            SubjectDdl.DataSource = dt;
            //SubjectDdl.DataTextField = "Subject";
            SubjectDdl.DataBind();
        }
    }
    private void BindSTDCourse(string standardSet, string grade, string subject)
    {

        SetDefault();
        string sql = string.Format("select distinct Course from Standards as s where s.standard_set='{0}' and s.grade='{1}' and s.subject='{2}'", standardSet, grade, subject);
        DataTable dt = GetDataTable(ElementsConnectionString, sql);
        if (dt.Rows.Count > 0)
        {
            CourseDdl.DataSource = dt;
            //CourseDdl.DataTextField = "Course";
            CourseDdl.DataBind();
        }
    }


    private DataTable GetDocStandards(string documentID, DocType documentType)
    {

        // Creates a new Custom table item provider
        CustomTableItemProvider customTableProvider = new CustomTableItemProvider(CMSContext.CurrentUser);

        // Checks if Custom table 'TG.DocumentPlanAssociation' exists
        DataClassInfo customTable = DataClassInfoProvider.GetDataClass(customTableClassName);
        if (customTable != null)
        {
            // Prepares the parameters
            string where = string.Format("DocumentID = {0} and DocumentTypeEnum = {1} and AssociationCategoryEnum = {2}", documentID.ToString(), (int)documentType, (int)AssociationCategory.Standard);
            int topN = 0;
            string orderBy = "";
            string columns = "DocumentID, AssociationID";

            // Gets the data set according to the parameters
            DataSet dataSet = customTableProvider.GetItems(customTableClassName, where, orderBy, topN, columns);
            

            if (!DataHelper.DataSourceIsEmpty(dataSet))
            {
                AssociationCountSTD = dataSet.Tables[0].Rows.Count;
                return dataSet.Tables[0];

            }
        }
        return null;
    }
    private void InsertNewSTDItems(RadGrid gridItemCollection, string documentID, DocType documentType)
    {
        DataTable dt = GetDocStandards(documentID, documentType);
        DataTable resultdt = (DataTable)ViewState["StandardsList"];

        for (int i = 0; i < resultdt.Rows.Count; i++)
        {
            if (gridItemCollection.SelectedIndexes.Contains(i.ToString()))
            {
                string theStandardID = resultdt.Rows[i]["ID"].ToString();
                DataRow[] dr = dt.Select("AssociationID = " + theStandardID);

                if (!string.IsNullOrEmpty(theStandardID) && dr.Length <= 0)
                {
                    InsertSTDItem(documentID, theStandardID);
                }
                
            }
        }

    }

    //    foreach (Telerik.Web.UI.GridDataItem gridDataItem in gridItemCollection.SelectedItems)
    //    {

    //        string theStandardID = gridDataItem["ID"].Text;

    //        DataRow[] dr = dt.Select("AssociationID = " + theStandardID);

    //        if (!string.IsNullOrEmpty(theStandardID) && dr.Length <= 0)
    //        {
    //            InsertSTDItem(documentID, theStandardID);
    //        }
    //    }

    //}


    //private void InsertNewSTDItems(DataTable resultdt, string documentID, DocType documentType)
    //{
    //    DataTable dt = GetDocStandards(documentID, documentType);

    //    //foreach (GridViewRow row in gridItemCollection.Rows)
    //    //{
    //    //    CheckBox chk = (CheckBox)row.FindControl("chkSelect");
    //    //    if (chk != null && chk.Checked)
    //    //    {

    //    //        string theStandardID = gridItemCollection.DataKeys[row.RowIndex]["ID"].ToString();
    //    //        DataRow[] dr = dt.Select("AssociationID = " + theStandardID);

    //    //        if (!string.IsNullOrEmpty(theStandardID) && dr.Length <= 0)
    //    //        {
    //    //            InsertSTDItem(documentID, theStandardID);
    //    //        }
    //    //    }

    //    //for (int i = 0; i < gridItemCollection.Rows.Count; i++)
    //    //{
    //    //    CheckBox chkb = (CheckBox)gridItemCollection.Rows[i].Cells[0].FindControl("chkSelect");
    //    //    if (chkb.Checked)
    //    //    {
    //    //        string theStandardID = gridItemCollection.Rows[i].Cells[1].Text;
    //    //        DataRow[] dr = dt.Select("AssociationID = " + theStandardID);

    //    //        if (!string.IsNullOrEmpty(theStandardID) && dr.Length <= 0)
    //    //        {
    //    //            InsertSTDItem(documentID, theStandardID);
    //    //        }
    //    //    }
            





    //        //foreach (GridViewRow gridDataItem in gridItemCollection.Rows)
    //        //{

    //        //    if (!string.IsNullOrEmpty(gridDataItem.Cells[0].Text))
    //        //    {
    //        //        bool chkSelect = Convert.ToBoolean(gridDataItem.Cells[0].Text);
    //        //        if (chkSelect)
    //        //        {
    //        //            string theStandardID = gridDataItem.Cells[1].ToString();
    //        //            DataRow[] dr = dt.Select("AssociationID = " + theStandardID);

    //        //            if (!string.IsNullOrEmpty(theStandardID) && dr.Length <= 0)
    //        //            {
    //        //                InsertSTDItem(documentID, theStandardID);
    //        //            }
    //        //        }
    //        //    }



    //        //}

    //    //}
    //}

    private void InsertSTDItem(string documentID, string theStandardID)
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
            newCustomTableItem.SetValue("DocumentTypeEnum", (int)this.DocumentType);
            newCustomTableItem.SetValue("UserID", Guid.Empty);
            newCustomTableItem.SetValue("AssociationCategoryEnum", (int)AssociationCategory.Standard);
            newCustomTableItem.SetValue("AssociationID", theStandardID);
            newCustomTableItem.SetValue("ParentDocumentID", this.ParentDocumentID); /// Empty for now..

            // Inserts the item into database
            newCustomTableItem.Insert();
        }
    }


    protected void btnAddNewStandardsOK_Click(object sender, EventArgs e)
    {
        
        int AssocCountBeforeUpdate = AssociationCountSTD;
        
      InsertNewSTDItems(standardsGrid, this.DocumentID, this.DocumentType);
        //DataTable dt = GetDocStandards(this.DocumentID, this.DocumentType);

        //for (int i = 0; i < griddt.Rows.Count; i++)
        //{

        //    if ((bool)griddt.Rows[i]["chkSelect"])
        //    {
        //        string theStandardID = griddt.Rows[i]["ID"].ToString();
        //        DataRow[] dr = dt.Select("AssociationID = " + theStandardID);

        //        if (!string.IsNullOrEmpty(theStandardID) && dr.Length <= 0)
        //        {
        //            InsertSTDItem(this.DocumentID, theStandardID);
        //        }
        //    }
        //}
        
        GetStandardsAssoc();
        badge_div_std.InnerHtml = AssociationCountSTD.ToString();

        StandardSetDdl.ClearSelection();
        GradeDdl.ClearSelection();
        SubjectDdl.ClearSelection();
        CourseDdl.ClearSelection();
        SetDefault();
      

        btnAddNewStandardsOK.Enabled = false;
        UpdatePanel_Standards.Update();
        if (AssocCountBeforeUpdate == 0)
        {
            mpeStandardsAddNewAssoc.Hide();
            mpeStandardsAssoc.Hide();
            mpeStandardsAssoc.PopupControlID = "pnlStandardsAssoc";
            // mpeStandardsAssoc.CancelControlID = "btnStandardsAssocClose";
            mpeStandardsAssoc.PopupDragHandleControlID = "pnlStandardsAssocTitle";
            UpdatePanel_Standards.Update();
            mpeStandardsAssoc.Show();

        }
        else
        {
            mpeStandardsAddNewAssoc.Hide();
            mpeStandardsAssoc.Show();
        }

    }



    protected void UpdatePanel_Standards_Load(object sender, EventArgs e)
    {
        if (badge_div_std.InnerHtml == "0")
        {
            mpeStandardsAssoc.PopupControlID = "pnlStandardsAddNewAssoc";

            mpeStandardsAssoc.PopupDragHandleControlID = "pnlStandardsAddNewAssocTitle";


        }
        else
        {
            mpeStandardsAssoc.PopupControlID = "pnlStandardsAssoc";

            mpeStandardsAssoc.PopupDragHandleControlID = "pnlStandardsAssocTitle";

        }
    }
    protected void btnStandardsAssocClose_Click(object sender, EventArgs e)
    {
        GetStandardsAssoc();
        if (AssociationCountSTD == 0)
        {
            mpeStandardsAssoc.PopupControlID = "pnlStandardsAddNewAssoc";
            mpeStandardsAssoc.PopupDragHandleControlID = "pnlStandardsAddNewAssocTitle";
            UpdatePanel_Standards.Update();


        }
    }

    protected void btnAddNewStandardsClose_Click(object sender, EventArgs e)
    {
        StandardSetDdl.ClearSelection();
        GradeDdl.ClearSelection();
        SubjectDdl.ClearSelection();
        CourseDdl.ClearSelection();
        SetDefault();
        btnAddNewStandardsOK.Enabled = false;

        mpeStandardsAddNewAssoc.Hide();
        if (AssociationCountSTD == 0)
        {
            mpeStandardsAssoc.Hide();
            UpdatePanel_Standards.Update();
        }
    }

}