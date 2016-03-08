using System;
using System.Activities.Statements;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using System.Web;
using System.Text.RegularExpressions;
using DataAccess;
using DocumentFormat.OpenXml.Drawing.Charts;
using SQLDataAcess;
using Thinkgate.Services.Contracts.Scheduling;
using Thinkgate.Services.Contracts.Shared;
using DataTable = System.Data.DataTable;

public partial class CMSWebParts_ThinkgateWebParts_btWebServices : System.Web.UI.Page
{

    public static int AssociationCount;
    private static readonly string CMSConnectionString = ConfigurationManager.ConnectionStrings["CMSConnectionString"].ConnectionString;
    private static readonly string ElementsConnectionString = ConfigurationManager.ConnectionStrings["root_application"].ConnectionString;
    private static RatingDataAccess _ratingDataAccess;
    private static AssociationToolBarDataAccess _associationToolBarDataAccess;

    protected void Page_Load(object sender, EventArgs e)
    {
    }

    private static string getLocalDbConnectionString(string connString)
    {
        return connString.Replace("ThinkgateConfig", ThinkgateKenticoHelper.FindDBName());
    }

    [System.Web.Services.WebMethod]
    public static void AddSelectedItems(string docid, string SelectedItems)
    {

        string[] selitems = SelectedItems.Split('|');

        for (var i = 0; i < selitems.Length; i++)
        {
            int nodeid;
            string docEntry = selitems[i];
            if (Int32.TryParse(docEntry, out nodeid))
            {

                string retQuery = string.Format("select count(distinct standardID) count from [dbo].[thinkgate_docToStandards] where docID='{0}' AND standardID = '{1}' ", docid, nodeid);
                DataTable dt = GetDataTable(CMSConnectionString, retQuery);
                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        if (Convert.ToInt32(dt.Rows[0]["count"]) == 0)
                        {
                            string query = string.Format("Insert into [dbo].[thinkgate_docToStandards] (docID, standardID) values ('{0}', {1})", docid, nodeid);
                            GetDataTable(CMSConnectionString, query);
                        }
                    }
                }
                else//May Not required.
                {
                    string query = string.Format("Insert into [dbo].[thinkgate_docToStandards] (docID, standardID) values ('{0}', {1})", docid, nodeid);
                    GetDataTable(CMSConnectionString, query);
                }
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    //[System.Web.Services.WebMethod, ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = false)]
    [System.Web.Services.WebMethod]
    public static StatusResponse saveTimelineData(string data)
    {
        StatusResponse stat = new StatusResponse();

        JavaScriptSerializer serializer = new JavaScriptSerializer();

        List<TimelineSchedule> result = serializer.Deserialize<List<TimelineSchedule>>(data);

        if (result.Count > 0)
        {
            stat = updateScheduleDetails(result);
        }
        return stat;
    }

    /// <summary>
    /// Cleans the dates.
    /// </summary>
    /// <param name="json">The json.</param>
    /// <returns></returns>
    private static string cleanDates(string json)
    {
        string result = "";
        result = json.Replace(":new Date(", ":");
        result = result.Replace("'),\"end", "',\"end");
        result = result.Replace("'),\"startDate", "',\"startDate");

        return result;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="schedules"></param>
    /// <returns></returns>
    private static StatusResponse updateScheduleDetails(List<TimelineSchedule> schedules)
    {
        StatusResponse stat = new StatusResponse();
        var prxy = new SchedulingProxy();

        string theDB = ThinkgateKenticoHelper.FindDBName();
        if (string.IsNullOrEmpty(theDB))
        {
            stat.StatusLevel = 8;
            stat.Message = "Database not found, check ThinkgateConfig table - User was: " + CMS.CMSHelper.CMSContext.CurrentUser.UserName;
            prxy = null;
            return stat;
        }

        try
        {
            prxy.TimelineSchedulesSave(schedules.ToArray(), theDB);
            stat.Message = "SUCCESS to: " + theDB;
        }
        catch (Exception ex)
        {
            string msg = ex.Message;
            if (!string.IsNullOrWhiteSpace(((ex.InnerException).InnerException).Message))
            {
                msg += " InnerException: " + ((ex.InnerException).InnerException).Message;
            }
            stat.Message = msg;
        }
        finally
        {
            prxy = null;
        }

        return stat;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="docid"></param>
    /// <returns></returns>
    [System.Web.Services.WebMethod]
    public static string getCurrentCurricula(string docid)
    {
        string sql = string.Format("exec Thinkgate_GetCurriculaValues {0}, '{1}'", docid, getDistrictFromUserName());
        DataTable dt = GetDataTable(CMSConnectionString, sql);
        if (dt.Rows.Count > 0)
        {
            return dt.ToJSON(false);
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="docid"></param>
    /// <returns></returns>
    [System.Web.Services.WebMethod]
    public static string getCurrentStandards(string docid)
    {
        string sql = string.Format("exec Thinkgate_GetStandardNames {0}, '{1}'", docid, getDistrictFromUserName());
        DataTable dt = GetDataTable(CMSConnectionString, sql);
        if (dt.Rows.Count > 0)
        {
            return dt.ToJSON(false);
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="docid"></param>
    /// <returns></returns>
    [System.Web.Services.WebMethod]
    public static string getCurriculaCount(string docid)
    {
        string sql = "select distinct count (distinct curriculumid) as 'COUNT' from [dbo].[thinkgate_docToCurriculums] where (docID = '" + docid + "') ";
        DataTable dt = GetDataTable(CMSConnectionString, sql);
        if (dt.Rows.Count > 0)
        {
            StringBuilder sb = new StringBuilder();
            foreach (DataRow dr in dt.Rows)
            {
                string rowItem = dr["COUNT"].ToString();

                sb.Append(rowItem);

            }
            return sb.ToString();
        }
        else
        {
            return string.Empty;
        }
    }



    /// <summary>
    /// 
    /// </summary>
    /// <param name="grade"></param>
    /// <param name="subject"></param>
    /// <returns></returns>
    [System.Web.Services.WebMethod]
    public static string getCurriculaList(string grade, string gradeVal, string subject, string subjectVal, string searchOption, string searchText)
    {
        if (!string.IsNullOrEmpty(searchText))
        {
            searchText = HttpContext.Current.Server.UrlDecode(searchText);
        }
        string sql = string.Format("SELECT DISTINCT ID, Grade, Subject, Course FROM CurrCourses as C WHERE C.Active='Yes'");

        if (!string.IsNullOrWhiteSpace(gradeVal) && gradeVal != "0" && gradeVal != "null" && gradeVal != "undefined")
        {
            sql += " AND C.Grade='" + grade + "'";
        }
        if (!string.IsNullOrWhiteSpace(subjectVal) && subjectVal != "0" && subjectVal != "null" && subjectVal != "undefined")
        {
            sql += " AND C.Subject='" + subject + "'";
        }

        if (!string.IsNullOrWhiteSpace(searchText))
        {
            string[] searchWords = System.Text.RegularExpressions.Regex.Split(searchText, " ");
            string[] searchColumns = new string[] { "C.Grade", "C.Subject", "C.Course" };

            if (searchOption == "any")
            {
                string anyWordsCondition = TextSearchCondition(searchWords, searchColumns, true);
                sql += " AND " + anyWordsCondition;
            }
            else if (searchOption == "all")
            {
                string allWordsCondition = TextSearchCondition(searchWords, searchColumns, false);
                sql += " And " + allWordsCondition;
            }
            else if (searchOption == "exact")
            {
                sql += " AND ( C.Grade LIKE '%" + searchText + "%' ";
                sql += " OR C.Subject LIKE '%" + searchText + "%' ";
                sql += " OR C.Course LIKE '%" + searchText + "%') ";
            }
        }

        DataTable dt = GetDataTable(getLocalDbConnectionString(ElementsConnectionString), sql);
        if (dt != null && dt.Rows.Count > 0)
        {
            return dt.ToJSON(false);
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [System.Web.Services.WebMethod]
    public static string getGradeList()
    {
        string sql = string.Format("SELECT DISTINCT Grade FROM CurrCourses C WHERE C.Active = 'Yes'  ORDER BY Grade asc");
        DataTable dt = GetDataTable(getLocalDbConnectionString(ElementsConnectionString), sql);
        if (dt.Rows.Count > 0)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<option  value=\"0\"> ----- Select Item ----- </option>");
            foreach (DataRow dr in dt.Rows)
            {
                string rowItem = dr["grade"].ToString();

                sb.Append("<option value='" + rowItem + "'>" + rowItem + "</option>");

            }
            return sb.ToString();
        }
        else
        {
            return string.Empty;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="docid"></param>
    /// <returns></returns>
    [System.Web.Services.WebMethod]
    public static string getStandardCount(string docid)
    {
        string sql = "select count (*) as 'COUNT' from [dbo].[thinkgate_docToStandards] where (docID = '" + docid + "') ";
        DataTable dt = GetDataTable(CMSConnectionString, sql);
        if (dt.Rows.Count > 0)
        {
            StringBuilder sb = new StringBuilder();
            foreach (DataRow dr in dt.Rows)
            {
                string rowItem = dr["COUNT"].ToString();

                sb.Append(rowItem);

            }
            return sb.ToString();
        }
        else
        {
            return string.Empty;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="standardSet"></param>
    /// <returns></returns>
    [System.Web.Services.WebMethod]
    public static string getStandardSetGrade(string standardSet)
    {
        //string sql = string.Format("select distinct Grade from Standards as s where s.standard_set='{0}' order by Grade asc", standardSet);
        string db = ThinkgateKenticoHelper.FindDBName();
        var scrubbedStandardSet = standardSet.Replace("'", "''");   //stops sql injection attacks
        // string sql = string.Format("SELECT DISTINCT s.[Grade] FROM [{0}]..[Standards] s JOIN [{0}]..[StandardSets] ss ON s.[Standard_Set] = ss.[StandardSet] WHERE  ISNULL(ss.[StandardsSearch], '') <> 'no' and ss.StandardSet = '{1}' ORDER BY s.[Grade]", db, scrubbedStandardSet);
        string sql = string.Format("SELECT DISTINCT s.[Grade] FROM [{0}]..[StandardCourses] s JOIN [{0}]..[StandardSets] ss ON s.[StandardSet] = ss.[StandardSet] WHERE  ISNULL(ss.[StandardsSearch], '') <> 'no' and ss.StandardSet = '{1}' ORDER BY s.[Grade]", db, scrubbedStandardSet);

        DataTable dt = GetDataTable(getLocalDbConnectionString(ElementsConnectionString), sql);
        if (dt.Rows.Count > 0)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<option  value=\"0\"> ----- Select Item ----- </option>");
            foreach (DataRow dr in dt.Rows)
            {
                string rowItem = dr["Grade"].ToString();

                sb.Append("<option value='" + rowItem + "'>" + rowItem + "</option>");

            }
            return sb.ToString();
        }
        else
        {
            return string.Empty;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="standardSet"></param>
    /// <param name="grade"></param>
    /// <returns></returns>
    [System.Web.Services.WebMethod]
    public static string getStandardSetGradeSubject(string standardSet, string grade)
    {
        //string sql = string.Format("select distinct Subject from Standards as s where s.standard_set='{0}' and s.grade='{1}' order by Subject asc", standardSet, grade);
        string db = ThinkgateKenticoHelper.FindDBName();
        string sql = string.Format("SELECT DISTINCT s.[Subject] FROM  [{0}]..[StandardCourses] s JOIN [{0}]..[StandardSets] ss ON s.[StandardSet] = ss.[StandardSet]  WHERE ISNULL(ss.[StandardsSearch], '') <> 'no' AND ss.[StandardSet] = '{1}' AND s.[Grade] = '{2}' ORDER BY s.[Subject]", db, standardSet, grade);

        DataTable dt = GetDataTable(getLocalDbConnectionString(ElementsConnectionString), sql);
        if (dt.Rows.Count > 0)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<option  value=\"0\"> ----- Select Item ----- </option>");
            foreach (DataRow dr in dt.Rows)
            {
                string rowItem = dr["Subject"].ToString();

                sb.Append("<option value='" + rowItem + "'>" + rowItem + "</option>");

            }
            return sb.ToString();
        }
        else
        {
            return string.Empty;
        }
    }

    [System.Web.Services.WebMethod]
    public static string getStandardSetGradeSubjectCourse(string standardSet, string grade, string subject)
    {
        //string sql = string.Format("select distinct Course from Standards as s where s.standard_set='{0}' and s.grade='{1}' and s.subject='{2}' order by Course asc", standardSet, grade, subject);
        string db = ThinkgateKenticoHelper.FindDBName();
        string sql = string.Format("SELECT DISTINCT s.[Course] FROM [{0}]..[StandardCourses] s JOIN [{0}]..[StandardSets] ss ON s.[StandardSet] = ss.[StandardSet] WHERE ISNULL(ss.[StandardsSearch], '') <> 'no' AND ss.[StandardSet] = '{1}' AND s.[Grade] = '{2}' AND s.[Subject] = '{3}' ORDER BY s.[Course]", db, standardSet, grade, subject);

        DataTable dt = GetDataTable(getLocalDbConnectionString(ElementsConnectionString), sql);
        if (dt.Rows.Count > 0)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<option  value=\"0\"> ----- Select Item ----- </option>");

            foreach (DataRow dr in dt.Rows)
            {
                string rowItem = dr["Course"].ToString();

                sb.Append("<option value='" + rowItem + "'>" + rowItem + "</option>");
            }
            return sb.ToString();
        }
        else
        {
            return string.Empty;
        }
    }

    [System.Web.Services.WebMethod]
    public static string getStandardSetList()
    {
        //string sql = "select distinct Standard_Set from Standards";
        string sql = "SELECT [StandardSet] FROM [StandardSets] WHERE  ISNULL([StandardsSearch], '') <> 'no'";

        DataTable dt = GetDataTable(getLocalDbConnectionString(ElementsConnectionString), sql);
        if (dt.Rows.Count > 0)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<option  value=\"0\"> ----- Select Item ----- </option>");
            foreach (DataRow dr in dt.Rows)
            {
                string rowItem = dr["StandardSet"].ToString();

                sb.Append("<option value='" + rowItem + "'>" + rowItem + "</option>");

            }
            return sb.ToString();
        }
        else
        {
            return string.Empty;
        }
    }

    [System.Web.Services.WebMethod]
    public static string getStandardsList(string standardSet, string standardSetVal, string grade, string gradeVal, string subject, string subjectVal, string course, string courseVal, string searchOption, string searchText)
    {
        if (!string.IsNullOrEmpty(searchText))
        {
            searchText = HttpContext.Current.Server.UrlDecode(searchText);
        }
         if (string.IsNullOrWhiteSpace(gradeVal) || gradeVal == "0" || gradeVal == "null" || gradeVal == "undefined")
         {
             grade = null;
         }
        if (string.IsNullOrWhiteSpace(standardSetVal) || standardSetVal == "0" || standardSetVal == "null" || standardSetVal == "undefined")
        {
            standardSet = null;
        }
        if (string.IsNullOrWhiteSpace(subjectVal) || subjectVal == "0" || subjectVal == "null" || subjectVal == "undefined")
        {
            subject = null;
        }
        if (string.IsNullOrWhiteSpace(courseVal) || courseVal == "0" || courseVal == "null" || courseVal == "undefined")
        {
            course = null;
        }
        if (string.IsNullOrWhiteSpace(searchText))
        {
            searchText = null;
            searchOption = null;
        }
        _associationToolBarDataAccess = new AssociationToolBarDataAccess();
        DataTable dt = _associationToolBarDataAccess.GetStandardsListFromClientDB(grade, standardSet, subject, course,
            searchText, searchOption);

        if (dt.Rows.Count > 0)
        {

            return dt.ToJSON(false);
        }
        else
        {
            return null;
        }
    }

    [System.Web.Services.WebMethod]
    public static string getSubjectList(string grade)
    {
        string sql = string.Format("SELECT DISTINCT Subject FROM CurrCourses as C WHERE C.Grade='{0}' and C.Active='{1}'", grade, "Yes");
        DataTable dt = GetDataTable(getLocalDbConnectionString(ElementsConnectionString), sql);
        if (dt.Rows.Count > 0)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<option  value=\"0\"> ----- Select Item ----- </option>");
            foreach (DataRow dr in dt.Rows)
            {
                string rowItem = dr["subject"].ToString();

                sb.Append("<option value='" + rowItem + "'>" + rowItem + "</option>");

            }
            return sb.ToString();
        }
        else
        {
            return string.Empty;
        }
    }

    private static DataTable GetDataTable(string connectionString, string query)
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
            if (!(ConnectionStringToUse.Contains("Kentico")))
            {
                conn.Open();
                conn.ChangeDatabase(ThinkgateKenticoHelper.FindDBName());
            }

            SqlDataAdapter adapter = new SqlDataAdapter();
            adapter.SelectCommand = new SqlCommand(query, conn);

            try
            {
                //conn.Open();
            }
            catch (SqlException ex)
            {
                //Debug.WriteLine("SqlException: " + ex.Message);
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

    [System.Web.Services.WebMethod]
    private static string getDistrictFromUserName()
    {
        string theDistrict = string.Empty;

        string username = CMS.CMSHelper.CMSContext.CurrentUser.UserName;
        string[] wrk = username.Split('-');

        theDistrict = wrk[0];

        return theDistrict;
    }

    [System.Web.Services.WebMethod]
    public static string getResourceCount(string docid)
    {
        string sql = "select count (Distinct res.ResourceID) as 'COUNT' from [dbo].[thinkgate_docToResources] DR Join [dbo].[thinkgate_resource] res ON DR.ResourceID = res.ResourceID where (CONVERT(date,res.ExpirationDate) >= CONVERT(date,CURRENT_TIMESTAMP) OR res.ExpirationDate IS NULL) AND (DR.docID = '" + docid + "') ";
        DataTable dt = GetDataTable(CMSConnectionString, sql);
        if (dt.Rows.Count > 0)
        {
            StringBuilder sb = new StringBuilder();
            foreach (DataRow dr in dt.Rows)
            {
                string rowItem = dr["COUNT"].ToString();

                sb.Append(rowItem);

            }
            return sb.ToString();
        }
        else
        {
            return string.Empty;
        }
    }

    [System.Web.Services.WebMethod]
    public static string getCategoryList()
    {
        UserInfo user = UserInfoProvider.GetUserInfo(CMSContext.CurrentUser.UserName);
        string sql = string.Format("select [Enum], [Description], [StateLEA] from TG_LookupDetails where LookupEnum = {0} order by [Description] ASC", 13);
        DataTable dataTable = GetDataTable(CMSConnectionString, sql);
        if (dataTable.Rows.Count > 0)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<option value=\"0\"> -- Select Item -- </option>");
            foreach (DataRow dr in dataTable.Rows)
            {
                string value = dr["Enum"].ToString();
                string rowItem = dr["Description"].ToString();
                if (ThinkgateKenticoHelper.CheckValidStateLEA(user, dr["StateLEA"].ToString()))
                    sb.Append("<option value='" + value + "'>" + rowItem + "</option>");

            }
            return sb.ToString();
        }
        else
        {
            return string.Empty;
        }
    }

    [System.Web.Services.WebMethod]
    public static string getRatingRange()
    {
       _ratingDataAccess= new RatingDataAccess();
        DataTable dataTable = _ratingDataAccess.GetRatingRangeFromClientDB();
        if (dataTable.Rows.Count > 0)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<option value=\"0\"> -- Select Item -- </option>");
            foreach (DataRow dr in dataTable.Rows)
            {
                string value = dr["ID"].ToString();
                string rowItem = dr["DisplayValue"].ToString();
                sb.Append("<option value='" + value + "'>" + rowItem + "</option>");

            }
            return sb.ToString();
        }
        return string.Empty;
    }

