<%@ Page Language="C#" AutoEventWireup="true" CodeFile="AssociationDataList.aspx.cs" Inherits="CMSFormControls_Selectors_Thinkgate_AssociationDataList" %>

<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>

<!DOCTYPE html>

<!-- Add jQuery library -->
<link href="http://ajax.googleapis.com/ajax/libs/jqueryui/1.8/themes/base/jquery-ui.css" rel="stylesheet" type="text/css" />
<script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jquery/1.8/jquery.min.js"></script>
<script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jqueryui/1.8/jquery-ui.min.js"></script>

<script type="text/javascript">
    function addAssociation() {
        document.getElementById("<% =HiddenField1.ClientID %>").value = "true";
    }
</script>

<style type="text/css">
    body {
        font-family: Verdana, Arial;
        font-size: 11px;
        margin: 0px;
    }

    #addAssociationIconDiv {
        position: relative;
        border: solid 1px gray;
        border-radius: 12px;
        padding: 7px;
        cursor: pointer;
        margin-top: 5px;
        width: 110px;
        margin-left: 5px;
        margin-right: 2px;
        float: right;
    }

    #addAssociationIcon {
        top: 5px;
        width: 16px;
        height: 16px;
        background: url('<%:this.ResolveUrl("~/")%>a.aspx?cmsimg=/ug/Add.png') no-repeat 0 0;
        float: left;
        margin-left: 2px;
        cursor: pointer;
    }

    .controlDataList {
        margin-left: 5px;
    }
    .auto-style1 {
        width: 67px;
    }
</style>
<html>
<body>
    <form id="formAssociationDataList" runat="server">
        <div id="sidebar" style="height: 209px; overflow: auto; margin-top: 10px; width: 263px;">
            <table>
            <asp:Repeater ID="rptAssociationDetail" runat="server">
                <ItemTemplate>
                    <div style="line-height: 20px;">
                        <tr>
                            <td>
                        <asp:LinkButton ID="lnkDeleteAssociation" OnClientClick="deleteCounter()" OnCommand="lnkDeleteAssociation_Command" CommandArgument='<%# Eval("ID")%>' CommandName="Delete" runat="server" >
                            <asp:Image BorderColor="Transparent" BorderWidth="0" ID="Image1" ImageUrl="~/a.aspx?cmsimg=/ug/Delete.png" runat="server"  />
                        </asp:LinkButton>
                            </td>
                            <td> &nbsp;&nbsp;&nbsp;&nbsp </td>
                            <td>
                         <span class="controlDataList" title='<%# Eval("Detail") %>'><%# Eval("Detail")%></span>
                                </td>
                            </tr>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
            </table>
                <br />
        </div>
            <div id="addAssociationIconDiv" title="Add Associations" onclick="addAssociation();">
               <span id="addAssociationIconLeft" style="margin-left: 5px;" onclick="openModalAssocToolBarDialog('<%=this.RedirectURL%>');">Add <%=this.Category.ToString() %></span><span id="addAssociationIcon" onclick="openModalAssocToolBarDialog('<%=this.RedirectURL%>');"></span>
            </div>
        <asp:HiddenField ID="HiddenField1" runat="server" Value="false" />

      <div id="associationdialog" title="Selector" style="display: none;">
            <iframe id="associationframe" style="width: auto; height:auto;" src="" frameborder="0">No frames</iframe>
        </div>

    <script type="text/javascript">

        $(function () {
            
            $('#associationdialog').dialog({                
                autoOpen: false, modal: true, width:'600', height: '800', position: 'center', close: function (event, ui) {
                    $('#associationframe').attr('src', '');
                }
            });
        });

        function openModalAssocToolBarDialog(url) {
            
            if (window.parent && window.parent.openModalAssocToolBarDialog) {
               
                ctrlid = '<%= Request.QueryString["ctrl"] %>';
                var documentid = '<%= Request.QueryString["documentid"] %>';
                var documenttype = '<%= Request.QueryString["documenttype"] %>';
                var associationType = '<%= Request.QueryString["category"] %>';
                var resource=0;
                if (associationType == 'Resource') {
                    resource = 1;
                }
                window.parent.openModalAssocToolBarDialog(event, documentid, documenttype, '0', 0, associationType, url, null, null, ctrlid, resource);

            }
            return;
        }

        function deleteCounter() {
            $(window).unload(function () {
               var counter = '<%= AssociationCount %>';
                counter--;
                window.parent.CloseMeAssocToolBar(counter, ctrlid);

                if (counter == 0) {
                    window.parent.location.reload();
                }
   
             });

        }
          $(document).ready(function () {
            ctrlid = '<%= Request.QueryString["ctrl"]%>';
           });

        $(window).unload(function () {
               var counter = '<%= AssociationCount %>';
            window.parent.CloseMeAssocToolBar(counter, ctrlid);
            
            
            });

    </script>

    

    </form>

    </body>
</html>
