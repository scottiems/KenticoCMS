<%@ Control Language="C#" AutoEventWireup="true" CodeFile="TimelineControl.ascx.cs" Inherits="CMSWebParts_ThinkgateWebParts_TimelineControl" %>

	<style type="text/css">
		body {
			font-size: 10pt;
			font-family: verdana, sans, arial, sans-serif;
		}
	</style>

	<script type="text/javascript">
		var timeline;
		var data;
		var timelineData;
		var options;

		// Format given date as "mm/dd/yyyy hh:ii:ss"
		// @param datetime   A Date object.
		function dateFormat(date) {
			var datetime =
					((date.getMonth()   <  9) ? "0" : "") + (date.getMonth() + 1) + "/" +
					((date.getDate()    < 10) ? "0" : "") +  date.getDate() + "/" +
					date.getFullYear();// + " " +
					//((date.getHours()   < 10) ? "0" : "") +  date.getHours() + ":" +
					//((date.getMinutes() < 10) ? "0" : "") +  date.getMinutes() + ":" +
					//((date.getSeconds() < 10) ? "0" : "") +  date.getSeconds();
			return datetime;
		}

		function saveTimeline(){

			var data = timeline.data;
			var array = new Array();

			for (var i = 0; i < data.length; i++) {
				var timelineObj = new Object();

				timelineObj.scheduleInstanceID = data[i].scheduleInstanceID;
				timelineObj.scheduleTypeID = data[i].scheduleTypeID;
				timelineObj.scheduleLevelID = data[i].scheduleLevelID;
				timelineObj.documentID = data[i].documentID;
				timelineObj.start = data[i].start;
				timelineObj.startDate = new Date(data[i].start);
				timelineObj.end = data[i].end;
				timelineObj.endDate = new Date(data[i].end);
				timelineObj.duration = data[i].duration;
				//timelineObj.content = data[i].content;  //Breaks JSON parse
				timelineObj.nodeAlias = data[i].nodeAlias;
				timelineObj.aliasPath = data[i].aliasPath;
				timelineObj.nodeParent = data[i].nodeParent;
				timelineObj.nodeLevel = data[i].nodeLevel;
				//timelineObj.group = data[i].group;        //Breaks JSON parse
				timelineObj.className = data[i].className;
				timelineObj.documentForeignKeyValue = data[i].documentForeignKeyValue;

				array.push(timelineObj);
			}

			var onTestPage = "<%= docIdURLParm %>";
			var savePostURL = "./btWebServices.aspx/saveTimelineData";

			if(onTestPage == ""){
				savePostURL = "~/CMSWebParts/ThinkgateWebParts/btWebServices.aspx/saveTimelineData";
			}

			$j.ajax({
				type: "POST",
				url: savePostURL,
				data: "{'data':'" + JSON.stringify(array) + "'}",
				contentType: "application/json; charset=utf-8",
				dataType: "json",

				error: function (XMLHttpRequest, textStatus, errorThrown) {
					alert(textStatus + "\n" + errorThrown);
				},
				success: function (result) {
					//alert("result: "+ JSON.stringify(result));
					//rerun pageLoad to 're-sort' the events
					console.log("Result message: " + result.d.Message);
					// I'm sure there's a better way, but for now this will do...
					location.reload();
				}
			});
			//alert("end saveTimeline");
		}

		function clearVars(){
			$j('#setTimelineRangeBtn').val("Set Range");
			$j('#setTimelineRangeBtn').attr('disabled','disabled');
			$j('#theItem').html("");
			$j('#startDate').val("");
			$j('#endDate').val("");
		}

		// Called on body load.
		function drawVisualization() {
			// Create a JSON data table
			/*
				'start': 		new Date(2010, 7, 15),
				'end': 			new Date(2010, 8, 2),	// Optional: a field 'end'
				'content': 		'Trajectory A',
				'group': 		'Group name',			// Optional: a field 'group'
				'className': 	'red',					// Optional: a field 'className'
				'editable': 	true					// Optional: a field 'editable'  -- Boolean
			*/
						
			// specify options
			options = {
				'width': '100%',
				//'height': '300px',					// default is 'auto'
				'start': ((new Date()).valueOf()
					- new Date(1000*60*60*24*5)),		// set the start of the view to current date/time - 5 days
				'end': new Date(1000*60*60*24*30
					+ (new Date()).valueOf()),			// set the end of the view to now + 30 days
				//'cluster': true,						// Group items together as a single item when zooming
				'snapEvents': true,
				'dragAreaWidth': 20,
				'editable': true,						// enable dragging and editing events
				'style': 'box',
				'stackEvents': true,					// stack events - does not work when using groups
				'showButtonNew': false,					// show add new item button
				'eventMargin': 6,						// minimal margin between events
				'eventMarginAxis': 0,					// minimal margin between events and the axis
				//'min': new Date(2012, 0, 1),			// lower limit of visible range
				//'max': new Date(2012, 11, 31),		// upper limit of visible range
				'zoomMin': 1000 * 60 * 60 * 24 * 7,		// 7 days in milliseconds
				'zoomMax': 1000 * 60 * 60 * 24 * 31 * 3,// about three months in milliseconds
				'showNavigation':true,					// show the navigation buttons
				'groupsChangeable' : false				// allow moving of events between groups
			};

			// Instantiate our timeline object.
			timeline = new links.Timeline(document.getElementById('mytimeline'));

			/* ----------------- START timeline event handlers ----------------- */
			//links.events.addListener(timeline, 'rangechanged', onRangeChanged);
			links.events.addListener(timeline, 'change', onchange);
			links.events.addListener(timeline, 'select', onselect);
			/* ----------------- END timeline event handlers ----------------- */

			/* ----------------- START timeline event handlers functions ----------------- */
			//function onRangeChanged(properties) {
			//	document.getElementById('info').innerHTML += 'rangechanged ' + properties.start + ' - ' + properties.end + '<br>';
			//}

			// callback function for the change event
			function onchange(){
				var sel = timeline.getSelection();
				if (sel.length) {
					if (sel[0].row != undefined) {
						var row = sel[0].row;

						$j('#theItem').html(timeline.data[row].content);
						$j('#startDate').val(dateFormat(timeline.data[row].start));
						$j('#endDate').val(dateFormat(timeline.data[row].end));
					}
				}
			}

			function onselect() {
				var sel = timeline.getSelection();

				if (sel.length) {
					if (sel[0].row != undefined) {
						var row = sel[0].row;

						$j('#setTimelineRangeBtn').removeAttr('disabled');
						$j('#theItem').html(timeline.data[row].content);
						$j('#setTimelineRangeBtn').val("Set Range for - " + $j('#theItem').text());
						$j('#startDate').val(dateFormat(timeline.data[row].start));
						$j('#endDate').val(dateFormat(timeline.data[row].end));

						/*
							//build dynamic tooltip
							var theSelectedItem = $j(document).find(".timeline-event-selected");
							var tooltipText = "<strong>blah</strong> test - sel: " +  JSON.stringify(timeline.data[row]);
							//theSelectedItem.attr('title',tooltipText);
							theSelectedItem.tooltip({
								'selector': '',
								'title': tooltipText,
								'placement': 'bottom',
								'html': true
							});

							theSelectedItem.tooltip('show');
						*/
					}
				}
			}
			/* ----------------- END timeline event handlers functions ----------------- */

			//Get the timline JSON object
			var timelineData = <%= timelineDataValue %>;

			// Draw our timeline with the created data and options
			timeline.draw(timelineData, options);

		}

		$j(document).ready(function() {
			initCalls();
		});

		function initCalls(){
			clearVars();
			drawVisualization();
		}


		// adjust start and end time.
		function setRange() {
			if (!timeline) return;

			var sel = timeline.getSelection();

			if (sel.length) {
				if (sel[0].row != undefined) {
					var row = sel[0].row;
				    var startData = $j('#startDate').val().split("/");
				    var endDate = $j('#endDate').val().split("/");

				    var newStart = new Date(startData[2], parseInt(startData[0], 10) - 1, startData[1]);
				    var newEnd = new Date(endDate[2], parseInt(endDate[0], 10) - 1, endDate[1]);

					if(newStart < newEnd){
						timeline.data[row].start = newStart;
						timeline.data[row].end = newEnd;
					}else{
						alert("Please make sure the Start Date is before the End Date");
					}
					clearVars();
					timeline.redraw();
				}
			}
		}

	</script>

