using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading;

namespace PicturesServer
{
    public class FileWatcher
    {
        private FileSystemWatcher _watcher = null;
        private string _path = string.Empty;
        private string _filter = string.Empty;
        private bool _isWatch = false;
        delegate void FileChangeInformation(string fullpath);
        private Picture.Pending _pending = null;


        /// <summary>
        /// 监控是否正在运行
        /// </summary>
        public bool IsWatch
        {
            get
            {
                return _isWatch;
            }
        }

        /// <summary>
        /// 初始化FileWatcher类
        /// </summary>
        /// <param name="path">监控路径</param>
        /// <param name="pending"></param>
        /// <param name="filter"></param>
        public FileWatcher(string path, Picture.Pending pending, string filter = "")
        {
            _path = path;
            _filter = filter;
            _pending = pending;

            if (!Directory.Exists(_path))
            {
                Directory.CreateDirectory(_path);
            }

            if (string.IsNullOrEmpty(_filter))
            {
                _watcher = new FileSystemWatcher(_path);
            }
            else
            {
                _watcher = new FileSystemWatcher(_path, _filter);
            }
            //注册监听事件
            _watcher.Created += new FileSystemEventHandler(OnProcess);
            _watcher.IncludeSubdirectories = true;//是否监视子目录
            _watcher.EnableRaisingEvents = true;//是否启用组件
            _isWatch = true;
        }

        /// <summary>
        /// 关闭监听器
        /// </summary>
        public void Close()
        {
            _isWatch = false;
            _watcher.Created -= new FileSystemEventHandler(OnProcess);
            _watcher.EnableRaisingEvents = false;
            _watcher = null;
        }

        /// <summary>
        /// 监听事件触发的方法
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnProcess(object sender, FileSystemEventArgs e)
        {
            switch (e.ChangeType)
            {
                case WatcherChangeTypes.Created:
                    if (File.GetAttributes(e.FullPath) != FileAttributes.Directory)//创建了文件 FileAttributes.Normal
                    {
                        _pending(e.FullPath);
                    }

                    break;
                default:
                    Console.WriteLine(e.ChangeType);
                    break;
            }
        }
    }
}