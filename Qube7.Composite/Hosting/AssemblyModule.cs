using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.Loader;

namespace Qube7.Composite.Hosting
{
    /// <summary>
    /// Discovers a module provided by the assembly resolved from the specified location.
    /// </summary>
    public class AssemblyModule : ModuleDiscovery
    {
        #region Fields

        /// <summary>
        /// The default absolute <see cref="Uri"/> that is the base for the assembly location.
        /// </summary>
        private static readonly Uri baseUri = new Uri(AppDomain.CurrentDomain.BaseDirectory);

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
                string assemblyPath = assemblyFile.IsAbsoluteUri ? assemblyFile.LocalPath : new Uri(baseUri, assemblyFile).LocalPath;

                if (AssemblyName != null)
                {
                    AssemblyHelper.VerifyAssemblyName(assemblyPath, AssemblyName);
                }

                yield return new AssemblyModuleContext(assemblyPath);
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
            /// The location of the assembly file.
            /// </summary>
            private readonly string assemblyPath;

            /// <summary>
            /// The underlying assembly load context.
            /// </summary>
            private ModuleAssemblyLoadContext context;

            #endregion

            #region Constructors

            /// <summary>
            /// Initializes a new instance of the <see cref="AssemblyModuleContext"/> class.
            /// </summary>
            /// <param name="assemblyPath">The location of the assembly file.</param>
            internal AssemblyModuleContext(string assemblyPath)
            {
                this.assemblyPath = assemblyPath;
            }

            #endregion

            #region Methods

            /// <summary>
            /// Creates the module instance to be associated with this <see cref="AssemblyModuleContext"/>.
            /// </summary>
            /// <returns>The created module object instance.</returns>
            protected override Module CreateModule()
            {
                context = new ModuleAssemblyLoadContext(assemblyPath);

                Assembly assembly = context.LoadFromAssemblyPath(assemblyPath);

                AssemblyModuleAttribute attribute = assembly.GetCustomAttribute<AssemblyModuleAttribute>();

                if (attribute == null)
                {
                    throw Error.AttributeNotDefined(typeof(AssemblyModuleAttribute), assembly);
                }

                return CreateModule(attribute.ModuleType);
            }

            /// <summary>
            /// Creates the module instance of the specified type.
            /// </summary>
            /// <param name="moduleType">The type of the module to create.</param>
            /// <returns>The created module object instance.</returns>
            private static Module CreateModule(Type moduleType)
            {
                ModuleHelper.VerifyModule(moduleType);

                return Activator.CreateInstance(moduleType) as Module;
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

                    return assemblyPath != null && AssemblyHelper.MatchAssemblyName(assemblyPath, assemblyName) ? LoadFromAssemblyPath(assemblyPath) : null;
                }

                #endregion
            }

            #endregion
        }

        #endregion
    }
}
