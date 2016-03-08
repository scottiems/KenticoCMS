<%@ Page Language="C#" AutoEventWireup="true" CodeFile="RatingReviewSummary.aspx.cs" Inherits="CMSWebParts_ThinkgateWebParts_IMRating_RatingReviewSummary" %>

<%@ Register Src="~/CMSWebParts/ThinkgateWebParts/IMRating/SpecialPopulationUC.ascx" TagPrefix="e3" TagName="SpecialPopulation" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
     <base target="_self" />

    <link href="RatingReview.css" rel="stylesheet" />
    <script src="RatingReview.js"></script> 
    <script type="text/javascript" src="../../../CMSScripts/Custom/jquery-1.9.1.min.js"></script>
	<script type="text/javascript" src="../../../CMSScripts/Custom/jquery-migrate-1.1.0.min.js"></script>
	<script type="text/javascript" src="../../../CMSScripts/Custom/jquery-core.js"></script>
	<script type="text/javascript" src="../../../CMSScripts/Custom/jquery-ui-1.10.0.custom.js"></script>


   <script type="text/javascript">

       //Show the ItemReviewMainScreen 
       //User should be able to edit the item review.
       function showReviewMainScreen() {
           var nodeId = document.getElementById("nodeId").value;
    
           showRatingViewSummary(nodeId);
       }

       //Set the parent dialog refresh flag 
       //So the respective review value will be refreshed.
       function setParentRefreshFlag() {
           var hidRefresh = $("#hidRefresh", parent.document.body);
           hidRefresh.val("true");
       }

       function setRefreshOnDialogClose() {
           var nodeid = document.getElementById("nodeId").value;
           window.parent.jQuery("#review-Summary-dialog_" + nodeid).dialog({
               close: function (event, ui) {
                   window.parent.location.reload();
               }
           });
       }


       //Shows the Special Population when user click on Special Population button.
       function showSpecialPopulationScreen(nodeid, userId) {
           var height = $(window).height()* 0.3;
           var width = $(window).width() * 0.5;




           var url = applicationVirtualPath + '/CMSWebParts/ThinkgateWebParts/IMRating/SpecialPopulation.aspx?nodeID=' + nodeid + "&userId=" + userId;


           window.parent.jQuery('#specialPopulation-dialog-content_' + nodeid)[0].innerHTML = "<IFRAME SRC=" + url + " width='" + (width) + "' height= '" + (height) + "' frameborder=0 >";
           window.parent.jQuery("#specialPopulation-dialog_" + nodeid).dialog({
               resizable: false,
               height: height+90,
               width: width+70,
               title: "Special Population",
               bgiframe: true,
               stack: true,
               modal: true,
               close: function (event) {
               }

           });
       }

       //ReadMore and ReadLess Functionality
       function showFullComment(obj, commentId) {

           var mode = $(obj).attr("CommentMode");

           switch (mode) {
               case "ReadMore":
                   $('.commentFull[CommentID="' + commentId + '"]').each(function () {
                       $(this).removeClass('display-none');
                       $(obj).attr("CommentMode", "ReadLess");
                       obj.innerHTML = 'Read Less';
                   });

                   $('.commentLimit[CommentID="' + commentId + '"]').each(function () {
                       $(this).addClass('display-none');
                   });

                   break;
               case "ReadLess":
                   $('.commentFull[CommentID="' + commentId + '"]').each(function () {
                       $(this).addClass('display-none');

                   });

                   $('.commentLimit[CommentID="' + commentId + '"]').each(function () {
                       $(this).removeClass('display-none');
                       $(obj).attr("CommentMode", "ReadMore");
                       obj.innerHTML = 'Read More';
                   });

                   break;

               default:
                   break;
           }

           return false;
       }

       //Set the rating control tool tip based on actual average value.
       function setRatingControl(control) {

           if (control != undefined) {
               var items = control._items;
               var average = document.getElementById("hidAverageRating");

               if (average != null) {
                   for (var i = 0; i < items.length; i++) {
                       if (items[i].firstChild != undefined) {
                           items[i].firstChild.title = average.value;
                       }
                   }
               }
           }
       }
       
       function showRatingViewSummary(nodeid) {

           if (nodeid == null || nodeid == 0) {
               alert("Can not determine the Document Node ID");
               return false;
           }

           var height = $(window).height();
           var width = $(window).width();
           var actionId = document.getElementById("hidActionId").value;
           var refresh = document.getElementById("needRefresh").value;


         
           var url = applicationVirtualPath + '/CMSWebParts/ThinkgateWebParts/IMRating/RatingReviewView.aspx?nodeID=' + nodeid+"&source=Summary&actionId="+actionId+"&needRefresh="+refresh;
           window.parent.jQuery('#addNew-Review-dialog-content_' + nodeid)[0].innerHTML = "<IFRAME SRC=" + url + " width='" + (width) + "' height= '" + (height) + "' frameborder=0 >";
           window.parent.jQuery("#review-Summary-dialog_" + nodeid).dialog({
               close: function(event, ui) {
               }
           });
           window.parent.jQuery("#review-Summary-dialog_" + nodeid).dialog("close");

           window.parent.jQuery("#addNew-Review_" + nodeid).dialog({
               resizable: false,
               height: height+70,
               width: width+50,
               title: "Edit Review",
               bgiframe: true,
               modal: true,
               close: function (event) {
               }

           });
          
           return false;
           
       }

    </script>

