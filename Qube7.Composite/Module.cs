namespace Qube7.Composite
{
    /// <summary>
    /// Provides a base class for module that represents the deployment-oriented set of functionality.
    /// </summary>
    public abstract class Module : Controller
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Module"/> class.
        /// </summary>
        protected Module()
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// When overridden in a derived class, initializes this instance of the module object.
        /// </summary>
        protected internal abstract void Initialize();

        #endregion
    }
}
