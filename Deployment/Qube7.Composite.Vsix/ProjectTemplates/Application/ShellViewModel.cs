using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Qube7.Composite;
using Qube7.Composite.Hosting;
using Qube7.Composite.Presentation;

namespace $safeprojectname$
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export(CompositeApp.ShellContractName, typeof(ViewModel))]
    [ExportScope(ExportScope.Private)]
    public class ShellViewModel : ViewModel
    {
        #region Fields

        private readonly RecomposableCollection<ViewModel> menu = new RecomposableCollection<ViewModel>();

        private readonly RecomposableCollection<ViewModel> toolBar = new RecomposableCollection<ViewModel>();

        private readonly Navigable<ViewModel, INavigationMetadata> content = new Navigable<ViewModel, INavigationMetadata>();

        #endregion

        #region Properties

        [ImportMany("Shell/Menu", AllowRecomposition = true, RequiredCreationPolicy = CreationPolicy.Shared)]
        public IEnumerable<ViewModel> Menu
        {
            get { return menu; }
            private set { menu.Recompose(value); }
        }

        [ImportMany("Shell/ToolBar", AllowRecomposition = true, RequiredCreationPolicy = CreationPolicy.Shared)]
        public IEnumerable<ViewModel> ToolBar
        {
            get { return toolBar; }
            private set { toolBar.Recompose(value); }
        }

        [ImportMany("Shell/Content", AllowRecomposition = true, RequiredCreationPolicy = CreationPolicy.Shared)]
        private IEnumerable<Lazy<ViewModel, INavigationMetadata>> Contents
        {
            get { return content; }
            set { content.Recompose(value); }
        }

        [Export("Shell/Content")]
        private INavigationService ContentService
        {
            get { return new NavigationServiceProxy(content); }
        }

        public ViewModel Content
        {
            get { return content.Current; }
        }

        #endregion

        #region Constructors

        public ShellViewModel()
        {
            content.Navigated += (s, e) => NotifyChanged(nameof(Content));
        }

        #endregion
    }
}
