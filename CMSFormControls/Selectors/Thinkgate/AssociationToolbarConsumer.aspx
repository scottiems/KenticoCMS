<%@ Page Language="C#" AutoEventWireup="true" CodeFile="AssociationToolbarConsumer.aspx.cs" Inherits="AssociationToolbarConsumer" %>

<%@ Register Src="~/CMSWebParts/ThinkgateWebParts/AssociationToolbar.ascx" TagPrefix="uc1" TagName="AssociationToolbar" %>
<%@ Register Src="~/CMSWebParts/ThinkgateWebParts/AssociationControl.ascx" TagPrefix="uc1" TagName="AssociationControl" %>


<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <uc1:AssociationToolbar runat="server" ID="AssociationToolbar" DocumentID="664" DocumentType="UnitPlan" UserID="tgtmanager">
                <AssociationControlTemplate>
                    <uc1:AssociationControl runat="server" ID="CurriculumAssociationControl" Text="Curriculum" Category="Curricula" URL="CurriculumPlanControlSelector.aspx" />
                    <uc1:AssociationControl runat="server" ID="StandardsAssociationControl" Text="Standards" Category="Standard" URL="StandardSelector.aspx" />
                    <uc1:AssociationControl runat="server" ID="LRMIAssociationControl" Text="Tags" Category="LRMI" URL="CurriculumPlanControlSelector.aspx" />
                    <uc1:AssociationControl runat="server" ID="AssociationControl1" Text="Resources" Category="Resource" URL="CurriculumPlanControlSelector.aspx" />
                </AssociationControlTemplate>

            </uc1:AssociationToolbar>
        </div>
    </form>
</body>
</html>
