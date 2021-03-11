using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Windows.Threading;
using Qube7.Composite.Threading;
using Qube7.Threading;

namespace Qube7.Composite
{
    /// <summary>
    /// Provides a base class for controller that encapsulates the logic required to support a use case or user task.
    /// </summary>
    public abstract class Controller : ExportProvider
    {
        #region Fields

        /// <summary>
        /// Specifies the metadata key created by the composition system to mark an export scope.
        /// </summary>
        public const string ExportScopeMetadataName = nameof(ExportScopeAttribute.ExportScope);

        /// <summary>
        /// Specifies the metadata key created by the composition system to mark a method that requires the user interface thread.
        /// </summary>
        private const string RequiresUIThreadMetadataName = nameof(RequiresUIThreadAttribute.RequiresUIThread);

        /// <summary>
        /// The event broker.
        /// </summary>
        private readonly EventBroker broker;

        /// <summary>
        /// The composition container.
        /// </summary>
        private XContainer container;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the <see cref="XContainer"/> this <see cref="Controller"/> is associated with.
        /// </summary>
        /// <value>The composition container.</value>
        protected XContainer Container
        {
            get { return Volatile.Read(ref container); }
        }

        /// <summary>
        /// Gets the <see cref="IRaiseEvent"/> to be used to publish brokered events.
        /// </summary>
        /// <value>The object that publishes brokered events.</value>
        protected IRaiseEvent RaiseEvent
        {
            get { return broker; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Controller"/> class.
        /// </summary>
        protected Controller()
        {
            broker = new EventBroker(this);
        }

        #endregion

        #region Methods

        /// <summary>
        /// When overridden in a derived class, activates behavior of the current controller instance.
        /// </summary>
        /// <remarks>This method is called when the <see cref="XContainer"/> instance gets associated with the current controller.</remarks>
        protected virtual void OnActivated()
        {
        }

        /// <summary>
        /// When overridden in a derived class, performs cleanup after deactivating behavior of the current controller instance.
        /// </summary>
        /// <remarks>This method is called when the <see cref="XContainer"/> instance gets disposed and disassociated with the current controller.</remarks>
        protected virtual void OnDeactivated()
        {
        }

        /// <summary>
        /// Executes the specified child <see cref="Controller"/> instance.
        /// </summary>
        /// <param name="controller">The child controller to execute.</param>
        protected void Execute(Controller controller)
        {
            Requires.NotNull(controller, nameof(controller));

            XContainer.Activate(controller, this);
        }

        /// <summary>
        /// Gets all the exports that match the constraint defined by the specified definition.
        /// </summary>
        /// <param name="definition">The object that defines the conditions of the <see cref="Export"/> objects to return.</param>
        /// <param name="atomicComposition">The transactional container for the composition.</param>
        /// <returns>A collection that contains all the exports that match the specified condition.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected sealed override IEnumerable<Export> GetExportsCore(ImportDefinition definition, AtomicComposition atomicComposition)
        {
            ImportSource source = ImportSource.Any;

            if (definition.Metadata.TryGetValue(CompositionConstants.ImportSourceMetadataName, out object value))
            {
                source = (ImportSource)value;
            }

            IEnumerable<Export> exports = Enumerable.Empty<Export>();

            using (Sync.Read())
            {
                if (container != null)
                {
                    bool raise = false;

                    switch (source)
                    {
                        case ImportSource.Any:
                            container.root.TryGetExports(definition, atomicComposition, out exports);
                            raise = definition.IsConstraintSatisfiedBy(broker.Definition);
                            break;
                        case ImportSource.Local:
                            container.local.TryGetExports(definition, atomicComposition, out exports);
                            raise = definition.IsConstraintSatisfiedBy(broker.Definition);
                            break;
                        case ImportSource.NonLocal:
                            container.ancestor.TryGetExports(definition, atomicComposition, out exports);
                            break;
                    }

                    if (exports.FastAny())
                    {
                        List<Export> list = new List<Export>();

                        foreach (Export export in exports)
                        {
                            list.Add(new XExport(export));
                        }

                        if (raise)
                        {
                            list.Add(broker);
                        }

                        return list;
                    }

                    if (raise)
                    {
                        return Params.Array(broker);
                    }
                }
            }

            return exports;
        }

        /// <summary>
        /// Called when the provided exports are changing.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="ExportsChangeEventArgs"/> that contains the event data.</param>
        private void OnExportsChanging(object sender, ExportsChangeEventArgs e)
        {
            OnExportsChanging(e);
        }

        /// <summary>
        /// Called when the exports in the provider change.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="ExportsChangeEventArgs"/> that contains the event data.</param>
        private void OnExportsChanged(object sender, ExportsChangeEventArgs e)
        {
            OnExportsChanged(e);
        }

        #endregion

        #region Nested types

        /// <summary>
        /// Represents a element in the composition hierarchy.
        /// </summary>
        private interface IComposable
        {
            #region Methods

            /// <summary>
            /// Attaches the specified container within the hierarchy.
            /// </summary>
            /// <param name="container">The container to attach.</param>
            void Attach(XContainer container);

            /// <summary>
            /// Attaches the specified container within the hierarchy.
            /// </summary>
            /// <param name="container">The container to attach.</param>
            /// <param name="current">The traversing container.</param>
            void Attach(XContainer container, XContainer current);

            /// <summary>
            /// Detaches the specified container from the hierarchy.
            /// </summary>
            /// <param name="container">The container to detach.</param>
            void Detach(XContainer container);

            /// <summary>
            /// Detaches the specified container from the hierarchy.
            /// </summary>
            /// <param name="container">The container to detach.</param>
            /// <param name="current">The traversing container.</param>
            void Detach(XContainer container, XContainer current);

            #endregion
        }

        /// <summary>
        /// Manages the extensibility and composition of a <see cref="Controller"/>.
        /// </summary>
        protected class XContainer : ICompositionContainer, IDispatcherObject, IDisposable, IComposable
        {
            #region Fields

            /// <summary>
            /// The underlying controller.
            /// </summary>
            private readonly Controller controller;

            /// <summary>
            /// The parent element.
            /// </summary>
            private readonly IComposable parent;

            /// <summary>
            /// The descendant containers.
            /// </summary>
            private readonly List<XContainer> containers;

            /// <summary>
            /// The aggregate catalog.
            /// </summary>
            private readonly AggregateCatalog aggregate;

            /// <summary>
            /// The catalog provider.
            /// </summary>
            private readonly CatalogExportProvider catalog;

            /// <summary>
            /// The composable part provider.
            /// </summary>
            private readonly ComposablePartExportProvider composable;

            /// <summary>
            /// The public scope provider.
            /// </summary>
            private readonly CompositeProvider scope0;

            /// <summary>
            /// The internal scope provider.
            /// </summary>
            private readonly CompositeProvider scope1;

            /// <summary>
            /// The protected scope provider.
            /// </summary>
            private readonly CompositeProvider scope2;

            /// <summary>
            /// The root provider.
            /// </summary>
            internal readonly AggregateExportProvider root;

            /// <summary>
            /// The local provider.
            /// </summary>
            internal readonly AggregateExportProvider local;

            /// <summary>
            /// The ancestor provider.
            /// </summary>
            internal readonly AggregateExportProvider ancestor;

            /// <summary>
            /// The composed parts collection.
            /// </summary>
            private IEnumerable<ComposablePart> parts;

            #endregion

            #region Properties

            /// <summary>
            /// Gets the <see cref="System.Windows.Threading.Dispatcher"/> this <see cref="XContainer"/> is associated with.
            /// </summary>
            /// <value>The associated <see cref="System.Windows.Threading.Dispatcher"/>.</value>
            public Dispatcher Dispatcher
            {
                get { return Composable.Dispatcher; }
            }

            /// <summary>
            /// Gets the underlying catalogs of the <see cref="XContainer"/> object.
            /// </summary>
            /// <value>A collection of <see cref="ComposablePartCatalog"/> objects that underlie the <see cref="XContainer"/> object.</value>
            public ICollection<ComposablePartCatalog> Catalogs
            {
                get
                {
                    VerifyAccess();

                    return aggregate.Catalogs;
                }
            }

            #endregion

            #region Constructors

            /// <summary>
            /// Initializes a new instance of the <see cref="XContainer"/> class.
            /// </summary>
            /// <param name="controller">The underlying controller.</param>
            /// <param name="parent">The parent element.</param>
            private XContainer(Controller controller, IComposable parent)
            {
                this.controller = controller;

                this.parent = parent;

                containers = new List<XContainer>();

                aggregate = new PrivateAggregateCatalog();

                CompositionOptions options = CompositionOptions.DisableSilentRejection | CompositionOptions.IsThreadSafe;

                catalog = new CatalogExportProvider(aggregate, options);
                catalog.SourceProvider = controller;

                composable = new ComposablePartExportProvider(options);
                composable.SourceProvider = controller;

                composable.ExportsChanging += OnExportsChanging;

                scope0 = new Scope0();
                scope1 = new Scope1();
                scope2 = new Scope2();

                root = new AggregateExportProvider(composable, catalog, scope2, scope1, scope0);
                local = new ExtensionProvider(this, composable, catalog);
                ancestor = new AggregateExportProvider(scope2, scope1, scope0);

                root.ExportsChanging += controller.OnExportsChanging;
                root.ExportsChanged += controller.OnExportsChanged;

                parent.Attach(this);
            }

            #endregion

            #region Methods

            /// <summary>
            /// Adds or removes the parts in the specified <see cref="CompositionBatch"/> from the container and executes composition.
            /// </summary>
            /// <param name="batch">Changes to the <see cref="XContainer"/> to include during the composition.</param>
            public void Compose(CompositionBatch batch)
            {
                VerifyAccess();

                using (Sync.Write())
                {
                    composable.Compose(batch);
                }
            }

            /// <summary>
            /// Activates behavior of the specified child <see cref="Controller"/> instance.
            /// </summary>
            /// <param name="child">The child controller to activate.</param>
            /// <param name="parent">The parent controller.</param>
            internal static void Activate(Controller child, Controller parent)
            {
                using (Sync.Write())
                {
                    if (parent.container == null)
                    {
                        throw Error.Argument(Strings.ParentNotActivated, nameof(parent));
                    }

                    if (child.container != null)
                    {
                        throw Error.Argument(Strings.ControllerActivated, nameof(child));
                    }

                    child.container = new XContainer(child, parent.container);
                }

                child.OnActivated();
            }

            /// <summary>
            /// Activates behavior of the specified <see cref="Module"/> instance.
            /// </summary>
            /// <param name="module">The module to activate.</param>
            internal static void Activate(Module module)
            {
                using (Sync.Write())
                {
                    if (module.container != null)
                    {
                        throw Error.Argument(Strings.ControllerActivated, nameof(module));
                    }

                    module.container = new XContainer(module, Composable.Instance);
                }

                module.OnActivated();
            }

            /// <summary>
            /// Traverses containers existing in the current container subtree and marks them for disposal.
            /// </summary>
            /// <param name="traverse">The traversed container collection.</param>
            private void Disposing(List<XContainer> traverse)
            {
                for (int i = 0; i < containers.Count; i++)
                {
                    containers[i].Disposing(traverse);
                }

                controller.container = null;

                root.ExportsChanging -= controller.OnExportsChanging;
                root.ExportsChanged -= controller.OnExportsChanged;

                traverse.Add(this);
            }

            /// <summary>
            /// Releases resources used by the current <see cref="XContainer"/> instance.
            /// </summary>
            /// <param name="errors">The collection of exceptions to populate.</param>
            private void Dispose(List<Exception> errors)
            {
                try
                {
                    local.Dispose();
                }
                catch (Exception e)
                {
                    errors.Add(e);
                }

                parent.Detach(this);

                composable.Dispose();

                catalog.Dispose();

                aggregate.Dispose();

                if (parts != null)
                {
                    foreach (ComposablePart part in parts)
                    {
                        try
                        {
                            Disposable.Dispose(part);
                        }
                        catch (Exception e)
                        {
                            errors.Add(e);
                        }
                    }

                    parts = null;
                }
            }

            /// <summary>
            /// Releases all resources used by the <see cref="XContainer"/>.
            /// </summary>
            /// <remarks>This method will also dispose any <see cref="XContainer"/> existing in the associated controller subtree.</remarks>
            public void Dispose()
            {
                VerifyAccess();

                List<Exception> errors = new List<Exception>();

                List<XContainer> traverse = new List<XContainer>();

                using (Sync.Write())
                {
                    if (controller.container == this)
                    {
                        Disposing(traverse);

                        for (int i = 0; i < traverse.Count; i++)
                        {
                            traverse[i].Dispose(errors);
                        }
                    }
                }

                for (int i = 0; i < traverse.Count; i++)
                {
                    try
                    {
                        traverse[i].controller.OnDeactivated();
                    }
                    catch (Exception e)
                    {
                        errors.Add(e);
                    }
                }

                if (errors.Count > 0)
                {
                    throw Error.InvalidOperation(Strings.ContainerDisposeException, Error.Aggregate(errors.ToArray()));
                }
            }

            /// <summary>
            /// Called when the provided exports are changing.
            /// </summary>
            /// <param name="sender">The source of the event.</param>
            /// <param name="e">An <see cref="ExportsChangeEventArgs"/> that contains the event data.</param>
            private void OnExportsChanging(object sender, ExportsChangeEventArgs e)
            {
                if (e.AtomicComposition.TryGetValue(sender, out IEnumerable<ComposablePart> parts))
                {
                    ComposableCompleted completed = new ComposableCompleted(this, parts);

                    e.AtomicComposition.AddCompleteAction(completed.Dispose);

                    return;
                }

                throw Error.NotImplemented();
            }

            /// <summary>
            /// Attaches the specified container within the hierarchy.
            /// </summary>
            /// <param name="container">The container to attach.</param>
            void IComposable.Attach(XContainer container)
            {
                Attach(container, container);

                containers.Add(container);
            }

            /// <summary>
            /// Attaches the specified container within the hierarchy.
            /// </summary>
            /// <param name="container">The container to attach.</param>
            /// <param name="current">The traversing container.</param>
            void IComposable.Attach(XContainer container, XContainer current)
            {
                Attach(container, current);
            }

            /// <summary>
            /// Attaches the specified container within the hierarchy.
            /// </summary>
            /// <param name="container">The container to attach.</param>
            /// <param name="current">The traversing container.</param>
            private void Attach(XContainer container, XContainer current)
            {
                scope1.Add(container.local);

                container.scope2.Add(local);

                for (int i = 0; i < containers.Count; i++)
                {
                    if (containers[i] != current)
                    {
                        containers[i].Attach1(container);
                    }
                }

                parent.Attach(container, this);
            }

            /// <summary>
            /// Attaches the specified container within the internal hierarchy.
            /// </summary>
            /// <param name="container">The container to attach.</param>
            private void Attach1(XContainer container)
            {
                scope1.Add(container.local);

                container.scope1.Add(local);

                for (int i = 0; i < containers.Count; i++)
                {
                    containers[i].Attach1(container);
                }
            }

            /// <summary>
            /// Attaches the specified container within the public hierarchy.
            /// </summary>
            /// <param name="container">The container to attach.</param>
            private void Attach0(XContainer container)
            {
                scope0.Add(container.local);

                container.scope0.Add(local);

                for (int i = 0; i < containers.Count; i++)
                {
                    containers[i].Attach0(container);
                }
            }

            /// <summary>
            /// Detaches the specified container from the hierarchy.
            /// </summary>
            /// <param name="container">The container to detach.</param>
            void IComposable.Detach(XContainer container)
            {
                containers.Remove(container);

                Detach(container, container);
            }

            /// <summary>
            /// Detaches the specified container from the hierarchy.
            /// </summary>
            /// <param name="container">The container to detach.</param>
            /// <param name="current">The traversing container.</param>
            void IComposable.Detach(XContainer container, XContainer current)
            {
                Detach(container, current);
            }

            /// <summary>
            /// Detaches the specified container from the hierarchy.
            /// </summary>
            /// <param name="container">The container to detach.</param>
            /// <param name="current">The traversing container.</param>
            private void Detach(XContainer container, XContainer current)
            {
                parent.Detach(container, this);

                for (int i = 0; i < containers.Count; i++)
                {
                    if (containers[i] != current)
                    {
                        containers[i].Detach1(container);
                    }
                }

                container.scope2.Remove(local);

                scope1.Remove(container.local);
            }

            /// <summary>
            /// Detaches the specified container from the internal hierarchy.
            /// </summary>
            /// <param name="container">The container to detach.</param>
            private void Detach1(XContainer container)
            {
                for (int i = 0; i < containers.Count; i++)
                {
                    containers[i].Detach1(container);
                }

                container.scope1.Remove(local);

                scope1.Remove(container.local);
            }

            /// <summary>
            /// Detaches the specified container from the public hierarchy.
            /// </summary>
            /// <param name="container">The container to detach.</param>
            private void Detach0(XContainer container)
            {
                for (int i = 0; i < containers.Count; i++)
                {
                    containers[i].Detach0(container);
                }

                container.scope0.Remove(local);

                scope0.Remove(container.local);
            }

            /// <summary>
            /// Enforces that the calling thread has access to the <see cref="XContainer"/>.
            /// </summary>
            private static void VerifyAccess()
            {
                Composable.VerifyAccess();
            }

            #endregion

            #region Nested types

            /// <summary>
            /// Represents a top-level composition element.
            /// </summary>
            private class Composable : IComposable
            {
                #region Fields

                /// <summary>
                /// The <see cref="Composable"/> singleton instance.
                /// </summary>
                internal static readonly Composable Instance = new Composable();

                /// <summary>
                /// The descendant containers.
                /// </summary>
                private readonly List<XContainer> containers = new List<XContainer>();

                #endregion

                #region Properties

                /// <summary>
                /// Gets the <see cref="System.Windows.Threading.Dispatcher"/> the <see cref="Composable"/> is associated with.
                /// </summary>
                /// <value>The associated <see cref="System.Windows.Threading.Dispatcher"/>.</value>
                internal static Dispatcher Dispatcher
                {
                    get { return UIThread.Dispatcher; }
                }

                #endregion

                #region Constructors

                /// <summary>
                /// Prevents a default instance of the <see cref="Composable"/> class from being created.
                /// </summary>
                private Composable()
                {
                }

                #endregion

                #region Methods

                /// <summary>
                /// Attaches the specified container within the hierarchy.
                /// </summary>
                /// <param name="container">The container to attach.</param>
                public void Attach(XContainer container)
                {
                    Attach(container, container);

                    containers.Add(container);
                }

                /// <summary>
                /// Attaches the specified container within the hierarchy.
                /// </summary>
                /// <param name="container">The container to attach.</param>
                /// <param name="current">The traversing container.</param>
                public void Attach(XContainer container, XContainer current)
                {
                    for (int i = 0; i < containers.Count; i++)
                    {
                        if (containers[i] != current)
                        {
                            containers[i].Attach0(container);
                        }
                    }
                }

                /// <summary>
                /// Detaches the specified container from the hierarchy.
                /// </summary>
                /// <param name="container">The container to detach.</param>
                public void Detach(XContainer container)
                {
                    containers.Remove(container);

                    Detach(container, container);
                }

                /// <summary>
                /// Detaches the specified container from the hierarchy.
                /// </summary>
                /// <param name="container">The container to detach.</param>
                /// <param name="current">The traversing container.</param>
                public void Detach(XContainer container, XContainer current)
                {
                    for (int i = 0; i < containers.Count; i++)
                    {
                        if (containers[i] != current)
                        {
                            containers[i].Detach0(container);
                        }
                    }
                }

                /// <summary>
                /// Enforces that the calling thread has access to the <see cref="Composable"/>.
                /// </summary>
                internal static void VerifyAccess()
                {
                    UIThread.VerifyAccess();
                }

                #endregion
            }

            /// <summary>
            /// A catalog that combines the elements of <see cref="ComposablePartCatalog"/> objects.
            /// </summary>
            private class PrivateAggregateCatalog : AggregateCatalog
            {
                #region Methods

                /// <summary>
                /// Raises the <see cref="AggregateCatalog.Changing"/> event.
                /// </summary>
                /// <param name="e">A <see cref="ComposablePartCatalogChangeEventArgs"/> object that contains the event data.</param>
                protected override void OnChanging(ComposablePartCatalogChangeEventArgs e)
                {
                    Composable.VerifyAccess();

                    using (Sync.Write())
                    {
                        base.OnChanging(e);
                    }
                }

                #endregion
            }

            /// <summary>
            /// Retrieves exports provided by a collection of <see cref="ExportProvider"/> objects.
            /// </summary>
            private class ExtensionProvider : AggregateExportProvider
            {
                #region Fields

                /// <summary>
                /// The underlying container.
                /// </summary>
                private readonly XContainer container;

                /// <summary>
                /// A value indicating whether the <see cref="ExtensionProvider"/> is rejected.
                /// </summary>
                private bool rejected;

                #endregion

                #region Constructors

                /// <summary>
                /// Initializes a new instance of the <see cref="ExtensionProvider"/> class.
                /// </summary>
                /// <param name="container">The underlying container.</param>
                /// <param name="providers">The prioritized list of export providers.</param>
                internal ExtensionProvider(XContainer container, params ExportProvider[] providers) : base(providers)
                {
                    this.container = container;
                }

                #endregion

                #region Methods

                /// <summary>
                /// Gets all the exports that match the constraint defined by the specified definition.
                /// </summary>
                /// <param name="definition">The object that defines the conditions of the <see cref="Export"/> objects to return.</param>
                /// <param name="atomicComposition">The transactional container for the composition.</param>
                /// <returns>A collection that contains all the exports that match the specified condition.</returns>
                protected override IEnumerable<Export> GetExportsCore(ImportDefinition definition, AtomicComposition atomicComposition)
                {
                    if (rejected)
                    {
                        return null;
                    }

                    return base.GetExportsCore(definition, atomicComposition);
                }

                /// <summary>
                /// Releases resources used by the <see cref="ExtensionProvider"/>.
                /// </summary>
                /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
                protected override void Dispose(bool disposing)
                {
                    if (disposing)
                    {
                        rejected = true;

                        List<ExportDefinition> exports = new List<ExportDefinition>();

                        if (container.parts != null)
                        {
                            exports.AddRange(container.parts.SelectMany(p => p.ExportDefinitions));
                        }

                        exports.AddRange(container.aggregate.Catalogs.SelectMany(c => c).SelectMany(d => d.ExportDefinitions));

                        using (AtomicComposition composition = new AtomicComposition())
                        {
                            OnExportsChanging(new ExportsChangeEventArgs(Enumerable.Empty<ExportDefinition>(), exports, composition));

                            composition.Complete();
                        }

                        OnExportsChanged(new ExportsChangeEventArgs(Enumerable.Empty<ExportDefinition>(), exports, null));
                    }

                    base.Dispose(disposing);
                }

                #endregion
            }

            /// <summary>
            /// Represents an action to be executed when the composition operation completes.
            /// </summary>
            private class ComposableCompleted : IDisposable
            {
                #region Fields

                /// <summary>
                /// The underlying container.
                /// </summary>
                private readonly XContainer container;

                /// <summary>
                /// The composed parts collection.
                /// </summary>
                private readonly IEnumerable<ComposablePart> parts;

                #endregion

                #region Constructors

                /// <summary>
                /// Initializes a new instance of the <see cref="ComposableCompleted"/> class.
                /// </summary>
                /// <param name="container">The underlying container.</param>
                /// <param name="parts">The composed parts collection.</param>
                internal ComposableCompleted(XContainer container, IEnumerable<ComposablePart> parts)
                {
                    this.container = container;

                    this.parts = parts;
                }

                #endregion

                #region Methods

                /// <summary>
                /// Executed when the composition operation completes.
                /// </summary>
                public void Dispose()
                {
                    container.parts = parts;
                }

                #endregion
            }

            /// <summary>
            /// Retrieves exports provided by a collection of <see cref="ExportProvider"/> objects.
            /// </summary>
            private abstract class CompositeProvider : ExportProvider
            {
                #region Fields

                /// <summary>
                /// The providers collection.
                /// </summary>
                private readonly List<ExportProvider> providers = new List<ExportProvider>();

                #endregion

                #region Methods

                /// <summary>
                /// Adds the specified <see cref="ExportProvider"/> to the collection.
                /// </summary>
                /// <param name="provider">The provider to add to the <see cref="CompositeProvider"/>.</param>
                internal void Add(ExportProvider provider)
                {
                    provider.ExportsChanging += OnExportsChanging;
                    provider.ExportsChanged += OnExportsChanged;

                    providers.Add(provider);
                }

                /// <summary>
                /// Removes the specified <see cref="ExportProvider"/> from the collection.
                /// </summary>
                /// <param name="provider">The provider to remove from the <see cref="CompositeProvider"/>.</param>
                internal void Remove(ExportProvider provider)
                {
                    providers.Remove(provider);

                    provider.ExportsChanged -= OnExportsChanged;
                    provider.ExportsChanging -= OnExportsChanging;
                }

                /// <summary>
                /// Gets all the exports that match the constraint defined by the specified definition.
                /// </summary>
                /// <param name="definition">The object that defines the conditions of the <see cref="Export"/> objects to return.</param>
                /// <param name="atomicComposition">The transactional container for the composition.</param>
                /// <returns>A collection that contains all the exports that match the specified condition.</returns>
                protected override IEnumerable<Export> GetExportsCore(ImportDefinition definition, AtomicComposition atomicComposition)
                {
                    List<Export> list = new List<Export>();

                    for (int i = 0; i < providers.Count; i++)
                    {
                        if (providers[i].TryGetExports(definition, atomicComposition, out IEnumerable<Export> exports) && exports.FastAny())
                        {
                            list.AddRange(exports);
                        }
                    }

                    return list;
                }

                /// <summary>
                /// Called when the provided exports are changing.
                /// </summary>
                /// <param name="sender">The source of the event.</param>
                /// <param name="e">An <see cref="ExportsChangeEventArgs"/> that contains the event data.</param>
                private void OnExportsChanging(object sender, ExportsChangeEventArgs e)
                {
                    OnExportsChanging(e);
                }

                /// <summary>
                /// Called when the exports in the provider change.
                /// </summary>
                /// <param name="sender">The source of the event.</param>
                /// <param name="e">An <see cref="ExportsChangeEventArgs"/> that contains the event data.</param>
                private void OnExportsChanged(object sender, ExportsChangeEventArgs e)
                {
                    OnExportsChanged(e);
                }

                #endregion
            }

            /// <summary>
            /// Wraps the <see cref="ImportDefinition"/> object.
            /// </summary>
            private abstract class ImportDefinitionProxy : ImportDefinition
            {
                #region Fields

                /// <summary>
                /// The wrapped import definition.
                /// </summary>
                private readonly ImportDefinition definition;

                #endregion

                #region Properties

                /// <summary>
                /// Gets the cardinality of the exports required by the import definition.
                /// </summary>
                /// <value>The cardinality of the <see cref="Export"/> objects required by the <see cref="ImportDefinition"/>.</value>
                public override ImportCardinality Cardinality
                {
                    get { return definition.Cardinality; }
                }

                /// <summary>
                /// Gets an expression that defines conditions that the import must satisfy to match the import definition.
                /// </summary>
                /// <value>An expression that contains a <see cref="Func{T, TResult}"/> object that defines the conditions an <see cref="Export"/> must satisfy to match the <see cref="ImportDefinition"/>.</value>
                public override Expression<Func<ExportDefinition, bool>> Constraint
                {
                    get { return definition.Constraint; }
                }

                /// <summary>
                /// Gets the name of the contract.
                /// </summary>
                /// <value>The contract name.</value>
                public override string ContractName
                {
                    get { return definition.ContractName; }
                }

                /// <summary>
                /// Gets a value that indicates whether the import definition must be satisfied before a part can start producing exported objects.
                /// </summary>
                /// <value><c>true</c> if the <see cref="ImportDefinition"/> must be satisfied before a <see cref="ComposablePart"/> object can start producing exported objects; otherwise, <c>false</c>.</value>
                public override bool IsPrerequisite
                {
                    get { return definition.IsPrerequisite; }
                }

                /// <summary>
                /// Gets a value that indicates whether the import definition can be satisfied multiple times.
                /// </summary>
                /// <value><c>true</c> if the <see cref="ImportDefinition"/> can be satisfied multiple times throughout the lifetime of a <see cref="ComposablePart"/> object; otherwise, <c>false</c>.</value>
                public override bool IsRecomposable
                {
                    get { return definition.IsRecomposable; }
                }

                /// <summary>
                /// Gets the metadata associated with this import.
                /// </summary>
                /// <value>A collection that contains the metadata associated with this import.</value>
                public override IDictionary<string, object> Metadata
                {
                    get { return definition.Metadata; }
                }

                #endregion

                #region Constructors

                /// <summary>
                /// Initializes a new instance of the <see cref="ImportDefinitionProxy"/> class.
                /// </summary>
                /// <param name="definition">The import definition to wrap.</param>
                protected ImportDefinitionProxy(ImportDefinition definition)
                {
                    this.definition = definition;
                }

                #endregion

                #region Methods

                /// <summary>
                /// Gets a value that indicates whether the export represented by the specified definition satisfies the constraints of this import definition.
                /// </summary>
                /// <param name="exportDefinition">The export definition to test.</param>
                /// <returns><c>true</c> if the constraints are satisfied; otherwise, <c>false</c>.</returns>
                public override bool IsConstraintSatisfiedBy(ExportDefinition exportDefinition)
                {
                    return definition.IsConstraintSatisfiedBy(exportDefinition);
                }

                #endregion
            }

            /// <summary>
            /// Wraps the <see cref="ContractBasedImportDefinition"/> object.
            /// </summary>
            private abstract class ContractBasedImportDefinitionProxy : ContractBasedImportDefinition
            {
                #region Fields

                /// <summary>
                /// The wrapped import definition.
                /// </summary>
                private readonly ContractBasedImportDefinition definition;

                #endregion

                #region Properties

                /// <summary>
                /// Gets the cardinality of the exports required by the import definition.
                /// </summary>
                /// <value>The cardinality of the <see cref="Export"/> objects required by the <see cref="ImportDefinition"/>.</value>
                public override ImportCardinality Cardinality
                {
                    get { return definition.Cardinality; }
                }

                /// <summary>
                /// Gets an expression that defines conditions that must be matched to satisfy the import described by this import definition.
                /// </summary>
                /// <value>An expression that contains a <see cref="Func{T, TResult}"/> object that defines the conditions that must be matched for the <see cref="ImportDefinition"/> to be satisfied by an <see cref="Export"/>.</value>
                public override Expression<Func<ExportDefinition, bool>> Constraint
                {
                    get { return definition.Constraint; }
                }

                /// <summary>
                /// Gets the name of the contract.
                /// </summary>
                /// <value>The contract name.</value>
                public override string ContractName
                {
                    get { return definition.ContractName; }
                }

                /// <summary>
                /// Gets a value that indicates whether the import definition must be satisfied before a part can start producing exported objects.
                /// </summary>
                /// <value><c>true</c> if the <see cref="ImportDefinition"/> must be satisfied before a <see cref="ComposablePart"/> object can start producing exported objects; otherwise, <c>false</c>.</value>
                public override bool IsPrerequisite
                {
                    get { return definition.IsPrerequisite; }
                }

                /// <summary>
                /// Gets a value that indicates whether the import definition can be satisfied multiple times.
                /// </summary>
                /// <value><c>true</c> if the <see cref="ImportDefinition"/> can be satisfied multiple times throughout the lifetime of a <see cref="ComposablePart"/> object; otherwise, <c>false</c>.</value>
                public override bool IsRecomposable
                {
                    get { return definition.IsRecomposable; }
                }

                /// <summary>
                /// Gets the metadata associated with this import.
                /// </summary>
                /// <value>A collection that contains the metadata associated with this import.</value>
                public override IDictionary<string, object> Metadata
                {
                    get { return definition.Metadata; }
                }

                /// <summary>
                /// Gets a value that indicates that the importer requires a specific <see cref="CreationPolicy"/> for the exports used to satisfy this import.
                /// </summary>
                /// <value>The creation policy of the <see cref="Export"/> objects required by the <see cref="ImportDefinition"/>.</value>
                public override CreationPolicy RequiredCreationPolicy
                {
                    get { return definition.RequiredCreationPolicy; }
                }

                /// <summary>
                /// Gets the metadata names of the export required by the import definition.
                /// </summary>
                /// <value>A collection of <see cref="String"/> objects that contain the metadata names of the <see cref="Export"/> objects required by the <see cref="ContractBasedImportDefinition"/>.</value>
                public override IEnumerable<KeyValuePair<string, Type>> RequiredMetadata
                {
                    get { return definition.RequiredMetadata; }
                }

                /// <summary>
                /// Gets the expected type of the export that matches this <see cref="ContractBasedImportDefinition"/>.
                /// </summary>
                /// <value>A string that is generated by calling the <see cref="AttributedModelServices.GetTypeIdentity(Type)"/> method on the type that this import expects.</value>
                public override string RequiredTypeIdentity
                {
                    get { return definition.RequiredTypeIdentity; }
                }

                #endregion

                #region Constructors

                /// <summary>
                /// Initializes a new instance of the <see cref="ContractBasedImportDefinitionProxy"/> class.
                /// </summary>
                /// <param name="definition">The import definition to wrap.</param>
                protected ContractBasedImportDefinitionProxy(ContractBasedImportDefinition definition)
                {
                    this.definition = definition;
                }

                #endregion

                #region Methods

                /// <summary>
                /// Returns a value indicating whether the constraint represented by this object is satisfied by the export represented by the given export definition.
                /// </summary>
                /// <param name="exportDefinition">The export definition to test.</param>
                /// <returns><c>true</c> if the constraint is satisfied; otherwise, <c>false</c>.</returns>
                public override bool IsConstraintSatisfiedBy(ExportDefinition exportDefinition)
                {
                    return definition.IsConstraintSatisfiedBy(exportDefinition);
                }

                #endregion
            }

            /// <summary>
            /// Retrieves exports provided by the public scope.
            /// </summary>
            private class Scope0 : CompositeProvider
            {
                #region Methods

                /// <summary>
                /// Gets all the exports that match the constraint defined by the specified definition.
                /// </summary>
                /// <param name="definition">The object that defines the conditions of the <see cref="Export"/> objects to return.</param>
                /// <param name="atomicComposition">The transactional container for the composition.</param>
                /// <returns>A collection that contains all the exports that match the specified condition.</returns>
                protected override IEnumerable<Export> GetExportsCore(ImportDefinition definition, AtomicComposition atomicComposition)
                {
                    return base.GetExportsCore(Filtered(definition), atomicComposition);
                }

                /// <summary>
                /// Raises the <see cref="ExportProvider.ExportsChanging"/> event.
                /// </summary>
                /// <param name="e">An <see cref="ExportsChangeEventArgs"/> that contains the event data.</param>
                protected override void OnExportsChanging(ExportsChangeEventArgs e)
                {
                    List<ExportDefinition> added = Process(e.AddedExports);
                    List<ExportDefinition> removed = Process(e.RemovedExports);
                    if (added.Count + removed.Count > 0)
                    {
                        base.OnExportsChanging(new ExportsChangeEventArgs(added, removed, e.AtomicComposition));
                    }
                }

                /// <summary>
                /// Raises the <see cref="ExportProvider.ExportsChanged"/> event.
                /// </summary>
                /// <param name="e">An <see cref="ExportsChangeEventArgs"/> that contains the event data.</param>
                protected override void OnExportsChanged(ExportsChangeEventArgs e)
                {
                    List<ExportDefinition> added = Process(e.AddedExports);
                    List<ExportDefinition> removed = Process(e.RemovedExports);
                    if (added.Count + removed.Count > 0)
                    {
                        base.OnExportsChanged(new ExportsChangeEventArgs(added, removed, e.AtomicComposition));
                    }
                }

                /// <summary>
                /// Specifies the scope constraint predicate to match the specified export definition.
                /// </summary>
                /// <param name="definition">The export definition to test.</param>
                /// <returns><c>true</c> if the constraint is satisfied; otherwise, <c>false</c>.</returns>
                private static bool Match(ExportDefinition definition)
                {
                    return !definition.Metadata.TryGetValue(ExportScopeMetadataName, out object value) || (value is ExportScope && (int)value < 1);
                }

                /// <summary>
                /// Filters the specified exports collection applying the scope constraint predicate.
                /// </summary>
                /// <param name="definitions">The exports collection to filter.</param>
                /// <returns>The exports that match the scope.</returns>
                private static List<ExportDefinition> Process(IEnumerable<ExportDefinition> definitions)
                {
                    List<ExportDefinition> list = new List<ExportDefinition>();

                    foreach (ExportDefinition definition in definitions)
                    {
                        if (Match(definition))
                        {
                            list.Add(definition);
                        }
                    }

                    return list;
                }

                /// <summary>
                /// Creates a filtered <see cref="ImportDefinition"/> for the specified definition object.
                /// </summary>
                /// <param name="definition">The import definition to filter.</param>
                /// <returns>A filtered import for the <paramref name="definition"/>.</returns>
                private static ImportDefinition Filtered(ImportDefinition definition)
                {
                    ContractBasedImportDefinition contract = definition as ContractBasedImportDefinition;
                    if (contract != null)
                    {
                        return new FilteredContractBasedImportDefinition(contract);
                    }

                    return new FilteredImportDefinition(definition);
                }

                #endregion

                #region Nested types

                /// <summary>
                /// Represents an import that is constrained to the scope exports only.
                /// </summary>
                private class FilteredImportDefinition : ImportDefinitionProxy
                {
                    #region Constructors

                    /// <summary>
                    /// Initializes a new instance of the <see cref="FilteredImportDefinition"/> class.
                    /// </summary>
                    /// <param name="definition">The import definition to filter.</param>
                    internal FilteredImportDefinition(ImportDefinition definition) : base(definition)
                    {
                    }

                    #endregion

                    #region Methods

                    /// <summary>
                    /// Gets a value that indicates whether the export represented by the specified definition satisfies the constraints of this import definition.
                    /// </summary>
                    /// <param name="exportDefinition">The export definition to test.</param>
                    /// <returns><c>true</c> if the constraints are satisfied; otherwise, <c>false</c>.</returns>
                    public override bool IsConstraintSatisfiedBy(ExportDefinition exportDefinition)
                    {
                        return base.IsConstraintSatisfiedBy(exportDefinition) && Match(exportDefinition);
                    }

                    #endregion
                }

                /// <summary>
                /// Represents an import that is constrained to the scope exports only.
                /// </summary>
                private class FilteredContractBasedImportDefinition : ContractBasedImportDefinitionProxy
                {
                    #region Constructors

                    /// <summary>
                    /// Initializes a new instance of the <see cref="FilteredContractBasedImportDefinition"/> class.
                    /// </summary>
                    /// <param name="definition">The import definition to filter.</param>
                    internal FilteredContractBasedImportDefinition(ContractBasedImportDefinition definition) : base(definition)
                    {
                    }

                    #endregion

                    #region Methods

                    /// <summary>
                    /// Returns a value indicating whether the constraint represented by this object is satisfied by the export represented by the given export definition.
                    /// </summary>
                    /// <param name="exportDefinition">The export definition to test.</param>
                    /// <returns><c>true</c> if the constraint is satisfied; otherwise, <c>false</c>.</returns>
                    public override bool IsConstraintSatisfiedBy(ExportDefinition exportDefinition)
                    {
                        return base.IsConstraintSatisfiedBy(exportDefinition) && Match(exportDefinition);
                    }

                    #endregion
                }

                #endregion
            }

            /// <summary>
            /// Retrieves exports provided by the internal scope.
            /// </summary>
            private class Scope1 : CompositeProvider
            {
                #region Methods

                /// <summary>
                /// Gets all the exports that match the constraint defined by the specified definition.
                /// </summary>
                /// <param name="definition">The object that defines the conditions of the <see cref="Export"/> objects to return.</param>
                /// <param name="atomicComposition">The transactional container for the composition.</param>
                /// <returns>A collection that contains all the exports that match the specified condition.</returns>
                protected override IEnumerable<Export> GetExportsCore(ImportDefinition definition, AtomicComposition atomicComposition)
                {
                    return base.GetExportsCore(Filtered(definition), atomicComposition);
                }

                /// <summary>
                /// Raises the <see cref="ExportProvider.ExportsChanging"/> event.
                /// </summary>
                /// <param name="e">An <see cref="ExportsChangeEventArgs"/> that contains the event data.</param>
                protected override void OnExportsChanging(ExportsChangeEventArgs e)
                {
                    List<ExportDefinition> added = Process(e.AddedExports);
                    List<ExportDefinition> removed = Process(e.RemovedExports);
                    if (added.Count + removed.Count > 0)
                    {
                        base.OnExportsChanging(new ExportsChangeEventArgs(added, removed, e.AtomicComposition));
                    }
                }

                /// <summary>
                /// Raises the <see cref="ExportProvider.ExportsChanged"/> event.
                /// </summary>
                /// <param name="e">An <see cref="ExportsChangeEventArgs"/> that contains the event data.</param>
                protected override void OnExportsChanged(ExportsChangeEventArgs e)
                {
                    List<ExportDefinition> added = Process(e.AddedExports);
                    List<ExportDefinition> removed = Process(e.RemovedExports);
                    if (added.Count + removed.Count > 0)
                    {
                        base.OnExportsChanged(new ExportsChangeEventArgs(added, removed, e.AtomicComposition));
                    }
                }

                /// <summary>
                /// Specifies the scope constraint predicate to match the specified export definition.
                /// </summary>
                /// <param name="definition">The export definition to test.</param>
                /// <returns><c>true</c> if the constraint is satisfied; otherwise, <c>false</c>.</returns>
                private static bool Match(ExportDefinition definition)
                {
                    return !definition.Metadata.TryGetValue(ExportScopeMetadataName, out object value) || (value is ExportScope && (int)value < 2);
                }

                /// <summary>
                /// Filters the specified exports collection applying the scope constraint predicate.
                /// </summary>
                /// <param name="definitions">The exports collection to filter.</param>
                /// <returns>The exports that match the scope.</returns>
                private static List<ExportDefinition> Process(IEnumerable<ExportDefinition> definitions)
                {
                    List<ExportDefinition> list = new List<ExportDefinition>();

                    foreach (ExportDefinition definition in definitions)
                    {
                        if (Match(definition))
                        {
                            list.Add(definition);
                        }
                    }

                    return list;
                }

                /// <summary>
                /// Creates a filtered <see cref="ImportDefinition"/> for the specified definition object.
                /// </summary>
                /// <param name="definition">The import definition to filter.</param>
                /// <returns>A filtered import for the <paramref name="definition"/>.</returns>
                private static ImportDefinition Filtered(ImportDefinition definition)
                {
                    ContractBasedImportDefinition contract = definition as ContractBasedImportDefinition;
                    if (contract != null)
                    {
                        return new FilteredContractBasedImportDefinition(contract);
                    }

                    return new FilteredImportDefinition(definition);
                }

                #endregion

                #region Nested types

                /// <summary>
                /// Represents an import that is constrained to the scope exports only.
                /// </summary>
                private class FilteredImportDefinition : ImportDefinitionProxy
                {
                    #region Constructors

                    /// <summary>
                    /// Initializes a new instance of the <see cref="FilteredImportDefinition"/> class.
                    /// </summary>
                    /// <param name="definition">The import definition to filter.</param>
                    internal FilteredImportDefinition(ImportDefinition definition) : base(definition)
                    {
                    }

                    #endregion

                    #region Methods

                    /// <summary>
                    /// Gets a value that indicates whether the export represented by the specified definition satisfies the constraints of this import definition.
                    /// </summary>
                    /// <param name="exportDefinition">The export definition to test.</param>
                    /// <returns><c>true</c> if the constraints are satisfied; otherwise, <c>false</c>.</returns>
                    public override bool IsConstraintSatisfiedBy(ExportDefinition exportDefinition)
                    {
                        return base.IsConstraintSatisfiedBy(exportDefinition) && Match(exportDefinition);
                    }

                    #endregion
                }

                /// <summary>
                /// Represents an import that is constrained to the scope exports only.
                /// </summary>
                private class FilteredContractBasedImportDefinition : ContractBasedImportDefinitionProxy
                {
                    #region Constructors

                    /// <summary>
                    /// Initializes a new instance of the <see cref="FilteredContractBasedImportDefinition"/> class.
                    /// </summary>
                    /// <param name="definition">The import definition to filter.</param>
                    internal FilteredContractBasedImportDefinition(ContractBasedImportDefinition definition) : base(definition)
                    {
                    }

                    #endregion

                    #region Methods

                    /// <summary>
                    /// Returns a value indicating whether the constraint represented by this object is satisfied by the export represented by the given export definition.
                    /// </summary>
                    /// <param name="exportDefinition">The export definition to test.</param>
                    /// <returns><c>true</c> if the constraint is satisfied; otherwise, <c>false</c>.</returns>
                    public override bool IsConstraintSatisfiedBy(ExportDefinition exportDefinition)
                    {
                        return base.IsConstraintSatisfiedBy(exportDefinition) && Match(exportDefinition);
                    }

                    #endregion
                }

                #endregion
            }

            /// <summary>
            /// Retrieves exports provided by the protected scope.
            /// </summary>
            private class Scope2 : CompositeProvider
            {
                #region Methods

                /// <summary>
                /// Gets all the exports that match the constraint defined by the specified definition.
                /// </summary>
                /// <param name="definition">The object that defines the conditions of the <see cref="Export"/> objects to return.</param>
                /// <param name="atomicComposition">The transactional container for the composition.</param>
                /// <returns>A collection that contains all the exports that match the specified condition.</returns>
                protected override IEnumerable<Export> GetExportsCore(ImportDefinition definition, AtomicComposition atomicComposition)
                {
                    return base.GetExportsCore(Filtered(definition), atomicComposition);
                }

                /// <summary>
                /// Raises the <see cref="ExportProvider.ExportsChanging"/> event.
                /// </summary>
                /// <param name="e">An <see cref="ExportsChangeEventArgs"/> that contains the event data.</param>
                protected override void OnExportsChanging(ExportsChangeEventArgs e)
                {
                    List<ExportDefinition> added = Process(e.AddedExports);
                    List<ExportDefinition> removed = Process(e.RemovedExports);
                    if (added.Count + removed.Count > 0)
                    {
                        base.OnExportsChanging(new ExportsChangeEventArgs(added, removed, e.AtomicComposition));
                    }
                }

                /// <summary>
                /// Raises the <see cref="ExportProvider.ExportsChanged"/> event.
                /// </summary>
                /// <param name="e">An <see cref="ExportsChangeEventArgs"/> that contains the event data.</param>
                protected override void OnExportsChanged(ExportsChangeEventArgs e)
                {
                    List<ExportDefinition> added = Process(e.AddedExports);
                    List<ExportDefinition> removed = Process(e.RemovedExports);
                    if (added.Count + removed.Count > 0)
                    {
                        base.OnExportsChanged(new ExportsChangeEventArgs(added, removed, e.AtomicComposition));
                    }
                }

                /// <summary>
                /// Specifies the scope constraint predicate to match the specified export definition.
                /// </summary>
                /// <param name="definition">The export definition to test.</param>
                /// <returns><c>true</c> if the constraint is satisfied; otherwise, <c>false</c>.</returns>
                private static bool Match(ExportDefinition definition)
                {
                    return !definition.Metadata.TryGetValue(ExportScopeMetadataName, out object value) || (value is ExportScope && (int)value < 3);
                }

                /// <summary>
                /// Filters the specified exports collection applying the scope constraint predicate.
                /// </summary>
                /// <param name="definitions">The exports collection to filter.</param>
                /// <returns>The exports that match the scope.</returns>
                private static List<ExportDefinition> Process(IEnumerable<ExportDefinition> definitions)
                {
                    List<ExportDefinition> list = new List<ExportDefinition>();

                    foreach (ExportDefinition definition in definitions)
                    {
                        if (Match(definition))
                        {
                            list.Add(definition);
                        }
                    }

                    return list;
                }

                /// <summary>
                /// Creates a filtered <see cref="ImportDefinition"/> for the specified definition object.
                /// </summary>
                /// <param name="definition">The import definition to filter.</param>
                /// <returns>A filtered import for the <paramref name="definition"/>.</returns>
                private static ImportDefinition Filtered(ImportDefinition definition)
                {
                    ContractBasedImportDefinition contract = definition as ContractBasedImportDefinition;
                    if (contract != null)
                    {
                        return new FilteredContractBasedImportDefinition(contract);
                    }

                    return new FilteredImportDefinition(definition);
                }

                #endregion

                #region Nested types

                /// <summary>
                /// Represents an import that is constrained to the scope exports only.
                /// </summary>
                private class FilteredImportDefinition : ImportDefinitionProxy
                {
                    #region Constructors

                    /// <summary>
                    /// Initializes a new instance of the <see cref="FilteredImportDefinition"/> class.
                    /// </summary>
                    /// <param name="definition">The import definition to filter.</param>
                    internal FilteredImportDefinition(ImportDefinition definition) : base(definition)
                    {
                    }

                    #endregion

                    #region Methods

                    /// <summary>
                    /// Gets a value that indicates whether the export represented by the specified definition satisfies the constraints of this import definition.
                    /// </summary>
                    /// <param name="exportDefinition">The export definition to test.</param>
                    /// <returns><c>true</c> if the constraints are satisfied; otherwise, <c>false</c>.</returns>
                    public override bool IsConstraintSatisfiedBy(ExportDefinition exportDefinition)
                    {
                        return base.IsConstraintSatisfiedBy(exportDefinition) && Match(exportDefinition);
                    }

                    #endregion
                }

                /// <summary>
                /// Represents an import that is constrained to the scope exports only.
                /// </summary>
                private class FilteredContractBasedImportDefinition : ContractBasedImportDefinitionProxy
                {
                    #region Constructors

                    /// <summary>
                    /// Initializes a new instance of the <see cref="FilteredContractBasedImportDefinition"/> class.
                    /// </summary>
                    /// <param name="definition">The import definition to filter.</param>
                    internal FilteredContractBasedImportDefinition(ContractBasedImportDefinition definition) : base(definition)
                    {
                    }

                    #endregion

                    #region Methods

                    /// <summary>
                    /// Returns a value indicating whether the constraint represented by this object is satisfied by the export represented by the given export definition.
                    /// </summary>
                    /// <param name="exportDefinition">The export definition to test.</param>
                    /// <returns><c>true</c> if the constraint is satisfied; otherwise, <c>false</c>.</returns>
                    public override bool IsConstraintSatisfiedBy(ExportDefinition exportDefinition)
                    {
                        return base.IsConstraintSatisfiedBy(exportDefinition) && Match(exportDefinition);
                    }

                    #endregion
                }

                #endregion
            }

            #endregion
        }

        /// <summary>
        /// Wraps the <see cref="Export"/> object.
        /// </summary>
        private class XExport : Export
        {
            #region Fields

            /// <summary>
            /// The wrapped export.
            /// </summary>
            private readonly Export export;

            #endregion

            #region Properties

            /// <summary>
            /// Gets the definition that describes the contract that the export satisfies.
            /// </summary>
            /// <value>A definition that describes the contract that the <see cref="Export"/> object satisfies.</value>
            public override ExportDefinition Definition
            {
                get { return export.Definition; }
            }

            #endregion

            #region Constructors

            /// <summary>
            /// Initializes a new instance of the <see cref="XExport"/> class.
            /// </summary>
            /// <param name="export">The wrapped export.</param>
            internal XExport(Export export)
            {
                this.export = export;
            }

            #endregion

            #region Methods

            /// <summary>
            /// Returns the exported object the export provides.
            /// </summary>
            /// <returns>The exported object the export provides.</returns>
            protected override object GetExportedValueCore()
            {
                using (Sync.Read())
                {
                    return export.Value;
                }
            }

            #endregion
        }

        /// <summary>
        /// Represents an object that publishes brokered events.
        /// </summary>
        [Export(typeof(IRaiseEvent))]
        [ExportScope(ExportScope.Private)]
        private class EventBroker : Export, IRaiseEvent
        {
            #region Fields

            /// <summary>
            /// The event broker definition.
            /// </summary>
            private static readonly ExportDefinition definition = AttributedModelServices.CreatePartDefinition(typeof(EventBroker), null).ExportDefinitions.First();

            /// <summary>
            /// The underlying controller.
            /// </summary>
            private readonly Controller controller;

            #endregion

            #region Properties

            /// <summary>
            /// Gets the definition that describes the contract that the export satisfies.
            /// </summary>
            /// <value>A definition that describes the contract that the <see cref="Export"/> object satisfies.</value>
            public override ExportDefinition Definition
            {
                get { return definition; }
            }

            #endregion

            #region Constructors

            /// <summary>
            /// Initializes a new instance of the <see cref="EventBroker"/> class.
            /// </summary>
            /// <param name="controller">The underlying controller.</param>
            internal EventBroker(Controller controller)
            {
                this.controller = controller;
            }

            #endregion

            #region Methods

            /// <summary>
            /// Raises the event that matches the specified contract name.
            /// </summary>
            /// <typeparam name="T">The type of the event data.</typeparam>
            /// <param name="contractName">The contract name of the event to raise.</param>
            /// <param name="sender">The source of the event.</param>
            /// <param name="e">An <typeparamref name="T"/> that contains the event data.</param>
            public void Raise<T>(string contractName, object sender, T e) where T : EventArgs
            {
                ContractBasedImportDefinition definition = new ContractBasedImportDefinition(contractName, null, null, ImportCardinality.ZeroOrMore, false, false, CreationPolicy.Shared);

                bool dispatch = !UIThread.CheckAccess();

                List<ExportedDelegate> list1 = null;
                List<ExportedDelegate> list2 = null;

                using (Sync.Read())
                {
                    if (controller.container != null)
                    {
                        foreach (Export export in controller.container.root.GetExports(definition))
                        {
                            ExportedDelegate exported = export.Value as ExportedDelegate;
                            if (exported != null)
                            {
                                if (dispatch && export.Metadata.TryGetValue(RequiresUIThreadMetadataName, out object value) && value is bool && (bool)value)
                                {
                                    if (list2 == null)
                                    {
                                        list2 = new List<ExportedDelegate>();
                                    }

                                    list2.Add(exported);

                                    continue;
                                }

                                if (list1 == null)
                                {
                                    list1 = new List<ExportedDelegate>();
                                }

                                list1.Add(exported);
                            }
                        }
                    }
                }

                if (list1 != null)
                {
                    for (int i = 0; i < list1.Count; i++)
                    {
                        Action<object, T> action = list1[i].CreateDelegate(typeof(Delegate)) as Action<object, T>;
                        if (action != null)
                        {
                            action(sender, e);
                        }
                    }
                }

                if (list2 != null)
                {
                    List<Action<object, T>> list = new List<Action<object, T>>();

                    for (int i = 0; i < list2.Count; i++)
                    {
                        Action<object, T> action = list2[i].CreateDelegate(typeof(Delegate)) as Action<object, T>;
                        if (action != null)
                        {
                            list.Add(action);
                        }
                    }

                    if (list.Count > 0)
                    {
                        UIThread.Invoke(() => DeliverEvent(sender, e, list));
                    }
                }
            }

            /// <summary>
            /// Delivers the event to each subscription in the specified list.
            /// </summary>
            /// <typeparam name="T">The type of the event data.</typeparam>
            /// <param name="sender">The source of the event.</param>
            /// <param name="e">An <typeparamref name="T"/> that contains the event data.</param>
            /// <param name="list">The subscriptions that receive the event.</param>
            private static void DeliverEvent<T>(object sender, T e, List<Action<object, T>> list) where T : EventArgs
            {
                for (int i = 0; i < list.Count; i++)
                {
                    list[i](sender, e);
                }
            }

            /// <summary>
            /// Returns the exported object the export provides.
            /// </summary>
            /// <returns>The exported object the export provides.</returns>
            protected override object GetExportedValueCore()
            {
                return this;
            }

            #endregion
        }

        /// <summary>
        /// Provides helpers for the composition synchronization.
        /// </summary>
        private static class Sync
        {
            #region Fields

            /// <summary>
            /// The synchronization lock.
            /// </summary>
            private static readonly Lock sync = new Lock();

            #endregion

            #region Methods

            /// <summary>
            /// Ensures the reader lock and returns object to be used for releasing the lock.
            /// </summary>
            /// <returns>An <see cref="IDisposable"/> used for releasing the lock.</returns>
            internal static IDisposable Read()
            {
                return sync.IsRead || sync.IsWrite ? null : sync.Read();
            }

            /// <summary>
            /// Ensures the writer lock and returns object to be used for releasing the lock.
            /// </summary>
            /// <returns>An <see cref="IDisposable"/> used for releasing the lock.</returns>
            internal static IDisposable Write()
            {
                return sync.IsWrite ? null : sync.Write();
            }

            #endregion
        }

        #endregion
    }
}
