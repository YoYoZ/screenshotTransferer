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

        private string fName = "NOT SET YET";

        private int numberOfFailedPackets;

        private string status = "NOT UPDATED";

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
            numberOfFailedPackets = 0;
            isFriendAlive = true;
            packetReceiver = new Thread(this.readPackets);
            packetReceiver.Name = "Packet Receiver for " + getLocalAddress();
            packetReceiver.Start();
            this.nm = manager;
        }

        public string getLocalAddress()
        {
            return address;
        }

        public string getRemoteIP()
        {
            return fSocket.RemoteEndPoint.ToString().Split(':')[0];
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
                    numberOfFailedPackets = 0;
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
                                nm.getScreenShower().changeIcon(ScreenShower.StateofIcon.sending);
                                binFor.Serialize(netStream, p);
                                nm.getScreenShower().changeIcon(ScreenShower.StateofIcon.normal);
                            }
                            catch (Exception exc)
                            {
                                StreamError();
                                Console.WriteLine("Error: " + exc.ToString());
                            }
                        }
                    });
                th.Name = "Thread Packet sender for " + getFriendName();
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
                    nm.getScreenShower().changeIcon(ScreenShower.StateofIcon.receiving);
                    p.setOwner(this);
                    p.performPacket(nm);
                    nm.getScreenShower().changeIcon(ScreenShower.StateofIcon.normal);
                }
                catch (Exception exc)
                {
                    Console.WriteLine("Error: " + exc.ToString());
                }
            }
        }

        private void StreamError()
        {
            ++numberOfFailedPackets;
            if (numberOfFailedPackets >= 5)
                nm.connectionError(this);

        }

        public void setStatus(string status)
        {
            this.status = "";
            this.status = fName + " | " + status;
        }

        public string getStatus()
        {
            return status;
        }

        public void UpdateStatus(long ping)
        {
            this.status = "";
            this.status = fName + " | ";
            this.status += fSocket.RemoteEndPoint.ToString() + " | ";
            this.status += ping + " ms | ";
            this.status += "Активен";
        }

        public bool isAlive()
        {
            return isFriendAlive;
        }
    }
}
