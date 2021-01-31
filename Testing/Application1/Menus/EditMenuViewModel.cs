using System.ComponentModel.Composition;
using Qube7.Composite;
using Qube7.Composite.Presentation;

namespace Application1.Menus
{
    [View(typeof(EditMenuView))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export("Shell/Menu", typeof(ViewModel))]
    [ExportScope(ExportScope.Private)]
    public partial class EditMenuViewModel : ViewModel
    {
    }
}
