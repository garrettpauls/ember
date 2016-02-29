using System;
using System.Collections.Generic;
using System.Linq;

namespace Ember
{
    /// <summary>
    ///     This class tracks multiple instances of <see cref="IDisposable" />
    ///     and disposes all tracked instances when the <see cref="DisposableTracker" /> is disposed.
    ///     <br />
    ///     This class is intended to be threadsafe.
    /// </summary>
    /// <exception cref="AggregateException">
    ///     When <see cref="Dispose" /> is called, an aggregate exception is thrown if any of the tracked
    ///     <see cref="IDisposable" /> objects throw an exception when
    ///     <see cref="IDisposable.Dispose" /> is called.
    /// </exception>
    public sealed class DisposableTracker : IDisposable
    {
        private readonly List<IDisposable> mDisposables = new List<IDisposable>();
        private readonly object mDisposedLock = new object();
        private bool mIsDisposed;

        /// <summary>
        ///     Disposes all tracked <see cref="IDisposable" /> objects.
        /// </summary>
        /// <exception cref="AggregateException">
        ///     An aggregate exception is thrown if any of the tracked <see cref="IDisposable" /> objects throw an exception when
        ///     <see cref="IDisposable.Dispose" /> is called.
        /// </exception>
        public void Dispose()
        {
            lock(mDisposedLock)
            {
                if(mIsDisposed)
                {
                    return;
                }

                mIsDisposed = true;
            }

            var exceptions = new List<Exception>();
            foreach(var disposable in mDisposables)
            {
                try
                {
                    disposable.Dispose();
                }
                catch(Exception ex)
                {
                    exceptions.Add(ex);
                }
            }

            if(exceptions.Any())
            {
                throw new AggregateException(exceptions);
            }
        }

        /// <summary>
        ///     Adds the given <paramref name="target" /> to the set of tracked disposables.
        ///     The <paramref name="target" /> will then be disposed when <see cref="Dispose" /> is called on the tracker.
        ///     <br />
        ///     If the tracker has already been disposed the <paramref name="target" /> will have
        ///     <see cref="IDisposable.Dispose" /> called immediately.
        /// </summary>
        /// <param name="target">The target to track.</param>
        /// <returns>The same <paramref name="target" /> that was passed is returned to allow chaining methods.</returns>
        public T Track<T>(T target)
            where T : IDisposable
        {
            var wasAdded = false;

            lock(mDisposedLock)
            {
                if(!mIsDisposed)
                {
                    mDisposables.Add(target);
                    wasAdded = true;
                }
            }

            if(!wasAdded)
            {
                target.Dispose();
            }

            return target;
        }
    }

    /// <summary>
    ///     Extension methods for <see cref="DisposableTracker" />.
    /// </summary>
    public static class DisposableTrackerExtensions
    {
        /// <summary>
        ///     An inversion of <see cref="DisposableTracker.Track{T}" /> which tracks the <paramref name="target" /> with the
        ///     given <see cref="tracker" />.
        /// </summary>
        /// <returns>The same <paramref name="target" /> that was passed is returned to allow chaining methods.</returns>
        public static T TrackWith<T>(this T target, DisposableTracker tracker)
            where T : IDisposable
        {
            return tracker.Track(target);
        }
    }
}
