<%@ Page Language="C#" AutoEventWireup="true" CodeFile="PageLayout_Templates.aspx.cs"
    Inherits="CMSModules_PortalEngine_UI_PageLayouts_PageLayout_Templates" Theme="Default"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Page Layouts - Templates" %>

<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <script type="text/javascript" language="javascript">
    //<![CDATA[
        function RefreshWOpener(w) {
            window.location = window.location.href;
        }
    //]]>
    </script>
    <cms:CMSPanel ID="pnlContentWrap" runat="server" ShortID="p">
        <cms:UniGrid ID="gridTemplates" runat="server" OrderBy="DisplayName" Columns="ObjectID, DisplayName, PageTemplateIsReusable"
            IsLiveSite="false" ObjectType="cms.pagetemplatelist" ShowObjectMenu="false">
            <GridActions Parameters="ObjectID">
                <ug:Action Name="edit" Caption="$General.Edit$" Icon="Edit.png" OnClick="EditPageTemplate({0}); return false;" />
            </GridActions>
            <GridColumns>
                <ug:Column Source="DisplayName" Caption="$general.displayname$" Wrap="false" Localize="true">
                    <Filter Type="text" />
                </ug:Column>
                <ug:Column Width="100%" />
            </GridColumns>
            <GridOptions DisplayFilter="true" />
        </cms:UniGrid>
    </cms:CMSPanel>
</asp:Content>
