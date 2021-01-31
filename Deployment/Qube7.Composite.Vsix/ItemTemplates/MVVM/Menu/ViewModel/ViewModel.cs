using System.ComponentModel.Composition;
using Qube7.Composite.Presentation;

namespace $rootnamespace$
{
    //[View("Shell/Menu", typeof($safeview$))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export("Shell/Menu", typeof(ViewModel))]
    public partial class $safeitemrootname$ : ViewModel<$safeitemrootname$.CommandAggregator>
    {
        #region Methods

        protected override CommandAggregator CreateCommands()
        {
            return new CommandAggregator(this);
        }

        #endregion

        #region Nested types

        public partial class CommandAggregator
        {
            #region Constructors

            protected internal CommandAggregator($safeitemrootname$ model)
            {
                Initialize(model);
            }

            #endregion

            #region Methods

            partial void Initialize($safeitemrootname$ model);

            #endregion
        }

        #endregion
    }
}
