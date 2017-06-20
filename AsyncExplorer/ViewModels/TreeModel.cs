using System.Collections.ObjectModel;
using System.Linq;
using AsyncExplorer.AsyncInfrastructure;
using AsyncExplorer.Services;

namespace AsyncExplorer.ViewModels
{
    public class TreeModel : TreeEntityModel
    {
        public IAsyncCommand GetNodeChildrens { get; private set; }

        private TreeModel _parent;
        public TreeModel Parent
        {
            get { return _parent; }
            set { _parent = value; OnPropertyChanged(nameof(Parent)); }
        }

        public ObservableCollection<TreeModel> Children { get; private set; }

        private bool hasChild;
        public bool HasChild
        {
            get { return Children.Any(); }
            set { hasChild = value; }
        }

        public TreeModel()
        {
            this.Children = new ObservableCollection<TreeModel>();
            GetNodeChildrens = AsyncCommand.Create((token, arg) => DirectoryService.GetNodeTreeAsync(arg, this, token));
        }

        public TreeModel(TreeModel node) : this()
        {
            Path = node.Path;
            Parent = this;
        }

        public void AddChild(TreeModel node)
        {
            node.Parent = this;
            Children.Add(node);
        }

        public static void SetHiddenProperty(ObservableCollection<TreeModel> nodes, bool value)
        {
            foreach (var node in nodes)
            {
                node.HideSystem = node.IsHidden & !value;
                SetHiddenProperty(node.Children, value);
            }
        }
    }
}
