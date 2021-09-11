using System;
using System.Collections.Generic;
using Qube7.Composite;
using Qube7.Composite.Presentation;

namespace $rootnamespace$.$safename$
{
    internal static class _$safename$
    {
        #region Properties

        internal static IEnumerable<Type> Parts
        {
            get
            {
                Type type = typeof(_$safename$);

                return type.Assembly
                    .GetTypes(TypeMatch
                    .FromNamespace(type.Namespace)
                    .AssignableTo<ViewModel>());
            }
        }

        #endregion
    }
}
