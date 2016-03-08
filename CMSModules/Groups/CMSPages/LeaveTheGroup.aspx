<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Groups_CMSPages_LeaveTheGroup"
    MasterPageFile="~/CMSMasterPages/LiveSite/Dialogs/ModalDialogPage.master" Theme="Default"
    CodeFile="LeaveTheGroup.aspx.cs" %>

<%@ Register Src="~/CMSModules/Groups/Controls/GroupLeave.ascx" TagName="GroupLeave"
    TagPrefix="uc1" %>
<asp:Content ID="cntContent" runat="server" ContentPlaceHolderID="plcContent">
    <div class="PageContent">
        <div class="CommunityJoinTheGroup">
            <uc1:GroupLeave ID="groupLeaveElem" runat="server" DisplayButtons="false"/>
        </div>
    </div>
</asp:Content>
<asp:Content ID="cntFooter" runat="server" ContentPlaceHolderID="plcFooter">
    <div class="FloatRight">
        <cms:CMSButton runat="server" CssClass="SubmitButton" ID="btnLeave" EnableViewState="false" /><cms:CMSButton
            CssClass="SubmitButton" ID="btnCancel" OnClientClick="Close();" runat="server" EnableViewState="false" />
    </div>
</asp:Content>
