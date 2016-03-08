<%@ Control Language="C#" AutoEventWireup="true" CodeFile="TGAssociationsToolbar_LRMI.ascx.cs" Inherits="CMSWebParts_ThinkgateWebParts_TGAssociationsToolbar_Curriculum" %>


<div id="<%= ID %>-<%= DocumentID %>-LRMI_tab" class="toolBarWrapper" style="">

    <span id="ToolBarItem_div_LRMI" class="toolBarItem" onclick='showLRMIAssoc(event,"<%= DocumentType %>","<%= DocumentID %>", "<%= ID %>");'>
        <div id="<%= ID %>-<%= DocumentID %>-lnkIAC_LRMI" class="defaultIcon tagsIcon" ></div>
        <asp:HyperLink ID="HyperLink3" runat="server" Text="Tags"></asp:HyperLink>
    </span>
</div>
