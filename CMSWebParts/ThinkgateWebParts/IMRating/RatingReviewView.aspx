<%@ Page Language="C#" AutoEventWireup="true" CodeFile="RatingReviewView.aspx.cs" Inherits="CMSWebParts.ThinkgateWebParts.IMRating.CMSWebParts_ThinkgateWebParts_IMRating_RatingReviewView" %>

<%@ Register Src="~/CMSWebParts/ThinkgateWebParts/IMRating/SpecialPopulationUC.ascx" TagPrefix="e3" TagName="SpecialPopulation" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="RatingReview.css" rel="stylesheet" />
    <script src="RatingReview.js"></script>
    <script type="text/javascript">

        
        var radRating;

        //SubmitReview validates the minimum criteria for successful save.
        //User should have entered comments or selected rating
        function submitReview() {
            var selectedItems = $('.rrtSelected');
            var reviews = $('#txtReivew').val();

            if (selectedItems.length == 0 && reviews.trim().length == 0) {
                $('#lblErrorMsg').css('visibility', 'visible');
            }
            else {
                $('#lblErrorMsg').css('visibility', 'hidden');
                __doPostBack('radSubmit', '');
            }
        }

        //Business Rule for No Rating Check box
        //When user selected No Rating checkbox the rating control will get disable with zero star selected.
        //When unchecked one star will get selected and rating control available for user selection.
        function noRatingCheckBox(sender) {
            var items = radRating._items;
            if (sender.checked) {

                for (var i = 0; i < items.length; i++) {
                    items[i].className = "";
                }
                radRating._value = 0;
                document.getElementById("rating_ClientState").value = "{\"value\":\"0\",\"readOnly\":false}";
                radRating._enableRatingControl();
            }
            else {
                if (items != null && items.length > 0) {
                    items[0].className = "rrtSelected";
                }
                radRating._value = 1;
                document.getElementById("rating_ClientState").value = "{\"value\":\"1\",\"readOnly\":false}";
                radRating._enableRatingControl(radRating);
            }
        }

        //Fires on rating control load. 
        //Get rating control instance and enable/disable the control based on No Rating Check box value.
        function setRatingControl(control) {
            radRating = control;

            var checkBox = document.getElementById('chkNoRating');

            if (checkBox.checked)
                radRating._enableRatingControl();
            else
                radRating._enableRatingControl(radRating);

            $('#lblErrorMsg').css('visibility', 'hidden');
        }

      
        //Check for max comments entered by user
        function isUnderCharLimit(val, event) {
            var len = val.value.length;
            if (len < 500 || checkForSelectionText(val) || event.keyCode == 8 || event.keyCode == 46) {
                return true;
            }
            else {
                return false;
            }
        }
        
        //Check for selection Text
        function checkForSelectionText(val) {
            if ((val.selectionEnd - val.selectionStart) > 0) {
                return true;
            }
            else {
                return false;
            }
        }

        //Check for max comments entered by user
        //Set the remaining count
        function countChar(val) {
            var len = val.value.length;
            var charsRemaining = 500 - len;
            $('#remainingCharacters').html('<span class="black">Remaining characters: </span>' + charsRemaining);
        }

        //Handles the copy paste functionality of comment
        function pasteComment(val, e) {
            var paste = e.clipboardData && e.clipboardData.getData ? e.clipboardData.getData('text/plain') :
            window.clipboardData && window.clipboardData.getData ? window.clipboardData.getData('Text') : false;
            var newText = '';
            var actualText = '';
            if ((val.value.length + paste.length) > 500) {
                newText = val.value.replaceAt(val.selectionStart, val.selectionEnd, paste);
                actualText = newText.substring(0, 500);
                document.getElementById('txtReivew').value = actualText;
                countChar(val);
                return false;
            }
            countChar(val);
        }
        
        //Replace the string based on start and end Index
        String.prototype.replaceAt = function (startIndex, endIndex, newString) {
            return this.substr(0, startIndex) + newString + this.substr(endIndex, this.length);
        }
        
        //Set the inital comment count when dialog loads.
        function init() {

            countChar(document.getElementById('txtReivew'));
        }
        window.onload = init;
        
        //Close current dialog Refresh the information on parent dialog.
        function closeAndRefresh() {

            var source = document.getElementById("source").value;
            var nodeid = document.getElementById("nodeId").value;
            if (source == "Summary") {
                document.getElementById("needRefreshSummary").value = "Yes";
                reopenSummaryPage(nodeid);
            }
            else
            window.parent.location.reload();

        }
        //Close current dialog Refresh the information on parent dialog.
        function closeCurrentCustomDialog() {

            
            var source = document.getElementById("source").value;
            var nodeid = document.getElementById("nodeId").value;
            if (source == "Summary") {
                reopenSummaryPage(nodeid);
            }
            else
                window.parent.jQuery("#addNew-Review_" + nodeid).dialog('close');

        }

        function reopenSummaryPage(nodeid) {
            var height = $(window).height();
            var width = $(window).width();

            var actionId = document.getElementById("actionId").value;
            var needRefresh = document.getElementById("needRefreshSummary").value;

            var url = applicationVirtualPath + '/CMSWebParts/ThinkgateWebParts/IMRating/RatingReviewSummary.aspx?nodeID=' + nodeid + '&actionId=' + actionId+"&needRefresh="+needRefresh;
            window.parent.jQuery('#review-Summary-dialog-content_' + nodeid)[0].innerHTML = "<IFRAME SRC=" + url + " width='" + (width) + "' height= '" + (height) + "' frameborder=0 >";

            window.parent.jQuery("#addNew-Review_" + nodeid).dialog('close');

            window.parent.jQuery("#review-Summary-dialog_" + nodeid).dialog({
                resizable: false,
                height: height + 70,
                width: width + 50,
                title: "Review Summary",
                bgiframe: true,
                modal: true,
                stack: true,
                close: function (event) {
                    if (needRefresh == "Yes") {
                        window.parent.location.reload();
                    }
                }

            });
        }
       
    </script>
