using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Qube7.Composite
{
    /// <summary>
    /// Represents an object that supports property change notification.
    /// </summary>
    public abstract class ObservableObject : INotifyPropertyChanged
    {
        #region Fields

        /// <summary>
        /// Identifies the default name of the compiler-generated indexer property.
        /// </summary>
        protected const string IndexerName = "Item";

        /// <summary>
        /// The trace category.
        /// </summary>
        private const string TraceCategory = nameof(ObservableObject);

        /// <summary>
        /// The property change notification.
        /// </summary>
        private readonly IRaiseChanged raiseChanged;

        /// <summary>
        /// The properties backing store dictionary.
        /// </summary>
        private readonly Dictionary<string, object> properties;

        #endregion

        #region Events

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableObject"/> class.
        /// </summary>
        protected ObservableObject()
        {
            raiseChanged = RaiseChanged.FromType(GetType());

            properties = new Dictionary<string, object>();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Notifies that object's property value has changed.
        /// </summary>
        /// <param name="propertyName">The name of the property that changed.</param>
        /// <remarks>The <c>null</c> or <see cref="String.Empty"/> used for the <paramref name="propertyName"/> indicates that all properties on the object have changed.</remarks>
        protected void NotifyChanged([CallerMemberName] string propertyName = null)
        {
            raiseChanged.Raise(this, propertyName);
        }

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="PropertyChangedEventArgs"/> instance containing the event data.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            Event.Raise(PropertyChanged, this, e);
        }

        /// <summary>
        /// Sets the value of the backing field of the property and notifies if the property value has changed.
        /// </summary>
        /// <typeparam name="T">The type of the property value.</typeparam>
        /// <param name="field">The backing field of the property to set.</param>
        /// <param name="value">The new value of the property to set.</param>
        /// <param name="propertyName">The name of the property to set.</param>
        /// <returns><c>true</c> if the property value has changed; otherwise, <c>false</c>.</returns>
        protected bool Set<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
            {
                return false;
            }

            field = value;

            NotifyChanged(propertyName);

            return true;
        }

        /// <summary>
        /// Gets the value of the property from the dictionary-based backing store.
        /// </summary>
        /// <typeparam name="T">The type of the property value.</typeparam>
        /// <param name="propertyName">The name of the property to get the value for.</param>
        /// <returns>The current value of the property.</returns>
        protected T Get<T>([CallerMemberName] string propertyName = null)
        {
            Requires.NotNullOrEmpty(propertyName, nameof(propertyName));

            lock (properties)
            {
                if (properties.TryGetValue(propertyName, out object value))
                {
                    return (T)value;
                }
            }

            return default;
        }

        /// <summary>
        /// Sets the value of the property in the dictionary-based backing store and notifies if the property value has changed.
        /// </summary>
        /// <typeparam name="T">The type of the property value.</typeparam>
        /// <param name="value">The new value of the property to set.</param>
        /// <param name="propertyName">The name of the property to set.</param>
        /// <returns><c>true</c> if the property value has changed; otherwise, <c>false</c>.</returns>
        protected bool Set<T>(T value, [CallerMemberName] string propertyName = null)
        {
            Requires.NotNullOrEmpty(propertyName, nameof(propertyName));

            lock (properties)
            {
                T field = default;

                if (properties.TryGetValue(propertyName, out object current))
                {
                    field = (T)current;
                }

                if (EqualityComparer<T>.Default.Equals(field, value))
                {
                    return false;
                }

                properties[propertyName] = value;
            }

            NotifyChanged(propertyName);

            return true;
        }

        /// <summary>
        /// Determines whether the current object declares an instance property with the specified name.
        /// </summary>
        /// <param name="propertyName">The name of the property being tested.</param>
        [Conditional("DEBUG")]
        private void ValidateProperty(string propertyName)
        {
            Type type = GetType();

            if (type.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance) == null)
            {
                Trace.WriteLine(string.Format(Strings.PropertyNotFound, propertyName, type), TraceCategory);
            }
        }

        #endregion

        #region Nested types

        /// <summary>
        /// Indicates that the attributed property is dependent on the value of another property.
        /// </summary>
        [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
        protected class DependsOnAttribute : Attribute
        {
            #region Fields

            /// <summary>
            /// The names of the related properties.
            /// </summary>
            private readonly string[] properties;

            #endregion

            #region Properties

            /// <summary>
            /// Gets the names of the related properties declared in this <see cref="DependsOnAttribute"/>.
            /// </summary>
            /// <value>The names of the related properties.</value>
            public virtual IEnumerable<string> Properties
            {
                get { return properties; }
            }

            #endregion

            #region Constructors

            /// <summary>
            /// Initializes a new instance of the <see cref="DependsOnAttribute"/> class.
            /// </summary>
            protected DependsOnAttribute()
            {
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="DependsOnAttribute"/> class.
            /// </summary>
            /// <param name="properties">The names of the properties that the property associated with this <see cref="DependsOnAttribute"/> depends on.</param>
            public DependsOnAttribute(params string[] properties)
            {
                this.properties = properties;
            }

            #endregion
        }

        /// <summary>
        /// Raises the property change notification for the <see cref="ObservableObject"/>.
        /// </summary>
        private interface IRaiseChanged
        {
            #region Methods

            /// <summary>
            /// Raises the property change notification for the specified <see cref="ObservableObject"/>.
            /// </summary>
            /// <param name="observable">The target observable.</param>
            /// <param name="propertyName">The name of the property that changed.</param>
            void Raise(ObservableObject observable, string propertyName);

            #endregion
        }

        /// <summary>
        /// Manages the cached <see cref="IRaiseChanged"/> objects.
        /// </summary>
        private static class RaiseChanged
        {
            #region Fields

            /// <summary>
            /// The change notification table.
            /// </summary>
            private static readonly ConditionalWeakTable<Type, IRaiseChanged> table = new ConditionalWeakTable<Type, IRaiseChanged>();

            #endregion

            #region Methods

            /// <summary>
            /// Returns an <see cref="IRaiseChanged"/> created for the specified target observable type.
            /// </summary>
            /// <param name="target">The type of the target observable.</param>
            /// <returns>The <see cref="IRaiseChanged"/> for the <paramref name="target"/>.</returns>
            internal static IRaiseChanged FromType(Type target)
            {
                return table.GetValue(target, Create);
            }

            /// <summary>
            /// Returns an <see cref="IRaiseChanged"/> created for the specified target observable type.
            /// </summary>
            /// <param name="target">The type of the target observable.</param>
            /// <returns>The <see cref="IRaiseChanged"/> for the <paramref name="target"/>.</returns>
            private static IRaiseChanged Create(Type target)
            {
                string indexerName = null;

                Dictionary<string, Entry> dictionary = new Dictionary<string, Entry>();

                PropertyInfo[] array = target.GetProperties(BindingFlags.Public | BindingFlags.Instance);

                for (int i = 0; i < array.Length; i++)
                {
                    Entry current = null;

                    PropertyInfo property = array[i];

                    string propertyName = property.Name;

                    object[] attributes = property.GetCustomAttributes(false);

                    for (int j = 0; j < attributes.Length; j++)
                    {
                        if (attributes[j] is DependsOnAttribute attribute)
                        {
                            IEnumerable<string> properties = attribute.Properties;

                            if (properties != null)
                            {
                                foreach (string otherName in properties)
                                {
                                    if (string.IsNullOrEmpty(otherName) || otherName == propertyName)
                                    {
                                        continue;
                                    }

                                    if (current == null)
                                    {
                                        current = Entry.Get(propertyName, dictionary);
                                    }

                                    Entry entry = Entry.Get(otherName, dictionary);

                                    if (entry.Contains(current))
                                    {
                                        continue;
                                    }

                                    entry.Add(current);
                                }
                            }
                        }
                    }

                    if (indexerName == null && property.GetIndexParameters().Length > 0)
                    {
                        indexerName = propertyName;
                    }
                }

                if (dictionary.Count > 0)
                {
                    MappedNotifyChanged mapped = CreateMapped(indexerName);

                    foreach (Entry entry in dictionary.Values)
                    {
                        string[] properties = entry.Resolve();

                        if (properties != null)
                        {
                            mapped.Add(entry.Name, properties);
                        }
                    }

                    return mapped;
                }

                if (indexerName == null)
                {
                    return NotifyChanged.Instance;
                }

                if (indexerName == IndexerName)
                {
                    return IndexerNotifyChanged.Default;
                }

                return new IndexerNotifyChanged(indexerName);
            }

            /// <summary>
            /// Creates the <see cref="MappedNotifyChanged"/> for the type that declares the specified indexer property.
            /// </summary>
            /// <param name="indexerName">The name of the indexer property.</param>
            /// <returns>The <see cref="MappedNotifyChanged"/> for the <paramref name="indexerName"/>.</returns>
            private static MappedNotifyChanged CreateMapped(string indexerName)
            {
                if (indexerName == null)
                {
                    return new MappedNotifyChanged();
                }

                return new IndexerMappedNotifyChanged(indexerName);
            }

            #endregion

            #region Nested types

            /// <summary>
            /// Represents the property element in the dependency map.
            /// </summary>
            private class Entry : List<Entry>
            {
                #region Fields

                /// <summary>
                /// The name of the associated property.
                /// </summary>
                private readonly string name;

                #endregion

                #region Properties

                /// <summary>
                /// Gets the name of the associated property.
                /// </summary>
                /// <value>The name of the property.</value>
                internal string Name
                {
                    get { return name; }
                }

                #endregion

                #region Constructors

                /// <summary>
                /// Initializes a new instance of the <see cref="Entry"/> class.
                /// </summary>
                /// <param name="name">The name of the associated property.</param>
                private Entry(string name)
                {
                    this.name = name;
                }

                #endregion

                #region Methods

                /// <summary>
                /// Returns an <see cref="Entry"/> associated with the specified property.
                /// </summary>
                /// <param name="name">The property name of the <see cref="Entry"/> to get.</param>
                /// <param name="dictionary">The dictionary from which to get the <see cref="Entry"/>.</param>
                /// <returns>An <see cref="Entry"/> associated with the specified property.</returns>
                internal static Entry Get(string name, Dictionary<string, Entry> dictionary)
                {
                    if (!dictionary.TryGetValue(name, out Entry entry))
                    {
                        entry = new Entry(name);

                        dictionary.Add(name, entry);
                    }

                    return entry;
                }

                /// <summary>
                /// Resolves the collection of dependent properties for the specified <see cref="Entry"/>.
                /// </summary>
                /// <param name="entry">The <see cref="Entry"/> to resolve.</param>
                /// <param name="list">The collection of dependent properties.</param>
                private static void Resolve(Entry entry, List<string> list)
                {
                    if (list.Contains(entry.Name))
                    {
                        return;
                    }

                    list.Add(entry.Name);

                    for (int i = 0; i < entry.Count; i++)
                    {
                        Resolve(entry[i], list);
                    }
                }

                /// <summary>
                /// Resolves the collection of dependent properties of the associated property.
                /// </summary>
                /// <returns>The collection of dependent properties.</returns>
                internal string[] Resolve()
                {
                    List<string> list = new List<string>();

                    Resolve(this, list);

                    if (list.Count > 1)
                    {
                        int count = list.Count - 1;

                        string[] array = new string[count];

                        list.CopyTo(1, array, 0, count);

                        return array;
                    }

                    return null;
                }

                #endregion
            }

            /// <summary>
            /// Manages the property change notification of the <see cref="ObservableObject"/>.
            /// </summary>
            private class NotifyChanged : IRaiseChanged
            {
                #region Fields

                /// <summary>
                /// Represents the singleton instance of the <see cref="NotifyChanged"/> class.
                /// </summary>
                internal static readonly NotifyChanged Instance = new NotifyChanged();

                #endregion

                #region Constructors

                /// <summary>
                /// Prevents a default instance of the <see cref="NotifyChanged"/> class from being created.
                /// </summary>
                private NotifyChanged()
                {
                }

                #endregion

                #region Methods

                /// <summary>
                /// Raises the property change notification for the specified <see cref="ObservableObject"/>.
                /// </summary>
                /// <param name="observable">The target observable.</param>
                /// <param name="propertyName">The name of the property that changed.</param>
                public void Raise(ObservableObject observable, string propertyName)
                {
                    if (string.IsNullOrEmpty(propertyName))
                    {
                        observable.OnPropertyChanged(EventArgsCache.ObjectPropertyChanged);

                        return;
                    }

                    observable.ValidateProperty(propertyName);

                    observable.OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
                }

                #endregion
            }

            /// <summary>
            /// Manages the property change notification of the <see cref="ObservableObject"/> that declares an indexer.
            /// </summary>
            private class IndexerNotifyChanged : IRaiseChanged
            {
                #region Fields

                /// <summary>
                /// Represents the instance of the <see cref="IndexerNotifyChanged"/> class for the default name of the indexer property.
                /// </summary>
                internal static readonly IndexerNotifyChanged Default = new IndexerNotifyChanged(IndexerName);

                /// <summary>
                /// The name of the indexer property.
                /// </summary>
                private readonly string indexerName;

                #endregion

                #region Constructors

                /// <summary>
                /// Initializes a new instance of the <see cref="IndexerNotifyChanged"/> class.
                /// </summary>
                /// <param name="indexerName">The name of the indexer property.</param>
                internal IndexerNotifyChanged(string indexerName)
                {
                    this.indexerName = indexerName;
                }

                #endregion

                #region Methods

                /// <summary>
                /// Raises the property change notification for the specified <see cref="ObservableObject"/>.
                /// </summary>
                /// <param name="observable">The target observable.</param>
                /// <param name="propertyName">The name of the property that changed.</param>
                public void Raise(ObservableObject observable, string propertyName)
                {
                    if (string.IsNullOrEmpty(propertyName))
                    {
                        observable.OnPropertyChanged(EventArgsCache.ObjectPropertyChanged);

                        return;
                    }

                    if (propertyName == indexerName)
                    {
                        observable.OnPropertyChanged(EventArgsCache.IndexerPropertyChanged);

                        return;
                    }

                    observable.ValidateProperty(propertyName);

                    observable.OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
                }

                #endregion
            }

            /// <summary>
            /// Manages the property change notification of the <see cref="ObservableObject"/> that specifies property dependencies.
            /// </summary>
            private class MappedNotifyChanged : Dictionary<string, string[]>, IRaiseChanged
            {
                #region Methods

                /// <summary>
                /// Raises the property change notification for the specified <see cref="ObservableObject"/>.
                /// </summary>
                /// <param name="observable">The target observable.</param>
                /// <param name="propertyName">The name of the property that changed.</param>
                void IRaiseChanged.Raise(ObservableObject observable, string propertyName)
                {
                    if (string.IsNullOrEmpty(propertyName))
                    {
                        observable.OnPropertyChanged(EventArgsCache.ObjectPropertyChanged);

                        return;
                    }

                    Raise(observable, propertyName);

                    if (TryGetValue(propertyName, out string[] properties))
                    {
                        for (int i = 0; i < properties.Length; i++)
                        {
                            Raise(observable, properties[i]);
                        }
                    }
                }

                /// <summary>
                /// Raises the property change notification for the specified <see cref="ObservableObject"/>.
                /// </summary>
                /// <param name="observable">The target observable.</param>
                /// <param name="propertyName">The name of the property that changed.</param>
                protected virtual void Raise(ObservableObject observable, string propertyName)
                {
                    observable.ValidateProperty(propertyName);

                    observable.OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
                }

                #endregion
            }

            /// <summary>
            /// Manages the property change notification of the <see cref="ObservableObject"/> that declares an indexer and specifies property dependencies.
            /// </summary>
            private class IndexerMappedNotifyChanged : MappedNotifyChanged
            {
                #region Fields

                /// <summary>
                /// The name of the indexer property.
                /// </summary>
                private readonly string indexerName;

                #endregion

                #region Constructors

                /// <summary>
                /// Initializes a new instance of the <see cref="IndexerMappedNotifyChanged"/> class.
                /// </summary>
                /// <param name="indexerName">The name of the indexer property.</param>
                internal IndexerMappedNotifyChanged(string indexerName)
                {
                    this.indexerName = indexerName;
                }

                #endregion

                #region Methods

                /// <summary>
                /// Raises the property change notification for the specified <see cref="ObservableObject"/>.
                /// </summary>
                /// <param name="observable">The target observable.</param>
                /// <param name="propertyName">The name of the property that changed.</param>
                protected override void Raise(ObservableObject observable, string propertyName)
                {
                    if (propertyName == indexerName)
                    {
                        observable.OnPropertyChanged(EventArgsCache.IndexerPropertyChanged);

                        return;
                    }

                    base.Raise(observable, propertyName);
                }

                #endregion
            }

            #endregion
        }

        #endregion
    }
}
