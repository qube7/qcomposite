using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using Qube7.Composite.Hosting;
using $safeprojectname$.Shell;

namespace $safeprojectname$
{
    [PartNotDiscoverable]
    public partial class App : CompositeApp
    {
        #region Properties

        protected override IEnumerable<ComposablePartDefinition> Parts
        {
            get { return new TypeCatalog(_Shell.Parts); }
        }

        #endregion
    }
}
