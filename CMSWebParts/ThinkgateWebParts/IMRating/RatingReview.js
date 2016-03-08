

function showRatingReviewSummary(nodeid) {
    
    if (nodeid == null || nodeid == 0) {
        alert("Can not determine the Document Node ID");
        return false;
    }
    
    var height = $j(window).height() * 0.8;
    var width = $j(window).width() * 0.8;
    

    var url = applicationVirtualPath + '/CMSWebParts/ThinkgateWebParts/IMRating/RatingReviewSummary.aspx?nodeID='+nodeid;
    $j('#review-Summary-dialog-content_' + nodeid)[0].innerHTML = "<IFRAME SRC=" + url + " width='" + (width - 50) + "' height= '" + (height - 70) + "' frameborder=0 >";

    $j("#review-Summary-dialog_" + nodeid).dialog({
        resizable: false,
        height: height,
        width: width,
        title: "Review Summary",
        resizable: true,
        bgiframe: true,
        modal: true,
        close: function (event) {     
        }
    });
}

function showRatingView(nodeid) {

    if (nodeid == null || nodeid == 0) {
        alert("Can not determine the Document Node ID");
        return false;
    }

    var height = $j(window).height() * 0.8;
    var width = $j(window).width() * 0.8;

    var url = applicationVirtualPath + '/CMSWebParts/ThinkgateWebParts/IMRating/RatingReviewView.aspx?nodeID='+nodeid;
    $j('#addNew-Review-dialog-content_' + nodeid)[0].innerHTML = "<IFRAME SRC=" + url + " width='" + (width-50)+"' height= '" + (height - 70) + "' frameborder=0 >";

    $j("#addNew-Review_" + nodeid).dialog({
        resizable: false,
        height: height,
        width: width,
        title: "Add New Review",
        resizable: true,
        bgiframe: true,
        modal: true,
        stack:true,
        close: function (event) {
       }
       
    });
    return false;
}


