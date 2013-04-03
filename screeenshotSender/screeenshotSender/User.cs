using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;

namespace screeenshotSender
{
    public sealed class User
    {
        private Socket Usocket;

        private BinaryFormatter binFor;

        private int port;

        private string name;

        public static bool isUserActive;

        private NetManager nm;

        private Thread accepterThread;

        public User(int port, string name, NetManager manager)
        {
            this.port = port;
            this.name = name;
            binFor = new BinaryFormatter();
            this.nm = manager;
        }

        public bool initServer()
        {
            try
            {
                IPEndPoint ip = new IPEndPoint(IPAddress.Any, port);
                Usocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                Usocket.Bind(ip);
                Usocket.Listen(Int32.MaxValue);
                isUserActive = true;
                accepterThread = new Thread(this.startAccepting);
                accepterThread.Start();
                return true;
            }
            catch(Exception exc)
            {
                Console.WriteLine(exc.ToString());
                return false;
            }
        }

        private void startAccepting()
        {

            while (isUserActive)
            {
                Socket socket = Usocket.Accept();
                Friend fr = new Friend(socket, nm);
                nm.addFriend(fr);
            }
        }
        public string getName()
        {
            return name;
        }
    }
}
