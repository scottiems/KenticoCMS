<%@ Page Title="" Language="C#" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" AutoEventWireup="true" 
      ValidateRequest="false" Theme="Default" EnableEventValidation="false"
    CodeFile="MessageCenterEdit.aspx.cs" Inherits="CMSModules_Content_CMSDesk_Edit_MessageCenterEdit" %>

<%@ Register Src="~/CMSModules/Content/Controls/EditMenu.ascx" TagName="editmenu"
    TagPrefix="cms" %>

<asp:Content ID="cntMenu" ContentPlaceHolderID="plcBeforeContent" runat="server">
    <cms:editmenu ID="menuElem" ShortID="m" runat="server" ShowProperties="false" ShowSpellCheck="true"
        IsLiveSite="false" />  

</style>  
</asp:Content>
<asp:Content ID="cntBody" ContentPlaceHolderID="plcContent" runat="server">   
      
    <div id="CMSHeaderDiv">
        <div id="CKToolbar">
        </div>
    </div>
    <asp:Panel runat="server" ID="pnlContent" CssClass="ContentEditArea">
        <cms:CMSForm runat="server" ID="formElem" Visible="false" HtmlAreaToolbarLocation="Out:CKToolbar"
            ShowOkButton="false" IsLiveSite="false" ShortID="f" />
        <span class="ClearBoth"></span>
        <br />
        <%-- SKU binding --%>
        <asp:PlaceHolder ID="plcSkuBinding" runat="server" Visible="false">
            <div class="PageSeparator">
                <cms:LocalizedLabel ID="lblBindSKUInfo" runat="server" ResourceString="com.bindAnSkuInfo"
                    CssClass="InfoLabel EditingFormLabel" EnableViewState="false" />
                <asp:LinkButton ID="btnBindSku" runat="server" CssClass="ContentNewLink" EnableViewState="false">
                    <cms:CMSImage ID="imgBindSku" runat="server" EnableViewState="false" />
                    <cms:LocalizedLabel ID="lblBindSKU" runat="server" ResourceString="com.skubinding.bind"
                        EnableViewState="false" />
                </asp:LinkButton>
                <span class="ClearBoth"></span>
            </div>
        </asp:PlaceHolder>
    </asp:Panel>
    <cms:CMSButton ID="btnRefresh" runat="server" CssClass="HiddenButton" EnableViewState="false"
        OnClick="btnRefresh_Click" UseSubmitBehavior="false" />
    <asp:PlaceHolder ID="plcCKFooter" runat="server">
        <div id="CMSFooterDiv">
            <div id="CKFooter">
            </div>
        </div>
    </asp:PlaceHolder> 

    <script src="<%:this.ResolveUrl("~/")%>CMSPages/GetResource.ashx?scriptfile=%7e%2fCMSScripts%2fjquery%2fjquery-core.js" type="text/javascript"></script>
    <script type="text/javascript">

        jQuery(document).ready(function () {
            jQuery("#CKFooter").hide();
        });

        jQuery(function () {
            var tableObj = jQuery("table.CheckBoxListField");
            var firstChkBox = jQuery("input:checkbox", tableObj).first();
            var allChkBoxes = jQuery("input:checkbox", tableObj);

            var checkedStatus = false;
            var firstChkCheckedStatus = false;
            firstChkBox.change(function (e) {
                var chk = jQuery(this);
                if (chk.val() == "All") {
                    if (chk.attr("checked") == "checked") {
                        jQuery("input:checkbox", tableObj).attr("checked", "checked");
                        firstChkCheckedStatus = true;
                    }
                    else {
                        jQuery("input:checkbox", tableObj).removeAttr("checked");
                        firstChkCheckedStatus = false;
                    }
                }
            });

            allChkBoxes.change(function (e) {
                if (firstChkCheckedStatus) {
                    for (var i = 1; i < allChkBoxes.length; i++) {
                        if (jQuery(allChkBoxes[i]).attr("checked") != "checked") {
                            jQuery(firstChkBox).removeAttr("checked");
                            firstChkCheckedStatus = false;
                        }
                    }
                }
                else {
                    checkedStatus = true;
                    for (var i = 1; i < allChkBoxes.length; i++) {
                        if (jQuery(allChkBoxes[i]).attr("checked") != "checked") {
                            checkedStatus = false;
                        }
                    }
                    if (checkedStatus) {
                        jQuery(firstChkBox).attr("checked", "checked");
                        firstChkCheckedStatus = true;
                    }
                    else {
                        if (jQuery(firstChkBox).attr("checked") == "checked") {
                            jQuery(firstChkBox).removeAttr("checked");
                        }
                    }
                }
            });
        });

    </script>

</asp:Content>

   

