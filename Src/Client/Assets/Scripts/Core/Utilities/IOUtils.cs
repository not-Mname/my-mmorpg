using System.IO;

namespace CommonUtility
{
    public class IOUtils
    {
        /// <summary>
        /// 创建txt文件的方法
        /// </summary>
        /// <param name="sFilePath"></param>
        /// <param name="content"></param>
        public static void CreatTextFile(string sFilePath, string content)
        {
            //文件存在则删除
            if (File.Exists(sFilePath))
            {
                File.Delete(sFilePath);
            }

            using (FileStream obj_versionStream = File.Create(sFilePath))
            {
                using (StreamWriter obj_writer = new StreamWriter(obj_versionStream))
                { 
                    content = content.Replace("\r", "");
                    if (content.EndsWith("\n"))
                    {
                        content = content.Substring(0, content.Length - 1);
                    }
                    obj_writer.Write(content);
                }
            }
        }

        /// <summary>
        /// 根据文件路径，创建其文件的文件夹路径，如果文件夹不存在则创建该路径。
        /// </summary>
        /// <param name="sFilePath"></param>
        public static void CreateDirectoryOfFile(string sFilePath)
        {
            if (!string.IsNullOrEmpty(sFilePath))
            {
                string sDirName = Path.GetDirectoryName(sFilePath);
                if (!Directory.Exists(sDirName))
                {
                    Directory.CreateDirectory(sDirName);
                }
            }
        }
    }
}