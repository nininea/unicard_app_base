using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Android.Content;
using Kuni.Core;
using Kuni.Core.Helpers.AppSettings;
using Kuni.Core.Helpers.Device;
using Kuni.Core.Plugins.Connectivity;
using Kuni.Core.Plugins.IUIThreadPlugin;
using Kuni.Core.Plugins.UIDialogPlugin;
using Kuni.Core.Providers.LocalDBProvider;
using Kuni.Core.Services.Abstract;
using Kuni.Core.Services.Concrete;
using Kuni.Core.UnicardApiProvider;
using Kuni.Core.Utilities.Logger;
using Kuni.Core.ViewModels;
using Kunicardus.Droid.Fragments;
using Kunicardus.Droid.Helpers.AppSettings;
using Kunicardus.Droid.Helpers.Device;
using Kunicardus.Droid.Plugins.Connectivity;
using Kunicardus.Droid.Plugins.UIDialogPlugin;
using Kunicardus.Droid.Plugins.UIThreadPlugin;
using Kunicardus.Droid.Providers.DroidSqLiteProvider;
using Kunicardus.Droid.Views;
using Microsoft.Extensions.Logging;
//using MvvmCross.Droid.Platform;
using MvvmCross;
using MvvmCross.Core;
using MvvmCross.IoC;
//using MvvmCross.Binding.Droid.Views;
using MvvmCross.Platforms.Android.Binding.Views;
using MvvmCross.Platforms.Android.Core;
using MvvmCross.Platforms.Android.Presenters;
using MvvmCross.Platforms.Android.Views;
//using MvvmCross.Platforms.Android.Core;
using MvvmCross.Plugin.File;
using MvvmCross.ViewModels;
using MvvmCross.Views;

namespace Kunicardus.Droid
{
    public class Setup : MvxAndroidSetup<App>
    {
        public static string PicturesPath
        {
            get
            {
                var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "Pics");
                Directory.CreateDirectory(path);
                return path;
            }
        }


        protected override ILoggerProvider CreateLogProvider()
        {
            return null;
        }

        protected override ILoggerFactory CreateLogFactory()
        {
            return null;
        }

        protected override IMvxApplication CreateApp(IMvxIoCProvider iocProvider)
        {
            iocProvider.RegisterType<IUIDialogPlugin, DroidUIDialogPlugin>();
            InitializeIoc(iocProvider);
            return new App();
        }

        protected override void InitializeLastChance(IMvxIoCProvider iocProvider)
        {
            base.InitializeLastChance(iocProvider);

            CreateConfigIfNotExists(iocProvider);
        }

        protected override IEnumerable<Assembly> AndroidViewAssemblies {
			get {
				var toReturn = base.AndroidViewAssemblies.ToList();
                toReturn.Add (typeof(LoginView).Assembly);
				toReturn.Add (typeof(Kunicardus.Droid.BaseTextView).Assembly);
				toReturn.Add (typeof(Kunicardus.Droid.BaseEditText).Assembly);
				toReturn.Add (typeof(Kunicardus.Droid.BaseButton).Assembly);
				toReturn.Add (typeof(Kunicardus.Droid.FocusableBaseEditText).Assembly);
				toReturn.Add (typeof(Kunicardus.Droid.BindableWebView).Assembly);
				toReturn.Add (typeof(Kunicardus.Droid.Adapters.CatalogListAdapter).Assembly);
				toReturn.Add (typeof(Kunicardus.Droid.Adapters.TransfersListViewAdapter).Assembly);
				toReturn.Add (typeof(Kunicardus.Droid.Adapters.NewsListViewAdapter).Assembly);
				toReturn.Add (typeof(MvxListView).Assembly);
				toReturn.Add (typeof(Kunicardus.Droid.BaseFBButton).Assembly);
				toReturn.Add (typeof(Kunicardus.Droid.BaseIngiriTextView).Assembly);
				toReturn.Add (typeof(Android.Support.V4.App.FragmentTabHost).Assembly);
				return toReturn;
			}
		}

        protected override IMvxAndroidViewPresenter CreateViewPresenter()
        {
            return new MvxAndroidViewPresenter(AndroidViewAssemblies);
        }

        private void CreateConfigIfNotExists (IMvxIoCProvider iocProvider)
		{
			var fileStore = iocProvider.Resolve<IMvxFileStore> ();
			var device = iocProvider.Resolve<IDevice> ();
			var bundleConfig = iocProvider.Resolve<IConfigBundlePlugin> ();
			var fullPath = fileStore.PathCombine (device.DataPath, Constants.ConfigFileName);

			fileStore.EnsureFolderExists (device.DataPath);

			if (fileStore.Exists (fullPath))
				return;


			fileStore.WriteFile (fullPath, bundleConfig.ConfigText);
		}

