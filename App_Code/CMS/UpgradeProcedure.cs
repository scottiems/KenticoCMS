using System;
using System.Data;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;

using CMS.CMSHelper;
using CMS.CMSImportExport;
using CMS.DataEngine;
using CMS.EventLog;
using CMS.FormEngine;
using CMS.GlobalHelper;
using CMS.IO;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.URLRewritingEngine;
using CMS.WorkflowEngine;


/// <summary>
/// Class carrying the code to perform the upgrade procedure.
/// </summary>
public static class UpgradeProcedure
{
    #region "Variables"

    // path to upgrade package
    private static string mUpgradePackagePath = null;

    private static string mWebsitePath = null;

    #endregion


    #region "Methods"

    /// <summary>
    /// Runs the update procedure.
    /// </summary>
    /// <param name="conn">Connection to use</param>
    public static void Update(GeneralConnection conn)
    {
        if (SqlHelper.IsDatabaseAvailable)
        {
            try
            {
                string version = SettingsKeyProvider.GetStringValue("CMSDataVersion");
                switch (version.ToLowerCSafe())
                {
                    case "5.5r2":
                        Update60();
                        break;
                }
            }
            catch
            {
            }
        }
    }

    #endregion


    #region "Update 7.0 RC"

    public static void Update70RC()
    {
        #region "Workflow - Timeout connection"

        var allSteps = WorkflowStepInfoProvider.GetWorkflowSteps("StepType IN(2,3,9,100,101)", null, -1, "StepDefinition");
        var sourcePointDBGuids = new List<string>();
        foreach (var step in allSteps)
        {
            var sourcePoint = step.StepDefinition.SourcePoints.Find(s => s.Type == SourcePointTypeEnum.Timeout);
            if (sourcePoint != null)
            {
                sourcePointDBGuids.Add(sourcePoint.Guid.ToString());
            }
        }

        string where = SqlHelperClass.GetWhereCondition<string>("TransitionSourcePointGUID", sourcePointDBGuids, true);
        ConnectionHelper.ExecuteQuery("cms.workflowtransition.updateall", null, where, null, -1, "TransitionType = 1");

        #endregion
    }

    #endregion


    #region "Update 6.0"

