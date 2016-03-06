using System;

using ReactiveUI;

namespace Ember.ReactiveUI.MVVM
{
    public abstract class DisposableViewModel : ReactiveObject, IDisposable
    {
        protected DisposableTracker Disposables { get; } = new DisposableTracker();

        public virtual void Dispose()
        {
            Disposables.Dispose();
        }
    }
}
