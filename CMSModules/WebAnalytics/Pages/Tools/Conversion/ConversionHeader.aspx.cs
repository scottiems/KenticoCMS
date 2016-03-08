using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.ExtendedControls;
using CMS.GlobalHelper;
using CMS.UIControls;

public partial class CMSModules_WebAnalytics_Pages_Tools_Conversion_ConversionHeader : CMSCampaignAndConversionPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string displayTabName = GetString(QueryHelper.GetString("displayTab", GetString("general.report")));
        string postBackRef = ScriptHelper.GetScript("function updateTabHeader () {" + ControlsHelper.GetPostBackEventReference(this, "") + "}");
        String statName = QueryHelper.GetString("statCodeName", String.Empty).Replace(".", "_");

        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "HeaderChanger", postBackRef);

        string[,] tabs = new string[2, 4];
        tabs[0, 0] = displayTabName;
        tabs[0, 1] = "SetHelpTopic('helpTopic', 'conversions_overview');";
        tabs[0, 2] = ResolveUrl("~/CMSModules/WebAnalytics/Pages/Tools/Campaign/CampaignReport.aspx" + URLHelper.Url.Query + "&displayTitle=0");

        tabs[1, 0] = GetString("analytics_codename.conversion");
        tabs[1, 1] = "SetHelpTopic('helpTopic', 'conversions_list');";
        tabs[1, 2] = "list.aspx?displayreport=" + QueryHelper.GetBoolean("displayreport", false);

        CurrentMaster.Tabs.Tabs = tabs;
        CurrentMaster.Tabs.UrlTarget = "content";

        PageTitle title = CurrentMaster.Title;
        title.TitleText = GetString(statName);
        title.TitleImage = GetImageUrl("Objects/Analytics_Conversion/object.png");

        // If stat is not 'conversion' (default) - use topic name from stat name
        String helpTopicName = "conversions_overview";                
        if (statName != "conversions")
        {
            helpTopicName = statName;
        }

        title.HelpTopicName = helpTopicName;
        title.HelpName = "helpTopic";

        // Register script for unimenu button selection
        AddMenuButtonSelectScript(this, "Conversions", null, "menu");
    }
}