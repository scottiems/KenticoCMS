using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using CMS.CMSHelper;
using CMS.GlobalHelper;
using Nustache.Core;
using System.Web;
using System.Text;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

/// <summary>
/// Summary description for ThinkgateTemplateBuilder
/// </summary>
public class ThinkgateTemplateBuilder
{
     
    private static readonly string CmsConnectionString = ConfigurationManager.ConnectionStrings["CMSConnectionString"].ConnectionString;
    private static readonly string ElementsConnectionString = ConfigurationManager.ConnectionStrings["root_application"].ConnectionString;
    private const string E3StandardsUrl = "E3StandardsURL";
    private const string E3AssessmentsUrl = "E3AssessmentsURl";

    private const string StandardsTemplate = @"<div class='defaultTemplate standardsTemplate'><table><tr><th align='left' style='width: 120px;'>Standard</th><th align='center'>Description</th></tr>{{#.}}<tr><td style='text-align:left;vertical-align:top;padding:1;width:350px'><a href='{{EncryptedStandardURL}}' target='_blank' rel='license'>{{StandardName}}</a></td> <td style='text-align:left;vertical-align:top;padding:1;'>{{{Description}}}</td></tr>{{/.}}</table></div>";
    private const string CurriculumTemplate = @"<div class='defaultTemplate curriculumTemplate'><table><tr><th align='left'>Grade</th><th align='left'>Subject</th><th align='left'>Course</th></tr>{{#.}}<tr><td>{{Grade}}</td> <td>{{Subject}}</td> <td>{{Course}}</td></tr>{{/.}}</table></div>";
    private const string CreativeCommonsTemplate = @"<div class='creativeCommonsTemplate'><p align='center'><a href='http://creativecommons.org/licenses/by-nc-sa/3.0/' target='_blank' rel='license'><img src='http://i.creativecommons.org/l/by-nc-sa/3.0/88x31.png' style='border-width:0' alt='Creative Commons License'></a><br>This work is licensed under the Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License (CC BY-NC-SA 3.0). Educators may use, adapt, and/or share. Not for commercial use. To view a copy of the license, visit <a href='http://creativecommons.org/licenses/by-nc-sa/3.0/' target='_blank' rel='license'>http://creativecommons.org/licenses/by-nc-sa/3.0/</a></p></div>";
    private const string MA_CreativeCommonsTemplate = @"<div class='creativeCommonsTemplate'><p align='center'><a href='http://creativecommons.org/licenses/by-nc-sa/3.0/' target='_blank' rel='license'><img src='http://i.creativecommons.org/l/by-nc-sa/3.0/88x31.png' style='border-width:0' alt='Creative Commons License'></a><br>This work is licensed by the MA Department of Elementary & Secondary Education under the Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License (CC BY-NC-SA 3.0). Educators may use, adapt, and/or share. Not for commercial use. To view a copy of the license, visit <a href='http://creativecommons.org/licenses/by-nc-sa/3.0/' target='_blank' rel='license'>http://creativecommons.org/licenses/by-nc-sa/3.0/</a></p></div>";
    private const string CreativeCommonsTemplate1 = @"<div class='creativeCommonsTemplate'><p align='center'><a href='http://creativecommons.org/licenses/by-sa/3.0/deed.en_US' target='_blank' rel='license'><img src='http://i.creativecommons.org/l/by-sa/3.0/88x31.png' style='border-width:0' alt='Creative Commons License'></a><br>This work is licensed under a <a href='http://creativecommons.org/licenses/by-sa/3.0/deed.en_US' target='_blank' rel='license'>Creative Commons Attribution-ShareAlike 3.0 Unported License</a></p></div>";
    private const string ResourceTemplate = @"<div class='defaultTemplate resourceTemplate'><table><tr><th align='left' style='width: 200px;'>Resource Type</th><th align='left' style='width: 250px;'>Resource Name</th><th align='left'>Description</th></tr>{{#.}}<tr><td  style='text-align:left;vertical-align:top;padding:1;'>{{ResourceType}}</td><td style='text-align:left;vertical-align:top;padding:1;'><a href='{{ResourceURL}}' target='_blank' rel='license'>{{ResourceName}}</a></td> <td>{{{Description}}}</td></tr>{{/.}}</table></div>";

