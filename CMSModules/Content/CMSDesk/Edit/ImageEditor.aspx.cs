using System;
using System.Collections;

using CMS.ExtendedControls;
using CMS.GlobalHelper;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSModules_Content_CMSDesk_Edit_ImageEditor : CMSModalPage
{
    #region "Page methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (QueryHelper.ValidateHash("hash"))
        {
            if (!imageEditor.IsUndoRedoPossible())
            {
                CurrentMaster.Body.Attributes["onunload"] = GetRefresh();
            }

            string title = GetString("general.editimage");
            Page.Title = title;
            CurrentMaster.Title.TitleText = title;
            CurrentMaster.Title.TitleImage = GetImageUrl("Design/Controls/ImageEditor/Title.png");

            CurrentMaster.Title.HelpTopicName = "imageeditor";
            CurrentMaster.Title.HelpName = "helpTopic";

            // Register postback
            ControlsHelper.RegisterPostbackControl(btnRedo);
            ControlsHelper.RegisterPostbackControl(btnUndo);
            ControlsHelper.RegisterPostbackControl(btnSave);
            ControlsHelper.RegisterPostbackControl(btnClose);

            btnSave.Click += btnSave_Click;
            btnClose.Click += btnClose_Click;
            btnUndo.Click += btnUndo_Click;
            btnRedo.Click += btnRedo_Click;

            btnUndo.Visible = imageEditor.IsUndoRedoPossible();
            btnRedo.Visible = imageEditor.IsUndoRedoPossible();
            btnSave.Visible = imageEditor.IsUndoRedoPossible();

            AddNoCacheTag();
        }
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        if (QueryHelper.ValidateHash("hash"))
        {
            btnUndo.Enabled = imageEditor.IsUndoEnabled();
            btnRedo.Enabled = imageEditor.IsRedoEnabled();

            if (btnUndo.Enabled || btnRedo.Enabled)
            {
                string confirmScript = String.Format("function discardchanges() {{ return confirm({0}); }}", ScriptHelper.GetString(GetString("imageeditor.discardchangesconfirmation")));
                ScriptHelper.RegisterClientScriptBlock(Page, typeof(string), "DiscardChanges", ScriptHelper.GetScript(confirmScript));

                btnClose.OnClientClick = "return discardchanges();";
            }
        }

        // Enable or disable save button
        btnSave.Enabled = imageEditor.Enabled;
    }

    #endregion


    #region "Buttons handling"

    protected void btnRedo_Click(object sender, EventArgs e)
    {
        imageEditor.ProcessRedo();
    }


    protected void btnUndo_Click(object sender, EventArgs e)
    {
        imageEditor.ProcessUndo();
    }


    protected void btnSave_Click(object sender, EventArgs e)
    {
        imageEditor.SaveCurrentVersion();
        if (!imageEditor.SavingFailed)
        {
            string script = "setTimeout(function() { window.skipCloseConfirm = true; CloseDialog(); }, 1);";
            if (imageEditor.IsUndoRedoPossible())
            {
                script += GetRefresh();
            }

            ScriptHelper.RegisterStartupScript(Page, typeof(string), "CloseImageEditor", script, true);
        }
    }


    protected void btnClose_Click(object sender, EventArgs e)
    {
        TempFileInfoProvider.DeleteTempFiles(TempFileInfoProvider.IMAGE_EDITOR_FOLDER, imageEditor.InstanceGUID);

        ScriptHelper.RegisterStartupScript(Page, typeof(string), "CloseImageEditor", "setTimeout(function() { window.skipCloseConfirm = true; CloseDialog(); },0);", true);
    }

    #endregion


    #region "Private methods"

    private static string GetRefresh()
    {
        if (QueryHelper.GetBoolean("refresh", false))
        {
            string attachmentGuid = null;
            string filepath = null;
            string clientid = null;
            int nodeId = 0;

            String identifier = QueryHelper.GetString("identifier", null);
            if (!String.IsNullOrEmpty(identifier))
            {
                Hashtable properties = WindowHelper.GetItem(identifier) as Hashtable;
                if (properties != null)
                {
                    attachmentGuid = ValidationHelper.GetString(properties["attachmentguid"], string.Empty);
                    filepath = ValidationHelper.GetString(properties["filepath"], string.Empty);
                    clientid = ValidationHelper.GetString(properties["clientid"], string.Empty);
                    nodeId = ValidationHelper.GetInteger(properties["nodeId"], 0);
                }
            }
            else
            {
                attachmentGuid = QueryHelper.GetString("attachmentguid", string.Empty);
                filepath = QueryHelper.GetString("filepath", string.Empty);
                clientid = QueryHelper.GetString("clientid", string.Empty);
                nodeId = QueryHelper.GetInteger("nodeId", 0);
            }

            if (!String.IsNullOrEmpty(filepath))
            {
                return String.Format("if (wopener.imageEdit_FileSystemRefresh) {{ wopener.imageEdit_FileSystemRefresh({0}); }}", ScriptHelper.GetString(String.Format("filepath|{0}", filepath)));
            }
            else if (nodeId > 0)
            {
                return String.Format("if (wopener.imageEdit_ContentRefresh) {{ wopener.imageEdit_ContentRefresh({0}); }}", ScriptHelper.GetString(String.Format("attachmentguid|{0}|nodeId|{1}", attachmentGuid, nodeId)));
            }
            else if (!string.IsNullOrEmpty(attachmentGuid))
            {
                return String.Format("if (wopener.imageEdit_AttachmentRefresh) {{ wopener.imageEdit_AttachmentRefresh('attachmentguid|{0}'); }} else {{ InitRefresh({1}, true, true, '{0}', 'refresh'); }}", ScriptHelper.GetString(attachmentGuid, false), ScriptHelper.GetString(clientid));
            }
        }

        return String.Empty;
    }

    #endregion
}