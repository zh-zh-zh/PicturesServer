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
    public class Compute
    {
        /// <summary>
        /// 快速获取文件信息
        /// </summary>
        /// <param name="pic"></param>
        public static Picture.Inf FastCompute(string filename)
        {
            Picture.Inf result = new Picture.Inf();

            if (System.IO.File.Exists(filename))
            {
                result.fileName = filename;
                result.FileInfo = new FileInfo(result.fileName);
                //result.FileInfo.Extension = "jpg"
                result.isPhoto = true;
                try
                {
                    using (FileStream stream = new FileStream(filename, FileMode.Open, FileAccess.Read))
                    {
                        result.SHA1 = Byte2String(SHA1(stream));
                        string date = GetExifDate(stream);
                        Console.WriteLine("EXIF:{1}{0}", filename, date);
                        if (date == "-1")//不是图片
                        {
                            result.isPhoto = false;
                        }
                        else
                        {
                            DateTime.TryParse(date, out result.ExifDate);
                        }

                    }
                }
                catch
                {

                }
                
            }

            return result;
        }

        /// <summary>
        /// 获取Exif中的照片拍摄日期
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <returns>拍摄日期</returns>
        public static string GetExifDate(FileStream stream)
        {
            Encoding ascii = Encoding.ASCII;
            string picDate;
            try
            {
                //FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                Image image = Image.FromStream(stream, true, false);

                foreach (PropertyItem p in image.PropertyItems)
                {
                    //获取拍摄日期时间
                    if (p.Id == 0x9003) // 0x0132 最后更新时间
                    {
                        //stream.Close();

                        picDate = ascii.GetString(p.Value);
                        if ((!"".Equals(picDate)) && picDate.Length >= 10)
                        {
                            // 拍摄日期
                            picDate = picDate.Substring(0, 10).Replace(":", "-") + picDate.Substring(10);
                            //picDate = picDate.Replace(":", "-");
                            return picDate;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return "-1";//可能不是图片
            }

            //stream.Close();
            return "";
        }

        /// <summary>
        ///  计算指定文件的MD5值
        /// </summary>
        /// <param name="fs">文件流</param>
        /// <returns>返回值的字符串形式</returns>
        public static Byte[] MD5(FileStream fs)
        {
            String hashMD5 = String.Empty;
            //计算文件的MD5值
            System.Security.Cryptography.MD5 calculator = System.Security.Cryptography.MD5.Create();
            Byte[] buffer = calculator.ComputeHash(fs);
            calculator.Clear();
            return buffer;


        }//ComputeMD5
         /// <summary>
         ///  计算指定文件的CRC32值
         /// </summary>
         /// <param name="fs">文件流</param>
         /// <returns>返回值的字符串形式</returns>
        public static Byte[] CRC32(FileStream fs)
        {
            String hashCRC32 = String.Empty;

            //计算文件的CSC32值
            Crc32 calculator = new Crc32();
            Byte[] buffer = calculator.ComputeHash(fs);
            calculator.Clear();
            return buffer;
        }//ComputeCRC32
         /// <summary>
         ///  计算指定文件的SHA1值
         /// </summary>
         /// <param name="fs">文件流</param>
         /// <returns>返回值的字符串形式</returns>
        public static Byte[] SHA1(FileStream fs)
        {
            String hashSHA1 = String.Empty;
            //检查文件是否存在，如果文件存在则进行计算，否则返回空值

            //计算文件的SHA1值
            System.Security.Cryptography.SHA1 calculator = System.Security.Cryptography.SHA1.Create();
            Byte[] buffer = calculator.ComputeHash(fs);
            calculator.Clear();
            return buffer;
        }
        public static Byte[] XOR(FileStream fs)
        {
            byte[] buf = FS2Byte(fs);

            var r = buf.Aggregate(0, (a, b) => b ^ a);
            Console.WriteLine("0x{0:X2}", r);

            return BitConverter.GetBytes(r);




            int a1 = 0;

            for (int i = 0; i < buf.Length - 1; i++)
            {
                a1 ^= buf[i];
            }
            return BitConverter.GetBytes(a1);

        }

        /// <summary>
        /// 字符转换
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static String Byte2String(Byte[] buffer)
        {
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < buffer.Length; i++)
            {
                stringBuilder.Append(buffer[i].ToString("x2"));
            }
            return stringBuilder.ToString();
        }

        /// <summary>
        /// 文件流转BYTES
        /// </summary>
        /// <param name="fs"></param>
        /// <returns></returns>
        public static Byte[] FS2Byte(FileStream fs)
        {
            byte[] buf = new byte[fs.Length];
            fs.Read(buf, 0, buf.Length);
            fs.Seek(0, SeekOrigin.Begin);
            return buf;
        }
    }
}
