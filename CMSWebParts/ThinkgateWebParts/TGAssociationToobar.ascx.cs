using System;

public partial class CMSWebParts_ThinkgateWebParts_TGAssociationToobar : TGAssociationToobarBase
{
	protected void Page_Load(object sender, EventArgs e)
	{
        if (Request.QueryString["portalName"] != null)
        {
            string portalName = Request.QueryString["portalName"].ToString();
            portalName = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(portalName.ToLower());
            bool isportalNameDefined = Enum.IsDefined(typeof(ThinkgateTemplateBuilder.EntityTypes), portalName);

            if (isportalNameDefined)
            {
                ThinkgateTemplateBuilder.PortalName = portalName;
            }
        }
	}
}