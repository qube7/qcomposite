using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows.Input;

namespace Qube7.Composite.Presentation
{
    /// <summary>
    /// Provides a base class for the view models that expose commands.
    /// </summary>
    /// <typeparam name="TCommands">The type of object that aggregates the commands.</typeparam>
    public abstract class ViewModel<TCommands> : ViewModel where TCommands : ViewModel<TCommands>.CommandAggregator
    {
        #region Fields

        /// <summary>
        /// The command aggregator.
        /// </summary>
        private TCommands commands;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the object that aggregates the commands associated with this <see cref="ViewModel{TCommands}"/>.
        /// </summary>
        /// <value>The object that aggregates the commands of this <see cref="ViewModel{TCommands}"/>.</value>
        public TCommands Commands
        {
            get
            {
                if (commands == null)
                {
                    TCommands created = CreateCommands();

                    if (created == null)
                    {
                        throw Error.ReturnsNull(nameof(CreateCommands));
                    }

                    Interlocked.CompareExchange(ref commands, created, null);
                }

                return commands;
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModel{TCommands}"/> class.
        /// </summary>
        protected ViewModel()
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// When overridden in a derived class, creates the object that aggregates the commands associated with this <see cref="ViewModel{TCommands}"/>.
        /// </summary>
        /// <returns>The object that aggregates the commands of this <see cref="ViewModel{TCommands}"/>.</returns>
        protected abstract TCommands CreateCommands();

        #endregion

        #region Nested types

        /// <summary>
        /// Provides a base class for the objects that aggregate the commands.
        /// </summary>
        public abstract class CommandAggregator : IEnumerable<ICommand>, INotifyPropertyChanged
        {
            #region Fields

            /// <summary>
            /// The aggregated commands.
            /// </summary>
            private List<ICommand> commands;

            /// <summary>
            /// The property changed event handler.
            /// </summary>
            private PropertyChangedEventHandler propertyChanged;

            #endregion

            #region Properties

            /// <summary>
            /// Gets the commands that are aggregated in the <see cref="CommandAggregator"/>.
            /// </summary>
            /// <value>The aggregated commands.</value>
            protected virtual IEnumerable<ICommand> Commands
            {
                get { return Enumerable.Empty<ICommand>(); }
            }

            #endregion

            #region Events

            /// <summary>
            /// Occurs when a property value changes.
            /// </summary>
            event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
            {
                add { Event.Subscribe(ref propertyChanged, value); }
                remove { Event.Unsubscribe(ref propertyChanged, value); }
            }

            #endregion

            #region Constructors

            /// <summary>
            /// Initializes a new instance of the <see cref="CommandAggregator"/> class.
            /// </summary>
            protected CommandAggregator()
            {
            }

            #endregion

            #region Methods

            /// <summary>
            /// Raises the <see cref="INotifyPropertyChanged.PropertyChanged"/> event.
            /// </summary>
            /// <param name="e">The <see cref="PropertyChangedEventArgs"/> instance containing the event data.</param>
            protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
            {
                Event.Raise(propertyChanged, this, e);
            }

            /// <summary>
            /// Returns an enumerator that iterates through the <see cref="CommandAggregator"/>.
            /// </summary>
            /// <returns>A enumerator that can be used to iterate through the <see cref="CommandAggregator"/>.</returns>
            public IEnumerator<ICommand> GetEnumerator()
            {
                if (commands == null)
                {
                    List<ICommand> list = new List<ICommand>();

                    list.AddRange(Commands ?? Enumerable.Empty<ICommand>());

                    Interlocked.CompareExchange(ref commands, list, null);
                }

                return commands.GetEnumerator();
            }

            /// <summary>
            /// Returns an enumerator that iterates through the <see cref="CommandAggregator"/>.
            /// </summary>
            /// <returns>A enumerator that can be used to iterate through the <see cref="CommandAggregator"/>.</returns>
            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            #endregion
        }

        #endregion
    }
}
