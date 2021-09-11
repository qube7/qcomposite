using System;
using System.Collections.Generic;
using System.Reflection;

namespace Qube7.Composite
{
    /// <summary>
    /// Provides helper and extension methods for filtering sequences of <see cref="Type"/> objects.
    /// </summary>
    public static class TypeMatch
    {
        #region Methods

        /// <summary>
        /// Gets the types defined in the assembly and filters them based on the specified predicate.
        /// </summary>
        /// <param name="assembly">The assembly to get the defined types from.</param>
        /// <param name="predicate">The predicate to test each type for a match.</param>
        /// <returns>The sequence that contains types defined in the assembly that match the <paramref name="predicate"/>.</returns>
        public static IEnumerable<Type> GetTypes(this Assembly assembly, Predicate predicate)
        {
            Requires.NotNull(assembly, nameof(assembly));

            return Where(assembly.GetTypes(), predicate);
        }

        /// <summary>
        /// Filters a sequence of type declarations by returning types that match the specified predicate.
        /// </summary>
        /// <param name="source">The sequence of type declarations to filter.</param>
        /// <param name="predicate">The predicate to test each type for a match.</param>
        /// <returns>The sequence that contains types from the input sequence that match the <paramref name="predicate"/>.</returns>
        public static IEnumerable<Type> Where(this IEnumerable<Type> source, Predicate predicate)
        {
            Requires.NotNull(source, nameof(source));
            Requires.NotNull(predicate, nameof(predicate));

            return WherePrivate(source, predicate);
        }

        /// <summary>
        /// Filters a sequence of type declarations by returning types that match the specified predicate.
        /// </summary>
        /// <param name="source">The sequence of type declarations to filter.</param>
        /// <param name="predicate">The predicate to test each type for a match.</param>
        /// <returns>The sequence that contains types from the input sequence that match the <paramref name="predicate"/>.</returns>
        private static IEnumerable<Type> WherePrivate(this IEnumerable<Type> source, Predicate predicate)
        {
            foreach (Type type in source)
            {
                if (predicate.Match(type))
                {
                    yield return type;
                }
            }
        }

        /// <summary>
        /// Creates a new <see cref="Predicate"/> with a rule to match types included in the specified namespace.
        /// </summary>
        /// <param name="namespaceName">The namespace of types to match.</param>
        /// <param name="includeNested"><c>true</c> to also match types from nested namespaces; otherwise, <c>false</c>.</param>
        /// <returns>A reference to the instance of <see cref="Predicate"/>.</returns>
        public static Predicate FromNamespace(string namespaceName, bool includeNested = false)
        {
            Predicate predicate = new Predicate();

            return predicate.FromNamespace(namespaceName, includeNested);
        }

        /// <summary>
        /// Creates a new <see cref="Predicate"/> with a rule to match types whose instances can be assigned to a variable of the specified type.
        /// </summary>
        /// <param name="type">The target type of the assignment for types to match.</param>
        /// <returns>A reference to the instance of <see cref="Predicate"/>.</returns>
        public static Predicate AssignableTo(Type type)
        {
            Predicate predicate = new Predicate();

            return predicate.AssignableTo(type);
        }

        /// <summary>
        /// Creates a new <see cref="Predicate"/> with a rule to match types whose instances can be assigned to a variable of the specified type.
        /// </summary>
        /// <typeparam name="T">The target type of the assignment for types to match.</typeparam>
        /// <returns>A reference to the instance of <see cref="Predicate"/>.</returns>
        public static Predicate AssignableTo<T>()
        {
            return AssignableTo(typeof(T));
        }

        #endregion

        #region Nested types

        /// <summary>
        /// Defines rules for filtering type declarations and determines whether the specified type matches those rules.
        /// </summary>
        public class Predicate
        {
            #region Fields

            /// <summary>
            /// The exact matching namespaces.
            /// </summary>
            private readonly List<string> exact = new List<string>();

            /// <summary>
            /// The parent matching namespaces.
            /// </summary>
            private readonly List<string> parent = new List<string>();

            /// <summary>
            /// The matching assignable to types.
            /// </summary>
            private readonly List<Type> types = new List<Type>();

            #endregion

            #region Constructors

            /// <summary>
            /// Initializes a new instance of the <see cref="Predicate"/> class.
            /// </summary>
            internal Predicate()
            {
            }

            #endregion

            #region Methods

            /// <summary>
            /// Adds a rule to the <see cref="Predicate"/> to match types included in the specified namespace.
            /// </summary>
            /// <param name="namespaceName">The namespace of types to match.</param>
            /// <param name="includeNested"><c>true</c> to also match types from nested namespaces; otherwise, <c>false</c>.</param>
            /// <returns>A reference to the current instance of <see cref="Predicate"/>.</returns>
            public Predicate FromNamespace(string namespaceName, bool includeNested = false)
            {
                exact.Add(namespaceName);

                if (includeNested && namespaceName != null)
                {
                    parent.Add(namespaceName);
                }

                return this;
            }

