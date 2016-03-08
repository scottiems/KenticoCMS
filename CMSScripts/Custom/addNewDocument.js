
function tgInitDataTable(tableID) {
    if (objExists(tableID)) {
        /* Init the table */
        var oTable = $j('#' + tableID).dataTable({
            "oLanguage": { "sSearch": "Search:" },
            "iDisplayLength": 10,
            "bJQueryUI": true,
            "sPaginationType": "full_numbers",
            "sScrollY": 350,
            "aaSorting": [[0, "asc"]]
        });
    }
}


function tgSelectAll(tableID) {
    if (objExists(tableID)) {
        if ($j('#chkSelectAll').is(':checked')) {
            $j('#' + tableID + ' tr').each(function () {
                $j(this).addClass('row_selected');
            });
        }
        else {
            $j('#' + tableID + ' tr').each(function () {
                $j(this).removeClass('row_selected');
            });
        }
        fnGetSelected(tableID);


    }
}

function tgMakeTableSelectable(tableID) {
    var docTypeVal = $j('input[name=hdnDocType]').val();
    if (objExists(tableID)) {
        /* Add a click handler to the table rows - this could be used as a callback */
        $j('#' + tableID + ' tr').click(function () {
            if (docTypeVal == "competencylist") {
                if ($j(this).hasClass('row_selected')) {
                    $j(this).removeClass('row_selected');
                } else {
                    $j('#' + tableID + ' tr').removeClass("row_selected");
                    $j(this).addClass('row_selected');
                }

            }
            else {
                if ($j(this).hasClass('row_selected')) {

                    $j(this).removeClass('row_selected');                   
                    if ($j('#chkSelectAll').is(':checked')) {
                        $j('#chkSelectAll').attr('checked', false);
                    }

                } else {
                    $j(this).addClass('row_selected');
                }
            }
            fnGetSelected(tableID);
        });
    }
}

function fnGetSelected(tableID) {
    var aReturn = new Array();
    var aTrs = $j(".row_selected").children();
    if (objExists(aTrs) && aTrs.length > 0) {

        switch (tableID) {
            case "currentStandardsDataTable":
                aReturn = getSelectedColumnValues_currentStandardsDataTable(aTrs);
                break;
            case "availableStandardsDataTable":
                aReturn = getSelectedColumnValues_availableStandardsDataTable(aTrs);
                break;
            case "currentCurriculaDataTable":
                aReturn = getSelectedColumnValues_currentCurriculaDataTable(aTrs);
                break;
            case "availableCurriculaDataTable":
                aReturn = getSelectedColumnValues_availableCurriculaDataTable(aTrs);
                break;
            case "currentResourcesDataTable":
                aReturn = getSelectedColumnValues_currentResourceDataTable(aTrs);
                break;
            case "availableResourcesDataTable":
                aReturn = getSelectedColumnValues_availableResourceDataTable(aTrs);
                break;
            case "currentAssessmentsDataTable":
            case "availableAssessmentsDataTable":
                aReturn = getSelectedColumnValues_DataTable(aTrs);
                break;
            case "tblAddExistingPlans":
                aReturn = getSelectedColumnValues_availableModelCurriculum(aTrs);
                break;
            default:
                aReturn = getSelectedColumnValues_tblYourTable(aTrs);
                break;
        }
    }

    $j('input[name=SelectedItems]').val(aReturn + "");
    return aReturn;
}

function getSelectedColumnValues_DataTable(dataSource) {
    var selectedRowValues = new Array();
    var columnCount = dataSource.dataTableSettings[0].aoColumns.length;

    for (var i = 0 ; i < dataSource.length ;) {
        var content = dataSource[i].textContent;
        selectedRowValues.push(content);
        i = i + columnCount;
    }
    return selectedRowValues;
}

