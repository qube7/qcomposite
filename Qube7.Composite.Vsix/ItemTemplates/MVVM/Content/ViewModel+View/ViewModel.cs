using System.ComponentModel.Composition;
using Qube7.Composite.Presentation;

namespace $rootnamespace$
{
    [View(typeof($safeview$))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export("Shell/Content", typeof(ViewModel))]
    [Navigable("$safename$")]
    public partial class $safeitemrootname$ : ViewModel<$safeitemrootname$.CommandAggregator>, INavigable
    {
    }
}
