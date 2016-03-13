using System;

using ReactiveUI;

namespace Ember.ReactiveUI.MVVM
{
    public abstract class RoutableViewModel : RoutableViewModel<IScreen>
    {
        protected RoutableViewModel(string urlPathSegment, IScreen hostScreen) : base(urlPathSegment, hostScreen)
        {
        }
    }

    public abstract class RoutableViewModel<THostScreen> : DisposableViewModel, IRoutableViewModel, IDisposable
        where THostScreen : IScreen
    {
        protected RoutableViewModel(string urlPathSegment, THostScreen hostScreen)
        {
            UrlPathSegment = urlPathSegment;
            HostScreen = hostScreen;
        }

        IScreen IRoutableViewModel.HostScreen => HostScreen;

        public THostScreen HostScreen { get; }

        public string UrlPathSegment { get; }
    }
}
