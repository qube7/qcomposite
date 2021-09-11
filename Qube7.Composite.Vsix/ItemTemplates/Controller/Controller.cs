using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using Qube7.Composite;
using $rootnamespace$.$safename$;

namespace $rootnamespace$
{
    [Export]
    [ExportScope(ExportScope.Protected)]
    [PartNotDiscoverable]
    public class $safecontroller$ : Controller
    {
        #region Fields

        private readonly TypeCatalog catalog;

        #endregion

        #region Constructors

        public $safecontroller$()
        {
            catalog = new TypeCatalog(_$safename$.Parts)
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

        #endregion
    }
}
