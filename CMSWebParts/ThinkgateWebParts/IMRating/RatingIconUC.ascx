<%@ Control Language="C#" AutoEventWireup="true" CodeFile="RatingIconUC.ascx.cs" Inherits="CMSWebParts_ThinkgateWebParts_IMRating_RatingIconUC" %>

   <script type="text/javascript" src="<%:this.ResolveUrl("~/")%>/CMSScripts/Custom/jquery-1.9.1.min.js"></script>
	<script type="text/javascript" src="<%:this.ResolveUrl("~/")%>/CMSScripts/Custom/jquery-migrate-1.1.0.min.js"></script>
	<script type="text/javascript" src="<%:this.ResolveUrl("~/")%>/CMSScripts/Custom/jquery-core.js"></script>
	<script type="text/javascript" src="<%:this.ResolveUrl("~/")%>/CMSScripts/Custom/jquery-ui-1.10.0.custom.js"></script>

<script type="text/javascript" src="<%:this.ResolveUrl("~/")%>/CMSScripts/Custom/jquery-cookie.js"></script>

<script type="text/javascript" src="<%:this.ResolveUrl("~/")%>CMSWebParts/ThinkgateWebParts/IMRating/RatingReview.js"></script>
	

	<script>var $j = jQuery.noConflict();</script>

	<link href="<%:this.ResolveUrl("~/")%>/CMSScripts/jquery/jquery-ui/css/smoothness/jquery-ui-1.10.0.custom.css" rel="stylesheet" />
	<link href="<%:this.ResolveUrl("~/")%>/CMSWebParts/ThinkgateWebparts/css/tgwebparts.css" rel="stylesheet" />
	<link href="<%:this.ResolveUrl("~/")%>CMSWebParts/ThinkgateWebParts/IMRating/RatingReview.css" rel="stylesheet" />

<div>
    <a href="#" onclick="showRatingView('<%= NodeID %>'); return false;">
    <img class="upheaderRating" src="<%:this.ResolveUrl("~/")%>CMSWebParts/ThinkgateWebParts/Images/RatingIcon.png" alt="Rating" title="Rating" />
    </a>
</div>

<div id="addNew-Review_<%= NodeID %>" title="Add New Review" style="display: none">
  <div id="addNew-Review-dialog-content_<%= NodeID %>"></div>
</div>