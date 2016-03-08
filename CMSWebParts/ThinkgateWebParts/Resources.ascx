<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Resources.ascx.cs" Inherits="CMSWebParts_ThinkgateWebParts_Resources" %>

<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>

<!-- Add jQuery library -->
<script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jquery/1.8/jquery.min.js"></script>
<script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jqueryui/1.8/jquery-ui.min.js"></script>
<link href="http://ajax.googleapis.com/ajax/libs/jqueryui/1.8/themes/base/jquery-ui.css" rel="stylesheet" type="text/css" />


<style type="text/css">
  
</style>


<telerik:RadCodeBlock ID="RadCodeBlock" runat="server">
    <script type="text/javascript">
        function getSelectedResources() {
            var editor = getHtmlEditor();
            if (!editor) {
                return null;
            }

            var selected = [];
            if (editor && editor.insertHtml) {
                $("input[id*=chkSelect]").each(function (i, e) {
                    if (e.checked) {
                        var resourceID = e.offsetParent.parentElement.cells[1].children[0].value;
                        var resourceName = e.offsetParent.parentElement.cells[1].children[1].innerText;
                        var type = e.offsetParent.parentElement.cells[2].innerText;
                        var subtype = e.offsetParent.parentElement.cells[3].innerText;
                        var description = e.offsetParent.parentElement.cells[4].innerText;

                        var resource = [];
                        resource.push(resourceID);
                        resource.push(resourceName);
                        resource.push(type);
                        resource.push(subtype);
                        resource.push(description);

                        selected.push(resource);
                    }
                });
                return selected;
            }
        }

        function insertSelectedResourcesAsContent() {
            var selected = getSelectedResources();
            if (selected && selected.length > 0) {
                var editor = getHtmlEditor();
                if (editor) {
                    for (var i = 0; i < selected.length; i++) {

                        var resourceID = selected[i][0];
                        var resourceName = selected[i][1];
                        var type = selected[i][2];
                        var subtype = selected[i][3];
                        var description = selected[i][4];

                        var html = "<div style='margin-top: 5px;'><b><a resourceID='" + resourceID + "' target='_blank' href='<%=this.E3RootURL%>/Record/StandardsPage.aspx?RID=" + resourceID + "'>" + resourceName + "-" + subtype + "-" + description + "</a></b><br />" + resourceName + "." + "</div>";
                     insertElementIntoEditor(editor, html);
                 }
             }
         }
         CloseDialog();
         return true;
     }


     function insertSelectedResourcesAsLinks() {
         var selected = getSelectedResources();
         if (selected && selected.length > 0) {
             var editor = getHtmlEditor();
             if (editor) {
                 for (var i = 0; i < selected.length; i++) {
                     var html = "<div style='margin-top: 5px;'><a resourceid='" + selected[i][0] + "' target='_blank' href='<%=this.E3RootURL %>/Record/StandardsPage.aspx?RID=" + selected[i][0] + "'>" + selected[i][1] + "</a></div>";
                     insertElementIntoEditor(editor, html);
                 }
             }
         }
         CloseDialog();
         return true;
     }

     function insertElementIntoEditor(editor, html) {
         editor.focus(); // For IE
         var fragment = editor.getSelection().getRanges()[0].extractContents();
         var container = wopener.CKEDITOR.dom.element.createFromHtml(html, editor.document);
         fragment.appendTo(container);
         editor.insertElement(container);
         return container;
     }

     function getHtmlEditor() {
         var editor = undefined;
         if (!wopener) {
             return null;
         }
         if (wopener.CKEDITOR.currentEditor) {
             editor = wopener.CKEDITOR.currentEditor;
         }
         else if (!editor) {
             for (var i in wopener.CKEDITOR.instances) {
                 var currentInstance = i;
                 break;
             }
             editor = wopener.CKEDITOR.instances[currentInstance];
         }
         return editor;
     }

     function CloseRadWindow() {
         window.parent.closedialog();
         //var oWindow = GetRadWindow();
         //oWindow.close();
     }

     function GetRadWindow() {
         var oWindow = null;

         if (window.radWindow)
         { oWindow = window.radWindow; }
         else if (window.frameElement.radWindow)
         { oWindow = window.frameElement.radWindow; }

         return oWindow;
     }


     //Association count logic
     $(document).ready(function () {
         ctrlid = '<%= Request.QueryString["ctrl"]%>';
     });
         $(window).unload(function () {            
             var counter = '<%= AssociationCount %>';
         window.parent.CloseMe(counter, ctrlid);
     });

    </script>
</telerik:RadCodeBlock>

