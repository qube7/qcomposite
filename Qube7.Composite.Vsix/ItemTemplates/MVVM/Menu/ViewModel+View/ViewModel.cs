using System.ComponentModel.Composition;
using Qube7.Composite.Presentation;

namespace $rootnamespace$
{
    [View("Shell/Menu", typeof($safeview$))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export("Shell/Menu", typeof(ViewModel))]
    public partial class $safeitemrootname$ : ViewModel<$safeitemrootname$.CommandAggregator>
    {
    }
}
