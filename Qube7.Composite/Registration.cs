using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Linq;

namespace Qube7.Composite
{
    /// <summary>
    /// Provides helpers for component registration.
    /// </summary>
    public static class Registration
    {
        #region Methods

        /// <summary>
        /// Creates composable parts from an array of attributed objects and composes them in the specified composition container.
        /// </summary>
        /// <param name="container">The composition container to perform composition in.</param>
        /// <param name="attributedParts">An array of attributed objects to compose.</param>
        public static void ComposeParts(this ICompositionContainer container, params object[] attributedParts)
        {
            Requires.NotNull(container, nameof(container));
            Requires.NotNull(attributedParts, nameof(attributedParts));

            ComposablePart[] parts = new ComposablePart[attributedParts.Length];

            for (int i = 0; i < parts.Length; i++)
            {
                parts[i] = AttributedModelServices.CreatePart(attributedParts[i]);
            }

            CompositionBatch batch = new CompositionBatch(parts, Enumerable.Empty<ComposablePart>());

            container.Compose(batch);
        }

        /// <summary>
        /// Filters a sequence of type declarations by returning elements from the specified namespace.
        /// </summary>
        /// <param name="source">The sequence of type declarations to be filtered.</param>
        /// <param name="namespaceName">The name of the namespace of type declarations to be included in the returned sequence.</param>
        /// <returns>The sequence that contains type declarations from the input sequence which namespace match the <paramref name="namespaceName"/>.</returns>
        public static NamespaceConvention FromNamespace(this IEnumerable<Type> source, string namespaceName)
        {
            Requires.NotNull(source, nameof(source));

            NamespaceConvention convention = source as NamespaceConvention ?? new NamespaceConvention(source);

            return convention.FromNamespace(namespaceName);
        }

        /// <summary>
        /// Filters a sequence of type declarations by returning elements that are assignable to the specified type.
        /// </summary>
        /// <param name="source">The sequence of type declarations to be filtered.</param>
        /// <param name="type">The type assignable from the type declarations to be included in the returned sequence.</param>
        /// <returns>The sequence that contains type declarations from the input sequence that are assignable to the <paramref name="type"/>.</returns>
        public static AssignableConvention AssignableTo(this IEnumerable<Type> source, Type type)
        {
            Requires.NotNull(source, nameof(source));

            AssignableConvention convention = source as AssignableConvention ?? new AssignableConvention(source);

            return convention.AssignableTo(type);
        }

        /// <summary>
        /// Filters a sequence of type declarations by returning elements that are assignable to the specified type.
        /// </summary>
        /// <typeparam name="T">The type assignable from the type declarations to be included in the returned sequence.</typeparam>
        /// <param name="source">The sequence of type declarations to be filtered.</param>
        /// <returns>The sequence that contains type declarations from the input sequence that are assignable to the <typeparamref name="T"/>.</returns>
        public static AssignableConvention AssignableTo<T>(this IEnumerable<Type> source)
        {
            return AssignableTo(source, typeof(T));
        }

        #endregion

        #region Nested types

        /// <summary>
        /// Filters a sequence of type declarations by returning elements from the given namespaces.
        /// </summary>
        public class NamespaceConvention : IEnumerable<Type>
        {
            #region Fields

            /// <summary>
            /// The input sequence.
            /// </summary>
            private readonly IEnumerable<Type> source;

            /// <summary>
            /// The matching namespaces.
            /// </summary>
            private readonly List<string> namespaces = new List<string>(2);

            #endregion

            #region Properties

            /// <summary>
            /// Gets the matching namespaces of this <see cref="NamespaceConvention"/>.
            /// </summary>
            /// <value>The matching namespaces.</value>
            public ICollection<string> Namespaces
            {
                get { return namespaces; }
            }

            #endregion

            #region Constructors

            /// <summary>
            /// Initializes a new instance of the <see cref="NamespaceConvention"/> class.
            /// </summary>
            /// <param name="source">The sequence of type declarations to be filtered.</param>
            internal NamespaceConvention(IEnumerable<Type> source)
            {
                this.source = source;
            }

