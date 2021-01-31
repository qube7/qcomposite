using System.ComponentModel.Composition;
using Module1.Controllers.Startup;
using Qube7.Composite;

namespace Module1
{
    [Export]
    [ExportScope(ExportScope.Protected)]
    public class AppModule : Module
    {
        #region Fields

        private StartupController startup;

        #endregion

        #region Methods

        protected override void Initialize()
        {
            startup = new StartupController();
        }

        protected override void OnActivated()
        {
            Container.ComposeParts(this);

            Execute(startup);
        }

        #endregion
    }
}
