using System.ComponentModel.Composition;
using Qube7.Composite;
using $safeprojectname$.Controllers;

namespace $safeprojectname$
{
    [PartNotDiscoverable]
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

            startup.Activate(this);

            base.OnActivated();
        }

        #endregion
    }
}