    public static string PortalName { get; set; }
    public static readonly string StartDelimiter = "[[";
    public static readonly string EndDelimiter = "]]";


    public static string FindTag(string htmlString)
    {
        int start = htmlString.IndexOf(StartDelimiter);
        int end = htmlString.IndexOf(EndDelimiter);

        if (end > 0 && end > start)
        {
            string result = htmlString.Substring(start + StartDelimiter.Length, end - (start + EndDelimiter.Length));
            return result;
        }
        return string.Empty;
    }

    public static string TagReplace(string currentTagFound, string htmlString, string nodeId)
    {
        string tagToFind = StartDelimiter + currentTagFound + EndDelimiter;
        switch (currentTagFound.ToUpper())
        {
            case "STANDARDS":
            case "S":
                htmlString = htmlString.Replace(tagToFind, BuildStandardsTag(nodeId));
                break;
            case "CURRICULA":
            case "CURRICULUM":
            case "C":
                htmlString = htmlString.Replace(tagToFind, BuildCurriculaTag(nodeId));
                break;
            case "CREATIVE COMMONS TEXT":
            case "CREATIVECOMMONSTEXT":
            case "CCT":
            case "CC":
                if (GetDistrictFromUserName().StartsWith("MA"))
                {
                    htmlString = htmlString.Replace(tagToFind, Build_MA_CreativeCommonsTag());
                }
                else
                {
                    htmlString = htmlString.Replace(tagToFind, BuildCreativeCommonsTag());
                }
                break;
            case "CC1":
                htmlString = htmlString.Replace(tagToFind, BuildCreativeCommonsTag1());
                break;
            case "RESOURCE":
            case "RESOURCES":
            case "R":
                htmlString = htmlString.Replace(tagToFind, BuildResourcesTag(nodeId));
                break;
            case "ASSESSMENT":
            case "ASSESSMENTS":
            case "A":
                htmlString = htmlString.Replace(tagToFind, BuildAssessmentsTag(nodeId));
                break;
            default:
                htmlString = htmlString.Replace(tagToFind, @"<b>Thinkgate quickTag (" + currentTagFound + ") not found.</b>");
                break;
        }
        return htmlString;
    }

    private class TGStandards
    {
        public string ID { get; set; }
        public string EncryptedStandardUrl { get; set; }
        public string StandardName { get; set; }
        public string Description { get; set; }
    }
    private static string BuildStandardsTag(string nodeId)
    {
        string retVal = string.Empty;

        //get standards for NodeID
        string sql = string.Format("exec Thinkgate_GetStandardNames {0}, '{1}'", nodeId, GetDistrictFromUserName());

        string e3StandardsUrl = CookieHelper.GetValue(E3StandardsUrl);
        if (string.IsNullOrEmpty(e3StandardsUrl) || !e3StandardsUrl.Contains(GetDistrictFromUserName()))
        {
            e3StandardsUrl = BuildStandardsUrl();
        }

        DataTable dt = ThinkgateKenticoHelper.GetDataTable(CmsConnectionString, sql);
        if (dt.Rows.Count > 0)
        {
            var myEnumerable = dt.AsEnumerable();

            List<TGStandards> tgStandardsList =
                (from item in myEnumerable
                 select new TGStandards
                 {
                     ID = item.Field<int>("ID").ToString(),
                     EncryptedStandardUrl = e3StandardsUrl + Standpoint.Core.Classes.Encryption.EncryptString(item.Field<int>("ID").ToString()),
                     StandardName = item.Field<string>("StandardName"),
                     Description = item.Field<string>("Description")
                 }).ToList();

            retVal = Render.StringToString(StandardsTemplate, tgStandardsList);
        }

        //var myData = new { FirstName = "Joe", LastName = "Dirt", NodeID = NodeID };
        //var templateString = @"A template to show fullname / NodeID - {{FirstName}} {{LastName}} / <font color='red'>{{NodeID}}</font>";
        ////var template = Template(templateString);
        ////var html = Render.FileToString("foo.template", myData);
        //var html2 = Render.StringToString(templateString, myData);
        //return "<h1>this is the 'aaa' tag replace</h1>";

        return retVal;
    }
    private static string BuildStandardsUrl()
    {
        string theUrl = string.Format(ConfigurationManager.AppSettings["CMSSiteUrl"] + "/{0}/Record/StandardsPage.aspx?xID=", GetDistrictFromUserName());

        CookieHelper.SetValue(E3StandardsUrl, theUrl, System.DateTime.Now.AddHours(1));
        return theUrl;
    }

