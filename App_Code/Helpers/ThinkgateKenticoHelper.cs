using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using CMS.CMSHelper;
using CMS.DocumentEngine;
using CMS.SiteProvider;
using Google.Apis.Util;
using Standpoint.Core.ExtensionMethods;
using Thinkgate.Services.Contracts.CommonService;


public class ThinkgateKenticoHelper
{
    private static readonly string ElementsConnectionString = ConfigurationManager.ConnectionStrings["root_application"].ConnectionString;
    private static readonly string CMSConnectionString = ConfigurationManager.ConnectionStrings["CMSConnectionString"].ConnectionString;
    private const int DefaultNumberOfRecordsToReturn = 300;

    private enum ResourceTypes
    {
        None = 0,
        MyDocuments = 1,
        DistrictDocuments = 2,
        SharedDocuments = 3,
    }

    public static string getClientIDFromKenticoUserName(string kenticoUserName)
    {

        string[] clientID = kenticoUserName.Split('-');

        if (!string.IsNullOrEmpty(clientID[0]))
        {
            return clientID[0];
        }
        return string.Empty;
    }

    public static bool CheckValidStateLEA(UserInfo ui, string clientList)
    {
        if (String.IsNullOrEmpty(clientList))
            return true;
        String clientId = getClientIDFromKenticoUserName(ui.UserName);
        string clientState = getParmValue("State");
        string[] seperator = { "," };
        var clientListArray = clientList.Split(seperator, StringSplitOptions.RemoveEmptyEntries);
        foreach (string clientItem in clientListArray)
        {
            if (clientId.ToLower().Equals(clientItem.ToLower()))
                return true;
            if (clientState.ToLower().Equals(clientItem.ToLower()))
                return true;
        }
        return false;
    }

    public static DataSet SearchDocumentTypeReferences(UserInfo ui, string className, TreeProvider treeProvider, string filterType, bool filterExpiredContent = false)
    {
        DataSet sharedDocDataSet = new DataSet();
        string clientId = getClientIDFromKenticoUserName(ui.UserName);

        if (string.IsNullOrEmpty(clientId))
        {
            return sharedDocDataSet;
        }

        string envState = getState(clientId);
        string nodeAlias = string.Empty;
        string userName = ui.UserName;

        DataSet userdocs = null;
        DataSet disDocs = null;
        DataSet stateDocs = null;
        DataSet sharedDocs = null;

        nodeAlias = "/" + envState + "/Users/" + userName + "/%";
        userdocs = SearchUserDocs(nodeAlias, className, ui, treeProvider, ResourceTypes.SharedDocuments, filterType, filterExpiredContent);
        if (userdocs != null && userdocs.Tables.Count > 0)
        {
            sharedDocDataSet.Merge(userdocs);
        }

        nodeAlias = "/" + envState + "/Districts/" + clientId + "/%";
        disDocs = SearchUserDocs(nodeAlias, className, ui, treeProvider, ResourceTypes.SharedDocuments, filterType, filterExpiredContent);
        if (disDocs != null && disDocs.Tables.Count > 0)
        {
            sharedDocDataSet.Merge(disDocs);
        }

        nodeAlias = "/" + envState + "/Documents/%";
        stateDocs = SearchUserDocs(nodeAlias, className, ui, treeProvider, ResourceTypes.SharedDocuments, filterType, filterExpiredContent);
        if (stateDocs != null && stateDocs.Tables.Count > 0)
        {
            sharedDocDataSet.Merge(stateDocs);
        }
        //US15667
        nodeAlias = "/" + envState + "/Shared/" + clientId + "/%";
        sharedDocs = SearchUserDocs(nodeAlias, className, ui, treeProvider, ResourceTypes.SharedDocuments, filterType, filterExpiredContent);
        if (sharedDocs != null && sharedDocs.Tables.Count > 0)
        {
            sharedDocDataSet.Merge(sharedDocs);
        }        

        if (sharedDocDataSet.Tables.Count > 0)
        {
            return sharedDocDataSet;
        }

        return null;

    }

    public static void UpdateDocumentTypeItem(int documentForeignKeyValue)
    {
        string query = "update t set t.Name=d.DocumentName from Thinkgate_CompetencyList t Join CMS_Document d on d.DocumentForeignKeyValue = t.CompetencyListID where d.DocumentNodeID=" + documentForeignKeyValue;
        string ConnectionString = ConfigurationManager.ConnectionStrings["CMSConnectionString"].ConnectionString;
        SqlConnection sqlconn = new SqlConnection(ConnectionString);
        SqlCommand cmd = new SqlCommand(query, sqlconn);
        try
        {
            sqlconn.Open();
            int i = cmd.ExecuteNonQuery();
        }
        catch (SqlException ex)
        {
            throw new Exception(String.Format("Records updation failed exception: {0}", ex.Message));
        }
        finally
        {
            sqlconn.Close();
        }
    }

