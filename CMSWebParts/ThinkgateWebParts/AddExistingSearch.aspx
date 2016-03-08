<%@ Page Language="C#" AutoEventWireup="true" CodeFile="AddExistingSearch.aspx.cs" Inherits="CMSWebParts_ThinkgateWebParts_AddExistingSearch" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>Existing Item search</title>
	<meta name="viewport" content="width=device-width, initial-scale=1.0">



	<link href="<%:this.ResolveUrl("~/")%>cmsscripts/bootstrap/css/bootstrap.min.css" rel="stylesheet" />
	<link href="<%:this.ResolveUrl("~/")%>cmsscripts/jquery/DataTables/css/demo_table_jui.css" rel="stylesheet" />
	<link href="<%:this.ResolveUrl("~/")%>cmsscripts/jquery/jquery-ui/css/smoothness/jquery-ui-1.10.0.custom.css" rel="stylesheet" />


</head>
<body>
	<div class="css_clear"></div>
	<form id="addExistingSearchForm" runat="server">
		<asp:HiddenField ID="ParentNode" runat="server" Value='<%# Eval("ParentNodeID") %>' />
		<asp:HiddenField ID="ParentName" runat="server" />
		<asp:HiddenField ID="SelectedItems" runat="server" />
		<asp:HiddenField ID="SelectedNames" runat="server" />
		<asp:HiddenField ID="DocumentAction" runat="server" />
		<div style="width: 95%; padding: 20px;">
			<div>
				<div id="dropDownList1Div" style="float: left; position: relative;">
					<asp:DropDownList ID="DropDownList1" runat="server" AutoPostBack="true" CssClass="dropdown-panel" OnSelectedIndexChanged="DropDownList1_SelectedIndexChanged">
						<asp:ListItem Selected="True">All</asp:ListItem>
						<asp:ListItem>Non-Templates</asp:ListItem>
						<asp:ListItem>Templates</asp:ListItem>
					</asp:DropDownList>
				</div>


				<div id="LinkButtonsDiv1" class="pull-right">
					<asp:LinkButton ID="okbtn" runat="server" OnClick="CopySelectedItems_Click" CssClass="btn btn-success"><i class="icon-share icon-white"></i>&nbsp;Copy Selected Items</asp:LinkButton>
					<asp:LinkButton ID="cancelbtn" runat="server" OnClientClick="closeAddExistingDialog();" CssClass="btn btn-danger"><i class="icon-remove icon-white"></i>&nbsp;Cancel</asp:LinkButton>
				</div>
				<div id="addExistingSearchCheckboxes" style="padding-right: 15px; position: relative; top: -8px;" class="pull-right">
					<div class="pull-left">Include: </div>
					<div class="pull-left">
						<label class="checkbox">
							<input type="checkbox" value="0" id="childNodesCB" name="childNodesCB" />
							Child Nodes
						</label>
						<label class="checkbox">
							<input type="checkbox" value="1" id="associationsCB" name="associationsCB"  />
							Associations
						</label>
					</div>
				</div>

			</div>
			<div>
				<div>
					<h4><span style="color: red; font-size: 12px;">
						<asp:Label ID="AddExistingMessage" runat="server"></asp:Label></span></h4>
				</div>
			</div>
			<asp:Repeater runat="server" ID="rptNames">
				<HeaderTemplate>
					<div id="example_wrapper" class="dataTables_wrapper" role="grid">
						<table id="tblAddExistingPlans" border="0" class="display" style="width: 100%">
							<thead>
								<tr>
									<th style="width: 0%; display: none">Parent Node </th>
									<th style="width: 0%; display: none">Plan Node </th>
                                    <th style="width: 10%;">Document Type </th>
									<th style="width: 25%;">Document Name</th>
									<th style="width: 40%;">Description</th>
                                    <th style="width: 15%;">Rating</th>
								</tr>
							</thead>
							<tbody>
				</HeaderTemplate>
				<ItemTemplate>
					<tr>
						<td id="hiddenNodeParentID" style=" width: 0%; display: none"><%# Eval("NodeParentID") %></td>
						<td id ="hiddenNodeID" style="width: 0%; display: none"><%# Eval("NodeId") %></td>
                        <td style="width: 15%;"><%# Eval("docDocumentType") %></td>
                        <td style="width: 25%;"><%# Eval("DocumentName") %></td>
						<td style="width: 40%;"><%# (DocumentType == "thinkgate.LessonPlan") ?  Eval("LessonPlanOverview") : ((DocumentType == "thinkgate.UnitPlan") ?  Eval("UnitPlanOverview") : ((DocumentType == "thinkgate.InstructionalPlan") ?  Eval("InstructionalPlanOverview") : ""))  %></td>
                        <td style="width: 10%;"><%# (Eval("AverageRating") == DBNull.Value || Convert.ToDecimal(Eval("AverageRating")) == 0) ? "No Rating" : Convert.ToDecimal(Eval("AverageRating")).ToString()  %></td>

					</tr>

				</ItemTemplate>
				<FooterTemplate>
					</tbody>
				</table>
				</div>
				</FooterTemplate>
			</asp:Repeater>



		</div>

	</form>

	<script type="text/javascript" src="<%:this.ResolveUrl("~/")%>cmsscripts/jquery/jquery-1.9.1.min.js"></script>
	<script type="text/javascript" src="<%:this.ResolveUrl("~/")%>cmsscripts/jquery/jquery-migrate-1.1.0.min.js"></script>
	<script type="text/javascript" src="<%:this.ResolveUrl("~/")%>cmsscripts/jquery/jquery-core.js"></script>
	<script type="text/javascript" src="<%:this.ResolveUrl("~/")%>cmsscripts/jquery/jquery-ui/js/jquery-ui-1.10.0.custom.js"></script>
	<script type="text/javascript" src="<%:this.ResolveUrl("~/")%>cmsscripts/jquery/jquery-cookie.js"></script>
	<script type="text/javascript" src="<%:this.ResolveUrl("~/")%>cmsscripts/jquery/DataTables/js/jquery.dataTables.js"></script>
	<script type="text/javascript" src="<%:this.ResolveUrl("~/")%>CMSScripts/Custom/bootstrap/js/bootstrap.min.js"></script>
	<script type="text/javascript" src="<%:this.ResolveUrl("~/")%>CMSScripts/Custom/addNewDocument.js"></script>
	<script type="text/javascript" src="<%:this.ResolveUrl("~/")%>CMSScripts/Custom/tgDivTools.js"></script>


	<script type="text/javascript">
		$j(document).ready(function () {
			var tgTableName = 'tblAddExistingPlans';
			/* Add a click handler to the rows - this could be used as a callback */
			tgMakeTableSelectable(tgTableName);
			/* Init the table */
			tgInitDataTable(tgTableName);

			var chkval = "";
			var checkboxes = $j('input[type=checkbox]').click(function () {			   
				var checkbox_id = this.id;
				var checkbox_chk = this.checked;
				chkval = checkbox_id + " | " + this.value + " | " + checkbox_chk;
				var action1 = 0;
				var action2 = 0;
				if ($j('input#childNodesCB').is(':checked')) {
					action1 = 1;
				}

				if ($j('input#associationsCB').is(':checked')) {
					action2 = 2;
				}
				
				$j('input[name=DocumentAction]').val((action1 + action2) + "");
			});


			
		});
	</script>

</body>
</html>
