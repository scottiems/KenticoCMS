<%@ Page Language="C#" EnableEventValidation="false" AutoEventWireup="true" CodeFile="StdAddNewAssoc.aspx.cs" Inherits="CMSWebParts_ThinkgateWebParts_StdAddNewAssoc" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>

    <meta name="viewport" content="width=device-width, initial-scale=1.0" />

    <script type="text/javascript" src="<%:this.ResolveUrl("~/")%>CMSScripts/Custom/jquery-1.9.1.min.js"></script>
    <script type="text/javascript" src="<%:this.ResolveUrl("~/")%>/CMSScripts/Custom/jquery-migrate-1.1.0.min.js"></script>
    <script type="text/javascript" src="<%:this.ResolveUrl("~/")%>/CMSScripts/Custom/jquery-core.js"></script>
    <script type="text/javascript" src="<%:this.ResolveUrl("~/")%>/CMSScripts/Custom/jquery-ui-1.10.0.custom.js"></script>
    <script type="text/javascript" src="<%:this.ResolveUrl("~/")%>/CMSScripts/Custom/jquery-cookie.js"></script>
    <script type="text/javascript" src="<%:this.ResolveUrl("~/")%>/CMSScripts/Custom/jquery.dataTables.js"></script>
    <script type="text/javascript" src="<%:this.ResolveUrl("~/")%>/CMSScripts/Custom/bootstrap/js/bootstrap.min.js"></script>
    <script type="text/javascript" src="<%:this.ResolveUrl("~/")%>/CMSScripts/Custom/addNewDocument.js"></script>
    <script type="text/javascript" src="<%:this.ResolveUrl("~/")%>/CMSScripts/Custom/tgDivTools.js"></script>
    <script type="text/javascript" src="<%:this.ResolveUrl("~/")%>/CMSScripts/jquery/DataTables/js/dataTableNaturalSort.js"></script>
    <script>var $j = jQuery.noConflict();</script>

    <link href="<%:this.ResolveUrl("~/")%>/CMSScripts/Custom/reset-min.css" rel="stylesheet" />
    <link href="<%:this.ResolveUrl("~/")%>/CMSScripts/Custom/site_jui.ccss" rel="stylesheet" />
    <link href="<%:this.ResolveUrl("~/")%>/CMSScripts/jquery/DataTables/css/demo_table_jui.css" rel="stylesheet" />
    <link href="<%:this.ResolveUrl("~/")%>/CMSScripts/jquery/jquery-ui/css/smoothness/jquery-ui-1.10.0.custom.css" rel="stylesheet" />
    <link href="<%:this.ResolveUrl("~/")%>/CMSScripts/Custom/bootstrap/css/bootstrap.min.css" rel="stylesheet" />
    <link href="<%:this.ResolveUrl("~/")%>/CMSWebParts/ThinkgateWebparts/css/tgwebparts.css" rel="stylesheet" />
    <link href="<%:this.ResolveUrl("~/")%>/CMSWebParts/ThinkgateWebparts/css/associationToolbar.css" rel="stylesheet" />
    <style type="text/css">
        select {
            width: 180px !important;
            font-size: 11px !important;
        }
    </style>