    private class TGCurriculum
    {
        public string ID { get; set; }
        public string Grade { get; set; }
        public string Subject { get; set; }
        public string Course { get; set; }
    }
    private static string BuildCurriculaTag(string nodeId)
    {
        string retVal = string.Empty;
        //get curricula for NodeID
        string sql = string.Format("exec Thinkgate_GetCurriculaValues {0}, '{1}'", nodeId, GetDistrictFromUserName());

        DataTable dt = ThinkgateKenticoHelper.GetDataTable(CmsConnectionString, sql);
        if (dt.Rows.Count > 0)
        {
            var myEnumerable = dt.AsEnumerable();

            List<TGCurriculum> tgCurriculumList =
                (from item in myEnumerable
                 select new TGCurriculum
                 {
                     ID = item.Field<int>("ID").ToString(),
                     Grade = item.Field<string>("Grade"),
                     Subject = item.Field<string>("Subject"),
                     Course = item.Field<string>("Course")
                 }).ToList();

            retVal = Render.StringToString(CurriculumTemplate, tgCurriculumList);
        }
        return retVal;
    }

    private class TGResources
    {
        public string ID { get; set; }
        public string ResourceURL { get; set; }
        public string ResourceType { get; set; }
        public string ResourceName { get; set; }
        public string Description { get; set; }
    }
    private static string BuildResourcesTag(string nodeId)
    {
        string userName = CMSContext.CurrentUser.UserName;
        string clientid = ThinkgateKenticoHelper.getClientIDFromKenticoUserName(userName);
        string clientFolder = ThinkgateKenticoHelper.getState(clientid);
        bool viewPermission = (bool)CMS.CMSHelper.CMSMacroMethods.HasMembership(CMSContext.CurrentUser, "TG_Edit_IMUsageRight_ExpirationDate", false) || (bool)CMS.CMSHelper.CMSMacroMethods.HasMembership(CMSContext.CurrentUser, "TG_View_IMUsageRight_ExpiredContent", false);
        string retVal = string.Empty;
        string sql = string.Format("exec Thinkgate_GetDocumentResources {0}, '{1}', '{2}', '{3}', '{4}'", nodeId, clientFolder, clientid, userName, viewPermission);
        DataTable dt = ThinkgateKenticoHelper.GetDataTable(CmsConnectionString, sql);
        if (dt.Rows.Count > 0)
        {
            var myEnumerable = dt.AsEnumerable();

            List<TGResources> tgResourcesList =
                (from item in myEnumerable
                 select new TGResources
                 {

                     ID = item.Field<int>("ID").ToString(),
                     ResourceURL = HttpContext.Current.Request.ApplicationPath + item.Field<string>("NodeAliasPath").Trim().Replace(" ", "-") + ".aspx",
                     ResourceType = item.Field<string>("Type"),
                     ResourceName = item.Field<string>("Name"),
                     Description = item.Field<string>("Description")
                 }).ToList();

            retVal = Render.StringToString(ResourceTemplate, tgResourcesList);
        }
        return retVal;
    }

