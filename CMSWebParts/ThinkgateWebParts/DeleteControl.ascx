<%@ Control Language="C#" AutoEventWireup="true" CodeFile="DeleteControl.ascx.cs" Inherits="CMSWebParts_ThinkgateWebParts_DeleteControl" %>

<style>
	.deleteAll
	{
		padding-left: 6em;
	}

	.optionButton
	{
		margin-left:4em;
		margin-left: 60px;
	}
</style>


    <script type="text/javascript" src="<%:this.ResolveUrl("~/")%>/CMSScripts/Custom/jquery-1.9.1.min.js"></script>
	<script type="text/javascript" src="<%:this.ResolveUrl("~/")%>/CMSScripts/Custom/jquery-migrate-1.1.0.min.js"></script>
	<script type="text/javascript" src="<%:this.ResolveUrl("~/")%>/CMSScripts/Custom/jquery-core.js"></script>
	<script type="text/javascript" src="<%:this.ResolveUrl("~/")%>/CMSScripts/Custom/jquery-ui-1.10.0.custom.js"></script>

<script type="text/javascript" src="<%:this.ResolveUrl("~/")%>/CMSScripts/Custom/jquery-cookie.js"></script>

	<script type="text/javascript" src="<%:this.ResolveUrl("~/")%>/CMSScripts/Custom/addNewDocument.js"></script>
	

	<script>var $j = jQuery.noConflict();</script>

	<link href="<%:this.ResolveUrl("~/")%>/CMSScripts/jquery/jquery-ui/css/smoothness/jquery-ui-1.10.0.custom.css" rel="stylesheet" />
	<link href="<%:this.ResolveUrl("~/")%>/CMSWebParts/ThinkgateWebparts/css/tgwebparts.css" rel="stylesheet" />
	<link href="<%:this.ResolveUrl("~/")%>/CMSWebParts/ThinkgateWebparts/css/associationToolbar.css" rel="stylesheet" />

<script type="text/javascript">
	$j(document).ready(function () {
	    initDialog(<%= NodeID %>);
	    /* Detect Safari */
	    if ($j.browser.safari) {
	        /* Do something for Safari */
	        var nodeId = "<%= NodeID %>_chkdeleteAll";
	        var nodeRemove = "<%= NodeID %>_rdoRemove";
	        var nodeDelete = "<%= NodeID %>_rdoDelete";
	        $j("#" + nodeId).css("vertical-align", "top");
	        $j("#" + nodeRemove).css("vertical-align", "top");
	        $j("#" + nodeDelete).css("vertical-align", "top");
	    }

	    var clsname = "<%= ClassName %>";

	    if (clsname == "thinkgate.resource" || clsname == "thinkgate.competencylist")
	    {
	        var nodeId = "<%= NodeID %>_chkdeleteAll";
	        var nodeRemove = "<%= NodeID %>_rdoRemove";
	        
	        $j("#" + nodeId).css("display", "none");
	        $j("#" + nodeRemove).css("display", "none");
	        $j("#" + nodeId + "lbl").css("display", "none");
	        $j("#" + nodeRemove + "lbl").css("display", "none");
	       
	    }
	});

</script>

<div id="<%= NodeID %>-dialog-modal" title="Delete Document" style="display:none;">
	<p>
		<span style="float: left; margin: 0 7px 20px 0;"></span>
	</p>
	<br />
	<input type="radio"  style="vertical-align:middle;" name="<%= NodeID %>_tgrdoRemove" value="remove" id="<%= NodeID %>_rdoRemove" checked  class="optionButton" onchange="enableCascadeCheck('<%= NodeID %>')" /><span id="<%= NodeID %>_rdoRemovelbl"> Remove from parent document but keep local copy</span><br />
	<input type="radio" style="vertical-align:middle;" name="<%= NodeID %>_tgrdoRemove" value="delete" id="<%= NodeID %>_rdoDelete" class="optionButton" onchange="enableCascadeCheck('<%= NodeID %>')" />Permanently delete document <br />
	<span style="display: inline-block; width: 4em;"></span>
    <input type="checkbox" style="vertical-align:middle;" name="<%= NodeID %>_tgchkdeleteAll" value="cascade" id="<%= NodeID %>_chkdeleteAll" disabled="disabled"/><span id="<%= NodeID %>_chkdeleteAlllbl"> Cascade delete all child documents</span>
</div>


<img class="upheaderdelete" src="<%:this.ResolveUrl("~/")%>/CMSHelp/Delete.png" id="<%= NodeID %>-deleteClick">

<asp:LinkButton ID="lnkDelete" runat="server"></asp:LinkButton>