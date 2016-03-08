<%@ Page Language="C#" EnableEventValidation="false" AutoEventWireup="true" CodeFile="CurriculaAddNewAssoc.cs" Inherits="CMSWebParts_ThinkgateWebParts_CurriculaAddNewAssoc" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>

    <meta name="viewport" content="width=device-width, initial-scale=1.0" />

    <script type="text/javascript" src="<%:this.ResolveUrl("~/")%>CMSScripts/Custom/jquery-1.9.1.min.js"></script>
    <script type="text/javascript" src="<%:this.ResolveUrl("~/")%>CMSScripts/Custom/jquery-migrate-1.1.0.min.js"></script>
    <script type="text/javascript" src="<%:this.ResolveUrl("~/")%>CMSScripts/Custom/jquery-core.js"></script>
    <script type="text/javascript" src="<%:this.ResolveUrl("~/")%>CMSScripts/Custom/jquery-ui-1.10.0.custom.js"></script>
    <script type="text/javascript" src="<%:this.ResolveUrl("~/")%>CMSScripts/Custom/jquery-cookie.js"></script>
    <script type="text/javascript" src="<%:this.ResolveUrl("~/")%>CMSScripts/Custom/jquery.dataTables.js"></script>
    <script type="text/javascript" src="<%:this.ResolveUrl("~/")%>CMSScripts/Custom/bootstrap/js/bootstrap.min.js"></script>
    <script type="text/javascript" src="<%:this.ResolveUrl("~/")%>CMSScripts/Custom/addNewDocument.js"></script>
    <script type="text/javascript" src="<%:this.ResolveUrl("~/")%>CMSScripts/Custom/tgDivTools.js"></script>

    <script>var $j = jQuery.noConflict();</script>

    <link href="<%:this.ResolveUrl("~/")%>CMSScripts/Custom/reset-min.css" rel="stylesheet" />
    <link href="<%:this.ResolveUrl("~/")%>CMSScripts/Custom/site_jui.ccss" rel="stylesheet" />
    <link href="<%:this.ResolveUrl("~/")%>CMSScripts/jquery/DataTables/css/demo_table_jui.css" rel="stylesheet" />    
    <link href="<%:this.ResolveUrl("~/")%>CMSScripts/jquery/jquery-ui/css/smoothness/jquery-ui-1.10.0.custom.css" rel="stylesheet" />
	<link href="<%:this.ResolveUrl("~/")%>CMSScripts/Custom/bootstrap/css/bootstrap.min.css" rel="stylesheet" />
    <link href="<%:this.ResolveUrl("~/")%>CMSWebParts/ThinkgateWebparts/css/tgwebparts.css" rel="stylesheet" />
    <link href="<%:this.ResolveUrl("~/")%>CMSWebParts/ThinkgateWebparts/css/associationToolbar.css" rel="stylesheet" />	
	
    <style type="text/css">
        select {
            width: 210px !important;
            font-size: 11px !important;
        }
		
		.ui-dialog-titlebar {font-size:140%; padding-left:.3em !important;font-family:Arial; }
    </style>
	
  
 <script type="text/javascript" src="<%:this.ResolveUrl("~/")%>CMSScripts/Custom/jquery-ui-1.10.0.custom.js"></script> 
	<script type="text/javascript">
	    function validationMessage() {
	        var isvalid = true;
	        var isExist=true;	
	        var documentType = $j('input[name=hdnDocType]').val(); 
	        var selectedItemsVal = $j('input[name=SelectedItems]').val(); 
	        if(selectedItemsVal=='undefined' || selectedItemsVal=='')
	        {
	            isExist=false;
	            isvalid=false;
	        }		
	        if(isExist==false) return isvalid;
	        if(documentType=='competencylist'&& isCompetencyExist==true && isExist )
	        {
		
	            jQuery( "#validatemessage" ).dialog({
	                modal: true,
	                autoOpen: false,
	                width:520,				
	                title: "Alert",
	                resizable: false,			
	                modal: true,			
	                buttons: {
	                    Ok: function() {
	                        jQuery( this ).dialog( "close" );
	                    }
	                }
	            });	

	            jQuery( "#validatemessage" ).dialog('open');		
	            isvalid=false;		
	        }
	        else
	        {
	            isvalid=true;
	            document.getElementById('btnAddNewStandards').click();	
	        }
	
	        return isvalid;
	
	    };
	</script>
