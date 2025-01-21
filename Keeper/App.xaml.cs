using Autofac;
using Data.Abstractions;
using Data.SimpleEncryptor;
using DataProvider.Abstractions;
using DataProvider.Yandex;
using Keeper.Models;
using Keeper.ViewModels;
using Keeper.Views;
using System.Globalization;
using System.Windows;

namespace Keeper
{
    public partial class App : Application
    {
        public static CultureInfo[] Languages { get; private set; }

        internal static IContainer Container { get; private set; } = null;
        internal static readonly string IconsDirecory = "/Keeper;component/Icons";

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var containerBuilder = new ContainerBuilder();

            var settings = Settings.Get();

            containerBuilder.RegisterType<SettingsViewModel>().SingleInstance();
            containerBuilder.RegisterType<MainPageViewModel>();
            containerBuilder.RegisterType<DataViewModel>().SingleInstance();
            containerBuilder.RegisterType<MainPage>();
            containerBuilder.RegisterType<ItemView>();
            containerBuilder.RegisterType<ApplicationLanguageManager>().As<IApplicationLanguageManager>().SingleInstance();
            containerBuilder.Register(r => settings).As<Settings>().SingleInstance();
            containerBuilder.Register(r=> new DataAdapter(settings.DataFileName))
                .As<IDataAdapter>().SingleInstance();
            containerBuilder.Register(r => new SimpleEncryptor(settings.Signature)).As<IEncryptor>()
                .SingleInstance();

            containerBuilder.RegisterType<SettingsView>();

            Container = containerBuilder.Build();

            if (!string.IsNullOrEmpty(settings.CurrentLanguageName))
            {
                Container.Resolve<IApplicationLanguageManager>().SetLanguage(settings.CurrentLanguageName);
            }

            Container.Resolve<MainPage>().Show();
        }
    }
}