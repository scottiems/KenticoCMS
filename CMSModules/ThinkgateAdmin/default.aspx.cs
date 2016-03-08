using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using System.Collections;
using System.Diagnostics;
using System.Web.Security;

using CMS.CMSHelper;
using CMS.GlobalEventHelper;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.UIControls;
using System.Text;

public partial class CMSModules_ThinkgateAdmin_default : CMSToolsPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!CMSContext.CurrentUser.IsAuthorizedPerResource("cms.thinkgateadmin", "read"))
        {
            RedirectToAccessDenied("cms.thinkgateadmin", "Read");
        }
    }

    protected void btnGetTime_Click(object sender, EventArgs e)
    {
        if (CMSContext.CurrentUser.IsAuthorizedPerResource("cms.thinkgateadmin", "gettime"))
        {
            lblTime.Text = DateTime.Now.ToString();
        }
        else
        {
            lblTime.Text = "You're not authorized to get the current date and time.";
        }
    }
    protected void ImportRoles_Click(object sender, EventArgs e)
    {
        string response = "Creating Roles.....<br/>";

        // INITIALIZE USER AND ROLE E3 PROVIDERS
        // THIS IS CONSIDERED TO BE A WORKAROUND UNTIL KENTICO
        // RELEASES A PREBUILT CONNECTOR FOR EXTERNAL USER STORES
        MembershipProvider mp = Membership.Providers["AspNetSqlMembershipProvider"];
        //MembershipUser theUser = mp.GetUser(e.UserName, false);
        MembershipRoleInfoProvider mrip = new MembershipRoleInfoProvider();
        RoleProvider rp = Roles.Providers["AspNetSqlRoleProvider"];

        string[] allRoles = rp.GetAllRoles();
        foreach (string role in allRoles)
        {
            //Don't add the E3 Administrator Role
            if (role != "Administrator")
            {
                CreateRole(role);
                response = response + "Added Role: " + role + "<br/>";
            }
        }

        Response.Text = response;
    }

    protected void Button2_Click(object sender, EventArgs e)
    {
        Stopwatch stopWatch = new Stopwatch();
        StringBuilder builder = new StringBuilder();
        stopWatch.Start();

        builder.Append("Creating Users and associating roles.....<br/>");
        int timeOut = Server.ScriptTimeout;
        Server.ScriptTimeout = 3600;   // Give it 1 hour = 3600 seconds


        // INITIALIZE USER AND ROLE E3 PROVIDERS
        // THIS IS CONSIDERED TO BE A WORKAROUND UNTIL KENTICO
        // RELEASES A PREBUILT CONNECTOR FOR EXTERNAL USER STORES
        MembershipProvider mp = Membership.Providers["AspNetSqlMembershipProvider"];
        //MembershipUser theUser = mp.GetUser(e.UserName, false);
        MembershipRoleInfoProvider mrip = new MembershipRoleInfoProvider();
        RoleProvider rp = Roles.Providers["AspNetSqlRoleProvider"];

        string[] allRoles = rp.GetAllRoles();

        int usersCount = 0;
        MembershipUserCollection muc = mp.GetAllUsers(0, 2147483647, out usersCount);
        string kenticoRole = "";
        string kenticoUsername = "";

        builder.Append("Users Found: " + usersCount + "<br/>");
        int usersCreated = 0;

        try
        {
            foreach (MembershipUser mu in muc)
            {
                if ((mu.UserName != "")  && (!mu.UserName.Contains(",")))
                {
                    CreateUser(mu);
                    builder.Append("User Created: " + mu.UserName + "&nbsp&nbsp&nbsp Number Of Roles:&nbsp");

                    kenticoUsername = mu.UserName.Replace(" ", "");
                    string[] roles = rp.GetRolesForUser(kenticoUsername);

                    if (roles.Length > 0)
                        builder.Append(roles.Length + "&nbsp Roles:&nbsp");
                    else
                        builder.Append(roles.Length + "&nbsp No Roles");

                    foreach (string role in roles)
                    {
                        UserInfo ui = UserInfoProvider.GetUserInfo(mu.UserName);

                        kenticoRole = role.Replace(" ", ""); //Kentico Roles Cannot contain spaces               

                        //If user is not already in role then add them
                        if (ui.IsInRole(kenticoRole, CMSContext.CurrentSiteName) == false)
                        {
                            CreateUserRole(kenticoUsername, kenticoRole);
                        } 
                        builder.Append(kenticoRole + ",&nbsp");
                    }

                    builder.Append("<br/>");
                    usersCreated++;
                }
            }
        }
        catch (Exception ex)
        {
            builder.Append(ex.Message);
        }

        Server.ScriptTimeout = timeOut;  //reset timeout

        stopWatch.Stop();
        // Get the elapsed time as a TimeSpan value.
        TimeSpan ts = stopWatch.Elapsed;

        builder.Append("<br/>Total Users Created: &nbsp" + usersCreated + "<br/>Total Time: &nbsp" + stopWatch.Elapsed);

        Response.Text = builder.ToString();
    }

    private bool CreateRole(string role)
    {
        bool success = true;
        string cmsRoleName = role.Replace(" ", ""); //Kentico Roles Cannot contain spaces

        //If Role doesn't exist then add it
        if (RoleInfoProvider.RoleExists(cmsRoleName, CMSContext.CurrentSiteName) == false)
        {
            RoleInfo ri = new RoleInfo();
            ri.SiteID = CMSContext.CurrentSiteID;
            ri.RoleName = role.Replace(" ", "");  //Kentico Roles Cannot contain spaces
            ri.DisplayName = role;
            ri.Description = "E3role name: " + role; //Store original name in description
            RoleInfoProvider.SetRoleInfo(ri);
        }

        return success;
    }

    private bool CreateUser(MembershipUser mu)
    {
        bool success = true;
        string kenticoUsername = "";
        kenticoUsername = mu.UserName.Replace(" ", "");

        if (UserInfoProvider.GetUserInfo(kenticoUsername) == null)
        {
            UserInfo ui = new UserInfo();
            ui.UserName = kenticoUsername;
            ui.Email = mu.Email;
            ui.UserEnabled = true;
            ui.UserIsExternal = true;
            ui.FullName = "E3user " + mu.UserName;
            ui.UserDescription = "E3user " + mu.UserName;
            ui.FirstName = mu.UserName;
            ui.LastName = mu.UserName;
            ui.IsEditor = true;
            ui.PreferredCultureCode = "en-us";

            UserInfoProvider.SetUserInfo(ui);
            UserInfoProvider.AddUserToSite(ui.UserName, CMSContext.CurrentSiteName);
        }

        return success;
    }

    private bool CreateUserRole(string username, string rolename)
    {
        // Get role and user objects                    
        RoleInfo role = RoleInfoProvider.GetRoleInfo(rolename, CMSContext.CurrentSiteID);
        UserInfo user = UserInfoProvider.GetUserInfo(username);

        if ((role != null) && (user != null))
        {
            // Create new user role object
            UserRoleInfo userRole = new UserRoleInfo();

            // Set the properties
            userRole.UserID = user.UserID;
            userRole.RoleID = role.RoleID;

            // Save the user role
            UserRoleInfoProvider.SetUserRoleInfo(userRole);

            return true;
        }

        return false;
    }
}