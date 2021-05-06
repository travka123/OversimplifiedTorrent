using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace OversimplifiedTorrent {
    public static class PeersConnectionsReciver {

        private static TcpListener listenSocket;

        public static int Port { get; private set; }

        public static bool StartListen() {
            bool open = false;
            Port = 6881;
            while ((!open) && (Port <= 6889)) {
                try {
                    listenSocket = new TcpListener(IPAddress.Any, Port);
                    listenSocket.Start();
                    Task task = new Task(() => ListenLoop());
                    task.Start();
                    open = true;
                }
                catch {
                    Port++;
                }
            }
            return open;
        }

        private static void ListenLoop() {
            while (true) {
                try {
                    TcpClient tcpClient = listenSocket.AcceptTcpClient();
                    PeersConectionsRouter.HandleIncomingConnection(tcpClient);
                }
                catch {
                    
                }
            }
        }

        public static void StopListen() {
            listenSocket.Stop();
        }
    }
}
