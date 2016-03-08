<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ATBStandards.ascx.cs" Inherits="CMSWebParts_ThinkgateWebParts_ATBStandards" %>

<link href="/Kentico/CMSWebParts/ThinkgateWebParts/css/associationToolbar.css" rel="stylesheet" />
	<script type="text/javascript" src="<%:this.ResolveUrl("~/")%>cmsscripts/jquery/jquery-1.9.1.min.js"></script>
	<script type="text/javascript" src="<%:this.ResolveUrl("~/")%>cmsscripts/jquery/jquery-migrate-1.1.0.min.js"></script>
	<script type="text/javascript" src="<%:this.ResolveUrl("~/")%>cmsscripts/jquery/jquery-core.js"></script>
	<script type="text/javascript" src="<%:this.ResolveUrl("~/")%>cmsscripts/jquery/jquery-ui/js/jquery-ui-1.10.0.custom.min.js"></script>
	<script type="text/javascript" src="<%:this.ResolveUrl("~/")%>cmsscripts/jquery/jquery-cookie.js"></script>
	<script type="text/javascript" src="<%:this.ResolveUrl("~/")%>cmsscripts/jquery/DataTables/js/jquery.dataTables.js"></script>

	<link href="<%:this.ResolveUrl("~/")%>cmsscripts/jquery/DataTables/css/demo_table_jui.css" rel="stylesheet" />
	<link href="<%:this.ResolveUrl("~/")%>cmsscripts/jquery/jquery-ui/css/smoothness/jquery-ui-1.10.0.custom.css" rel="stylesheet" />
	

<%--<asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>--%>
  <asp:UpdatePanel ID="UpdatePanel_Standards" runat="server" UpdateMode="Conditional" OnLoad="UpdatePanel_Standards_Load">

    <ContentTemplate>
        <div class="toolBar">
            <div id="Standards" class="toolBarWrapper" style="/*border-right: 1px solid lightslategrey; */">
                <div id="Standards_ToolBarItem_div" class="toolBarItem" runat="server">
                  
                    <div id="lnkIAC_std" runat="server" class="defaultIcon standardsIcon"></div>
                    <asp:HyperLink ID="lnkAC_std" runat="server" Text="Standards"></asp:HyperLink>
                    <div id="badge_div_std" class="badge" runat="server"></div>
                  
        </div>
            </div>
        </div>

        <ajaxToolkit:ModalPopupExtender runat="server" ID="mpeStandardsAssoc" BehaviorID="mpeStandardsAssoc"
            TargetControlID="Standards_ToolBarItem_div" PopupControlID="pnlStandardsAssoc" BackgroundCssClass="modalBackground"
            PopupDragHandleControlID="pnlStandardsAssocTitle" DropShadow="False"
            RepositionMode="RepositionOnWindowResizeAndScroll">
        </ajaxToolkit:ModalPopupExtender>

  </ContentTemplate>
</asp:UpdatePanel>


<asp:Panel ID="pnlStandardsAssoc" runat="server" Style="display: none; width: 350px; height: 400px" CssClass="modalPopup" DefaultButton="">


   <%-- <div>--%>
        <asp:Panel ID="pnlStandardsAssocTitle" runat="server" Style="cursor: move; background-color: dimgray; border: 1px solid black; color: Black; text-align: center;">
           <%-- <div>--%>
                <p style="font-weight: bold;">
                    Associatied Standards
                </p>
           <%-- </div>--%>
        </asp:Panel>
  <%--  </div>--%>
    <br />
 <%--   <div>--%>
        <asp:Panel ID="pnlStandardsAssocContent" runat="server" Style="background-color: white;" Height="200px" ScrollBars="Vertical">
            <asp:UpdatePanel ID="upStandardsAssocContent" runat="server" UpdateMode="Conditional">
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="btnAddNewStandardsOK" />
                </Triggers>

                <ContentTemplate>
                  <%--  <div>--%>

                        <table>
                            <asp:Repeater ID="rptAssociationDetail" runat="server">
                                <ItemTemplate>
                                    <%--<div style="line-height: 20px;">--%>
                                        <tr>
                                            <td>
                                                <asp:LinkButton ID="lnkDeleteAssociation" OnCommand="lnkDeleteAssociation_Command" CommandArgument='<%# Eval("ID")%>' CommandName="Delete" runat="server">
                                                    <asp:Image BorderColor="Transparent" BorderWidth="0" ID="Image1" ImageUrl="~/a.aspx?cmsimg=/ug/Delete.png" runat="server" />
                                                </asp:LinkButton>
                                            </td>
                                            <td>&nbsp;&nbsp;&nbsp;&nbsp </td>
                                            <td>
                                                <span class="controlDataList" title='<%# Eval("Detail") %>'><%# Eval("Sub")%></span>
                                            </td>
                                        </tr>
                                 <%--   </div>--%>
                                </ItemTemplate>
                            </asp:Repeater>
                        </table>


               <%--     </div>--%>
                </ContentTemplate>
            </asp:UpdatePanel>
        </asp:Panel>
 <%--   </div>--%>
    <br />
