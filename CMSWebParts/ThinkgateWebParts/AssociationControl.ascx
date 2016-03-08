<%@ Control Language="C#" AutoEventWireup="true" CodeFile="AssociationControl.ascx.cs" Inherits="CMSTemplates_CorporateSite_AssociationControl" %>
<script src="~/CMSWebParts/ThinkgateWebParts/js/associationToolbar.js" type="text/javascript"></script>

<div id="toolBarItem_div" class="toolBarItem" runat="server">
	<div id="lnkIAC" runat="server"></div>
	<asp:HyperLink ID="lnkAC" runat="server"></asp:HyperLink>
	<div id="badge_div" class="badge" runat="server"></div> </div>

<div id="modalContainer"></div>

<div id="addAssocToolBardialog" title="Associated"  style="display: none;">
    <iframe id="addAssocToolBarframe" style="width: 100%; height: 100%;" frameborder="0">No frames</iframe>
</div>
