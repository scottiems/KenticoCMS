<%@ Control Language="C#" AutoEventWireup="true" CodeFile="AssociationControlTester.ascx.cs" Inherits="CMSWebParts_ThinkgateWebParts_AssociationControlTester" %>

<link href="http://ajax.googleapis.com/ajax/libs/jqueryui/1.10.1/themes/base/jquery-ui.css" rel="stylesheet" type="text/css" />
<script type="text/javascript" src="//ajax.googleapis.com/ajax/libs/jquery/1.9.1/jquery.min.js"></script>
<script type="text/javascript" src="//ajax.googleapis.com/ajax/libs/jqueryui/1.10.1/jquery-ui.min.js"></script>

<%@ Register Src="~/CMSWebParts/ThinkgateWebParts/AssociationControl.ascx" TagPrefix="uc1" TagName="AssociationControl" %>

<div>
<uc1:AssociationControl runat="server" ID="CurriculumAssociationControl" Text="Curricula" Category="Curricula" URL="~/CMSFormControls/Selectors/Thinkgate/CurriculumPlanControlSelector.aspx" />
</div>