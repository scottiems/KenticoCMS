<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSWebParts_ThinkgateWebParts_AssociationFullControl" CodeFile="~/CMSWebParts/ThinkgateWebParts/AssociationFullControl.ascx.cs" %>

<!-- Add jQuery library -->
<link href="http://ajax.googleapis.com/ajax/libs/jqueryui/1.10.1/themes/base/jquery-ui.css" rel="stylesheet" type="text/css" />
<script type="text/javascript" src="//ajax.googleapis.com/ajax/libs/jquery/1.9.1/jquery.min.js"></script>
<script type="text/javascript" src="//ajax.googleapis.com/ajax/libs/jqueryui/1.10.1/jquery-ui.min.js"></script>

<link href="<%:this.ResolveUrl("~/")%>CMSWebParts/ThinkgateWebParts/css/associationToolbar.css" rel="stylesheet" />

<%@ Register Src="~/CMSWebParts/ThinkgateWebParts/AssociationToolbar.ascx" TagPrefix="uc2" TagName="AssociationToolbar" %>
<%@ Register Src="~/CMSWebParts/ThinkgateWebParts/AssociationControl.ascx" TagPrefix="uc1" TagName="AssociationControl" %>

<div style="padding-left:10px;">
    <uc2:AssociationToolbar runat="server" ID="AssociationToolbar" DocumentID="955" DocumentType="InstructionPlan" >
        <AssociationControlTemplate>
			<div class="toolBar">
            
				<div id="Curricula" class="toolBarWrapper">
                        <uc1:AssociationControl runat="server" ID="CurriculumAssociationControl" Text="Curricula" Category="Curricula"
							URL="~/CMSFormControls/Selectors/Thinkgate/CurriculumPlanControlSelector.aspx"
							AssociationDataListURL="~/CMSFormControls/Selectors/Thinkgate/AssociationDataList.aspx"
                            AssociationDataListDialogTitle="Associated Curricula" />
				</div>
				<div id="Standards" class="toolBarWrapper">
                        <uc1:AssociationControl runat="server" ID="StandardsAssociationControl" Text="Standards" Category="Standard"
                            URL="~/CMSFormControls/Selectors/Thinkgate/StandardSelector.aspx?FromToolBar=1"
                            AssociationDataListURL="~/CMSFormControls/Selectors/Thinkgate/AssociationDataList.aspx"
                            AssociationDataListDialogTitle="Associated Standards" />
				</div>
				<div id="Resources" class="toolBarWrapper">
                    <uc1:AssociationControl runat="server" ID="AssociationControl1" Text="Resources" Category="Resource"
                        URL="~/CMSFormControls/Selectors/Thinkgate/ResourceSelector.aspx"
                        AssociationDataListURL="~/CMSFormControls/Selectors/Thinkgate/AssociationDataList.aspx"
                        AssociationDataListDialogTitle="Associated Resources" />
				</div>
				<div id="Tags" class="toolBarWrapper">
                        <uc1:AssociationControl runat="server" ID="LRMIAssociationControl" Text="Tags" Category="LRMI"
                            URL="~/CMSFormControls/Selectors/Thinkgate/LRMITagSelector.aspx" />
				</div>
			</div>
        </AssociationControlTemplate>
    </uc2:AssociationToolbar>
</div>

