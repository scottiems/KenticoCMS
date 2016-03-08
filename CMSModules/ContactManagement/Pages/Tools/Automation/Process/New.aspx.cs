using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.UIControls;
using CMS.OnlineMarketing;
using CMS.WorkflowEngine;
using CMS.SettingsProvider;

// Breadcrumbs
[Breadcrumbs(2)]
[Breadcrumb(0, "ma.process.list", "~/CMSModules/ContactManagement/Pages/Tools/Automation/List.aspx", null)]
[Breadcrumb(1, "ma.process.new")]

[Help("ma_process_new")]
public partial class CMSModules_ContactManagement_Pages_Tools_Automation_Process_New : CMSAutomationPage
{
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        if (!CurrentUser.IsGlobalAdministrator)
        {
            PageTitle title = CurrentMaster.Title;
            title.TitleText = GetString("ma.process.new");
            title.TitleImage = GetImageUrl("Objects/MA_AutomationProcess/new.png");
            title.HelpName = "helpTopic";
            title.HelpTopicName = "ma_process_new";
        }

        editElem.CurrentWorkflow.WorkflowAllowedObjects = ";" + OnlineMarketingObjectType.CONTACT + ";";
        editElem.CurrentWorkflow.WorkflowRecurrenceType = ProcessRecurrenceTypeEnum.Recurring;
        editElem.CurrentWorkflow.WorkflowType = WorkflowTypeEnum.Automation;

        editElem.Form.RedirectUrlAfterCreate = AddSiteQuery("Frameset.aspx?processid={%EditedObject.ID%}&saved=1", null);

        // Check permissions
        editElem.Form.SecurityCheck.Resource = ModuleEntry.ONLINEMARKETING;
        editElem.Form.SecurityCheck.Permission = "ManageProcesses";
    }
}