function getSelectedColumnValues_availableStandardsDataTable(aTrs) {
    var aReturn = new Array();
    var nbrcols = aTrs.dataTableSettings[0].aoColumns.length;
    for (var i = 0 ; i < aTrs.length ;) {
        var planNode = aTrs[i].textContent;
        //var planNode = aTrs[i].innerText;
        aReturn.push(planNode);
        i = i + nbrcols;
    }
    return aReturn;
}

function getSelectedColumnValues_currentStandardsDataTable(aTrs) {
    var aReturn = new Array();
    var nbrcols = aTrs.dataTableSettings[0].aoColumns.length;
    for (var i = 0 ; i < aTrs.length ;) {
        var tblid = aTrs[i].textContent;

        aReturn.push(tblid);
        i = i + nbrcols;
    }
    return aReturn;
}


function getSelectedColumnValues_currentCurriculaDataTable(aTrs) {
    var aReturn = new Array();
    var nbrcols = aTrs.dataTableSettings[0].aoColumns.length;
    for (var i = 0 ; i < aTrs.length ;) {
        var tblid = aTrs[i].textContent;

        aReturn.push(tblid);
        i = i + nbrcols;
    }
    return aReturn;
}


function getSelectedColumnValues_availableCurriculaDataTable(aTrs) {
    var aReturn = new Array();
    var nbrcols = aTrs.dataTableSettings[0].aoColumns.length;
    for (var i = 0 ; i < aTrs.length ;) {
        var tblid = aTrs[i].textContent;

        aReturn.push(tblid);
        i = i + nbrcols;
    }
    return aReturn;
}

function getSelectedColumnValues_currentResourceDataTable(aTrs) {
    var aReturn = new Array();
    var nbrcols = aTrs.dataTableSettings[0].aoColumns.length;
    for (var i = 0 ; i < aTrs.length ;) {
        var tblid = aTrs[i].textContent;

        aReturn.push(tblid);
        i = i + nbrcols;
    }
    return aReturn;
}

function getSelectedColumnValues_availableResourceDataTable(aTrs) {
    var aReturn = new Array();
    var nbrcols = aTrs.dataTableSettings[0].aoColumns.length;
    for (var i = 0 ; i < aTrs.length ;) {
        var tblid = aTrs[i].textContent;

        aReturn.push(tblid);
        i = i + nbrcols;
    }
    return aReturn;
}

function getSelectedColumnValues_availableModelCurriculum(aTrs) {
    var aReturn = new Array();
    var nbrcols = aTrs.dataTableSettings[0].aoColumns.length;
    for (var i = 0 ; i < aTrs.length ;) {
        //var parid = aTrs[i].textContent;
        var planNode = aTrs[i + 1].textContent;
        var documentName = aTrs[i + 2].textContent;

        aReturn.push(planNode + "|" + documentName);
        i = i + nbrcols;
    }
    return aReturn;
}

function getSelectedColumnValues_tblYourTable(aTrs) {
    var aReturn = new Array();
    var nbrcols = aTrs.dataTableSettings[0].aoColumns.length;
    for (var i = 0 ; i < aTrs.length ;) {
        //var parentNode = aTrs[i].textContent;
        var planNode = aTrs[i + 1].textContent;
        var documentName = aTrs[i + 2].textContent;

        aReturn.push(planNode + "|" + documentName);
        i = i + nbrcols;
    }
    return aReturn;
}



function showAddNewDocumentModal(doctype, doctypelbl, parentid, classid, ctrlid) {

    if (doctypelbl == "Unknown Document Type")
        throw "Invalid Document Type";

    if (doctype == "thinkgate.curriculumUnit") {
        showAddNewPlan(doctype, doctypelbl, parentid, classid, ctrlid);
    } else {

        var rbgrp = ctrlid + "-" + parentid + "_tgAddNewDoc";
        var dialogid = ctrlid + "-" + parentid + "-dialog-confirm";

        $j("#" + dialogid).dialog({
            resizable: false,
            height: 170,
            modal: true,
            title: "Add New " + doctypelbl,
            buttons: {
                OK: function () {
                    var rbval = $j('input[name=' + rbgrp + ']:checked').val();
                    if (rbval == "new") {
                        showAddNewPlan(doctype, doctypelbl, parentid, classid, ctrlid);
                    } else if (rbval == "existing") {
                        showAddExistingPlan(doctype, doctypelbl, parentid);
                    }

                    $j(this).dialog("close");
                },
                Cancel: function () {
                    $j(this).dialog("close");
                }
            }
        });
    }
}

