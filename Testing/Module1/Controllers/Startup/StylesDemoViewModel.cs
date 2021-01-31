using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Windows.Data;
using Qube7.Composite.Presentation;

namespace Module1.Controllers.Startup
{
    [View(typeof(StylesDemoView))]
    [View("Shell/Menu/Tools", typeof(StylesDemoMenuView))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export("Shell/Content", typeof(ViewModel))]
    [Export("Shell/Menu/Tools", typeof(ViewModel))]
    [Navigable("StylesDemo")]
    public partial class StylesDemoViewModel : ViewModel<StylesDemoViewModel.CommandAggregator>, IDataErrorInfo
    {
        #region Fields

        private readonly ObservableCollection<string> collection = new ObservableCollection<string>();

        private readonly ICollectionView items;

        private string text = "Text";

        private string password = string.Empty;

        private INavigationService contentService;

        #endregion

        #region Properties

        public string Text
        {
            get { return text; }
            set
            {
                text = value;

                NotifyChanged(nameof(Text));
            }
        }

        public string Password
        {
            get { return password; }
            set
            {
                password = value;

                NotifyChanged(nameof(Password));
            }
        }

        public ICollectionView Items
        {
            get { return items; }
        }

        public string Error
        {
            get { return null; }
        }

        [Import("Shell/Content")]
        private INavigationService ContentService
        {
            get { return contentService; }
            set
            {
                contentService = new NavigationServiceProxy(value);

                contentService.Navigated += (s, e) => Command.RaiseChanged(Commands.Navigate);
            }
        }

        #endregion

        #region Indexers

        public string this[string columnName]
        {
            get
            {
                switch (columnName)
                {
                    case nameof(Text):
                        return text.Length > 5 ? "Error: Text" : null;
                    case nameof(Password):
                        return password.Length > 5 ? "Error: Password" : null;
                }

                return null;
            }
        }

        #endregion

        #region Constructors

        public StylesDemoViewModel()
        {
            for (int i = 0; i < 10; i++)
            {
                collection.Add(string.Format("Item #{0}", i));
            }

            items = CollectionViewSource.GetDefaultView(collection);

            items.GroupDescriptions.Add(new PropertyGroupDescription());
        }

        #endregion

        #region Methods

        protected override CommandAggregator CreateCommands()
        {
            return new CommandAggregator(this);
        }

        private void ExecuteNavigateCommand(object parameter)
        {
            ContentService.Navigate("StylesDemo");
        }

        private bool CanExecuteNavigateCommand(object parameter)
        {
            return ContentService.Current != this;
        }

        #endregion

        #region Nested types

        public partial class CommandAggregator
        {
            #region Constructors

            protected internal CommandAggregator(StylesDemoViewModel model)
            {
                Initialize(model);
            }

            #endregion

            #region Methods

            partial void Initialize(StylesDemoViewModel model);

            #endregion
        }

        #endregion
    }
}
