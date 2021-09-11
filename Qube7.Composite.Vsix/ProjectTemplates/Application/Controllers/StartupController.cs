using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using Qube7.Composite;
using $safeprojectname$.Controllers.Startup;

namespace $safeprojectname$.Controllers
{
    [Export]
    [ExportScope(ExportScope.Protected)]
    [PartNotDiscoverable]
    public class StartupController : Controller
    {
        #region Fields

        private readonly TypeCatalog catalog;

        #endregion

        #region Constructors

        public StartupController()
        {
            catalog = new TypeCatalog(_Startup.Parts)
                .EnsureInitialized();
        }

        #endregion

        #region Methods

        protected override void OnActivated()
        {
            Container.Update(c =>
            {
                c.ComposeParts(this);
                c.Catalogs.Add(catalog);
            });

            base.OnActivated();
        }

        public void Hello()
        {
        }

        #endregion
    }
}
