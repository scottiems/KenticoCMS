<%@ Control Language="C#" AutoEventWireup="true" CodeFile="UpDownButton.ascx.cs" Inherits="CMSModules_Thinkgate_Controls_UpDownButton" %>

<!-- the stopOnClickPropagation() function is defined in the IP Header transformation -->
<div class="updownbtn" style="position:relative;float:right; z-index:10;">
    <div class="up">
        <asp:ImageButton ID="imgbtnUp" runat="server" 
            OnClick="imgbtnUp_Click"
			OnClientClick="stopOnClickPropagation();"
            ImageUrl="~/CMSModules/Thinkgate/Controls/Images/up_arrow.png"/>    
    </div>
    <div class="down">
        <asp:ImageButton ID="imgbtnDown" runat="server" 
            OnClick="imgbtnDown_Click"
			OnClientClick="stopOnClickPropagation();"
            ImageUrl="~/CMSModules/Thinkgate/Controls/Images/down_arrow.png" />    
    </div>    
</div>
<%--<script>
    function Pagereload()
    {
        window.parent.location.reload();
            }
</script>--%>
