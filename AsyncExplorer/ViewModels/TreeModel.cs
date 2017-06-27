using System.Collections.ObjectModel;
using System.Linq;
using AsyncExplorer.AsyncInfrastructure;
using AsyncExplorer.Services;

namespace AsyncExplorer.ViewModels
{
    public class TreeModel : TreeEntityModel
    {
        public IAsyncCommand GetNode { get; private set; }

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
            GetNode = AsyncCommand.Create((token, arg) => DirectoryService.GetNode(this, token));
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

        public int GetChildrenCount(TreeModel node, ref int i)
        {
            i += node.Children.Count;

            foreach (var child in Children)
                i = i + GetChildrenCount(child, ref i);

            return i;
        }

        public void SetDefaults()
        {
            FilesCount = 0;
            FoldersCount = 0;
            Count = 0;
            SizeStr = "Calculating...";
            Size = 0;
            Progress = 0;
        }
    }
}
