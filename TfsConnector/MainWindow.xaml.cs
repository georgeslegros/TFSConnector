using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace TfsConnector
{
    public partial class MainWindow
    {
        private readonly MainWindowViewModel viewModel = new MainWindowViewModel();

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindowLoaded;
        }

        void MainWindowLoaded(object sender, RoutedEventArgs e)
        {
            DataContext = viewModel;
            viewModel.Init();
        }

        private void FilterTextChanged(object sender, TextChangedEventArgs e)
        {
            Filter();
        }

        private void Filter()
        {
            var source = CollectionViewSource.GetDefaultView(dependenciesList.ItemsSource);
            if (source != null) source.Filter = FilterDependencies;
        }

        private bool FilterDependencies(object obj)
        {
            var dep = obj as Dependency;
            return dep == null || dep.Id.ToLowerInvariant().Contains(depFilter.Text.ToLowerInvariant());
        }


    }
}