<%--    <div>--%>


        <asp:Button ID="btnStandardsAssocAdd" runat="server" Text="Add New Associations" Style="margin-right: 5px" Width="230px" />
        <ajaxToolkit:ModalPopupExtender ID="mpeStandardsAddNewAssoc" BehaviorID="mpeStandardsAddNewAssoc" runat="server" TargetControlID="btnStandardsAssocAdd"
            PopupControlID="pnlStandardsAddNewAssoc" BackgroundCssClass="modalBackground"
            DropShadow="False" PopupDragHandleControlID="pnlStandardsAddNewAssocTitle" RepositionMode="RepositionOnWindowResizeAndScroll" />


        <asp:Button ID="btnStandardsAssocClose" runat="server" Text="Cancel" Width="100px" OnClick="btnStandardsAssocClose_Click" />

<%--    </div>--%>


</asp:Panel>

<asp:Panel ID="pnlStandardsAddNewAssoc" runat="server" Style="/*display: none; */ width: 750px; height: 500px;" CssClass="modalPopup">

    <%--<div>--%>
        <asp:Panel ID="pnlStandardsAddNewAssocTitle" runat="server" Style="cursor: move; background-color: dimgray; border: 1px solid black; color: Black; text-align: center;">
          <%--  <div>--%>
                <p style="font-weight: bold;">
                    Add New Standards Association
                </p>
           <%-- </div>--%>
        </asp:Panel>
   <%-- </div>--%>
    <br />
<%--    <div>--%>
        <asp:Panel ID="pnlStandardsAddNewAssocContent" runat="server" Style="background-color: white;" Height="400px">


            <asp:UpdatePanel ID="upStandardsAddNewAssocContent" runat="server" UpdateMode="Conditional">
                <ContentTemplate>

                    <b><i>
                        <asp:Label ID="lblStandards_AddNew_DocID" runat="server" Text="DocumentID:"></asp:Label></i></b>
                    <br />
                    <b><i>
                        <asp:Label ID="lblStandards_AddNew_DocType" runat="server" Text="DocumentType:"></asp:Label></i></b>
                    <br />
                    <br />

                   <%-- <div>--%>
                        <telerik:RadComboBox ID="StandardSetDdl" Label="Standard Set:" Width="120px" runat="server" AutoPostBack="true" OnSelectedIndexChanged="Standard_Set_SelectedIndexChanged" Skin="Web20"
                            CausesValidation="False" ToolTip="Select a Standard Set" EmptyMessage="Select One" DataTextField="Standard_Set" ZIndex="500000">
                        </telerik:RadComboBox>
                        <telerik:RadComboBox ID="GradeDdl" Label="Grade:" Width="60px" runat="server" AutoPostBack="true" OnSelectedIndexChanged="GradeDbl_SelectedIndexChanged" Skin="Web20"
                            CausesValidation="False" ToolTip="Select a Grade" EmptyMessage="Select One" DataTextField="Grade" ZIndex="500000">
                        </telerik:RadComboBox>
                        <telerik:RadComboBox ID="SubjectDdl" Width="120px" Label="Subject Set:" runat="server" AutoPostBack="true" OnSelectedIndexChanged="SubjectDdl_SelectedIndexChanged" Skin="Web20"
                            CausesValidation="False" ToolTip="Select a Subject" EmptyMessage="Select One" DataTextField="Subject" ZIndex="500000">
                        </telerik:RadComboBox>
                        <telerik:RadComboBox ID="CourseDdl" Label="Course:" Width="120px" runat="server" AutoPostBack="true" OnSelectedIndexChanged="CourseDdl_SelectedIndexChanged" Skin="Web20"
                            CausesValidation="False" ToolTip="Select a Course" EmptyMessage="Select One" DataTextField="Course" ZIndex="500000">
                        </telerik:RadComboBox>
                   <%-- </div>--%>
                    <br />
                  <%--  <div>--%>
                        <asp:Panel ID="pnlstandardSelectorPanel" runat="server" ScrollBars="Vertical" Height="300px" Visible="false">
                              <telerik:RadGrid ID="standardsGrid" CssClass="assessmentSearchHeader" Skin="Web20"
                    AllowSorting="True"
                    AllowPaging="false"
                    runat="server"
                    GridLines="None" DataKeys="ID">
                    <MasterTableView Width="100%" Summary="RadGrid table">
                    </MasterTableView>
                    <PagerStyle Mode="NextPrevAndNumeric"></PagerStyle>
                </telerik:RadGrid>
                          

                         <%--   <asp:GridView ID="rptSTDlist" runat="server" AllowSorting="True" DataKeyNames="ID" AutoGenerateColumns="False" >

                                <Columns>

                                   
                               <asp:TemplateField  ItemStyle-Width="50" ItemStyle-HorizontalAlign="Center" HeaderText="Select">
                                      <ItemTemplate>
                                          <asp:CheckBox id="Select" runat="server" Checked='<%# Eval("chkSelect").ToString().Equals("1") %>' />
                                     </ItemTemplate>
                                 </asp:TemplateField>

                                    <asp:BoundField DataField="ID" HeaderText="ID" InsertVisible="False" ReadOnly="True" SortExpression="ID" />
                                    <asp:BoundField DataField="StandardName" HeaderText="StandardName" SortExpression="StandardName" />
                                    
                                    <asp:BoundField DataField="Standard_Set" HeaderText="Standard_Set" SortExpression="Standard_Set" />
                                    <asp:BoundField DataField="Grade" HeaderText="Grade" SortExpression="Grade" />
                                    <asp:BoundField DataField="Subject" HeaderText="Subject" SortExpression="Subject" />
                                    <asp:BoundField DataField="Course" HeaderText="Course" SortExpression="Course" />
                                   
                                    <asp:BoundField DataField="Level" HeaderText="Level" SortExpression="Level" />
                                    <asp:BoundField DataField="Description" HeaderText="Desc" SortExpression="Desc" />
                                    <asp:BoundField DataField="chkSelect" HeaderText="chkSelect"  Visible="false"/>
                                   
                                </Columns>
                            </asp:GridView>--%>

         <%--  <asp:Repeater runat="server" ID="rptSTDlist">
			<HeaderTemplate>
				<div id="example_wrapper" class="dataTables_wrapper" role="grid">
				<table id="tblSTDlist" border="0" class="display" style="width:100%">
					<thead>
						<tr>
							<th> ID </th>
							<th> Standard Name </th>
							<th>Standard Set</th>
							<th>Grade</th>
                            <th> Subject </th>
							<th> Course </th>
							<th>Level</th>
							<th>Description</th>

						</tr>
					</thead>
					<tbody>
			</HeaderTemplate>
			<ItemTemplate>
				<tr>
					<td><%# Eval("ID") %></td>
					<td><%# Eval("StandardName") %></td>
					<td><%# Eval("Standard_Set") %></td>
					<td><%# Eval("Grade") %></td>
					<td><%# Eval("Subject") %></td>
					<td><%# Eval("Course") %></td>
                    <td><%# Eval("Level") %></td>
					<td><%# Eval("Description") %></td>
					
				</tr>

			</ItemTemplate>
			<FooterTemplate>
				</tbody>
				</table>
				</div>
			</FooterTemplate>
		</asp:Repeater>

                            <asp:HiddenField ID="SelectedItemsSTD" runat="server" />--%>

                        </asp:Panel>
                 <%--   </div>--%>
                    <asp:Button ID="btnAddNewStandardsOK" Enabled="false" runat="server" Text="Associate Selected Standard" OnClick ="btnAddNewStandardsOK_Click" />&nbsp;
           <asp:Button ID="btnAddNewStandardsClose" runat="server" Text="Cancel" OnClick="btnAddNewStandardsClose_Click" />
                  <%--  </div>--%>
                </ContentTemplate>

            </asp:UpdatePanel>

        </asp:Panel>
   <%-- </div>--%>

