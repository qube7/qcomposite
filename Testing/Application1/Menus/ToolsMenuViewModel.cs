using System.Collections.Generic;
using System.ComponentModel.Composition;
using Qube7.Composite;
using Qube7.Composite.Presentation;

namespace Application1.Menus
{
    [View(typeof(ToolsMenuView))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export("Shell/Menu", typeof(ViewModel))]
    [ExportScope(ExportScope.Private)]
    public partial class ToolsMenuViewModel : ViewModel
    {
        #region Fields

        private readonly RecomposableCollection<ViewModel> tools = new RecomposableCollection<ViewModel>();

        #endregion

        #region Properties

        [ImportMany("Shell/Menu/Tools", AllowRecomposition = true, RequiredCreationPolicy = CreationPolicy.Shared)]
        public IEnumerable<ViewModel> Tools
        {
            get { return tools; }
            private set { tools.Recompose(value); }
        }

        #endregion
    }
}
