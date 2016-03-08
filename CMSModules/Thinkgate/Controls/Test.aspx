<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Test.aspx.cs" Inherits="CMSModules_Thinkgate_Controls_Test" %>

<%@ Register Src="~/CMSModules/Thinkgate/Controls/UpDownButton.ascx" TagPrefix="uc1" TagName="UpDownButton" %>


<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="Styles/UpDownButton.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <uc1:UpDownButton runat="server" ID="UpDownButton" NumberChildren="5" Position="3" />
    </div>
    </form>
</body>
</html>