<%--<telerik:RadAjaxPanel runat="server" ID="ResourcePanel" LoadingPanelID="ResourceLoadingPanel" Height="400px" Width="1080px">--%>

    <table style="width: 950px;">
        <tr>
            <th align="left">Category : </th>
            <th align="left">Type : </th>
            <th align="left">Sub Type : </th>
            <th align="left">Resource Name : </th>
            <th align="left">Text Seaerch : </th>
            <th>&nbsp;</th>
            <th>&nbsp;</th>

        </tr>
        <tr>
            <td style="width: 180px;">
                <telerik:RadComboBox runat="server" Width="100px" ID="rcbCategory" Skin="Web20" EmptyMessage="<Select>" ToolTip="Select a Category" />
            </td>
            <td style="width: 200px;">
                <telerik:RadComboBox runat="server" Width="120px" ID="rcbType" Skin="Web20" EmptyMessage="<Select>" ToolTip="Select a Type" />
            </td>
            <td style="width: 280px;">
                <telerik:RadComboBox runat="server" Width="180px" ID="rcbSubType" Skin="Web20" EmptyMessage="<Select>" ToolTip="Select a Sub - Type" />
            </td>
            <td style="width: 230px;">
                <telerik:RadTextBox runat="server" ID="rtbResourceName" Skin="Web20" Width="150px" MaxLength="50" ToolTip="Enter resource name" />
            </td>
            <td style="width: 200px;">
                <telerik:RadTextBox runat="server" Width="120px" ID="rtbTextSearch" Skin="Web20" MaxLength="50" ToolTip="Enter Text Search" /></td>
            <%--<td><telerik:RadTextBox runat="server" ID="rtbTextSearchOption" Skin="Web20" MaxLength="200" ToolTip="Select Text Search option" /></td>--%>
            <td style="width: 200px;">
                <telerik:RadComboBox Width="120px" runat="server" ID="radSearchOption" ToolTip="Select text search option" />
            </td>
            <%--  <td><asp:TextBox id="txtchange" runat="server" OnTextChanged="txtchange_TextChanged"  ></asp:TextBox> </td>--%>
            <td style="width: 100px;">
                <telerik:RadButton runat="server" ID="RadButtonSearch" Skin="Web20" Text="Search" OnClick="RadButtonSearch_Click" />
            </td>
        </tr>
    </table>
    <div>

        <asp:Panel runat="server" ID="gridResultsPanel" Width="98%">
            <telerik:RadGrid runat="server" ID="radGridResults" AutoGenerateColumns="False" Width="95%"
                AllowFilteringByColumn="False" PageSize="20" AllowPaging="True" AllowSorting="True" Height="350"
                OnPageIndexChanged="RadGridResults_PageIndexChanged" AllowMultiRowSelection="true"
                OnItemDataBound="radGridResults_ItemDataBound" CssClass="assessmentSearchHeader" Skin="Web20" OnNeedDataSource="RadGrid_NeedDataSource">
                <PagerStyle Mode="NextPrevAndNumeric"></PagerStyle>
                <ClientSettings EnableRowHoverStyle="true">
                    <Selecting AllowRowSelect="True" UseClientSelectColumnOnly="true" />
                    <%-- <ClientEvents OnGridCreated="changeGridHeaderWidth" />--%>
                    <Scrolling AllowScroll="True" UseStaticHeaders="True" SaveScrollPosition="True" ScrollHeight="460px"></Scrolling>
                </ClientSettings>
                <MasterTableView TableLayout="Auto">
                    <Columns>
                        <telerik:GridTemplateColumn HeaderText="Select" DataField="ViewLink" ItemStyle-Font-Size="Small" HeaderStyle-Width="50" ItemStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <%--<a href="<%# ResolveUrl(Eval("ViewLink").ToString()) %>" target="_blank">
                                <asp:Image ID="Image1" runat="server" ImageUrl="../Images/ViewPage.png" Width="15" /></a>--%>
                                <asp:CheckBox ID="chkSelect" runat="server" ClientIDMode="Static" />
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridTemplateColumn HeaderText="Name" DataField="Name" UniqueName="Name" HeaderStyle-Width="180px"
                            ItemStyle-Font-Size="Small">
                            <ItemTemplate>
                                <asp:HiddenField ID="hdnDocID" runat="server" Value='<%# Eval("ID").ToString() %>' />
                                <asp:Label ID="lblResourceName" runat="server" Text='<%# Eval("Name").ToString() %>'></asp:Label>
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridBoundColumn DataField="Type" HeaderText="Type" ShowSortIcon="true" HeaderStyle-Width="200px"
                            ItemStyle-Font-Size="Small" />
                        <telerik:GridBoundColumn DataField="Subtype" HeaderText="Subtype" ShowSortIcon="true" HeaderStyle-Width="220px"
                            ItemStyle-Font-Size="Small" />
                        <telerik:GridBoundColumn DataField="Description" HeaderText="Description" ShowSortIcon="true"
                            ItemStyle-Font-Size="Small" />

                    </Columns>
                </MasterTableView>
            </telerik:RadGrid>
            <asp:PlaceHolder ID="initialDisplayText" runat="server"></asp:PlaceHolder>
        </asp:Panel>


    </div>
    <div>
        <br />
        <asp:Button ID="btnAddAsHyperlink" runat="server" Skin="Web20" Text="Add as Links" OnClientClick="return insertSelectedResourcesAsLinks();" />&nbsp;
            <asp:Button ID="btnAddAsContent" runat="server" Skin="Web20" Text="Add as Content" OnClientClick="return insertSelectedResourcesAsContent();" />&nbsp;            

            <asp:Button ID="btnSave" runat="server" Text="Associate Selected Resource to Document" Skin="Web20" OnClick="radButtonSave_Click" />
        <input id="btnClose" runat="server" type="button" skin="Web20" value="Cancel" onclick="CloseRadWindow();" />
        <input id="btnCloseCKEditor" runat="server" type="button" value="Cancel" onclick="CloseDialog();" />



    </div>
    <asp:HiddenField ID="HiddenField1" runat="server" Value="false" />
<%--</telerik:RadAjaxPanel>

<telerik:RadAjaxLoadingPanel ID="ResourceLoadingPanel" runat="server" Height="75px" Width="75px" Transparency="50" BackgroundPosition="Center">
    <img alt="Loading..." src="../../../CMSWebParts/Viewers/Effects/lightbox_files/images/loading.gif" style="border: 0;" />
</telerik:RadAjaxLoadingPanel>--%>

