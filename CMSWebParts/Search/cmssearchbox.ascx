<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSWebParts_Search_cmssearchbox" CodeFile="~/CMSWebParts/Search/cmssearchbox.ascx.cs" %>
<asp:Panel ID="pnlSearch" runat="server" DefaultButton="btnImageButton" CssClass="searchBox">
    <asp:Label ID="lblSearch" runat="server" AssociatedControlID="txtWord" EnableViewState="false" /><cms:CMSTextBox
        ID="txtWord" runat="server" /><cms:CMSButton ID="btnGo" runat="server" OnClick="btnGo_Click"
            EnableViewState="false" /><asp:ImageButton ID="btnImageButton" runat="server" Visible="false"
                OnClick="btnImageButton_Click" EnableViewState="false" />
</asp:Panel>
