<%@ Control Language="C#" AutoEventWireup="true" CodeFile="TGAssociationToobar.ascx.cs" Inherits="CMSWebParts_ThinkgateWebParts_TGAssociationToobar" %>

<%--Load common javascript and css here--%>
<script type="text/javascript" src="<%:this.ResolveUrl("~/")%>/CMSWebParts/ThinkgateWebParts/js/associationToolbar.js"></script>
<link href="<%:this.ResolveUrl("~/")%>/CMSWebParts/ThinkgateWebParts/css/associationToolbar.css" rel="stylesheet" />

<style type="text/css">
    .curriculaIcon {
        background-image: url('<%:this.ResolveUrl("~/")%>/cmswebparts/thinkgatewebparts/images/curriculum-icon.png');
    }
</style>

<%--Need a hidden(display: none) div to show each item in that can appear in the toolbar--%>
<div id="standards_addnew_association" title="Standards Associations" style="display: none">
	<div id="standards_addnew_association-content"></div>
</div>

<div id="curricula_addnew_association" title="Curricula Associations" style="display: none">
	<div id="curricula_addnew_association-content"></div>
</div>

<div id="assessment_addnew_association" title="Assessments Associations" style="display: none">
	<div id="assessment_addnew_association-content"></div>
</div>

<div id="resources_addnew_association" title="Resource Associations" style="display: none">
	<div id="resources_addnew_association-content"></div>
</div>

<div id="lrmi_addnew_association" title="Tags Associations" style="display: none">
	<div id="lrmi_addnew_association-content"></div>
</div>


<div id="required_criteria_message" title="Warning" style="display: none">
	<br/>
    <span style="color: red"> Please select required criteria </span>
    <br/>
</div>