using System.IO;
using System.IO.Compression;

namespace Live2Dmodel
{
    public static class ModelDataConverter
    {
        /// <summary>
        /// This method converts the data to zip file and returns the path of the zip file.
        /// </summary>
        /// <param name="path"></param>
        /// <returns>zip file path : string</returns>
        public static string ConvertToZip(string path)
        {
            var parentDirectory = Directory.GetParent(path);
            if (parentDirectory is null) return string.Empty;
            var dirPath = UnityEngine.Application.persistentDataPath + "/models";
            var zipPath = dirPath + "/myModel.zip";
            if(!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);
            if(File.Exists(zipPath)) File.Delete(zipPath);
            ZipFile.CreateFromDirectory(parentDirectory.FullName, zipPath);
            return zipPath;
        }
    }
}
