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
    public class Picture
    {


        public delegate void Pending(string filename);
        public struct Inf
        {
            public string fileName;
            public string MD5;
            public string SHA1;
            public string CRC32;
            public bool isPhoto;
            public DateTime ExifDate;
            public FileInfo FileInfo;
        }

        private static Pending _pending = null;


        public static bool init()
        {
            
            return true;
        }

        public static bool setMonitorFile(string MonitorPath, Pending pending)
        {
            _pending = pending;
            return true;
        }







    }
}