<%@ Control Language="C#" AutoEventWireup="true" CodeFile="LRMITag.ascx.cs" Inherits="CMSWebParts_ThinkgateWebParts_LRMITag" %>

<telerik:RadCodeBlock ID="RadCodeBlock" runat="server">
    <script type="text/javascript">

        $j(document).ready(function () {
            $j('#divsucc').css('display', 'block');
            $j('#divsucc').hide();
        });

        function ClosePopup() {
            window.parent.jQuery('.tgAddExistingDialog').children(".ui-dialog-titlebar").children("button").click();
        }



        function showmessage() {
            $j('#divsucc').show();
            $j('#divsucc').hide(10000);

        }


        function SaveStandards(docid, selectedItems) {

            $j.ajax({
                type: "POST",
                url: "./btWebServices.aspx/AddSelectedItems",
                data: "{'docid':'" + docid + "', 'SelectedItems':'" + selectedItems + "'}",

                contentType: "application/json; charset=utf-8",
                dataType: "json",

                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    //alert(textStatus + "\n" + errorThrown);
                },
                success: function (result) {
                    //getStandardCount(docid);
                    //alert("succeed");
                }
            });
        }

        var currentDisplayControl = 'undefined';
        var currentInputControl = 'undefined';

        $j(function () {
            $j('#addStandardsDialog').dialog({
                autoOpen: false,
                modal: true,
                width: 940,
                height: 530,
                position: 'center',
                close: function (event, ui) {

                }
            });
        });

        function openModalDialog(displayControlId, inputControlId) {
            currentDisplayControl = displayControlId;
            currentInputControl = inputControlId;
            if (currentInputControl) {
                oldselectedItems = $j("#" + currentInputControl).val();
            }

            var url = '<%:Request.ApplicationPath%>/CMSWebParts/ThinkgateWebParts/StdAddNewAssoc.aspx?parentnodeid=' + <%=this.DocumentID.ToString() %> + '&ReturnURL=LRMI&LRMIItemID=' + $j("#<%=hdnLRMIItemId.ClientID%>").val();
            $j('#addStandardsFrame').attr('src', url);
            $j('#addStandardsDialog').dialog("option", "title", "Tags - Standards Association");

            $j('#addStandardsDialog').dialog("open");
        }

        function CloseDialog() {
            $j('#addStandardsDialog').dialog("close");
        }

        function closeModalDialog(standardIds, standardName) {
            //get the transferred arguments
            if (currentInputControl) {
                var stdIds = $j("#" + currentInputControl).val();
                stdIds += standardIds;
                $j("#" + currentInputControl).val(stdIds);
                selectedItems = $j("#" + currentInputControl).val();
            }
            if (currentDisplayControl) {
                var ctrlval = $j("#" + currentDisplayControl).html();
                ctrlval += standardName;
                $j("#" + currentDisplayControl).html(ctrlval);
            }
        }

        function getDocIDValue() {
            return $j("#DocID").val();
        }

    </script>
</telerik:RadCodeBlock>
<style type="text/css">
    body, form {
        font-family: Tahoma;
        font-size: 12px;
    }

    .inputLabel {
        font-weight: bold;
        padding-top: 8px;
        padding-bottom: 8px;
        vertical-align: middle;
        width: 200px;
    }

    .frmLabel {
        font-weight: bold;
        padding-top: 5px;
        padding-bottom: 5px;
        vertical-align: middle;
    }

    .frmValue {
        vertical-align: middle;
    }

    .validationMessage {
        color: red;
    }

    #addStandardIconDiv {
        /*display: block;*/
        position: relative;
        /*float: right;*/
        border: solid 1px gray;
        border-radius: 15px;
        padding: 3px;
        cursor: pointer;
        margin-top: 5px;
        width: 110px;
    }

    #addStandardIcon {
        top: 5px;
        width: 16px;
        height: 16px;
        background: url('a.aspx?cmsimg=/ug/Add.png') no-repeat 0 0;
        float: left;
        margin-left: 5px;
        cursor: pointer;
    }
</style>