            #endregion

            #region Methods

            /// <summary>
            /// Filters a sequence of type declarations by returning elements from the specified namespace.
            /// </summary>
            /// <param name="namespaceName">The name of the namespace of type declarations to be included in the returned sequence.</param>
            /// <returns>The sequence that contains type declarations from the input sequence which namespace match the <paramref name="namespaceName"/>.</returns>
            public NamespaceConvention FromNamespace(string namespaceName)
            {
                namespaces.Add(namespaceName);

                return this;
            }

            /// <summary>
            /// Returns an enumerator that iterates through the <see cref="NamespaceConvention"/>.
            /// </summary>
            /// <returns>A enumerator that can be used to iterate through the <see cref="NamespaceConvention"/>.</returns>
            public IEnumerator<Type> GetEnumerator()
            {
                string[] array = namespaces.OfType<string>().ToArray();

                if (array.Length > 0)
                {
                    foreach (Type type in source)
                    {
                        if (type != null)
                        {
                            for (int i = 0; i < array.Length; i++)
                            {
                                if (type.Namespace == array[i])
                                {
                                    yield return type;

                                    break;
                                }
                            }
                        }
                    }
                }
            }

            /// <summary>
            /// Returns an enumerator that iterates through the <see cref="NamespaceConvention"/>.
            /// </summary>
            /// <returns>A enumerator that can be used to iterate through the <see cref="NamespaceConvention"/>.</returns>
            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            #endregion
        }

        /// <summary>
        /// Filters a sequence of type declarations by returning elements that are assignable to the any of the given types.
        /// </summary>
        public class AssignableConvention : IEnumerable<Type>
        {
            #region Fields

            /// <summary>
            /// The input sequence.
            /// </summary>
            private readonly IEnumerable<Type> source;

            /// <summary>
            /// The matching assignable to types.
            /// </summary>
            private readonly List<Type> types = new List<Type>(2);

            #endregion

            #region Properties

            /// <summary>
            /// Gets the matching assignable to types of this <see cref="AssignableConvention"/>.
            /// </summary>
            /// <value>The matching assignable to types.</value>
            public ICollection<Type> Types
            {
                get { return types; }
            }

            #endregion

            #region Constructors

            /// <summary>
            /// Initializes a new instance of the <see cref="AssignableConvention"/> class.
            /// </summary>
            /// <param name="source">The sequence of type declarations to be filtered.</param>
            internal AssignableConvention(IEnumerable<Type> source)
            {
                this.source = source;
            }

            #endregion

            #region Methods

            /// <summary>
            /// Filters a sequence of type declarations by returning elements that are assignable to the specified type.
            /// </summary>
            /// <param name="type">The type assignable from the type declarations to be included in the returned sequence.</param>
            /// <returns>The sequence that contains type declarations from the input sequence that are assignable to the <paramref name="type"/>.</returns>
            public AssignableConvention AssignableTo(Type type)
            {
                types.Add(type);

                return this;
            }

            /// <summary>
            /// Returns an enumerator that iterates through the <see cref="AssignableConvention"/>.
            /// </summary>
            /// <returns>A enumerator that can be used to iterate through the <see cref="AssignableConvention"/>.</returns>
            public IEnumerator<Type> GetEnumerator()
            {
                Type[] array = types.OfType<Type>().ToArray();

                if (array.Length > 0)
                {
                    foreach (Type type in source)
                    {
                        if (type != null)
                        {
                            for (int i = 0; i < array.Length; i++)
                            {
                                if (array[i].IsAssignableFrom(type))
                                {
                                    yield return type;

                                    break;
                                }
                            }
                        }
                    }
                }
            }

            /// <summary>
            /// Returns an enumerator that iterates through the <see cref="AssignableConvention"/>.
            /// </summary>
            /// <returns>A enumerator that can be used to iterate through the <see cref="AssignableConvention"/>.</returns>
            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            #endregion
        }

        #endregion
    }
}
