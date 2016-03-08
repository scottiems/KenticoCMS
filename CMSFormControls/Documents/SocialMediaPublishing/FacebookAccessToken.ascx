<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSFormControls_Documents_SocialMediaPublishing_FacebookAccessToken" CodeFile="FacebookAccessToken.ascx.cs" %>

<cms:CMSUpdatePanel ID="pnlUpdate" runat="server">
    <ContentTemplate>
        <cms:CMSTextBox ID="txtToken" runat="server" CssClass="SelectorTextBox" />
        <cms:LocalizedButton ID="btnSelect" runat="server" CssClass="ContentButton" ResourceString="socialnetworking.get"/>
        <asp:Label ID="lblMessage" runat="server" EnableViewState="false" style="display: block;" />
    </ContentTemplate>
</cms:CMSUpdatePanel>