function showAddNewPlan(doctype, doctypelbl, parentid, classid, ctrlid) {

    if (classid == null || classid.length === 0) {
        showSelectionForFormType(doctype, doctypelbl, parentid, ctrlid);
        return false;
    }

    var url = applicationVirtualPath + '/CMSModules/Content/CMSDesk/Edit/Edit.aspx?action=new&classid=' + classid + '&parentnodeid=' + parentid + '&culture=en-US';
    $j('#addNew-dialog-content')[0].innerHTML = "<IFRAME SRC=" + url + "  width=1000 height=600 frameborder=0 >";

    $j("#addNew-dialog").dialog({
        resizable: false,
        height: 680,
        width: 1020,
        title: "Add New " + doctypelbl,
        resizable: true,
        bgiframe: true,
        modal: true,
        close: function (event) {
            window.location.reload(true);
        }
    });

}

function showSelectionForFormType(doctype, doctypelbl, parentid, ctrlid) {

    var dialogid = ctrlid + "-" + parentid + "-dialog-choose-formtype";
    var rbgrp = ctrlid + "-" + parentid + "-formType";
    var classid;

    $j("#" + dialogid).dialog({
        resizable: false,
        height: 170,
        modal: true,
        title: " Please choose the " + doctypelbl + "Form Type",
        buttons: {
            OK: function () {
                var rbval = $j('input[name=' + rbgrp + ']:checked').val();
                classid = GetClassIdAfterFormTypeSelection(doctype, rbval);
                showAddNewPlan(doctype, doctypelbl, parentid, classid, ctrlid);
                $j(this).dialog("close");
            },
            Cancel: function () {
                $j(this).dialog("close");
            }
        }
    });

}



function GetClassIdAfterFormTypeSelection(doctype, selectedForm) {

    var classid;

    $j.ajax({
        type: "POST",
        url: applicationVirtualPath + "/CMSWebParts/ThinkgateWebParts/btWebServices.aspx/GetClassIdFromDocType",
        data: "{'doctype':'" + doctype + "','selectedForm':'" + selectedForm + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: false,

        error: function (XMLHttpRequest, textStatus, errorThrown) {
            //alert(textStatus + "\n" + errorThrown);
        },
        success: function (result) {
            classid = result.d;
        }
    });

    return classid;


}


function showAddExistingPlan(doctype, doctypelbl, parentid) {
    var url = applicationVirtualPath + '/CMSWebParts/ThinkgateWebParts/AddExistingSearch.aspx?doctype=' + doctype + '&parentnodeid=' + parentid;
    $j('#addExisting-dialog-content')[0].innerHTML = "<IFRAME SRC=" + url + "  width=1000 height=600 frameborder=0 >";

    $j("#addExisting-dialog").dialog({
        height: 680,
        width: 1020,
        title: "Copy Existing " + doctypelbl,
        resizable: true,
        bgiframe: true,
        modal: true,
        dialogClass: "tgAddExistingDialog",
        close: function (event) {
            window.location.reload(true);
        }
    });

}



function closeAddExistingDialog() {
    try {
        window.parent.jQuery('.tgAddExistingDialog').children(".ui-dialog-titlebar").children("button").click();
    } catch (ex) {
        alert(ex);
    }
    return false;
}

function requireCriteriaMessage() {
    window.parent.jQuery("#required_criteria_message").dialog({
        resizable: false,
        height: 150,
        width: 450,
        title: "Warning",
        bgiframe: true,
        modal: true,
        stack: true      
    });
}