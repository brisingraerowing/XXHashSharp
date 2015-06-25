using System;
using System.IO.Compression;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace XXHashSharp
{
    internal static class ModuleInitializer
    {
        public static void Initialize()
        {
            ExtractAssembly(Environment.Is64BitProcess);
        }

        private static readonly string OutputPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "GryphonSoft Technologies", "XXHashSharp");

        [DllImport("Kernel32.dll")]
        private static extern IntPtr LoadLibrary([MarshalAs(UnmanagedType.LPStr)]string lpFileName);

        private static void ExtractAssembly(bool is64bit)
        {
            var p = Path.Combine(OutputPath, is64bit ? "x64" : "x86", "xxhash.dll");
            if(!Directory.Exists(Path.GetDirectoryName(p)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(p));
            }
            if (!File.Exists(p))
            {
                string resourceName = string.Format("XXHashSharp.{0}.xxhash.dll.gz", is64bit ? "x64" : "x86");

                using (var s = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
                {
                    using (var gz = new GZipStream(s, CompressionMode.Decompress))
                    {
                        using (var fs = new FileStream(p, FileMode.Create, FileAccess.Write))
                        {
                            gz.CopyTo(fs);
                            fs.Flush(true);
                        }
                    }
                }
            }
            LoadLibrary(p);
        }

    }
}
