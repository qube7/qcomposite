using System;
using System.Threading;

namespace Qube7.Threading
{
    /// <summary>
    /// Represents a lock that is used to manage access to a resource.
    /// </summary>
    public class Lock : IDisposable
    {
        #region Fields

        /// <summary>
        /// The inner lock.
        /// </summary>
        private readonly ReaderWriterLockSlim inner = new ReaderWriterLockSlim();

        /// <summary>
        /// The reader lock release.
        /// </summary>
        private readonly IDisposable read;

        /// <summary>
        /// The upgradeable reader lock release.
        /// </summary>
        private readonly IDisposable upgradeable;

        /// <summary>
        /// The writer lock release.
        /// </summary>
        private readonly IDisposable write;

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether the reader lock is acquired by the current thread.
        /// </summary>
        /// <value><c>true</c> if the lock is acquired; otherwise, <c>false</c>.</value>
        public bool IsRead
        {
            get { return inner.RecursiveReadCount > 0; }
        }

        /// <summary>
        /// Gets a value indicating whether the upgradeable reader lock is acquired by the current thread.
        /// </summary>
        /// <value><c>true</c> if the lock is acquired; otherwise, <c>false</c>.</value>
        public bool IsUpgradeable
        {
            get { return inner.RecursiveUpgradeCount > 0; }
        }

        /// <summary>
        /// Gets a value indicating whether the writer lock is acquired by the current thread.
        /// </summary>
        /// <value><c>true</c> if the lock is acquired; otherwise, <c>false</c>.</value>
        public bool IsWrite
        {
            get { return inner.RecursiveWriteCount > 0; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Lock"/> class.
        /// </summary>
        public Lock()
        {
            read = new ReadRelease(inner);
            upgradeable = new UpgradeableRelease(inner);
            write = new WriteRelease(inner);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Acquires the reader lock and returns object to be used for releasing the lock.
        /// </summary>
        /// <returns>An <see cref="IDisposable"/> used for releasing the lock.</returns>
        public IDisposable Read()
        {
            inner.TryEnterReadLock(-1);

            return read;
        }

        /// <summary>
        /// Acquires the upgradeable reader lock and returns object to be used for releasing the lock.
        /// </summary>
        /// <returns>An <see cref="IDisposable"/> used for releasing the lock.</returns>
        public IDisposable Upgradeable()
        {
            inner.TryEnterUpgradeableReadLock(-1);

            return upgradeable;
        }

        /// <summary>
        /// Acquires the writer lock and returns object to be used for releasing the lock.
        /// </summary>
        /// <returns>An <see cref="IDisposable"/> used for releasing the lock.</returns>
        public IDisposable Write()
        {
            inner.TryEnterWriteLock(-1);

            return write;
        }

        /// <summary>
        /// Releases all resources used by the <see cref="Lock"/>.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases resources used by the <see cref="Lock"/>.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                inner.Dispose();
            }
        }

        #endregion

        #region Nested types

        /// <summary>
        /// Represents an object used for releasing the reader lock.
        /// </summary>
        private struct ReadRelease : IDisposable
        {
            #region Fields

            /// <summary>
            /// The inner lock.
            /// </summary>
            private ReaderWriterLockSlim inner;

            #endregion

            #region Constructors

            /// <summary>
            /// Initializes a new instance of the <see cref="ReadRelease"/> struct.
            /// </summary>
            /// <param name="inner">The inner lock.</param>
            internal ReadRelease(ReaderWriterLockSlim inner)
            {
                this.inner = inner;
            }

            #endregion

            #region Methods

            /// <summary>
            /// Releases the current reader lock.
            /// </summary>
            public void Dispose()
            {
                inner.ExitReadLock();
            }

            #endregion
        }

        /// <summary>
        /// Represents an object used for releasing the upgradeable reader lock.
        /// </summary>
        private struct UpgradeableRelease : IDisposable
        {
            #region Fields

            /// <summary>
            /// The inner lock.
            /// </summary>
            private ReaderWriterLockSlim inner;

            #endregion

            #region Constructors

            /// <summary>
            /// Initializes a new instance of the <see cref="UpgradeableRelease"/> struct.
            /// </summary>
            /// <param name="inner">The inner lock.</param>
            internal UpgradeableRelease(ReaderWriterLockSlim inner)
            {
                this.inner = inner;
            }

            #endregion

            #region Methods

            /// <summary>
            /// Releases the current upgradeable reader lock.
            /// </summary>
            public void Dispose()
            {
                inner.ExitUpgradeableReadLock();
            }

            #endregion
        }

        /// <summary>
        /// Represents an object used for releasing the writer lock.
        /// </summary>
        private struct WriteRelease : IDisposable
        {
            #region Fields

            /// <summary>
            /// The inner lock.
            /// </summary>
            private ReaderWriterLockSlim inner;

            #endregion

            #region Constructors

            /// <summary>
            /// Initializes a new instance of the <see cref="WriteRelease"/> struct.
            /// </summary>
            /// <param name="inner">The inner lock.</param>
            internal WriteRelease(ReaderWriterLockSlim inner)
            {
                this.inner = inner;
            }

            #endregion

            #region Methods

            /// <summary>
            /// Releases the current writer lock.
            /// </summary>
            public void Dispose()
            {
                inner.ExitWriteLock();
            }

            #endregion
        }

        #endregion
    }
}
