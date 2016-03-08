<%@ Page Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_BizForms_Tools_BizForm_Edit" Theme="Default"
    ValidateRequest="false" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Title="BizForm edit - New record" EnableEventValidation="false" CodeFile="BizForm_Edit.aspx.cs" %>

<asp:Content ID="cntCtrl" ContentPlaceHolderID="plcControls" runat="server">
<%--        <table cellspacing="0" cellpadding="0" border="0">
            <tbody>
                <tr>
                    <td>
                        <cms:LocalizedCheckBox ID="chkSendNotification" runat="server" ResourceString="bizform.sendnotification" />
                    </td>
                    <td style="width: 20px;">&nbsp;</td>
                    <td>
                        <cms:LocalizedCheckBox ID="chkSendAutoresponder" runat="server" ResourceString="bizform.sendautoresponder" />
                    </td>
                </tr>
            </tbody>
        </table>--%>
</asp:Content>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
<script type="text/javascript" src="../../../CMSScripts/jquery/jquery.min.js"></script><script type="text/javascript">
var e3$ = $.noConflict();
var studentID, issueID;
var sitename = location.hostname;
e3$(document).ready(function () {
    var pathName = location.pathname;
    var fileName = pathName.substr(pathName.lastIndexOf('/') + 1);
    var formName = e3$('#bizFormName').val();
    var studentInput = e3$('input').filter(function () {
        if (e3$(this).attr('id') && e3$(this).attr('id').toLowerCase().indexOf('student_no_textbox') > -1)
            return true;
    });
    studentID = studentInput.length > 0 ? studentInput.val() : null;
    var issueInput = e3$('input').filter(function () {
        if (e3$(this).attr('id') && e3$(this).attr('id').toLowerCase().indexOf('issue_id_textbox') > -1)
            return true;
    });
    issueID = issueInput.length > 0 ? issueInput.val() : null;
    var formVars = {};
    var clientID = e3$('#clientID').val();
    var clientIDLegacy = clientID + 'Legacy';
    formVars.StudentID = studentID;
    formVars.FormName = formName;
    formVars.RTIClassID = issueID;

    var formParms = "formVars=" + JSON.stringify(formVars);

    var studentHTML =
        '<table border="0" cellspacing="0" cellpadding="0" style="width:100%;">' +
        '<tr><td>'
        + '<table border="0" cellspacing="0" cellpadding="2" style="width:100%;"><tr>'
        + '<td style="vertical-align:bottom; font-family:Arial,sans-serif; font-size: 11pt; font-weight:normal !important; text-decoration:none !important; width:1%; white-space:nowrap;">Student Name</td><td class="mtssStandardHeaderStudentName" style="vertical-align:bottom; border-bottom:solid 1px #000;white-space:nowrap;height:20px;padding-left:10px; font-weight:normal !important; text-decoration:none !important;"></td>'
        + '<td style="vertical-align:bottom; font-family:Arial,sans-serif; font-size: 11pt; font-weight:normal !important; text-decoration:none !important; width:1%; white-space:nowrap;">Gender</td><td class="mtssStandardHeaderGender" style="vertical-align:bottom; border-bottom:solid 1px #000;white-space:nowrap;width:10%;height:20px;padding-left:10px; font-weight:normal !important; text-decoration:none !important;"></td>'
        + '<td style="vertical-align:bottom; font-family:Arial,sans-serif; font-size: 11pt; font-weight:normal !important; text-decoration:none !important; width:1%; white-space:nowrap;">DOB</td><td class="mtssStandardHeaderDOB" style="vertical-align:bottom; border-bottom:solid 1px #000;white-space:nowrap;width:10%;height:20px;padding-left:10px; font-weight:normal !important; text-decoration:none !important;"></td>'
        + '</tr></table>'
        + '</td></tr>'
        + '<tr><td>'
        + '<table border="0" cellspacing="0" cellpadding="2" style="width:100%;"><tr>'
        + '<td style="vertical-align:bottom; font-family:Arial,sans-serif; font-size: 11pt; font-weight:normal !important; text-decoration:none !important; width:1%; white-space:nowrap;">Student No.</td><td class="mtssStandardHeaderStudentNo" style="vertical-align:bottom; border-bottom:solid 1px #000;white-space:nowrap;width:10%;height:20px;padding-left:10px; font-weight:normal !important; text-decoration:none !important;"></td>'
        + '<td style="vertical-align:bottom; font-family:Arial,sans-serif; font-size: 11pt; font-weight:normal !important; text-decoration:none !important; width:1%; white-space:nowrap;">School</td><td class="mtssStandardHeaderSchool" style="vertical-align:bottom; border-bottom:solid 1px #000;white-space:nowrap;height:20px;padding-left:10px; font-weight:normal !important; text-decoration:none !important;"></td>'
        + '<td style="vertical-align:bottom; font-family:Arial,sans-serif; font-size: 11pt; font-weight:normal !important; text-decoration:none !important; width:1%; white-space:nowrap;">Grade</td><td class="mtssStandardHeaderGrade" style="vertical-align:bottom; border-bottom:solid 1px #000;white-space:nowrap;width:10%;height:20px;padding-left:10px; font-weight:normal !important; text-decoration:none !important;"></td>'
        + '</tr></table>'
        + '</td></tr>'
        + '<tr><td>'
        + '<table border="0" cellspacing="0" cellpadding="2" style="width:100%;"><tr>'
        + '<td style="vertical-align:bottom; font-family:Arial,sans-serif; font-size: 11pt; font-weight:normal !important; text-decoration:none !important; width:1%; white-space:nowrap;">Parent/Guardian</td><td class="mtssStandardHeaderParentGuardian" style="vertical-align:bottom; border-bottom:solid 1px #000;white-space:nowrap;height:20px;padding-left:10px; font-weight:normal !important; text-decoration:none !important;"></td>'
        + '</tr></table>'
        + '</td></tr>'
        + '<tr><td>'
        + '<table border="0" cellspacing="0" cellpadding="2" style="width:100%;"><tr>'
        + '<td style="vertical-align:bottom; font-family:Arial,sans-serif; font-size: 11pt; font-weight:normal !important; text-decoration:none !important; width:1%; white-space:nowrap;">Address</td><td class="mtssStandardHeaderAddress" style="vertical-align:bottom; border-bottom:solid 1px #000;white-space:nowrap;height:20px;padding-left:10px; font-weight:normal !important; text-decoration:none !important;"></td>'
        + '</tr></table>' +
        '</td></tr></table>';

    if (studentID && studentID.length == 0) {
        e3$('.studentDemographics').html(studentHTML);
        e3$('.studentDemographics').css('background-color', '');
        e3$('.iframeContainer').remove();
        return false;
    }

    e3$.ajax({
        type: 'GET',
        contentType: "application/json; charset=utf-8",
        url: '/' + clientID + '/Services/KenticoCMSRequests.asmx/GetFormData',
        data: formParms,
        dataType: "json"
    }).done(function (data) {
        var msg;
        if (data.hasOwnProperty('d')) {
            msg = data.d;
        }
        else {
            msg = data;
        }

        e3$('.studentDemographics').html(studentHTML);
        e3$('.studentDemographics').css('background-color', '');

        for (position in msg) {
            if (typeof (msg[position]) != 'object' || msg[position].length == 0) continue;

            switch (msg[position][0]) {
                case "Student":
                    switch (msg[position][1]) {
                        case 'Program':
                            insertValue(e3$('#studentIssues'), msg[position][2], true);
                            break;
                        case 'Description':
                            insertValue(e3$('#rtiDescription'), msg[position][2], true);
                            break;
                        case 'Tier':
                            insertValue(e3$('#rtiTier'), msg[position][2], true);
                            break;
                        case 'Race':
                            insertValue(e3$('#studentRace'), msg[position][2], true);
                            break;
                        case 'Teacher Name':
                            insertValue(e3$('#studentTeacher'), msg[position][2], true);
                            break;
                        case 'Home Phone':
                            insertValue(e3$('#studentHomePhone'), msg[position][2], true);
                            break;
                        case 'Work Phone':
                            insertValue(e3$('#studentWorkPhone'), msg[position][2], true);
                            break;
                        case 'Emergency Phone':
                            insertValue(e3$('#studentEmergencyPhone'), msg[position][2], true);
                            break;
                        case 'School No.':
                            insertValue(e3$('#schoolID'), msg[position][2], true);
                            break;
                        case 'Principal Name':
                            insertValue(e3$('#principalName'), msg[position][2], true);
                            break;
                        case 'Student Name':
                            if(e3$('.mtssStandardHeaderStudentName').length > 0) insertValue(e3$('.mtssStandardHeaderStudentName'), msg[position][2]);
                            if(e3$('.studentName').length > 0) insertValue(e3$('.studentName'), msg[position][2], true);
                            break;
                        case 'Benchmark':
                            insertValue(e3$('.benchmark'), msg[position][2], true);
                            break;
                        case 'Peers':
                            insertValue(e3$('.peers'), msg[position][2], true);
                            break;
                        case 'Student Scores':
                            insertValue(e3$('.studentScores'), msg[position][2], true);
                            break;
                        case 'Intervention Start Date':
                            insertValue(e3$('.rtiStartDate'), msg[position][2], true);
                            break;
                        case 'Intervention Target Date':
                            insertValue(e3$('.rtiTargetDate'), msg[position][2], true);
                            break;
                        case 'Chronological Age':
                            insertValue(e3$('.chronologicalAge'), msg[position][2]);
                            break;
                        case 'Gender':
                            insertValue(e3$('.mtssStandardHeaderGender'), msg[position][2]);
                            break;
                        case 'DOB':
                            insertValue(e3$('.mtssStandardHeaderDOB'), msg[position][2]);
                            break;
                        case 'Student No.':
                            insertValue(e3$('.mtssStandardHeaderStudentNo'), msg[position][2]);
                            break;
                        case 'School':
                            insertValue(e3$('.mtssStandardHeaderSchool'), msg[position][2]);
                            break; 
                        case 'Grade':
                            insertValue(e3$('.mtssStandardHeaderGrade'), msg[position][2]);
                            break;
                        case 'Parent/Guardian':
                            insertValue(e3$('.mtssStandardHeaderParentGuardian'), msg[position][2]);
                            break;
                        case 'Address':
                            insertValue(e3$('.mtssStandardHeaderAddress'), msg[position][2]);
                            break;
                    }
						
                    break;
            }
        }
    }).fail(function (jqXHR, textStatus, error) {
        alert(textStatus + ' ' + error);
    });

    e3$('#formCountSpan').html(e3$('#formCount').val());

    try {
        if(issueID > 0 && studentID > 0) {
            e3$('#progressReport_iframe').attr('src', 'http://' + sitename + '/' + clientIDLegacy + '/rtichart.asp?chart=2&kentico=yes&classID=' + issueID + '&studentID=' + studentID);
            e3$('#effectivenessReport_iframe').attr('src', 'http://' + sitename + '/' + clientIDLegacy + '/rtichart.asp?chart=1&kentico=yes&classID=' + issueID + '&studentID=' + studentID);
            e3$('#attendanceReport_iframe').attr('src', 'http://' + sitename + '/' + clientIDLegacy + '/rtichart.asp?chart=3&kentico=yes&classID=' + issueID + '&studentID=' + studentID);
            e3$('#participationReport_iframe').attr('src', 'http://' + sitename + '/' + clientIDLegacy + '/rtichart.asp?chart=5&kentico=yes&classID=' + issueID + '&studentID=' + studentID + '&auto=yes');
        }
        else {
            e3$('.iframeContainer').remove();
        }
    }
    catch (e) {
    }

});

