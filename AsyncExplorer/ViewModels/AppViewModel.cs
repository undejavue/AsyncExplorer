using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using AsyncExplorer.AsyncInfrastructure;
using AsyncExplorer.Services;

namespace AsyncExplorer.ViewModels
{
    public class AppViewModel : INotifyPropertyChanged
    {
        public RelayCommand SetDrive { get; private set; }

        public ObservableCollection<DriveInfo> Drives { get; set; }

        private DriveInfo _selectedDrive;
        public DriveInfo SelectedDrive
        {
            get { return _selectedDrive; }
            set { _selectedDrive = value; OnPropertyChanged(nameof(SelectedDrive)); }
        }


        public ObservableCollection<TreeModel> Children { get; set; }

        private TreeModel _selectedItem;
        public TreeModel SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                OnPropertyChanged(nameof(SelectedItem));
            }
        }

        public AppViewModel()
        {
            Drives = new ObservableCollection<DriveInfo>(DirectoryService.GetDrives());
            SelectedDrive = Drives[0];
            SelectedItem = new TreeModel {Name = SelectedDrive.Name, Path = SelectedDrive.RootDirectory.FullName};
            Children = DirectoryService.GetNodeTree(SelectedItem, new CancellationToken()).Result;

            SetDrive = new RelayCommand(obj =>
            {
                SelectedItem  = new TreeModel { Name = SelectedDrive.Name, Path = SelectedDrive.RootDirectory.FullName };
                Children.Clear();
                foreach (var node in DirectoryService.GetNodeTree(SelectedItem, new CancellationToken()).Result)
                {
                    Children.Add(node);
                }
            });
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