<telerik:RadAjaxPanel runat="server" ID="LRMIPanel" LoadingPanelID="LRMILoadingPanel">
    <asp:HiddenField ID="DocID" runat="server" />
    <%
        int nodeID = Convert.ToInt32(this.DocumentID);
        bool createAllowed = false;
        bool modifyAllowed = false;

        if (nodeID > 0)
        {
            CMS.DocumentEngine.TreeNode treenode = TreeHelper.SelectSingleNode(nodeID);

            createAllowed = treenode.CheckPermissions(CMS.SettingsProvider.PermissionsEnum.Create, CMSContext.CurrentSiteName, CMSContext.CurrentUser);
            modifyAllowed = treenode.CheckPermissions(CMS.SettingsProvider.PermissionsEnum.Modify, CMSContext.CurrentSiteName, CMSContext.CurrentUser);
        }
    %>
    <div id="divMain" style="width: 400px; border: solid 1px black; padding: 10px;">
        <div>
            <div id="divsucc" style="display: none;">
                <span id="m_pM_lS" class="AdvancedLabel Succ" style="opacity: 1; position: fixed; top: 29px; margin-top: 10px; max-width: 861px; left: 10px;">The changes were saved.</span>
            </div>
            <table id="tblEducationalAlignment" runat="server" width="100%">
                <tr id="trEducationalAlignment" runat="server">
                    <td class="inputLabel">Educational Alignment:
                    </td>
                    <td style="text-align: right; padding-right: 30px;">
                        <asp:LinkButton runat="server" ID="lnkSelectTypes" CssClass="frmValue" CausesValidation="false" Text="Select Type(s)" OnClick="lnkSelectTypes_Click"></asp:LinkButton>
                    </td>
                </tr>
                <tr>
                    <td colspan="2" style="padding-left: 120px; padding-right: 30px;">
                        <asp:Panel runat="server" ID="pnlSelectEducationalAlignment" Visible="false" Width="100%" BorderWidth="1px" BorderColor="Black">
                            <asp:CheckBoxList runat="server" ID="cblEducationalAlignment" RepeatDirection="Vertical" RepeatColumns="2">
                            </asp:CheckBoxList>
                            <div style="text-align: right; padding: 5px;">
                                <telerik:RadButton runat="server" ID="RadButtonOK" Skin="Web20" CausesValidation="false" Text="OK" OnClick="RadButtonOK_Click" />
                            </div>
                        </asp:Panel>
                    </td>
                </tr>
                <tr>
                    <td colspan="2" style="padding-left: 30px; padding-right: 24px;">
                        <asp:Panel runat="server" ID="pnlEducationalAlignmentGrid" Visible="false">
                            <telerik:RadGrid runat="server" ID="rgEducationalAlignment" GridLines="None" AutoGenerateColumns="false" ShowHeader="false" Width="100%" BorderWidth="0px" OnItemDataBound="rgEducationalAlignment_ItemDataBound">
                                <ItemStyle BackColor="White" Font-Names="Tahoma" />
                                <AlternatingItemStyle BackColor="White" Font-Names="Tahoma" />
                                <MasterTableView DataKeyNames="ItemID, EducationalAlignmentEnum" ClientDataKeyNames="ItemID, EducationalAlignmentEnum">
									<Columns>
                                        <telerik:GridBoundColumn DataField="ItemID" UniqueName="ItemID" Display="false"></telerik:GridBoundColumn>
                                        <telerik:GridBoundColumn DataField="EducationalAlignmentEnum" UniqueName="EducationalAlignmentEnum" Display="false"></telerik:GridBoundColumn>
                                        <telerik:GridBoundColumn DataField="EducationalAlignmentText" UniqueName="EducationalAlignmentText" ItemStyle-BorderWidth="0px" ItemStyle-VerticalAlign="Top" ItemStyle-Width="160px"></telerik:GridBoundColumn>
                                        <telerik:GridTemplateColumn UniqueName="EducationalAlignmentGridTemplateColumn" ItemStyle-Width="90" ItemStyle-BorderWidth="0px" ItemStyle-VerticalAlign="Top" ItemStyle-HorizontalAlign="Right">
                                            <ItemTemplate>
                                                <asp:LinkButton ID="lblSelectedStandards" runat="server" OnClientClick="return false;" Style="cursor: text;"></asp:LinkButton>
                                                <asp:Label ID="btnAddStandards" runat="server">
                                                    <div id="addStandardIconDiv" title="Add Standards" runat="server" clientidmode="Static">
                                                        <span id="addStandardIcon" runat="server" clientidmode="Static"></span>&nbsp;&nbsp;Add Standards
                                                    </div>
                                                </asp:Label>
                                                <asp:TextBox ID="txtEducationalAlignmentValue" runat="server" MaxLength="200" Style="border: solid 1px #6788be; color: #333; font-size: 11.5px; font-family: 'segoe ui', arial, sans-serif; padding-bottom: 3px; padding-top: 2px; padding-left: 5px; padding-right: 5px; width: 148px;"></asp:TextBox>
                                            </ItemTemplate>
                                        </telerik:GridTemplateColumn>
                                        <telerik:GridBoundColumn DataField="EducationalAlignmentValues" UniqueName="EducationalAlignmentValues" Display="false"></telerik:GridBoundColumn>
                                        <telerik:GridBoundColumn DataField="LRMIItemID" UniqueName="LRMIItemID" Display="false"></telerik:GridBoundColumn>
                                    </Columns>
                                </MasterTableView>
                            </telerik:RadGrid>
                        </asp:Panel>
						<table style="width:100%" runat="server">
							<tr id="EducationalSubjectRow" runat="server" Visible="False">
								<td style="padding-left: 7px;">
									<asp:Label ID="lblEducationalSubject" runat="server" Width="160px">Educational Subject:</asp:Label>
								</td>
								<td >
									<telerik:RadComboBox runat="server" ID="rcbEducationalSubject" Skin="Web20" EmptyMessage="<Select>" ToolTip="Select an Educational Subject" />
								</td>
							</tr>
							<tr id="EducationalLevelRow" runat="server" Visible="False">
								<td style="padding-left: 7px;">
									<asp:Label ID="lblEducationalLevel" runat="server" Width="160px">Educational Level:</asp:Label>
								</td>
								<td>
									<telerik:RadComboBox runat="server" ID="rcbEducationalLevel" Skin="Web20" EmptyMessage="<Select>" ToolTip="Select an Educational Level" />
								</td>
							</tr>
						</table>
						

                    </td>
                </tr>
            </table>
            <table id="tblLRMIDetails" runat="server" width="100%">
                <tr>
                    <td class="inputLabel">Instruction Type:
                    </td>
                    <td>
                        <telerik:RadComboBox runat="server" ID="rcbInstructionType" Skin="Web20" EmptyMessage="<Select>" ToolTip="Select a Instruction Type" />
                    </td>
                </tr>
                <tr>
                    <td class="inputLabel">Activity Type:
                    </td>
                    <td>
                        <telerik:RadComboBox runat="server" ID="rcbActivityType" Skin="Web20" EmptyMessage="<Select>" ToolTip="Select a Activity Type" />
                    </td>
                </tr>
                <tr>
                    <td class="inputLabel">Learning Resource Type:
                    </td>
                    <td>
                        <telerik:RadComboBox runat="server" ID="rcbLearningResourceType" Skin="Web20" EmptyMessage="<Select>" ToolTip="Select a Learning Resource Type" />
                    </td>
                </tr>
                <tr>
                    <td class="inputLabel">Duration:
                    </td>
                    <td>
                        <telerik:RadTextBox runat="server" ID="rtbDuration" Skin="Web20" MaxLength="10" ToolTip="Enter Duration in Minutes" />
                        <asp:RegularExpressionValidator runat="server" CssClass="validationMessage" ErrorMessage="Duration should be numeric" Text="*" ValidationExpression="^[0-9]+$" ControlToValidate="rtbDuration" />
                    </td>
                </tr>
                <tr>
                    <td class="inputLabel">Target Audience:
                    </td>
                    <td>
                        <telerik:RadComboBox runat="server" ID="rcbTargetAudience" Skin="Web20" EmptyMessage="<Select>" ToolTip="Select a Target Audience" />
                    </td>
                </tr>
                <tr>
                    <td class="inputLabel">Age Appropriate Criteria:
                    </td>
                    <td>
                        <telerik:RadTextBox runat="server" ID="rtbAgeAppropriateCriteriaMin" Skin="Web20" Width="40px" MaxLength="2" ToolTip="Enter Minimum Age Criteria" />
                        <asp:RegularExpressionValidator runat="server" CssClass="validationMessage" ErrorMessage="Minimum Age Appropriate Criteria should be numeric" Text="*" ValidationExpression="^[0-9]+$" ControlToValidate="rtbAgeAppropriateCriteriaMin" />
                        &nbsp;-&nbsp;&nbsp;
                        <telerik:RadTextBox runat="server" ID="rtbAgeAppropriateCriteriaMax" Skin="Web20" Width="40px" MaxLength="2" ToolTip="Enter Maximum Age Criteria" />
                        <asp:RegularExpressionValidator runat="server" CssClass="validationMessage" ErrorMessage="Maximum Age Appropriate Criteria should be numeric" Text="*" ValidationExpression="^[0-9]+$" ControlToValidate="rtbAgeAppropriateCriteriaMax" />
                    </td>
                </tr>
                <br/>
                <tr>
                    <td colspan="2" style="text-align: right; padding-top: 20px; padding-right: 30px;">
                        <%
                            int nodeID = Convert.ToInt32(this.DocumentID);
                            bool createAllowed = false;
                            bool modifyAllowed = false;

                            if (nodeID > 0)
                            {
                                CMS.DocumentEngine.TreeNode treenode = TreeHelper.SelectSingleNode(nodeID);

                                createAllowed = treenode.CheckPermissions(CMS.SettingsProvider.PermissionsEnum.Create, CMSContext.CurrentSiteName, CMSContext.CurrentUser);
                                modifyAllowed = treenode.CheckPermissions(CMS.SettingsProvider.PermissionsEnum.Modify, CMSContext.CurrentSiteName, CMSContext.CurrentUser);
                            }
                        %>
                         <%
                            if (modifyAllowed)
                            {
                         %>
                        <telerik:RadButton runat="server" ID="RadButtonSave" Text="Associate Tags to Document" OnClick="RadButtonSave_Click" />
                        <%
                            }
                        %>
                        <%--<telerik:RadButton runat="server" ID="RadButtonCancel" Skin="Web20" CausesValidation="false" Text="Cancel" OnClick="CloseDialog();"  />--%>
                        <input runat="server" id="RadButtonCancel" type="button" value="Cancel" onclick="ClosePopup();" />
                    </td>
                </tr>
            </table>
            <asp:HiddenField runat="server" ID="hdnLRMIItemId" Value="0" />
            <asp:ValidationSummary runat="server" EnableClientScript="true" ShowMessageBox="true" ShowSummary="false" />
        </div>
        <hr />
        <div>
            <table id="tblDocDetails" runat="server">
                <tr>
                    <td>
                        <span class="frmLabel">Title:
                        </span>
                        <asp:Label runat="server" ID="lblTitle" CssClass="frmValue"></asp:Label>
                    </td>
                </tr>
                <tr>

                    <td>
                        <span class="frmLabel">Topic:
                        </span>
                        <asp:Label runat="server" ID="lblTopic" CssClass="frmValue"></asp:Label>
                    </td>
                </tr>
                <tr>

                    <td>
                        <span class="frmLabel">Created:
                        </span>
                        <asp:Label runat="server" ID="lblCreated" CssClass="frmValue"></asp:Label>
                    </td>
                </tr>
                <tr>

                    <td>
                        <span class="frmLabel">Creator:
                        </span>
                        <asp:Label runat="server" ID="lblCreator" CssClass="frmValue"></asp:Label>
                    </td>
                </tr>
                <tr>

                    <td>
                        <span class="frmLabel">Publisher:
                        </span>
                        <asp:Label runat="server" ID="lblPublisher" CssClass="frmValue"></asp:Label>
                    </td>
                </tr>
                <tr>

                    <td>
                        <span class="frmLabel">Language:
                        </span>
                        <asp:Label runat="server" ID="lblLanguage" CssClass="frmValue"></asp:Label>
                    </td>
                </tr>
                <tr>

                    <td>
                        <span class="frmLabel">Media Type:
                        </span>
                        <asp:Label runat="server" ID="lblMediaType" CssClass="frmValue"></asp:Label>
                    </td>
                </tr>
            </table>
        </div>
    </div>

</telerik:RadAjaxPanel>

<div id="addStandardsDialog" title="" style="display: none;">
    <iframe id="addStandardsFrame" width="910" height="480" src="" frameborder="0">No frames</iframe>
</div>

<telerik:RadAjaxLoadingPanel ID="LRMILoadingPanel" runat="server" Height="100%" Width="75px" Transparency="50" BackgroundPosition="Center">
    <img alt="Loading..." src="../../CMSWebParts/Viewers/Effects/lightbox_files/images/loading.gif" style="border: 0;" />
</telerik:RadAjaxLoadingPanel>
