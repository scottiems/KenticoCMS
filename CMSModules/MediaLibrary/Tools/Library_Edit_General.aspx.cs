using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.GlobalHelper;
using CMS.MediaLibrary;
using CMS.UIControls;

public partial class CMSModules_MediaLibrary_Tools_Library_Edit_General : CMSMediaLibraryPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        MediaLibraryInfo mli = MediaLibraryInfoProvider.GetMediaLibraryInfo(QueryHelper.GetInteger("libraryid", 0));
        if (!MediaLibraryInfoProvider.IsUserAuthorizedPerLibrary(mli, "Read"))
        {
            RedirectToCMSDeskAccessDenied("cms.medialibrary", "Read");
        }
        EditedObject = mli;

        elemEdit.MediaLibraryID = mli.LibraryID;
        elemEdit.OnCheckPermissions += new CMSAdminControl.CheckPermissionsEventHandler(elemEdit_OnCheckPermissions);
    }


    private void elemEdit_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        MediaLibraryInfo mli = MediaLibraryInfoProvider.GetMediaLibraryInfo(QueryHelper.GetInteger("libraryid", 0));
        if (!MediaLibraryInfoProvider.IsUserAuthorizedPerLibrary(mli, "Read"))
        {
            RedirectToCMSDeskAccessDenied("cms.medialibrary", "Read");
        }
    }
}