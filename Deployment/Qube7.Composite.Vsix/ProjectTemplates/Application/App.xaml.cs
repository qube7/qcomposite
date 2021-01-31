using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using Qube7;
using Qube7.Composite;
using Qube7.Composite.Hosting;

namespace $safeprojectname$
{
    public partial class App : CompositeApp
    {
        #region Properties

        protected override IEnumerable<ComposablePartDefinition> Parts
        {
            get
            {
                Type type = typeof(App);

                return new TypeCatalog(type.Assembly.GetTypes()
                    .FromNamespace(string.Concat(type.Namespace, ".Menus"))
                    .FromNamespace(string.Concat(type.Namespace, ".ToolBars"))
                    .Union(typeof(ShellViewModel)));
            }
        }

        #endregion
    }
}