</head>
<body>
    <div class="css_clear"></div>
    <form id="frmCurriculaAddNewAssoc" runat="server">
        <asp:ScriptManager ID="Scriptmanager1" runat="server"></asp:ScriptManager>
        <asp:HiddenField ID="SelectedItems" runat="server" />
        <asp:HiddenField ID="DocID" runat="server" />
        <asp:HiddenField ID="hdnDocType" runat="server" />

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

        <div id="tabs">
            <ul>
                <li><a href="#tabs-1">Current</a></li>
                <%
                    if (modifyAllowed)
                    {
                %>
                <li><a href="#tabs-2">Available</a></li>
                <%
                    }
                %>
            </ul>

            <div id="tabs-1" style="padding-top: -5px;">
                <div style="padding: 0px; height: 435px; width: 100%; position: relative; top: -25px;">

                    <div id="showCurriculaModal" title="" style="display: block">
                        <div id="showCurriculaModal-dialog-content"></div>
                        <br />
                        <table id="currentCurriculaDataTable" border="0" class="display" style="width: 95%"></table>
                    </div>

                    <div id="LinkButtonsDiv1" class="pull-left" style="position: relative; top: 5px;">

                        <%
                            if (modifyAllowed)
                            {
                        %>
                        <asp:LinkButton ID="btnDelNewCurricula" runat="server"  OnClick="DelSelectedItems_Click" CssClass="btn btn-success"><i class="icon-trash icon-white"></i>&nbsp;Delete Selected Curricula</asp:LinkButton>
                        <%
                        }
                        %>
                    </div>
                </div>
            </div>

            <%
                if (modifyAllowed)
                {
            %>
            <div id="tabs-2" style="padding-top: -5px;">
                <%
            }
            else
            {
                %>
                <div id="Div1" style="padding-top: -5px; display: none;">
                    <%
            }
                    %>
                    <div style="padding: 0px; height: 435px; width: 100%; position: relative; top: -5px;">

                        <div class="ui-widget" id="buttonBarDiv2" style="display: block">
                            <span style="float: left; padding-right: 1em;">
                                <label class="tgLabel1">Grade: </label> <label class="warningMessage">*</label>
                                <div id="GradeDdldiv">
                                    <select id="GradeDdl" onchange="getSubjectList();"></select>
                                </div>
                            </span>
                            <span style="float: left; padding-right: 1em;">
                                <label class="tgLabel1">Subject: </label> <label class="warningMessage">*</label>
                                <div id="SubjectDdldiv">
                                    <select id="SubjectDdl"></select>
                                </div>
                            </span>
                            <span style="float: left; padding-right: 1em;">
                                <label class="tgLabel1">Text Search:</label>
                                <div id="TextSearchTxtdiv">
                                    <input id="TextSearch" type="text" value="" style="width: 140px;" />
                                </div>
                            </span>
                            <span style="float: left; padding-right: 1em;">
                                <label class="tgLabel1">&nbsp;</label>
                                <div id="TextSearchOptionsDdldiv">
                                    <select id="TextSearchOptionsDdl" style="width: 140px;">
                                        <option value="any">Any Words</option>
                                        <option value="all">All Words</option>
                                        <option value="exact">Exact Phrase</option>
                                    </select>
                                </div>
                            </span>
                            <span style="float: left; padding-right: 1em;">
                                <label class="tgLabel1">&nbsp;&nbsp;</label>
                                <div id="SearchResourcesButton" style="padding-left: 5px; vertical-align: bottom;">
                                    <input id="btnSearchResources" type="image" src="Images/searchBT-Blue.png" onclick="getCurriculaDataTable(); return false;" />
                                </div>
                            </span>
                        </div>

                        <br />

                        <div id="showCurriculaModal2" title="" style="display: block">
                            <div id="showCurriculaModal-dialog-content2"></div>
                            <br />
                            <table id="availableCurriculaDataTable" border="0" class="display" style="width: 95%"></table>
                        </div>                       
						<div id="LinkButtonsDiv2" class="pull-left" style="position: relative; top: 5px;">
                                <a href="#" onclick="validationMessage()" id="btnAddNewStandards2" class="btn btn-success">
                                    <i class="icon-share icon-white"></i>&nbsp;Associate Selected Curricula</a>
                                <asp:LinkButton ID="btnAddNewStandards" PostBackUrl="#" ClientIDMode="Static" Style="display: none" runat="server" OnClick="AddSelectedItems_Click"></asp:LinkButton>
                       </div>
                    </div>
                </div>
            </div>
			</div>
			<div>
			
			
			<div id="validatemessage" style="display:none;width:500px; height:250px;padding:2em 3em;text-align:justify;text-justify:distribute">

			  <p>
				A curriculum association already exist. Only one curriculum association can be set for a Competency List. If you would like to change the curriculum association, please delete the existing association on the Current Tab first.
			  </p>			 
			</div></div>
    </form>
	

    <script>        
        var isCompetencyExist=false;
        function getInitialGradeList() {
            $j.ajax({
                type: "POST",
                url: "./btWebServices.aspx/getGradeList",
                data: "{}",

                contentType: "application/json; charset=utf-8",
                dataType: "json",

                error: function (XMLHttpRequest, textStatus, errorThrown) {                    
                    if (XMLHttpRequest.responseText != '') {
                        alert(XMLHttpRequest.responseText + " " + textStatus + "\n" + errorThrown);
                    }
                },
                success: function (result) {

                    $j('#GradeDdldiv')[0].innerHTML = "<select id='GradeDdl' onchange='getSubjectList();'>" + result.d + "</select>";
                    tgClearSelectOptionsFast("SubjectDdl");
                }
            });
        }

        function getCurriculaCount(docid) {
            var count = 0;
            $j.ajax({
                type: "POST",
                url: "./btWebServices.aspx/getCurriculaCount",
                data: "{'docid':'" + docid + "'}",

                contentType: "application/json; charset=utf-8",
                dataType: "json",

                error: function (XMLHttpRequest, textStatus, errorThrown) {                    
                    if (XMLHttpRequest.responseText != '') {
                        alert(XMLHttpRequest.responseText);
                    }
                    //alert(textStatus + "\n" + errorThrown);
                },
                success: function (result) {
                    count = result.d;
                    if (count > 0) {
                        isCompetencyExist=true;
                        getCurrentCurricula(docid);					                     
                        $j('#tabs').tabs("option", "active", 0);
                    } else {
                        isCompetencyExist=false;
                        updateCurrentCurriculaDataTable(null);
                        $j('#tabs').tabs("option", "active", 1);
                    }
                    updateAvailableCurriculaDataTable("");
                    getInitialGradeList();
                }
            });
            return count;
        }

        function getCurrentCurricula(docid) {

            $j.ajax({
                type: "POST",
                url: "./btWebServices.aspx/getCurrentCurricula",
                data: "{'docid':'" + docid + "'}",
                contentType: "application/json; charset=utf-8",
                dataType: "json",

                error: function (XMLHttpRequest, textStatus, errorThrown) {                    
                    if (XMLHttpRequest.responseText != '') {
                        alert(textStatus + "\n" + errorThrown);
                    }
                },
                success: function (result) {
                    //alert(result.d);
                    updateCurrentCurriculaDataTable(JSON.parse(result.d));
                }
            });
        }

        function getClientID() {
            $j.ajax({
                type: "POST",
                url: "./btWebServices.aspx/getDistrictFromParmsTable",
                data: "{}",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    //alert(textStatus + "\n" + errorThrown);
                    return null;
                },
                success: function (result) {
                    return result.d;
                }
            });

        }

        function getSubjectList() {
            var grade = $j('#GradeDdl').find(':selected').text();
            $j.ajax({
                type: "POST",
                url: "./btWebServices.aspx/getSubjectList",
                data: "{'grade':'" + grade + "'}",

                contentType: "application/json; charset=utf-8",
                dataType: "json",

                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    //alert(textStatus + "\n" + errorThrown);
                },
                success: function (result) {
                    $j('#SubjectDdldiv')[0].innerHTML = "<select id='SubjectDdl'>" + result.d + "</select>";
                }
            });
        }

        function getCurriculaDataTable() {
       
            var grade = $j('#GradeDdl').find(':selected').text();
            var subject = $j('#SubjectDdl').find(':selected').text();

            var gradeVal = $j('#GradeDdl').find(':selected').val();
            var subjectVal = $j('#SubjectDdl').find(':selected').val();

            var searchOption = $j("#TextSearchOptionsDdl").val();
            var searchText = escape($j("#TextSearch").val());
            $j('input[name=SelectedItems]').val('');

            if (grade == null || grade == "" || grade.indexOf("Select Item") != -1 || subject == null || subject == "" || subject.indexOf("Select Item") != -1 || gradeVal == 0 || subjectVal == 0 ) {
                requireCriteriaMessage();
            } else {

                $j.ajax({
                    type: "POST",
                    url: "./btWebServices.aspx/getCurriculaList",
                    data: "{'grade':'" + grade + "','gradeVal':'" + gradeVal + "', 'subject':'" + subject + "','subjectVal':'" + subjectVal + "', 'searchOption':'" + searchOption + "', 'searchText':'" + searchText + "'}",

                    contentType: "application/json; charset=utf-8",
                    dataType: "json",

                    error: function(XMLHttpRequest, textStatus, errorThrown) {
                        //alert(textStatus + "\n" + errorThrown);
                    },
                    success: function(result) {
                        var data = [];
                        if (result && result.d) {
                            data = JSON.parse(result.d);
                        }
                        updateAvailableCurriculaDataTable(data);
                    }
                });
            }
        }

        $j(document).ready(function () {
            $j("#tabs").tabs();
            updateCurrentCurriculaDataTable({});
            updateAvailableCurriculaDataTable({});
            $j(".dataTables_filter").css('display', 'none');
            //getCurriculaCount(getDocIDValue());
			
            $j("#tabs-1").click(getCurriculaCount(getDocIDValue()));
          
        });

        function getDocIDValue() {
            return $j("#DocID").val();
        }

        function updateCurrentCurriculaDataTable(jsondata) {
            try {
                if (typeof oTable1 == 'undefined') {
                    oTable1 = $j('#currentCurriculaDataTable').dataTable({
                        //"iDisplayLength": 25,
                        "bJQueryUI": true,
                        "bPaginate": false,
                        "bLengthChange": false,
                        "sScrollY": 260,
                        "aaSorting": [[1, "asc"]],
                        "aaData": jsondata,
                        "aoColumns":
                        [
                            { "sTitle": "ID", "mData": "ID", "sClass": "headerwidth", },
                            { "sTitle": "Grade", "mData": "Grade" },
                            { "sTitle": "Subject", "mData": "Subject" },
                            { "sTitle": "Course", "mData": "Course" }
                        ]
                    });
                }
                else {
                    oTable1.fnClearTable(0);

                    if (jsondata !== null) {
                        oTable1.fnAddData(jsondata);
                    }

                    oTable1.fnDraw();
                }
            } catch (e) {
                alert("ex: " + e.message);
            }

            tgMakeTableSelectable('currentCurriculaDataTable');

            return oTable1;
        }

        function updateAvailableCurriculaDataTable(jsondata) {
            try {
                if (typeof oTable2 == 'undefined') {
                    oTable2 = $j('#availableCurriculaDataTable').dataTable({
                        //"iDisplayLength": 100,
                        "bJQueryUI": true,
                        "bPaginate": false,
                        "bLengthChange": false,
                        "sScrollY": 275,
                        "aaSorting": [[1, "asc"]],
                        "aaData": jsondata,
                        "aoColumns":
                        [
                            { "sTitle": "ID", "mData": "ID", "sClass": "headerwidth" },
                            { "sTitle": "Grade", "mData": "Grade" },
                            { "sTitle": "Subject", "mData": "Subject" },
                            { "sTitle": "Course", "mData": "Course" }
                        ]
                    });

                }
                else {
                    oTable2.fnClearTable(0);

                    oTable2.fnAddData(jsondata);

                    oTable2.fnDraw();
                }
            } catch (e) {
                alert("ex: " + e.message);
            }

            tgMakeTableSelectable('availableCurriculaDataTable');

        }


    </script>
</body>
</html>
