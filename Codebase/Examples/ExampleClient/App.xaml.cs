using System.Windows;

using Unity;

namespace ExampleClient
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public UnityContainer Container { get; set; }


        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var window = Container.Resolve<MainWindow>();
            MainWindow = window;
            window.Show();
        }
    }
}