</head>
<body class="lightBackground">
    <form id="frmMain" runat="server">
   <telerik:RadScriptManager ID="radScriptManager1" EnableScriptCombine="False" runat="server">
            <Scripts>
                <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.Core.js" />
                <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQuery.js" />
                <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQueryInclude.js" />
            </Scripts>
       </telerik:RadScriptManager>
        <asp:HiddenField runat="server" ID="nodeId"/>
        <asp:HiddenField runat="server" ID="source"/>
        <asp:HiddenField runat="server" ID="actionId"/>
         <asp:HiddenField runat="server" ID="needRefreshSummary"/>
            <table>
               
                <tr id="topTableRow">
                    <td colspan="2">
                        <asp:TextBox ID="txtReivew" runat="server" TextMode="MultiLine" Width="100%" Height="150px" onkeypress="return isUnderCharLimit(this, event)" onkeyup="countChar(this)" onpaste=" return pasteComment(this, event)"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <span id="remainingCharacters">
                            <span class="black">Remaining characters: </span>500
                        </span>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblRole" CssClass="float-left" runat="server">Role of Submitter</asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:DropDownList ClientIDMode="Static" CssClass="dropdown_role float-left" ID="ddlRoles" runat="server"></asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <hr />
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <e3:SpecialPopulation ClientIDMode="Static" ID="specialPopulationUC" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2"></td>
                </tr>
               <tr>
                    <td class="centeredText" colspan="2">
                        <div style="margin:auto;display:inline-block;">
                            <div class="rating-text">Click to Rate</div>   
                        <telerik:RadRating ID="rating" runat="server" Skin="Default" ItemCount="5" 
                            SelectionMode="Continuous" Precision="Item" Orientation="Horizontal" 
                            OnClientLoad="setRatingControl" CssClass="rating"></telerik:RadRating>
                             
                        </div>
                    </td>
                </tr>
                <tr>
                    <td class="centeredText" colspan="2">
                        <asp:CheckBox ID="chkNoRating" CssClass="checkbox" runat="server"
                            Text="No Rating Given" AutoPostBack="false" onclick="noRatingCheckBox(this)"
                            Font-Size="12pt" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2"></td>
                </tr>
                <tr>
                    <td class="centeredText" colspan="2">
                        <div class="center">
                            <telerik:RadButton ClientIDMode="Static" ID="radCancel" Width="150px" runat="server" Text="Cancel" AutoPostBack="false" OnClientClicked="closeCurrentCustomDialog"></telerik:RadButton>
                            <telerik:RadButton ClientIDMode="Static" ID="radSubmit" Width="150px" runat="server" Text="Submit" AutoPostBack="false" OnClientClicked="submitReview" />
                        </div>
                    </td>
                </tr>
                <tr>
                    <td class="centeredText" colspan="2">
                        <span id="lblErrorMsg" class="centeredText errorMessage"  style="width: 100%; visibility: hidden">
                            Enter comment or select a rating
                        </span>
                      
                    </td>
                </tr>
               <tr>
                   <td>
                       &nbsp;
                   </td>
               </tr>
                <tr>
                     <td colspan="2" class="disclaimer centeredText">
                         Any views or opinions expressed in this review are solely those of the author and do not necessarily represent those of the School, District, or State. 
                     </td>
                </tr>
            </table>
      
    </form>
</body>
</html>