    private static string BuildCreativeCommonsTag()
    {
        //In this case there is just static HTML in _creativeCommonsTemplate, so no need to pass in an object (could just retun the template...)
        string retVal = Render.StringToString(CreativeCommonsTemplate, null);
        return retVal;
    }

    private static string BuildCreativeCommonsTag1()
    {
        //In this case there is just static HTML in _creativeCommonsTemplate, so no need to pass in an object (could just retun the template...)
        string retVal = Render.StringToString(CreativeCommonsTemplate1, null);
        return retVal;
    }

    private static string Build_MA_CreativeCommonsTag()
    {


        //In this case there is just static HTML in _creativeCommonsTemplate, so no need to pass in an object (could just retun the template...)
        string retVal = Render.StringToString(MA_CreativeCommonsTemplate, null);
        return retVal;
    }

    private static string GetDistrictFromUserName()
    {
        string username = CMS.CMSHelper.CMSContext.CurrentUser.UserName;
        string[] wrk = username.Split('-');

        string theDistrict = wrk[0];

        return theDistrict;
    }

    #region Assessment Associations

    #region Private Methods

    private static string BuildAssessmentsTag(string nodeId)
    {
        string assessmentsTemplate = string.Empty;

        dtGeneric_Int assessmentsIDs = getCurrentAssessmentsList(nodeId);

        SqlParameterCollection sqlParameterCollection = new SqlCommand().Parameters;

        SqlParameter sqlParameter = new SqlParameter("@DocumentId", SqlDbType.Int);
        sqlParameter.Value = nodeId;
        sqlParameterCollection.Add(sqlParameter);

        sqlParameter = new SqlParameter("@ClientDatabaseName", SqlDbType.VarChar);
        sqlParameter.Value = GetDistrictFromUserName();
        sqlParameterCollection.Add(sqlParameter);

        sqlParameter = new SqlParameter("@AssessmentsIDs", SqlDbType.Structured);
        sqlParameter.Value = assessmentsIDs.ToSql().Data;
        sqlParameter.TypeName = assessmentsIDs.ToSql().TypeName;
        sqlParameterCollection.Add(sqlParameter);

        DataTable assessments = ThinkgateKenticoHelper.Connection_StoredProc_DataTable("Thinkgate_GetAssessmentsValues", sqlParameterCollection, CmsConnectionString);


        string e3AssessmentsUrl = CookieHelper.GetValue(E3AssessmentsUrl);

        if (string.IsNullOrEmpty(e3AssessmentsUrl) || !e3AssessmentsUrl.Contains(GetDistrictFromUserName()))
        { e3AssessmentsUrl = BuildAssessmentsUrl(); }

        if (assessments.Rows.Count > 0)
        {
            var assessmentsValues = assessments.AsEnumerable();

            List<TGAssessments> tgAssessments =
                (from item in assessmentsValues
                 select new TGAssessments
                 {
                     ID = item.Field<int>("ID").ToString(),
                     Category = item.Field<string>("Category").ToString(),
                     Level = PortalName,
                     AssessmentUrl = e3AssessmentsUrl + item.Field<int>("ID").ToString() + "&testcategory=" + item.Field<string>("Category").ToString() + "&level=" + PortalName + "&IsKentico=true",
                     AssessmentName = item.Field<string>("AssessmentName"),
                     Description = item.Field<string>("Description"),
                     UserAccess = item.Field<int>("UserAccess")
                 }).ToList();

            assessmentsTemplate = @"<div class='defaultTemplate assessmentsTemplate'><table><tr><th align='left' style='width:351px;'>Assessment</th><th align='left'>Description</th></tr>[[rows]]</table></div>";
            string assessmentRow = @"<tr><td>{{AssessmentName}}</td> <td>{{{Description}}}</td></tr>";

            List<string> assessmentRowsHTML = new List<string>();

            foreach (var item in tgAssessments)
            {
                var hyperlink = @"<a href='{{AssessmentUrl}}' target='_blank' rel='license'>{{AssessmentName}}</a>";
                string assessmentRowHTML = assessmentRow.Replace("{{{Description}}}", item.Description);

                if (item.UserAccess == 1)
                { assessmentRowHTML = assessmentRowHTML.Replace("{{AssessmentName}}", hyperlink.Replace("{{AssessmentName}}", item.AssessmentName).Replace("{{AssessmentUrl}}", item.AssessmentUrl)); }
                else
                { assessmentRowHTML = assessmentRowHTML.Replace("{{AssessmentName}}", item.AssessmentName); }

                assessmentRowsHTML.Add(assessmentRowHTML);
            }

            StringBuilder assessmentsHTML = new StringBuilder();

            foreach (var onerow in assessmentRowsHTML)
            { assessmentsHTML.Append(onerow); }

            assessmentsTemplate = assessmentsTemplate.Replace("[[rows]]", assessmentsHTML.ToString());
        }
        return assessmentsTemplate;
    }

