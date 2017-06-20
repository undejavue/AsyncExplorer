using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using AsyncExplorer.AsyncInfrastructure;
using AsyncExplorer.Services;

namespace AsyncExplorer.ViewModels
{
    public class AppViewModel : INotifyPropertyChanged
    {
        public IAsyncCommand GetTreeNodes { get; private set; }
        public IAsyncCommand GetNodeSize { get; private set; }

        public IAsyncCommand TestAsyncCommand { get; private set; }

        public ObservableCollection<TreeModel> Nodes { get; set; }

        private StatusModel status;
        public StatusModel Status {
            get { return status; }
            set { status = value; OnPropertyChanged(nameof(Status)); }
        }

        private TreeModel selectedNode;
        public TreeModel SelectedNode
        {
            get { return selectedNode; }
            set
            {
                selectedNode = value;
                OnPropertyChanged(nameof(SelectedNode));
            }
        }

        private long sizeProgress;

        public long SizeProgress
        {
            get { return sizeProgress; }
            set { sizeProgress = value; OnPropertyChanged(nameof(SizeProgress)); }
        }

        public AppViewModel()
        {
            GetTreeNodes = AsyncCommand.Create((token, arg) => DirectoryService.GetNodeTreeAsync(arg, SelectedNode, token));
            GetNodeSize = AsyncCommand.Create((token, arg) => DirectoryService.CalcDirectorySizeAsync(arg, Status, token));

            TestAsyncCommand = AsyncCommand.Create((token, args) => DirectoryService.TestDelay(token));

            SelectedNode = new TreeModel {Name = "Root Directory", Path = @"d:/" };
            Nodes = new ObservableCollection<TreeModel>();
            Status = new StatusModel();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