    private static DataSet SearchUserDocs(string treeselectionparamters, string className, UserInfo ui, TreeProvider treeProvider, ResourceTypes ResourceTypes, string filterType, bool filterExpiredItem = false)
    {
        DataSet documents = null;
        DataSet dsResults = null;
        NodeSelectionParameters nsp = new NodeSelectionParameters();
        nsp.SiteName = CMSContext.CurrentSiteName;
        nsp.AliasPath = cleanUserAliasPath(treeselectionparamters);
        nsp.CultureCode = (string.IsNullOrEmpty(ui.PreferredCultureCode) ? "en-US" : ui.PreferredCultureCode); //CMSContext.CurrentUser.PreferredCultureCode;
        nsp.CombineWithDefaultCulture = false;
        nsp.ClassNames = className;
        nsp.Where = null; // Build WHERE condition according to the search expression
        nsp.OrderBy = "DocumentModifiedWhen desc"; // Order by DocumentName
        if (className.ToLower() == "thinkgate.curriculumunit")
        {
            nsp.MaxRelativeLevel = 1;
        }
        else
        {
            nsp.MaxRelativeLevel = -1;
        }

        nsp.SelectOnlyPublished = true;

        if (ResourceTypes == ResourceTypes.MyDocuments)
        {
            nsp.SelectOnlyPublished = false;
        }
        else
        {
            nsp.SelectOnlyPublished = true;
        }
        nsp.TopN = GetNumberOfRecordsToReturn(getParmValue("NumberOfKenticoRecordsToReturn"));

        string flagname = string.Empty;
        switch (className.ToLower())
        {
            case "thinkgate.unitplan":
            case "thinkgate.unitplan_scberkeley":
                flagname = "UnitPlanTemplateFlag";
                break;
            case "thinkgate.unitplan_ma":
                flagname = "UPMATemplateFlag";
                break;
            case "thinkgate.lessonplan":
            case "thinkgate.lessonplan_ma":
                flagname = "LessonPlanTemplateFlag";
                break;
            case "thinkgate.instructionalplan":
                flagname = "InstructionalPlanTemplateFlag";
                break;
            case "thinkgate.curriculumunit":
                flagname = "CurriculumUnitTemplateFlag";
                break;
            case "thinkgate.resource":
                flagname = "ResourceTemplateFlag";
                break;

        }

        if (className != "thinkgate.curriculumUnit" && className != "thinkgate.CurriculumUnit")
        {
            if (filterType == "Templates")
            {
                nsp.Where = "(" + flagname + " = '1')";
            }
            if (filterType == "Non-Templates")
            {
                nsp.Where = "(" + flagname + " = '0' OR " + flagname + " IS NULL)";
            }
        }

        if (filterExpiredItem)
        {
            if (!string.IsNullOrEmpty(nsp.Where))
                nsp.Where = nsp.Where + " AND ";
            nsp.Where = nsp.Where +
                        " (ExpirationDate IS NULL OR CONVERT (date, ExpirationDate) >= CONVERT (date, GETDATE()))";
        }
        CurrentUserInfo currentUserInfo = new CurrentUserInfo(ui, true);
        try
        {
            documents = DocumentHelper.GetDocuments(nsp, treeProvider);
            if (documents != null && documents.Tables[0].Rows.Count > 0)
            {
                //If the current user can't read the document then remove it from the result set
                dsResults = TreeSecurityProvider.FilterDataSetByPermissions(documents, NodePermissionsEnum.Read, currentUserInfo);
            }
            else
            {
                dsResults = documents;
            }
        }
        catch (Exception ex)
        {
            throw new Exception(String.Format("The node passed treeselectionparamters does not exists/document does not exist {0}. \n{1}", treeselectionparamters, ex.Message));
        }
        return dsResults;

    }

    private static int GetNumberOfRecordsToReturn(string districtParmValue)
    {
        if (!string.IsNullOrWhiteSpace(districtParmValue))
        {
            int numberOfKenticoRecordsToReturn;
            return int.TryParse(districtParmValue, out numberOfKenticoRecordsToReturn) ? numberOfKenticoRecordsToReturn : DefaultNumberOfRecordsToReturn;
        }
        else
        {
            return DefaultNumberOfRecordsToReturn;
        }
    }



