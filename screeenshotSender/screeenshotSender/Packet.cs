using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;
namespace screeenshotSender
{
    [Serializable]
    public abstract class Packet
    {
        [NonSerialized]
        protected Friend fr;
        public abstract void performPacket(NetManager manager);
        public void setOwner(Friend f) { this.fr = f; }
        public Friend getSender() { return fr; }
    }

    [Serializable]
    public class Packet001FriendName : Packet
    {
        public string name;
        public bool direction;

        public Packet001FriendName(string name)
        {
            this.name = name;
            direction = true;
        }

        public override void performPacket(NetManager manager)
        {
            manager.friendNameExchanger(this);
        }
    }
    [Serializable]
    public class Packet002Screenshot : Packet
    {
        
        public byte[] bytes;

        public Packet002Screenshot(Image im)
        {
            MemoryStream ms = new MemoryStream();
            im.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            bytes = ms.ToArray();
        }

        public override void performPacket(NetManager manager)
        {
            manager.Screenshotshot(this);
        }
    }
}
