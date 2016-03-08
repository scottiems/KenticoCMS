<%@ Page Language="C#" AutoEventWireup="true" CodeFile="controlTest.aspx.cs" Inherits="CMSWebParts_ThinkgateWebParts_controlTest" %>

<%@ Register Src="~/CMSWebParts/ThinkgateWebParts/TGAssociationToobar.ascx" TagPrefix="uc1" TagName="TGAssociationToobar" %>
<%@ Register Src="~/CMSWebParts/ThinkgateWebParts/TGAssociationsToolbar_Curriculum.ascx" TagPrefix="uc1" TagName="TGAssociationsToolbar_Curriculum" %>
<%@ Register Src="~/CMSWebParts/ThinkgateWebParts/TGAssociationsToolbar_Standards.ascx" TagPrefix="uc1" TagName="TGAssociationsToolbar_Standards" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title></title>
	

	<link href="<%:this.ResolveUrl("~/")%>cmsscripts/bootstrap/css/bootstrap.min.css" rel="stylesheet" />
	<link href="<%:this.ResolveUrl("~/")%>cmsscripts/jquery/DataTables/css/demo_table_jui.css" rel="stylesheet" />
	<link href="<%:this.ResolveUrl("~/")%>cmsscripts/jquery/jquery-ui/css/smoothness/jquery-ui-1.10.0.custom.css" rel="stylesheet" />
	<link href="<%:this.ResolveUrl("~/")%>CMSWebParts/ThinkgateWebParts/css/associationToolbar.css" rel="stylesheet" />
	<link href="<%:this.ResolveUrl("~/")%>CMSWebParts/ThinkgateWebParts/css/tgwebparts.css" rel="stylesheet" />

	<script type="text/javascript" src="<%:this.ResolveUrl("~/")%>cmsscripts/jquery/jquery-1.9.1.min.js"></script>
	<script type="text/javascript" src="<%:this.ResolveUrl("~/")%>cmsscripts/jquery/jquery-migrate-1.1.0.min.js"></script>
	<script type="text/javascript" src="<%:this.ResolveUrl("~/")%>cmsscripts/jquery/jquery-core.js"></script>
	<script type="text/javascript" src="<%:this.ResolveUrl("~/")%>cmsscripts/jquery/jquery-ui/js/jquery-ui-1.10.0.custom.js"></script>
	<script type="text/javascript" src="<%:this.ResolveUrl("~/")%>cmsscripts/jquery/jquery-cookie.js"></script>
	<script type="text/javascript" src="<%:this.ResolveUrl("~/")%>cmsscripts/jquery/DataTables/js/jquery.dataTables.js"></script>
	<script type="text/javascript" src="<%:this.ResolveUrl("~/")%>CMSScripts/Custom/bootstrap/js/bootstrap.min.js"></script>
	<script type="text/javascript" src="<%:this.ResolveUrl("~/")%>CMSWebParts/ThinkgateWebParts/js/associationToolbar.js"></script>
	<script type="text/javascript" src="<%:this.ResolveUrl("~/")%>CMSWebParts/ThinkgateWebParts/js/tgwebparts.js"></script>
	<script type="text/javascript" src="<%:this.ResolveUrl("~/")%>CMSScripts/Custom/tgDivTools.js"></script>

	<script>
		var $j = jQuery.noConflict();
	</script>

</head>
	<body>
		<%--<uc1:TGAssociationToobar runat="server" ID="TGAssociationToobar1" DocumentID='<%# Eval("NodeID") %>' DocumentType="InstructionPlan" />--%>

		
		<div class="toolBar">
			<uc1:TGAssociationToobar runat="server" ID="TGAssociationToobar1" DocumentID="2000" DocumentType="InstructionPlan" />
			<uc1:TGAssociationsToolbar_Standards runat="server" ID="TGAssociationsToolbar_Standards" DocumentID="1100" DocumentType="InstructionPlan" />
			<uc1:TGAssociationsToolbar_Curriculum runat="server" ID="TGAssociationsToolbar_Curriculum" DocumentID="1000" DocumentType="InstructionPlan" />
			
			<uc1:TGAssociationsToolbar_Standards runat="server" ID="TGAssociationsToolbar_Standards1" DocumentID="1200" DocumentType="InstructionPlan" />
			<uc1:TGAssociationsToolbar_Curriculum runat="server" ID="TGAssociationsToolbar_Curriculum1" DocumentID="1300" DocumentType="InstructionPlan" />
		</div>
<%--			<div class="toolBar">
				<uc1:TGAssociationsToolbar_Standards runat="server" ID="TGAssociationsToolbar_Standards" />
				<uc1:TGAssociationsToolbar_Curriculum runat="server" ID="TGAssociationsToolbar_Curriculum" />
			</div>
		</uc1:TGAssociationToobar>--%>

	</body>
</html>
