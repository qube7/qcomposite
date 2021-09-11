using System.ComponentModel.Composition;
using Qube7.Composite.Presentation;

namespace $safeprojectname$.Controllers.Startup
{
    [View(typeof(HelloWorldView))]
    [View("Shell/Menu/Tools", typeof(HelloWorldMenuView))]
    [View("Shell/ToolBar", typeof(HelloWorldToolBarView))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export("Shell/Menu/Tools", typeof(ViewModel))]
    [Export("Shell/ToolBar", typeof(ViewModel))]
    [Export("Shell/Content", typeof(ViewModel))]
    [Navigable("HelloWorld")]
    public partial class HelloWorldViewModel : ViewModel<HelloWorldViewModel.CommandAggregator>, INavigable
    {
        #region Properties

        [Import]
        private StartupController Controller { get; set; }

        [Import("Shell/Content")]
        private INavigationService ContentService { get; set; }

        #endregion

        #region Methods

        void INavigable.NavigatedTo(string regionName, object data)
        {
            Commands.Hello.RaiseChanged();
        }

        void INavigable.NavigatedFrom(string regionName)
        {
            Commands.Hello.RaiseChanged();
        }

        private void ExecuteHelloCommand(object parameter)
        {
            ContentService.Navigate("HelloWorld");

            Controller.Hello();
        }

        private bool CanExecuteHelloCommand(object parameter)
        {
            return ContentService.Current != this;
        }

        #endregion
    }
}