</head>

<body class="lightBackground">
    <form id="form1" runat="server">
        
          <telerik:RadScriptManager ID="radScriptManager1" EnableScriptCombine="False" runat="server">
            <Scripts>
                <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.Core.js" />
                <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQuery.js" />
                <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQueryInclude.js" />
               <%-- <asp:ScriptReference Path="../../../CMSScripts/Custom/jquery-1.9.1.min.js" />
                <asp:ScriptReference Path="../../../CMSScripts/Custom/jquery-migrate-1.1.0.min.js" />
                <asp:ScriptReference Path="../../../CMSScripts/Custom/jquery-core.js" />
                <asp:ScriptReference Path="../../../CMSScripts/Custom/jquery-ui-1.10.0.custom.js" />--%>

            </Scripts>
       </telerik:RadScriptManager>


      <telerik:RadWindowManager ID="RadWindowManager1" runat="server" EnableShadow="false" Skin="Thinkgate_Window" EnableEmbeddedSkins="false" />

        <div id="dvHeader">
        </div>
        <telerik:RadAjaxPanel ID="ajaxReviewSummaryPanel" runat="server" LoadingPanelID="reviewSummaryProgress" Width="100%">
            <asp:UpdatePanel ID="updReviewCount" runat="server">
                <ContentTemplate>
                    <asp:HiddenField runat="server" ID="nodeId"/>
                    <asp:HiddenField ClientIDMode="Static" runat="server" ID="needRefresh"/>
                    <asp:HiddenField ClientIDMode="Static" ID="hidActionId" runat="server" />
                    <br />
                    <div id="dvRatingCount">
                        <table id="headerTable">
                            <tr>
                                <td>
                                    <telerik:RadRating ID="averageRating" runat="server" Skin="Default" ItemCount="5" SelectionMode="Continuous"
                                        Orientation="Horizontal" Enabled="False" ReadOnly="True" OnClientLoad="setRatingControl">
                                    </telerik:RadRating>
                                </td>
                                <td>
                                    <asp:Label ID="lblAverageRating" runat="server"></asp:Label>
                                    <asp:HiddenField runat="server" ID="hidAverageRating" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label ID="lblTotalRating" runat="server"></asp:Label>
                                </td>
                                <td>
                                    <asp:Label runat="server" ID="lblShowAll">Show All</asp:Label>
                                    <asp:LinkButton runat="server" ID="btnShowAll" OnClick="showReviews_OnClick">Show All</asp:LinkButton>
                                </td>
                            </tr>
                            <asp:Repeater ID="rptRating" runat="server" OnItemDataBound="rptRating_OnItemDataBound">
                                <ItemTemplate>
                                    <tr>
                                        <td>
                                            <telerik:RadRating ID="rating" runat="server" Skin="Default" ItemCount="5" SelectionMode="Continuous" Precision="Item" Orientation="Horizontal" Enabled="False" ReadOnly="True"></telerik:RadRating>
                                        </td>
                                        <td>
                                            <asp:LinkButton runat="server" ID="btnReviewCount" OnClick="showReviews_OnClick"></asp:LinkButton>
                                            <asp:Label runat="server" ID="lblReviewCount">0</asp:Label>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                            <tr>
                                <td>
                                    <asp:Label ID="lblNaRating" runat="server">No Rating Given</asp:Label>
                                </td>
                                <td>
                                    <asp:Label runat="server" ID="lblNAReviewCount">0</asp:Label>
                                    <asp:LinkButton runat="server" ID="btnNAReviewCount" OnClick="showReviews_OnClick">0</asp:LinkButton>
                                </td>
                            </tr>
                        </table>

                    </div>

                    <br />

                    <div id="dvRatingSummary">
                        <table>
                            <tr>
                                <td>Review</td>
                                <td>Rank</td>
                                <td>Reviewer</td>
                                <td>Date</td>
                            </tr>
                            <asp:Repeater ID="rptReviewSummary" runat="server" OnItemDataBound="rptReviewSummary_OnItemDataBound" >
                                <ItemTemplate>
                                    <tr>
                                        <td class="left-text" id="commentTableColumn">
                                            <p class="commentLimit word-wrap-m" commentid='<%# Eval("ID") %>'>
                                                <asp:Literal ID="ltrReviewLimited" runat="server"></asp:Literal>
                                            </p>
                                            <p class="commentFull display-none word-wrap-m" commentid='<%# Eval("ID") %>'>
                                                <asp:Literal ID="ltrReviewFull" runat="server"></asp:Literal>
                                            </p>
                                            <asp:LinkButton ID="btnComment" runat="server" CssClass="font-small" Visible="false" CommentMode="ReadMore"
                                                OnClick='<%#Eval("ID", "return showFullComment(this, {0})") %>'>Read More</asp:LinkButton>
                                        </td>
                                        <td class="summaryRatingTableColumn">
                                            <telerik:RadRating ID="rating" runat="server" Skin="Default" ItemCount="5" SelectionMode="Continuous" Precision="Item" Orientation="Horizontal" Enabled="False" ReadOnly="True"></telerik:RadRating>
                                        </td>
                                        <td class="ratingCreatorNameTableColumn">
                                            <asp:Label ID="lblReviewer" runat="server"></asp:Label>
                                        </td>
                                        <td class="dateTableColumn">
                                            <asp:Label ID="lblDate" runat="server"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="left-text dot-line btn-style">
                                            <asp:Button ID="btnEdit" runat="server" Text="Edit" onclientclick="showReviewMainScreen()"></asp:Button>
                                            &nbsp;
                                            <asp:Button ID="btnDelete" runat="server" Text="Delete" ReviewId='<%# Eval("ID") %>' OnClick="btnDelete_OnClick"></asp:Button>
                                            &nbsp;
                                            <asp:Button ID="btnSP" runat="server" Text="Special Populations" AutoPostBack="false" OnClientClick='<%# "return showSpecialPopulationScreen(\""+ Eval("NodeID") +"\", \""+ Eval("UserID") +"\");" %>' />
                                        </td>
                                        <td colspan="3" class="dot-line"></td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                        </table>
                    </div>

                    <div class="disclaimer centeredText">
                        Any views or opinions expressed in this review are solely those of the author and do not necessarily represent those of the School, District, or State.  
               
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </telerik:RadAjaxPanel>
        <telerik:RadAjaxLoadingPanel ClientIDMode="Static" ID="reviewSummaryProgress" runat="server" Width="100%" />
   
        <div id="edit-Review" title="Add New Review" style="display: none" clientidmode="Static">
             <div id="edit-Review-dialog-content"></div>
         </div>
    </form>
</body>
</html>