    private static string BuildAssessmentsUrl()
    {
        string e3AssessmentsUrl = string.Format(ConfigurationManager.AppSettings["CMSSiteUrl"] + "/{0}/Controls/Assessment/AssessmentAssignmentSearch_Expanded.aspx?encrypted=false&assessmentID=", GetDistrictFromUserName());

        CookieHelper.SetValue(E3AssessmentsUrl, e3AssessmentsUrl, System.DateTime.Now.AddHours(1));
        return e3AssessmentsUrl;
    }

    private static dtGeneric_Int getCurrentAssessmentsList(string docid)
    {
        dtGeneric_Int currList = GetCurriculumCourseIDs();

        string sql = string.Format("exec Thinkgate_GetAssessmentsYears {0}, '{1}'", docid, GetDistrictFromUserName());
        DataTable year = GetDataTable(CmsConnectionString, sql);

        string sqlCategory = string.Format("SELECT DISTINCT Category FROM TestTypes as T WHERE T.ExcludeFromSearchCriteria='No' ORDER BY Category asc");
        DataTable assessmentCategory = GetDataTable(ElementsConnectionString, sqlCategory);


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
        sqlParameter.Value = GetUserName();
        sqlParameterCollection.Add(sqlParameter);

        return ThinkgateKenticoHelper.Connection_StoredProc_DataTable("E3_Assessment_SearchByUser", sqlParameterCollection);
    }

    private static dtGeneric_Int GetCurriculumCourseIDs()
    {
        SqlParameterCollection sqlParameterCollection = new SqlCommand().Parameters;

        SqlParameter sqlParameter = new SqlParameter("@UserName", SqlDbType.NVarChar);
        sqlParameter.Value = GetUserName();
        sqlParameterCollection.Add(sqlParameter);

        DataTable dataTable = ThinkgateKenticoHelper.Connection_StoredProc_DataTable("E3_CurrCourseIDs_SearchByUser", sqlParameterCollection);

        string sql = "Active = 'Yes'";
        var curriculumCourseIDs = new dtGeneric_Int();
        DataRow[] dataRows = dataTable.Select(sql);

        if (dataRows.Count() > 0)
        {
            foreach (DataRow dataRow in dataRows)
            { curriculumCourseIDs.Add(Convert.ToInt32(dataRow["ID"])); }
        }

        return curriculumCourseIDs;
    }

    private static string GetUserName()
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
                ConnectionStringToUse = CmsConnectionString;
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

    #endregion

    private class TGAssessments
    {
        public string ID { get; set; }
        public string AssessmentUrl { get; set; }
        public string AssessmentName { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string Level { get; set; }
        public int UserAccess { get; set; }
    }

    public enum EntityTypes
    {
        District,
        School,
        Student,
        Teacher,
        State
    }

    #endregion

}