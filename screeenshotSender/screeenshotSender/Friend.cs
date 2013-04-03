using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;

namespace screeenshotSender
{
    public sealed class Friend
    {

        private string address;

        private int port;

        private BinaryFormatter binFor;

        private Socket fSocket;

        private NetworkStream netStream;

        private bool isFriendAlive;

        private Thread packetReceiver;

        private object locker = new object();

        private NetManager nm;

        private string fName;

        public Friend(string address, int port, NetManager manager)
        {
            this.address = address;
            this.port = port;
            binFor = new BinaryFormatter();
            this.nm = manager;
        }

        public Friend(Socket socket, NetManager manager)
        {
            this.fSocket = socket;
            netStream = new NetworkStream(fSocket);
            binFor = new BinaryFormatter();
            isFriendAlive = true;
            packetReceiver = new Thread(this.readPackets);
            packetReceiver.Start();
            this.nm = manager;
        }

        public bool connectToFriend()
        {
            try
            {
                IPAddress ip = IPAddress.Parse(address);
                IPEndPoint remoteIPEndPoint = new IPEndPoint(ip, port);
                fSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                fSocket.Connect(remoteIPEndPoint);
                if (fSocket.Connected)
                {
                    netStream = new NetworkStream(fSocket);
                    isFriendAlive = true;
                    packetReceiver = new Thread(this.readPackets);
                    packetReceiver.Start();
                    return true;
                }
                else
                    return false;
            }
            catch (Exception exc)
            {
                Console.WriteLine("Error: " + exc.ToString());
                return false;
            }
        }

        public void killFriend()
        {
            try
            {
                isFriendAlive = false;
                netStream.Close();
                fSocket.Shutdown(SocketShutdown.Both);
            }
            catch { }
        }

        public void sendPacket(Packet p)
        {
            if (isFriendAlive)
            {
                Thread th = new Thread(delegate()
                    {
                        lock (locker)
                        {
                            try
                            {
                                binFor.Serialize(netStream, p);
                            }
                            catch (Exception exc)
                            {
                                Console.WriteLine("Error: " + exc.ToString());
                            }
                        }
                    });
                th.Start();
            }
        }

        public void setFriendName(string name)
        {
            this.fName = name;
        }

        public string getFriendName()
        {
            return fName;
        }

        private void readPackets()
        {
            while (isFriendAlive)
            {
                try
                {
                    Packet p = (Packet)binFor.Deserialize(netStream);
                    p.setOwner(this);
                    p.performPacket(nm);
                }
                catch (Exception exc)
                {
                    Console.WriteLine("Error: " + exc.ToString());
                }
            }
        }
    }
}
