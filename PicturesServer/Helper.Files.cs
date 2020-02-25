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
    public class Files
    {
        /// <summary>
        /// 移动文件
        /// </summary>
        /// <param name="srcPath">原地址</param>
        /// <param name="dstPath">目标地址</param>
        /// <param name="move">是否为剪切</param>
        /// <returns>操作结果 true 在用 FALSE 成功 </returns>
        public static bool moveFile(string srcPath, string dstPath, bool move = false)
        {
            bool result = true;
            
            while (result)
            {
                try
                {
                    if (move)
                    {
                        File.Move(srcPath, dstPath);
                    }
                    else
                    {
                        File.Copy(srcPath, dstPath);
                    }
                    result = false;
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine("操作失败");
                    Thread.Sleep(10);
                }
                finally
                {

                }
                break;
            }
            return result;
        }

        /// <summary>
        /// 判断文件是否被占用
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static bool IsFileInUse(string filename)
        {
            bool inUse = true;
            FileStream fs = null;

            try
            {
                fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.None);
                inUse = false;

            }
            catch
            {

            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                }
            }
            return inUse;
        }
    }
}
