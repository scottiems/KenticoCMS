<%@ Page Language="C#" AutoEventWireup="true" CodeFile="StandardAssocList.aspx.cs" Inherits="CMSWebParts_ThinkgateWebParts_StandardAssocList" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">

<meta name="viewport" content="width=device-width, initial-scale=1.0" />

	<link href="<%:this.ResolveUrl("~/")%>cmsscripts/jquery/DataTables/css/demo_table_jui.css" rel="stylesheet" />
	<link href="<%:this.ResolveUrl("~/")%>cmsscripts/jquery/jquery-ui/css/smoothness/jquery-ui-1.10.0.custom.css" rel="stylesheet" />
	<link href="<%:this.ResolveUrl("~/")%>cmsscripts/bootstrap/css/bootstrap.min.css" rel="stylesheet" />

    <title></title>
    
</head>
<body>
    <div class="css_clear"></div>
	<form id="frmStandardsDataList" runat="server">
        <asp:scriptmanager ID="Scriptmanager1" runat="server"></asp:scriptmanager>
        <div style="width: 95%; padding: 20px;">
			<asp:Panel ID="pnlStdAssocList" runat="server" ScrollBars="Vertical"  Height="500px">
            <asp:UpdatePanel ID="upStdAssocList" runat="server" UpdateMode="Conditional">
                 <ContentTemplate>
            <asp:Repeater runat="server" ID="rptStdAssocDetail">
				<HeaderTemplate>
					<div id="std_example_wrapper" class="dataTables_wrapper" role="grid">
						<table id="tblStdAssocList" border="0" class="display" style="width: 100%">
							<thead>
								<tr>
									<th> Delete </th>
									<th>Standards </th>
                                    <th>Grade</th>
                                    <th> Subject</th>
                                    <th> Course</th>
								</tr>
							</thead>
							<tbody>
				</HeaderTemplate>
				<ItemTemplate>
					<tr>
						<td> <asp:LinkButton ID="lnkStdDeleteAssociation" OnCommand="lnkStdDeleteAssociation_Command" CommandArgument='<%# Eval("ID")%>' CommandName="Delete" runat="server">
                                                    <asp:Image BorderColor="Transparent" BorderWidth="0" ID="Image1" ImageUrl="~/a.aspx?cmsimg=/ug/Delete.png" runat="server" />
                                                </asp:LinkButton></td>
						<td> <span class="StdDataList" title='<%# Eval("Detail") %>'><%# Eval("StdName")%></span>
                            <td> <span class="StdDataList_grade" title='<%# Eval("Detail") %>'><%# Eval("Grade")%></span>
                                <td> <span class="StdDataList_sub" title='<%# Eval("Detail") %>'><%# Eval("Subject")%></span>
                                    <td> <span class="StdDataList_course" title='<%# Eval("Detail") %>'><%# Eval("Course")%></span>
						
					</tr>

				</ItemTemplate>
				<FooterTemplate>
					</tbody>
				</table>
				 
                    </div>
				</FooterTemplate>
			</asp:Repeater>
             </ContentTemplate>
            </asp:UpdatePanel>
                </asp:Panel>


		</div>
    
       <div id="LinkButtonsDiv1" class="pull-left">
					<asp:LinkButton ID="btnStdAssocAdd" runat="server" CssClass="btn btn-success"><i class="icon-share icon-white"></i>&nbsp;Add New Associations</asp:LinkButton>
					<asp:LinkButton ID="btnStdAssocClose" runat="server" CssClass="btn btn-danger"><i class="icon-remove icon-white"></i>&nbsp;Cancel</asp:LinkButton>
				</div>       
       
          

    </form>

	<script type="text/javascript" src="<%:this.ResolveUrl("~/")%>cmsscripts/jquery/jquery-1.9.1.min.js"></script>
	<script type="text/javascript" src="<%:this.ResolveUrl("~/")%>cmsscripts/jquery/jquery-migrate-1.1.0.min.js"></script>
	<script type="text/javascript" src="<%:this.ResolveUrl("~/")%>cmsscripts/jquery/jquery-core.js"></script>
	<script type="text/javascript" src="<%:this.ResolveUrl("~/")%>cmsscripts/jquery/jquery-ui/js/jquery-ui-1.10.0.custom.min.js"></script>
	<script type="text/javascript" src="<%:this.ResolveUrl("~/")%>cmsscripts/jquery/jquery-cookie.js"></script>
	<script type="text/javascript" src="<%:this.ResolveUrl("~/")%>cmsscripts/jquery/DataTables/js/jquery.dataTables.js"></script>

	<script type="text/javascript">
		$j(document).ready(function () {
			alert("ready1");
			var tgTableName = 'tblStdAssocList';

			/* Init the table */
			tgInitDataTable(tgTableName);

		});




		function tgInitDataTable(tableID) {
			if (objExists(tableID)) {
				/* Init the table */
				var oTable = $j('#' + tableID).dataTable({
					"oLanguage": { "sSearch": "Search:" },
					"iDisplayLength": 20,
					"bJQueryUI": true,
					"sPaginationType": "full_numbers",
					"aaSorting": [[0, "asc"]]
				});
			}
		}
	</script>
</body>
</html>

