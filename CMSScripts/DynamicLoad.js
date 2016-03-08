// dynamically load any javascript file.
load.getScript = function (filename) {
	var script = document.createElement('script')
	script.setAttribute("type", "text/javascript")
	script.setAttribute("onreadystatechange", "DOMLoaded()")
	script.setAttribute("onload", "DOMLoaded()")
	script.setAttribute("src", filename)
	if (typeof script != "undefined")
		document.getElementsByTagName("head")[0].appendChild(script)
}

function DOMLoaded() {

	// do jQuery(or whatever script you load) stuff here
};
