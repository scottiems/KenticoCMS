<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ResourceSelector.aspx.cs" Inherits="CMSFormControls_Selectors_Thinkgate_ResourceSelector" %>

<%@ Register Src="~/CMSWebParts/ThinkgateWebParts/Resources.ascx" TagPrefix="uc1" TagName="Resources" %>


<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <telerik:RadScriptManager ID="LRMIScriptManager" runat="server" EnableScriptCombine="false"></telerik:RadScriptManager>
    <div>
        <uc1:Resources runat="server" ID="Resources" />
    </div>
    </form>
</body>
</html>
