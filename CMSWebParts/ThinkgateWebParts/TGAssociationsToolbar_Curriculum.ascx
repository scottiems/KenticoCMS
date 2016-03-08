<%@ Control Language="C#" AutoEventWireup="true" CodeFile="TGAssociationsToolbar_Curriculum.ascx.cs" Inherits="CMSWebParts_ThinkgateWebParts_TGAssociationsToolbar_Curriculum" %>


	<div id="<%= ID %>-<%= DocumentID %>-Curricula_tab" class="toolBarWrapper" style="" >
		
		<span id="ToolBarItem_div_Curr" class="toolBarItem" onclick='showCurriculaAssoc("<%= DocumentType %>","<%= DocumentID %>", "<%= ID %>");'>
			<div id="<%= ID %>-<%= DocumentID %>-lnkIAC_Curr" class="defaultIcon curriculaIcon" style="background-image: url('<%:this.ResolveUrl("~/")%>/cmswebparts/thinkgatewebparts/images/curriculum-icon.png');"></div>
			<asp:HyperLink ID="HyperLink2" runat="server" Text="Curricula"></asp:HyperLink>
			<div id="<%= ID %>-<%= DocumentID %>-badge_div_Curr" class="badge"><%= CurriculaAssociatonCount %></div>
		</span>
	</div>