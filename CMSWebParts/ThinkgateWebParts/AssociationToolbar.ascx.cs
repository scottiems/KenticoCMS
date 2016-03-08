
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CMS.PortalControls;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

public partial class CMSWebParts_ThinkgateWebParts_AssociationToolbar : AssociationToolbarBase, IAssociationParameters
{
    #region " Private Variables "

    private ITemplate associationControlTemplate = null;

    #endregion

    #region " Public Properties "

    public string DocumentType { get; set; }
    public string DocumentID { get; set; }
    public string UserID { get; set; }

    [TemplateContainer(typeof(AssociationControlContainer))]
    [PersistenceMode(PersistenceMode.InnerProperty)]
    public ITemplate AssociationControlTemplate
    {
        get
        {
            return associationControlTemplate;
        }
        set
        {
            associationControlTemplate = value;
        }
    }

    #endregion

    #region " Event Methods "

    void Page_Init()
    {
        if (associationControlTemplate != null)
        {
            AssociationControlContainer container = new AssociationControlContainer();
            associationControlTemplate.InstantiateIn(container);
            phTemplatePlaceHolder.Controls.Add(container);
        }

    }
    
}
    #endregion