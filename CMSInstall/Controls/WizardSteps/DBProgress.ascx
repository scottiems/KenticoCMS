<%@ Control Language="C#" AutoEventWireup="true" CodeFile="DBProgress.ascx.cs" Inherits="CMSInstall_Controls_WizardSteps_DBProgress" %>
<div class="InstallDBProgressLabel">
    <asp:Label ID="lblDBProgress" runat="server" EnableViewState="false" />
</div>
<asp:Panel ID="pnlDBProgress" runat="server">
    <div class="InstallDBProgress">
        <table class="InstallWizard" border="0" cellpadding="0" cellspacing="0">
            <tr>
                <td align="left" style="vertical-align: top">
                    <div class="InstallProgress">
                        <div style="margin: 5px 0px 5px 5px;">
                            <asp:Literal ID="ltlDBProgress" runat="server" />
                        </div>
                    </div>
                </td>
            </tr>
        </table>
    </div>
</asp:Panel>
