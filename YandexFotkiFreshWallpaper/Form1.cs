﻿using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows.Forms;
using System.Xml.Linq;

namespace YandexFotkiFreshWallpaper
{
    public partial class Form1 : Form
    {
        private readonly Timer _updateTimer;
        public Form1()
        {
            InitializeComponent();
            _updateTimer = new Timer();
            var span = new TimeSpan(0, 3, 0, 0);
            _updateTimer.Interval = (int)Math.Round(span.TotalMilliseconds);
            _updateTimer.Enabled = true;
            _updateTimer.Tick += UpdateTimerOnTick;
            _updateTimer.Start();
            Visible = false;
            notifyIcon1.Visible = true;
            GetInitImage();
        }

        private void UpdateTimerOnTick(object sender, EventArgs eventArgs)
        {
            try
            {
                var getImmageRequest =
                    WebRequest.Create(@"http://api-fotki.yandex.ru/api/podhistory/");
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
            catch (Exception e)
            {
                //TODO: Cath the exceptions
            }
        }

        private void GetInitImage()
        {
            var getImmageRequest =
                   WebRequest.Create(@"http://api-fotki.yandex.ru/api/podhistory/");
            var response = getImmageRequest.GetResponse();
            var stream = response.GetResponseStream();
            if (stream != null)
                using (var reader = new StreamReader(stream))
                {
                    var s = reader.ReadToEnd();
                    var uri = GetImageUri(s);
                    var client = new WebClient();
                    client.DownloadFile(uri, Environment.CurrentDirectory + "\\wallpaper.jpg");

                    pictureBox1.Load(Environment.CurrentDirectory + "\\wallpaper.jpg");
                    //pictureBox1.Image.Save(Environment.CurrentDirectory + "\\wallpaper.jpg", ImageFormat.Png);
                    Wallpaper.Set(new Uri(uri), Wallpaper.Style.Filled, Wallpaper.FileType.Jpg);
                }
        }

        private void SetImage(string uri)
        {
            var client = new WebClient();
            client.DownloadFile(uri, Environment.CurrentDirectory + "\\wallpaper.jpg");

            pictureBox1.Load(Environment.CurrentDirectory + "\\wallpaper.jpg");
            Wallpaper.Set(new Uri(uri), Wallpaper.Style.Filled, Wallpaper.FileType.Jpg);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //TODO:Auth will be here
        }

        private string GetImageUri(string responseXml)
        {
            var xmlDoc = XDocument.Parse(responseXml);

            var xElement = xmlDoc.Element(XName.Get("feed", "http://www.w3.org/2005/Atom"));
            if (xElement != null)
            {
                var element = xElement.Element(XName.Get("entry", "http://www.w3.org/2005/Atom"));
                if (element != null)
                {
                    var entries = element.Elements(XName.Get("img", "yandex:fotki"));
                    var imgUri = entries.Where(x => x.Attribute(XName.Get("size", "")).Value == "orig").Select(x => x.Attribute(XName.Get("href", "")).Value).FirstOrDefault();
                    return imgUri;
                }
            }
            return null;
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
