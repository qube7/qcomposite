using System.Windows;
using System.Windows.Controls;

namespace Module1.Controllers.Startup
{
    public partial class StylesDemoView : UserControl
    {
        #region Constructors

        public StylesDemoView()
        {
            InitializeComponent();
        }

        #endregion

        #region Methods

        private void OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            PasswordBox password = sender as PasswordBox;
            if (password != null)
            {
                password.Tag = password.Password;
            }
        }

        #endregion
    }
}
