<%@ Control Language="C#" AutoEventWireup="true" CodeFile="TGAssociationsToolbar_Resources.ascx.cs" Inherits="CMSWebParts_ThinkgateWebParts_TGAssociationsToolbar_Resources" %>

<div id="<%= ID %>-<%= DocumentID %>-Resources_tab" class="toolBarWrapper" style="">

    <span id="ToolBarItem_div_Res" class="toolBarItem" onclick='showResAssoc("<%= DocumentType %>","<%= DocumentID %>", "<%= ID %>");'>
        <div id="<%= ID %>-<%= DocumentID %>-lnkIAC_Res" class="defaultIcon curriculaIcon" style="background-image: url('<%:this.ResolveUrl("~/")%>/cmswebparts/thinkgatewebparts/images/curriculum-icon.png');"></div>
        <asp:HyperLink ID="HyperLink1" runat="server" Text="Resources"></asp:HyperLink>
        <div id="<%= ID %>-<%= DocumentID %>-badge_div_Res" class="badge"><%= AssociationCount %></div>
    </span>
</div>