/*
	Prints a customized layout using temporary CSS manipulation and the DOM.
*/
function printForm() {
    var formWrapper = getFormWrapper();

    //Expose all text entries
    exposeFullTextEntries(formWrapper);

    //Rebuild layout to fit print pages
    reconstructFormToPrintLayout(formWrapper);

    //Print form
    window.print();

    //Put form back to original state
    revertFormToScreenLayout(formWrapper);
}

/*
	Use the DOM to copy text entries into new DIVs for full text exposure.
*/
function exposeFullTextEntries(formWrapper) {
    //If no wrapper exists, abort mission!
    if (formWrapper === null) return false;

    //Add DIVs for all text entry fields for full text exposure in print format
    //Only applies if DIVs do not already exist
    e3$('textarea,input[type="text"]', formWrapper).each(function (index) {
        var currElement = e3$(this);

        if ((currElement.css('display') != 'none' && currElement.css('visibility') != 'hidden') || currElement.attr('class').indexOf('dynamicDDField') > -1) {
            if (e3$('#' + currElement.attr('id') + '_print').length == 0) {
                var elementH = e3$('#' + currElement.attr('id')).height() + 'px';
                var elementW = e3$('#' + currElement.attr('id')).width() + 'px'
                var newStyle = 'min-height: ' + elementH + '; width: ' + elementW + ';';

                if (currElement.attr('id').indexOf('Phone') == -1) {
                    currElement.after('<div class="printText" id="' + currElement.attr('id') + '_print" style="' + newStyle + '">' + currElement.val() + '&nbsp;</div>');
                    currElement.addClass('formEntry');
                }
            }
            else {
                e3$('#' + currElement.attr('id') + '_print').html(currElement.val());
            }
        }
    });
}

