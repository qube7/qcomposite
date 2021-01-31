using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Qube7.Collections
{
    /// <summary>
    /// Represents a collection of values associated with weakly referenced keys.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys in the table.</typeparam>
    /// <typeparam name="TValue">The type of the values in the table.</typeparam>
    [DebuggerDisplay("Count = {Count}")]
    public class WeakTable<TKey, TValue> : IDictionary<TKey, TValue>, IDisposable where TKey : class
    {
        #region Fields

        /// <summary>
        /// The key equality comparer.
        /// </summary>
        private readonly IEqualityComparer<TKey> comparer;

        /// <summary>
        /// The buckets array.
        /// </summary>
        private int[] buckets;

        /// <summary>
        /// The table entries array.
        /// </summary>
        private Entry[] entries;

        /// <summary>
        /// The index of the first free element.
        /// </summary>
        private int freeList;

        /// <summary>
        /// The index of the last element.
        /// </summary>
        private int lastIndex;

        /// <summary>
        /// The total number of elements.
        /// </summary>
        private int count;

        /// <summary>
        /// The current version.
        /// </summary>
        private int version;

        /// <summary>
        /// The keys collection.
        /// </summary>
        private KeyCollection keys;

        /// <summary>
        /// The values collection.
        /// </summary>
        private ValueCollection values;

        #endregion

        #region Properties

        /// <summary>
        /// Gets a collection containing the keys in the <see cref="WeakTable{TKey, TValue}"/>.
        /// </summary>
        /// <value>A collection containing the keys in the <see cref="WeakTable{TKey, TValue}"/>.</value>
        public ICollection<TKey> Keys
        {
            get
            {
                if (keys == null)
                {
                    keys = new KeyCollection(this);
                }

                return keys;
            }
        }

        /// <summary>
        /// Gets a collection containing the values in the <see cref="WeakTable{TKey, TValue}"/>.
        /// </summary>
        /// <value>A collection containing the values in the <see cref="WeakTable{TKey, TValue}"/>.</value>
        public ICollection<TValue> Values
        {
            get
            {
                if (values == null)
                {
                    values = new ValueCollection(this);
                }

                return values;
            }
        }

        /// <summary>
        /// Gets the number of key/value pairs contained in the <see cref="WeakTable{TKey, TValue}"/>.
        /// </summary>
        /// <value>The number of key/value pairs contained in the <see cref="WeakTable{TKey, TValue}"/>.</value>
        public int Count
        {
            get { return count; }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="WeakTable{TKey, TValue}"/> is read-only.
        /// </summary>
        /// <value><c>false</c> indicating that the <see cref="WeakTable{TKey, TValue}"/> is not read-only.</value>
        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly
        {
            get { return false; }
        }

        #endregion

        #region Indexers

        /// <summary>
        /// Gets or sets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key of the value to get or set.</param>
        /// <value>The value associated with the specified key.</value>
        public TValue this[TKey key]
        {
            get
            {
                int index = FindEntry(key);
                if (index >= 0)
                {
                    return entries[index].value;
                }

                throw Error.KeyNotFound();
            }
            set
            {
                Insert(key, value, false);
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WeakTable{TKey, TValue}"/> class.
        /// </summary>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}"/> implementation to use when comparing keys, or <c>null</c> to use the default equality comparer.</param>
        public WeakTable(IEqualityComparer<TKey> comparer)
        {
            this.comparer = comparer ?? EqualityComparer<TKey>.Default;

            Initialize(0);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WeakTable{TKey, TValue}"/> class.
        /// </summary>
        public WeakTable() : this(null)
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes the <see cref="WeakTable{TKey, TValue}"/> with the specified capacity.
        /// </summary>
        /// <param name="capacity">The capacity.</param>
        private void Initialize(int capacity)
        {
            int length = HashHelper.GetPrime(capacity);

            entries = new Entry[length];
            buckets = new int[length];

            freeList = -1;
        }

        /// <summary>
        /// Ensures that the capacity of the <see cref="WeakTable{TKey, TValue}"/> is at least the specified value.
        /// </summary>
        /// <param name="capacity">The minimum capacity to ensure.</param>
        private void EnsureCapacity(int capacity)
        {
            if (capacity < 0)
            {
                throw Error.Argument(Strings.CapacityOverflow, nameof(capacity));
            }

            if (entries.Length < capacity)
            {
                int length = capacity * 2;

                if ((length = HashHelper.GetPrime(length < capacity ? capacity : length)) == 0)
                {
                    throw Error.Argument(Strings.CapacityOverflow, nameof(capacity));
                }

                Resize(length);
            }
        }

        /// <summary>
        /// Changes the current capacity of the <see cref="WeakTable{TKey, TValue}"/> to the specified value.
        /// </summary>
        /// <param name="capacity">The new capacity.</param>
        private void Resize(int capacity)
        {
            Entry[] entryArray = new Entry[capacity];

            Array.Copy(entries, 0, entryArray, 0, lastIndex);

            int[] bucketArray = new int[capacity];
            for (int i = 0; i < lastIndex; i++)
            {
                int index = entryArray[i].hashCode % capacity;
                entryArray[i].next = bucketArray[index] - 1;
                bucketArray[index] = i + 1;
            }

            entries = entryArray;
            buckets = bucketArray;
        }

        /// <summary>
        /// Returns a hash code for the specified key.
        /// </summary>
        /// <param name="key">The key to retrieve the hash code for.</param>
        /// <returns>A hash code for the specified key.</returns>
        private int GetHashCode(TKey key)
        {
            return comparer.GetHashCode(key) & int.MaxValue;
        }

        /// <summary>
        /// Adds the specified key and value to the <see cref="WeakTable{TKey, TValue}"/>.
        /// </summary>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="value">The value of the element to add.</param>
        public void Add(TKey key, TValue value)
        {
            Insert(key, value, true);
        }

        /// <summary>
        /// Adds the specified key/value pair to the <see cref="WeakTable{TKey, TValue}"/>.
        /// </summary>
        /// <param name="item">The key/value pair to add.</param>
        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        /// <summary>
        /// Inserts the specified key and value to the <see cref="WeakTable{TKey, TValue}"/>.
        /// </summary>
        /// <param name="key">The key of the element to insert.</param>
        /// <param name="value">The value of the element to insert.</param>
        /// <param name="add"><c>true</c> to throw an exception if the key is found; <c>false</c> to set the specified value.</param>
        private void Insert(TKey key, TValue value, bool add)
        {
            Requires.NotNull(key, nameof(key));

            int hashCode = GetHashCode(key);

            for (int i = buckets[hashCode % buckets.Length] - 1; i >= 0; i = entries[i].next)
            {
                if (entries[i].hashCode == hashCode && comparer.Equals(entries[i].Target, key))
                {
                    if (add)
                    {
                        throw Error.Argument(Strings.AddingDuplicate, nameof(key));
                    }

                    entries[i].value = value;

                    version++;

                    return;
                }
            }

            int free;
            if (freeList >= 0)
            {
                free = freeList;

                freeList = entries[free].next;

                entries[free].Target = key;
            }
            else
            {
                EnsureCapacity(lastIndex + 1);

                free = lastIndex;

                lastIndex++;

                entries[free] = new Entry(key);
            }

            int index = hashCode % buckets.Length;

            entries[free].hashCode = hashCode;
            entries[free].next = buckets[index] - 1;
            entries[free].value = value;

            buckets[index] = free + 1;

            count++;
            version++;
        }

        /// <summary>
        /// Removes all keys and values from the <see cref="WeakTable{TKey, TValue}"/>.
        /// </summary>
        public void Clear()
        {
            if (lastIndex > 0)
            {
                for (int i = 0; i < lastIndex; i++)
                {
                    entries[i].Dispose();
                }

                Array.Clear(entries, 0, lastIndex);
                Array.Clear(buckets, 0, buckets.Length);

                freeList = -1;
                lastIndex = 0;
                count = 0;
            }

            version++;
        }

        /// <summary>
        /// Determines whether the <see cref="WeakTable{TKey, TValue}"/> contains the specified key.
        /// </summary>
        /// <param name="key">The key to locate in the <see cref="WeakTable{TKey, TValue}"/>.</param>
        /// <returns><c>true</c> if the <see cref="WeakTable{TKey, TValue}"/> contains an element with the specified key; otherwise, <c>false</c>.</returns>
        public bool ContainsKey(TKey key)
        {
            return FindEntry(key) >= 0;
        }

        /// <summary>
        /// Determines whether the <see cref="WeakTable{TKey, TValue}"/> contains the specified value.
        /// </summary>
        /// <param name="value">The value to locate in the <see cref="WeakTable{TKey, TValue}"/>.</param>
        /// <returns><c>true</c> if the <see cref="WeakTable{TKey, TValue}"/> contains an element with the specified value; otherwise, <c>false</c>.</returns>
        public bool ContainsValue(TValue value)
        {
            if (value == null)
            {
                for (int i = 0; i < lastIndex; i++)
                {
                    if (entries[i].hashCode >= 0 && entries[i].value == null)
                    {
                        return true;
                    }
                }
            }
            else
            {
                EqualityComparer<TValue> comparer = EqualityComparer<TValue>.Default;

                for (int i = 0; i < lastIndex; i++)
                {
                    if (entries[i].hashCode >= 0 && comparer.Equals(entries[i].value, value))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Determines whether the <see cref="WeakTable{TKey, TValue}"/> contains the specified key/value pair.
        /// </summary>
        /// <param name="item">The key/value pair to locate in the <see cref="WeakTable{TKey, TValue}"/>.</param>
        /// <returns><c>true</c> if the <see cref="WeakTable{TKey, TValue}"/> contains the specified key/value pair; otherwise, <c>false</c>.</returns>
        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
        {
            int index = FindEntry(item.Key);

            return index >= 0 && EqualityComparer<TValue>.Default.Equals(entries[index].value, item.Value);
        }

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key of the value to get.</param>
        /// <param name="value">When this method returns, contains the value associated with the specified key, if the key is found; otherwise, the default value for the type of the value parameter.</param>
        /// <returns><c>true</c> if the <see cref="WeakTable{TKey, TValue}"/> contains an element with the specified key; otherwise, <c>false</c>.</returns>
        public bool TryGetValue(TKey key, out TValue value)
        {
            int index = FindEntry(key);
            if (index >= 0)
            {
                value = entries[index].value;

                return true;
            }

            value = default;

            return false;
        }

        /// <summary>
        /// Searches for an element with the specified key.
        /// </summary>
        /// <param name="key">The key of the element to find.</param>
        /// <returns>The zero-based index of an element with the specified key, if found; otherwise, -1.</returns>
        private int FindEntry(TKey key)
        {
            Requires.NotNull(key, nameof(key));

            int hashCode = GetHashCode(key);

            for (int i = buckets[hashCode % buckets.Length] - 1; i >= 0; i = entries[i].next)
            {
                if (entries[i].hashCode == hashCode && comparer.Equals(entries[i].Target, key))
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// Copies the elements of the <see cref="WeakTable{TKey, TValue}"/> to the specified array, starting at the specified index.
        /// </summary>
        /// <param name="array">The one-dimensional array that is the destination of the elements copied from the current <see cref="WeakTable{TKey, TValue}"/>.</param>
        /// <param name="index">The zero-based index in <paramref name="array"/> at which copying begins.</param>
        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int index)
        {
            Requires.NotNull(array, nameof(array));
            Requires.NonNegative(index, nameof(index));

            if (array.Length - index < count)
            {
                throw Error.Argument(Strings.OffsetLengthInvalid);
            }

            for (int i = 0; i < lastIndex; i++)
            {
                if (entries[i].hashCode >= 0)
                {
                    array[index++] = KeyValuePair.Create(entries[i].Target, entries[i].value);
                }
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="WeakTable{TKey, TValue}"/>.
        /// </summary>
        /// <returns>A enumerator that can be used to iterate through the <see cref="WeakTable{TKey, TValue}"/>.</returns>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return new Enumerator(this);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="WeakTable{TKey, TValue}"/>.
        /// </summary>
        /// <returns>A enumerator that can be used to iterate through the <see cref="WeakTable{TKey, TValue}"/>.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator(this);
        }

        /// <summary>
        /// Removes the value with the specified key from the <see cref="WeakTable{TKey, TValue}"/>.
        /// </summary>
        /// <param name="key">The key of the element to remove.</param>
        /// <returns><c>true</c> if the element is successfully found and removed; otherwise, <c>false</c>.</returns>
        public bool Remove(TKey key)
        {
            Requires.NotNull(key, nameof(key));

            int hashCode = GetHashCode(key);

            int index = -1;
            for (int i = buckets[hashCode % buckets.Length] - 1; i >= 0; i = entries[i].next)
            {
                if (entries[i].hashCode == hashCode && comparer.Equals(entries[i].Target, key))
                {
                    if (index < 0)
                    {
                        buckets[hashCode % buckets.Length] = entries[i].next + 1;
                    }
                    else
                    {
                        entries[index].next = entries[i].next;
                    }

                    entries[i].hashCode = -1;
                    entries[i].next = freeList;
                    entries[i].value = default;
                    freeList = i;

                    count--;
                    version++;

                    return true;
                }

                index = i;
            }

            return false;
        }

        /// <summary>
        /// Removes the specified key/value pair from the <see cref="WeakTable{TKey, TValue}"/>.
        /// </summary>
        /// <param name="item">The key/value pair to remove.</param>
        /// <returns><c>true</c> if the key/value pair is successfully found and removed; otherwise, <c>false</c>.</returns>
        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            int index = FindEntry(item.Key);
            if (index >= 0 && EqualityComparer<TValue>.Default.Equals(entries[index].value, item.Value))
            {
                Remove(item.Key);

                return true;
            }

            return false;
        }

        /// <summary>
        /// Sets the capacity to the actual number of elements in the <see cref="WeakTable{TKey, TValue}"/>, if that number is less than a threshold value.
        /// </summary>
        public void TrimExcess()
        {
            int length = HashHelper.GetPrime(count);

            if (length < entries.Length * 0.9)
            {
                Entry[] entryArray = new Entry[length];
                int[] bucketArray = new int[length];

                int j = 0;
                for (int i = 0; i < lastIndex; i++)
                {
                    if (entries[i].hashCode >= 0)
                    {
                        entryArray[j] = entries[i];

                        int index = entryArray[j].hashCode % length;
                        entryArray[j].next = bucketArray[index] - 1;
                        bucketArray[index] = j + 1;

                        j++;
                    }
                    else
                    {
                        entries[i].Dispose();
                    }
                }

                lastIndex = j;

                entries = entryArray;
                buckets = bucketArray;

                freeList = -1;
            }
        }

        /// <summary>
        /// Removes from the <see cref="WeakTable{TKey, TValue}"/> values associated with the keys that are no longer accessible.
        /// </summary>
        /// <returns>The number of elements removed from the <see cref="WeakTable{TKey, TValue}"/>.</returns>
        public int Purge()
        {
            return Purge(out _);
        }

        /// <summary>
        /// Removes from the <see cref="WeakTable{TKey, TValue}"/> values associated with the keys that are no longer accessible.
        /// </summary>
        /// <param name="array">The array of values removed from the <see cref="WeakTable{TKey, TValue}"/>.</param>
        /// <returns>The number of elements removed from the <see cref="WeakTable{TKey, TValue}"/>.</returns>
        public int Purge(out TValue[] array)
        {
            List<TValue> values = new List<TValue>();

            int length = HashHelper.GetPrime(count);

            Entry[] entryArray = new Entry[length];
            int[] bucketArray = new int[length];

            int j = 0;
            int num = 0;
            for (int i = 0; i < lastIndex; i++)
            {
                if (entries[i].hashCode >= 0)
                {
                    if (entries[i].IsAlive)
                    {
                        entryArray[j] = entries[i];

                        int index = entryArray[j].hashCode % length;
                        entryArray[j].next = bucketArray[index] - 1;
                        bucketArray[index] = j + 1;

                        j++;
                    }
                    else
                    {
                        values.Add(entries[i].value);

                        entries[i].Dispose();

                        num++;
                    }
                }
                else
                {
                    entries[i].Dispose();
                }
            }

            lastIndex = j;

            entries = entryArray;
            buckets = bucketArray;

            freeList = -1;
            count -= num;
            version++;

            array = values.ToArray();

            return num;
        }

        /// <summary>
        /// Releases all resources used by the <see cref="WeakTable{TKey, TValue}"/>.
        /// </summary>
        void IDisposable.Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases resources used by the <see cref="WeakTable{TKey, TValue}"/>.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Clear();
            }
        }

        #endregion

        #region Nested types

        /// <summary>
        /// Defines the key/value pair stored in the <see cref="WeakTable{TKey, TValue}"/>.
        /// </summary>
        private class Entry : Weak<TKey>
        {
            #region Fields

            /// <summary>
            /// The hash code of the key.
            /// </summary>
            internal int hashCode;

            /// <summary>
            /// The value of the key/value pair.
            /// </summary>
            internal TValue value;

            /// <summary>
            /// The index of the next element in the list.
            /// </summary>
            internal int next;

            #endregion

            #region Constructors

            /// <summary>
            /// Initializes a new instance of the <see cref="Entry"/> class.
            /// </summary>
            /// <param name="key">The key of the key/value pair.</param>
            internal Entry(TKey key) : base(key)
            {
            }

            #endregion
        }

        /// <summary>
        /// Enumerates the elements of a <see cref="WeakTable{TKey, TValue}"/>.
        /// </summary>
        private struct Enumerator : IEnumerator<KeyValuePair<TKey, TValue>>
        {
            #region Fields

            /// <summary>
            /// The underlying table.
            /// </summary>
            private readonly WeakTable<TKey, TValue> table;

            /// <summary>
            /// The current position.
            /// </summary>
            private int index;

            /// <summary>
            /// The version of the table.
            /// </summary>
            private int version;

            /// <summary>
            /// The current entry.
            /// </summary>
            private Entry current;

            #endregion

            #region Properties

            /// <summary>
            /// Gets the element at the current position of the enumerator.
            /// </summary>
            /// <value>The element in the <see cref="WeakTable{TKey, TValue}"/> at the current position of the enumerator.</value>
            public KeyValuePair<TKey, TValue> Current
            {
                get { return KeyValuePair.Create(current.Target, current.value); }
            }

            /// <summary>
            /// Gets the element at the current position of the enumerator.
            /// </summary>
            /// <value>The element in the <see cref="WeakTable{TKey, TValue}"/> at the current position of the enumerator.</value>
            object IEnumerator.Current
            {
                get
                {
                    if (index == 0 || index > table.lastIndex)
                    {
                        throw Error.InvalidOperation(Strings.EnumeratorInvalidPosition);
                    }

                    return KeyValuePair.Create(current.Target, current.value);
                }
            }

            #endregion

            #region Constructors

            /// <summary>
            /// Initializes a new instance of the <see cref="Enumerator"/> struct.
            /// </summary>
            /// <param name="table">The underlying table.</param>
            internal Enumerator(WeakTable<TKey, TValue> table) : this()
            {
                this.table = table;

                version = table.version;
            }

            #endregion

            #region Methods

            /// <summary>
            /// Advances the enumerator to the next element of the <see cref="WeakTable{TKey, TValue}"/>.
            /// </summary>
            /// <returns><c>true</c> if the enumerator was successfully advanced to the next element; <c>false</c> if the enumerator has passed the end of the collection.</returns>
            public bool MoveNext()
            {
                if (version != table.version)
                {
                    throw Error.InvalidOperation(Strings.EnumeratorFailedVersion);
                }

                while (index < table.lastIndex)
                {
                    if (table.entries[index].hashCode >= 0)
                    {
                        current = table.entries[index++];

                        return true;
                    }

                    index++;
                }

                index = table.lastIndex + 1;

                current = null;

                return false;
            }

            /// <summary>
            /// Sets the enumerator to its initial position, which is before the first element in the collection.
            /// </summary>
            public void Reset()
            {
                if (version != table.version)
                {
                    throw Error.InvalidOperation(Strings.EnumeratorFailedVersion);
                }

                index = 0;

                current = null;
            }

            /// <summary>
            /// Releases all resources used by the <see cref="Enumerator"/>.
            /// </summary>
            public void Dispose()
            {
                index = 0;

                current = null;
            }

            #endregion
        }

        /// <summary>
        /// Represents the collection of keys in a <see cref="WeakTable{TKey, TValue}"/>.
        /// </summary>
        [DebuggerDisplay("Count = {Count}")]
        private class KeyCollection : ICollection<TKey>
        {
            #region Fields

            /// <summary>
            /// The underlying table.
            /// </summary>
            private readonly WeakTable<TKey, TValue> table;

            #endregion

            #region Properties

            /// <summary>
            /// Gets the number of elements contained in the <see cref="KeyCollection"/>.
            /// </summary>
            /// <value>The number of elements contained in the <see cref="KeyCollection"/>.</value>
            public int Count
            {
                get { return table.Count; }
            }

            /// <summary>
            /// Gets a value indicating whether the <see cref="KeyCollection"/> is read-only.
            /// </summary>
            /// <value><c>true</c> indicating that the <see cref="KeyCollection"/> is read-only.</value>
            public bool IsReadOnly
            {
                get { return true; }
            }

            #endregion

            #region Constructors

            /// <summary>
            /// Initializes a new instance of the <see cref="KeyCollection"/> class.
            /// </summary>
            /// <param name="table">The underlying table.</param>
            internal KeyCollection(WeakTable<TKey, TValue> table)
            {
                this.table = table;
            }

            #endregion

            #region Methods

            /// <summary>
            /// Determines whether the <see cref="KeyCollection"/> contains a specific value.
            /// </summary>
            /// <param name="item">The object to locate in the <see cref="KeyCollection"/>.</param>
            /// <returns><c>true</c> if <paramref name="item"/> is found in the <see cref="KeyCollection"/>; otherwise, <c>false</c>.</returns>
            public bool Contains(TKey item)
            {
                return table.ContainsKey(item);
            }

            /// <summary>
            /// Copies the elements of the <see cref="KeyCollection"/> to the specified array, starting at the specified index.
            /// </summary>
            /// <param name="array">The one-dimensional array that is the destination of the elements copied from the current <see cref="KeyCollection"/>.</param>
            /// <param name="index">The zero-based index in <paramref name="array"/> at which copying begins.</param>
            public void CopyTo(TKey[] array, int index)
            {
                Requires.NotNull(array, nameof(array));
                Requires.NonNegative(index, nameof(index));

                if (array.Length - index < table.count)
                {
                    throw Error.Argument(Strings.OffsetLengthInvalid);
                }

                int lastIndex = table.lastIndex;
                Entry[] entries = table.entries;

                for (int i = 0; i < lastIndex; i++)
                {
                    if (entries[i].hashCode >= 0)
                    {
                        array[index++] = entries[i].Target;
                    }
                }
            }

            /// <summary>
            /// Returns an enumerator that iterates through the <see cref="KeyCollection"/>.
            /// </summary>
            /// <returns>A enumerator that can be used to iterate through the <see cref="KeyCollection"/>.</returns>
            public IEnumerator<TKey> GetEnumerator()
            {
                return new Enumerator(table);
            }

            /// <summary>
            /// Returns an enumerator that iterates through the <see cref="KeyCollection"/>.
            /// </summary>
            /// <returns>A enumerator that can be used to iterate through the <see cref="KeyCollection"/>.</returns>
            IEnumerator IEnumerable.GetEnumerator()
            {
                return new Enumerator(table);
            }

            /// <summary>
            /// Add operation is not supported.
            /// </summary>
            /// <param name="item">The object to add to the <see cref="KeyCollection"/>.</param>
            void ICollection<TKey>.Add(TKey item)
            {
                throw Error.NotSupported(Strings.ReadOnlyCollection);
            }

            /// <summary>
            /// Clear operation is not supported.
            /// </summary>
            void ICollection<TKey>.Clear()
            {
                throw Error.NotSupported(Strings.ReadOnlyCollection);
            }

            /// <summary>
            /// Remove operation is not supported.
            /// </summary>
            /// <param name="item">The object to remove from the <see cref="KeyCollection"/>.</param>
            /// <returns><c>true</c> if <paramref name="item"/> was successfully removed from the <see cref="KeyCollection"/>; otherwise, <c>false</c>.</returns>
            bool ICollection<TKey>.Remove(TKey item)
            {
                throw Error.NotSupported(Strings.ReadOnlyCollection);
            }

            #endregion

            #region Nested types

            /// <summary>
            /// Enumerates the elements of a <see cref="KeyCollection"/>.
            /// </summary>
            private struct Enumerator : IEnumerator<TKey>
            {
                #region Fields

                /// <summary>
                /// The underlying table.
                /// </summary>
                private readonly WeakTable<TKey, TValue> table;

                /// <summary>
                /// The current position.
                /// </summary>
                private int index;

                /// <summary>
                /// The version of the table.
                /// </summary>
                private int version;

                /// <summary>
                /// The current entry.
                /// </summary>
                private Entry current;

                #endregion

                #region Properties

                /// <summary>
                /// Gets the element at the current position of the enumerator.
                /// </summary>
                /// <value>The element in the <see cref="KeyCollection"/> at the current position of the enumerator.</value>
                public TKey Current
                {
                    get { return current.Target; }
                }

                /// <summary>
                /// Gets the element at the current position of the enumerator.
                /// </summary>
                /// <value>The element in the <see cref="KeyCollection"/> at the current position of the enumerator.</value>
                object IEnumerator.Current
                {
                    get
                    {
                        if (index == 0 || index > table.lastIndex)
                        {
                            throw Error.InvalidOperation(Strings.EnumeratorInvalidPosition);
                        }

                        return current.Target;
                    }
                }

                #endregion

                #region Constructors

                /// <summary>
                /// Initializes a new instance of the <see cref="Enumerator"/> struct.
                /// </summary>
                /// <param name="table">The underlying table.</param>
                internal Enumerator(WeakTable<TKey, TValue> table) : this()
                {
                    this.table = table;

                    version = table.version;
                }

                #endregion

                #region Methods

                /// <summary>
                /// Advances the enumerator to the next element of the <see cref="KeyCollection"/>.
                /// </summary>
                /// <returns><c>true</c> if the enumerator was successfully advanced to the next element; <c>false</c> if the enumerator has passed the end of the collection.</returns>
                public bool MoveNext()
                {
                    if (version != table.version)
                    {
                        throw Error.InvalidOperation(Strings.EnumeratorFailedVersion);
                    }

                    while (index < table.lastIndex)
                    {
                        if (table.entries[index].hashCode >= 0)
                        {
                            current = table.entries[index++];

                            return true;
                        }

                        index++;
                    }

                    index = table.lastIndex + 1;

                    current = null;

                    return false;
                }

                /// <summary>
                /// Sets the enumerator to its initial position, which is before the first element in the collection.
                /// </summary>
                public void Reset()
                {
                    if (version != table.version)
                    {
                        throw Error.InvalidOperation(Strings.EnumeratorFailedVersion);
                    }

                    index = 0;

                    current = null;
                }

                /// <summary>
                /// Releases all resources used by the <see cref="Enumerator"/>.
                /// </summary>
                public void Dispose()
                {
                    index = 0;

                    current = null;
                }

                #endregion
            }

            #endregion
        }

        /// <summary>
        /// Represents the collection of values in a <see cref="WeakTable{TKey, TValue}"/>.
        /// </summary>
        [DebuggerDisplay("Count = {Count}")]
        private class ValueCollection : ICollection<TValue>
        {
            #region Fields

            /// <summary>
            /// The underlying table.
            /// </summary>
            private readonly WeakTable<TKey, TValue> table;

            #endregion

            #region Properties

            /// <summary>
            /// Gets the number of elements contained in the <see cref="ValueCollection"/>.
            /// </summary>
            /// <value>The number of elements contained in the <see cref="ValueCollection"/>.</value>
            public int Count
            {
                get { return table.Count; }
            }

            /// <summary>
            /// Gets a value indicating whether the <see cref="ValueCollection"/> is read-only.
            /// </summary>
            /// <value><c>true</c> indicating that the <see cref="ValueCollection"/> is read-only.</value>
            public bool IsReadOnly
            {
                get { return true; }
            }

            #endregion

            #region Constructors

            /// <summary>
            /// Initializes a new instance of the <see cref="ValueCollection"/> class.
            /// </summary>
            /// <param name="table">The underlying table.</param>
            internal ValueCollection(WeakTable<TKey, TValue> table)
            {
                this.table = table;
            }

            #endregion

            #region Methods

            /// <summary>
            /// Determines whether the <see cref="ValueCollection"/> contains a specific value.
            /// </summary>
            /// <param name="item">The object to locate in the <see cref="ValueCollection"/>.</param>
            /// <returns><c>true</c> if <paramref name="item"/> is found in the <see cref="ValueCollection"/>; otherwise, <c>false</c>.</returns>
            public bool Contains(TValue item)
            {
                return table.ContainsValue(item);
            }

            /// <summary>
            /// Copies the elements of the <see cref="ValueCollection"/> to the specified array, starting at the specified index.
            /// </summary>
            /// <param name="array">The one-dimensional array that is the destination of the elements copied from the current <see cref="ValueCollection"/>.</param>
            /// <param name="index">The zero-based index in <paramref name="array"/> at which copying begins.</param>
            public void CopyTo(TValue[] array, int index)
            {
                Requires.NotNull(array, nameof(array));
                Requires.NonNegative(index, nameof(index));

                if (array.Length - index < table.count)
                {
                    throw Error.Argument(Strings.OffsetLengthInvalid);
                }

                int lastIndex = table.lastIndex;
                Entry[] entries = table.entries;

                for (int i = 0; i < lastIndex; i++)
                {
                    if (entries[i].hashCode >= 0)
                    {
                        array[index++] = entries[i].value;
                    }
                }
            }

            /// <summary>
            /// Returns an enumerator that iterates through the <see cref="ValueCollection"/>.
            /// </summary>
            /// <returns>A enumerator that can be used to iterate through the <see cref="ValueCollection"/>.</returns>
            public IEnumerator<TValue> GetEnumerator()
            {
                return new Enumerator(table);
            }

            /// <summary>
            /// Returns an enumerator that iterates through the <see cref="ValueCollection"/>.
            /// </summary>
            /// <returns>A enumerator that can be used to iterate through the <see cref="ValueCollection"/>.</returns>
            IEnumerator IEnumerable.GetEnumerator()
            {
                return new Enumerator(table);
            }

            /// <summary>
            /// Add operation is not supported.
            /// </summary>
            /// <param name="item">The object to add to the <see cref="ValueCollection"/>.</param>
            void ICollection<TValue>.Add(TValue item)
            {
                throw Error.NotSupported(Strings.ReadOnlyCollection);
            }

            /// <summary>
            /// Clear operation is not supported.
            /// </summary>
            void ICollection<TValue>.Clear()
            {
                throw Error.NotSupported(Strings.ReadOnlyCollection);
            }

            /// <summary>
            /// Remove operation is not supported.
            /// </summary>
            /// <param name="item">The object to remove from the <see cref="ValueCollection"/>.</param>
            /// <returns><c>true</c> if <paramref name="item"/> was successfully removed from the <see cref="ValueCollection"/>; otherwise, <c>false</c>.</returns>
            bool ICollection<TValue>.Remove(TValue item)
            {
                throw Error.NotSupported(Strings.ReadOnlyCollection);
            }

            #endregion

            #region Nested types

            /// <summary>
            /// Enumerates the elements of a <see cref="ValueCollection"/>.
            /// </summary>
            private struct Enumerator : IEnumerator<TValue>
            {
                #region Fields

                /// <summary>
                /// The underlying table.
                /// </summary>
                private readonly WeakTable<TKey, TValue> table;

                /// <summary>
                /// The current position.
                /// </summary>
                private int index;

                /// <summary>
                /// The version of the table.
                /// </summary>
                private int version;

                /// <summary>
                /// The current entry.
                /// </summary>
                private Entry current;

                #endregion

                #region Properties

                /// <summary>
                /// Gets the element at the current position of the enumerator.
                /// </summary>
                /// <value>The element in the <see cref="ValueCollection"/> at the current position of the enumerator.</value>
                public TValue Current
                {
                    get { return current.value; }
                }

                /// <summary>
                /// Gets the element at the current position of the enumerator.
                /// </summary>
                /// <value>The element in the <see cref="ValueCollection"/> at the current position of the enumerator.</value>
                object IEnumerator.Current
                {
                    get
                    {
                        if (index == 0 || index > table.lastIndex)
                        {
                            throw Error.InvalidOperation(Strings.EnumeratorInvalidPosition);
                        }

                        return current.value;
                    }
                }

                #endregion

                #region Constructors

                /// <summary>
                /// Initializes a new instance of the <see cref="Enumerator"/> struct.
                /// </summary>
                /// <param name="table">The underlying table.</param>
                internal Enumerator(WeakTable<TKey, TValue> table) : this()
                {
                    this.table = table;

                    version = table.version;
                }

                #endregion

                #region Methods

                /// <summary>
                /// Advances the enumerator to the next element of the <see cref="ValueCollection"/>.
                /// </summary>
                /// <returns><c>true</c> if the enumerator was successfully advanced to the next element; <c>false</c> if the enumerator has passed the end of the collection.</returns>
                public bool MoveNext()
                {
                    if (version != table.version)
                    {
                        throw Error.InvalidOperation(Strings.EnumeratorFailedVersion);
                    }

                    while (index < table.lastIndex)
                    {
                        if (table.entries[index].hashCode >= 0)
                        {
                            current = table.entries[index++];

                            return true;
                        }

                        index++;
                    }

                    index = table.lastIndex + 1;

                    current = null;

                    return false;
                }

                /// <summary>
                /// Sets the enumerator to its initial position, which is before the first element in the collection.
                /// </summary>
                public void Reset()
                {
                    if (version != table.version)
                    {
                        throw Error.InvalidOperation(Strings.EnumeratorFailedVersion);
                    }

                    index = 0;

                    current = null;
                }

                /// <summary>
                /// Releases all resources used by the <see cref="Enumerator"/>.
                /// </summary>
                public void Dispose()
                {
                    index = 0;

                    current = null;
                }

                #endregion
            }

            #endregion
        }

        #endregion
    }
}
