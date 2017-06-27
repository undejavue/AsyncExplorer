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
            Thread.Sleep(100);
        }


        public static List<DriveInfo> GetDrives()
        {
            return FileManager.GetDrives().Where(x=>x.DriveType == DriveType.Fixed).ToList();
        }

        public static async Task GetNode(TreeModel selectedNode, CancellationToken token = new CancellationToken())
        {
            selectedNode.SetDefaults();

            var task2 = new Task(() => GetSizesDoWork(selectedNode, token));
            task2.Start();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            
            selectedNode.Children.Clear();
            var nodes = await GetNodeTree(selectedNode, token);
            foreach (var item in nodes)
            {
                selectedNode.AddChild(item);
            }

            await task2;
            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            selectedNode.Elapsed = (double)elapsedMs / 1000;
        }

        public static Task<ObservableCollection<TreeModel>> GetNodeTree(TreeModel selectedNode, CancellationToken token)
        {
            var result = Task.Run(()=>FileManager.GetNodeTree(selectedNode, token), token);
            selectedNode.IsExpanded = true;
            return result;
        }


        private static void GetSizesDoWork(TreeModel node, CancellationToken token)
        {
            long sizes = 0;
            if (node != null)
            {
                if (node.IsFile)
                {
                    sizes =  new FileInfo(node.Path).Length;
                    node.SizeStr = FileManager.GetReadableSize(sizes);
                    return;
                }

                var dirs = FileManager.GetAllDirectories(node.Path, "*.*", token, true);
                node.FoldersCount = dirs.Count;
                var files = FileManager.GetAllFiles(node.Path, "*.*", token);
                node.FilesCount = files.Count;
                node.Count = node.FilesCount + node.FoldersCount;
                node.Progress = 0;

                int i = 0;
                foreach (var file in files)
                {
                    if (token.IsCancellationRequested) break;

                    if (file.Length < 260)
                    {
                        var info = new FileInfo(file);
                        if (FileManager.IsHiddenFile(info))
                            continue;

                        sizes += info.Length;
                        node.Progress = (int)(100 / (double)files.Count * i++);
                        node.Size = sizes;
                        node.SizeStr = $"{FileManager.GetReadableSize(sizes)} (Calculating {node.Progress} %)";
                    }
                }
                node.SizeStr = FileManager.GetReadableSize(sizes);
            }
        }
    }
}
