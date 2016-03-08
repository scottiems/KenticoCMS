<%@ Page Language="C#" EnableEventValidation="false" AutoEventWireup="true" CodeFile="LRMIAddNewAssoc.aspx.cs" Inherits="CMSWebParts_ThinkgateWebParts_LRMIAddNewAssoc" %>

<%@ Register Src="~/CMSWebParts/ThinkgateWebParts/LRMITags.ascx" TagPrefix="uc1" TagName="LRMITag" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title></title>
	<meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <link href="../../CMSScripts/jquery/jquery-ui/css/smoothness/jquery-ui-1.10.0.custom.css" rel="stylesheet" />
    <link href="../../App_Themes/Default/DesignMode.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <script type="text/javascript" src="<%:Request.ApplicationPath%>/CMSScripts/Custom/jquery-1.9.1.min.js"></script>
    <script type="text/javascript" src="<%:Request.ApplicationPath%>/CMSScripts/Custom/jquery-migrate-1.1.0.min.js"></script>
    <script type="text/javascript" src="<%:Request.ApplicationPath%>/CMSScripts/Custom/jquery-core.js"></script>
    <script type="text/javascript" src="<%:Request.ApplicationPath%>/CMSScripts/Custom/jquery-ui-1.10.0.custom.js"></script>
    <script type="text/javascript" src="<%:Request.ApplicationPath%>/CMSScripts/Custom/jquery-cookie.js"></script>
    <script type="text/javascript" src="<%:Request.ApplicationPath%>/CMSScripts/Custom/jquery.dataTables.js"></script>
    <script type="text/javascript" src="<%:Request.ApplicationPath%>/CMSScripts/Custom/bootstrap/js/bootstrap.min.js"></script>
    <script type="text/javascript" src="<%:Request.ApplicationPath%>/CMSScripts/Custom/addNewDocument.js"></script>
    <script type="text/javascript" src="<%:Request.ApplicationPath%>/CMSScripts/Custom/tgDivTools.js"></script>

    <script>var $j = jQuery.noConflict();</script>
    <script>

        $j(document).ready(function () {
            //
        });

    </script>

</head>
<body>
    <script>
		function showmessage() {
		    alert("Your changes were saved.");
		    closeDialog();
		}
		function closeDialog() {
		    window.parent.jQuery('#lrmi_addnew_association').dialog('close');
		}
	</script>
	<div class="css_clear"></div>
	<form id="frmLRMIAddNewAssoc" runat="server">
		<asp:ScriptManager ID="Scriptmanager1" runat="server"></asp:ScriptManager>
		<div id="tabs">
			<uc1:LRMITag runat="server" ID="LRMITag" />
		</div>
	</form>

</body>
</html>
