<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_ImportExport_SiteManager_Site_New"
    Theme="Default" CodeFile="Site_New.aspx.cs" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Title="Site Manager - New Site" %>

<%@ Register Src="~/CMSAdminControls/UI/PageElements/PageTitle.ascx" TagName="PageTitle"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/ImportExport/Controls/NewSiteWizard.ascx" TagName="NewSiteWizard"
    TagPrefix="cms" %>
<asp:Content runat="server" ID="cntContent" ContentPlaceHolderID="plcBeforeContent">
    <asp:Panel ID="PanelBody" runat="server" CssClass="PageBody">
        <asp:Panel ID="PanelTitle" runat="server" CssClass="PageHeader SimpleHeader">
            <cms:PageTitle ID="ptNewSite" runat="server" HelpTopicName="step_1" />
        </asp:Panel>
        <asp:Panel ID="PanelNewSite" runat="server" CssClass="PageContent">
            <cms:NewSiteWizard ID="NewSiteWizard" runat="server" />
        </asp:Panel>
    </asp:Panel>
</asp:Content>