    [System.Web.Services.WebMethod]
    public static string getResourceTypeList()
    {
        UserInfo user = UserInfoProvider.GetUserInfo(CMSContext.CurrentUser.UserName);
        string sql = string.Format("select [Enum], [Description], [StateLEA] from TG_LookupDetails where [Description] like '%Resource%' and LookupEnum in ({0}) order by [Description] ASC", 14);
        DataTable dataTable = GetDataTable(CMSConnectionString, sql);
        if (dataTable.Rows.Count > 0)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<option value=\"0\"> -- Select Item -- </option>");
            foreach (DataRow dr in dataTable.Rows)
            {
                string value = dr["Enum"].ToString();
                string rowItem = dr["Description"].ToString();

                if (ThinkgateKenticoHelper.CheckValidStateLEA(user, dr["StateLEA"].ToString()))
                    sb.Append("<option value='" + value + "'>" + rowItem + "</option>");

            }
            return sb.ToString();
        }
        return string.Empty;
    }

    [System.Web.Services.WebMethod]
    public static string getResourceSubTypeList(string resourceType)
    {
        string sql = string.Empty;
        //if (resourceType == "0")
        //{
        //    return "<option value=\"0\"> -- Select Item -- </option>";
        //}
        if (resourceType == "All" || resourceType == "0")
        {
            sql = string.Format("select Distinct Enum, Description from TG_LookupDetails where LookupEnum in (select Enum from TG_LookupDetails where lookupenum={0}) order by Description asc", 14);
        }
        else
        {
            sql = string.Format("select [Enum], [Description] from TG_LookupDetails where LookupEnum = {0} order by [Description] ASC", resourceType);
        }
        DataTable dataTable = GetDataTable(CMSConnectionString, sql);
        if (dataTable.Rows.Count > 0)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<option value=\"0\"> -- Select Item -- </option>");
            foreach (DataRow dr in dataTable.Rows)
            {
                string value = dr["Enum"].ToString();
                string rowItem = dr["Description"].ToString();

                sb.Append("<option value='" + value + "'>" + rowItem + "</option>");

            }
            return sb.ToString();
        }
        else
        {
            return string.Empty;
        }
    }

    /// <summary>
    /// getCourseList
    /// </summary>
    /// <param name="grade"></param>
    /// <param name="subject"></param>
    /// <returns></returns>
    [System.Web.Services.WebMethod]
    public static string getCourseList(string grade, string subject)
    {
        string sql = string.Format("SELECT DISTINCT Course FROM CurrCourses as C WHERE C.Grade='{0}' and C.subject='{1}' and C.Active='{2}'", grade, subject, "Yes");
        DataTable dt = GetDataTable(getLocalDbConnectionString(ElementsConnectionString), sql);
        if (dt.Rows.Count > 0)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<option value=\"0\"> ----- Select Item ----- </option>");
            foreach (DataRow dr in dt.Rows)
            {
                string rowItem = dr["Course"].ToString();

                sb.Append("<option value='" + rowItem + "'>" + rowItem + "</option>");

            }
            return sb.ToString();
        }
        else
        {
            return string.Empty;
        }
    }

    [System.Web.Services.WebMethod]
    public static string searchResources(string category, string type, string subtype, string name, string searchOption, string searchText, string grade, string standardSet, string course, string ratingRange)
    {


        if (!string.IsNullOrEmpty(searchText))
        {
            searchText = HttpContext.Current.Server.UrlDecode(searchText);
        }

        String userName = CMSContext.CurrentUser.UserName;
        string clientid = ThinkgateKenticoHelper.getClientIDFromKenticoUserName(userName);
        string clientFolder = ThinkgateKenticoHelper.getState(clientid);


        string dbName = ThinkgateKenticoHelper.FindDBName();
        string sql = @"select distinct res.ResourceID ID, res.Name, isnull(t.Description, '') Type, isnull(st.Description, '') SubType, res.Description, res.AverageRating
                        from thinkgate_resource res
                        left join TG_LookupDetails t on t.Enum = res.Type
                        left join TG_LookupDetails st on st.Enum = res.SubType
                        inner join  CMS_Document doc on doc.DocumentForeignKeyValue=res.ResourceID 
                        INNER JOIN CMS_PageTemplate pt on doc.DocumentPageTemplateID = pt.PageTemplateID and pt.PageTemplateCodeName = 'ResourceDefault'
				        left join thinkgate_docToCurriculums dc on dc.docID = doc.DocumentNodeID
                        left join [" + dbName + "]..CurrCourses eCurr on eCurr.ID=dc.curriculumID";

        name = System.Web.HttpContext.Current.Server.UrlDecode(name).Replace("'", "''");
        searchText = System.Web.HttpContext.Current.Server.UrlDecode(searchText).Replace("'", "''");
        sql += " WHERE (doc.DocumentNamePath like '%/" + clientFolder + "/Users/" + userName + "/%' OR doc.DocumentNamePath like '%/" + clientFolder + "/Districts/" + clientid + "/%' OR doc.DocumentNamePath like '%/" + clientFolder + "/Documents/%' OR doc.DocumentNamePath like '%/" + clientFolder + "/Shared/" + clientid + "/%' )";

        // Preventing bad or deleted data from showing up
        sql += " AND ISNULL(res.Type,0)<> 0 and ISNULL(res.SubType,0)<> 0";

        //Preventing Expired Resources from showing up

        sql += " AND (RES.ExpirationDate is Null OR Convert(date,Res.ExpirationDate)>= Convert(date,GetDate()))";

        if (!string.IsNullOrWhiteSpace(type) && type != "0" && type != "null")
        {
            sql += " AND res.type = '" + type + "'";
        }
        if (!string.IsNullOrWhiteSpace(subtype) && subtype != "0" && subtype != "null")
        {
            sql += " AND res.subtype = '" + subtype + "'";
        }
        if (!string.IsNullOrWhiteSpace(name))
        {
            sql += " AND res.name LIKE '%" + name + "%'";
        }

        if (!string.IsNullOrWhiteSpace(grade) && grade != "0" && grade != "null")
        {
            sql += " AND eCurr.grade = '" + grade + "'";
        }
        if (!string.IsNullOrWhiteSpace(standardSet) && standardSet != "0" && standardSet != "null")
        {
            sql += " AND eCurr.subject = '" + standardSet + "'";
        }
        if (!string.IsNullOrWhiteSpace(course) && course != "0" && course != "null")
        {
            sql += " AND eCurr.Course = '" + course + "'";
        }
        if (!string.IsNullOrWhiteSpace(ratingRange) && ratingRange != "0" && ratingRange != "null")
        {
            _ratingDataAccess = new RatingDataAccess();
            DataTable dtRangeValues = _ratingDataAccess.GetSpecificRatingRangeFromClientDB(Convert.ToInt32(ratingRange));
            if (dtRangeValues.Rows.Count > 0)
            {
                double minValue = dtRangeValues.Rows[0]["MinValue"] != null ? Convert.ToDouble(dtRangeValues.Rows[0]["MinValue"]) : default (double);
                double maxValue = dtRangeValues.Rows[0]["MaxValue"] != null ? Convert.ToDouble(dtRangeValues.Rows[0]["MaxValue"]) : Double.MaxValue;

                sql += " AND  ( res.Averagerating  Between " + minValue + " AND " + maxValue  ;
                sql += minValue == 0.00 ? " OR res.AverageRating Is Null ) " : " )";
            }
        }


        if (!string.IsNullOrWhiteSpace(searchText))
        {
            string[] searchWords = Regex.Split(searchText, " ");
            string[] searchColumns = new string[] { "res.[Name]", "res.[Description]", "t.[Description]", "st.[Description]" };

            if (searchOption == "any")
            {
                string anyWordsCondition = TextSearchCondition(searchWords, searchColumns, true);
                sql += " And " + anyWordsCondition;
            }
            else if (searchOption == "all")
            {
                string allWordsCondition = TextSearchCondition(searchWords, searchColumns, false);
                sql += " And " + allWordsCondition;
            }
            else if (searchOption == "exact")
            {
                sql += " AND ( res.Name LIKE '%" + searchText + "%' ";
                sql += " OR t.[Description] LIKE '%" + searchText + "%' ";
                sql += " OR st.[Description] LIKE '%" + searchText + "%' ";
                sql += " OR res.Description LIKE '%" + searchText + "%') ";
            }
            else if (searchOption == "createdBy")
            {
                sql += " AND res.createdBy LIKE '%" + searchText + "%' ";
            }
        }

        DataTable dataTable = GetDataTable(CMSConnectionString, sql);
        if (dataTable.Rows.Count > 0)
        {
            return dataTable.ToJSON(false);
        }
        return null;
    }

    private static string TextSearchCondition(string[] searchWords, string[] searchColumns, bool anyWords)
    {
        StringBuilder perColumnCondition = new StringBuilder(" (");
        for (int j = 0; j < searchColumns.Length; j++)
        {
            StringBuilder perWordCondition = new StringBuilder(" (");
            for (int i = 0; i < searchWords.Length; i++)
            {
                if (i > 0)
                {
                    perWordCondition.Append(anyWords == true ? " OR " : " AND ");
                }
                perWordCondition
                    .Append(" ")
                    .Append(searchColumns[j])
                    .Append(" LIKE '%")
                    .Append(searchWords[i])
                    .Append("%' ");
            }
            perWordCondition.Append(") ");
            if (j > 0)
                perColumnCondition.Append(" OR ").Append(perWordCondition);
            else
                perColumnCondition.Append(perWordCondition);
        }
        perColumnCondition.Append(") ");
        return perColumnCondition.ToString();
    }

    [System.Web.Services.WebMethod]
    public static string getCurrentResources(string docId)
    {
        string userName = CMSContext.CurrentUser.UserName;
        string clientid = ThinkgateKenticoHelper.getClientIDFromKenticoUserName(userName);
        string clientFolder = ThinkgateKenticoHelper.getState(clientid);
        string sql = string.Format("exec Thinkgate_GetDocumentResources {0}, '{1}', '{2}', '{3}'", docId, clientFolder, clientid, userName);
        DataTable dt = GetDataTable(CMSConnectionString, sql);
        if (dt.Rows.Count > 0)
        {
            return dt.ToJSON(false);
        }
        else
        {
            return null;
        }
    }

    [System.Web.Services.WebMethod]
    public static void AddSelectedResources(string docId, string selectedItems)
    {
        string[] selItems = selectedItems.Split(',');
        for (var i = 0; i < selItems.Length; i++)
        {
            int nodeid;
            string docEntry = selItems[i];
            if (Int32.TryParse(docEntry, out nodeid))
            {
                string retQuery = string.Format("select count(distinct docID) count from [dbo].[thinkgate_docToResources] where docID='{0}' AND ResourceID = '{1}' ", docId, nodeid);
                DataTable dt = GetDataTable(CMSConnectionString, retQuery);
                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        if (Convert.ToInt32(dt.Rows[0]["count"]) == 0)
                        {
                            string query = string.Format("Insert into [dbo].[thinkgate_docToResources] (docID, ResourceID) values ('{0}', {1})", docId, nodeid);
                            GetDataTable(CMSConnectionString, query);
                        }
                    }
                }
                else//May Not required.
                {
                    string query = string.Format("Insert into [dbo].[thinkgate_docToResources] (docID, ResourceID) values ('{0}', {1})", docId, nodeid);
                    GetDataTable(CMSConnectionString, query);
                }
            }
        }
    }

    [System.Web.Services.WebMethod]
    public static string GetClassIdFromDocType(string doctype, string selectedForm)
    {

        DataSet ds = ThinkgateKenticoHelper.GetTileMapLookupDataSet(doctype);
        DataRow selectRow = ds.Tables[0].Select("FriendlyName like'" + selectedForm + "'").FirstOrDefault();
        if (selectRow["KenticoDocumentTypeToShow"] != null)
        {
            string resourceToShow = selectRow["KenticoDocumentTypeToShow"].ToString();
            DataClassInfo dci = new DataClassInfo();
            dci = DataClassInfoProvider.GetDataClass(resourceToShow);
            string ClassID = dci.ClassID.ToString();
            return ClassID;
        }

        return string.Empty;

    }

    #region Assessment Associations

    #region Public Methods

    [System.Web.Services.WebMethod]
    public static string getAssessmentCategoryList()
    {
        string sql = string.Format("SELECT DISTINCT Category FROM TestTypes as T WHERE T.ExcludeFromSearchCriteria='No' AND Category <> 'Intervention' ORDER BY Category asc");

        DataTable dataTable = GetDataTable(getLocalDbConnectionString(ElementsConnectionString), sql);

        if (dataTable.Rows.Count > 0)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("<option  value=\"0\"> ----- Select Item ----- </option>");
            foreach (DataRow dataRow in dataTable.Rows)
            {
                string category = dataRow["category"].ToString();
                stringBuilder.Append("<option value='" + category + "'>" + category + "</option>");
            }
            return stringBuilder.ToString();
        }
        else
        { return string.Empty; }
    }

    [System.Web.Services.WebMethod]
    public static string getTypeList(string category)
    {
        string sql = string.Format("SELECT DISTINCT Type FROM TestTypes as T WHERE Seq<>-1 AND T.Category='{0}' AND t.Secure = 0", category);
        DataTable dataTable = GetDataTable(getLocalDbConnectionString(ElementsConnectionString), sql);

        if (dataTable.Rows.Count > 0)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("<option  value=\"0\"> ----- Select Item ----- </option>");
            foreach (DataRow dataRow in dataTable.Rows)
            {
                string type = dataRow["type"].ToString();
                stringBuilder.Append("<option value='" + type + "'>" + type + "</option>");
            }
            return stringBuilder.ToString();
        }
        else
        { return string.Empty; }
    }

    [System.Web.Services.WebMethod]
    public static string getTermList()
    {
        string sql = string.Format("exec E3_Terms_Get {0}", 0);
        DataTable dataTable = GetDataTable(getLocalDbConnectionString(ElementsConnectionString), sql);

        if (dataTable.Rows.Count > 0)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("<option  value=\"0\"> ----- Select Item ----- </option>");
            foreach (DataRow dataRow in dataTable.Rows)
            {
                string term = dataRow["term"].ToString();
                stringBuilder.Append("<option value='" + term + "'>" + term + "</option>");
            }
            return stringBuilder.ToString();
        }
        else
        { return string.Empty; }
    }

    [System.Web.Services.WebMethod]
    public static string getYearList()
    {
        string sql = string.Format("exec E3_Years_Select");

        DataTable dataTable = GetDataTable(getLocalDbConnectionString(ElementsConnectionString), sql);

        if (dataTable.Rows.Count > 0)
        {
            StringBuilder stringBuilder = new StringBuilder();

            foreach (DataRow dataRow in dataTable.Rows)
            {
                string year = dataRow["Year"].ToString();
                if (dataRow["YearTense"].ToString() == "Current")
                { stringBuilder.Append("<option value='" + year + "' selected='selected'>" + year + "</option>"); }
                else
                { stringBuilder.Append("<option value='" + year + "'>" + year + "</option>"); }
            }
            return stringBuilder.ToString();
        }
        else
        { return string.Empty; }
    }

    [System.Web.Services.WebMethod]
    public static dtGeneric_Int getCurrentAssessmentsList(string docid)
    {
        dtGeneric_Int currList = getCurriculumCourseIDs("", "", "", "", "", "");

        string sql = string.Format("exec Thinkgate_GetAssessmentsYears {0}, '{1}'", docid, getDistrictFromUserName());
        DataTable year = GetDataTable(getLocalDbConnectionString(CMSConnectionString), sql);

        string sqlCategory = string.Format("SELECT DISTINCT Category FROM TestTypes as T WHERE T.ExcludeFromSearchCriteria='No' ORDER BY Category asc");
        DataTable assessmentCategory = GetDataTable(getLocalDbConnectionString(ElementsConnectionString), sqlCategory);


        DataTable dataTable = new DataTable();

        if (year.Rows.Count > 0)
        {
            foreach (DataRow yearDataRow in year.Rows)
            {
                
                if (dataTable.Rows.Count > 0)
                {
                    DataTable assessment = new DataTable();
                    assessment = getCurrentAssessmentsListByYear(yearDataRow["Year"].ToString(), currList, assessmentCategory);
                    if (assessment.Rows.Count > 0)
                    {
                        foreach (DataRow dataRow in assessment.Rows)
                        { dataTable.Rows.Add(dataRow.ItemArray); }
                    }
                }
                else
                {
                    dataTable = getCurrentAssessmentsListByYear(yearDataRow["Year"].ToString(), currList, assessmentCategory);
                }
            }
        }

        var assessmentsIDs = new dtGeneric_Int();

        if (dataTable.Rows.Count > 0)
        {
            foreach (DataRow dataRow in dataTable.Rows)
            { assessmentsIDs.Add(Convert.ToInt32(dataRow["TestID"])); }
        }
        return assessmentsIDs;
    }

    [System.Web.Services.WebMethod]
    public static DataTable getCurrentAssessmentsListByYear(string year, dtGeneric_Int currList, DataTable assessmentCategory)
    {
        dtGeneric_String testTypes = new dtGeneric_String();
        dtGeneric_String terms = new dtGeneric_String();

        DataTable dataTable = new DataTable();
        
        if (assessmentCategory.Rows.Count > 0)
        {
            foreach (DataRow assessmentCategoryDataRow in assessmentCategory.Rows)
            {
                
                if (dataTable.Rows.Count > 0)
                {
                    DataTable assessment = new DataTable();
                    assessment = getAssessments(currList, assessmentCategoryDataRow["Category"].ToString(), testTypes, terms, year, "", "");

                    foreach (DataRow dataRow in assessment.Rows)
                    { dataTable.Rows.Add(dataRow.ItemArray); }
                }
                else
                {
                    dataTable = getAssessments(currList, assessmentCategoryDataRow["Category"].ToString(), testTypes, terms, year, "", "");
                }
            }
        }
        return dataTable;
    }

    [System.Web.Services.WebMethod]
    public static string getAssessmentsList(string category, string categoryVal, string categoryAll, string grade, string gradeVal, string subject, string subjectVal, string course, string courseVal, string type, string typeVal, string term, string termVal, string year, string searchOption, string searchText)
    {
        if (!string.IsNullOrEmpty(searchText))
        {
            searchText = HttpContext.Current.Server.UrlDecode(searchText);
        }
        if (!string.IsNullOrEmpty(searchOption) && !string.IsNullOrEmpty(searchText))
        {
            switch (searchOption)
            {
                case "Any Words":
                    searchOption = "any";
                    break;
                case "All Words":
                    searchOption = "all";
                    break;
                case "Exact Phrase":
                    searchOption = "exact";
                    break;
            }
        }

        dtGeneric_Int currList = getCurriculumCourseIDs(grade, gradeVal, subject, subjectVal, course, courseVal);
        dtGeneric_String testTypes = new dtGeneric_String();

        if (!string.IsNullOrWhiteSpace(typeVal) && typeVal != "0" && typeVal != "null" && typeVal != "undefined")
        { testTypes.Add(type); }

        dtGeneric_String terms = new dtGeneric_String();
        if (!string.IsNullOrWhiteSpace(termVal) && termVal != "0" && termVal != "null" && termVal != "undefined")
        { terms.Add(term); }

        DataTable dataTable = new DataTable();


        if (!string.IsNullOrWhiteSpace(categoryVal) && categoryVal != "0" && categoryVal != "null" && categoryVal != "undefined")
        {
            dataTable = getAssessments(currList, category, testTypes, terms, year, searchText, searchOption);
        }
        else
        {
            string[] categories = categoryAll.Split('|');
            foreach (string categoryName in categories)
            {
                if (!categoryName.Contains("Select Item") && !string.IsNullOrEmpty(categoryName))
                {
                    if (dataTable.Rows.Count > 0)
                    {
                        DataTable assessment = new DataTable();
                        assessment = getAssessments(currList, categoryName, testTypes, terms, year, searchText, searchOption);
                        foreach (DataRow dataRow in assessment.Rows)
                        { dataTable.Rows.Add(dataRow.ItemArray); }
                    }
                    else
                    {
                        dataTable = getAssessments(currList, categoryName, testTypes, terms, year, searchText, searchOption);
                    }
                }
            }
        }

        if (dataTable != null && dataTable.Rows.Count > 0)
        { return dataTable.ToJSON(false); }
        else
        { return null; }
    }

    [System.Web.Services.WebMethod]
    public static string getCurrentAssessments(string docid)
    {
        dtGeneric_Int assessmentsIDs = getCurrentAssessmentsList(docid);

        SqlParameterCollection sqlParameterCollection = new SqlCommand().Parameters;

        SqlParameter sqlParameter = new SqlParameter("@DocumentId", SqlDbType.Int);
        sqlParameter.Value = docid;
        sqlParameterCollection.Add(sqlParameter);

        sqlParameter = new SqlParameter("@ClientDatabaseName", SqlDbType.VarChar);
        sqlParameter.Value = getDistrictFromUserName();
        sqlParameterCollection.Add(sqlParameter);

        sqlParameter = new SqlParameter("@AssessmentsIDs", SqlDbType.Structured);
        sqlParameter.Value = assessmentsIDs.ToSql().Data;
        sqlParameter.TypeName = assessmentsIDs.ToSql().TypeName;
        sqlParameterCollection.Add(sqlParameter);

        DataTable dataTable = ThinkgateKenticoHelper.Connection_StoredProc_DataTable("Thinkgate_GetAssessmentsValues", sqlParameterCollection, CMSConnectionString);

        DataRow[] dataRows = dataTable.Select("UserAccess = 1");
        if (dataRows.Count() > 0)
        {
            dataTable = dataRows.CopyToDataTable();
            return dataTable.ToJSON(false);
        }
        else
        { return null; }
    }

    [System.Web.Services.WebMethod]
    public static string getAssessmentCount(string docid)
    {
        string sql = "Select Count(distinct AssessmentID) as 'COUNT' from [dbo].[thinkgate_docToAssessments] where (docID = '" + docid + "') ";
        DataTable dataTable = GetDataTable(CMSConnectionString, sql);

        if (dataTable.Rows.Count > 0)
        { return dataTable.Rows[0]["COUNT"].ToString(); }
        else
        { return string.Empty; }
    }

    #endregion

    #region Private Methods

    private static DataTable getAssessments(dtGeneric_Int currCourseIds, string testCategory, dtGeneric_String testTypes, dtGeneric_String terms, string year, string textWords, string textWordsOpt)
    {
        SqlParameterCollection sqlParameterCollection = new SqlCommand().Parameters;

        SqlParameter sqlParameter = new SqlParameter("@CurrCourseIDs", SqlDbType.Structured);
        sqlParameter.Value = currCourseIds.ToSql().Data;
        sqlParameter.TypeName = currCourseIds.ToSql().TypeName;
        sqlParameterCollection.Add(sqlParameter);

        sqlParameter = new SqlParameter("@TestCategory", SqlDbType.VarChar);
        sqlParameter.Value = testCategory;
        sqlParameterCollection.Add(sqlParameter);

        sqlParameter = new SqlParameter("@TestType", SqlDbType.Structured);
        sqlParameter.Value = testTypes.ToSql().Data;
        sqlParameter.TypeName = testTypes.ToSql().TypeName;
        sqlParameterCollection.Add(sqlParameter);

        sqlParameter = new SqlParameter("@Term", SqlDbType.Structured);
        sqlParameter.Value = terms.ToSql().Data;
        sqlParameter.TypeName = terms.ToSql().TypeName;
        sqlParameterCollection.Add(sqlParameter);

        sqlParameter = new SqlParameter("@TextWords", SqlDbType.VarChar);
        sqlParameter.Value = textWords;
        sqlParameterCollection.Add(sqlParameter);

        sqlParameter = new SqlParameter("@TextWordsOpt", SqlDbType.VarChar);
        sqlParameter.Value = textWordsOpt;
        sqlParameterCollection.Add(sqlParameter);

        sqlParameter = new SqlParameter("@Year", SqlDbType.VarChar);
        sqlParameter.Value = year;
        sqlParameterCollection.Add(sqlParameter);

        sqlParameter = new SqlParameter("@UserName", SqlDbType.VarChar);
        sqlParameter.Value = getUserName();
        sqlParameterCollection.Add(sqlParameter);

        return ThinkgateKenticoHelper.Connection_StoredProc_DataTable("E3_Assessment_SearchByUser", sqlParameterCollection);
    }

    private static dtGeneric_Int getCurriculumCourseIDs(string grade, string gradeVal, string subject, string subjectVal, string course, string courseVal)
    {
        SqlParameterCollection sqlParameterCollection = new SqlCommand().Parameters;

        SqlParameter sqlParameter = new SqlParameter("@UserName", SqlDbType.NVarChar);
        sqlParameter.Value = getUserName();
        sqlParameterCollection.Add(sqlParameter);

        DataTable dataTable = ThinkgateKenticoHelper.Connection_StoredProc_DataTable("E3_CurrCourseIDs_SearchByUser", sqlParameterCollection);

        string sql = "Active = 'Yes'";

        if (!string.IsNullOrWhiteSpace(gradeVal) && gradeVal != "0" && gradeVal != "null" && gradeVal != "undefined")
        { sql += " AND Grade='" + grade + "'"; }

        if (!string.IsNullOrWhiteSpace(subjectVal) && subjectVal != "0" && subjectVal != "null" && subjectVal != "undefined")
        { sql += " AND Subject='" + subject + "'"; }

        if (!string.IsNullOrWhiteSpace(courseVal) && courseVal != "0" && courseVal != "null" && courseVal != "undefined")
        { sql += " AND Course='" + course + "'"; }

        var curriculumCourseIDs = new dtGeneric_Int();
        DataRow[] dataRows = dataTable.Select(sql);

        if (dataRows.Count() > 0)
        {
            foreach (DataRow dataRow in dataRows)
            { curriculumCourseIDs.Add(Convert.ToInt32(dataRow["ID"])); }
        }

        return curriculumCourseIDs;
    }

    private static string getUserName()
    {
        string[] fullName = CMS.CMSHelper.CMSContext.CurrentUser.UserName.Split('-');
        string[] username = fullName[1].Split('_');

        if (username.Count() > 1)
        {
            string sqlUserName = string.Format("SELECT UserName FROM aspnet_Users as U WHERE U.UserName like '" + username[0] + "%' ORDER BY UserName asc");
            DataTable datatable = GetDataTable(ElementsConnectionString, sqlUserName);

            if (datatable.Rows.Count > 0)
            {
                foreach (DataRow datarow in datatable.Rows)
                {
                    string user = datarow["UserName"].ToString();
                    if (Regex.Replace(user.Trim(), @"([^A-Za-z0-9])", "_") == fullName[1])
                    { return user; }
                }
            }
        }
        return fullName[1];
    }

    #endregion

    #endregion
}
