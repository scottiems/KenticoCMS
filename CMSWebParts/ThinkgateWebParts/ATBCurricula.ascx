<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ATBCurricula.ascx.cs" Inherits="CMSWebParts_ThinkgateWebParts_ATBCurricula" %>

<link href="<%:this.ResolveUrl("~/")%>CMSWebParts/ThinkgateWebParts/css/associationToolbar.css" rel="stylesheet" />

<%--<asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>--%>
 <asp:UpdatePanel ID="UpdatePanel_Curricula" runat="server" UpdateMode="Conditional" OnLoad="UpdatePanel_Curricula_Load">

     <ContentTemplate>
<div class="toolBar">
	<div id="Curricula" class="toolBarWrapper" style="/*border-right: 1px solid lightslategrey;*/">
		<div id="Curricula_ToolBarItem_div" class="toolBarItem" runat="server">
           
  
			<div id="lnkIAC" runat="server" class="defaultIcon curriculaIcon"></div>
			<asp:HyperLink ID="lnkAC" runat="server" Text="Curricula"></asp:HyperLink>
			<div id="badge_div" class="badge" runat="server"></div>
           
		</div>
	</div>
</div> 
       
         <ajaxToolkit:ModalPopupExtender runat="server" ID="mpeCurriculaAssoc" BehaviorID="mpeCurriculaAssoc"
			TargetControlID="Curricula_ToolBarItem_div" PopupControlID="pnlCurriculaAssoc" BackgroundCssClass="modalBackground"
			 PopupDragHandleControlID="pnlCurriculaAssocTitle" DropShadow="False"
			RepositionMode="RepositionOnWindowResizeAndScroll"></ajaxToolkit:ModalPopupExtender>
      
         </ContentTemplate>
    </asp:UpdatePanel>
        

        <asp:Panel ID="pnlCurriculaAssoc" runat="server" Style="display: none; width: 400px; height: 500px" CssClass="modalPopup">
                            
            
                              <div> <asp:Panel ID="pnlCurriculaAssocTitle" runat="server" Style="cursor: move; background-color: dimgray;
                                    border: 1px solid black; color: Black; text-align: center;">
                                    <div>
                                        <p style="font-weight: bold;">
                                            Associatied Curricula</p>
                                    </div>
                                </asp:Panel> </div>
          <br />
                             <div>  <asp:Panel ID="pnlCurriculaAssocContent" runat="server" Style="background-color: white;" Height="350px" ScrollBars="Vertical">
                                <asp:UpdatePanel ID="upCurriculaAssocContent" runat="server" UpdateMode="Conditional">
                                    <Triggers>
                                        <asp:AsyncPostBackTrigger ControlID="btnCurriculaAddNewAssocOK" />
                                    </Triggers>
                                    
                                    <ContentTemplate>
                                            <div>
                                               
                                               <table>
            <asp:Repeater ID="rptAssociationDetail" runat="server">
                <ItemTemplate>
                    <div style="line-height: 20px;">
                        <tr>
                            <td>
                        <asp:LinkButton ID="lnkDeleteAssociation" OnCommand="lnkDeleteAssociation_Command" CommandArgument='<%# Eval("ID")%>' CommandName="Delete" runat="server" >
                            <asp:Image BorderColor="Transparent" BorderWidth="0" ID="Image1" ImageUrl="~/a.aspx?cmsimg=/ug/Delete.png" runat="server"  />
                        </asp:LinkButton>
                            </td>
                            <td> &nbsp;&nbsp;&nbsp </td>
                            <td>
                         <span class="controlDataList" title="Grade"><%# Eval("Grade")%></span>
                                </td>
                            <td> &nbsp;&nbsp;&nbsp </td>
                            <td>
                         <span class="controlDataList" title="Subject"><%# Eval("Sub")%></span>
                                </td>
                             <td> &nbsp;&nbsp;&nbsp </td>
                            <td>
                         <span class="controlDataList" title="Course"><%# Eval("Course")%></span>
                                </td>
                            </tr>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
            </table>
                                                
                                               
                                            </div>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                        </asp:Panel>
                            </div> 
            <br />
                        <div>
                            
                         
                                   <asp:Button ID="btnCurriculaAssocAdd" runat="server" Text="Add New Associations" style="margin-right: 5px" Width="230px"/>
                                          <ajaxToolkit:ModalPopupExtender ID="mpeCurriculaAddNewAssoc" BehaviorID="mpeCurriculaAddNewAssoc" runat="server" TargetControlID="btnCurriculaAssocAdd"
                                               PopupControlID="pnlCurriculaAddNewAssoc" BackgroundCssClass="modalBackground"  
                                                DropShadow="False" PopupDragHandleControlID="pnlCurriculaAddNewAssocTitle" RepositionMode="RepositionOnWindowResizeAndScroll"
                                                />
                             
                          
                         <asp:Button ID="btnCurriculaAssocClose" runat="server" Text="Cancel" Width="100px" OnClick="btnCurriculaAssocClose_Click" />
                       
                         </div>

        
        </asp:Panel>

        <asp:Panel ID="pnlCurriculaAddNewAssoc" runat="server" Style="display: none; width: 350px; height: 400px;" CssClass="modalPopup">
        
                <div>
                         <asp:Panel ID="pnlCurriculaAddNewAssocTitle" runat="server" Style="cursor: move; background-color: dimgray;
                            border: 1px solid black; color: Black; text-align: center;">
                            <div>
                                <p style="font-weight: bold;">
                                    Add New Curricula Association</p>
                            </div>
                        </asp:Panel>
                </div>
            <br />
            <div>
                        <asp:Panel ID="pnlCurriculaAddNewAssocContent" runat="server" Style="background-color: white;" Height="188px" >
                             
                          
                               <asp:UpdatePanel ID="upCurriculaAddNewAssocContent" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                                      
                               <b><i>
    <asp:Label ID="lblCurricula_AddNew_DocID" runat="server" Text="DocumentID:"></asp:Label></i></b>