/*
	Add the header to each page and page breaks to table elements
	accordingly.
*/
function reconstructFormToPrintLayout(formWrapper) {
    //If no wrapper exists, abort mission!
    if (formWrapper === null) return false;

    //Add repeating header and page breaks
    var formHeader = e3$('.customFormHeader', formWrapper);
    var iframeContainer = e3$('.iframeContainer', formWrapper);
    var tableList = e3$('> table[class!="customFormHeader"][class!="iframeContainer"]', formWrapper); //Top level tables
    var headerHeight = formHeader.outerHeight(true);
    var maxHeight = 1300;
    var accumulatedHeight = 0;

    if (formHeader.length == 0) return false;

    tableList.each(function (index) {
        var table = e3$(this);
        var nextIndex = index + 1;
        var tableHeight = calculateActualTableHeight(table);

        if (accumulatedHeight == 0) { //It's a new page.
            //If this is a subsequent page, add the header to the top of the page
            if (index > 0) table.before(formHeader.clone());

            //Accumulate height
            accumulatedHeight += headerHeight + tableHeight;
        }
        else {
            //Accumulate height
            accumulatedHeight += tableHeight;
        }

        if (accumulatedHeight == maxHeight) { //End page if max height is met
            table.addClass('temporaryPageBreak');
            accumulatedHeight = 0;
            return true;
        }
        else if (accumulatedHeight > maxHeight) { //Fit as many table rows possible, then dump remaining rows into new table for next page
            var calcHeight = accumulatedHeight - tableHeight;
            var cutoffRowPos = 0;
            var cutoffRowSet = false;
            var tBorder = parseInt(table[0].border > 0 ? table[0].border : 0);
            var tableRows = table.find('tbody').length > 0 ? table.children().children() : table.children();

            //Calculate number of rows to fit on current page
            //Set fixed width to each cell of each row
            tableRows.each(function (rowIndex) {
                var tdHeight = 0;
                e3$(this).children().each(function (cellIndex) {
                    var td = e3$(this);

                    td.css('width', (td.width() - tBorder) + 'px');
                    tdHeight = td.outerHeight() + tBorder;

                    if (!cutoffRowSet) {
                        var elementHeight = 0;

                        td.find('div[class="printText"]').each(function (elementIndex) {
                            elementHeight += e3$(this).outerHeight(true);
                        });

                        if (elementHeight > tdHeight) {
                            tdHeight = elementHeight + tBorder;
                        }
                    }
                });

                calcHeight += tdHeight;

                if (calcHeight > maxHeight && !cutoffRowSet) {
                    cutoffRowPos = rowIndex;
                    cutoffRowSet = true;
                }
            });

            //Create clone of table
            var tableClone = table.clone();
            tableClone.addClass('table-clone');

            //Hide cut-off rows in original table
            tableRows.slice(cutoffRowPos).addClass('hiddenRow');

            //Remove rows before cut-off rows in cloned table
            var tableCloneRows = tableClone.find('tbody').length > 0 ? tableClone.children().children() : tableClone.children();
            tableCloneRows.slice(0, cutoffRowPos).remove();

            //Add cloned table after clone parent
            table.after(tableClone);

            //Add header before cloned table so it will appear at the top of the next page
            tableClone.before(formHeader.clone());

            //Set accumulated height to header height + cloned table height
            accumulatedHeight = processThroughCloneTable(tableClone, formHeader, headerHeight, maxHeight); //headerHeight + calculateActualTableHeight(tableClone);

            //Add page break to original table
            table.addClass('temporaryPageBreak');
        }
    });

    //Add header to each RTI graph iframe
    iframeContainer.find('iframe').each(function (index) {
        e3$(this).before(formHeader.clone());
    });
}

