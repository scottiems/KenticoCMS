<%@ Control Language="C#" AutoEventWireup="true" CodeFile="AddDocumentControl.ascx.cs" Inherits="CMSWebParts_ThinkgateWebParts_AddDocumentControl" %>

<asp:PlaceHolder runat="server" ID="showAddNew" Visible="false"> 
	<a class="lpheaderadda" onclick="showAddNewDocumentModal('<%= DocumentType %>','<%= DocumentTypeLabel %>','<%= DocumentID %>', '<%= ClassID %>','<%= ID %>')">
		<span class="lpheaderadd">Add New <%= AddNewLevelLabel %> </span>
	</a>
</asp:PlaceHolder>

<div id="<%= ID %>-<%= DocumentID %>-dialog-confirm" title="Add New <%= DocumentTypeLabel %>?" style="display: none">
	<input type="radio" name="<%= ID %>-<%= DocumentID %>_tgAddNewDoc" value="new" id="<%= ID %>-<%= DocumentID %>_radioBtnNew" checked /> Add New<br />
	<input type="radio" name="<%= ID %>-<%= DocumentID %>_tgAddNewDoc" value="existing" id="<%= ID %>-<%= DocumentID %>_radioBtnExisting" /> Copy Existing	
</div>


<div id="<%= ID %>-<%= DocumentID %>-dialog-choose-formtype" title="Please choose the <%= DocumentTypeLabel %> Form Type" style="display: none">
	 <div id="divFormTypeSelectionRadioBtns" runat="server">
    </div>
</div>

<div id="addNew-dialog" title="Add New <%= DocumentTypeLabel %>" style="display: none">
  <div id="addNew-dialog-content"></div>
</div>
<div id="addExisting-dialog" title="Add New <%= DocumentTypeLabel %>" style="display: none">
  <div id="addExisting-dialog-content"></div>
</div>