            /// <summary>
            /// Adds a rule to the <see cref="Predicate"/> to match types whose instances can be assigned to a variable of the specified type.
            /// </summary>
            /// <param name="type">The target type of the assignment for types to match.</param>
            /// <returns>A reference to the current instance of <see cref="Predicate"/>.</returns>
            public Predicate AssignableTo(Type type)
            {
                Requires.NotNull(type, nameof(type));

                types.Add(type);

                return this;
            }

            /// <summary>
            /// Adds a rule to the <see cref="Predicate"/> to match types whose instances can be assigned to a variable of the specified type.
            /// </summary>
            /// <typeparam name="T">The target type of the assignment for types to match.</typeparam>
            /// <returns>A reference to the current instance of <see cref="Predicate"/>.</returns>
            public Predicate AssignableTo<T>()
            {
                return AssignableTo(typeof(T));
            }

            /// <summary>
            /// Determines whether the specified type matches rules defined in the <see cref="Predicate"/>.
            /// </summary>
            /// <param name="type">The type to test.</param>
            /// <returns><c>true</c> if the <paramref name="type"/> matches the rules; otherwise, <c>false</c>.</returns>
            public bool Match(Type type)
            {
                if (type != null && Test1(type, out bool match1) && Test2(type, out bool match2))
                {
                    return match1 || match2;
                }

                return false;
            }

            /// <summary>
            /// Determines whether the specified type matches the namespace rules defined in the <see cref="Predicate"/>.
            /// </summary>
            /// <param name="type">The type to test.</param>
            /// <param name="match"><c>true</c> if the <paramref name="type"/> matches any of the namespace rules; otherwise, <c>false</c>.</param>
            /// <returns><c>true</c> if processing other rules should continue; otherwise, <c>false</c>.</returns>
            private bool Test1(Type type, out bool match)
            {
                match = false;

                if (exact.Count > 0)
                {
                    string namespaceName = type.Namespace;

                    for (int i = 0; i < exact.Count; i++)
                    {
                        if (namespaceName == exact[i])
                        {
                            match = true;

                            return true;
                        }
                    }

                    if (namespaceName != null)
                    {
                        for (int i = 0; i < parent.Count; i++)
                        {
                            if (namespaceName.Length > parent[i].Length && namespaceName.StartsWith(parent[i]) && namespaceName[parent[i].Length] == Type.Delimiter)
                            {
                                match = true;

                                return true;
                            }
                        }
                    }

                    return false;
                }

                return true;
            }

            /// <summary>
            /// Determines whether the specified type matches the assignment rules defined in the <see cref="Predicate"/>.
            /// </summary>
            /// <param name="type">The type to test.</param>
            /// <param name="match"><c>true</c> if the <paramref name="type"/> matches any of the assignment rules; otherwise, <c>false</c>.</param>
            /// <returns><c>true</c> if processing other rules should continue; otherwise, <c>false</c>.</returns>
            private bool Test2(Type type, out bool match)
            {
                match = false;

                if (types.Count > 0)
                {
                    for (int i = 0; i < types.Count; i++)
                    {
                        if (types[i].IsAssignableFrom(type))
                        {
                            match = true;

                            return true;
                        }
                    }

                    return false;
                }

                return true;
            }

            #endregion

            #region Operators

            /// <summary>
            /// Defines an implicit conversion of a <see cref="Predicate"/> to a <see cref="Predicate{T}"/>.
            /// </summary>
            /// <param name="predicate">The <see cref="Predicate"/> to convert.</param>
            /// <returns>An <see cref="Predicate{T}"/> representation of the <paramref name="predicate"/>.</returns>
            public static implicit operator Predicate<Type>(Predicate predicate)
            {
                if (predicate == null)
                {
                    return null;
                }

                return new Predicate<Type>(predicate.Match);
            }

            /// <summary>
            /// Defines an implicit conversion of a <see cref="Predicate"/> to a <see cref="Func{T, TResult}"/>.
            /// </summary>
            /// <param name="predicate">The <see cref="Predicate"/> to convert.</param>
            /// <returns>An <see cref="Func{T, TResult}"/> representation of the <paramref name="predicate"/>.</returns>
            public static implicit operator Func<Type, bool>(Predicate predicate)
            {
                if (predicate == null)
                {
                    return null;
                }

                return new Func<Type, bool>(predicate.Match);
            }

            #endregion
        }

        #endregion
    }
}