</head>
<body>
    <div class="css_clear"></div>
    <form id="frmStandardsAddNewAssoc" runat="server">
        <asp:ScriptManager ID="Scriptmanager1" runat="server"></asp:ScriptManager>
        <asp:HiddenField ID="SelectedItems" runat="server" />
        <asp:HiddenField ID="DocID" runat="server" />

        <%
            bool createAllowed = false;
            bool modifyAllowed = false;
        %>
        <% if (this.FromLRMI == false)
           {
               int nodeID = Convert.ToInt32(this.DocumentID);

               if (nodeID > 0)
               {
                   CMS.DocumentEngine.TreeNode treenode = TreeHelper.SelectSingleNode(nodeID);

                   createAllowed = treenode.CheckPermissions(CMS.SettingsProvider.PermissionsEnum.Create, CMSContext.CurrentSiteName, CMSContext.CurrentUser);
                   modifyAllowed = treenode.CheckPermissions(CMS.SettingsProvider.PermissionsEnum.Modify, CMSContext.CurrentSiteName, CMSContext.CurrentUser);
               }
           }
        %>

        <div id="tabs">
            <ul>
                <li><a href="#tabs-1">Current</a></li>
                <%
                    if (modifyAllowed || this.FromLRMI == true)
                    {
                %>
                <li><a href="#tabs-2">Available</a></li>
                <%
                    }
                %>
            </ul>

            <div id="tabs-1" style="padding-top: -5px;">
                <div style="padding: 0px; height: 450px; width: 100%; position: relative; top: -25px;">

                    <div id="showStandardsModal" title="" style="display: block">

                        <div id="showStandardsModal-dialog-content"></div>
                        <br />
                        <table id="currentStandardsDataTable" border="0" class="display" style="width: 95%"></table>
                    </div>

                    <div id="LinkButtonsDiv1" class="pull-left" style="position: relative; top: 5px;">
                        <%
                            if (modifyAllowed || this.FromLRMI == true)
                            {
                        %>
                        <asp:LinkButton ID="btnDelNewStandards" runat="server" OnClick="DelSelectedItems_Click" CssClass="btn btn-success"><i class="icon-trash icon-white"></i>&nbsp;Delete Selected Standard</asp:LinkButton>
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
                <div id="tabs-2" style="padding-top: -5px; display: none;">
                    <%
                }
                    %>
                    <div style="padding: 0px; height: 450px; width: 100%; position: relative; top: -5px;">

                        <div class="ui-widget" id="buttonBarDiv2" style="display: block">
                            <div class="ui-widget" id="Div2" style="display: block">
                                <span style="float: left; padding-right: 1em;">
                                    <label class="tgLabel1">Standard: </label>
                                    <label class="warningMessage">*</label>
                                    <div id="StandardSetDdldiv">
                                        <select id="StandardSetDdl" onchange="getStandardSetList();"></select>
                                    </div>
                                </span>
                                <span style="float: left; padding-right: 1em;">
                                    <label class="tgLabel1">Grade: </label>
                                    <label class="warningMessage">*</label>
                                    <div id="GradeDdldiv">
                                        <select id="GradeDdl" onchange="getSubjectList();"></select>
                                    </div>
                                </span>
                                <span style="float: left; padding-right: 1em;">
                                    <label class="tgLabel1">Subject: </label>
                                    <label class="warningMessage">*</label>
                                    <div id="SubjectDdldiv">
                                        <select id="SubjectDdl" onchange="getCourseList();"></select>
                                    </div>
                                </span>
                                <span style="float: left; padding-right: 1em;">
                                    <label class="tgLabel1">Course: </label>
                                    <div id="CourseDdldiv">
                                        <select id="CourseDdl"></select>
                                    </div>
                                </span>

                            </div>
                            <br />
                            <br />
                            &nbsp;
                        <div class="ui-widget" id="Div1" style="display: block">
                            <span style="float: left; padding-right: 1em;">
                                <label class="tgLabel1">Text Search: </label>
                                <div id="TextSearchTxtdiv">
                                    <input id="TextSearch" type="text" value="" style="width: 165px;" />
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
                                <div id="SearchResourcesButton" style="padding-left: 8px; vertical-align: bottom;">
                                    <input id="btnSearchResources" type="image" src="Images/searchBT-Blue.png" onclick="getStandardsDataTable(); return false;" />
                                </div>
                            </span>
                        </div>

                        </div>

                        <br />

                        <div id="showStandardsModal2" title="" style="display: block">
                            <div id="showStandardsModal-dialog-content2"></div>
                            <br />
                            <div class="dataTables_wrapper">
                                <table class="fg-toolbar ui-toolbar ui-widget-header ui-corner-tl ui-corner-tr ui-helper-clearfix" width="100%">
                                    <thead>
                                        <tr>
                                            <th id="IsStandardsAvailable" style="display:none">
                                                <input id="chkSelectAll" type="checkbox" onclick="toggleClick(this)"> Select All Standards </input>
                                            </th>
                                        </tr>
                                    </thead>
                                    <tr>
                                        <td>
                                            <table id="availableStandardsDataTable" border="0" class="display" style="width: 95%"></table>
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </div>
                        <div id="LinkButtonsDiv2" class="pull-left" style="position: relative; top: 5px;">
                            <asp:LinkButton ID="btnAddNewStandards" runat="server" OnClick="AddSelectedItems_Click" CssClass="btn btn-success"><i class="icon-share icon-white"></i>&nbsp;Associate Selected Standard</asp:LinkButton>
                            <asp:LinkButton ID="btnAddNewStandardsFromLRMI" runat="server" OnClientClick="return attachStandardsToTagEA();return false;" CssClass="btn btn-success" Visible="false"><i class="icon-share icon-white"></i>&nbsp;Associate Selected Standard</asp:LinkButton>
                        </div>
                    </div>

                </div>
            </div>
        </div>
    </form>

    <script>
        /* Select stanadrd to tag's educational alignments in tag's UI. This needs to be saved into EA as pipe separated values.
           It is different from association. Also, selected standards must be associated only when tag is saved, not before that.
        */
        function attachStandardsToTagEA() {
            var fromLRMI = '<%=this.FromLRMI %>';
            var selectedItems = $j('input[name=SelectedItems]').val();
            if (fromLRMI == "True") {
                var ids = '', names = '';
                $j(".row_selected").each(function (i, e) {
                    if (e.cells[0].childNodes.length > 0 && e.cells[1].childNodes.length > 0) {
                        ids = ids + e.cells[0].childNodes[0].nodeValue + '|';
                        names = names + e.cells[1].childNodes[0].nodeValue + '<BR/>';
                    }
                });
                parent.closeModalDialog(ids, names);
                window.parent.CloseDialog();
                return false;
            }
            else {
                $j.ajax({
                    type: "POST",
                    url: "./btWebServices.aspx/AddSelectedItems",
                    data: "{'docid':'" + getDocIDValue() + "', 'SelectedItems':'" + selectedItems + "'}",

                    contentType: "application/json; charset=utf-8",
                    dataType: "json",

                    error: function (XMLHttpRequest, textStatus, errorThrown) {
                        //alert(textStatus + "\n" + errorThrown);
                    },
                    success: function (result) {
                        getStandardCount(getDocIDValue());
                    }
                });
                return true;
            }
        }

        function getStandardsList() {
            $j.ajax({
                type: "POST",
                url: "./btWebServices.aspx/getStandardSetList",
                data: "{}",

                contentType: "application/json; charset=utf-8",
                dataType: "json",

                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    //alert(textStatus + "\n" + errorThrown);
                },
                success: function (result) {
                    $j('#StandardSetDdldiv')[0].innerHTML = "<select id='StandardSetDdl' onchange='getStandardSetList();'>" + result.d + "</select>";
                    tgClearSelectOptionsFast("GradeDdl");
                    tgClearSelectOptionsFast("SubjectDdl");
                    tgClearSelectOptionsFast("CourseDdl");

                }
            });
        }

        function getStandardCount(docid) {
            var count = 0;

            $j.ajax({
                type: "POST",
                url: "./btWebServices.aspx/getStandardCount",
                data: "{'docid':'" + docid + "'}",

                contentType: "application/json; charset=utf-8",
                dataType: "json",

                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    //alert(textStatus + "\n" + errorThrown);
                },
                success: function (result) {
                    count = result.d;
                    if (count > 0) {
                        getCurrentStandards(docid);
                        $j('#tabs').tabs("option", "active", 0);
                    } else {
                        updateCurrentStandardsDataTable(null);
                        $j('#tabs').tabs("option", "active", 1);
                    }
                    updateAvailableStandardsDataTable("");
                    getStandardsList();
                }
            });
            return count;
        }

        function getCurrentStandards(docid) {

            $j.ajax({
                type: "POST",
                url: "./btWebServices.aspx/getCurrentStandards",
                data: "{'docid':'" + docid + "'}",
                contentType: "application/json; charset=utf-8",
                dataType: "json",

                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    //alert(textStatus + "\n" + errorThrown);
                },
                success: function (result) {
                    updateCurrentStandardsDataTable(JSON.parse(result.d));
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

        function getStandardSetList() {
            var stdset = $j('#StandardSetDdl').find(':selected').text();
            $j.ajax({
                type: "POST",
                url: "./btWebServices.aspx/getStandardSetGrade",
                data: "{'standardSet':'" + stdset + "'}",

                contentType: "application/json; charset=utf-8",
                dataType: "json",

                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    //alert(textStatus + "\n" + errorThrown);
                },
                success: function (result) {
                    $j('#GradeDdldiv')[0].innerHTML = "<select id='GradeDdl' onchange='getSubjectList();'>" + result.d + "</select>";
                    tgClearSelectOptionsFast("SubjectDdl");
                    tgClearSelectOptionsFast("CourseDdl");
                }
            });
        }

        function getSubjectList() {
            var stdset = $j('#StandardSetDdl').find(':selected').text();
            var grade = $j('#GradeDdl').find(':selected').text();
            $j.ajax({
                type: "POST",
                url: "./btWebServices.aspx/getStandardSetGradeSubject",
                data: "{'standardSet':'" + stdset + "', 'grade':'" + grade + "'}",

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
            var stdset = $j('#StandardSetDdl').find(':selected').text();
            var grade = $j('#GradeDdl').find(':selected').text();
            var subject = $j('#SubjectDdl').find(':selected').text();
            $j.ajax({
                type: "POST",
                url: "./btWebServices.aspx/getStandardSetGradeSubjectCourse",
                data: "{'standardSet':'" + stdset + "', 'grade':'" + grade + "', 'subject':'" + subject + "'}",

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

        function getStandardsDataTable() {           
            var stdset = $j('#StandardSetDdl').find(':selected').text();
            var grade = $j('#GradeDdl').find(':selected').text();
            var subject = $j('#SubjectDdl').find(':selected').text();
            var course = $j('#CourseDdl').find(':selected').text();
            var stdsetVal = $j('#StandardSetDdl').find(':selected').val();
            var gradeVal = $j('#GradeDdl').find(':selected').val();
            var subjectVal = $j('#SubjectDdl').find(':selected').val();
            var courseVal = $j('#CourseDdl').find(':selected').val();
            var searchOption = $j("#TextSearchOptionsDdl").val();
            var searchText = escape($j("#TextSearch").val());
            if (stdset == null || stdset == "" || stdset.indexOf("Select Item") != -1 || grade == null || grade == "" || grade.indexOf("Select Item") != -1 || subject == null || subject == "" || subject.indexOf("Select Item") != -1) {
                requireCriteriaMessage();
            } else {

                $j.ajax({
                    type: "POST",
                    url: "./btWebServices.aspx/getStandardsList",
                    data: "{'standardSet':'" + stdset + "', 'standardSetVal':'" + stdsetVal + "', 'grade':'" + grade + "', 'gradeVal':'" + gradeVal + "', 'subject':'" + subject + "', 'subjectVal':'" + subjectVal + "', 'course':'" + course + "', 'courseVal':'" + courseVal + "', 'searchOption':'" + searchOption + "', 'searchText':'" + searchText + "'}",

                    contentType: "application/json; charset=utf-8",
                    dataType: "json",

                    error: function (XMLHttpRequest, textStatus, errorThrown) {
                        //alert(textStatus + "\n" + errorThrown);
                    },
                    success: function (result) {
                        var data = [];
                        if (result && result.d) {
                            data = JSON.parse(result.d);
                        }
                        if (data.length > 0) {
                            $j("#IsStandardsAvailable").show();
                        } else { $j("#IsAssessmentAvailable").hide(); }
                        updateAvailableStandardsDataTable(data);
                    }
                });
            }
        }

        $j(document).ready(function () {
            $j("#tabs").tabs();
            updateCurrentStandardsDataTable({});
            updateAvailableStandardsDataTable({});

            getStandardCount(getDocIDValue());

            $j("#tabs-1").click(getStandardCount(getDocIDValue()));

            setLRMITagStandardAssocUI();
            $j(".dataTables_filter").css('display', 'none');
        });

        function setLRMITagStandardAssocUI() {
            var fromLRMI = '<%=this.FromLRMI %>';
	        if (fromLRMI == "True") {
	            $j(".ui-tabs-nav").hide();  // hide both tabs
	            $j("#tabs-1").hide();   // hide first DIV which contains available standards
	            $j("#tabs-2").show();   // show only standard selection DIV
	            getStandardsList(); // populate standard set dropdown. A cascading population will occur then.
	            $j(".dataTables_scrollBody").css("height", "170px");
	            //$j("select").css("width", "190px");
	        }
	    }

	    function getDocIDValue() {
	        return $j("#DocID").val();
	    }


	    function updateCurrentStandardsDataTable(jsondata) {
	        try {
	            if (typeof oTable1 == 'undefined') {
	                oTable1 = $j('#currentStandardsDataTable').dataTable({
	                    //"iDisplayLength": 25,
	                    "bJQueryUI": true,
	                    "bPaginate": false,
	                    "bLengthChange": false,
	                    "sScrollY": 260,
	                    "aaSorting": [],
	                    "aaData": jsondata,
	                    "aoColumns":
					[
						{ "sTitle": "ID", "mData": "ID", "sClass": "headerwidth" },
						{ "sTitle": "Standard", "mData": "StandardName" },
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

	        tgMakeTableSelectable('currentStandardsDataTable');
	        return oTable1;
	    }

	    function updateAvailableStandardsDataTable(jsondata) {
	        try {
	            if (typeof oTable2 == 'undefined') {
	                oTable2 = $j('#availableStandardsDataTable').dataTable({
	                    //"iDisplayLength": 100,
	                    "bJQueryUI": true,
	                    "bPaginate": false,
	                    "bLengthChange": false,
	                    "sScrollY": 200,
	                    "aaSorting": [],
	                    "aaData": jsondata,
	                    "aoColumns":
					[
						//ID, Standard_Set, Grade, Subject, Course, Level, StandardName, \"Desc\" as Description                        
						{ "sTitle": "ID", "mData": "ID", "sClass": "headerwidth" },
						{ "sTitle": "Standard", "mData": "StandardName" },
						{ "sTitle": "Description", "mData": "Description" }
					]
	                });

	            }
	            else {
	                oTable2.fnClearTable(0);

	               // oTable2.fnAddData(jsondata);

	                if (jsondata !== null) {
	                    oTable2.fnAddData(jsondata);
	                }


	                oTable2.fnDraw();
	            }
	        } catch (e) {
	            alert("ex: " + e.message);
	        }

	        tgMakeTableSelectable('availableStandardsDataTable');

	    }

	    function toggleClick(ctrl) {           
	        tgSelectAll('availableStandardsDataTable');
	    }


    </script>
</body>
</html>
