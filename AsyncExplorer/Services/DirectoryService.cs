using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using AsyncExplorer.ViewModels;

namespace AsyncExplorer.Services
{
    public static class DirectoryService
    {

        public static async Task TestDelay(CancellationToken token)
        {
            //await Task.Delay(TimeSpan.FromSeconds(3), token).ConfigureAwait(false);
            var task = Task.Run(() => GetDelay(), token);

            await task;
        }

        private static void GetDelay()
        {
            Thread.Sleep(3000);

        }


        public static async Task<ObservableCollection<TreeModel>> GetNodeTreeAsync(object arg, TreeModel selectedNode, CancellationToken token = new CancellationToken())
        {
            var param = /*arg as TreeModel ??*/ selectedNode;

            var result =  await Task.Run(() => FileManager.GetNodeTree(param, token), token);

            selectedNode.Children.Clear();
            foreach (var item in result)
            {
                selectedNode.AddChild(item);
            }
            selectedNode.IsExpanded = true;

            return result;
        }

        public static async Task<long> CalcDirectorySizeAsync(object arg, StatusModel state, CancellationToken token = new CancellationToken())
        {
            var node = arg as TreeModel;
            return await Task.Run(() => GetSizesDoWork(node, state, token), token).ConfigureAwait(false);
        }

        private static long GetSizesDoWork(TreeModel node, StatusModel state, CancellationToken sizesToken = new CancellationToken())
        {
            long sizes = 0;
            //var state = new StatusModel();

            if (node != null)
            {
                var dirs = FileManager.GetAllDirectories(node.Path, "*.*", sizesToken, true);

                state.FoldersCount = dirs.Count;

                var files = FileManager.GetAllFiles(node.Path, "*.*", sizesToken);
                state.FilesCount = files.Count;

                state.ItemsCount = state.FilesCount + state.FoldersCount;

                int i = 0;
                foreach (var file in files)
                {
                    if (sizesToken.IsCancellationRequested) break;

                    if (file.Length < 260)
                    {
                        var info = new FileInfo(file);
                        if (FileManager.IsHiddenFile(info) && !true)
                            continue;

                        sizes += info.Length;
                        var percent = (int)(100 / (double)files.Count * i++);
                        state.Size = sizes;
                    }
                }         
            }

            return sizes;
        }
    }
}
