using System.ComponentModel.Composition;
using Qube7.Composite.Presentation;

namespace Application1.Controllers.Startup
{
    [View(typeof(HelloWorldView))]
    [View("Shell/Menu/Tools", typeof(HelloWorldMenuView))]
    [View("Shell/ToolBar", typeof(HelloWorldToolBarView))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export("Shell/Menu/Tools", typeof(ViewModel))]
    [Export("Shell/ToolBar", typeof(ViewModel))]
    [Export("Shell/Content", typeof(ViewModel))]
    [Navigable("HelloWorld")]
    public partial class HelloWorldViewModel : ViewModel<HelloWorldViewModel.CommandAggregator>
    {
        #region Fields

        private INavigationService contentService;

        #endregion

        #region Properties

        [Import]
        private StartupController Controller { get; set; }

        [Import("Shell/Content")]
        private INavigationService ContentService
        {
            get { return contentService; }
            set
            {
                contentService = new NavigationServiceProxy(value);

                contentService.Navigated += (s, e) => Command.RaiseChanged(Commands.Hello);
            }
        }

        #endregion

        #region Methods

        protected override CommandAggregator CreateCommands()
        {
            return new CommandAggregator(this);
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

        #region Nested types

        public partial class CommandAggregator
        {
            #region Constructors

            protected internal CommandAggregator(HelloWorldViewModel model)
            {
                Initialize(model);
            }

            #endregion

            #region Methods

            partial void Initialize(HelloWorldViewModel model);

            #endregion
        }

        #endregion
    }
}
