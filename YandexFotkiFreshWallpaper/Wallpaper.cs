using System;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace YandexFotkiFreshWallpaper
{
    public static class Wallpaper
    {
        const int SpiSetdeskwallpaper = 20;
        const int SpifUpdateinifile = 0x01;
        const int SpifSendwininichange = 0x02;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

        public enum Style
        {
            Tiled,
            Centered,
            Stretched,
            Filled
        }

        public enum FileType
        {
            Png,
            Bmp,
            Jpg
        }

        public static void Set(Uri uri, Style style, FileType fileType)
        {
            var s = new System.Net.WebClient().OpenRead(uri.ToString());

            if (s != null)
            {
                var img = System.Drawing.Image.FromStream(s);
                var tempPath = "";
                switch (fileType)
                {
                    case FileType.Png:
                        tempPath = Path.Combine(Path.GetTempPath(), "wallpaper.png");
                        img.Save(tempPath, System.Drawing.Imaging.ImageFormat.Png);
                        break;
                    case FileType.Jpg:
                        tempPath = Path.Combine(Path.GetTempPath(), "wallpaper.jpg");
                        img.Save(tempPath, System.Drawing.Imaging.ImageFormat.Jpeg);
                        break;
                    case FileType.Bmp:
                        tempPath = Path.Combine(Path.GetTempPath(), "wallpaper.bmp");
                        img.Save(tempPath, System.Drawing.Imaging.ImageFormat.Bmp);
                        break;
                }

                var key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true);
                if (key != null)
                {

                    switch (style)
                    {
                        case Style.Stretched:
                            key.SetValue(@"WallpaperStyle", 2.ToString(CultureInfo.InvariantCulture));
                            key.SetValue(@"TileWallpaper", 0.ToString(CultureInfo.InvariantCulture));
                            break;
                        case Style.Centered:
                            key.SetValue(@"WallpaperStyle", 1.ToString(CultureInfo.InvariantCulture));
                            key.SetValue(@"TileWallpaper", 0.ToString(CultureInfo.InvariantCulture));
                            break;
                        case Style.Tiled:
                            key.SetValue(@"WallpaperStyle", 1.ToString(CultureInfo.InvariantCulture));
                            key.SetValue(@"TileWallpaper", 1.ToString(CultureInfo.InvariantCulture));
                            break;
                        case Style.Filled:
                            key.SetValue(@"WallpaperStyle", 10.ToString(CultureInfo.InvariantCulture));
                            key.SetValue(@"TileWallpaper", 0.ToString(CultureInfo.InvariantCulture));
                            break;
                    }
                }
                SystemParametersInfo(SpiSetdeskwallpaper,
                    0,
                    tempPath,
                    SpifUpdateinifile | SpifSendwininichange);
            }
        }
    }
}
