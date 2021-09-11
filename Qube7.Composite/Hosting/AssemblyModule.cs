using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading;

namespace Qube7.Composite.Hosting
{
    /// <summary>
    /// Discovers a module provided by the assembly resolved from the specified location.
    /// </summary>
    public class AssemblyModule : ModuleIterator
    {
        #region Fields

        /// <summary>
        /// The default absolute <see cref="Uri"/> that is the base for the assembly location.
        /// </summary>
        private static readonly Uri BaseUri = new Uri(AppDomain.CurrentDomain.BaseDirectory);

        /// <summary>
        /// The location of the assembly file.
        /// </summary>
        private Uri assemblyFile;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the identity of the assembly that is to be resolved.
        /// </summary>
        /// <value>The identity of the assembly.</value>
        [TypeConverter(typeof(AssemblyNameConverter))]
        public AssemblyName AssemblyName { get; set; }

        /// <summary>
        /// Gets or sets the location of the file that contains the manifest of the assembly that is to be resolved.
        /// </summary>
        /// <value>The location of the assembly file.</value>
        public Uri AssemblyFile
        {
            get { return assemblyFile; }
            set
            {
                Requires.NotNull(value, nameof(value));

                assemblyFile = value;
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AssemblyModule"/> class.
        /// </summary>
        public AssemblyModule()
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="AssemblyModule"/>.
        /// </summary>
        /// <returns>A enumerator that can be used to iterate through the <see cref="AssemblyModule"/>.</returns>
        public override IEnumerator<ModuleContext> GetEnumerator()
        {
            if (assemblyFile != null)
            {
                string assemblyPath = assemblyFile.IsAbsoluteUri ? assemblyFile.LocalPath : new Uri(BaseUri, assemblyFile).LocalPath;

                AssemblyName assemblyName = AssemblyName.GetAssemblyName(assemblyPath);

                assemblyName.CodeBase = assemblyPath;

                if (AssemblyName != null)
                {
                    AssemblyHelper.VerifyAssemblyName(AssemblyName, assemblyName);
                }

                yield return new AssemblyModuleContext(assemblyName);
            }
        }

        #endregion

        #region Nested types

        /// <summary>
        /// Manages the lifetime of associated module object.
        /// </summary>
        private class AssemblyModuleContext : ModuleContext
        {
            #region Fields

            /// <summary>
            /// The identity of the assembly.
            /// </summary>
            private readonly AssemblyName assemblyName;

            /// <summary>
            /// The assembly load context.
            /// </summary>
            private AssemblyLoadContext context;

            #endregion

            #region Properties

            /// <summary>
            /// Gets the underlying assembly load context.
            /// </summary>
            /// <value>The assembly load context.</value>
            private AssemblyLoadContext Context
            {
                get
                {
                    if (context == null)
                    {
                        AssemblyLoadContext created = new ModuleAssemblyLoadContext(assemblyName.CodeBase);

                        Interlocked.CompareExchange(ref context, created, null);
                    }

                    return context;
                }
            }

            #endregion

            #region Constructors

            /// <summary>
            /// Initializes a new instance of the <see cref="AssemblyModuleContext"/> class.
            /// </summary>
            /// <param name="assemblyName">The identity of the assembly.</param>
            internal AssemblyModuleContext(AssemblyName assemblyName)
            {
                this.assemblyName = assemblyName;
            }

            #endregion

            #region Methods

            /// <summary>
            /// Creates the module instance to be associated with this <see cref="AssemblyModuleContext"/>.
            /// </summary>
            /// <returns>The created module object instance.</returns>
            protected override Module CreateModule()
            {
                Assembly assembly = Context.LoadFromAssemblyName(assemblyName);

                AssemblyModuleAttribute attribute = assembly.GetCustomAttribute<AssemblyModuleAttribute>();

                if (attribute == null)
                {
                    throw Error.AttributeNotDefined(typeof(AssemblyModuleAttribute), assembly);
                }

                return ModuleFactory.CreateModule(attribute.ModuleType);
            }

            /// <summary>
            /// Releases resources used by the <see cref="AssemblyModuleContext"/>.
            /// </summary>
            /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
            protected override void Dispose(bool disposing)
            {
                base.Dispose(disposing);

                if (disposing && context != null)
                {
                    context.Unload();

                    context = null;
                }
            }

            #endregion

            #region Nested types

            /// <summary>
            /// Represents the scope for module assembly loading.
            /// </summary>
            private class ModuleAssemblyLoadContext : AssemblyLoadContext
            {
                #region Fields

                /// <summary>
                /// The underlying assembly dependency resolver.
                /// </summary>
                private readonly AssemblyDependencyResolver resolver;

                #endregion

                #region Constructors

                /// <summary>
                /// Initializes a new instance of the <see cref="ModuleAssemblyLoadContext"/> class.
                /// </summary>
                /// <param name="assemblyPath">The location of the assembly file.</param>
                internal ModuleAssemblyLoadContext(string assemblyPath) : base(true)
                {
                    resolver = new AssemblyDependencyResolver(assemblyPath);

                    ResolvingUnmanagedDll += OnResolvingUnmanaged;

                    Resolving += OnResolving;
                }

                #endregion

                #region Methods

                /// <summary>
                /// Called when attempting to resolve and load a native library.
                /// </summary>
                /// <param name="assembly">The requesting assembly.</param>
                /// <param name="unmanagedName">The name of the requested native library.</param>
                /// <returns>A handle to the loaded library, or <see cref="IntPtr.Zero"/> if the library cannot be resolved.</returns>
                private IntPtr OnResolvingUnmanaged(Assembly assembly, string unmanagedName)
                {
                    string unmanagedPath = resolver.ResolveUnmanagedDllToPath(unmanagedName);

                    return unmanagedPath != null ? LoadUnmanagedDllFromPath(unmanagedPath) : IntPtr.Zero;
                }

                /// <summary>
                /// Called when attempting to resolve and load an assembly into the specified assembly load context.
                /// </summary>
                /// <param name="context">The requesting assembly load context.</param>
                /// <param name="assemblyName">The identity of the requested assembly.</param>
                /// <returns>The resolved assembly, or <c>null</c> if the assembly cannot be resolved.</returns>
                private Assembly OnResolving(AssemblyLoadContext context, AssemblyName assemblyName)
                {
                    string assemblyPath = resolver.ResolveAssemblyToPath(assemblyName);

                    return assemblyPath != null && AssemblyHelper.MatchAssemblyName(assemblyName, AssemblyName.GetAssemblyName(assemblyPath)) ? LoadFromAssemblyPath(assemblyPath) : null;
                }

                #endregion
            }

            #endregion
        }

        #endregion
    }
}
