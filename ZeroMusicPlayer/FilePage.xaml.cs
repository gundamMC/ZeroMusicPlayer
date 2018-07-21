using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ZeroMusicPlayer.Model;
using Microsoft.WindowsAPICodePack.Shell;
using System.Drawing;
using System.Windows.Interop;
using System.IO;
using NAudio.Wave;

namespace ZeroMusicPlayer
{
    /// <summary>
    /// Interaction logic for FilePage.xaml
    /// </summary>
    public partial class FilePage : Page
    {
        public FilePage()
        {
            InitializeComponent();

            List<Item> items = GetItems(Properties.Settings.Default.DirectoryPath);

            Explore(items);
        }

        Thread IconThread;

        private void Explore(List<Item> items)
        {
            List<SongItemControl> SongItems = new List<SongItemControl>();

            if (IconThread != null && IconThread.IsAlive)
                IconThread.Abort();

            IconThread = new Thread(new ThreadStart(() => LoadSongInfo(SongItems)));

            SongsPanel.Children.Clear();

            foreach (Item item in items)
            {

                if (item is FileItem)
                {
                    SongItemControl tmp = new SongItemControl()
                    {
                        SongName = item.Name,
                        Path = item.Path
                    };

                    SongsPanel.Children.Add(tmp);
                    SongItems.Add(tmp);
                }
            }

            // to "push" the items up the panel
            SongsPanel.Children.Add(new Label() { Height = 50 });

            IconThread.Start();
        }

        private void LoadSongInfo(List<SongItemControl> items)
        {
            foreach (SongItemControl item in items)
            {
                String time = GetDuration(item.Path);

                String SongName;
                String Author = "UNKNOW";

                using (TagLib.File fileTags = TagLib.File.Create(item.Path))
                {
                    SongName = fileTags.Tag.Title;
                    if (fileTags.Tag.Performers.Count() > 0)
                        Author = fileTags.Tag.Performers[0];
                }

                using (ShellFile shellFile = ShellFile.FromFilePath(item.Path))
                {
                    using (Bitmap shellThumb = shellFile.Thumbnail.ExtraLargeBitmap)
                    {
                        IntPtr hBitMap = shellThumb.GetHbitmap();
                        try
                        {
                            var bs = Imaging.CreateBitmapSourceFromHBitmap(hBitMap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                            bs.Freeze();
                            var image = new ImageBrush(bs);
                            image.Freeze();
                            item.Dispatcher.Invoke(() =>
                            {
                                item.Icon = image;
                                item.Time = time;
                                if (!String.IsNullOrWhiteSpace(SongName))
                                    item.SongName = SongName;
                                item.Author = Author;
                            });
                        }
                        finally
                        {
                            DeleteObject(hBitMap); // prevent memory leak from Imaging.CreateBitmapSourceFromHBitmap
                        }
                    }
                }
            }

            items.Clear();

            // force garbage collect
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true);

        }

        public List<Item> GetItems(string path)
        {
            List<Item> items = new List<Item>();
            DirectoryInfo dir = new DirectoryInfo(path);

            foreach (DirectoryInfo directory in dir.GetDirectories())
            {
                items.Add(new DirectoryItem
                {
                    Name = directory.Name,
                    Path = directory.FullName,
                    Items = GetItems(directory.FullName)
                });
            }

            foreach (FileInfo file in dir.GetFiles())
            {
                if (!SupportedAudio.Extensions.Contains(file.Extension.ToLower()))
                    continue;

                items.Add(new FileItem
                {
                    Name = file.Name.Replace(file.Extension, ""),
                    Path = file.FullName
                });
            }

            return items;
        }

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);

        private static String GetDuration(string fileName)
        {
            try
            {
                MediaFoundationReader wf = new MediaFoundationReader(fileName);
                String result = MusicPlayer.FormatTimeSpan(wf.TotalTime);
                wf.Dispose();
                return result;
            }
            catch
            {
                return "UNKNOW";
            }

        }
    }
}
