<%@ Control Language="C#" AutoEventWireup="true" CodeFile="StandardsBar.ascx.cs" Inherits="CMSWebParts_ThinkgateWebParts_StandardsBar" %>

<div class="toolBar">
	<div id="<%= ID %>-<%= DocumentID %>-Standards_tab" class="toolBarWrapper" style="" >
		
		<span id="ToolBarItem_div_Std" class="toolBarItem" onclick='showStdAssoc("<%= DocumentType %>","<%= DocumentID %>", "<%= ID %>");'>
			<div id="<%= ID %>-<%= DocumentID %>-lnkIAC_Std" class="defaultIcon curriculaIcon"></div>
			<asp:HyperLink ID="HyperLink1" runat="server" Text="Standards"></asp:HyperLink>
			<div id="<%= ID %>-<%= DocumentID %>-badge_div_Std" class="badge"><%= AssociationCount %></div>
		</span>
	</div>
	<div id="<%= ID %>-<%= DocumentID %>-Curricula_tab" class="toolBarWrapper" style="" >
		
		<span id="ToolBarItem_div_Curr" class="toolBarItem" onclick='showCurriculaAssoc("<%= DocumentType %>","<%= DocumentID %>", "<%= ID %>");'>
			<div id="<%= ID %>-<%= DocumentID %>-lnkIAC_Curr" class="defaultIcon curriculaIcon"></div>
			<asp:HyperLink ID="HyperLink2" runat="server" Text="Curricula"></asp:HyperLink>
			<div id="<%= ID %>-<%= DocumentID %>-badge_div_Curr" class="badge"><%= CurriculaAssociatonCount %></div>
		</span>
	</div>
</div>

<div id="standards_addnew_association" title="Standards Associations" style="display: none">
	<div id="standards_addnew_association-content"></div>
</div>

<div id="curricula_addnew_association" title="Curricula Associations" style="display: none">
	<div id="curricula_addnew_association-content"></div>
</div>

<script type="text/javascript">

	function showStdAssoc(doctype, docid, ctrlid) {
		var bdgid = ctrlid + "-" + docid + "-badge_div_Std";
		var url = '<%:this.ResolveUrl("~/")%>CMSWebParts/ThinkgateWebParts/StdAddNewAssoc.aspx?doctype=' + doctype + '&parentnodeid=' + docid + '&ctrlid=' + ctrlid;
		$j("#standards_addnew_association-content")[0].innerHTML = "<IFRAME SRC=" + url + "  width=1000 height=540 frameborder=0 >";

		$j("#standards_addnew_association").dialog({
			height: 640,
			width: 1020,
			title: "Standards Associations",
			resizable: false,
			bgiframe: true,
			modal: true,
			dialogClass: "tgAddExistingDialog",
			close: function (event, ui) { setBadgeStandardsCount(docid, bdgid); }
		});
	}

	function showCurriculaAssoc(doctype, docid, ctrlid) {
		var bdgid = ctrlid + "-" + docid + "-badge_div_Curr";
		var url = '<%:this.ResolveUrl("~/")%>CMSWebParts/ThinkgateWebParts/CurriculaAddNewAssoc.aspx?doctype=' + doctype + '&parentnodeid=' + docid + '&ctrlid=' + ctrlid;
		$j("#curricula_addnew_association-content")[0].innerHTML = "<IFRAME SRC=" + url + "  width=1000 height=540 frameborder=0 >";

		$j("#curricula_addnew_association").dialog({
			height: 640,
			width: 1020,
			title: "Curricula Associations",
			resizable: false,
			bgiframe: true,
			modal: true,
			dialogClass: "tgAddExistingDialog",
			close: function (event, ui) { setBadgeCurriculaCount(docid, bdgid); }
		});
	}


	function setBadgeCurriculaCount(docid, bdgid) {
		var count = 0;

		$j.ajax({
			type: "POST",
			url: "<%:this.ResolveUrl("~/")%>CMSWebParts/ThinkgateWebParts/btWebServices.aspx/getCurriculaCount",
			data: "{'docid':'" + docid + "'}",

			contentType: "application/json; charset=utf-8",
			dataType: "json",

			error: function (XMLHttpRequest, textStatus, errorThrown) {
				alert(textStatus + "\n" + errorThrown);
			},
			success: function (result) {
				count = result.d;
				if (count === undefined || count === null) {
					count = 0;
				}
				$j("#" + bdgid)[0].innerHTML = count + "";
			}
		});
		return count;
	}

	function setBadgeStandardsCount(docid, bdgid) {
		var count = 0;

		$j.ajax({
			type: "POST",
			url: "<%:this.ResolveUrl("~/")%>CMSWebParts/ThinkgateWebParts/btWebServices.aspx/getStandardCount",
			data: "{'docid':'" + docid + "'}",

			contentType: "application/json; charset=utf-8",
			dataType: "json",

			error: function (XMLHttpRequest, textStatus, errorThrown) {
				alert(textStatus + "\n" + errorThrown);
			},
			success: function (result) {
				count = result.d;
				if (count === undefined || count === null) {
					count = 0;
				}
				$j("#" + bdgid)[0].innerHTML = count + "";
			}
		});
		return count;
	}


</script>