using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class CMSWebParts_ThinkgateWebParts_TestControl : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
		string docid = Request.QueryString["docid"];

		if (!string.IsNullOrEmpty(docid))
		{
			TimelineControl.DocumentID = docid;
		}
		else
		{
			throw new Exception("Please pass the 'docid' URL parm, i.e. ?docid=1938");
		}

		
    }
}