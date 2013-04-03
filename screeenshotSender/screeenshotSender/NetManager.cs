using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading;
using System.IO;
using System.Windows.Forms;

namespace screeenshotSender
{
    public sealed class NetManager
    {
        private static List<Friend> friends;

        private object blocker;

        private User user;

        public static bool isManagerAlive;

        public NetManager()
        {
            friends = new List<Friend>();
            blocker = new object();
        }

        public Friend[] getFriends()
        {
            return friends.ToArray();
        }

        public void sendScreen(Image image)
        {
                Thread th = new Thread(delegate()
                    {
                        foreach (Friend fr in friends)
                        {
                            Packet p = new Packet002Screenshot(image);
                            fr.sendPacket(p);
                        }
                    });
                th.Start();
        }

        /// <summary>
        /// Start server side.
        /// </summary>
        /// <param name="port"></param>
        /// <param name="username"></param>
        /// <returns>False if Server initialization failed</returns>
        public bool initServer(int port, string username)
        {
            user = new User(port, username, this);
            bool result = user.initServer();
            isManagerAlive = result;
            return result;
        }

        public void addFriend(Friend fr)
        {
            friends.Add(fr);
        }

        public void friendNameExchanger(Packet001FriendName p)
        {
            lock (blocker)
            {
                if (p.direction)
                {
                    p.getSender().setFriendName(p.name);
                    p.direction = false;
                    p.name = user.getName();
                    p.getSender().sendPacket(p);
                }
                else
                {
                    p.getSender().setFriendName(p.name);
                }
            }
        }
        public void Screenshotshot(Packet002Screenshot p)
        {
            lock (blocker)
            {
                DialogResult result = MessageBox.Show(null, "Вам скриншот от " + p.getSender().getFriendName(), "Ура! Новый скриншот в мою коллекцию", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    MemoryStream ms = new MemoryStream(p.bytes);
                    Image im = Image.FromStream(ms);
                    Thread guiThread = new Thread(delegate()
                        {
                            Screenshot screenShot = new Screenshot(im, p.getSender().getFriendName());
                            Application.Run(screenShot);
                        });
                    guiThread.Start();
                }
            }
        }
    }
}
