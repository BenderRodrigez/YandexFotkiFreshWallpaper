using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

namespace YandexFotkiFreshWallpaper
{
    public partial class Form1 : Form
    {
        private Timer _updateTimer;
        public Form1()
        {
            InitializeComponent();
            _updateTimer = new Timer();
            var span = new TimeSpan(1,0,0,0);
            _updateTimer.Interval = (int)Math.Round(span.TotalMilliseconds);
            _updateTimer.Enabled = true;
            _updateTimer.Tick += UpdateTimerOnTick;
            _updateTimer.Start();
            this.Visible = false;
            notifyIcon1.Visible = true;
            GetInitImage();
        }

        private void UpdateTimerOnTick(object sender, EventArgs eventArgs)
        {
            var getImmageRequest =
                System.Net.WebRequest.Create(@"http://api-fotki.yandex.ru/api/podhistory/");
            var response = getImmageRequest.GetResponse();
            var stream = response.GetResponseStream();
            if (stream != null)
                using (var reader = new StreamReader(stream))
                {
                    var s = reader.ReadToEnd();
                    var uri = GetImageUri(s);
                    pictureBox1.Invoke(new Action<string>(SetImage), uri);
                }
        }

        private void GetInitImage()
        {
            var getImmageRequest =
                   System.Net.WebRequest.Create(@"http://api-fotki.yandex.ru/api/podhistory/");
            var response = getImmageRequest.GetResponse();
            var stream = response.GetResponseStream();
            if (stream != null)
                using (var reader = new StreamReader(stream))
                {
                    var s = reader.ReadToEnd();
                    var uri = GetImageUri(s);
                    pictureBox1.Load(uri);
                    pictureBox1.Image.Save(Environment.CurrentDirectory + "\\wallpaper.png", ImageFormat.Png);
                    Wallpaper.Set(new Uri(Environment.CurrentDirectory + "\\wallpaper.png"), Wallpaper.Style.Filled, Wallpaper.FileType.Png);
                }
        }

        private void SetImage(string uri)
        {
            pictureBox1.Load(uri);
            pictureBox1.Image.Save(Environment.CurrentDirectory + "\\wallpaper.png", ImageFormat.Png);
            Wallpaper.Set(new Uri(Environment.CurrentDirectory + "\\wallpaper.png"), Wallpaper.Style.Filled, Wallpaper.FileType.Png);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //TODO:Auth will be here
        }

        private string GetImageUri(string responseXml)
        {
            var xmlDoc = XDocument.Parse(responseXml);

            var entries = xmlDoc.Element(XName.Get("feed", "http://www.w3.org/2005/Atom")).Element(XName.Get("entry", "http://www.w3.org/2005/Atom")).Elements(XName.Get("img", "yandex:fotki"));
            var imgUri = entries.Where(x => x.Attribute(XName.Get("size", "")).Value == "orig").Select(x => x.Attribute(XName.Get("href", "")).Value).FirstOrDefault();
            return imgUri;
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Visible = !Visible;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                Visible = false;
                WindowState = FormWindowState.Normal;
            }
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Visible = !Visible;
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
