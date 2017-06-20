using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AsyncExplorer.ViewModels
{
    public class StatusModel : INotifyPropertyChanged
    {
        private int itemsCount;
        [DisplayName("Items")]
        public int ItemsCount
        {
            get { return itemsCount; }
            set { itemsCount = value; OnPropertyChanged(nameof(ItemsCount)); }
        }

        private int filesCount;
        [DisplayName("Files")]
        public int FilesCount
        {
            get { return filesCount; }
            set { filesCount = value; OnPropertyChanged(nameof(FilesCount)); }
        }

        private int foldersCount;
        [DisplayName("Folders")]
        public int FoldersCount
        {
            get { return foldersCount; }
            set { foldersCount = value; OnPropertyChanged(nameof(FoldersCount)); }
        }

        private long size;
        [DisplayName("Size")]
        public long Size
        {
            get { return size; }
            set { size = value; OnPropertyChanged(nameof(Size)); }
        }

        public StatusModel()
        {   

        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
