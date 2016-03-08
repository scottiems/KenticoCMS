<%@ Page Language="C#" AutoEventWireup="true" CodeFile="AssociationConsumer.aspx.cs" Inherits="CMSFormControls_Selectors_Thinkgate_AssociationConsumer" %>

<%@ Register Src="~/CMSWebParts/ThinkgateWebParts/AssociationFullControl.ascx" TagPrefix="uc1" TagName="AssociationFullControl" %>
<%@ Register Src="~/CMSWebParts/ThinkgateWebParts/ATBCurricula.ascx" TagPrefix="uc1" TagName="ATBCurricula" %>
<%@ Register Src="~/CMSWebParts/ThinkgateWebParts/ATBContainer.ascx" TagPrefix="uc1" TagName="ATBContainer" %>
<%--<%@ Register Src="~/CMSWebParts/ThinkgateWebParts/ATBStandards_Test.ascx" TagPrefix="uc1" TagName="ATBStandards_Test" %>--%>
<%@ Register Src="~/CMSWebParts/ThinkgateWebParts/StandardsBar.ascx" TagPrefix="uc1" TagName="StandardsBar" %>
<%@ Register Src="~/CMSWebParts/ThinkgateWebParts/AddDocumentControl.ascx" TagPrefix="uc1" TagName="AddDocumentControl" %>








<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>

  

	<meta name="viewport" content="width=device-width, initial-scale=1.0" />

	<script type="text/javascript" src="<%:this.ResolveUrl("~/")%>cmsscripts/jquery/jquery-1.9.1.min.js"></script>
	<script type="text/javascript" src="<%:this.ResolveUrl("~/")%>cmsscripts/jquery/jquery-migrate-1.1.0.min.js"></script>
	<script type="text/javascript" src="<%:this.ResolveUrl("~/")%>cmsscripts/jquery/jquery-core.js"></script>
	<script type="text/javascript" src="<%:this.ResolveUrl("~/")%>cmsscripts/jquery/jquery-ui/js/jquery-ui-1.10.0.custom.min.js"></script>
	<script type="text/javascript" src="<%:this.ResolveUrl("~/")%>cmsscripts/jquery/jquery-cookie.js"></script>
	<script type="text/javascript" src="<%:this.ResolveUrl("~/")%>cmsscripts/jquery/DataTables/js/jquery.dataTables.js"></script>

	<link href="<%:this.ResolveUrl("~/")%>cmsscripts/jquery/DataTables/css/demo_table_jui.css" rel="stylesheet" />
	<link href="<%:this.ResolveUrl("~/")%>cmsscripts/jquery/jquery-ui/css/smoothness/jquery-ui-1.10.0.custom.css" rel="stylesheet" />
	<link href="<%:this.ResolveUrl("~/")%>cmsscripts/bootstrap/css/bootstrap.min.css" rel="stylesheet" />

   
</head>
<body>
    <form id="form1" runat="server">
    <div>
        
<asp:scriptmanager ID="Scriptmanager1" runat="server"></asp:scriptmanager>
       
         
    
                <uc1:StandardsBar runat="server" ID="StandardsBar" />
        <br />
                <uc1:AddDocumentControl runat="server" ID="AddDocumentControl" />
              
    </div>
    </form>
</body>
</html>
