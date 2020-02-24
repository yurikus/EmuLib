using System;
using System.IO;
using UnityEngine;

namespace EmuLib.Utils.HTTP
{
    public static class DumpUtils
    {
        private static readonly string DumpsPath = Directory.GetCurrentDirectory();

        public static void DumpData(string type, string url, string data)
        {
            var urlUri = new Uri(url);

            var path = string.Concat(DumpsPath,
                "\\HTTP_DATA\\", urlUri.Host, "\\", 
                urlUri.LocalPath.Replace("/", "\\"),
                "\\", type, "\\"
            ).Replace("\\\\", "\\");

            if (!MakeDirs(path)) return;

            var timeStamp = DateTime.Now.ToString("dd.MM.yy__H.mm.ss");
            var filePath = string.Concat(path, timeStamp, ".json");

            File.WriteAllText(filePath, data);
        }


        private static bool MakeDirs(string path)
        {
            var result = Directory.CreateDirectory(path).Exists;
            if (!result)
                Debug.LogError($"DumpUtils DumpRequest method. make dirs result for path: {path} is false");

            return result;
        }
    }
}