using System;

using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace Ember.Tests
{
    [TestClass]
    public sealed class DisposableTrackerTests
    {
        [TestMethod]
        public void AddAfterTrackerDisposedDisposesObjectImmediately()
        {
            var tracker = new DisposableTracker();
            var disposable = new Disposable();

            tracker.Dispose();
            tracker.Track(disposable);

            Assert.IsTrue(
                disposable.IsDisposed,
                "the disposable should be disposed immediately when added to a disposed tracker");
        }

        [TestMethod]
        public void AddBeforeTrackerDisposedDoesNotDisposeItem()
        {
            var tracker = new DisposableTracker();

            var disposable = new Disposable();
            tracker.Track(disposable);

            Assert.IsFalse(
                disposable.IsDisposed,
                "the disposable should not be disposed until the tracker is");
        }

        [TestMethod]
        public void AddReturnsSameInstance()
        {
            var tracker = new DisposableTracker();
            var disposable = new Disposable();

            Assert.AreSame(
                disposable, tracker.Track(disposable),
                "the value returned by Add should be the same as the passed object");
        }

        private sealed class Disposable : IDisposable
        {
            public Disposable()
            {
                IsDisposed = false;
            }

            public bool IsDisposed { get; private set; }

            public void Dispose()
            {
                IsDisposed = true;
            }
        }

        [TestMethod]
        public void DisposingTrackerDisposesTrackedObjects()
        {
            var tracker = new DisposableTracker();

            var disposableA = new Disposable();
            var disposableB = new Disposable();

            tracker.Track(disposableA);
            tracker.Track(disposableB);

            Assert.IsFalse(disposableA.IsDisposed);
            Assert.IsFalse(disposableB.IsDisposed);

            tracker.Dispose();

            Assert.IsTrue(disposableA.IsDisposed, "the tracker should dispose all tracked objects (A) when disposed");
            Assert.IsTrue(disposableB.IsDisposed, "the tracker should dispose all tracked objects (B) when disposed");
        }
    }
}
