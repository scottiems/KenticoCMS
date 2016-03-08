
/*********************
Standards Association
**********************/
function showStdAssoc(doctype, docid, ctrlid) {
		var bdgid = ctrlid + "-" + docid + "-badge_div_Std";
		var url = applicationVirtualPath + '/CMSWebParts/ThinkgateWebParts/StdAddNewAssoc.aspx?doctype=' + doctype + '&parentnodeid=' + docid + '&ctrlid=' + ctrlid;
		$j("#standards_addnew_association-content")[0].innerHTML = "<IFRAME SRC=" + url + "  width=1000 height=540 frameborder=0 >";
		$j("#standards_addnew_association").dialog({
			height: 640,
			width: 1020,
			title: "Standards Associations",
			resizable: false,
			bgiframe: true,
			modal: true,
			dialogClass: "tgAddExistingDialog",
			close: function (event, ui) { setBadgeStandardsCount(docid, bdgid); }
		});
	}

	function setBadgeStandardsCount(docid, bdgid) {
		var count = 0;

		$j.ajax({
			type: "POST",
			url: applicationVirtualPath + "/CMSWebParts/ThinkgateWebParts/btWebServices.aspx/getStandardCount",
			data: "{'docid':'" + docid + "'}",

			contentType: "application/json; charset=utf-8",
			dataType: "json",

			error: function (XMLHttpRequest, textStatus, errorThrown) {
				alert(textStatus + "\n" + errorThrown);
			},
			success: function (result) {
				count = result.d;
				if (count === undefined || count === null) {
					count = 0;
				}
				$j("#" + bdgid)[0].innerHTML = count + "";
			}
		});
		return count;
	}
/** END Standards Association **/

/*********************
Curriculum Association
**********************/
	function showCurriculaAssoc(doctype, docid, ctrlid) {
		var bdgid = ctrlid + "-" + docid + "-badge_div_Curr";
		var url = applicationVirtualPath + '/CMSWebParts/ThinkgateWebParts/CurriculaAddNewAssoc.aspx?doctype=' + doctype + '&parentnodeid=' + docid + '&ctrlid=' + ctrlid;
		$j("#curricula_addnew_association-content")[0].innerHTML = "<IFRAME SRC=" + url + "  width=1000 height=540 frameborder=0 >";
		$j("#curricula_addnew_association").dialog({
			height: 640,
			width: 1020,
			title: "Curricula Associations",
			resizable: false,
			bgiframe: true,
			modal: true,
			dialogClass: "tgAddExistingDialog",
			close: function (event, ui) { setBadgeCurriculaCount(docid, bdgid); }
		});
	}

	function setBadgeCurriculaCount(docid, bdgid) {
		var count = 0;

		$j.ajax({
			type: "POST",
			url: applicationVirtualPath + "/CMSWebParts/ThinkgateWebParts/btWebServices.aspx/getCurriculaCount",
			data: "{'docid':'" + docid + "'}",

			contentType: "application/json; charset=utf-8",
			dataType: "json",

			error: function (XMLHttpRequest, textStatus, errorThrown) {
				alert(textStatus + "\n" + errorThrown);
			},
			success: function (result) {
				count = result.d;
				if (count === undefined || count === null) {
					count = 0;
				}
				$j("#" + bdgid)[0].innerHTML = count + "";
			}
		});
		return count;
	}
/** END Curriculum Association **/

/*********************
Assessment Association
**********************/
function showAssessmentAssoc(doctype, docid, ctrlid) {
	var bdgid = ctrlid + "-" + docid + "-badge_div_Assmt";	    
	var url = applicationVirtualPath + '/CMSWebParts/ThinkgateWebParts/AssessmentAddNewAssoc.aspx?doctype=' + doctype + '&parentnodeid=' + docid + '&ctrlid=' + ctrlid;
	$j("#assessment_addnew_association-content")[0].innerHTML = "<IFRAME SRC=" + url + "  width=1000 height=540 frameborder=0 >";
	$j("#assessment_addnew_association").dialog({
	    height: 640,
	    width: 1020,
	    title: "Assessments Associations",
	    resizable: false,
	    bgiframe: true,
	    modal: true,
	    dialogClass: "tgAddExistingDialog",
	    close: function (event, ui) { setBadgeAssessmentCount(docid, bdgid); }
	});
}

