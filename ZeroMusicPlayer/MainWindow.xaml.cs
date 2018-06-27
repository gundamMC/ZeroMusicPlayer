using Microsoft.WindowsAPICodePack.Shell;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
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
            try {
                MediaFoundationReader wf = new MediaFoundationReader(fileName);
                String result = MusicPlayer.FormatTimeSpan(wf.TotalTime);
                wf.Dispose();
                return result;
            } catch (Exception e) {
                
            }
            return null;
        }

        private void Files_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            // TODO: Multithread for loading the icons

            if(Files.SelectedItem is DirectoryItem)
            {
                SongsPanel.Children.Clear();


                foreach (Item item in ((DirectoryItem)Files.SelectedItem).Items)
                {
                    if(item is FileItem)
                    {
                        using (TagLib.File fileTags = TagLib.File.Create(item.Path))
                        {
                            SongItemControl tmp = new SongItemControl()
                            {
                                SongName = (String.IsNullOrEmpty(fileTags.Tag.Title)) ? item.Name : fileTags.Tag.Title,
                                Path = item.Path,
                                Time = GetDuration(item.Path),
                                Author = (fileTags.Tag.Artists.Count() > 0) ? fileTags.Tag.Artists[0] : "UNKNOWN"
                            };

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
                                        tmp.Icon = image;
                                    }
                                    finally
                                    {
                                        DeleteObject(hBitMap); // prevent memory leak from Imaging.CreateBitmapSourceFromHBitmap
                                    }


                                }
                            }

                            SongsPanel.Children.Add(tmp);
                        }
                    }
                }

                SongsPanel.Children.Add(new Label() { Height = 50 });

                // force garbage collect
                GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true);
            }
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            Player.PlayNow(new SongItem() { Name = SelectedSongControl.Name, Path = SelectedSongControl.Path});
        }
    }
}
