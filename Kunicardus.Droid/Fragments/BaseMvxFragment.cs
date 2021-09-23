using Android.OS;
using Android.Views;
using MvvmCross.Droid.Support.V4;
//using MvvmCross.Droid.Views.Fragments;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using MvvmCross.ViewModels;

namespace Kunicardus.Droid
{
    public abstract class BaseMvxFragment<TViewModel> : MvxFragment<TViewModel> where TViewModel : class, IMvxViewModel
    {
        public virtual void OnActivate()
        {
        }

        protected View GetAndInflateView(int layoutRes)
        {
            return this.BindingInflate(layoutRes, null);
        }
    }

    public abstract class BaseMvxFragment : MvxFragment
    {
        public virtual void OnActivate()
        {
        }

        protected View GetAndInflateView(int layoutRes)
        {
            return this.BindingInflate(layoutRes, null);
        }
    }
}

