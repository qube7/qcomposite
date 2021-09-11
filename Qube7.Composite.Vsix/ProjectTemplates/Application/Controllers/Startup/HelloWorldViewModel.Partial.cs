using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Qube7.Composite.Presentation;

namespace $safeprojectname$.Controllers.Startup
{
    partial class HelloWorldViewModel
    {
        #region Methods

        protected override CommandAggregator CreateCommands()
        {
            return new CommandAggregator(this);
        }

        #endregion

        #region Nested types

        public new partial class CommandAggregator : ViewModel<CommandAggregator>.CommandAggregator
        {
            #region Properties

            protected override IEnumerable<ICommand> Commands
            {
                get
                {
                    List<ICommand> commands = null;

                    Aggregate(commands = new List<ICommand>());

                    return commands?.Concat(base.Commands) ?? base.Commands;
                }
            }

            #endregion

            #region Constructors

            protected internal CommandAggregator(HelloWorldViewModel model)
            {
                Initialize(model);
            }

            #endregion

            #region Methods

            partial void Initialize(HelloWorldViewModel model);

            partial void Aggregate(ICollection<ICommand> commands);

            #endregion
        }

        #endregion
    }
}