    public static void Update60()
    {
        EventLogProvider evp = new EventLogProvider();
        evp.LogEvent("I", DateTime.Now, "Upgrade to 6.0", "Upgrade - Start");

        DataClassInfo dci = null;


        #region "CMS.UserSettings"

        try
        {
            dci = DataClassInfoProvider.GetDataClass("cms.usersettings");
            if (dci != null)
            {
                FormInfo fi = new FormInfo(dci.ClassFormDefinition);
                if (fi != null)
                {
                    FormFieldInfo ffi = new FormFieldInfo();
                    ffi.Name = "UserAuthenticationGUID";
                    ffi.DataType = FormFieldDataTypeEnum.GUID;
                    ffi.AllowEmpty = true;
                    ffi.PublicField = false;
                    ffi.System = true;
                    ffi.FieldType = FormFieldControlTypeEnum.LabelControl;
                    ffi.Visible = false;

                    fi.AddFormField(ffi);

                    ffi = new FormFieldInfo();
                    ffi.Name = "UserBounces";
                    ffi.DataType = FormFieldDataTypeEnum.Integer;
                    ffi.AllowEmpty = true;
                    ffi.PublicField = false;
                    ffi.System = true;
                    ffi.FieldType = FormFieldControlTypeEnum.TextBoxControl;
                    ffi.Visible = false;
                    ffi.Caption = "UserBounces";

                    fi.AddFormField(ffi);

                    ffi = new FormFieldInfo();
                    ffi.Name = "UserLinkedInID";
                    ffi.DataType = FormFieldDataTypeEnum.Text;
                    ffi.Size = 100;
                    ffi.AllowEmpty = true;
                    ffi.PublicField = false;
                    ffi.System = true;
                    ffi.FieldType = FormFieldControlTypeEnum.LabelControl;
                    ffi.Visible = false;

                    fi.AddFormField(ffi);

                    ffi = new FormFieldInfo();
                    ffi.Name = "UserLogActivities";
                    ffi.DataType = FormFieldDataTypeEnum.Boolean;
                    ffi.AllowEmpty = true;
                    ffi.PublicField = false;
                    ffi.System = true;
                    ffi.FieldType = FormFieldControlTypeEnum.LabelControl;
                    ffi.Visible = false;

                    fi.AddFormField(ffi);

                    ffi = new FormFieldInfo();
                    ffi.Name = "UserPasswordRequestHash";
                    ffi.DataType = FormFieldDataTypeEnum.Text;
                    ffi.Size = 100;
                    ffi.AllowEmpty = true;
                    ffi.PublicField = false;
                    ffi.System = true;
                    ffi.FieldType = FormFieldControlTypeEnum.LabelControl;
                    ffi.Visible = false;

                    fi.AddFormField(ffi);

                    dci.ClassFormDefinition = fi.GetXmlDefinition();

                    TableManager tm = new TableManager(dci.ClassConnectionString);
                    dci.ClassXmlSchema = tm.GetXmlSchema("CMS_UserSettings");

                    DataClassInfoProvider.SetDataClass(dci);

                    // Generate queries
                    SqlGenerator.GenerateDefaultQueries(dci, true, false);
                    tm.RefreshCustomViews("CMS_UserSettings");
                }
            }
        }
        catch (Exception ex)
        {
            evp.LogEvent("CMS.UserSettings - Upgrade", "Upgrade", ex);
        }

        #endregion


        #region "Ecommerce - Customer"

        try
        {
            dci = DataClassInfoProvider.GetDataClass("ecommerce.customer");
            if (dci != null)
            {
                FormInfo fi = new FormInfo(dci.ClassFormDefinition);
                if (fi != null)
                {
                    FormFieldInfo ffi = new FormFieldInfo();
                    ffi.Name = "CustomerSiteID";
                    ffi.DataType = FormFieldDataTypeEnum.Integer;
                    ffi.AllowEmpty = true;
                    ffi.PublicField = false;
                    ffi.System = true;
                    ffi.FieldType = FormFieldControlTypeEnum.TextBoxControl;
                    ffi.Visible = false;

                    fi.AddFormField(ffi);

                    TableManager tm = new TableManager(dci.ClassConnectionString);

                    dci.ClassFormDefinition = fi.GetXmlDefinition();
                    dci.ClassXmlSchema = tm.GetXmlSchema("COM_Customer");

                    DataClassInfoProvider.SetDataClass(dci);

                    // Generate queries
                    SqlGenerator.GenerateDefaultQueries(dci, true, false);

                    tm.RefreshCustomViews("COM_Customer");
                }
            }
        }
        catch (Exception ex)
        {
            evp.LogEvent("Ecommerce.Customer - Upgrade", "Upgrade", ex);
        }

        #endregion


        #region "Ecommerce - Order"

        try
        {
            dci = DataClassInfoProvider.GetDataClass("ecommerce.order");
            if (dci != null)
            {
                FormInfo fi = new FormInfo(dci.ClassFormDefinition);
                if (fi != null)
                {
                    FormFieldInfo ffi = new FormFieldInfo();
                    ffi.Name = "OrderCulture";
                    ffi.DataType = FormFieldDataTypeEnum.Text;
                    ffi.Size = 10;
                    ffi.AllowEmpty = true;
                    ffi.PublicField = false;
                    ffi.System = true;
                    ffi.FieldType = FormFieldControlTypeEnum.LabelControl;
                    ffi.Visible = false;

                    fi.AddFormField(ffi);

                    ffi = new FormFieldInfo();
                    ffi.Name = "OrderIsPaid";
                    ffi.DataType = FormFieldDataTypeEnum.Boolean;
                    ffi.AllowEmpty = true;
                    ffi.PublicField = false;
                    ffi.System = true;
                    ffi.FieldType = FormFieldControlTypeEnum.LabelControl;
                    ffi.Visible = false;

                    fi.AddFormField(ffi);

                    ffi = new FormFieldInfo();
                    ffi.Name = "OrderTotalPriceInMainCurrency";
                    ffi.DataType = FormFieldDataTypeEnum.Decimal;
                    ffi.AllowEmpty = true;
                    ffi.PublicField = false;
                    ffi.System = true;
                    ffi.FieldType = FormFieldControlTypeEnum.LabelControl;
                    ffi.Visible = false;

                    fi.AddFormField(ffi);

                    ffi = fi.GetFormField("OrderStatusID");
                    if (ffi != null)
                    {
                        ffi.AllowEmpty = true;
                        fi.UpdateFormField("OrderStatusID", ffi);
                    }

                    ffi = fi.GetFormField("OrderShippingAddressID");
                    if (ffi != null)
                    {
                        ffi.AllowEmpty = true;
                        fi.UpdateFormField("OrderShippingAddressID", ffi);
                    }

                    dci.ClassFormDefinition = fi.GetXmlDefinition();

                    TableManager tm = new TableManager(dci.ClassConnectionString);
                    dci.ClassXmlSchema = tm.GetXmlSchema("COM_Order");

                    DataClassInfoProvider.SetDataClass(dci);

                    // Generate queries
                    SqlGenerator.GenerateDefaultQueries(dci, true, false);
                    tm.RefreshCustomViews("COM_Order");
                }
            }
        }
        catch (Exception ex)
        {
            evp.LogEvent("Ecommerce.Order - Upgrade", "Upgrade", ex);
        }

        #endregion


        #region "Ecommerce - OrderItem"

        try
        {
            dci = DataClassInfoProvider.GetDataClass("ecommerce.orderitem");
            if (dci != null)
            {
                FormInfo fi = new FormInfo(dci.ClassFormDefinition);
                if (fi != null)
                {
                    FormFieldInfo ffi = new FormFieldInfo();
                    ffi.Name = "OrderItemBundleGUID";
                    ffi.DataType = FormFieldDataTypeEnum.GUID;
                    ffi.AllowEmpty = true;
                    ffi.PublicField = false;
                    ffi.System = true;
                    ffi.FieldType = FormFieldControlTypeEnum.LabelControl;
                    ffi.Visible = false;

                    fi.AddFormField(ffi);

                    ffi = new FormFieldInfo();
                    ffi.Name = "OrderItemIsPrivate";
                    ffi.DataType = FormFieldDataTypeEnum.Boolean;
                    ffi.AllowEmpty = true;
                    ffi.PublicField = false;
                    ffi.System = true;
                    ffi.FieldType = FormFieldControlTypeEnum.LabelControl;
                    ffi.Visible = false;

                    fi.AddFormField(ffi);

                    ffi = new FormFieldInfo();
                    ffi.Name = "OrderItemPrice";
                    ffi.DataType = FormFieldDataTypeEnum.Decimal;
                    ffi.AllowEmpty = true;
                    ffi.PublicField = false;
                    ffi.System = true;
                    ffi.FieldType = FormFieldControlTypeEnum.LabelControl;
                    ffi.Visible = false;

                    fi.AddFormField(ffi);

                    ffi = new FormFieldInfo();
                    ffi.Name = "OrderItemSendNotification";
                    ffi.DataType = FormFieldDataTypeEnum.Boolean;
                    ffi.AllowEmpty = true;
                    ffi.PublicField = false;
                    ffi.System = true;
                    ffi.FieldType = FormFieldControlTypeEnum.LabelControl;
                    ffi.Visible = false;

                    fi.AddFormField(ffi);

                    ffi = new FormFieldInfo();
                    ffi.Name = "OrderItemSKU";
                    ffi.DataType = FormFieldDataTypeEnum.LongText;
                    ffi.AllowEmpty = true;
                    ffi.PublicField = false;
                    ffi.System = true;
                    ffi.FieldType = FormFieldControlTypeEnum.LabelControl;
                    ffi.Visible = false;

                    fi.AddFormField(ffi);

                    ffi = new FormFieldInfo();
                    ffi.Name = "OrderItemText";
                    ffi.DataType = FormFieldDataTypeEnum.LongText;
                    ffi.AllowEmpty = true;
                    ffi.PublicField = false;
                    ffi.System = true;
                    ffi.FieldType = FormFieldControlTypeEnum.LabelControl;
                    ffi.Visible = false;

                    fi.AddFormField(ffi);

                    ffi = new FormFieldInfo();
                    ffi.Name = "OrderItemTotalPriceInMainCurrency";
                    ffi.DataType = FormFieldDataTypeEnum.Decimal;
                    ffi.AllowEmpty = true;
                    ffi.PublicField = false;
                    ffi.System = true;
                    ffi.FieldType = FormFieldControlTypeEnum.LabelControl;
                    ffi.Visible = false;

                    fi.AddFormField(ffi);

                    ffi = new FormFieldInfo();
                    ffi.Name = "OrderItemValidTo";
                    ffi.DataType = FormFieldDataTypeEnum.DateTime;
                    ffi.AllowEmpty = true;
                    ffi.PublicField = false;
                    ffi.System = true;
                    ffi.FieldType = FormFieldControlTypeEnum.LabelControl;
                    ffi.Visible = false;

                    fi.AddFormField(ffi);

                    dci.ClassFormDefinition = fi.GetXmlDefinition();

                    TableManager tm = new TableManager(dci.ClassConnectionString);
                    dci.ClassXmlSchema = tm.GetXmlSchema("COM_OrderItem");

                    DataClassInfoProvider.SetDataClass(dci);

                    // Generate queries
                    SqlGenerator.GenerateDefaultQueries(dci, true, false);
                    tm.RefreshCustomViews("COM_OrderItem");
                }
            }
        }
        catch (Exception ex)
        {
            evp.LogEvent("Ecommerce.OrderItem - Upgrade", "Upgrade", ex);
        }

        #endregion


        #region "Ecommerce - Shopping cart item"

        try
        {
            dci = DataClassInfoProvider.GetDataClass("ecommerce.shoppingcartitem");
            if (dci != null)
            {
                FormInfo fi = new FormInfo(dci.ClassFormDefinition);
                if (fi != null)
                {
                    FormFieldInfo ffi = new FormFieldInfo();
                    ffi.Name = "CartItemBundleGUID";
                    ffi.DataType = FormFieldDataTypeEnum.GUID;
                    ffi.AllowEmpty = true;
                    ffi.PublicField = false;
                    ffi.System = true;
                    ffi.FieldType = FormFieldControlTypeEnum.LabelControl;
                    ffi.Visible = false;

                    fi.AddFormField(ffi);

                    ffi = new FormFieldInfo();
                    ffi.Name = "CartItemIsPrivate";
                    ffi.DataType = FormFieldDataTypeEnum.Boolean;
                    ffi.AllowEmpty = true;
                    ffi.PublicField = false;
                    ffi.System = true;
                    ffi.FieldType = FormFieldControlTypeEnum.LabelControl;
                    ffi.Visible = false;

                    fi.AddFormField(ffi);

                    ffi = new FormFieldInfo();
                    ffi.Name = "CartItemPrice";
                    ffi.DataType = FormFieldDataTypeEnum.Decimal;
                    ffi.AllowEmpty = true;
                    ffi.PublicField = false;
                    ffi.System = true;
                    ffi.FieldType = FormFieldControlTypeEnum.LabelControl;
                    ffi.Visible = false;

                    fi.AddFormField(ffi);

                    ffi = new FormFieldInfo();
                    ffi.Name = "CartItemText";
                    ffi.DataType = FormFieldDataTypeEnum.LongText;
                    ffi.AllowEmpty = true;
                    ffi.PublicField = false;
                    ffi.System = true;
                    ffi.FieldType = FormFieldControlTypeEnum.LabelControl;
                    ffi.Visible = false;

                    fi.AddFormField(ffi);

                    ffi = new FormFieldInfo();
                    ffi.Name = "CartItemValidTo";
                    ffi.DataType = FormFieldDataTypeEnum.DateTime;
                    ffi.AllowEmpty = true;
                    ffi.PublicField = false;
                    ffi.System = true;
                    ffi.FieldType = FormFieldControlTypeEnum.LabelControl;
                    ffi.Visible = false;

                    fi.AddFormField(ffi);

                    ffi = fi.GetFormField("CartItemGuid");
                    if (ffi != null)
                    {
                        ffi.AllowEmpty = true;
                        fi.UpdateFormField("CartItemGuid", ffi);
                    }

                    dci.ClassFormDefinition = fi.GetXmlDefinition();

                    TableManager tm = new TableManager(dci.ClassConnectionString);
                    dci.ClassXmlSchema = tm.GetXmlSchema("COM_ShoppingCartSKU");

                    DataClassInfoProvider.SetDataClass(dci);

                    // Generate queries
                    SqlGenerator.GenerateDefaultQueries(dci, true, false);
                    tm.RefreshCustomViews("COM_ShoppingCartSKU");
                }
            }
        }
        catch (Exception ex)
        {
            evp.LogEvent("Ecommerce.ShoppingCartItem - Upgrade", "Upgrade", ex);
        }

        #endregion


        #region "Ecommerce - SKU"

        try
        {
            dci = DataClassInfoProvider.GetDataClass("ecommerce.sku");
            if (dci != null)
            {
                FormInfo fi = new FormInfo(dci.ClassFormDefinition);
                if (fi != null)
                {
                    FormFieldInfo ffi = new FormFieldInfo();
                    ffi.Name = "SKUBundleInventoryType";
                    ffi.DataType = FormFieldDataTypeEnum.Text;
                    ffi.Size = 50;
                    ffi.AllowEmpty = true;
                    ffi.PublicField = false;
                    ffi.System = true;
                    ffi.FieldType = FormFieldControlTypeEnum.LabelControl;
                    ffi.Visible = false;

                    fi.AddFormField(ffi);

                    ffi = new FormFieldInfo();
                    ffi.Name = "SKUConversionName";
                    ffi.DataType = FormFieldDataTypeEnum.Text;
                    ffi.Size = 100;
                    ffi.AllowEmpty = true;
                    ffi.PublicField = false;
                    ffi.System = true;
                    ffi.FieldType = FormFieldControlTypeEnum.LabelControl;
                    ffi.Visible = false;

                    fi.AddFormField(ffi);

                    ffi = new FormFieldInfo();
                    ffi.Name = "SKUConversionValue";
                    ffi.DataType = FormFieldDataTypeEnum.Text;
                    ffi.Size = 200;
                    ffi.AllowEmpty = true;
                    ffi.PublicField = false;
                    ffi.System = true;
                    ffi.FieldType = FormFieldControlTypeEnum.LabelControl;
                    ffi.Visible = false;

                    fi.AddFormField(ffi);

                    ffi = new FormFieldInfo();
                    ffi.Name = "SKUMaxDownloads";
                    ffi.DataType = FormFieldDataTypeEnum.Integer;
                    ffi.AllowEmpty = true;
                    ffi.PublicField = false;
                    ffi.System = true;
                    ffi.FieldType = FormFieldControlTypeEnum.LabelControl;
                    ffi.Visible = false;

                    fi.AddFormField(ffi);

                    ffi = new FormFieldInfo();
                    ffi.Name = "SKUMaxItemsInOrder";
                    ffi.DataType = FormFieldDataTypeEnum.Integer;
                    ffi.AllowEmpty = true;
                    ffi.PublicField = false;
                    ffi.System = true;
                    ffi.FieldType = FormFieldControlTypeEnum.LabelControl;
                    ffi.Visible = false;

                    fi.AddFormField(ffi);

                    ffi = new FormFieldInfo();
                    ffi.Name = "SKUMaxPrice";
                    ffi.DataType = FormFieldDataTypeEnum.Decimal;
                    ffi.AllowEmpty = true;
                    ffi.PublicField = false;
                    ffi.System = true;
                    ffi.FieldType = FormFieldControlTypeEnum.LabelControl;
                    ffi.Visible = false;

                    fi.AddFormField(ffi);

                    ffi = new FormFieldInfo();
                    ffi.Name = "SKUMembershipGUID";
                    ffi.DataType = FormFieldDataTypeEnum.GUID;
                    ffi.AllowEmpty = true;
                    ffi.PublicField = false;
                    ffi.System = true;
                    ffi.FieldType = FormFieldControlTypeEnum.LabelControl;
                    ffi.Visible = false;

                    fi.AddFormField(ffi);

                    ffi = new FormFieldInfo();
                    ffi.Name = "SKUMinPrice";
                    ffi.DataType = FormFieldDataTypeEnum.Decimal;
                    ffi.AllowEmpty = true;
                    ffi.PublicField = false;
                    ffi.System = true;
                    ffi.FieldType = FormFieldControlTypeEnum.LabelControl;
                    ffi.Visible = false;

                    fi.AddFormField(ffi);

                    ffi = new FormFieldInfo();
                    ffi.Name = "SKUNeedsShipping";
                    ffi.DataType = FormFieldDataTypeEnum.Boolean;
                    ffi.AllowEmpty = true;
                    ffi.PublicField = false;
                    ffi.System = true;
                    ffi.FieldType = FormFieldControlTypeEnum.LabelControl;
                    ffi.Visible = false;

                    fi.AddFormField(ffi);

                    ffi = new FormFieldInfo();
                    ffi.Name = "SKUPrivateDonation";
                    ffi.DataType = FormFieldDataTypeEnum.Boolean;
                    ffi.AllowEmpty = true;
                    ffi.PublicField = false;
                    ffi.System = true;
                    ffi.FieldType = FormFieldControlTypeEnum.LabelControl;
                    ffi.Visible = false;

                    fi.AddFormField(ffi);

                    ffi = new FormFieldInfo();
                    ffi.Name = "SKUProductType";
                    ffi.DataType = FormFieldDataTypeEnum.Text;
                    ffi.Size = 50;
                    ffi.AllowEmpty = true;
                    ffi.PublicField = false;
                    ffi.System = true;
                    ffi.FieldType = FormFieldControlTypeEnum.LabelControl;
                    ffi.Visible = false;

                    fi.AddFormField(ffi);

                    ffi = new FormFieldInfo();
                    ffi.Name = "SKUSiteID";
                    ffi.DataType = FormFieldDataTypeEnum.Integer;
                    ffi.AllowEmpty = true;
                    ffi.PublicField = false;
                    ffi.System = true;
                    ffi.FieldType = FormFieldControlTypeEnum.LabelControl;
                    ffi.Visible = false;

                    fi.AddFormField(ffi);

                    ffi = new FormFieldInfo();
                    ffi.Name = "SKUValidFor";
                    ffi.DataType = FormFieldDataTypeEnum.Integer;
                    ffi.AllowEmpty = true;
                    ffi.PublicField = false;
                    ffi.System = true;
                    ffi.FieldType = FormFieldControlTypeEnum.LabelControl;
                    ffi.Visible = false;

                    fi.AddFormField(ffi);

                    ffi = new FormFieldInfo();
                    ffi.Name = "SKUValidity";
                    ffi.DataType = FormFieldDataTypeEnum.Text;
                    ffi.Size = 50;
                    ffi.AllowEmpty = true;
                    ffi.PublicField = false;
                    ffi.System = true;
                    ffi.FieldType = FormFieldControlTypeEnum.LabelControl;
                    ffi.Visible = false;

                    fi.AddFormField(ffi);

                    ffi = new FormFieldInfo();
                    ffi.Name = "SKUValidUntil";
                    ffi.DataType = FormFieldDataTypeEnum.DateTime;
                    ffi.AllowEmpty = true;
                    ffi.PublicField = false;
                    ffi.System = true;
                    ffi.FieldType = FormFieldControlTypeEnum.LabelControl;
                    ffi.Visible = false;

                    fi.AddFormField(ffi);

                    ffi = fi.GetFormField("SKUDepartmentID");
                    if (ffi != null)
                    {
                        ffi.AllowEmpty = true;
                        fi.UpdateFormField("SKUDepartmentID", ffi);
                    }

                    dci.ClassFormDefinition = fi.GetXmlDefinition();

                    TableManager tm = new TableManager(dci.ClassConnectionString);
                    dci.ClassXmlSchema = tm.GetXmlSchema("COM_SKU");

                    DataClassInfoProvider.SetDataClass(dci);

                    // Generate queries
                    SqlGenerator.GenerateDefaultQueries(dci, true, false);
                    tm.RefreshCustomViews("COM_SKU");
                }
            }
        }
        catch (Exception ex)
        {
            evp.LogEvent("Ecommerce.SKU - Upgrade", "Upgrade", ex);
        }

        #endregion


        #region "Community - Group"

        try
        {
            dci = DataClassInfoProvider.GetDataClass("Community.Group");
            if (dci != null)
            {
                FormInfo fi = new FormInfo(dci.ClassFormDefinition);
                if (fi != null)
                {
                    FormFieldInfo ffi = new FormFieldInfo();
                    ffi.Name = "GroupLogActivity";
                    ffi.DataType = FormFieldDataTypeEnum.Boolean;
                    ffi.AllowEmpty = true;
                    ffi.PublicField = false;
                    ffi.System = true;
                    ffi.FieldType = FormFieldControlTypeEnum.CheckBoxControl;
                    ffi.Visible = true;
                    ffi.DefaultValue = "true";
                    ffi.Caption = "GroupLogActivity";

                    fi.AddFormField(ffi);

                    dci.ClassFormDefinition = fi.GetXmlDefinition();

                    TableManager tm = new TableManager(dci.ClassConnectionString);
                    dci.ClassXmlSchema = tm.GetXmlSchema("Community_Group");

                    DataClassInfoProvider.SetDataClass(dci);

                    // Generate queries
                    SqlGenerator.GenerateDefaultQueries(dci, true, false);
                    tm.RefreshCustomViews("Community_Group");
                }
            }
        }
        catch (Exception ex)
        {
            evp.LogEvent("Community.Group - Upgrade", "Upgrade", ex);
        }

        #endregion


        #region "Newsletter - Subscriber"

        try
        {
            dci = DataClassInfoProvider.GetDataClass("newsletter.subscriber");
            if (dci != null)
            {
                FormInfo fi = new FormInfo(dci.ClassFormDefinition);
                if (fi != null)
                {
                    FormFieldInfo ffi = new FormFieldInfo();
                    ffi.Name = "SubscriberBounces";
                    ffi.DataType = FormFieldDataTypeEnum.Boolean;
                    ffi.AllowEmpty = true;
                    ffi.PublicField = false;
                    ffi.System = true;
                    ffi.FieldType = FormFieldControlTypeEnum.LabelControl;
                    ffi.Visible = false;

                    fi.AddFormField(ffi);

                    dci.ClassFormDefinition = fi.GetXmlDefinition();

                    TableManager tm = new TableManager(dci.ClassConnectionString);
                    dci.ClassXmlSchema = tm.GetXmlSchema("Newsletter_Subscriber");

                    DataClassInfoProvider.SetDataClass(dci);

                    // Generate queries
                    SqlGenerator.GenerateDefaultQueries(dci, true, false);
                    tm.RefreshCustomViews("Newsletter_Subscriber");
                }
            }
        }
        catch (Exception ex)
        {
            evp.LogEvent("Newsletter.Subscriber - Upgrade", "Upgrade", ex);
        }

        #endregion


        #region "CMS.Document"

        try
        {
            dci = DataClassInfoProvider.GetDataClass("cms.document");
            if (dci != null)
            {
                SearchSettings ss = dci.ClassSearchSettingsInfos;
                SearchSettingsInfo ssi = ss.GetSettingsInfo("42f446ee-9818-4596-8124-54a38f64aa05");
                if (ssi != null)
                {
                    ssi.Searchable = true;
                    ss.SetSettingsInfo(ssi);
                }

                DataClassInfoProvider.SetDataClass(dci);
            }
        }
        catch (Exception ex)
        {
            evp.LogEvent("CMS.Document - Upgrade", "Upgrade", ex);
        }

        #endregion


        // Set the path to the upgrade package
        mUpgradePackagePath = HttpContext.Current.Server.MapPath("~/CMSSiteUtils/Import/upgrade_55R2_60.zip");

        mWebsitePath = HttpContext.Current.Server.MapPath("~/");

        TableManager dtm = new TableManager(null);

        // Update all views
        dtm.RefreshDocumentViews();

        // Set data version
        ObjectHelper.SetSettingsKeyValue("CMSDataVersion", "6.0");

        // Clear hashtables
        CMSObjectHelper.ClearHashtables();

        // Clear the cache
        CacheHelper.ClearCache(null, true);

        // Drop the routes
        CMSMvcHandler.DropAllRoutes();

        // Init the Mimetype helper (required for the Import)
        MimeTypeHelper.LoadMimeTypes();

        CMSThread thread = new CMSThread(Upgrade60Import);
        thread.Start();
    }


