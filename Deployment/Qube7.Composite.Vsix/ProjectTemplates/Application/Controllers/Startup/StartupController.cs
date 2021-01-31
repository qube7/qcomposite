using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using Qube7;
using Qube7.Composite;

namespace $safeprojectname$.Controllers.Startup
{
    [Export]
    [ExportScope(ExportScope.Protected)]
    public class StartupController : Controller
    {
        #region Fields

        private readonly TypeCatalog catalog;

        #endregion

        #region Constructors

        public StartupController()
        {
            Type type = typeof(StartupController);

            catalog = new TypeCatalog(type.Assembly.GetTypes()
                .FromNamespace(type.Namespace)
                .Except(type));
        }

        #endregion

        #region Methods

        protected override void OnActivated()
        {
            Container.ComposeParts(this);

            Container.Catalogs.Add(catalog);
        }

        public void Hello()
        {
        }

        #endregion
    }
}
