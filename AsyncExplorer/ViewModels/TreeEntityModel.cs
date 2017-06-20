using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AsyncExplorer.ViewModels
{
    public class TreeEntityModel : INotifyPropertyChanged
    {
        private string name;
        public string Name
        {
            get { return name; }
            set { name = value; OnPropertyChanged(nameof(Name)); }
        }

        private string path;
        public string Path
        {
            get { return path; }
            set { path = value; OnPropertyChanged(nameof(Path)); }
        }

        private long size;
        public long Size
        {
            get { return size; }
            set { size = value; OnPropertyChanged(nameof(Size)); }
        }

        private bool isFile;
        public bool IsFile
        {
            get { return isFile; }
            set { isFile = value; OnPropertyChanged(nameof(IsFile)); }
        }

        private bool isDenied;
        public bool IsDenied
        {
            get { return isDenied; }
            set { isDenied = value; OnPropertyChanged(nameof(IsDenied)); }
        }

        private bool isHidden;
        public bool IsHidden
        {
            get { return isHidden; }
            set { isHidden = value; OnPropertyChanged(nameof(IsHidden)); }
        }

        private bool isExpanded;
        public bool IsExpanded
        {
            get { return isExpanded; }
            set { isExpanded = value; OnPropertyChanged(nameof(IsExpanded)); }
        }

        private bool _hideSystem;
        public bool HideSystem
        {
            get { return _hideSystem; }
            set { _hideSystem = value; OnPropertyChanged(nameof(HideSystem)); }
        }

        public TreeEntityModel() { }


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
