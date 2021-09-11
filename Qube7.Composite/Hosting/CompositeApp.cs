using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Qube7.Composite.Presentation;

namespace Qube7.Composite.Hosting
{
    /// <summary>
    /// Provides a base class for the entry point of the composite application.
    /// </summary>
    public abstract class CompositeApp : Application
    {
        #region Fields

        /// <summary>
        /// The location of the module iterator resource.
        /// </summary>
        private Uri modulesUri;

        /// <summary>
        /// The collection of managed <see cref="ModuleContext"/> objects.
        /// </summary>
        private List<ModuleContext> modules;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the <see cref="Uri"/> that represents the location of the <see cref="ModuleIterator"/> providing the available modules.
        /// </summary>
        /// <value>The location of the module iterator resource.</value>
        public Uri ModulesUri
        {
            get { return modulesUri; }
            set
            {
                VerifyAccess();

                modulesUri = value;
            }
        }

        /// <summary>
        /// Gets the collection of <see cref="ComposablePartDefinition"/> provided by the <see cref="CompositeApp"/>.
        /// </summary>
        /// <value>The collection of <see cref="ComposablePartDefinition"/> objects.</value>
        protected abstract IEnumerable<ComposablePartDefinition> Parts { get; }

        /// <summary>
        /// Gets the <see cref="ViewModel"/> representing the shell model.
        /// </summary>
        /// <value>The shell model.</value>
        [Import(ContractName.Shell, RequiredCreationPolicy = CreationPolicy.Shared)]
        public ViewModel Shell { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositeApp"/> class.
        /// </summary>
        protected CompositeApp()
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Raises the <see cref="Application.Startup"/> event.
        /// </summary>
        /// <param name="e">A <see cref="StartupEventArgs"/> that contains the event data.</param>
        protected override void OnStartup(StartupEventArgs e)
        {
            VerifyAccess();

            if (modules == null)
            {
                modules = new List<ModuleContext>(4);

                if (modulesUri != null && LoadComponent(modulesUri) is ModuleIterator iterator)
                {
                    modules.AddRange(iterator.OfType<ModuleContext>());
                }

                AppModule.Run(this);
            }

            base.OnStartup(e);
        }

        /// <summary>
        /// Initializes this instance of the composite application.
        /// </summary>
        private void Initialize()
        {
            Resources resources = new Resources();

            if (Resources == null)
            {
                Resources = resources;

                return;
            }

            Resources.MergedDictionaries.Add(resources);
        }

        #endregion

        #region Nested types

        /// <summary>
        /// Provides the built-in infrastructure for the composite application.
        /// </summary>
        private class AppModule : Module
        {
            #region Fields

            /// <summary>
            /// The underlying application.
            /// </summary>
            private readonly CompositeApp app;

            /// <summary>
            /// The wrapping catalog.
            /// </summary>
            private readonly AppCatalog catalog;

            #endregion

            #region Constructors

            /// <summary>
            /// Initializes a new instance of the <see cref="AppModule"/> class.
            /// </summary>
            /// <param name="app">The underlying application.</param>
            private AppModule(CompositeApp app)
            {
                this.app = app;

                IEnumerable<ComposablePartDefinition> parts = app.Parts;

                if (parts == null)
                {
                    throw Error.ReturnsNull(nameof(Parts));
                }

                catalog = new AppCatalog(parts);
            }

            #endregion

            #region Methods

            /// <summary>
            /// Starts the specified composite application.
            /// </summary>
            /// <param name="app">The application to start.</param>
            internal static void Run(CompositeApp app)
            {
                List<Task<Module>> tasks = new List<Task<Module>>();

                foreach (ModuleContext context in app.modules)
                {
                    tasks.Add(Task.Run(() => context.Module));
                }

                AppModule module = new AppModule(app);

                module.Initialize();

                XContainer.Activate(module);

                while (tasks.Count > 0)
                {
                    int index = Task.WaitAny(tasks.ToArray());

                    XContainer.Activate(tasks[index].Result);

                    tasks.RemoveAt(index);
                }
            }

            /// <summary>
            /// Initializes this instance of the module object.
            /// </summary>
            protected internal override void Initialize()
            {
                app.Initialize();
            }

            /// <summary>
            /// Raises the <see cref="Controller.Activated"/> event.
            /// </summary>
            protected override void OnActivated()
            {
                Container.Catalogs.Add(catalog);

                Container.ComposeParts(app);

                base.OnActivated();
            }

            #endregion

            #region Nested types

            /// <summary>
            /// Wraps the collection of <see cref="ComposablePartDefinition"/>.
            /// </summary>
            private class AppCatalog : ComposablePartCatalog
            {
                #region Fields

                /// <summary>
                /// The underlying collection.
                /// </summary>
                private readonly List<ComposablePartDefinition> parts;

                #endregion

                #region Constructors

                /// <summary>
                /// Initializes a new instance of the <see cref="AppCatalog"/> class.
                /// </summary>
                /// <param name="collection">The collection to wrap.</param>
                internal AppCatalog(IEnumerable<ComposablePartDefinition> collection)
                {
                    parts = new List<ComposablePartDefinition>(collection);
                }

                #endregion

                #region Methods

                /// <summary>
                /// Returns an enumerator that iterates through the <see cref="AppCatalog"/>.
                /// </summary>
                /// <returns>A enumerator that can be used to iterate through the <see cref="AppCatalog"/>.</returns>
                public override IEnumerator<ComposablePartDefinition> GetEnumerator()
                {
                    return parts.GetEnumerator();
                }

                #endregion
            }

            #endregion
        }

        #endregion
    }
}
