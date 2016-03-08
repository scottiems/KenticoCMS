<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Membership_FormControls_Users_UserName" CodeFile="UserName.ascx.cs" %>
<cms:CMSTextBox runat="server" ID="txtUserName" MaxLength="100" CssClass="TextBoxField" />
<cms:CMSRequiredFieldValidator ID="RequiredFieldValidatorUserName" runat="server" EnableViewState="false"
    Display="dynamic" ControlToValidate="txtUserName"  />
<cms:LocalizedLabel ID="lblUserName" AssociatedControlID="txtUserName" EnableViewState="false"
    ResourceString="general.username" Display="false" runat="server" />