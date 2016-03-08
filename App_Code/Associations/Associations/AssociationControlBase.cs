
using System;
using CMS.PortalControls;

public class AssociationControlBase : CMSAbstractWebPart
{
    public AssociationCategory Category { get; set; }
    public string URL { get; set; }
    public string Text { get; set; }
    public string AssociationDataListURL { get; set; }
    public string AssociationDataListDialogTitle { get; set; }

    public IAssociationParameters GetParentAssociationParameters()
    {
        IAssociationParameters parentAssociationParameters = (IAssociationParameters)this.Parent.Parent.Parent;
        return parentAssociationParameters;
    }
}