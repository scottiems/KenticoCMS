<%@ Page Language="C#" AutoEventWireup="true" CodeFile="StandardSelector.aspx.cs" Inherits="CMSFormControls_Selectors_Thinkgate_StandardSelector" %>

<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<%@ Register Src="~/CMSWebParts/ThinkgateWebParts/newDTS.ascx" TagPrefix="uc1" TagName="newDTS" %>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
        body
        {
            font-family: Arial, Verdana;
            font-size: 12pt;
        }

        div#main
        {
            width: 100%;
        }

        div#sidebar
        {
            position: absolute;
            height: auto;
            max-height: 700px;
            right: 0px;
            width: 200px;
            background-color: rgba(0, 0, 0, 0.1);
            -moz-border-radius: 15px;
            border-radius: 15px;
            padding: 1em;
            overflow: auto;
        }

        div#banner
        {
            position: absolute;
            margin-right: 245px;
            width: 95%;
        }

        .standardsList
        {
            font-weight: bold;
        }

            .standardsList:hover
            {
                color: blue;
            }

        .standardsDescription
        {
            padding-left: 20px;
            display: inline-block;
        }

        #addStandardIconDiv:hover
        {
            background-color: darkgray;
        }

        #addStandardIconDiv
        {
            display: block;
            position: relative;
            float: right;
            border: solid 1px gray;
            border-radius: 15px;
            padding: 3px;
            cursor: pointer;
            margin-top: 5px;
        }

        #addStandardIcon
        {
            top: 5px;
            width: 16px;
            height: 16px;
            background: url('../../../a.aspx?cmsimg=/ug/Add.png') no-repeat 0 0;
            float: right;
            margin-left: 5px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <telerik:RadScriptManager ID="RadScriptManager1" runat="server" EnableScriptCombine="false">
        </telerik:RadScriptManager>
        <div>
            <uc1:newDTS runat="server" ID="newDTS" />
        </div>
    </form>
</body>
</html>
