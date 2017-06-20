using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AsyncExplorer.ViewModels;

namespace AsyncExplorer.Services
{
    public class FileManager
    {
        private const int filePathMaxLen = 260;
        private const int dirNameMaxLen = 248;

        /// <summary>
        /// Get Node directories and files tree by node directory path
        /// </summary>
        /// <param name="parent">Parent tree node</param>
        /// <param name="token">Cancellation Token for break operation</param>
        /// <returns></returns>
        public static ObservableCollection<TreeModel> GetNodeTree(TreeModel parent, CancellationToken token)
        {
            var result = new ObservableCollection<TreeModel>();
            DirectoryInfo[] rootLevel = { };
            try
            {
                rootLevel = new DirectoryInfo(parent.Path).GetDirectories();
            }
            catch (Exception ex)
            {
                parent.IsDenied = true;
            }

            foreach (var item in rootLevel)
            {
                if (token.IsCancellationRequested)
                    break;

                bool isHidden = IsHiddenDirectory(item);
                var node = new TreeModel { Path = item.FullName, Name = item.Name, IsFile = false, IsHidden = isHidden };
                DirectoryInfo[] innerLevel = { };
                try
                {
                    innerLevel = item.GetDirectories();
                }
                catch (Exception ex)
                {
                    node.IsDenied = true;
                }

                foreach (var inner in innerLevel)
                {
                    if (token.IsCancellationRequested)
                        break;

                    isHidden = IsHiddenDirectory(inner);
                    node.AddChild(new TreeModel { Path = inner.FullName, Name = inner.Name, IsFile = false, IsHidden = isHidden });
                }

                var innerFiles = GetFiles(item.FullName);
                foreach (var file in innerFiles)
                {
                    if (token.IsCancellationRequested)
                        break;

                    isHidden = IsHiddenFile(file);
                    node.AddChild(new TreeModel { Path = file.FullName, Name = file.Name, IsFile = true, IsHidden = isHidden });
                }

                result.Add(node);
            }

            var files = GetFiles(parent.Path);
            foreach (var file in files)
            {
                if (token.IsCancellationRequested)
                    break;

                var isHidden = IsHiddenFile(file);
                result.Add(new TreeModel { Path = file.FullName, Name = file.Name, IsFile = true, IsHidden = isHidden });
            }

            //parent.Children.Clear();

            //foreach (var r in result)
            //{
            //    parent.AddChild(r);    
            //}

            return result;
        }

        /// <summary>
        /// Recursive enumeration of all inner directories
        /// </summary>
        /// <param name="path">Parent directory path</param>
        /// <param name="pattern">Search pattern</param>
        /// <param name="token">Cancellation token</param>
        /// <param name="withHiddens">Include hidden & system directories</param>
        /// <param name="deepLevel">Level of recursion</param>
        /// <returns></returns>
        public static List<string> GetAllDirectories(string path, string pattern, CancellationToken token, bool withHiddens, int deepLevel = 0)
        {
            var result = new List<string>();
            //if (deepLevel > 0)
            try
            {
                if (path.Length < dirNameMaxLen)
                {
                    result.AddRange(Directory.GetDirectories(path, pattern, SearchOption.TopDirectoryOnly));
                    foreach (var directory in Directory.GetDirectories(path))
                    {
                        if (token.IsCancellationRequested)
                        {
                            break;
                        }

                        if (withHiddens)
                        {
                            result.AddRange(GetAllDirectories(directory, pattern, token, true, deepLevel - 1));
                        }
                        else if (!IsHiddenDirectory(new DirectoryInfo(directory)))
                        {
                            result.AddRange(GetAllDirectories(directory, pattern, token, true, deepLevel - 1));
                        }
                    }
                }
            }
            catch (UnauthorizedAccessException)
            {
                //Ignore, skip current dir and continue
            }
            return result;
        }


        /// <summary>
        /// Recursively get all files paths for directory
        /// </summary>
        /// <param name="path">Directory path</param>
        /// <param name="pattern">Search patterns</param>
        /// <param name="token">Cancellation token</param>
        /// <returns>List of files full names</returns>
        public static List<string> GetAllFiles(string path, string pattern, CancellationToken token)
        {
            var files = new List<string>();
            try
            {
                if (path.Length <= dirNameMaxLen)
                {
                    files.AddRange(Directory.GetFiles(path, pattern, SearchOption.TopDirectoryOnly));
                    foreach (var directory in Directory.GetDirectories(path))
                    {
                        if (token.IsCancellationRequested)
                        {
                            break;
                        }
                        files.AddRange(GetAllFiles(directory, pattern, token));
                    }
                }
            }
            catch (UnauthorizedAccessException)
            {
                //Ignore, skip current dir and continue
            }
            return files;
        }


        private static FileInfo[] GetFiles(string dir)
        {
            FileInfo[] result = new FileInfo[] { };
            try
            {
                DirectoryInfo dInfo = new DirectoryInfo(dir);
                result = dInfo.GetFiles("*.*", SearchOption.TopDirectoryOnly);

            }
            catch (UnauthorizedAccessException UAEx)
            {
                //Ignore, skip current dir and continue
            }
            catch (PathTooLongException PathEx)
            {
                //Ignore, skip current dir and continue
            }

            return result;
        }


        /// <summary>
        /// Check directory for Hidden or System attribute
        /// </summary>
        private static bool IsHiddenDirectory(DirectoryInfo dirInfo)
        {
            if (dirInfo == null) return false;
            if (dirInfo.Attributes.HasFlag(FileAttributes.Hidden) || dirInfo.Attributes.HasFlag(FileAttributes.System))
                return true;
            return false;
        }


        /// <summary>
        /// Check file for Hidden or System attribute
        /// </summary>
        public static bool IsHiddenFile(FileInfo fi)
        {
            if (fi.Attributes.HasFlag(FileAttributes.Hidden) || fi.Attributes.HasFlag(FileAttributes.System))
                return true;
            // If you're worried about parent directories hidden:
            return IsHiddenDirectory(fi.Directory);
            // otherwise:
            return false;
        }

        public static FileInfo GetFileInfo(string path)
        {
            var fileInfo = new FileInfo(path);
            return fileInfo;
        }

        public static List<string> GetDrives()
        {
            return Directory.GetLogicalDrives().ToList();
        }
    }
}
