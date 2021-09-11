using System;
using System.Collections.Generic;

namespace Qube7.Composite.Hosting
{
    /// <summary>
    /// Represents an module whose class type is resolvable to the current composite host application.
    /// </summary>
    /// <remarks>The module type must be concrete type, assignable to the <see cref="Module"/> type, that has public parameterless constructor.</remarks>
    public class LocalModule : ModuleIterator
    {
        #region Fields

        /// <summary>
        /// The type of the module.
        /// </summary>
        private Type type;

        /// <summary>
        /// The name of the type of the module.
        /// </summary>
        private string typeName;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the type of the module represented by this <see cref="LocalModule"/>.
        /// </summary>
        /// <value>The type of the module.</value>
        public Type Type
        {
            get { return type; }
            set
            {
                Requires.NotNull(value, nameof(value));

                type = value;

                typeName = null;
            }
        }

        /// <summary>
        /// Gets or sets the type name of the module represented by this <see cref="LocalModule"/>.
        /// </summary>
        /// <value>The assembly-qualified name of the type of the module.</value>
        public string TypeName
        {
            get { return typeName; }
            set
            {
                Requires.NotNullOrEmpty(value, nameof(value));

                typeName = value;

                type = null;
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalModule"/> class.
        /// </summary>
        public LocalModule()
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="LocalModule"/>.
        /// </summary>
        /// <returns>A enumerator that can be used to iterate through the <see cref="LocalModule"/>.</returns>
        public override IEnumerator<ModuleContext> GetEnumerator()
        {
            Type value = type;

            if (value == null && typeName != null)
            {
                value = Type.GetType(typeName, true);
            }

            if (value != null)
            {
                yield return new LocalModuleContext(value);
            }
        }

        #endregion

        #region Nested types

        /// <summary>
        /// Manages the lifetime of associated module object.
        /// </summary>
        private class LocalModuleContext : ModuleContext
        {
            #region Fields

            /// <summary>
            /// The type of the module.
            /// </summary>
            private readonly Type type;

            #endregion

            #region Constructors

            /// <summary>
            /// Initializes a new instance of the <see cref="LocalModuleContext"/> class.
            /// </summary>
            /// <param name="type">The type of the module.</param>
            internal LocalModuleContext(Type type)
            {
                this.type = type;
            }

            #endregion

            #region Methods

            /// <summary>
            /// Creates the module instance to be associated with this <see cref="LocalModuleContext"/>.
            /// </summary>
            /// <returns>The created module object instance.</returns>
            protected override Module CreateModule()
            {
                return ModuleFactory.CreateModule(type);
            }

            #endregion
        }

        #endregion
    }
}
