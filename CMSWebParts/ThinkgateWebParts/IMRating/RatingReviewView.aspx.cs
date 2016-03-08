using System;
using System.Activities.Expressions;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Business;
using Classes.Rating;
using CMS.CMSHelper;
using System.Data;
using Telerik.Web.UI;

namespace CMSWebParts.ThinkgateWebParts.IMRating
{
    public partial class CMSWebParts_ThinkgateWebParts_IMRating_RatingReviewView : Page
    {

        private RatingBusiness _ratingBusiness;
        private int _nodeId;
        private DataTable existingReview;
        private Review _review;


        protected void Page_Init(object sender, EventArgs e)
        {
            if (!this.ClientScript.IsClientScriptBlockRegistered(this.GetType(), "ApplicationVirtualPath"))
            {
                string script = "var applicationVirtualPath = '" + Request.ApplicationPath + "';";
                this.ClientScript.RegisterClientScriptBlock(this.GetType(), "ApplicationVirtualPath", script, true);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Initialize();

            if (!Page.IsPostBack)
            {
                GetData();
                SetRoleListAccess();
                SetControls();
            }

            if (Request.Form["__EVENTTARGET"] != null && !string.IsNullOrEmpty(Request.Form["__EVENTTARGET"]))
            {
                RaiseEventByTarget(Request.Form["__EVENTTARGET"]);
            }

        }
        /// <summary>
        /// Initialize the instances and get the respective itemId from query string
        /// </summary>
        private void Initialize()
        {
            _ratingBusiness = new RatingBusiness();
            _nodeId = Request.QueryString["nodeID"] == null ? default(int) : Convert.ToInt32(Request.QueryString["nodeID"]);
            nodeId.Value = _nodeId.ToString(CultureInfo.InvariantCulture);
            source.Value = Request.QueryString["source"] ?? string.Empty;
            actionId.Value = Request.QueryString["actionId"] ?? string.Empty;
            needRefreshSummary.Value = Request.QueryString["needRefresh"];
        }

        /// <summary>
        /// Get the event raised from javascript
        /// </summary>
        /// <param name="eventName"></param>
        private void RaiseEventByTarget(string eventName)
        {
            if (eventName == "radSubmit")
            {
                SubmitReview();
            }
        }

        /// <summary>
        /// Disables the Roles selection control if the logged in user only has one Role.  If the logged in user 
        /// has more than one Role associated with them, then the control remains Enabled
        /// </summary>
        private void SetRoleListAccess()
        {
            List<string> e3RoleList = ThinkgateKenticoHelper.GetUsersE3RoleList(CMSContext.CurrentUser);


            // Check to see if User Has multiple roles.
            if (e3RoleList != null)
            {

                ddlRoles.DataSource = e3RoleList;
                ddlRoles.DataBind();
                ddlRoles.Enabled = e3RoleList.Count > 1;

                // Set up initial value if available.
                if (existingReview != null && existingReview.Rows.Count>0 && existingReview.Rows[0]["RoleName"] != null)
                {
                    ddlRoles.SelectedItem.Text = existingReview.Rows[0]["RoleName"].ToString();
                }

            }


        }

        /// <summary>
        /// Gets all the data is a review has already been provided by the user.
        /// </summary>
        private void GetData()
        {
            existingReview = _ratingBusiness.GetExistingReview(_nodeId, CMSContext.CurrentUser.UserID);
        }

        /// <summary>
        /// Set controls intial values. 
        /// </summary>
        private void SetControls()
        {
            if (existingReview != null && existingReview.Rows.Count>0)
            {
                SetComment();
                SetAgeDropDown();
                SetGradeDropDown();
                SetRating();
                SetSpecialPopulation();
            }
            else
            {
                SetInitialRules();
            }
        }

        /// <summary>
        /// Set the inital rule for the checkbox 
        /// </summary>
        private void SetInitialRules()
        {
            chkNoRating.Checked = true;
        }

        /// <summary>
        /// Set the comment
        /// </summary>
        private void SetComment()
        {
            if (existingReview.Rows[0]["Comment"] != null)
                txtReivew.Text = existingReview.Rows[0]["Comment"].ToString();
        }
        /// <summary>
        /// Set Age Dropdown Value
        /// </summary>
        private void SetAgeDropDown()
        {
            if (existingReview.Rows[0]["Age"] != null)
            {
                specialPopulationUC.Age = Convert.ToInt32(existingReview.Rows[0]["Age"]);
            }
        }

        /// <summary>
        /// Set Grade Dropdown Value
        /// </summary>
        private void SetGradeDropDown()
        {
            if (existingReview.Rows[0]["Grade"] != null)
            {
                specialPopulationUC.Grade = existingReview.Rows[0]["Grade"].ToString();
            }
        }


        /// <summary>
        /// Set the rating control and No rating checkbox
        /// </summary>
        private void SetRating()
        {
            if (existingReview.Rows[0]["Rating"] != null)
            {
                decimal ratingValue = Convert.ToDecimal(existingReview.Rows[0]["Rating"]);
                if (ratingValue == -1)
                {
                    chkNoRating.Checked = true;
                    rating.ReadOnly = true;
                }
                else
                {
                    rating.Value = Convert.ToInt32(ratingValue);
                }
            }
        }


        /// <summary>
        /// Set special population control
        /// </summary>
        private void SetSpecialPopulation()
        {
            specialPopulationUC.SpecialPopulations = new List<int>();
            foreach (DataRow row in existingReview.Rows)
            {
                if (!string.IsNullOrEmpty(row["SpecialPopulationID"].ToString()))
                {
                   specialPopulationUC.SpecialPopulations.Add(Convert.ToInt32(row["SpecialPopulationID"]));
                }
            }
        }


        /// <summary>
        /// Save the review details
        /// </summary>
        private void SubmitReview()
        {
          
            BuildReview();
            _ratingBusiness.SaveReview(_review);
            CloseAndRefreshDialog();
        }

        private void BuildReview()
        {
            List<int> specialPopulationSelected = new List<int>();
            foreach (string itemId in from RepeaterItem repeaterItem in ((Repeater)specialPopulationUC.FindControl("rptSpecialPopulation")).Items
                                      select repeaterItem.FindControl("chkSpecialPopulation") as RadButton
                                          into chkSpecialPopulation
                                          where chkSpecialPopulation != null &&
                                                chkSpecialPopulation.Checked
                                          select chkSpecialPopulation.Attributes["ItemID"])
            {
                specialPopulationSelected.Add(Convert.ToInt32(itemId));
            }
            
            _review = new Review
            {
                UserID = CMSContext.CurrentUser.UserID,
                NodeID = _nodeId,
                Age = ((DropDownList)specialPopulationUC.FindControl("ddlAge")).SelectedValue == "Select Age" ? 0 : Convert.ToInt32(((DropDownList)specialPopulationUC.FindControl("ddlAge")).SelectedValue),
                Grade = ((DropDownList)specialPopulationUC.FindControl("ddlGrade")).SelectedValue == "Select Grade" ? string.Empty : ((DropDownList)specialPopulationUC.FindControl("ddlGrade")).SelectedValue,
                Comment = txtReivew.Text,
                Rating = chkNoRating.Checked ? -1 : rating.Value,
                SpecialPopulationIDs = specialPopulationSelected,
                RoleName = ddlRoles.SelectedValue
         
            };

        }
        private void CloseAndRefreshDialog()
        {
            ScriptManager.RegisterStartupScript(Page, typeof(Page), "closeDialog", "closeAndRefresh();", true);
        }
    }
}