    private static string cleanUserAliasPath(string userAliasPath)
    {
        string returnVal = userAliasPath;

        //so string manipulation...
        returnVal = userAliasPath.Replace(".", "-");

        return returnVal;
    }

    public static String getState(string clientId)
    {
        string expression = string.Empty;
        string envState = "DEV";
        expression = @"select State from Clients where ClientID='" + clientId + "'";
        string ConnectionString = ConfigurationManager.ConnectionStrings["root_application"].ConnectionString;
        SqlConnection conn = new SqlConnection(ConnectionString);
        SqlCommand command = new SqlCommand(expression, conn);
        try
        {
            conn.Open();
            envState = (string)command.ExecuteScalar();
        }
        catch (SqlException ex)
        {
            throw new Exception(String.Format("getState call failed exception: {0}", ex.Message));
        }
        finally
        {
            conn.Close();
        }
        return envState;
    }

    public static string FindDBName()
    {
        string dbname = null;
        string username = CMS.CMSHelper.CMSContext.CurrentUser.UserName;
        if (!string.IsNullOrEmpty(username))
        {
            string[] wrk = username.Split('-');

            var client = new CommonProxy();
            if (client != null && !string.IsNullOrEmpty(wrk[0]))
            {
                dbname = client.GetClientDBName(wrk[0]);
            }
        }
        return dbname;
    }

    public static string FindDBName(string clientId)
    {
        string dbname =  null;
        if (clientId != null)
            {
            var client = new CommonProxy();
            dbname = client.GetClientDBName(clientId);
            }
      
         return dbname;
    }


    public static string getLocalDbConnectionString(string connString)
    {
        return connString.Replace("ThinkgateConfig", FindDBName());
    }
    public static string getLocalDbConnectionString(string connString, string clientId)
    {
        return connString.Replace("ThinkgateConfig", FindDBName(clientId));
    }

    public static string getParmValue(string parmNameToGet)
    {
        string parmValue = string.Empty;
        string sql = @"SELECT Val FROM Parms Where Name = '" + parmNameToGet + "'";
        string ConnectionString = getLocalDbConnectionString(ElementsConnectionString);
        SqlConnection conn = new SqlConnection(ConnectionString);
        SqlCommand command = new SqlCommand(sql, conn);
        try
        {
            conn.Open();
            parmValue = (string)command.ExecuteScalar();
        }
        catch (SqlException ex)
        {
            //something
        }
        finally
        {
            conn.Close();
        }
        return parmValue;
    }

    public static DataTable GetDataTable(string connectionString, string query)
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

