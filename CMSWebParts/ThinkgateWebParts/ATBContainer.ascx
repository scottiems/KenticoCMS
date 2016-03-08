<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ATBContainer.ascx.cs" Inherits="CMSWebParts_ThinkgateWebParts_ATBContainer" %>
<%@ Register Src="~/CMSWebParts/ThinkgateWebParts/ATBCurricula.ascx" TagPrefix="uc1" TagName="ATBCurricula" %>

<!--link href="css/associationToolbar.css" rel="stylesheet" /-->

<div>
	<asp:Panel ID="pnlATBContainer" runat="server" Style="width: auto; float: left; max-width: 200px; max-height: 100px;" ScrollBars="Auto">
		<asp:PlaceHolder runat="server" ID="phATBTemplatePlaceHolder"></asp:PlaceHolder>
	</asp:Panel>
</div>