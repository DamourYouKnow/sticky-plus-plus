using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StickyPlusPlus.Utils.IO {
    public static class IO {
        public static void Write(string path, string contents) {
            File.WriteAllText(path, contents);
        }

        public static string Read(string path) {
            return File.ReadAllText(path);
        }
    }
}