function processThroughCloneTable(table, formHeader, headerHeight, maxHeight) {
    var tableHeight = calculateActualTableHeight(table);
    var accumulatedHeight = headerHeight + tableHeight;

    if (accumulatedHeight > maxHeight) {
        var calcHeight = headerHeight;
        var cutoffRowPos = 0;
        var cutoffRowSet = false;
        var tBorder = parseInt(table[0].border > 0 ? table[0].border : 0);
        var tableRows = table.find('tbody').length > 0 ? table.children().children() : table.children();

        //Calculate number of rows to fit on current page
        //Set fixed width to each cell of each row
        tableRows.each(function (rowIndex) {
            var tdHeight = 0;
            e3$(this).children().each(function (cellIndex) {
                var td = e3$(this);

                td.css('width', (td.width() - tBorder) + 'px');
                tdHeight = td.outerHeight() + tBorder;

                if (!cutoffRowSet) {
                    var elementHeight = 0;

                    td.find('div[class="printText"]').each(function (elementIndex) {
                        elementHeight += e3$(this).outerHeight(true);
                    });

                    if (elementHeight > tdHeight) {
                        tdHeight = elementHeight + tBorder;
                    }
                }
            });

            calcHeight += tdHeight;

            if (calcHeight > maxHeight && !cutoffRowSet) {
                cutoffRowPos = rowIndex;
                cutoffRowSet = true;
            }
        });

        //Create clone of table
        var tableClone = table.clone();
        tableClone.addClass('table-clone');

        //Hide cut-off rows in original table
        tableRows.slice(cutoffRowPos).addClass('hiddenRow');

        //Remove rows before cut-off rows in cloned table
        var tableCloneRows = tableClone.find('tbody').length > 0 ? tableClone.children().children() : tableClone.children();
        tableCloneRows.slice(0, cutoffRowPos).remove();

        //Add cloned table after clone parent
        table.after(tableClone);

        //Add header before cloned table so it will appear at the top of the next page
        tableClone.before(formHeader.clone());

        //Add page break to original table
        table.addClass('temporaryPageBreak');

        //Run process again
        processThroughCloneTable(tableClone, formHeader, headerHeight, maxHeight);
    }
    else {
        return accumulatedHeight;
    }
}

