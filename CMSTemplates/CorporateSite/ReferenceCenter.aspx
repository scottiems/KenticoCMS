<%@ Page Title="" Language="C#" MasterPageFile="~/CMSTemplates/CorporateSite/MasterPage.master" AutoEventWireup="true" CodeFile="ReferenceCenter.aspx.cs" Inherits="CMSTemplates_CorporateSite_ReferenceCenter" %>

<asp:Content ID="Content1" ContentPlaceHolderID="plcMain" Runat="Server">
    <cms:CMSDataList ID="referenceCenterDataList" runat="server" 
        ClassNames="Thinkgate.ReferenceCenter" OrderBy="Title ASC" 
        RepeatColumns="2" 
        SelectedItemTransformationName="Thinkgate.ReferenceCenter.default" 
        TransformationName="Thinkgate.ReferenceCenter.preview" />
</asp:Content>

