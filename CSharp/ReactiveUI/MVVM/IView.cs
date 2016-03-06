using ReactiveUI;

namespace Ember.ReactiveUI.MVVM
{
    public interface IView : IViewFor
    {
    }

    public interface IView<TViewModel> : IViewFor<TViewModel>, IView
        where TViewModel : class
    {
    }
}