    public static void deleteStandard(int documentID, int standardID, bool deleteLRMI)
    {
        string query = string.Format("Delete from [dbo].[thinkgate_docToStandards] WHERE (docID={0} AND standardID={1}) ", documentID, standardID);
        GetDataTable(CMSConnectionString, query);
        if (deleteLRMI == true)
        {
            query = string.Format("select ld.* from [dbo].[thinkgate_docToLRMIDetails] ld join [dbo].[thinkgate_docToLRMI] l on ld.LRMIItemID = l.ItemID where l.DocumentID = {0} AND ld.EducationalAlignmentEnum in ({1},{2},{3})", documentID, (int)LookupDetails.Assessed, (int)LookupDetails.Teaches, (int)LookupDetails.Requires);
            DataTable dt = GetDataTable(CMSConnectionString, query);
            if (dt != null)
            {
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        string rowStandards = dr["EducationalAlignmentValues"].ToString();
                        if (!string.IsNullOrEmpty(rowStandards))
                        {
                            rowStandards = rowStandards.Remove(rowStandards.Length - 1, 1);
                            string[] standards = rowStandards.Split('|');
                            string newStandards = "";
                            foreach (string st in standards)
                            {
                                newStandards = st != standardID.ToString() ? newStandards + st + "|" : newStandards;
                            }
                            if ((rowStandards + "|") != newStandards)
                            {
                                query = string.Format("update [dbo].[thinkgate_docToLRMIDetails] set EducationalAlignmentValues='{0}' where ItemID = {1}", newStandards, Convert.ToInt32(dr["ItemID"]));
                                GetDataTable(CMSConnectionString, query);
                            }
                        }
                    }
                }
            }
        }
    }

    public static DataSet GetTileMapLookupDataSet(string resourceToShow)
    {
        UserInfo userInfo = UserInfoProvider.GetUserInfo(CMSContext.CurrentUser.UserName);
        string clientId = getClientIDFromKenticoUserName(userInfo.UserName);
        string clientState = getParmValue("State");

        int lookupval = GetDocTypeValueFromTileMap(resourceToShow);

        CustomTableItemProvider tp = new CustomTableItemProvider(userInfo);
        string filtercriteria = "LookupValue = " + lookupval + " and (ISNULL(StateLEA,'')='' or StateLEA = '" + clientId + "' or StateLEA = '" + clientState + "')";
        DataSet ds = tp.GetItems("thinkgate.TileMap_Lookup", filtercriteria, string.Empty);
        return ds;
    }

    public static List<string> GetUsersE3RoleList(UserInfo userInfo)
    {
        DataTable kenticoRoles = UserInfoProvider.GetUserRoles(CMSContext.CurrentUser);
        List<string> e3RoleList = new List<string>();
        foreach (DataRow dr in kenticoRoles.Rows)
        {
            if (dr["RoleName"].ToString().Contains("-Admin-"))
            {
                string e3Role = ConvertKenticoRoleToE3Role(dr["RoleName"].ToString());
                if (!e3Role.IsNullOrEmpty())
                e3RoleList.Add(e3Role);
            }
        }
        return e3RoleList;
    }

    public static string ConvertKenticoRoleToE3Role(string kenticoRole)
    {
        if (IEnumerableExtensions.IsNullOrEmpty(kenticoRole)) return string.Empty;
        string e3Role = kenticoRole.Substring(kenticoRole.LastIndexOf("-Admin-") + 7);
        e3Role = e3Role.Replace("_", " ");
        return e3Role;
    }

    public static int GetDocTypeValueFromTileMap(string resourceToShow)
    {
        UserInfo userInfo = UserInfoProvider.GetUserInfo(CMSContext.CurrentUser.UserName);
        CustomTableItemProvider tp = new CustomTableItemProvider(userInfo);
        DataSet lookupDataSet = tp.GetItems("thinkgate.TileMap", "BaseTileType = '" + resourceToShow + "' ", string.Empty);
        int lookupval = 0;
        if (lookupDataSet.Tables[0].Rows.Count > 0)
        {
            lookupval = (int)lookupDataSet.Tables[0].Rows[0]["Value"];
        }
        return lookupval;

    }
    public static DataTable Connection_StoredProc_DataTable(string storedProcedure, SqlParameterCollection sqlParamList = null)
    {
        SqlConnection connection = null;

        try
        {
            connection = new SqlConnection(getLocalDbConnectionString(ConfigurationManager.ConnectionStrings["root_application"].ConnectionString));

            SqlCommand sqlCommand = new SqlCommand(storedProcedure, connection);
            sqlCommand.CommandType = CommandType.StoredProcedure;
            if (sqlParamList != null)
                foreach (SqlParameter parm in sqlParamList)
                {
                    SqlParameter sqlParm = new SqlParameter(parm.ParameterName, parm.SqlDbType)
                    {
                        Value = parm.Value,
                        TypeName = parm.TypeName
                    };
                    sqlCommand.Parameters.Add(sqlParm);
                }

            SqlDataAdapter adapter = new SqlDataAdapter();

            adapter.SelectCommand = sqlCommand;

            DataTable table = new DataTable();
            adapter.Fill(table);

            return table;
        }
        catch (DataException ex)
        {
            Debug.WriteLine(ex.ToString());
            throw;
        }
        finally
        {
            if (connection != null)
            {
                connection.Close();
                connection.Dispose();
            }
        }
    }

    public static DataTable Connection_StoredProc_DataTable(string storedProcedure, SqlParameterCollection sqlParamList = null, string connectionString = null)
    {
        connectionString = connectionString == null ? CMSConnectionString : connectionString;
        SqlConnection sqlConnection = null;

        try
        {
            sqlConnection = new SqlConnection(connectionString);

            SqlCommand sqlCommand = new SqlCommand(storedProcedure, sqlConnection);
            sqlCommand.CommandType = CommandType.StoredProcedure;

            if (sqlParamList != null)
                foreach (SqlParameter sqlParameter in sqlParamList)
                {
                    SqlParameter sqlParm = new SqlParameter(sqlParameter.ParameterName, sqlParameter.SqlDbType)
                    {
                        Value = sqlParameter.Value,
                        TypeName = sqlParameter.TypeName
                    };
                    sqlCommand.Parameters.Add(sqlParm);
                }

            SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
            sqlDataAdapter.SelectCommand = sqlCommand;

            DataTable dataTable = new DataTable();
            sqlDataAdapter.Fill(dataTable);

            return dataTable;
        }
        catch (DataException ex)
        {
            Debug.WriteLine(ex.ToString());
            throw;
        }
        finally
        {
            if (sqlConnection != null)
            {
                sqlConnection.Close();
                sqlConnection.Dispose();
            }
        }
    }
}
