using System;
using System.Windows;

namespace ExampleClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private ViewModel viewModel;
        

        public MainWindow(ViewModel viewModel)
        {
            DataContext = this.viewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));

            InitializeComponent();
        }


        private async void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            await viewModel.Connect();
        }


        private async void InputButton_Click(object sender, RoutedEventArgs e)
        {
            await viewModel.PostMessage();
        }
    }
}
