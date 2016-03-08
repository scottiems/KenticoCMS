using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Linq;
using CMS.CMSHelper;
using CMS.DocumentEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using CMS.GlobalHelper;
using Thinkgate.Services.Contracts.Kentico;
using Thinkgate.Services.Contracts.Shared;
using Thinkgate.Services.Contracts.Scheduling;

public partial class CMSWebParts_ThinkgateWebParts_TimelineControl : System.Web.UI.UserControl
{
	
	private static int DEFALUT_CLASS_LENGTH_IN_MINUTES = 50;
	private static string CLASS_LENGTH_IN_MINUTES_COOKIE = "classLengthInMinutes_cookie";

	#region "Public Properties"

	public string DocumentID { get; set; }
    public string DocumentRootURL { get; set; }
    public string docIdURLParm { get; set; }
    protected string timelineDataValue { get { return getScheduleDetails(this.DocumentID, this.DocumentRootURL); } }

	protected int classLengthInMiniutes { get { return getClassLength(); } }

    protected void Page_Load(object sender, EventArgs e)
    {
        //classLengthInMinutes = getSetClassLength(DEFALUT_CLASS_LENGTH_IN_MINUTES);

        docIdURLParm = string.Empty;
        if (!string.IsNullOrEmpty(Request.QueryString["docid"]))
        {
            docIdURLParm = Request.QueryString["docid"];
        }
    }

	#endregion

	/// <summary>
	/// Gets the schedule details.
	/// </summary>
	/// <param name="docID">The doc ID.</param>
	/// <returns></returns>
	private string getScheduleDetails(string docID, string docRootUrl)
	{
		string timelineSchedules = "[]";

		var prxy = new TimelineServiceProxy();

		try
		{
			ScheduleDetails details = prxy.GetScheduleDetails(docID, ThinkgateKenticoHelper.FindDBName(), getClassLength(), docRootUrl);
			string msg = details.Status.Message;
			timelineSchedules = details.TimelineSchedules;

		}
		catch (Exception ex)
		{
			string msg = ex.Message;
		}
		finally
		{
			prxy = null;
		}
		return timelineSchedules;
	}

	/// <summary>
	/// Gets the length of the set class.
	/// </summary>
	/// <param name="classLengthInMinutes">The class length in minutes.</param>
	/// <returns></returns>
	private static int getClassLength()
	{
		int num;
		int classLength;

		if (!CookieHelper.RequestCookieExists(CLASS_LENGTH_IN_MINUTES_COOKIE))
		{
			classLength = int.TryParse(ThinkgateKenticoHelper.getParmValue("ClassLengthInMinutes"), out num) ? num : DEFALUT_CLASS_LENGTH_IN_MINUTES;
			CookieHelper.SetValue(CLASS_LENGTH_IN_MINUTES_COOKIE, classLength.ToString(), "/", DateTime.Now.AddHours(1));
		}
		else
		{
			classLength = int.TryParse(CookieHelper.GetValue(CLASS_LENGTH_IN_MINUTES_COOKIE), out num) ? num : DEFALUT_CLASS_LENGTH_IN_MINUTES;
		}
		return classLength;
	}

}
