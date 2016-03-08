using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml.Linq;
using CMS.CMSHelper;
using CMS.FormControls;
using CMS.DataEngine;
using CMS.SettingsProvider;
using CMS.UIControls;
using Enrich;
using Newtonsoft.Json;
using Thinkgate.Services.Contracts.CommonService;

// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
public partial class CMSFormControls_Thinkgate_EnrichViewUrl : FormEngineUserControl
// ReSharper restore InconsistentNaming
{

    #region Constants
    private const string ErrorSubmissionToEnrichFailed = "ERROR - Submission to Enrich Failed";
    private const string MtssCareDiscussionColumnName = "MTSS_CARE_Discussion";
    private const string MtssCareRecommendationsColumnName = "MTSS_CARE_Recommendations";
    private const string MtssCareEvaluationDateColumnName = "MTSS_CARE_Evaluation_Date";
    private const string BizformClassName = "BizForm.MTSSCAREOutcomesAndRecomendations";
    private const string PrimaryKeyColumnName = "MTSSCAREOutcomesAndRecomendationsID";
    private const string SubmitDateColumnName = "EnrichSubmitDate";
    private const string ViewUrlColumnName = "EnrichViewUrl";
    private const string ViewUrlDisplayText = "Click here to view the Enrich form";
    private const string MtssCareEvaluationColumnName = "MTSS_CARE_Evaluation";
    private const string ItemDefIdParmName = "ItemDefID";
    private const string OutcomeIdName = "OutcomeID";
    private const string ReasonIdName = "ReasonId";
    private const string Q1Name = "Q1";
    private const string Q2Name = "Q2";
    private const string ApiKeyName = "api_key";
    private const string ReferenceReason = "RefReas";
    private const string EnrichApiEndpointName = "EnrichApiEndpointName";
    private const string EnrichAuditLogMessage = "Created by Thinkgate - MTSS CARE Form";
    #endregion

    #region Private Init Properties
    /// <summary>
    /// Indicates if the postback for this control was triggered by the Enrich button
    /// </summary>
    private bool EnrichSubmitButtonIsPostBackTrigger { get; set; }

    /// <summary>
    /// Indicates if submission to Enrich is allowed
    /// </summary>
    private bool CanSubmitToEnrich { get; set; }

    /// <summary>
    /// Holds the date that the submission was made to Enrich for the form this control is sited on
    /// </summary>
    private string EnrichSubmissionDate { get; set; }

    /// <summary>
    /// Indicates if the form the control is sited on is readonly
    /// </summary>
    private bool IsViewOnlyMode
    {
        get
        {
            if (!Request.QueryString.AllKeys.Contains("viewmode"))
                return false;

            return Request.QueryString["viewmode"] == "3";
        }
    }
    #endregion

    #region Session State Properties
    /// <summary>
    /// Allows ascx control reporting of any Enrich submission errors
    /// </summary>
    public string EnrichErrors
    {
        get
        {
            return GetFormPrimaryKey() == 0
                ? string.Empty
                : TransformObjectToString(Session[GetFormPrimaryKey() + "_Enrich_EnrichErrors"]);
        }
        set
        {
            if (GetFormPrimaryKey() == 0)
                return;

            Session[GetFormPrimaryKey() + "_Enrich_EnrichErrors"] = value;
        }
    }

    /// <summary>
    /// Allows ascx control reporting of any Enrich validation errors
    /// </summary>
    public string EnrichValidationErrors
    {
        get
        {
            return GetFormPrimaryKey() == 0
                ? string.Empty
                : TransformObjectToString(Session[GetFormPrimaryKey() + "_Enrich_ValidationErrors"]);
        }
        set
        {
            if (GetFormPrimaryKey() == 0)
                return;

            Session[GetFormPrimaryKey() + "_Enrich_ValidationErrors"] = value;
        }
    }
    //_Enrich_ValidationErrors
    #endregion

    #region Used to set html control properties during rendering process
    /// <summary>
    /// The label showing the user the submission date to Enrich
    /// </summary>
    public string SubmitToEnrichDate { get; set; }

    /// <summary>
    /// Sets the disabled attribute on the Enrich submit button to prevent multiple Enrich submissions
    /// </summary>
    public string EnrichSubmissionDisabledAttribute
    {
        get
        {
            // forcing another data update as the primary data for the form is routinely reset on us by Kentico
            InitEnrichData(true);
            return CanSubmitToEnrich ? string.Empty : "disabled";
        }
    }

    /// <summary>
    /// Gets the label text for the hyperlink control, but only if a Url exists
    /// </summary>
    public string EnrichViewUrlLabelText
    {
        get
        {
            if (Form.Data.GetValue(PrimaryKeyColumnName) != null)
                InitEnrichData(true);

            return !string.IsNullOrEmpty(Value.ToString()) ? ViewUrlDisplayText : string.Empty;
        }
    }

    /// <summary>
    /// Used to hide the submit to enrich button whenever the form is displayed in view only mode
    /// </summary>
    public string EnrichButtonVisibilityAttribute
    {
        get
        {
            return IsViewOnlyMode ? "display:none;" : string.Empty;
        }
    }
    #endregion

    #region Overrides
    /// <summary>
    /// Gets or sets form control value (in our case it's the Enrich View Url).
    /// </summary>
    public override object Value
    {
        get
        {
            return GetFormValue(Form.Data, ViewUrlColumnName);
        }
        set { Form.Data[ViewUrlColumnName] = value; }
    }

    /// <summary>
    /// Hook into the form events;
    /// Capture after save and before data load events
    /// </summary>
    /// <param name="e"></param>
    protected override void OnInit(EventArgs e)
    {
        Form.OnAfterSave += OnAfterFormSave;
        Form.OnAfterDataLoad += OnAfterFormDataLoad;

        base.OnInit(e);
    }

    #endregion

    #region Kentico Parent Control Events
    private void OnAfterFormSave(object sender, EventArgs e)
    {

        ValidateAndSubmitToEnrich();

        // force re-init of data now that we've changed it;
        // The values written on the ascx page are driven by the properties populated by this method
        InitEnrichData(true);

    }

    /// <summary>
    /// Fires whenever there's a postback and data is either: 
    /// a) Retrieved from the database on form load
    /// b) What came back from the form postback that WILL be updated to the database
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnAfterFormDataLoad(object sender, EventArgs e)
    {
        InitEnrichData();
    }
    #endregion

    #region Helper Functions
    /// <summary>
    /// The Kentico API will either return a null, empty string, or a date equal to DateTime.MinValue;
    /// All of which means that its not been set yet and we should return out an empty string to prevent mass confusion
    /// </summary>
    /// <returns></returns>
    private string GetEnrichSubmitDate()
    {
        var temp = GetFormValue(Form.Data, SubmitDateColumnName);
        if (string.IsNullOrWhiteSpace(temp))
            return temp;

        DateTime enrichSubmitDate;
        if (DateTime.TryParse(temp, out enrichSubmitDate))
        {
            return enrichSubmitDate == DateTime.MinValue ? string.Empty : enrichSubmitDate.ToShortDateString();
        }

        return string.Empty;
    }

    /// <summary>
    /// Build the text needed to populate the Resason field for the Enrich API
    /// </summary>
    /// <param name="dataContainer"></param>
    /// <returns></returns>
    private string GetReasonText(ISimpleDataContainer dataContainer)
    {
        return string.Format("{0}{1}{1}{2}", GetFormValue(dataContainer, MtssCareDiscussionColumnName), Environment.NewLine,
            GetFormValue(dataContainer, MtssCareRecommendationsColumnName));
    }

    /// <summary>
    /// Disable everything on the ENTIRE form - read only mode
    /// </summary>
    private void DisableForm()
    {
        Form.Enabled = false;

        if (Form == null || Form.Page == null)
            return;

        var headerActions = ((AbstractCMSPage)Form.Page).HeaderActions;
        if (headerActions != null)
            headerActions.Visible = false;
    }

    /// <summary>
    /// Set values on the response that indicates an error condition
    /// </summary>
    /// <returns></returns>
    private EnrichResponse CreateErrorResponse()
    {
        return new EnrichResponse
        {
            Id = string.Empty,
            ViewUrl = "CRITICAL ERROR - NO RESPONSE RETURNED FROM ENRICH API"
        };
    }

    /// <summary>
    /// Return a date as Round-trip "0" format; ISO 8601;
    /// See: http://msdn.microsoft.com/en-us/library/az4se3k1(v=vs.110).aspx#Roundtrip
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    private string GetDateAsRoundTripFormat(object date)
    {
        if (date == null)
            return string.Empty;

        DateTime tempDate;
        if (DateTime.TryParse(date.ToString(), out tempDate))
            return tempDate.ToString("O");

        return string.Empty;

    }

    /// <summary>
    /// Take an unknown type that can be null and return its string representation
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    private string TransformObjectToString(object value)
    {
        return value == null ? string.Empty : value.ToString();
    }

    /// <summary>
    /// Get the client Id for the given 
    /// </summary>
    /// <returns></returns>
    private string GetClientId()
    {
        return ThinkgateKenticoHelper.getClientIDFromKenticoUserName(CMSContext.CurrentUser.UserName);
    }

    /// <summary>
    /// For a given column name and data container, get the value
    /// </summary>
    /// <param name="dataContainer"></param>
    /// <param name="columnName"></param>
    /// <returns></returns>
    private string GetFormValue(ISimpleDataContainer dataContainer, string columnName)
    {
        return TransformObjectToString(dataContainer[columnName]).Trim();
    }

    /// <summary>
    /// Get the primary key
    /// </summary>
    /// <returns></returns>
    private int GetFormPrimaryKey()
    {
        var tempId = TransformObjectToString(Form.Data.GetValue(PrimaryKeyColumnName));
        return string.IsNullOrWhiteSpace(tempId) ? 0 : int.Parse(tempId);
    }

    /// <summary>
    /// Get the Enrich Web request; 
    /// Builds out and returns the correctly configured request headers and writes the post data to the request stream
    /// </summary>
    /// <param name="json"></param>
    /// <param name="clientParms"></param>
    /// <returns></returns>
    private HttpWebRequest GetWebRequest(string json, Dictionary<string, string> clientParms)
    {
        var enrichApiUri = new Uri(clientParms.First(x => x.Key == EnrichApiEndpointName).Value);

        var httpWebRequest = WebRequest.Create(enrichApiUri) as HttpWebRequest;
        if (httpWebRequest == null)
            return null;

        httpWebRequest.ContentType = "application/json; charset=UTF-8";
        httpWebRequest.Method = "POST";
        httpWebRequest.Accept = "*/*";
        httpWebRequest.KeepAlive = false;
        httpWebRequest.Headers.Add(ApiKeyName, clientParms.First(x => x.Key == ApiKeyName).Value);

        var bytes = Encoding.ASCII.GetBytes(json);
        httpWebRequest.ContentLength = bytes.Length;
        var requestStream = httpWebRequest.GetRequestStream();
        requestStream.Write(bytes, 0, bytes.Length); //Push it out there
        requestStream.Close();

        return httpWebRequest;
    }

    /// <summary>
    /// Builds and returns the EnrichRequest object; Will be used later as source for json
    /// </summary>
    /// <param name="sarasotaStudentNumber"></param>
    /// <param name="dateOfReferral"></param>
    /// <param name="reasonText"></param>
    /// <param name="clientParms"></param>
    /// <returns></returns>
    private EnrichRequest GetEnrichRequest(string sarasotaStudentNumber, string dateOfReferral, string reasonText, Dictionary<string, string> clientParms)
    {
        var enrichRequest = new EnrichRequest
        {
            Student = new Student { LocalNumber = sarasotaStudentNumber },
            StartDate = dateOfReferral,
            EndDate = dateOfReferral,
            ItemDefID = clientParms.First(x => x.Key == ItemDefIdParmName).Value,
            OutcomeID = clientParms.First(x => x.Key == OutcomeIdName).Value,
            ReasonId = clientParms.First(x => x.Key == ReasonIdName).Value,
            ProgramVariantID = null,
            CustomFormSections =
                new List<CustomFormSection>
                {
                    new CustomFormSection
                    {
                        Code = "SPEDReferral",
                        Fields =
                            new Dictionary<string, string>
                            {
                                {ReferenceReason, reasonText},
                                {Q1Name, clientParms.First(x => x.Key == Q1Name).Value},
                                {Q2Name, clientParms.First(x => x.Key == Q2Name).Value}
                            }
                    }
                },
            AuditLogMessage = EnrichAuditLogMessage
        };
        return enrichRequest;
    }
    #endregion

    #region Service Calls to E3
    /// <summary>
    /// Reach out to the common service and get the properties related to Enrich
    /// </summary>
    /// <returns></returns>
    private Dictionary<string, string> GetClientParms()
    {
        return new CommonProxy().GetAllProperties(GetClientId()).ClientParms;
    }

    /// <summary>
    /// Service call to E3 to determine if a given student has an IEP
    /// </summary>
    /// <param name="studentNumber"></param>
    /// <returns></returns>
    private bool GetStudentHasAnIep(string studentNumber)
    {
        var studentHasIep = false;
        try
        {
            using (var client = new WebClient())
            {
                client.Headers.Clear();
                client.Headers.Add(HttpRequestHeader.ContentType, "application/x-www-form-urlencoded");

                var cookieHeaderString = Page.Request.Headers["cookie"];
                client.Headers.Add(HttpRequestHeader.Cookie, cookieHeaderString);
                var url = string.Format("{0}://{1}/{2}/Services/EnrichApi.asmx/StudentHasIep", Request.Url.Scheme,
                    Request.Url.Host, GetClientId());
                var e3ServerUri = new Uri(url);
                var rawResponse =
                    client.UploadString(e3ServerUri, string.Format("studentNumber={0}", studentNumber));

                if (string.IsNullOrWhiteSpace(rawResponse))
                    throw new Exception("The server response was an empty string");

                var responseXml = XElement.Parse(rawResponse);

                studentHasIep = (string.Compare(true.ToString(), responseXml.Value, StringComparison.OrdinalIgnoreCase) == 0);
            }
        }
        catch (Exception ex)
        {
            EnrichValidationErrors = string.Concat(EnrichErrors, Environment.NewLine, ex.ToString());
        }

        return studentHasIep;

    }
    #endregion

    /// <summary>
    /// Validate and Submit Form to Enrich
    /// </summary>
    private void ValidateAndSubmitToEnrich()
    {
        // bail out if this isn't an Enrich submission
        if (!EnrichSubmitButtonIsPostBackTrigger)
        {
            EnrichValidationErrors = string.Empty;
            EnrichErrors = string.Empty;
            return;
        }

        // submit the enrich payload
        EnrichResponse enrichResponse;
        try
        {
            // Grab what we need
            var sarasotaStudentNumber = SarasotaStudentNo.Value;
            var dateOfReferral = GetFormValue(Form.Data, MtssCareEvaluationDateColumnName);
            var summaryText = GetFormValue(Form.Data, MtssCareDiscussionColumnName);
            var recommendationText = GetFormValue(Form.Data, MtssCareRecommendationsColumnName);
            var reasonText = GetReasonText(Form.Data);
            var evaluationDetermination = GetFormValue(Form.Data, MtssCareEvaluationColumnName);

            // Validate before submission
            EnrichValidationErrors = string.Empty;
            EnrichErrors = string.Empty;
            CheckForValidationErrors(dateOfReferral, sarasotaStudentNumber, evaluationDetermination, summaryText, recommendationText);
            if (!string.IsNullOrWhiteSpace(EnrichValidationErrors))
                return;

            enrichResponse = SubmitToEnrich(sarasotaStudentNumber, dateOfReferral, reasonText);
        }
        catch (Exception)
        {
            EnrichErrors = ErrorSubmissionToEnrichFailed;
            return;
        }

        // Interogate Response
        CheckForEnrichResponseErrors(enrichResponse);
        if (!string.IsNullOrWhiteSpace(EnrichErrors))
            return;

        // save to the backing store
        try
        {
            SaveEnrichRequestInfoToForm(enrichResponse.ViewUrl);
        }
        catch (Exception ex)
        {
            EnrichErrors =
                string.Format(
                    "Critial ERROR, the submission to Enrich was successful, but saving the form failed with the following error: {0}{1}",
                    Environment.NewLine, ex);
        }
    }

    /// <summary>
    /// Check for and handle the response errors from Enrich
    /// </summary>
    /// <param name="enrichResponse"></param>
    private void CheckForEnrichResponseErrors(EnrichResponse enrichResponse)
    {
        // not sure how this could happen but let's be defensive anyway
        if (enrichResponse == null)
        {
            EnrichErrors = ErrorSubmissionToEnrichFailed;
            return;
        }

        // trap error specific to enrich and bail out
        if (!string.IsNullOrEmpty(enrichResponse.ErrorId))
        {
            EnrichErrors = string.Format("{0}{1}ErrorID:{2}{1}Error Message:{3}", ErrorSubmissionToEnrichFailed,
                "<br />", enrichResponse.ErrorId, enrichResponse.ErrorMessage);
        }
    }

    /// <summary>
    /// All Enrich form validation takes place here
    /// </summary>
    /// <param name="dateOfReferral"></param>
    /// <param name="sarasotaStudentNumber"></param>
    /// <param name="evaluationYesNo"></param>
    /// <param name="summaryText"></param>
    /// <param name="recommendationText"></param>
    private void CheckForValidationErrors(string dateOfReferral, string sarasotaStudentNumber, string evaluationYesNo, string summaryText, string recommendationText)
    {
        var validationErrors = new List<string>();

        var formatedEndDate = GetDateAsRoundTripFormat(dateOfReferral);

        if (string.IsNullOrWhiteSpace(sarasotaStudentNumber))
            validationErrors.Add("Student Number cannot be blank");

        if (string.IsNullOrWhiteSpace(evaluationYesNo) || evaluationYesNo.Trim() != "1")
            validationErrors.Add("Evaluation Determination must be 'Yes' to submit to Enrich");

        if (string.IsNullOrWhiteSpace(summaryText))
            validationErrors.Add("Summary of Discussion cannot be blank");

        if (string.IsNullOrWhiteSpace(recommendationText))
            validationErrors.Add("Recommendations cannot be blank");

        if (string.IsNullOrWhiteSpace(formatedEndDate))
            validationErrors.Add("Invalid Date of referral");
        else
        {
            var tempDate = DateTime.Parse(formatedEndDate);
            if (tempDate > DateTime.Now)
                validationErrors.Add("Invalid Date of referral - Future dates are not allowed");

            if (tempDate < DateTime.Now.AddMonths(-6))
                validationErrors.Add("Invalid Date of referral - Date is more than 6 months into the past");
        }

        if (!GetStudentHasAnIep(sarasotaStudentNumber))
            validationErrors.Add("Student does not have an IEP - IEP require for submission to Enrich");

        if (validationErrors.Any())
        {
            EnrichValidationErrors = string.Join("\\n", validationErrors);
        }

    }

    /// <summary>
    /// Save the response from Enrich to the Kentico backing store for this form
    /// </summary>
    /// <param name="enrichViewUrl"></param>
    private void SaveEnrichRequestInfoToForm(string enrichViewUrl)
    {
        var dataClass = DataClassFactory.NewDataClass(BizformClassName, GetFormPrimaryKey());

        dataClass.SetValue(SubmitDateColumnName, DateTime.Today.ToString(CultureInfo.InvariantCulture));
        dataClass.SetValue(ViewUrlColumnName, enrichViewUrl);
        dataClass.Update();
    }

    /// <summary>
    /// Init the basic values needed for submission to Enrich or those needed by this control
    /// </summary>
    /// <param name="forceRefreshFromDatastore"></param>
    private void InitEnrichData(bool forceRefreshFromDatastore = false)
    {
        if (forceRefreshFromDatastore)
        {
            var dataClass = DataClassFactory.NewDataClass(BizformClassName);
            dataClass.LoadData(GetFormPrimaryKey());

            Form.Data = dataClass;
        }

        EnrichSubmissionDate = GetEnrichSubmitDate();
        CanSubmitToEnrich = string.IsNullOrEmpty(EnrichSubmissionDate);
        EnrichSubmitButtonIsPostBackTrigger = Page.Request[IsEnrichSubmission.Name] == "1" && CanSubmitToEnrich;

        if (CanSubmitToEnrich)
        {
            SubmitToEnrichDate = string.Empty;
        }
        else
        {
            SubmitToEnrichDate = string.Format("Date Submitted to Enrich: {0}", GetEnrichSubmitDate());
            DisableForm();
        }
    }

    /// <summary>
    /// Submit the Enrich Form to the Enrich API
    /// </summary>
    /// <returns></returns>
    private EnrichResponse SubmitToEnrich(string sarasotaStudentNumber, string dateOfReferral, string reasonText)
    {

        try
        {
            var clientParms = GetClientParms();
            var enrichRequest = GetEnrichRequest(sarasotaStudentNumber, dateOfReferral, reasonText, clientParms);

            var json = JsonConvert.SerializeObject(enrichRequest);

            var httpWebRequest = GetWebRequest(json, clientParms);
            var webResponse = httpWebRequest.GetResponse();
            var responseStream = webResponse.GetResponseStream();
            if (responseStream == null)
                return CreateErrorResponse();

            var streamReader = new System.IO.StreamReader(responseStream);
            var response = streamReader.ReadToEnd().Trim();

            if (string.IsNullOrWhiteSpace(response))
                return CreateErrorResponse();

            var enrichResponse = JsonConvert.DeserializeObject<EnrichResponse>(response);

            return enrichResponse;
        }
        catch (WebException webException)
        {
            if (((HttpWebResponse)webException.Response).StatusCode != HttpStatusCode.BadRequest)
                return new EnrichResponse
                {
                    ErrorMessage = webException.Message,
                    ErrorId =
                        ((int)((HttpWebResponse)webException.Response).StatusCode).ToString(
                            CultureInfo.InvariantCulture)
                };

            var stream = webException.Response.GetResponseStream();
            var responseMessage = stream != null ? new System.IO.StreamReader(stream).ReadToEnd().Trim() : webException.Message;

            return new EnrichResponse
            {
                ErrorMessage = responseMessage,
                ErrorId = HttpStatusCode.BadRequest.ToString()
            };
        }
        catch (Exception exception)
        {
            return new EnrichResponse
            {
                ErrorMessage = exception.Message,
                ErrorId = ""
            };
        }

    }

}


