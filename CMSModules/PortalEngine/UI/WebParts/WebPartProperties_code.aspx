<%@ Page Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_PortalEngine_UI_WebParts_WebPartProperties_code" EnableEventValidation="false" Theme="default"
    ValidateRequest="false" CodeFile="WebPartProperties_code.aspx.cs" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server" enableviewstate="false">
    <title>Webpart properties - Custom code</title>
</head>
<body class="TabsBody <%=mBodyClass%>">
    <form id="form1" runat="server">
        <asp:Panel runat="server" ID="pnlBody" CssClass="TabsPageBody">
            <asp:Panel runat="server" ID="pnlScroll" CssClass="TabsPageScrollArea">
                <asp:Panel runat="server" ID="pnlTab" CssClass="TabsPageContent">
                    <asp:Panel runat="server" ID="Panel1" CssClass="PageContent">
                        <cms:MessagesPlaceHolder ID="plcMess" runat="server" IsLiveSite="false" />
                        <asp:Label runat="server" ID="lblInfo" CssClass="InfoLabel" EnableViewState="false" />
                        <cms:ExtendedTextArea ID="txtCode" runat="server" EnableViewState="false" 
                            EditorMode="Advanced" Language="CSharp" Height="315px" />
                    </asp:Panel>
                </asp:Panel>
            </asp:Panel>
        </asp:Panel>
    </form>
</body>
</html>
