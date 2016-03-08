using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.SettingsProvider;
using System.Data;
using CMS.CMSHelper;

namespace CMS.Controls
{
    /// <summary>
    /// Summary description for CMSTransformation
    /// </summary>
    public partial class CMSTransformation
    {
        protected const float MinutesInADay = 1440;
        protected const int DefaultDecimalPlace = 2;

        public CMSTransformation()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        /// <summary>
        /// Converts minutes to Hours
        /// </summary>
        /// <param name="minutes"></param>
        /// /// <param name="decimalPlaces"></param>
        /// <returns>Decimal as string to two places</returns>
        public string ConvertMinutesToDays(object minutes, object decimalPlaces = null)
        {
            if (minutes == null || minutes == DBNull.Value)
                return string.Empty;

            int formatDecimalPlaces;
            if (decimalPlaces == null || decimalPlaces == DBNull.Value)
                formatDecimalPlaces = DefaultDecimalPlace;
            else
                if (!int.TryParse(decimalPlaces.ToString(), out formatDecimalPlaces)) formatDecimalPlaces = DefaultDecimalPlace;

            int minutesToBeConverted;
            if (!int.TryParse(minutes.ToString(), out minutesToBeConverted)) return string.Empty;

            var formatString = string.Concat("#.", new string('#', formatDecimalPlaces));
            return minutesToBeConverted < 0 ? string.Empty : (minutesToBeConverted / Convert.ToDouble(MinutesInADay)).ToString(formatString);
        }

        public string ConvertDaysToMinutes(object days)
        {
            if (days == null || days == DBNull.Value)
                return string.Empty;

            double daysToBeConverted;
            if (!double.TryParse(days.ToString(), out daysToBeConverted)) 
                return string.Empty;

            return (daysToBeConverted * MinutesInADay).ToString(CultureInfo.InvariantCulture);
        }

        public string GetLookUpValue(int ID)
        {
            try
            {

                DataClassInfo customTable = DataClassInfoProvider.GetDataClass("TG.LookupDetails");
                if (customTable != null)
                {
                    DataSet lookupDataSet = (new CMS.SiteProvider.CustomTableItemProvider(CMSContext.CurrentUser)).GetItems("TG.LookupDetails", string.Empty, string.Empty);
                    if (lookupDataSet != null && lookupDataSet.Tables.Count > 0 && lookupDataSet.Tables[0].Rows.Count > 0)
                    {

                        DataRow[] row = lookupDataSet.Tables[0].Select("Enum=" + ID.ToString());
                        if (row.Any())
                        {
                            return row[0]["Description"].ToString();
                        }

                    }
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }


        public bool ShowHideResourceControl()
        {

            int NodeID = CMSContext.CurrentDocument.NodeID;

            bool result = false;
           // if (CMS.GlobalHelper.ValidationHelper.GetInteger(nodeID, 0) != 0)
            //{
                TreeNode tNode = CMS.CMSHelper.TreeHelper.SelectSingleNode(NodeID);

                bool isStateUser = false;
                

                string kenticoRoles = string.Empty;


                if (tNode != null)
                {

                    int ownerID = tNode.NodeOwner;
                    CMS.SiteProvider.UserInfo ownerInfo = CMS.SiteProvider.UserInfoProvider.GetUserInfo(ownerID);


                    kenticoRoles = getKenticoUserRoles(ownerInfo);


                    if (kenticoRoles.ToString().ToUpper().Contains("STATE_USER"))
                    {
                        isStateUser = true;
                    }




                    //If owner is state user and current logged in user is state user then document can be deleted.
                    kenticoRoles = getKenticoUserRoles(CurrentUser);
                    if (kenticoRoles.ToString().ToUpper().Contains("STATE_USER"))
                    {
                        if (isStateUser)
                        {
                            result = true;
                        }
                    }
                    //If owner is state user and current logged in user is District Administrator then document can not be deleted only viewed.
                    else if (kenticoRoles.ToString().ToUpper().Contains("DISTRICT_ADMINISTRATOR"))
                    {
                        if (isStateUser)
                        {
                            result = false;
                        }
                    }

                    if (ownerInfo.UserID == CurrentUser.UserID)
                    {
                        result = true;
                    }
                    //if (!result)
                    //{
                    //    if (CMS.DocumentEngine.TreeSecurityProvider.IsAuthorizedPerNode(tNode, CMS.DocumentEngine.NodePermissionsEnum.Delete, CurrentUser) == CMS.DocumentEngine.AuthorizationResultEnum.Allowed)
                    //    {
                    //        result = true;
                    //    }
                    //}

                }

              
                
            //}
            return result;
        }


        internal  string getKenticoUserRoles(CMS.SiteProvider.UserInfo user)
        {
            
            DataTable table = new DataTable();
            string colValue = string.Empty;
           
            if (user != null)
            {
                // Get table with all user roles
                
                table = CMS.SiteProvider.UserInfoProvider.GetUserRoles(user);
                if (table != null)
                {
                    // the datatable now contains the users that belong to the given role
                    for (int i = 0; i < table.Rows.Count; i++)
                    {
                        colValue += string.Format(" {0} : {1} ", table.Rows[i]["RoleName"].ToString(), table.Rows[i]["RoleID"].ToString());
                    }
                }
            }
            return colValue;
        }

    }
}