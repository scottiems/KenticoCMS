<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ControlSettings.ascx.cs"
    Inherits="CMSModules_AdminControls_Controls_Class_FieldEditor_ControlSettings" %>
<asp:Panel ID="pnlSettings" runat="server" CssClass="FieldPanel">
    <cms:BasicForm ID="form" runat="server" />
    <cms:FormModeSwitch ID="fms" runat="server">
        <AdvancedModeLink ID="whatever" runat="server" CssClass="LinkSortDown FloatRight FieldTableMenu" ResourceString="fieldeditor.tabs.advanced"/>
        <SimpleModeLink runat="server" CssClass="LinkSortUp FloatRight FieldTableMenu" ResourceString="fieldeditor.tabs.simplified"/>
    </cms:FormModeSwitch>
</asp:Panel>
