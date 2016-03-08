<%@ Page Language="C#" AutoEventWireup="true" CodeFile="TestControl.aspx.cs" Inherits="CMSWebParts_ThinkgateWebParts_TestControl" %>
<%@ Register Src="~/CMSWebParts/ThinkgateWebParts/TGAssociationToobar.ascx" TagPrefix="uc1" TagName="TGAssociationToobar" %>
<%@ Register Src="~/CMSWebParts/ThinkgateWebParts/TGAssociationsToolbar_Curriculum.ascx" TagPrefix="uc1" TagName="TGAssociationsToolbar_Curriculum" %>
<%@ Register Src="~/CMSWebParts/ThinkgateWebParts/TGAssociationsToolbar_Standards.ascx" TagPrefix="uc1" TagName="TGAssociationsToolbar_Standards" %>
<%@ Register Src="~/CMSWebParts/ThinkgateWebParts/TGAssociationsToolbar_LRMI.ascx" TagPrefix="uc1" TagName="TGAssociationsToolbar_LRMI" %>

<%@ Register Src="~/CMSWebParts/ThinkgateWebParts/TimelineControl.ascx" TagPrefix="uc1" TagName="TimelineControl" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title></title>

	<script type="text/javascript" src="<%:this.ResolveUrl("~/")%>CMSScripts/Custom/jquery-1.9.1.min.js"></script>
	<script type="text/javascript" src="<%:this.ResolveUrl("~/")%>CMSScripts/Custom/jquery-migrate-1.1.0.min.js"></script>
	<script type="text/javascript" src="<%:this.ResolveUrl("~/")%>CMSScripts/Custom/jquery-core.js"></script>
	<script type="text/javascript" src="<%:this.ResolveUrl("~/")%>CMSScripts/Custom/jquery-ui-1.10.0.custom.js"></script>
	<script type="text/javascript" src="<%:this.ResolveUrl("~/")%>CMSScripts/Custom/jquery-cookie.js"></script>
	<script type="text/javascript" src="<%:this.ResolveUrl("~/")%>CMSScripts/Custom/jquery.dataTables.js"></script>
	<script type="text/javascript" src="<%:this.ResolveUrl("~/")%>CMSScripts/Custom/bootstrap/js/bootstrap.min.js"></script>
	<script type="text/javascript" src="<%:this.ResolveUrl("~/")%>CMSScripts/Custom/addNewDocument.js"></script>
	<script type="text/javascript" src="<%:this.ResolveUrl("~/")%>CMSScripts/Custom/tgDivTools.js"></script>
	<script type="text/javascript" src="<%:this.ResolveUrl("~/")%>CMSScripts/Custom/timeline/timeline.js"></script>
	<script type="text/javascript" src="<%:this.ResolveUrl("~/")%>CMSScripts/Custom/deleteDocument.js"></script>
	
	<!-- IP Header begin -->
	<link href="<%:this.ResolveUrl("~/")%>cmsscripts/jquery/DataTables/css/demo_table_jui.css" rel="stylesheet" />
	<link href="<%:this.ResolveUrl("~/")%>cmsscripts/jquery/jquery-ui/css/smoothness/jquery-ui-1.10.0.custom.css" rel="stylesheet" />
	<link href="<%:this.ResolveUrl("~/")%>CMSScripts/Custom/bootstrap/css/bootstrap.min.css" rel="stylesheet" />
	<link href="<%:this.ResolveUrl("~/")%>CMSWebParts/ThinkgateWebparts/css/tgwebparts.css" rel="stylesheet" />
	<link href="<%:this.ResolveUrl("~/")%>CMSWebParts/ThinkgateWebparts/css/associationToolbar.css" rel="stylesheet" />
	<link href="<%:this.ResolveUrl("~/")%>CMSScripts/Custom/timeline/timeline.css" rel="stylesheet" />

</head>
<body>
	<form id="form1" runat="server">


	  <div class="toolBar">
		  <uc1:TGAssociationToobar runat="server" ID="TGAssociationToobar1" DocumentID="764" DocumentType="InstructionPlan" />    
		  <uc1:TGAssociationsToolbar_Standards runat="server" ID="TGAssociationsToolbar_Standards" DocumentID="764" DocumentType="InstructionPlan" />
		  <uc1:TGAssociationsToolbar_Curriculum runat="server" ID="TGAssociationsToolbar_Curriculum" DocumentID="764" DocumentType="InstructionPlan" />
		  <uc1:TGAssociationsToolbar_LRMI runat="server" ID="TGAssociationsToolbar_LRMI" DocumentID="764" DocumentType="InstructionPlan" />
	  </div>
<br />
<br />
	<uc1:TimelineControl runat="server" ID="TimelineControl"  DocumentID="" /> <!-- prod 511   | DEV 1936 2072 2438-->
	</form>
</body>
</html>
