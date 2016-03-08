<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SpecialPopulation.aspx.cs" Inherits="CMSWebParts_ThinkgateWebParts_IMRating_SpecialPopulation" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="X-UA-Compatible" content="IE=8" />
    <meta http-equiv="PRAGMA" content="NO-CACHE" />
    <meta http-equiv="Expires" content="-1" />
    <meta http-equiv="CACHE-CONTROL" content="NO-STORE" />
    <link href="RatingReview.css" rel="stylesheet" />
    <title></title>
</head>
<body class="lightBackground">
    <form id="form1" runat="server">
        
          <telerik:RadScriptManager ID="radScriptManager1" EnableScriptCombine="False" runat="server">
            <Scripts>
                <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.Core.js" />
                <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQuery.js" />
                <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQueryInclude.js" />
             
            </Scripts>
       </telerik:RadScriptManager>
    <div id="dvSP">
    <table>
        <tr>
            <td>
                <asp:Label ID="lblAge" CssClass="float-left" runat="server">Age</asp:Label>
            </td>
            <td>
                <asp:Label ID="lblGrade" CssClass="float-right" runat="server">Grade</asp:Label>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblAgeValue" CssClass="float-left" runat="server"/>
            </td>
            <td>
                <asp:Label ID="lblGradeValue" CssClass="float-right" runat="server"/>
             </td>
        </tr>
        <tr>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td colspan="2">
                <table>
                    <tr>
                        <asp:Repeater ID="rptSpecialPopulation" runat="server" OnItemDataBound="specialPopulations_ItemDataBound">
                            <ItemTemplate>
                                <td>
                                    <telerik:RadButton ID="chkSpecialPopulation" runat="server" Skin="Outlook" ButtonType="ToggleButton" Checked="false" 
                                        ToggleType="CheckBox" Enabled="false" AutoPostBack="false" Height="14"  Font-Size="12pt" ItemID='<%# Eval("ID") %>'></telerik:RadButton>
                                     <asp:Label ID="lblSpecialPopulation" runat="server"/>
                                </td>
                            </ItemTemplate>
                        </asp:Repeater>
                    </tr>
                </table>

            </td>
        </tr>
    </table>
</div>

</form>
</body>
</html>
