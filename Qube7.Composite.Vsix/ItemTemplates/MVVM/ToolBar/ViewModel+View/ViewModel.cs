using System.ComponentModel.Composition;
using Qube7.Composite.Presentation;

namespace $rootnamespace$
{
    [View("Shell/ToolBar", typeof($safeview$))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export("Shell/ToolBar", typeof(ViewModel))]
    public partial class $safeitemrootname$ : ViewModel<$safeitemrootname$.CommandAggregator>
    {
    }
}
