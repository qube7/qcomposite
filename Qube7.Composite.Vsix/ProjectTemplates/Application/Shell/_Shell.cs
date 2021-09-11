using System;
using System.Collections.Generic;
using Qube7.Composite;
using Qube7.Composite.Presentation;

namespace $safeprojectname$.Shell
{
    internal static class _Shell
    {
        #region Properties

        internal static IEnumerable<Type> Parts
        {
            get
            {
                Type type = typeof(_Shell);

                return type.Assembly
                    .GetTypes(TypeMatch
                    .FromNamespace(type.Namespace, true)
                    .AssignableTo<ViewModel>());
            }
        }

        #endregion
    }
}
