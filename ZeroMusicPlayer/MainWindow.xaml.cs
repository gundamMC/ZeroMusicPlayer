using Microsoft.WindowsAPICodePack.Shell;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ZeroMusicPlayer.Model;

namespace ZeroMusicPlayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private MusicPlayer Player = new MusicPlayer();
        public void AddSong(SongItem song)
        {
            Player.Add(song);
        }
        SongItemControl SelectedSongControl = null;
        public void SetSelectedSong(SongItemControl control)
        {
            if (SelectedSongControl != null)
                SelectedSongControl.Selected = false;
            control.Selected = true;

            SelectedSongControl = control;
        }

        public MainWindow()
        {
            InitializeComponent();

            //原:@"\\excalibur\music"
            //抱歉懒得改回去了【雾 快做设置吧 #乱打注释感觉会被打
            var items = GetItems(@"D:\youxi\osu\Songs");

            Files.DataContext = items;
        }

        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }


        public List<Item> GetItems(string path)
        {
            List<Item> items = new List<Item>();
            DirectoryInfo dir = new DirectoryInfo(path);

            foreach(DirectoryInfo directory in dir.GetDirectories())
            {
                items.Add(new DirectoryItem {
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

        Thread IconThread;

        private void Files_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            List<SongItemControl> SongItems = new List<SongItemControl>();

            if (IconThread != null && IconThread.IsAlive)
                IconThread.Abort();

            IconThread = new Thread(new ThreadStart(() => LoadSongInfo(SongItems)));

            if (Files.SelectedItem is DirectoryItem)
            {
                SongsPanel.Children.Clear();


                foreach (Item item in ((DirectoryItem)Files.SelectedItem).Items)
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

                SongsPanel.Children.Add(new Label() { Height = 50 });

                IconThread.Start();
            }
        }

        private void LoadSongInfo(List<SongItemControl> items)
        {

            foreach(SongItemControl item in items)
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
                            item.Dispatcher.Invoke(() => {
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

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            Player.PlayNow(new SongItem() { Name = SelectedSongControl.Name, Path = SelectedSongControl.Path});
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e) {
            App.Current.Shutdown();
        }
    }
}
