function initDialog(nodeID) {

    var deleteNodeID = nodeID + "-deleteClick";
    var uniqueDialog = nodeID + "-dialog-modal";

    $j("#" + uniqueDialog).dialog(
{
    autoOpen: false,
    height: 250,
    width: 500,
    modal: true,
    buttons: {
        "Ok": function () {
            $j(this).dialog("close");

            var rdoDeleteId = nodeID + "_rdoDelete";
            var chIdid = nodeID + "_chkdeleteAll";
            var deleteOption = document.getElementById(rdoDeleteId);
            var chkBoxObj = document.getElementById(chIdid);
            var param = deleteOption.checked + "," + chkBoxObj.checked;
            var target = nodeID;
            param = param + "," + target;
            __doPostBack("lnkDelete", param);
          
        },
        Cancel: function () { $j(this).dialog("close"); }
    }
}
        );

    $j("#" + deleteNodeID).click(function () {
        $j("#" + uniqueDialog).dialog("open");

    });

}

function enableCascadeCheck(nodeID) {
    var chIdid = nodeID + "_chkdeleteAll";
    var rdoDeleteId = nodeID + "_rdoDelete";

    if ($j('input[ID=' + rdoDeleteId + ']').is(':checked'))
        $j('input[ID=' + chIdid + ']').attr("disabled", false);
    else
        $j('input[ID=' + chIdid + ']').attr("disabled", true);
}
