using System;
using System.Collections.Generic;
using Qube7.Composite;
using Qube7.Composite.Presentation;

namespace $safeprojectname$.Controllers.Startup
{
    internal static class _Startup
    {
        #region Properties

        internal static IEnumerable<Type> Parts
        {
            get
            {
                Type type = typeof(_Startup);

                return type.Assembly
                    .GetTypes(TypeMatch
                    .FromNamespace(type.Namespace)
                    .AssignableTo<ViewModel>());
            }
        }

        #endregion
    }
}
