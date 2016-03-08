<%@ Control Language="C#" AutoEventWireup="true" CodeFile="TGAssociationsToolbar_Assessment.ascx.cs" Inherits="CMSWebParts_ThinkgateWebParts_TGAssociationsToolbar_Assessment" %>

<div id="<%= ID %>-<%= DocumentID %>-Assessment_tab" class="toolBarWrapper" style="">
    <span id="ToolBarItem_div_Assmt" class="toolBarItem" onclick='showAssessmentAssoc("<%= DocumentType %>","<%= DocumentID %>", "<%= ID %>");'>
        <div id="<%= ID %>-<%= DocumentID %>-lnkIAC_Assmt" class="defaultIcon curriculaIcon" style="background-image: url('<%:this.ResolveUrl("~/")%>/cmswebparts/thinkgatewebparts/images/curriculum-icon.png');"></div>
        <asp:HyperLink ID="HyperLink2" runat="server" Text="Assessments"></asp:HyperLink>
        <div id="<%= ID %>-<%= DocumentID %>-badge_div_Assmt" class="badge"><%= AssessmentAssociatonCount %></div>
    </span>
</div>
