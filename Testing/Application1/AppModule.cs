using System.ComponentModel.Composition;
using Application1.Controllers.Startup;
using Qube7.Composite;

namespace Application1
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
