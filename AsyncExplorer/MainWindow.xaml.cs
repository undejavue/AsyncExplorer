using System.Windows;
using System.Windows.Input;
using AsyncExplorer.ViewModels;

namespace AsyncExplorer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            DataContext = new AppViewModel();
            InitializeComponent();
            treeView.SelectedItemChanged += (sender, args) => 
            { };

            treeView.MouseLeftButtonDown += TreeView_MouseLeftButtonDown;
            treeView.MouseLeftButtonUp += TreeView_MouseLeftButtonUp;

        }

        private void TreeView_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void TreeView_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void TreeView_OnSelected(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