function calculateActualTableHeight(table) {
    var calcHeight = 0;
    var tBorder = parseInt(table[0].border > 0 ? table[0].border : 0);
    var tableRows = table.find('tbody').length > 0 ? table.children().children() : table.children();

    tableRows.each(function (rowIndex) {
        var tdHeight = 0;
        e3$(this).children().each(function (cellIndex) {
            var td = e3$(this);
            var elementHeight = 0;

            tdHeight = td.outerHeight() + tBorder;

            td.find('div[class="printText"]').each(function (elementIndex) {
                elementHeight += e3$(this).outerHeight(true);
            });

            if (elementHeight > tdHeight) {
                tdHeight = elementHeight + tBorder;
            }
        });

        calcHeight += tdHeight;
    });

    return calcHeight;
}

/*
	Removes all temporary CSS and DOM manipulation to put the form
	back in its original state.
*/
function revertFormToScreenLayout(formWrapper) {
    //If no wrapper exists, abort mission!
    if (formWrapper === null) return false;

    //Remove all duplicate headers
    e3$('.customFormHeader', formWrapper).slice(1).remove();

    //Remove all temporary page break classes
    e3$('.temporaryPageBreak').removeClass('temporaryPageBreak');

    //Remove all cloned tables
    e3$('.table-clone', formWrapper).remove();

    //Remove all hidden row classes
    e3$('.hiddenRow').removeClass('hiddenRow');
}