    private static void Upgrade60Import()
    {
        EventLogProvider evp = new EventLogProvider();
        // Import
        try
        {
            RequestStockHelper.Remove("CurrentDomain", true);

            SiteImportSettings importSettings = new SiteImportSettings(CMSContext.CurrentUser)
                                                    {
                                                        DefaultProcessObjectType = ProcessObjectEnum.All,
                                                        SourceFilePath = mUpgradePackagePath,
                                                        WebsitePath = mWebsitePath
                                                    };

            ImportProvider.ImportObjectsData(importSettings);

            // Regenerate time zones
            TimeZoneInfoProvider.GenerateTimeZoneRules();

            #region "Separability"

            String webPartsPath = mWebsitePath + "CMSWebParts\\";
            List<String> files = new List<string>();
            // Create list of files to remove
            if (!ModuleEntry.IsModuleLoaded(ModuleEntry.BIZFORM))
            {
                files.AddRange(GetAllFiles(webPartsPath + "BizForms"));
            }
            if (!ModuleEntry.IsModuleLoaded(ModuleEntry.BLOGS))
            {
                files.AddRange(GetAllFiles(webPartsPath + "Blogs"));
            }
            if (!ModuleEntry.IsModuleLoaded(ModuleEntry.COMMUNITY))
            {
                files.AddRange(GetAllFiles(webPartsPath + "Community"));
            }
            if (!ModuleEntry.IsModuleLoaded(ModuleEntry.ECOMMERCE))
            {
                files.AddRange(GetAllFiles(webPartsPath + "Ecommerce"));
            }
            if (!ModuleEntry.IsModuleLoaded(ModuleEntry.EVENTMANAGER))
            {
                files.AddRange(GetAllFiles(webPartsPath + "Events"));
            }
            if (!ModuleEntry.IsModuleLoaded(ModuleEntry.FORUMS))
            {
                files.AddRange(GetAllFiles(webPartsPath + "Forums"));
            }
            if (!ModuleEntry.IsModuleLoaded(ModuleEntry.MEDIALIBRARY))
            {
                files.AddRange(GetAllFiles(webPartsPath + "MediaLibrary"));
            }
            if (!ModuleEntry.IsModuleLoaded(ModuleEntry.MESSAGEBOARD))
            {
                files.AddRange(GetAllFiles(webPartsPath + "MessageBoards"));
            }
            if (!ModuleEntry.IsModuleLoaded(ModuleEntry.MESSAGING))
            {
                files.AddRange(GetAllFiles(webPartsPath + "Messaging"));
            }
            if (!ModuleEntry.IsModuleLoaded(ModuleEntry.NEWSLETTER))
            {
                files.AddRange(GetAllFiles(webPartsPath + "Newsletters"));
            }
            if (!ModuleEntry.IsModuleLoaded(ModuleEntry.NOTIFICATIONS))
            {
                files.AddRange(GetAllFiles(webPartsPath + "Notifications"));
            }
            if (!ModuleEntry.IsModuleLoaded(ModuleEntry.ONLINEMARKETING))
            {
                files.AddRange(GetAllFiles(webPartsPath + "OnlineMarketing"));
            }
            if (!ModuleEntry.IsModuleLoaded(ModuleEntry.POLLS))
            {
                files.AddRange(GetAllFiles(webPartsPath + "Polls"));
            }
            if (!ModuleEntry.IsModuleLoaded(ModuleEntry.PROJECTMANAGEMENT))
            {
                files.AddRange(GetAllFiles(webPartsPath + "ProjectManagement"));
            }
            if (!ModuleEntry.IsModuleLoaded(ModuleEntry.REPORTING))
            {
                files.AddRange(GetAllFiles(webPartsPath + "Reporting"));
            }

            // Remove webparts for separated modules
            foreach (String file in files)
            {
                try
                {
                    File.Delete(file);
                }
                catch (Exception ex)
                {
                    evp.LogEvent("Upgrade to 6.0", "Upgrade", ex);
                }
            }

            #endregion

            evp.LogEvent("I", DateTime.Now, "Upgrade to 6.0", "Upgrade - Finish");
        }
        catch (Exception ex)
        {
            evp.LogEvent("Upgrade to 6.0", "Upgrade", ex);
        }
    }

    #endregion


    private static List<String> GetAllFiles(String folder)
    {
        List<String> files = new List<string>();

        files.AddRange(Directory.GetFiles(folder));

        string[] dirs = Directory.GetDirectories(folder);

        foreach (string dir in dirs)
        {
            files.AddRange(GetAllFiles(dir));
        }

        return files;
    }
}