<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Content_CMSDesk_Edit_Edit"
    ValidateRequest="false" Theme="Default" EnableEventValidation="false" CodeFile="Edit.aspx.cs"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" %>

<%@ Register Src="~/CMSModules/Content/Controls/EditMenu.ascx" TagName="editmenu"
    TagPrefix="cms" %>
<asp:Content ID="cntMenu" ContentPlaceHolderID="plcBeforeContent" runat="server">
    <cms:editmenu ID="menuElem" ShortID="m" runat="server" ShowProperties="false" ShowSpellCheck="true"
        IsLiveSite="false" />
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
                </asp:LinkButton><span class="ClearBoth"></span></div></asp:PlaceHolder></asp:Panel><cms:CMSButton ID="btnRefresh" runat="server" CssClass="HiddenButton" EnableViewState="false"
        OnClick="btnRefresh_Click" UseSubmitBehavior="false" /><asp:PlaceHolder ID="plcCKFooter" runat="server"><div id="CMSFooterDiv"><div id="CKFooter"></div></div></asp:PlaceHolder><script src="<%:this.ResolveUrl("~/")%>CMSPages/GetResource.ashx?scriptfile=%7e%2fCMSScripts%2fjquery%2fjquery-core.js" type="text/javascript"></script>
    <script language="javascript" type="text/javascript">
        
        jQuery(document).ready(function () {

            var title = '<%= SetFormTitle %>';
            jQuery("table.rwTitlebarControls td").context.title = title;

            ShowHideAttachSection();
          
            
            
            jQuery("img[alt=Save]").parent().click(function () {
                window.onbeforeunload = null;
            });

            if (window.top == window.self) {
                window.onbeforeunload = function () {
                    return 'Are you sure you wish to close this window? Any unsaved changes will be lost.';
                };
            }
        });


        
     
        window.onunload = refreshParent;
        function refreshParent() {
            window.opener.location.reload();
            return true;
        }

        // This function Show/hide web Link and Attach file field on the basis
        // of SubType selected. It is written for Resource Add/Edit. Bydefault this section
        // will be hidden. Tried to server side coding to hide these fields conditionaly.
        // But this was not working because it is created by Kentico and we have not server side ID for these labels.
        function ShowHideAttachSection() {
            var subTypeDropDown = jQuery("#m_c_f_f_SubType_dropDownList");
            var lblAttachmentName = jQuery("#m_c_f_f_AttachmentName_lb");
            var lblWebLink = jQuery("#m_c_f_f_Weblink_lb");
            var lblAttachSection = jQuery(".CollapsibleFieldSet");
            lblAttachSection.hide();
            lblAttachmentName.hide();
            lblWebLink.hide();
            var selectedValue = subTypeDropDown.val();
            if (typeof(subTypeDropDown[0]) != 'undefined') {
                for (var i = 0; i < subTypeDropDown[0].length; i++) {
                    var items = subTypeDropDown[0][i];
                    if (selectedValue == items.value) {
                        //if (items.childNodes[0].textContent.search('Web-Based') > 0) {
                        if (items.childNodes[0].nodeValue.search('Web-Based') > 0) {
                            lblWebLink.show();
                            lblAttachmentName.hide();
                            lblAttachSection.show();
                        }
                        if (items.childNodes[0].nodeValue.search('Attachment') > 0) {
                            lblAttachmentName.show();
                            lblWebLink.hide();
                            lblAttachSection.show();
                        }
                    }
                }
            }
        }
    </script>



</asp:Content>
