function CloseMeAssocToolBar(val, acID) {
    if (acID != undefined) {
        if (val == 0) {
            document.getElementById(acID).innerHTML = "0";
        }
        else {
            document.getElementById(acID).innerHTML = val;
        }
    }
}

function OpenChild(url, acID) {
    var fullURL = url + "?ctrl=" + acID;
    var newwindow;      
    openStandardsControl(fullURL);
}
    
function openModalAssocToolBarDialog(event, documentId, documentType, userID, count, category, url, associationDataListURL, associationListDialogTitle, ctrlid, resource)
{
    var pageUrl = undefined;
    if (parseInt(count) == 0 || category == "LRMI") {
           
        if (category == "Curricula") {
                

            pageUrl = url + "?category=" + category + "&documentid=" + documentId + "&documentType=" + documentType + "&userID=" + userID + "&ctrl=" + ctrlid + "&FromToolBar=" + resource;
            $('#addAssocToolBardialog').dialog("option", "title", "Add New Association");
            $('#addAssocToolBardialog').dialog("option", "width", 450);
            $('#addAssocToolBardialog').dialog("option", "height", 350);
            $('#addAssocToolBardialog').dialog("option", "position", 'center');
        } else {
               
            pageUrl = url + "?category=" + category + "&documentid=" + documentId + "&documentType=" + documentType + "&userID=" + userID + "&ctrl=" + ctrlid + "&FromToolBar=" + resource;
            $('#addAssocToolBardialog').dialog("option", "title", "Add New Association");
            $('#addAssocToolBardialog').dialog("option", "width", 1080);
            $('#addAssocToolBardialog').dialog("option", "height", 550);
            $('#addAssocToolBardialog').dialog("option", "position", 'center');
        }
    }
    else {
        pageUrl = associationDataListURL + "?category=" + category + "&documentid=" + documentId + "&documentType=" + documentType + "&userID=" + userID + "&url=" + url + "&ctrl=" + ctrlid + "&FromToolBar=" + resource;
        $('#addAssocToolBardialog').dialog("option", "width", 300);
        $('#addAssocToolBardialog').dialog("option", "height", 320);
        $('#addAssocToolBardialog').dialog("option", "title", associationListDialogTitle);
        $('#addAssocToolBardialog').dialog("option", "position", 'center');
    }

    $('#addAssocToolBarframe').attr('src', pageUrl);
    $('#addAssocToolBardialog').dialog("open");
   
    return false;
}

function closeAssocToolBardialog() {
    $('#addAssocToolBardialog').dialog("close");
}

$(function () {
    $('#addAssocToolBardialog').dialog({
        autoOpen: false, modal: true, width: 1080, height: 340, position: 'center', close: function (event, ui) {
            $('#addAssocToolBarframe').attr('src', '');
        }
    });
});