</asp:Panel>






<telerik:RadCodeBlock ID="RadCodeBlock_Std" runat="server">

	<%--<link href="<%:this.ResolveUrl("~/")%>cmsscripts/bootstrap/css/bootstrap.min.css" rel="stylesheet" />--%>
	

<%--<script type="text/javascript">

  
    $j(document).ready(function () {
        //var stuff = '<%# Eval("ParentNodeID") %>';
		    //$j('input[name=ParentNode]').val( stuff);

		    /* Add a click handler to the rows - this could be used as a callback */
		    $j('#tblSTDlist tr').click(function () {
		        if ($j(this).hasClass('row_selected')) {
		            $j(this).removeClass('row_selected');
		        } else {
		            $j(this).addClass('row_selected');
		            SelectedNodes = fnGetSelectedSTD();
		        }
		    });

		    /* Init the table */
		    var oTable = $j('#tblSTDlist').dataTable({
		        "oLanguage": { "sSearch": "Search:" },
		        "iDisplayLength": 10,
		        "bJQueryUI": true,
		        "sPaginationType": "full_numbers",
		        "aaSorting": [[0, "asc"]]
		    });

		});

		function fnGetSelectedSTD() {
		    var aReturn = new Array();
		    var aTrs = $j(".row_selected").children();

		    for (var i = 0 ; i < aTrs.length ;) {
		        aReturn.push(aTrs[i + 1].textContent + "|" + aTrs[i + 2].textContent);
		        i = i + 4;
		    }
		    var stuff = aReturn + "";

		    //$j("SelectedItems").context.forms[0].SelectedItems.innerHTML = stuff;

		    $j('input[name=SelectedItemsSTD]').val(stuff);
		    return aReturn;
		}
</script>--%>
    
</telerik:RadCodeBlock>