using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Juego2.H2F03.Evidence
{
    /// <summary>Results location (<c>-h2f03-results</c>), JSON output, hashing and batch exit for the H2F-03 evidence code.</summary>
    public static class H2F03Io
    {
        public static string ProjectRoot => Path.GetFullPath(Path.Combine(Application.dataPath, ".."));

        public static string Results
        {
            get
            {
                var args = Environment.GetCommandLineArgs();
                var i = Array.IndexOf(args, "-h2f03-results");
                var dir = i >= 0 && i + 1 < args.Length ? args[i + 1] : Path.Combine(ProjectRoot, "../../results");
                Directory.CreateDirectory(dir);
                return Path.GetFullPath(dir);
            }
        }

        public static string Arg(string name)
        {
            var args = Environment.GetCommandLineArgs();
            var i = Array.IndexOf(args, name);
            return i >= 0 && i + 1 < args.Length ? args[i + 1] : null;
        }

        public static void WriteJson(string name, object value) => WriteText(name, JsonUtility.ToJson(value, true) + "\n");

        public static void WriteText(string name, string text)
        {
            var path = Path.Combine(Results, name);
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            File.WriteAllText(path, text, new UTF8Encoding(false));
        }

        public static string Sha(string text)
        {
            using (var sha = SHA256.Create())
                return string.Concat(sha.ComputeHash(Encoding.UTF8.GetBytes(text)).Select(b => b.ToString("x2")));
        }

        public static void Exit(int code)
        {
            if (Application.isBatchMode) EditorApplication.Exit(code);
        }
    }
}
