<%@ Page Language="C#" EnableEventValidation="false" AutoEventWireup="true" CodeFile="ResAddNewAssoc.aspx.cs" Inherits="CMSWebParts_ThinkgateWebParts_ResAddNewAssoc" %>

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

    <link href="<%:this.ResolveUrl("~/")%>CMSScripts/jquery/DataTables/css/demo_table_jui.css" rel="stylesheet" />
    <link href="<%:this.ResolveUrl("~/")%>CMSScripts/jquery/jquery-ui/css/smoothness/jquery-ui-1.10.0.custom.css" rel="stylesheet" />
    <link href="<%:this.ResolveUrl("~/")%>CMSScripts/Custom/bootstrap/css/bootstrap.min.css" rel="stylesheet" />
    <link href="<%:this.ResolveUrl("~/")%>CMSWebParts/ThinkgateWebparts/css/tgwebparts.css" rel="stylesheet" />
    <link href="<%:this.ResolveUrl("~/")%>CMSWebParts/ThinkgateWebparts/css/associationToolbar.css" rel="stylesheet" />

    <style type="text/css">
        select {
            width: 180px !important;
            font-size:11px !important;
        }
    </style>

</head>
<body>
    <div class="css_clear"></div>
    <form id="frmResourcesAddNewAssoc" runat="server">
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
                <div style="padding: 0px; height: 435px; width: 100%; position: relative; top: -25px;">

                    <div id="showResourcesModal" title="" style="display: block">
                        <div id="showResourcesModal-dialog-content"></div>
                        <br />
                        <table id="currentResourcesDataTable" border="0" class="display" style="width: 95%"></table>
                    </div>

                    <div id="LinkButtonsDiv1" class="pull-left" style="position: relative; top: 5px;">
                        <%
                            if (modifyAllowed)
                            {
                        %>
                        <asp:LinkButton ID="btnDelNewResources" runat="server" OnClick="DelSelectedItems_Click" CssClass="btn btn-success"><i class="icon-trash icon-white"></i>&nbsp;Delete Selected Resource</asp:LinkButton>
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
                    <div style="padding: 0px; height: 435px; width: 100%; position: relative; top: -5px;">

                        <div class="ui-widget" id="buttonBarDiv2" style="display: block;">

                            <span style="float: left; padding-right: 1em;">
                               <%-- <label class="tgLabel1">Category: </label>
                                <div id="CategoryDdldiv">
                                    <select id="CategoryDdl" style="width: 150px;"></select>
                                </div>--%>
                            </span>
                            <span style="float: left; padding-right: 1em;">
                                <label class="tgLabel1"><b>Type:</b> </label>
                                <div id="ResourceTypeDdldiv">
                                    <select id="ResourceTypeDdl"  onchange="getSelectedResourceSubTypesList();" style="width: 220px;"></select>
                                </div>
                            </span>
                            <span style="float: left; padding-right: 1em;">
                                <label class="tgLabel1"><b>Subtype: </b></label>
                                <div id="ResourceSubTypeDdldiv">
                                    <select id="ResourceSubTypeDdl" style="width: 220px;"></select>
                                </div>
                            </span>
                            <span style="float: left; padding-right: 1em;">
                              <%--  <label class="tgLabel1">Resource Name: </label>--%>
                                <div id="ResourceNameTxtdiv">
                                    <%--<input id="ResourceName" type="text" value="" style="width: 120px;" />--%>
                                </div>
                            </span>
                            <span style="float: left; padding-right: 1em;">
                                <label class="tgLabel1"><b>Text Search:</b> </label>
                                <div id="TextSearchTxtdiv">
                                    <input id="TextSearch" type="text" value="" style="width: 120px;" />
                                </div>
                            </span>
                            <span style="float: left; padding-right: 1em;">
                                <label class="tgLabel1">&nbsp;</label>
                                <div id="TextSearchOptionsDdldiv">
                                    <select id="TextSearchOptionsDdl" style="width: 120px;">
                                        <option value="any">Any Words</option>
                                        <option value="all">All Words</option>
                                        <option value="exact">Exact Phrase</option>
                                        <option value="createdBy">Author</option>
                                    </select>
                                </div>
                            </span>

                             <span style="float: left; padding-right: 1em;">
                                <label class="tgLabel1"><b>Rating: </b></label>
                                <div id="RatingRange">
                                    <select id="ddlRating" style="width: 80px; font-size: 9px;" ></select>
                                </div>
                            </span>
                      
                        </div>

                        <br /><br /><br />
                        &nbsp;
                        <div class="ui-widget" id="Div1" style="display: block">
                            <span style="float: left; padding-right: 1em;">
                                <label class="tgLabel1"><b>Grade:</b> </label>
                                <div id="GradeDdldiv">
                                    <select id="GradeDdl" style="width: 120px; font-size: 9px;" onchange="getInitialGradeList();"></select>
                                </div>
                            </span>
                            <span style="float: left; padding-right: 1em;">
                                <label class="tgLabel1"><b>Subject:</b> </label>
                                <div id="SubjectDdldiv">
                                    <select id="SubjectDdl" style="width: 150px; font-size: 9px;" onchange="getSubjectList();"></select>
                                </div>
                            </span>
                            <span style="float: left; padding-right: 1em;">
                                <label class="tgLabel1"><b>Course: </b></label>
                                <div id="CourseDdldiv">
                                    <select id="CourseDdl" style="width: 150px; font-size: 9px;" ></select>
                                </div>
                            </span>
                           <span style="float: left; padding-right: 1em;">
                                <label class="tgLabel1">&nbsp;&nbsp;</label>
                                <div id="SearchResourcesButton" style="padding-left: 110px;vertical-align:bottom;">
                                    <input id="btnSearchResources" type="image" src="Images/searchBT-Blue.png" onclick="getResourcesList(); return false;" />
                                </div>
                            </span>
                        </div>
                        <br />
                        <div id="showResourcesModal2" title="" style="display: block">
                           <div id="showResourcesModal-dialog-content2"></div>
                           <br />
                            <table id="availableResourcesDataTable" border="0" class="display" style="width: 95%;"></table>
                        </div>
                        <div id="LinkButtonsDiv2" class="pull-left" style="position: relative; top: 5px;">
                            <asp:LinkButton ID="btnAddNewResources" runat="server" OnClick="AddSelectedItems_Click" CssClass="btn btn-success"><i class="icon-share icon-white"></i>&nbsp;Associate Selected Resource</asp:LinkButton>
                        </div>
                    </div>
                </div>
            </div></div>
    </form>

    <script>

        $j(document).ready(function () {
            $j("#tabs").tabs();
            updateCurrentResourcesDataTable({});
            updateAvailableResourcesDataTable({});

            getResourceCount(getDocIDValue());
            getCategoryList();
            getResourceTypesList();
            getResourceSubTypesList('All');
            getRatingRange();

            //  $j("#tabs-1").click(getResourceCount(getDocIDValue()));
            overrideBootstrapCSS();
            $j(".dataTables_filter").css('display', 'none');
        });
        function addResources() {
            var selectedItems = $j('input[name=SelectedItems]').val();
            var docID = getDocIDValue();  //Corey Creech added this line on 1/16/2014
            $j.ajax({
                type: "POST",
                url: "./btWebServices.aspx/AddSelectedResources",
                data: "{'docId':'" + docID + "', 'selectedItems':'" + selectedItems + "'}",

                contentType: "application/json; charset=utf-8",
                dataType: "json",

                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    //alert(textStatus + "\n" + errorThrown);
                },
                success: function (result) {
                    getResourceCount(docID);
                }
            });
            return true;
        }

        function getCurrentResources(docid) {
            $j.ajax({
                type: "POST",
                url: "./btWebServices.aspx/getCurrentResources",
                data: "{'docId':'" + docid + "'}",
                contentType: "application/json; charset=utf-8",
                dataType: "json",

                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    //alert(textStatus + "\n" + errorThrown);
                },
                success: function (result) {
                    var data = JSON.parse(result.d) || [];
                    updateCurrentResourcesDataTable(data);
                }
            });
        }

        function getResourcesList() {
           //var category = $j("#CategoryDdl").val();
            var category = "";
            var type = $j("#ResourceTypeDdl").val();
            var subtype = $j("#ResourceSubTypeDdl").val();
            //var name = escape($j("#ResourceName").val());
            var name = "";
            var searchOption = $j("#TextSearchOptionsDdl").val();
            var searchText = escape($j("#TextSearch").val());
            var grade = $j("#GradeDdl").val();
            var standardSet = $j("#SubjectDdl").val();
            var course = $j("#CourseDdl").val();
            var ratingRange = $j("#ddlRating").val();
            searchText = searchText.replace(/'/g, "\\'");

                $j.ajax({
                    type: "POST",
                    url: "./btWebServices.aspx/searchResources",
                    data: "{ 'category' : '" + category + "', 'type' : '" + type + "', 'subtype' : '" + subtype + "', 'name' : '" + name + "', 'searchOption' : '" + searchOption + "', 'searchText' : '" + searchText + "' , 'grade' : '" + grade + "' , 'standardSet' : '" + standardSet + "' , 'course' : '" + course + "', 'ratingRange': '" + ratingRange + "' }",

                    contentType: "application/json; charset=utf-8",
                    dataType: "json",

                    error: function(XMLHttpRequest, textStatus, errorThrown) {
                        /*
                            Internal Server Error will also be thrown if size of result JSON increases beyond the default maxJsonLength setting for JavaScriptSerializer.
                            Configure this value in web.config to increase it.
                        */
                    },
                    success: function(result) {
                        var data = [];
                        if (result && result.d) {
                            data = JSON.parse(result.d);
                        }
                        updateAvailableResourcesDataTable(data);
                    }
                });
            
        }


        function getCategoryList() {
            $j.ajax({
                type: "POST",
                url: "./btWebServices.aspx/getCategoryList",
                data: "{}",

                contentType: "application/json; charset=utf-8",
                dataType: "json",

                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    //alert(textStatus + "\n" + errorThrown);
                },
                success: function (result) {
                    /* $j('#CategoryDdldiv')[0].innerHTML = "<select id='CategoryDdl' onchange='getResourceTypesList();'>" + result.d + "</select>";
                    tgClearSelectOptionsFast("ResourceTypeDdl");
                    tgClearSelectOptionsFast("ResourceSubTypeDdl");
                    */

                    // $j('#CategoryDdldiv')[0].innerHTML = "<select id='CategoryDdl'>" + result.d + "</select>";
                    //  overrideBootstrapCSS();
                }
            });
        }

        function getRatingRange() {
            $j.ajax({
                type: "POST",
                url: "./btWebServices.aspx/getRatingRange",
                data: "{}",

                contentType: "application/json; charset=utf-8",
                dataType: "json",

                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    //alert(textStatus + "\n" + errorThrown);
                },
                success: function (result) {
                   
                    $j('#RatingRange')[0].innerHTML = "<select id='ddlRating'>" + result.d + "</select>";

                }
            });
        }

        function getResourceTypesList() {
            $j.ajax({
                type: "POST",
                url: "./btWebServices.aspx/getResourceTypeList",
                data: "{}",

                contentType: "application/json; charset=utf-8",
                dataType: "json",

                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    //alert(textStatus + "\n" + errorThrown);
                },
                success: function (result) {
                    /* $j('#ResourceTypeDdldiv')[0].innerHTML = "<select id='ResourceTypeDdl' onchange='getResourceSubTypesList(this.value);'>" + result.d + "</select>";
                    tgClearSelectOptionsFast("ResourceSubTypeDdl");                    
                    */

                    $j('#ResourceTypeDdldiv')[0].innerHTML = "<select id='ResourceTypeDdl' onchange='getSelectedResourceSubTypesList();'>" + result.d + "</select>";
                    overrideBootstrapCSS();
                }
            });
        }

        function getResourceSubTypesList(resourceType) {
            $j.ajax({
                type: "POST",
                url: "./btWebServices.aspx/getResourceSubTypeList",
                data: "{ 'resourceType' : '" + resourceType + "' }",

                contentType: "application/json; charset=utf-8",
                dataType: "json",

                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    //alert(textStatus + "\n" + errorThrown);
                },
                success: function (result) {
                    $j('#ResourceSubTypeDdldiv')[0].innerHTML = "<select id='ResourceSubTypeDdl'>" + result.d + "</select>";
                    overrideBootstrapCSS();
                }
            });
        }

        function getSelectedResourceSubTypesList() {
            var resourceType = $j('#ResourceTypeDdl').find(':selected').val();
         
            $j.ajax({
                type: "POST",
                url: "./btWebServices.aspx/getResourceSubTypeList",
                data: "{ 'resourceType' : '" + resourceType + "' }",

                contentType: "application/json; charset=utf-8",
                dataType: "json",

                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    //alert(textStatus + "\n" + errorThrown);
                },
                success: function (result) {
                    $j('#ResourceSubTypeDdldiv')[0].innerHTML = "<select id='ResourceSubTypeDdl'>" + result.d + "</select>";
                    overrideBootstrapCSS();
                }
            });
        }

        function getResourceCount(docid) {
            var count = 0;

            $j.ajax({
                type: "POST",
                url: "./btWebServices.aspx/getResourceCount",
                data: "{'docid':'" + docid + "'}",

                contentType: "application/json; charset=utf-8",
                dataType: "json",

                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    //alert(textStatus + "\n" + errorThrown);
                },
                success: function (result) {
                    count = result.d;
                    if (count > 0) {
                        getCurrentResources(docid);
                        $j('#tabs').tabs("option", "active", 0);
                    } else {
                        updateCurrentResourcesDataTable(null);
                        $j('#tabs').tabs("option", "active", 1);
                    }
                    updateAvailableResourcesDataTable("");
                    getInitialGradeList();
                    getCurrentCurricula(docid);
                }
            });
            return count;
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

        function getDocIDValue() {
            return $j("#DocID").val();
        }

        function updateCurrentResourcesDataTable(jsondata) {
            try {
                if (typeof oTable1 == 'undefined') {
                    oTable1 = $j('#currentResourcesDataTable').dataTable({
                        "bJQueryUI": true,
                        "bPaginate": false,
                        "bLengthChange": false,
                        "sScrollY": 280,
                        "aaSorting": [[1, "asc"]],
                        "aaData": jsondata,
                        "aoColumns":
					[
						{ "sTitle": "ID", "mData": "ID", "sClass": "headerwidth" },
						{ "sTitle": "Name", "mData": "Name" },
						{ "sTitle": "Type", "mData": "Type" },
					    { "sTitle": "SubType", "mData": "SubType" },
				        { "sTitle": "Description", "mData": "Description" },
					    { "sTitle": "Rating", "mData": "AverageRating" }
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

            tgMakeTableSelectable('currentResourcesDataTable');
            return oTable1;

        }

        function updateAvailableResourcesDataTable(jsondata) {
            try {
                if (typeof oTable2 == 'undefined') {
                    oTable2 = $j('#availableResourcesDataTable').dataTable({
                        "bJQueryUI": true,
                        "bPaginate": false,
                        "bLengthChange": false,
                        "sScrollY": 200,
                        "aaSorting": [[1, "asc"]],
                        "aaData": jsondata,
                        "aoColumns":
					[
						{ "sTitle": "ID", "mData": "ID", "sClass": "headerwidth" },
						{ "sTitle": "Name", "mData": "Name" },
						{ "sTitle": "Type", "mData": "Type" },
					    { "sTitle": "SubType", "mData": "SubType" },
				        { "sTitle": "Description", "mData": "Description" },
					    { "sTitle": "Rating", "mData": "AverageRating" }
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

            tgMakeTableSelectable('availableResourcesDataTable');

        }

        function overrideBootstrapCSS() {
            $j("select").css({ "width": "130px", "font-size": "11px" });
            $j("input[type=text]").css({ "width": "120px", "font-size": "11px" });
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
                }
            });
        }

        function getSubjectList(Subject) {
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
                    $j("#SubjectDdl option[value='" + Subject + "']").attr('selected', 'selected');
                    getCourseList();
                }
            });
        }

        function getInitialGradeList() {
            $j.ajax({
                type: "POST",
                url: "./btWebServices.aspx/getGradeList",
                data: "{}",

                contentType: "application/json; charset=utf-8",
                dataType: "json",

                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    //alert(textStatus + "\n" + errorThrown);
                },
                success: function (result) {

                    $j('#GradeDdldiv')[0].innerHTML = "<select id='GradeDdl' onchange='getSubjectList();'>" + result.d + "</select>";
                    tgClearSelectOptionsFast("SubjectDdl");
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
                    $j('#CourseDdldiv')[0].innerHTML = "<select id='CourseDdl'>" + result.d + "</select>";
                }
            });
        }
        function getCurrentCurricula(docid) {

            $j.ajax({
                type: "POST",
                url: "./btWebServices.aspx/getCurrentCurricula",
                data: "{'docid':'" + docid + "'}",
                contentType: "application/json; charset=utf-8",
                dataType: "json",

                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    //alert(textStatus + "\n" + errorThrown);
                },
                success: function (result) {
                    var json = JSON.parse(result.d);
                    $j("#GradeDdl option[value='" + json[0].Grade + "']").attr('selected', 'selected');
                    getSubjectList(json[0].Subject);
                    // debugger;
                    //$j("#SubjectDdl option[value='" + json[0].Subject + "']").attr('selected', 'selected');

                	//removed the following as this is being called in getSubjectList()
                	//getCourseList();
                }
            });
        }

    </script>
</body>
</html>