function setBadgeAssessmentCount(docid, bdgid) {
	var count = 0;

	$j.ajax({
	    type: "POST",
	    url: applicationVirtualPath + "/CMSWebParts/ThinkgateWebParts/btWebServices.aspx/getAssessmentCount",
	    data: "{'docid':'" + docid + "'}",

	    contentType: "application/json; charset=utf-8",
	    dataType: "json",

	    error: function (XMLHttpRequest, textStatus, errorThrown) {
	        alert(textStatus + "\n" + errorThrown);
	    },
	    success: function (result) {
	        count = result.d;
	        if (count === undefined || count === null) {
	            count = 0;
	        }
	        $j("#" + bdgid)[0].innerHTML = count + "";
	    }
	});
	return count;
}
/** END Assessment Association **/


/*********************
LRMI Association
**********************/
function showLRMIAssoc(e, doctype, docid, ctrlid) {
    if (!e) var e = window.event; // coded for firefox
    var targetObjectId = (e.srcElement || e.target).id; //code change for firefox   
    //var targetObjectId = (window.event.srcElement || window.event.target).id; //commented for firefox TFS Bug #9739
	var targetToolbar = $j($j("#" + targetObjectId).parents(".toolBar")[0]);
	var targetStandardBadge = targetToolbar.find(".toolBarWrapper").find("div[id*=badge_div_Std]");

	var url = applicationVirtualPath + '/CMSWebParts/ThinkgateWebParts/LRMIAddNewAssoc.aspx?doctype=' + doctype + '&parentnodeid=' + docid + '&ctrlid=' + ctrlid;
	$j("#lrmi_addnew_association-content")[0].innerHTML = "<IFRAME SRC=" + url + " width=980 height=660 frameborder=0 >";
	$j("#lrmi_addnew_association").dialog({
        height: 740,
        width: 1000,
	    title: "Tags Associations",
	    resizable: false,
	    bgiframe: true,
	    modal: true,
	    dialogClass: "tgAddExistingDialog",
	    beforeClose: function (event, ui) {
	        return confirmDialogClose()
	    },
	    close: function (event, ui)
	    {	        
	        if (targetStandardBadge && targetStandardBadge.length > 0) {
	            var bdgid = targetStandardBadge[0].id;
	            setBadgeStandardsCount(docid, bdgid);
	        }	      
	    }
	});
}
/** END LRMI Association **/

/*********************
Resources Association
**********************/
function showResAssoc(doctype, docid, ctrlid) {
    var bdgid = ctrlid + "-" + docid + "-badge_div_Res";
    var url = applicationVirtualPath + '/CMSWebParts/ThinkgateWebParts/ResAddNewAssoc.aspx?doctype=' + doctype + '&parentnodeid=' + docid + '&ctrlid=' + ctrlid;
    $j("#resources_addnew_association-content")[0].innerHTML = "<IFRAME SRC=" + url + "  width=1000 height=540 frameborder=0 >";
    $j("#resources_addnew_association").dialog({
        height: 640,
        width: 1020,
        title: "Resource Associations",
        resizable: false,
        bgiframe: true,
        modal: true,
        dialogClass: "tgAddExistingDialog",
        close: function (event, ui) { setBadgeResourcesCount(docid, bdgid); }
    });
}

function setBadgeResourcesCount(docid, bdgid) {
    var count = 0;

    $j.ajax({
        type: "POST",
        url: applicationVirtualPath + "/CMSWebParts/ThinkgateWebParts/btWebServices.aspx/getResourceCount",
        data: "{'docid':'" + docid + "'}",

        contentType: "application/json; charset=utf-8",
        dataType: "json",

        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert(textStatus + "\n" + errorThrown);
        },
        success: function (result) {
            count = result.d;
            if (count === undefined || count === null) {
                count = 0;
            }
            $j("#" + bdgid)[0].innerHTML = count + "";
        }
    });
    return count;
}

function confirmDialogClose() {
    return confirm("Are you sure you wish to close this window? Any unsaved changes will be lost.");
}
/** END Resources Association **/
