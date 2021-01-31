using System.ComponentModel.Composition;
using Qube7.Composite;
using $safeprojectname$.Controllers.Startup;

namespace $safeprojectname$
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
