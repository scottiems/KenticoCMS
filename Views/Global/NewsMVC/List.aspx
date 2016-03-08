<%@ Page Language="C#" AutoEventWireup="true" CodeFile="List.aspx.cs" Inherits="Views_Global_NewsMVC_List"
    MasterPageFile="Root.master" %>

<asp:Content ID="cntMain" ContentPlaceHolderID="plcMain" runat="Server">
    <div class="innerContent">
        <h1>
            MVC Example</h1>
        <p>
            This is a sample for the page rendered by MVC. It displays the news list. The list is routed by using Route URL pattern <strong>/NewsMVC/List</strong> to provide extension-less URL, and then displayed with an MVC page template type using <strong>NewsMVC</strong> controller and <strong>~/Views/Global/NewsMVC/List.aspx</strong> view.
        </p>
        <div class="LightGradientBox ">
            <% foreach (var doc in NewsList)
               { %>
            <h2>
                <a href="<%=ResolveUrl(doc.RelativeURL)%>">
                    <%=doc.GetValue("DocumentName")%></a>
            </h2>
            <p>
                <%=doc.GetValue("NewsText")%></p>
            <% } %>
        </div>
    </div>
</asp:Content>