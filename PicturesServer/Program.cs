using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Xml;

namespace PicturesServer
{
    class Program
    {
        public static string srcPath;//监视的目录
        public static string decPath;//新文件保存目录
        public static string databaseFile;//数据库文件

        public static bool deleteFile;//是否为剪切
        public static bool monitorFile;//是否监视目录

        public delegate bool ControlCtrlDelegate(int CtrlType);
        [DllImport("kernel32.dll")]
        private static extern bool SetConsoleCtrlHandler(ControlCtrlDelegate HandlerRoutine, bool Add);
        private static ControlCtrlDelegate cancelHandler = new ControlCtrlDelegate(HandlerRoutine);

        static void Main(string[] args)
        {

            SetConsoleCtrlHandler(cancelHandler, true);

            loadConfig();
            DataBase.Load(databaseFile);

            if (monitorFile)//是否监视文件夹
            {
                Picture.Pending pending = new Picture.Pending(OnChange);
                FileWatcher fw = new FileWatcher(srcPath, pending);//监视文件夹
            }



            //ImportFile(srcPath);

            //while (Console.Read() != 'q')
            while(true)
            {
                string[] cmd = Console.ReadLine().Split(' ');
                switch (cmd[0].ToLower())
                {
                    case "hash":
                        DataBase.List();
                        break;
                    case "import":
                        ImportFile(cmd[1]);
                        break;
                    default:
                        Console.WriteLine("hash");
                        Console.WriteLine("import");
                        break;
                }


            };

        }

        /// <summary>
        /// 控制台退出事件
        /// </summary>
        /// <param name="CtrlType"></param>
        /// <returns></returns>
        public static bool HandlerRoutine(int CtrlType)
        {
            //SqlDependency.Stop(DataConnection);
            Console.WriteLine("正在保存数据");
            DataBase.Save(databaseFile);
            Console.WriteLine("BYE");
            return false;
        }

        public static void OnChange(string filename)
        {
            while (true)
            {
                if (!Files.IsFileInUse(filename))
                {
                    Console.WriteLine("{0}->文件占用释放", filename);

                    Picture.Inf pic = Compute.FastCompute(filename);
                    if (!pic.isPhoto)//不是图片就退出
                    {
                        Console.WriteLine("不是图片");
                        if (deleteFile)
                        {
                            File.Delete(pic.fileName);
                            Console.WriteLine("删除文件");
                        }
                        return;
                    }

                    //Console.WriteLine("{0} {1} {2}", pic.ExifDate.ToString(), pic.SHA1, pic.FileInfo.LastWriteTime);
                    //数据库操作(查重)
                    Console.WriteLine("图片查重");
                    if (DataBase.Compared(pic.SHA1))//重复文件
                    {
                        Console.WriteLine("重复文件");
                        if (deleteFile)
                            File.Delete(pic.fileName);
                        break;
                    }
                    Console.WriteLine("加入数据库");
                    DataBase.Add(pic);
                    string tmpPath;

                    if (pic.ExifDate.Year == 1)//非法日期
                    {
                        tmpPath = pic.FileInfo.LastWriteTime.ToString("yyyy-MM-dd");
                    }
                    else
                    {
                        Console.WriteLine("不是图片");
                        tmpPath = pic.ExifDate.ToString("yyyy-MM-dd");
                    }

                    tmpPath = string.Format("{0}{1}\\", decPath, tmpPath);

                    if (!Directory.Exists(tmpPath))//没有目录就创建目录
                        Directory.CreateDirectory(tmpPath);

                    //尝试存储图片
                    string filePath = string.Format("{0}{1}", tmpPath, pic.FileInfo.Name);

                    Console.WriteLine("新文件{0}", filePath);

                    int i = 1;
                    if (File.Exists(filePath))
                    {
                        bool ifExists = true;
                        while (ifExists)
                        {
                            filePath = string.Format("{0}{1}({2}){3}",
                                tmpPath,
                                pic.FileInfo.Name.Substring(0, pic.FileInfo.Name.Length - pic.FileInfo.Extension.Length),
                                i,
                                pic.FileInfo.Extension
                                );
                            i++;
                            Console.WriteLine("尝试新文件{0}", filePath);
                            ifExists = File.Exists(filePath);
                        }
                    }
 
                    bool movefile = Files.moveFile(pic.fileName, filePath, deleteFile);
                    
                    Console.WriteLine("{0}->{1}->{2}", pic.fileName, filePath, movefile ? "失败" : "成功");

                    break;
                }

            }
        }
        /// <summary>
        /// 导入图片
        /// </summary>
        /// <param name="path">导入路径</param>
        public static void ImportFile(string path)
        {
            Console.WriteLine("导入命令 {0}", path);
            var files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                OnChange(file);
            }
        }


        public static void loadConfig()
        {
            try
            {
                XmlDocument xmldoc = new XmlDocument();
                xmldoc.Load(@"config.xml");

                XmlElement root = xmldoc.DocumentElement;


                srcPath = root.SelectSingleNode("srcPath").InnerText;//监视的目录
                decPath = root.SelectSingleNode("decPath").InnerText;//新文件保存目录
                databaseFile = root.SelectSingleNode("databaseFile").InnerText;//数据库文件

                deleteFile = bool.Parse(root.SelectSingleNode("deleteFile").InnerText);//是否允许删除文件，移动时为剪切 重复时为删除
                monitorFile = bool.Parse(root.SelectSingleNode("monitorFile").InnerText);//是否监视目录

            }
            catch (Exception ex)
            {

            }

        }
    }
}
