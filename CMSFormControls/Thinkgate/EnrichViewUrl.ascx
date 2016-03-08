<%@ Control Language="C#" AutoEventWireup="true" CodeFile="EnrichViewUrl.ascx.cs" Inherits="CMSFormControls_Thinkgate_EnrichViewUrl" %>

<%-- Set as a server control so it will post back and who up in view state --%>
<input type="hidden" runat="server" id="IsEnrichSubmission" value="0" />

<%-- Set as a server control so it will post back and who up in view state --%>
<input type="hidden" runat="server" id="SarasotaStudentNo" value="0" />

<%-- Simulates a user clicking the Kentico form Save button --%>
<div>
    <input <%=EnrichSubmissionDisabledAttribute %> style="height: 35px; font-size: large;<%=EnrichButtonVisibilityAttribute %>" type="button" id="Enrich" value="Create an Enrich Referral" onclick="submitEnrich();" />
</div>

<%-- What we write the return enrich url to (if successful) --%>
<div style="padding-top: 20px;font-size: large">
    <a href="<%=Value.ToString()  %>" id="EnrichViewUrl"><%=EnrichViewUrlLabelText %></a>
</div>

<%-- What we write the return enrich submit date-time to (if successful) --%>
<div style="padding-top: 10px;font-size: medium">
    <%=SubmitToEnrichDate %>
</div>

<%-- Errors returned from Enrich --%>
<div style="padding-top: 10px;font-size: medium;color: red;">
    <%=EnrichErrors %>
</div>

<script type="text/javascript" src='<%=ResolveClientUrl("~/CMSScripts/jquery/jquery.min.js")%>'></script>
<script type="text/javascript">

    var e3$ = $.noConflict();

    e3$(document).ready(function () {
        e3$('#<%=IsEnrichSubmission.ClientID%>')[0].value = 0;
        ValidateForEnrich();
    });

    // This allows us to set a value that gets posted back to our cutom control that tells us we intended to do an Enrich submission
    // Also simulates a user clicking the Kentico form Save button
    function submitEnrich() {

        e3$('#Enrich').prop('disabled', true);
        e3$('#<%=IsEnrichSubmission.ClientID%>')[0].value = 1;
        e3$('#<%=SarasotaStudentNo.ClientID%>')[0].value = e3$('.mtssStandardHeaderStudentNo')[0].innerText;

        // see if it's this one
        var kenticoSaveButton = e3$('.FormButton')[0];

        // or see if it's this one
        if (kenticoSaveButton == null)
            kenticoSaveButton = e3$('.MenuItemEdit')[0];

        // none of them, really?
        if (kenticoSaveButton == null)
            return;

        // assuming we actually found the button, click it to trigger a save operation
        kenticoSaveButton.click();
    }

    function ValidateForEnrich() {

        var validationErrors = "<%=EnrichValidationErrors%>";

        if (validationErrors === "")
            return;

        var referralDateControl = e3$('#EnrichViewUrl');
        if (referralDateControl[0].innerText === "")


        window.alert("The following Enrich Sumission Validation Error(s) ocurred:\n" + validationErrors);
    }
</script>
