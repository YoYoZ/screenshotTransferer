using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.Serialization.Formatters.Binary;
using System.Net;

namespace screeenshotSender
{
    public sealed class NetManager
    {
        private static List<Friend> friends;

        private object blocker;

        private User user;

        public static bool isManagerAlive;

        private Thread threadUpdater;

        private Packet002Screenshot lastScreenShot;

        private ScreenShower sShower;

        public NetManager(ScreenShower screenShower)
        {
            friends = new List<Friend>();
            blocker = new object();
            sShower = screenShower;
        }

        public ScreenShower getScreenShower()
        {
            return sShower;
        }

        public bool isLocalIP(string address)
        {
            try
            {
                IPAddress[] ipArray = Dns.GetHostAddresses("localhost");
                foreach (IPAddress ip in ipArray)
                {
                    if (ip.ToString().Equals(address))
                        return true;
                }
                return false;
            }
            catch(Exception exc)
            {
                ScreenShower.log.writeParamToLog("Error during resolving local ip's " + exc.Message);
                return false;
            }
        }

        public void showLastScreenshot()
        {
            if (lastScreenShot != null)
            {
                Thread guiThread = new Thread(delegate()
                {
                    MemoryStream ms = new MemoryStream(lastScreenShot.bytes);
                    Image im = Image.FromStream(ms);
                    Screenshot screenShot = new Screenshot(im, lastScreenShot.getSender().getFriendName());
                    Application.Run(screenShot);
                });
                guiThread.Start();
            }
        }

        public void saveScreenshot(Packet002Screenshot p)
        {
            try
            {
                ScreenShower.log.writeParamToLog("Saving screenshot for " + p.getSender().getFriendName());
                string pathToUserDir = ScreenShower.settings.path + "\\Screenshots\\" + p.getSender().getFriendName();
                if (!Directory.Exists(pathToUserDir))
                    Directory.CreateDirectory(pathToUserDir);
                string dateTime = DateTime.Now.Day + "." + DateTime.Now.Month + "." + DateTime.Now.Year + " " + DateTime.Now.Hour + "_" + DateTime.Now.Minute + "_" + DateTime.Now.Second;
                string pathToFile = pathToUserDir + "\\Image_" + dateTime + ".png";
                FileStream fs = new FileStream(pathToFile, FileMode.Create, FileAccess.Write);
                fs.Write(p.bytes, 0, p.bytes.Length);
                fs.Close();
            }
            catch (Exception exc)
            {
                ScreenShower.log.writeParamToLog("Error saving image for " + p.getSender().getFriendName() + " " + exc.Message);
            }
        }

        public void updateUsername(string username)
        {
            user.setUsername(username);
        }

        public void saveFriendList()
        {
            try
            {

                string filePath = ScreenShower.settings.path + "\\FriendList.frdg";
                if (File.Exists(filePath))
                    File.Delete(filePath);
                FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite);
                BinaryFormatter bf = new BinaryFormatter();
                Friend[] friends = getFriends();
                string[] list = new string[friends.Length];
                for (int i = 0; i < friends.Length; i++)
                {
                    list[i] = friends[i].getLocalAddress();
                }
                bf.Serialize(fs, list);
                fs.Close();
            }
            catch (Exception exc)
            {
                ScreenShower.log.writeParamToLog("Exception happened!: " + exc.Message);  
                MessageBox.Show(null, "Ошибка во время сохранения списка друзей.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void loadAndConnectToFriends()
        {
            try
            {
                string filePath = ScreenShower.settings.path + "\\FriendList.frdg";
                FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                BinaryFormatter bf = new BinaryFormatter();
                string[] listFriends = (string[])bf.Deserialize(fs);
                fs.Close();
                foreach (string str in listFriends)
                {
                    Friend fr = new Friend(str, ScreenShower.settings.port, this);
                    if (!fr.connectToFriend())
                        sShower.displayIconNotification("Ошибка", "Один из друзей умер :(");
                    else
                        friends.Add(fr);
                }
            }
            catch(Exception exc)
            {
                ScreenShower.log.writeParamToLog("Error: unable to load friend list " + exc.Message);
                sShower.displayIconNotification("Ошибка", "Не удалось загрузить список друзей");
            }
        }

        public Friend[] getFriends()
        {
            return friends.ToArray();
        }

        public int getFriendsCount()
        {
            return friends.Count;
        }

        public void connectionError(Friend friend)
        {
            friend.killFriend();
            friend.setStatus("Потеряно соединение: " + DateTime.Now.ToString());
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

        public void startUpdater()
        {
            threadUpdater = new Thread(this.updater);
            threadUpdater.Name = "Thread Updater";
            threadUpdater.Start();
        }

        private void updater()
        {
            while (isManagerAlive)
            {
                try
                {
                    foreach (Friend friend in friends)
                    {
                        if (friend.isAlive())
                        {
                            Packet p = new Packet001FriendName(user.getName());
                            friend.sendPacket(p);
                            Packet p1 = new Packet003PingCalculator();
                            friend.sendPacket(p1);
                        }
                    }
                    Thread.Sleep(2500);
                }
                catch (Exception exc)
                {
                    ScreenShower.log.writeParamToLog("Exception: Out of updating circle " + exc.ToString());
                }
            }
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
            lock (blocker)
            {
                if (!isDuplicate(fr))
                friends.Add(fr);
                else
                    sShower.displayIconNotification("Дупликат друга", "Мы обнаружили двух одинаковых друзей. Они успешно нейтрализованы.");
            }
        }

        private bool isDuplicate(Friend fr)
        {
            string remoteIPAdress = fr.getRemoteIP();
            foreach (Friend friend in friends)
            {
                if (friend.getRemoteIP().Equals(remoteIPAdress))
                return true;
            }
            return false;
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
        public void showScreenshot(Packet002Screenshot p)
        {
            lock (blocker)
            {
                float sizeofImage = ((float)p.bytes.Length / (float)1024) / (float)1024;
                saveScreenshot(p);
                lastScreenShot = p;
                    if (ScreenShower.settings.showNotificationImmideatly == true)
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
                    else
                    {
                        sShower.isNotWatched = true;
                        sShower.changeIcon(ScreenShower.StateofIcon.notwatched);
                        sShower.displayIconNotification("Выскакивалка", "Вам скриншот от " + p.getSender().getFriendName() + " (" + sizeofImage + " MB)");
                    }
            }
        }

        public void calcPing(Packet003PingCalculator p)
        {
            lock (blocker)
            {
                if (p.direction)
                {
                    p.direction = false;
                    p.getSender().sendPacket(p);
                }
                else
                {
                    DateTime date1 = DateTime.Now;
                    DateTime date2 = p.timeMilles;
                    p.getSender().UpdateStatus((long)(date1 - date2).TotalMilliseconds);
                }
            }
        }
    }
}
