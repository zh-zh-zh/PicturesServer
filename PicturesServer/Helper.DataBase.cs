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

    public class DataBase
    {
        private static Dictionary<string, string> PictureList = new Dictionary<string, string>();



        /// <summary>
        /// 载入数据库
        /// </summary>
        /// <returns></returns>
        public static bool Load(string filename)
        {
            PictureList.Clear();

            try
            {
                using (StreamReader sr = new StreamReader(filename))
                {
                    string _line;
                    while ((_line = sr.ReadLine()) != null)
                    {
                        PictureList.Add(_line.Substring(0, 40), _line.Substring(40));
                    }
                }
            }
            catch
            {
                Console.WriteLine("载入数据文件 {0} 失败。", filename);
            }
            
            return true;
        }
        public static bool Save(string filename)
        {
            
            using (StreamWriter sw = new StreamWriter(filename, false))
            {
                foreach (var item in PictureList)
                {
                    sw.WriteLine(string.Format("{0}{1}", item.Key, item.Value));
                    sw.Flush();
                }
            }
            return true;
        }
        public static bool Add(Picture.Inf pic)
        {
            PictureList.Add(pic.SHA1, pic.fileName);
            return true;
        }
        public static bool Del(string hash)
        {
            return PictureList.Remove(hash);
        }
        public static bool Compared(string hash)
        {
            return PictureList.ContainsKey(hash);
        }
        public static void List()
        {
            Console.WriteLine("HASH TABLE:");
            foreach (var item in PictureList)
            {
                Console.WriteLine("{0} {1}", item.Key, item.Value);
            }
        }
    }


}