/*
	Searches for one of two possible container elements that holds all
	of the form content. Returns container object.
*/
function getFormWrapper() {
    var formWrapper;

    if (e3$('.mainBlock').length > 0) { //We're inside a template or new form
        formWrapper = e3$('.mainBlock').find('.FormPanel');
    }
    else if (e3$('.PageContent').length > 0) { //We're inside a form edit view
        formWrapper = e3$('.PageContent').find('.FormPanel');
    }

    return formWrapper;
}

/*
	Inserts the specified value into the passed in place holder and 
	replaces any empty text field contained within.
	- placeHolder: Container object that holds value [Object]
	- value: Text value to be placed into placeHolder object [String]
	- addSpan: (Optional) If set to true, wraps value in a left-padded span [Boolean]
*/
function insertValue(placeHolder, value, addSpan) {
    if (placeHolder.length > 0) {
        var editableField = e3$('input,textarea', placeHolder);

        if (
			(
				editableField.length > 0 &&
				editableField[0].value.replace(' ', '').length == 0 &&
				value.length > 0
			)

			||

			(
				editableField.length == 0 &&
				value.length > 0
			)
		) {
            if (value.indexOf('@@') > -1) {
                createDropdown(value.split('@@'), editableField);
            }
            else if (addSpan) {
                placeHolder.html('<span style="padding-left:10px;">' + value + '</span>');
            }
            else {
                placeHolder.html(value);
            }
        }
        else if (value.indexOf('@@') > -1) {
            createDropdown(value.split('@@'), editableField);
        }

        placeHolder.css('background-color', '');
    }
}

function createDropdown(list, field) {
    var dd = document.createElement('SELECT');

    var blankOption = document.createElement('option');
    blankOption.text = '';
    blankOption.selected = true;
    dd.add(blankOption);

    for (n in list) {
        var option = document.createElement('option');
        option.text = list[n];
        option.selected = field.val() == list[n];
        dd.add(option);
    }

    e3$(dd).attr('fieldID', field.attr('id')).addClass('dynamicDD').change(function () {
        e3$('#' + e3$(this).attr('fieldID')).val(e3$(this).val());
    });

    field.after(dd).addClass('dynamicDDField').hide();
}
</script>
<style media="screen" type="text/css">
	.printText {
		display: none;
	}

    .mtss_requiredField {
		font-family: Arial, sans-serif !important;
		color: #F00 !important;	
		font-weight: bold !important;
		font-size: 8pt;
	}

	.mtss_formAlerts {
		visibility: hidden;
		padding: 5px;
	}

	.mtss_formAlerts>span {
		color: #000 !important;
		display: inline !important;
	}

	.ErrorLabel {
		visibility: hidden;
	}

	#mtssFormErrorDialog {
		display: none;
		left: 10px;
		top: 10px; 
		position: fixed; 
		opacity: 0.87;
	}
</style>
<style media="print" type="text/css">
	.formEntry, .dynamicDD {
		display: none !important;
	}
	.printText {
		display: block;
	}
	.CMSEditMenu {
		display: none !important;
	}
	body {
		margin: 0px;
	}
	.temporaryPageBreak {
		page-break-after: always;
	}
	.hiddenRow {
		display: none;
	}
	.clone-table {
		border-top-width: 0;
		border-bottom-width: 0;
	}
    .CalendarAction, .CalendarIcon {
		display: none;
	}
	.CalendarTextBox {
		white-space: nowrap;
	}
</style>

    <cms:BizForm ID="formElem" runat="server" IsLiveSite="false" />
	<asp:HiddenField runat="server" ID="bizFormName" ClientIDMode="Static"/>
	<asp:HiddenField runat="server" ID="clientID" ClientIDMode="Static"/>
</asp:Content>
