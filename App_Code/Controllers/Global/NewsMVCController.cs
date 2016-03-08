using System.Data;
using System.Web.Mvc;

using CMS.CMSHelper;
using CMS.DocumentEngine;
using CMS.URLRewritingEngine;

namespace CMS.Controllers.Global
{
    /// <summary>
    /// Sample controller for the news.
    /// </summary>
    public class NewsMVCController : Controller
    {
        /// <summary>
        /// Process the detail action.
        /// </summary>
        public ActionResult Detail()
        {
            // Prepare the data for view
			TreeNode document = TreeHelper.GetDocument(CMSContext.CurrentSiteName, "/Dev/Users/TMTest2011-tgtmanager/Brandon-IP-test/" + RouteData.Values["id"], CMSContext.PreferredCultureCode, true, "thinkgate.InstructionalPlan", true);

			document = TreeHelper.GetDocument(CMSContext.CurrentSiteName, "/Dev/Users/TMTest2011-tgtmanager/Brandon-IP-test/", CMSContext.PreferredCultureCode, true, "thinkgate.InstructionalPlan", true);

			var documentList = TreeHelper.GetDocuments(CMSContext.CurrentSiteName, "/Dev/Users/TMTest2011-tgtmanager/Brandon-IP-test/%", CMSContext.PreferredCultureCode, true, "thinkgate.InstructionalPlan", null, null, -1, true, 0);
			
			if (document != null)
            {
                document.SetValue("NewsTitle", document.GetValue("NewsTitle"));
                ViewData["Document"] = document;
            }
            else
            {
                // Document not found
                URLRewriter.PageNotFound();
                return null;
            }

            return View();
        }


        /// <summary>
        /// Process the list action.
        /// </summary>
        public ActionResult List()
        {
            // Prepare the data for view
			var documents = TreeHelper.GetDocuments(CMSContext.CurrentSiteName, "/Dev/Users/TMTest2011-tgtmanager/%", CMSContext.PreferredCultureCode, true, "thinkgate.InstructionalPlan", null, null, -1, true, 0);
            if (documents != null)
            {
                ViewData["NewsList"] = documents;
            }

            return View();
        }
    }
}