<%@ Control Language="C#" AutoEventWireup="true" CodeFile="TGAssociationsToolbar_Standards.ascx.cs" Inherits="CMSWebParts_ThinkgateWebParts_TGAssociationsToolbar_Standards" %>


	<div id="<%= ID %>-<%= DocumentID %>-Standards_tab" class="toolBarWrapper" style="" >
		
		<span id="ToolBarItem_div_Std" class="toolBarItem" onclick='showStdAssoc("<%= DocumentType %>","<%= DocumentID %>", "<%= ID %>");'>
			<div id="<%= ID %>-<%= DocumentID %>-lnkIAC_Std" class="defaultIcon curriculaIcon"style="background-image: url('<%:this.ResolveUrl("~/")%>/cmswebparts/thinkgatewebparts/images/curriculum-icon.png');"></div>
			<asp:HyperLink ID="HyperLink1" runat="server" Text="Standards"></asp:HyperLink>
			<div id="<%= ID %>-<%= DocumentID %>-badge_div_Std" class="badge"><%= AssociationCount %></div>
		</span>
	</div>