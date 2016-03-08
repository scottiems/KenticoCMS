using System;
    using System.Web;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Web.Services;
    using System.Web.Services.Protocols;
    using AjaxControlToolkit;
    using System.Data;
    using System.Data.SqlClient;
using System.Configuration;

/// <summary>
/// Summary description for StdAddNewAssocCascadeDdl
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
[System.Web.Script.Services.ScriptService]



public class StdAddNewAssocCascadeDdl : System.Web.Services.WebService {

    private readonly string ConnectionString = ConfigurationManager.ConnectionStrings["root_application"].ConnectionString;
   
    public StdAddNewAssocCascadeDdl () {

        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }

      
[WebMethod]  
public AjaxControlToolkit.CascadingDropDownNameValue[] GetSetForStandards(string knownCategoryValues, string category)  
{
    SqlConnection sqlConn = new SqlConnection(ConnectionString);  
    sqlConn.Open();
    SqlCommand sqlSelect = new SqlCommand("select distinct Standard_Set from Standards", sqlConn);  
    sqlSelect.CommandType = System.Data.CommandType.Text;  
    SqlDataAdapter sqlAdapter = new SqlDataAdapter(sqlSelect);  
    DataSet myDataset = new DataSet();  
    sqlAdapter.Fill(myDataset);  
    sqlConn.Close();  
  
    List<AjaxControlToolkit.CascadingDropDownNameValue> cascadingValues = new List<AjaxControlToolkit.CascadingDropDownNameValue>();  
  
    foreach (DataRow dRow in myDataset.Tables[0].Rows)  
    {  
        string setID = dRow["setID"].ToString();  
        string setName = dRow["setName"].ToString();  
        cascadingValues.Add(new AjaxControlToolkit.CascadingDropDownNameValue(setName,setID));  
    }  
  
    return cascadingValues.ToArray();  
}  
  
[WebMethod]  
public AjaxControlToolkit.CascadingDropDownNameValue[] GetGradeForStandards(string knownCategoryValues, string category)  
{  
    int setID;  
  
    StringDictionary categoryValues = AjaxControlToolkit.CascadingDropDown.ParseKnownCategoryValuesString(knownCategoryValues);  
      
    setID = Convert.ToInt32(categoryValues["set"]);

    SqlConnection sqlConn = new SqlConnection(ConnectionString);  
    sqlConn.Open();  
    SqlCommand sqlSelect = new SqlCommand("SELECT Distinct Grade FROM Standards where standard_set = @setID", sqlConn);  
    sqlSelect.CommandType = System.Data.CommandType.Text;  
    sqlSelect.Parameters.Add("@setID", SqlDbType.Int).Value = setID;  
    SqlDataAdapter sqlAdapter = new SqlDataAdapter(sqlSelect);  
    DataSet myDataset = new DataSet();  
    sqlAdapter.Fill(myDataset);  
    sqlConn.Close();  
  
    List<AjaxControlToolkit.CascadingDropDownNameValue> cascadingValues = new List<AjaxControlToolkit.CascadingDropDownNameValue>();  
  
    foreach (DataRow dRow in myDataset.Tables[0].Rows)  
    {  
        string gradeID = dRow["gradeID"].ToString();  
        string gradeName = dRow["gradeName"].ToString();  
        cascadingValues.Add(new AjaxControlToolkit.CascadingDropDownNameValue(gradeName, gradeID));  
    }  
  
    return cascadingValues.ToArray();  
}


[WebMethod]
public AjaxControlToolkit.CascadingDropDownNameValue[] GetSubjectForStandards(string knownCategoryValues, string category)
{
    int gradeID;

    StringDictionary categoryValues = AjaxControlToolkit.CascadingDropDown.ParseKnownCategoryValuesString(knownCategoryValues);

    gradeID = Convert.ToInt32(categoryValues["grade"]);

    SqlConnection sqlConn = new SqlConnection(ConnectionString);
    sqlConn.Open();
    SqlCommand sqlSelect = new SqlCommand("SELECT Distinct Subject FROM Standards where Grade = @gradeID", sqlConn);
    sqlSelect.CommandType = System.Data.CommandType.Text;
    sqlSelect.Parameters.Add("@gradeID", SqlDbType.Int).Value = gradeID;
    SqlDataAdapter sqlAdapter = new SqlDataAdapter(sqlSelect);
    DataSet myDataset = new DataSet();
    sqlAdapter.Fill(myDataset);
    sqlConn.Close();

    List<AjaxControlToolkit.CascadingDropDownNameValue> cascadingValues = new List<AjaxControlToolkit.CascadingDropDownNameValue>();

    foreach (DataRow dRow in myDataset.Tables[0].Rows)
    {
        string subjectID = dRow["subjectID"].ToString();
        string subjectName = dRow["subjectName"].ToString();
        cascadingValues.Add(new AjaxControlToolkit.CascadingDropDownNameValue(subjectName, subjectID));
    }

    return cascadingValues.ToArray();
}


[WebMethod]
public AjaxControlToolkit.CascadingDropDownNameValue[] GetCourseForStandards(string knownCategoryValues, string category)
{
    int subjectID;

    StringDictionary categoryValues = AjaxControlToolkit.CascadingDropDown.ParseKnownCategoryValuesString(knownCategoryValues);

    subjectID = Convert.ToInt32(categoryValues["set"]);

    SqlConnection sqlConn = new SqlConnection(ConnectionString);
    sqlConn.Open();
    SqlCommand sqlSelect = new SqlCommand("SELECT Distinct Course FROM Standards where Subject = @subjectID", sqlConn);
    sqlSelect.CommandType = System.Data.CommandType.Text;
    sqlSelect.Parameters.Add("@subjectID", SqlDbType.Int).Value = subjectID;
    SqlDataAdapter sqlAdapter = new SqlDataAdapter(sqlSelect);
    DataSet myDataset = new DataSet();
    sqlAdapter.Fill(myDataset);
    sqlConn.Close();

    List<AjaxControlToolkit.CascadingDropDownNameValue> cascadingValues = new List<AjaxControlToolkit.CascadingDropDownNameValue>();

    foreach (DataRow dRow in myDataset.Tables[0].Rows)
    {
        string courseID = dRow["courseID"].ToString();
        string courseName = dRow["courseName"].ToString();
        cascadingValues.Add(new AjaxControlToolkit.CascadingDropDownNameValue(courseName, courseID));
    }

    return cascadingValues.ToArray();
} 
}
