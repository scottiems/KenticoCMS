<%@ Control Language="C#" AutoEventWireup="true" CodeFile="RatingAverageUC.ascx.cs" Inherits="CMSWebParts_ThinkgateWebParts_IMRating_RatingAverageUC" %>

<div id="dvRatingAverage" class="float-right" >
   
    <telerik:RadRating ID="rating" Skin="Default" runat="server" ItemCount="5"  SelectionMode="Continuous" Precision="Exact" Orientation="Horizontal" ReadOnly="True" CssClass="float-left" Style="top: 5px"  ></telerik:RadRating>
    <a id="ratingReview" class="float-right" style="padding-top: 2px"href="#" onclick="showRatingReviewSummary('<%= NodeID %>');return false;">
        <asp:Label ID="lblReviewFormat" runat="server"></asp:Label>
    </a>
    <asp:Label ID="lblNoReviewMsg" class="float-right" style="padding-top: 2px" runat="server"></asp:Label>

</div>

<div id="review-Summary-dialog_<%= NodeID %>" title="Add New Review" style="display: none">
  <div id="review-Summary-dialog-content_<%= NodeID %>"></div>
</div>


<div id="specialPopulation-dialog_<%= NodeID %>" title="Special Population" style="display: none">
  <div id="specialPopulation-dialog-content_<%= NodeID %>"></div>
</div>

