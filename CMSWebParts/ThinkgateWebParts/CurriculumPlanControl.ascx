<%@ Control Language="C#" AutoEventWireup="true" CodeFile="CurriculumPlanControl.ascx.cs" Inherits="CMSWebParts_ThinkgateWebParts_CurriculumPlanControl" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>

<!-- Add jQuery library -->
<link href="http://ajax.googleapis.com/ajax/libs/jqueryui/1.8/themes/base/jquery-ui.css" rel="stylesheet" type="text/css" />
<script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jquery/1.8/jquery.min.js"></script>
<script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jqueryui/1.8/jquery-ui.min.js"></script>
<link href="css/Button.css" rel="stylesheet" />
<b><i>
    <asp:Label ID="Label1" runat="server" Text="Label"></asp:Label></i></b>
<br />
<asp:UpdatePanel ID="UpdatePanel1" runat="server">
                        <ContentTemplate>
<telerik:RadCodeBlock ID="RadCodeBlock" runat="server">
    <script type="text/javascript">
        
        $(document).ready(function () {
         
            ctrlid = '<%= Request.QueryString["ctrl"]%>';

        });

        $(window).unload(function () {
       
           
            var counter = '<%= AssociationCount %>';
          
            window.parent.CloseMeAssocToolBar(counter, ctrlid);
        
         });

        function dialogCancelled() {
          
            window.parent.closeAssocToolBardialog();
     
           popupAssociateCurricula();
 }




        function associationAdded() {
         
            $(window).unload(function () {
         
                var counter = '<%= AssociationCount %>';
                counter++;
          
                window.parent.CloseMeAssocToolBar(counter, ctrlid);
                if (counter == 1)
                {   window.parent.location.reload();
                }
       
            });

            window.parent.closeAssocToolBardialog();
         

        }


      function popupAssociateCurricula()
      {
       
          var CPCurl = '<%= CPCurl %>';
          var CPCAssociationDataListURL = '<%= CPCAssociationDataListURL %>';
          var CPCAssociationDataListDialogTitle = '<%= CPCAssociationDataListDialogTitle %>';
          var docID = '<%= docID %>';
          var docType = '<%= docType %>';
          var uID = '<%= uID %>';
          alert(docType);
         
          window.parent.openModalAssocToolBarDialog(event, docID, docType, uID, '<%= AssociationCount%>', "Curricula", CPCurl, CPCAssociationDataListURL, CPCAssociationDataListDialogTitle, ctrlid + " ");

      }


       
    </script>
 
</telerik:RadCodeBlock>

        <div id="main">
            <div id="banner" style="width: 385px; height: 221px;">
                <div id="curriculumSelectionDiv" style="display: block;">
                    <div id="dropdowns" style="text-align: left; padding: 1em;">
       
                 
                        <table>
                            <tr>
                                <td>
                                    <asp:Label ID="Label2" runat="server" Text="Grade:"></asp:Label>
                                </td>
                                <td>
                                    <telerik:RadComboBox ID="cmbGrade" runat="server" AutoPostBack="true" CausesValidation="False" EmptyMessage="Select&nbsp;One" OnSelectedIndexChanged="cmbGrade_SelectedIndexChanged" Skin="Web20" ToolTip="Select&nbsp;a&nbsp;Grade" Width="220px">
                                    </telerik:RadComboBox>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label ID="Label3" runat="server" Text="Subject:"></asp:Label>
                                </td>
                                <td>
                                    <telerik:RadComboBox ID="cmbSubject" runat="server" AutoPostBack="true" CausesValidation="False" EmptyMessage="Select&nbsp;One" OnSelectedIndexChanged="cmbSubject_SelectedIndexChanged" Skin="Web20" ToolTip="Select&nbsp;a&nbsp;Subject" Width="220px">
                                    </telerik:RadComboBox>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label ID="Label4" runat="server" Text="Course:"></asp:Label>
                                </td>
                                <td>
                                    <telerik:RadComboBox ID="cmbCurriculum" runat="server" AutoPostBack="true" CausesValidation="False" EmptyMessage="Select&nbsp;One" OnSelectedIndexChanged="cmbCurriculum_SelectedIndexChanged" Skin="Web20" ToolTip="Select&nbsp;a&nbsp;Course" Width="220px">
                                    </telerik:RadComboBox>
                                </td>
                            </tr>
                        </table>
                          
         

                     
                    </div>
                </div>
                <div>
                    <asp:Button ID="btnAssociateCurricula" runat="server" Height="26px" Enabled="false" OnClick="btnAssociateCurricula_Click" OnClientClick="associationAdded()" Text="Associate Selected Curriculum" Width="280px" />
                    &nbsp;&nbsp;
                    <asp:Button ID="btnCancel" Height="26px" OnClientClick="dialogCancelled()" Text="Cancel" Width="80px" runat="server" />
                  
                
                </div>
               
                <asp:HiddenField ID="HiddenField1" runat="server" Value="false" />
                 
            </div>
        </div>
   
 </ContentTemplate>
</asp:UpdatePanel>
