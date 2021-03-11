using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using Qube7;
using Qube7.Composite;

namespace $rootnamespace$.$safename$
{
    [Export]
    [ExportScope(ExportScope.Protected)]
    public class $safeitemrootname$ : Controller
    {
        #region Fields

        private readonly TypeCatalog catalog;

        #endregion

        #region Constructors

        public $safeitemrootname$()
        {
            Type type = typeof($safeitemrootname$);

            catalog = new TypeCatalog(type.Assembly.GetTypes()
                .FromNamespace(type.Namespace)
                .Except(type));
        }

        #endregion

        #region Methods

        protected override void OnActivated()
        {
            Container.DispatchAsync(Compose);
        }

        private void Compose()
        {
            Container.ComposeParts(this);

            Container.Catalogs.Add(catalog);
        }

        #endregion
    }
}
