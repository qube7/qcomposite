using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using System.Windows;
using Qube7.ComponentModel;
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
        /// Specifies the contract name for a <see cref="ViewModel"/> representing the shell model.
        /// </summary>
        public const string ShellContractName = "Application/Shell";

        /// <summary>
        /// The location of the module discovery resource.
        /// </summary>
        private Uri modulesUri;

        /// <summary>
        /// The collection of managed <see cref="ModuleContext"/> objects.
        /// </summary>
        private List<ModuleContext> modules;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the <see cref="Uri"/> that represents the location of the <see cref="ModuleDiscovery"/> providing the available modules.
        /// </summary>
        /// <value>The location of the module discovery resource.</value>
        public Uri ModulesUri
        {
            get { return modulesUri; }
            set
            {
                VerifyAccess();

                Requires.NotNull(value, nameof(value));

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
        [Import(ShellContractName, RequiredCreationPolicy = CreationPolicy.Shared)]
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

                if (modulesUri != null)
                {
                    ModuleDiscovery discovery = LoadComponent(modulesUri) as ModuleDiscovery;
                    if (discovery != null)
                    {
                        try
                        {
                            modules.AddRange(discovery.OfType<ModuleContext>());
                        }
                        finally
                        {
                            Disposable.Dispose(discovery);
                        }
                    }
                }

                ParallelQuery<Module> query = modules.Prepend(null).AsParallel().WithMergeOptions(ParallelMergeOptions.NotBuffered).Select(Initialize);

                using (IEnumerator<Module> enumerator = query.GetEnumerator())
                {
                    enumerator.MoveNext();

                    Initialize(this);

                    Module module = new AppModule(this);

                    Module.Execute(module);

                    do
                    {
                        module = enumerator.Current;

                        if (module != null)
                        {
                            Module.Execute(module);
                        }
                    }
                    while (enumerator.MoveNext());
                }
            }

            base.OnStartup(e);
        }

        /// <summary>
        /// Initializes the specified module context and returns the associated module object.
        /// </summary>
        /// <param name="context">The module context to initialize.</param>
        /// <returns>The associated module instance.</returns>
        private static Module Initialize(ModuleContext context)
        {
            if (context != null)
            {
                Initializable.Initialize(context);

                return context.Module;
            }

            return null;
        }

        /// <summary>
        /// Initializes the specified composite application.
        /// </summary>
        /// <param name="app">The application to initialize.</param>
        private static void Initialize(CompositeApp app)
        {
            Resources resources = new Resources();

            if (app.Resources == null)
            {
                app.Resources = resources;

                return;
            }

            app.Resources.MergedDictionaries.Add(resources);
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
            internal AppModule(CompositeApp app)
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
            /// Activates behavior of the current module instance.
            /// </summary>
            protected override void OnActivated()
            {
                Container.Catalogs.Add(catalog);

                Container.ComposeParts(app);
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
