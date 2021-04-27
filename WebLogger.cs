using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OversimplifiedTorrent {
    public static class WebLogger {

        private static readonly object locker = new object();

        public static void Log(string text) {
            lock (locker) {
                using (StreamWriter filew = new StreamWriter(new FileStream("log", FileMode.Append))) {
                    filew.Write(text + Environment.NewLine);
                }
            }
        }
    }
}
