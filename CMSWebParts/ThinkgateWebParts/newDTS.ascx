<%@ Control Language="C#" AutoEventWireup="true" CodeFile="newDTS.ascx.cs" Inherits="CMSWebParts_ThinkgateWebParts_newDTS" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>

<!-- Add jQuery library -->
<link href="http://ajax.googleapis.com/ajax/libs/jqueryui/1.8/themes/base/jquery-ui.css" rel="stylesheet" type="text/css" />
<script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jquery/1.8/jquery.min.js"></script>
<script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jqueryui/1.8/jquery-ui.min.js"></script>

<b><i>
    <asp:Label ID="Label1" runat="server" Visible="false" Text="Label"></asp:Label></i></b>
<br />
<style type="text/css">
    /* CSS is stored in the webpart "CSS" section */
</style>

<telerik:RadCodeBlock ID="RadCodeBlock" runat="server">

    <script type="text/javascript">

        $(document).ready(function () {
            if (checkHiddenVal()) {
                $("#standardsSelectionDiv").css("display", "block");
            }
        });

        function addStandards() {
            $("#standardsSelectionDiv").css("display", "block");
            document.getElementById("<% =HiddenField1.ClientID %>").value = "true";
        }

        function checkHiddenVal() {
            var theVal = document.getElementById("<% =HiddenField1.ClientID %>").value;

            if (theVal != null && theVal.toUpperCase() == "TRUE") {
                return true;
            } else {
                return false;
            }
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

        function getSelectedStandards() {
            var editor = getHtmlEditor();
            if (!editor) {
                return null;
            }
            var selected = [];
            if (editor && editor.insertHtml) {
                $("input[id*=columnSelectCheckBox]").each(function (i, e) {
                    if (e.checked) {
                        var standardId = e.offsetParent.parentElement.cells[1].innerText;
                        var standardSet = e.offsetParent.parentElement.cells[2].innerText;
                        var grade = e.offsetParent.parentElement.cells[3].innerText;
                        var subject = e.offsetParent.parentElement.cells[4].innerText;
                        var course = e.offsetParent.parentElement.cells[5].innerText;
                        var standardName = e.offsetParent.parentElement.cells[7].innerText;
                        var standardDesc = e.offsetParent.parentElement.cells[8].innerText;
                        var xID = e.offsetParent.parentElement.cells[10].innerText;

                        var standard = [];
                        standard.push(standardId);
                        standard.push(standardSet);
                        standard.push(grade);
                        standard.push(subject);
                        standard.push(course);
                        standard.push(standardName);
                        standard.push(standardDesc);
                        standard.push(xID);

                        selected.push(standard);
                    }
                });
                return selected;
            }
        }

        function insertSelectedStandardsAsContent() {
            var selected = getSelectedStandards();
            if (selected && selected.length > 0) {
                var editor = getHtmlEditor();
                if (editor) {
                    for (var i = 0; i < selected.length; i++) {

                        var id = selected[i][0];
                        var set = selected[i][1];
                        var grade = selected[i][2];
                        var subject = selected[i][3];
                        var course = selected[i][4];
                        var desc = selected[i][6];
                        var xid = selected[i][7];

                        var html = "<div style='margin-top: 5px;'><b><a standardid='" + id + "' target='_blank' href='<%=this.E3RootURL%>/Record/StandardsPage.aspx?xID=" + xid + "'>" + set + "-" + grade + "-" + subject + "-" + course + "</a></b><br />" + desc + "." + "</div>";
                        insertElementIntoEditor(editor, html);
                    }
                }
            }
            CloseDialog();
            return true;
        }

        function insertSelectedStandardsAsLinks() {
            var selected = getSelectedStandards();
            if (selected && selected.length > 0) {
                var editor = getHtmlEditor();
                if (editor) {
                    for (var i = 0; i < selected.length; i++) {
                        var html = "<div style='margin-top: 5px;'><a standardid='" + selected[i][0] + "' target='_blank' href='<%=this.E3RootURL %>/Record/StandardsPage.aspx?xID=" + selected[i][7] + "'>" + selected[i][5] + "</a></div>";
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

        function getSelectedStandardsForLRMI() {
            var selectedStandardIds = '';
            var selectedStandardName = '';

            $("input[id*=columnSelectCheckBox]").each(function (i, e) {
                if (e.checked) {
                    var standardId = e.offsetParent.parentElement.cells[1].innerText;
                    var standardName = e.offsetParent.parentElement.cells[7].innerText;

                    if (!isNaN(standardId)) {
                        selectedStandardIds += standardId + '|';
                        selectedStandardName += standardName + '<br>';
                    }
                }
            });

            //create the argument that will be returned to the parent page
            var selectedStandards = new Object();
            selectedStandards.StandardIds = selectedStandardIds;
            selectedStandards.StandardName = selectedStandardName;

            //close the RadWindow and send the argument to the parent page
            if (selectedStandardIds != '') {
                //var oWindow = GetRadWindow();
                //oWindow.close(selectedStandards);

            window.parent.closeModalDialog(selectedStandardIds, selectedStandardName);
        }
        else {
            alert("Please select at least one standard.");
            return false;
        }
    }


       

    function CloseRadWindow() {
        //var oWindow = GetRadWindow();
        //oWindow.close();
        window.parent.closedialog();
       
    }

    function GetRadWindow() {
        var oWindow = null;

        if (window.radWindow)
        { oWindow = window.radWindow; }
        else if (window.frameElement.radWindow)
        { oWindow = window.frameElement.radWindow; }

        return oWindow;
    }

    /* Association Count*/
    $(document).ready(function () {
        ctrlid = '<%= Request.QueryString["ctrl"]%>';
    });

    $(window).unload(function () {
       
        var counter = '<%= AssociationCount %>';
        var returnURL = '<%= Request.QueryString["ReturnURL"] %>'

        if (returnURL != null) {
            if (returnURL == "LRMI") {
                //window.parent.parent.CloseMe(counter, ctrlid);
            }
            else {
                window.parent.CloseMe(counter, ctrlid);
            }
        }
        else
            window.parent.CloseMe(counter, ctrlid);
    });

        function STDdialogCancelled() {

            window.parent.closeAssocToolBardialog();

            popupAssociateCurricula();
        }


        /* Association Count*/
    </script>


</telerik:RadCodeBlock>

<div id="main">
    <br />
    <div id="banner">
        <div id="dropdowns">
            <div>
                <telerik:RadComboBox ID="StandardSetDdl" Label="Standard Set:" Width="120px" runat="server" AutoPostBack="true" OnSelectedIndexChanged="Standard_Set_SelectedIndexChanged" Skin="Web20"
                    CausesValidation="False" ToolTip="Select a Standard Set" ClientIDMode="Static" EmptyMessage="Select One">
                </telerik:RadComboBox>
                <telerik:RadComboBox ID="GradeDbl" Label="Grade:" Width="60px" runat="server" AutoPostBack="true" OnSelectedIndexChanged="GradeDbl_SelectedIndexChanged" Skin="Web20"
                    CausesValidation="False" ToolTip="Select a Grade" EmptyMessage="Select One">
                </telerik:RadComboBox>
                <telerik:RadComboBox ID="SubjectDdl" Width="120px" Label="Subject Set:" runat="server" AutoPostBack="true" OnSelectedIndexChanged="SubjectDdl_SelectedIndexChanged" Skin="Web20"
                    CausesValidation="False" ToolTip="Select a Subject" ClientIDMode="Static" EmptyMessage="Select One">
                </telerik:RadComboBox>
                <telerik:RadComboBox ID="CourseDdl" Label="Course:" Width="120px" runat="server" AutoPostBack="true" OnSelectedIndexChanged="CourseDdl_SelectedIndexChanged" Skin="Web20"
                    CausesValidation="False" ToolTip="Select a Course" ClientIDMode="Static" EmptyMessage="Select One">
                </telerik:RadComboBox>
            </div>
            <br />
            <div>
                <asp:Panel ID="standardSelectorPanel" runat="server" ScrollBars="Vertical">
                <telerik:RadGrid ID="standardsSearchGrid" CssClass="assessmentSearchHeader" Skin="Web20"
                    AllowSorting="True"
                    AllowPaging="false"
                    runat="server"
                    GridLines="None">
                    <MasterTableView Width="100%" Summary="RadGrid table">
                    </MasterTableView>
                    <PagerStyle Mode="NextPrevAndNumeric"></PagerStyle>
                </telerik:RadGrid>
                </asp:Panel>
            </div>
            <asp:Button ID="btnAddAsHyperlink" runat="server" Text="Add as Links" OnClientClick="return insertSelectedStandardsAsLinks();" />&nbsp;
            <asp:Button ID="btnAddAsContent" runat="server" Text="Add as Content" OnClientClick="return insertSelectedStandardsAsContent();" />&nbsp;
            <asp:Button ID="btnAddToLRMI" runat="server" Text="Add to Tag" OnClientClick="return getSelectedStandardsForLRMI();" />&nbsp;
            <asp:Button ID="btnAddStandards" Visible="false" runat="server" Text="Associate Selected Standard to Document" OnClick="btnAddStandards_Click" />&nbsp;
            <input id="btnClose" runat="server" type="button" value="Cancel" onclick="STDdialogCancelled();" />
            <asp:Button ID="btnCloseWnd" runat="server" Text="Cancel" OnClientClick="return CloseRadWindow();" />
        </div>
        <asp:HiddenField ID="HiddenField1" runat="server" Value="false" />

    </div>

</div>
