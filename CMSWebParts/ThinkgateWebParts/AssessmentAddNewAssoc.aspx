<%@ Page Language="C#" EnableEventValidation="false" AutoEventWireup="true" CodeFile="AssessmentAddNewAssoc.aspx.cs" Inherits="CMSWebParts_ThinkgateWebParts_AssessmentAddNewAssoc" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
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
            width: 170px !important;
            font-size: 11px !important;
        }

        .ui-dialog-titlebar {
            font-size: 140%;
            padding-left: .3em !important;
            font-family: Arial;
        }
    </style>
</head>
<body>
    <div class="css_clear"></div>
    <form id="frmAssessmentsAddNewAssoc" runat="server">
        <asp:ScriptManager ID="Scriptmanager1" runat="server"></asp:ScriptManager>
        <asp:HiddenField ID="SelectedItems" runat="server" />
        <asp:HiddenField ID="DocID" runat="server" />

        <%
            bool createAllowed = false;
            bool modifyAllowed = false;
            int nodeID = Convert.ToInt32(this.DocumentID);

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
                <div style="padding: 0px; height: 450px; width: 100%; position: relative; top: -25px;">

                    <div id="showAssessmentsModal" title="" style="display: block">

                        <div id="showAssessmentsModal-dialog-content"></div>
                        <br />
                        <table id="currentAssessmentsDataTable" border="0" class="display" style="width: 95%"></table>
                    </div>

                    <div id="LinkButtonsDiv1" class="pull-left" style="position: relative; top: 5px;">
                        <%
                            if (modifyAllowed)
                            {
                        %>
                        <asp:LinkButton ID="btnDelAssessments" OnClick="DelSelectedItems_Click" runat="server" CssClass="btn btn-success"><i class="icon-trash icon-white"></i>&nbsp;Delete Selected Assessment</asp:LinkButton>
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
                    <div style="padding: 0px; height: 450px; width: 100%; position: relative; top: -5px;">

                        <div class="ui-widget" id="buttonBarDiv2" style="display: block">
                            <div class="ui-widget" id="Div2" style="display: block">
                                <span style="float: left; padding-right: 1em;">
                                    <label class="tgLabel1">Category: </label>
                                    <div id="CategoryDdldiv">
                                        <select id="CategoryDdl"></select>
                                    </div>
                                </span>
                                <span style="float: left; padding-right: 1em;">
                                    <label class="tgLabel1">Grade: </label><label class="warningMessage">*</label>
                                    <div id="GradeDdldiv">
                                        <select id="GradeDdl"></select>
                                    </div>
                                </span>
                                <span style="float: left; padding-right: 1em;">
                                    <label class="tgLabel1">Subject: </label><label class="warningMessage">*</label>
                                    <div id="SubjectDdldiv">
                                        <select id="SubjectDdl"></select>
                                    </div>
                                </span>
                                <span style="float: left; padding-right: 1em;">
                                    <label class="tgLabel1">Course: </label>
                                    <div id="CourseDdldiv">
                                        <select id="CourseDdl"></select>
                                    </div>
                                </span>
                                <span style="float: left; padding-right: 1em;">
                                    <label class="tgLabel1">Type: </label>
                                    <div id="TypeDdldiv">
                                        <select id="TypeDdl"></select>
                                    </div>
                                </span>
                            </div>
                            <br />
                            <br />
                            &nbsp;
                        <div class="ui-widget" id="Div3" style="display: block">
                            <span style="float: left; padding-right: 1em;">
                                <label class="tgLabel1">Term: </label>
                                <div id="TermDdldiv">
                                    <select id="TermDdl"></select>
                                </div>
                            </span>
                            <span style="float: left; padding-right: 1em;">
                                <label class="tgLabel1">Year: </label>
                                <div id="YearDdldiv">
                                    <select id="YearDdl"></select>
                                </div>
                            </span>
                            <span style="float: left; padding-right: 1em;">
                                <label class="tgLabel1">Text Search: </label>
                                <div id="TextSearchTxtdiv">
                                    <input id="TextSearch" type="text" value="" style="width: 157px;" />
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
                                <div id="SearchAssessmentsButton" style="padding-left: 8px; vertical-align: bottom;">
                                    <input id="btnSearchAssessments" type="image" src="Images/searchBT-Blue.png" onclick="getAssessmentsList(); return false;" />
                                </div>
                            </span>
                        </div>

                        </div>

                        <br />

                        <div id="showAssessmentsModal2" title="" style="display: block">
                            <div id="showAssessmentsModal-dialog-content2"></div>
                            <br />
                            <div class="dataTables_wrapper">
                                <table class="fg-toolbar ui-toolbar ui-widget-header ui-corner-tl ui-corner-tr ui-helper-clearfix" width="100%">
                                    <thead>
                                        <tr>
                                             <th id="IsAssessmentAvailable" style="display:none">
                                                <input id="chkSelectAll" type="checkbox" onclick="toggleClick(this)"> Select All Assessments </input>
                                            </th>
                                        </tr>
                                    </thead>
                                    <tr>
                                        <td>
                                            <table id="availableAssessmentsDataTable" border="0" class="display" style="width: 95%"></table>
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </div>
                        <div id="LinkButtonsDiv2" class="pull-left" style="position: relative; top: 5px;">
                            <asp:LinkButton ID="btnAddNewAssessments" runat="server" OnClick="AddSelectedItems_Click" CssClass="btn btn-success"><i class="icon-share icon-white"></i>&nbsp;Associate Selected Assessment</asp:LinkButton>
                        </div>
                    </div>

                </div>
            </div></div>
    </form>
    <script type="text/javascript">
        $j(document).ready(function () {
            $j("#tabs").tabs();
            updateCurrentAssessmentsDataTable({});
            updateAvailableAssessmentsDataTable({});
            $j(".dataTables_filter").css('display', 'none');
            $j("#tabs-1").click(getAssessmentCount(getDocIDValue()));
        });

        function getDocIDValue() {
            return $j("#DocID").val();
        }

        function updateCurrentAssessmentsDataTable(jsondata) {
            try {
                if (typeof oTable1 == 'undefined') {
                    oTable1 = $j('#currentAssessmentsDataTable').dataTable({
                        //"iDisplayLength": 25,
                        "bJQueryUI": true,
                        "bPaginate": false,
                        "bLengthChange": false,
                        "sScrollY": 260,
                        "aaSorting": [[1, "asc"]],
                        "aaData": jsondata,
                        "aoColumns":
					[
						{ "sTitle": "ID", "mData": "ID", "sClass": "headerwidth" },
                        { "sTitle": "Assessment", "mData": "AssessmentName"},
                        { "sTitle": "Description", "mData": "Description" }
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

            tgMakeTableSelectable('currentAssessmentsDataTable');

            return oTable1;
        }

        function updateAvailableAssessmentsDataTable(jsondata) {
            try {
                if (typeof oTable2 == 'undefined') {
                    oTable2 = $j('#availableAssessmentsDataTable').dataTable({
                        //"iDisplayLength": 100,
                        "bJQueryUI": true,
                        "bPaginate": false,
                        "bLengthChange": false,
                        "sScrollY": 200,
                        "aaSorting": [[1, "asc"]],
                        "aaData": jsondata,
                        "aoColumns":
					[
						{ "sTitle": "ID", "mData": "TestID", "sClass": "headerwidth" },
                        { "sTitle": "Assessment", "mData": "TestName" },
                        { "sTitle": "Description", "mData": "Description" }
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

            tgMakeTableSelectable('availableAssessmentsDataTable');

        }

        function getCategoryList() {
            $j.ajax({
                type: "POST",
                url: "./btWebServices.aspx/getAssessmentCategoryList",
                data: "{}",

                contentType: "application/json; charset=utf-8",
                dataType: "json",

                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    //alert(XMLHttpRequest.responseText + " " + textStatus + "\n" + errorThrown);
                },
                success: function (result) {
                    $j('#CategoryDdldiv')[0].innerHTML = "<select id='CategoryDdl' onchange='getTypeList();'>" + result.d + "</select>";
                    tgClearSelectOptionsFast("TypeDdl");
                }
            });
        }

        function getGradeList() {
            $j.ajax({
                type: "POST",
                url: "./btWebServices.aspx/getGradeList",
                data: "{}",

                contentType: "application/json; charset=utf-8",
                dataType: "json",

                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    //alert(XMLHttpRequest.responseText + " " + textStatus + "\n" + errorThrown);
                },
                success: function (result) {
                    $j('#GradeDdldiv')[0].innerHTML = "<select id='GradeDdl' onchange='getSubjectList();'>" + result.d + "</select>";
                    tgClearSelectOptionsFast("SubjectDdl");
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
                    $j('#SubjectDdldiv')[0].innerHTML = "<select id='SubjectDdl' onchange='getCourseList();'>" + result.d + "</select>";
                    tgClearSelectOptionsFast("CourseDdl");
                }
            });
        }

        function getCourseList() {
            var grade = $j('#GradeDdl').find(':selected').text();
            var subject = $j('#SubjectDdl').find(':selected').text();
            $j.ajax({
                type: "POST",
                url: "./btWebServices.aspx/getCourseList",
                data: "{'grade':'" + grade + "', 'subject':'" + subject + "'}",

                contentType: "application/json; charset=utf-8",
                dataType: "json",

                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    //alert(textStatus + "\n" + errorThrown);
                },
                success: function (result) {
                    $j('#CourseDdldiv')[0].innerHTML = "<select id='CourseDdl' >" + result.d + "</select>";
                }
            });
        }

        function getTypeList() {
            var category = $j('#CategoryDdl').find(':selected').text();
            $j.ajax({
                type: "POST",
                url: "./btWebServices.aspx/getTypeList",
                data: "{'category':'" + category + "'}",

                contentType: "application/json; charset=utf-8",
                dataType: "json",

                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    //alert(textStatus + "\n" + errorThrown);
                },
                success: function (result) {
                    $j('#TypeDdldiv')[0].innerHTML = "<select id='TypeDdl'>" + result.d + "</select>";
                }
            });
        }

        function getTermList() {
            $j.ajax({
                type: "POST",
                url: "./btWebServices.aspx/getTermList",
                data: "{}",

                contentType: "application/json; charset=utf-8",
                dataType: "json",

                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    //alert(textStatus + "\n" + errorThrown);
                },
                success: function (result) {
                    $j('#TermDdldiv')[0].innerHTML = "<select id='TermDdl'>" + result.d + "</select>";
                }
            });
        }

        function getYearList() {
            $j.ajax({
                type: "POST",
                url: "./btWebServices.aspx/getYearList",
                data: "{}",

                contentType: "application/json; charset=utf-8",
                dataType: "json",

                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    //alert(textStatus + "\n" + errorThrown);
                },
                success: function (result) {
                    $j('#YearDdldiv')[0].innerHTML = "<select id='YearDdl'>" + result.d + "</select>";
                }
            });
        }

        function toggleClick(ctrl) {
            tgSelectAll('availableAssessmentsDataTable');
        }

        function getAssessmentCount(docid) {
            var count = 0;

            $j.ajax({
                type: "POST",
                url: "./btWebServices.aspx/getCurrentAssessments",
                data: "{'docid':'" + docid + "'}",

                contentType: "application/json; charset=utf-8",
                dataType: "json",

                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    //alert(textStatus + "\n" + errorThrown);
                },
                success: function (result) {
                    if (result.d != null) {
                        count = JSON.parse(result.d).length;
                    }
                    if (count > 0) {
                        updateCurrentAssessmentsDataTable(JSON.parse(result.d));
                        $j('#tabs').tabs("option", "active", 0);
                    } else {
                        updateCurrentAssessmentsDataTable(null);
                        $j('#tabs').tabs("option", "active", 1);
                    }
                    updateAvailableAssessmentsDataTable("");
                    getCategoryList();
                    getGradeList();
                    getTermList();
                    getYearList();
                }
            });
            return count;
        }

        function getAssessmentsList() {
            var category = $j('#CategoryDdl').find(':selected').text();
            var categoryAll = "";
            var categoryDdl = document.getElementById('CategoryDdl');
            for (i = 0; i < categoryDdl.options.length; i++) {
                categoryAll += categoryDdl.options[i].text + "|";
            }
            var grade = $j('#GradeDdl').find(':selected').text();
            var subject = $j('#SubjectDdl').find(':selected').text();
            var course = $j('#CourseDdl').find(':selected').text();
            var type = $j('#TypeDdl').find(':selected').text();
            var term = $j('#TermDdl').find(':selected').text();
            var year = $j('#YearDdl').find(':selected').text();
            var categoryVal = $j('#CategoryDdl').find(':selected').val();
            var gradeVal = $j('#GradeDdl').find(':selected').val();
            var subjectVal = $j('#SubjectDdl').find(':selected').val();
            var courseVal = $j('#CourseDdl').find(':selected').val();
            var typeVal = $j('#TypeDdl').find(':selected').val();
            var termVal = $j('#TermDdl').find(':selected').val();
            var searchOption = $j("#TextSearchOptionsDdl").val();
            var searchText = escape($j("#TextSearch").val());


            if (grade == null || grade == "" || grade.indexOf("Select Item") != -1 || subject == null || subject == "" || subject.indexOf("Select Item") != -1) {
                requireCriteriaMessage();
            } else {


                $j.ajax({
                    type: "POST",
                    url: "./btWebServices.aspx/getAssessmentsList",
                    data: "{'category':'" + category + "', 'categoryVal':'" + categoryVal + "','categoryAll':'" + categoryAll + "', 'grade':'" + grade + "', 'gradeVal':'" + gradeVal + "', 'subject':'" + subject + "', 'subjectVal':'" + subjectVal + "', 'course':'" + course + "', 'courseVal':'" + courseVal + "', 'type':'" + type + "', 'typeVal':'" + typeVal + "', 'term':'" + term + "', 'termVal':'" + termVal + "','year':'" + year + "','searchOption':'" + searchOption + "', 'searchText':'" + searchText + "'}",

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
                        if (data.length > 0) {
                            $j("#IsAssessmentAvailable").show();
                        }
                        else {
                            $j("#IsAssessmentAvailable").hide();
                        }
                        
                        updateAvailableAssessmentsDataTable(data);
                    }
                });
            }
        }
    </script>
</body>
</html>