		protected void InitializeIoc (IMvxIoCProvider iocProvider)
		{
            //base.InitializeIoC();
            iocProvider.RegisterType<IMvxViewsContainer, ViewsContainer>();
            iocProvider.RegisterType<IDevice, DroidDevicePlugin> ();
			iocProvider.RegisterType<IConfigBundlePlugin, DroidConfigBundleProvider> ();
			iocProvider.RegisterType<IAppSettings, AppSettings> ();
			iocProvider.RegisterType<ILocalDbProvider, DroidSqLiteProvider> ();
			iocProvider.RegisterType<IConnectivityPlugin, DroidConnectivityProviderPlugin> ();
			iocProvider.RegisterType<ILoggerService, CoreLogger> ();
			iocProvider.RegisterType<IUnicardApiProvider, UnicardApiProvider> ();
			iocProvider.RegisterType<IAuthService,AuthService> ();
			iocProvider.RegisterType<IUIThreadPlugin,UIThreadPlugin> ();
			iocProvider.RegisterType<ICustomSecurityProvider,CustomSecurityProvider> ();
			iocProvider.RegisterType<IConfigReader, ConfigReader> ();
			iocProvider.RegisterType<IGoogleAnalyticsService, GoogleAnalyticsDroid> ();
            iocProvider.RegisterType<IUserService, UserService>();
            iocProvider.RegisterType<IProductsService, ProductsService>();
            iocProvider.RegisterType<INewsService, NewsService>();
        }

        protected override IDictionary<Type, Type> InitializeLookupDictionary(IMvxIoCProvider iocProvider)
        {
            //    return base.InitializeLookupDictionary(iocProvider);
            //}

            //protected override IMvxViewsContainer InitializeViewLookup(IDictionary<Type, Type> viewModelViewLookup, IMvxIoCProvider iocProvider)
            //{
            var viewModelViewLookup = new Dictionary<Type, Type>()
            {
                { typeof (LoginViewModel), typeof(LoginView) },
                { typeof (LoginAuthViewModel), typeof(LoginAuthView) },
                { typeof (BaseRegisterViewModel), typeof(BaseRegisterView) },
                { typeof (BaseResetPasswordViewModel), typeof(BaseResetPasswordView) },
                { typeof (AboutViewModel), typeof(AboutFragment) },
                { typeof (BuyProductViewModel), typeof(BuyProductFragment) },
                { typeof (BaseCatalogViewModel), typeof(BaseCatalogFragment) },
                { typeof (CatalogDetailViewModel), typeof(CatalogDetailFragment) },
                { typeof (CatalogListViewModel), typeof(CatalogListViewFragment) },
                { typeof (ChangePasswordViewModel), typeof(ChangePasswordFragment) },
                { typeof (MenuViewModel), typeof(MenuFragment) },
                { typeof (ChooseCardExistanceViewModel), typeof(ChooseCardExistanceViewFragment) },
                { typeof (EmailRegistrationViewModel), typeof(EmailRegistrationFragment) },
                { typeof (FBRegisterViewModel), typeof(FBRegisterFragment) },
                { typeof (HomePageViewModel), typeof(HomePageFragment) },
                { typeof (OrganisationListViewModel), typeof(OrganisationListFragment) },
                { typeof (PinViewModel), typeof(SettingsPinFragment) },
                { typeof (SMSVerificationViewModel), typeof(SMSVerificationFragment) },
               // { typeof (TabsViewModel), typeof(tabs) },
                { typeof (TransactionVerificationViewModel), typeof(TransactionVerificationFragment) },
                { typeof (UnicardNumberInputViewModel), typeof(UnicardInputFragment) },
                { typeof (MyPageViewModel), typeof(MyPageFragment) },
                { typeof (NewsListViewModel), typeof(NewsListFragment) },
                { typeof (ImageSliderViewModel), typeof(ImageSliderAdapter) },
                { typeof (MainViewModel), typeof(MainView) },
                { typeof (MerchantsViewModel), typeof(MerchantsView) },
                { typeof (NewsDetailsViewModel), typeof(NewsDetailsView) },
                { typeof (OrganizationDetailsViewModel), typeof(OrganizationDetailsViewModel) },
            };
            var container = iocProvider.Resolve<IMvxViewsContainer>();
            container.AddAll(viewModelViewLookup);

            return viewModelViewLookup;
        }

    }
}