<br />
                                        <b><i>
    <asp:Label ID="lblCurricula_AddNew_DocType" runat="server" Text="DocumentType:"></asp:Label></i></b>
                                        <br />
<br />              
                     <table>
                            <tr>
                                <td>
                                    <asp:Label ID="Label2" runat="server" Text="Grade:"></asp:Label>
                                </td>
                                <td>
                                    
                                   <telerik:RadComboBox ID="cmbGrade" runat="server" AutoPostBack="true" CausesValidation="False" EmptyMessage="Select&nbsp;One" OnSelectedIndexChanged="cmbGrade_SelectedIndexChanged" Skin="Web20" ToolTip="Select&nbsp;a&nbsp;Grade" Width="220px" maxheight="170px" ZIndex="500000">
                                    </telerik:RadComboBox>

                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label ID="Label1" runat="server" Text="Subject:"></asp:Label>
                                </td>
                                <td>
                                     <telerik:RadComboBox ID="cmbSubject" runat="server"  AutoPostBack="true" CausesValidation="False" EmptyMessage="Select&nbsp;One" OnSelectedIndexChanged="cmbSubject_SelectedIndexChanged" Skin="Web20" ToolTip="Select&nbsp;a&nbsp;Subject" Width="220px" maxheight="150px" ZIndex="500000">
                                    </telerik:RadComboBox>
                                </td>
                               
                            </tr>
                         
                         
                            <tr>
                                <td>
                                    <asp:Label ID="Label4" runat="server" Text="Course:"></asp:Label>
                                </td>
                                 <td>
                                     <telerik:RadComboBox ID="cmbCurriculum" runat="server" AutoPostBack="true" CausesValidation="False" EmptyMessage="Select&nbsp;One" OnSelectedIndexChanged="cmbCurriculum_SelectedIndexChanged" Skin="Web20" ToolTip="Select&nbsp;a&nbsp;Course" Width="220px" maxheight="130px" ZIndex="500000">
                                    </telerik:RadComboBox>
                                    </td>
                            </tr>
                        </table>
                     
                         <%-- Buttons for the modal popup 2 --%>  
                                        <div><br /><br /><br /> </div>
                                        

                                <div>       
                           
                      <asp:Button ID="btnCurriculaAddNewAssocOK" runat="server" Text="Add Selected Association" Style="margin-left: 40px; margin-bottom: 30px; margin-top: 10px" Enabled="false" OnClick="btnCurriculaAddNewAssocOK_Click" />
                      <%--<asp:Button ID="btnCurriculaAddNewAssocClose" runat="server" Text="Cancel" OnclientClick="$find('mpeCurriculaAddNewAssoc').hide(); return false;" Style="margin-left: 40px; margin-bottom: 30px; margin-top: 10px"/>
                         --%>
                                    <asp:Button ID="btnCurriculaAddNewAssocClose" runat="server" Text="Cancel" OnClick="btnCurriculaAddNewAssocClose_Click" Style="margin-left: 40px; margin-bottom: 30px; margin-top: 10px"/>
                         
                                </div>
                       
                       
                       
                             </ContentTemplate>
                                   
                                </asp:UpdatePanel>
                               
                      
                            
                             
                              
                        
                                       
                 </asp:Panel>
                  </div>               
        
        </asp:Panel>

<script>


    //$(document).keyup(function (e) {
    //    alert();
        
    //    if (e.keyCode == 27) {
    //        alert();

    //        var mdpopup1 = $find('mpeCurriculaAssoc');
    //        var mdpopup2 = $find('mpeCurriculaAddNewAssoc');
            
    //        if (mdpopup2) {
    //            mdpopup2.hide();

    //        }
    //        else {
    //            if (mdpopup1) {

    //            }
    //        }
    //    }
    //});
</script>