<body>
	<br /><br /><br />
	<div id="timelineInfoDiv1" class="" style="padding: 0em 1em;">
		Item: <span id="theItem"></span>
		<p>
			Start Date: <input type="text" id="startDate" value="">
			End Date: <input type="text" id="endDate" value="">
			<input type="button" id="setTimelineRangeBtn" class="btn btn-success" value="Set Range" disabled onclick="setRange();"/>
		</p>
	</div>

	<% 
		var tgnode2 = CMS.CMSHelper.TreeHelper.SelectSingleNode(int.Parse(DocumentID));
		var tguser2 = CMS.CMSHelper.CMSContext.CurrentUser;

		if (CMS.DocumentEngine.TreeSecurityProvider.IsAuthorizedPerNode(tgnode2, CMS.DocumentEngine.NodePermissionsEnum.Delete, tguser2) == CMS.DocumentEngine.AuthorizationResultEnum.Allowed)
			{
	%>
			<div>
				<p>
					<input type="button" id="timelineUpdateBtn" class="btn btn-success" value="Save Plan Timeline" onclick="saveTimeline();"/>
				</p>
			</div>
	<%
			} 
	%>

	<div id="<%= ID %>-<%= DocumentID %>-timeline" class="" >
		<div id="mytimeline"></div>
		<div id="info"></div>
	</div>
</body>