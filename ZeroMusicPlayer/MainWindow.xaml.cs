using Microsoft.WindowsAPICodePack.Shell;
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

        public MainWindow()
        {
            InitializeComponent();

            var items = GetItems(@"your path here");

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
                items.Add(new FileItem
                {
                    Name = file.Name,
                    Path = file.FullName
                });
            }

            return items;
        }

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);

        private void Files_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            // TODO: Multithread for loading the icons

            Console.WriteLine(((Item)Files.SelectedItem).Name);

            if(Files.SelectedItem is DirectoryItem)
            {

                SongsPanel.Children.Clear();

                foreach (Item item in ((DirectoryItem)Files.SelectedItem).Items)
                {
                    if(item is FileItem)
                    {
                        ShellFile shellFile = ShellFile.FromFilePath(item.Path);
                        using (Bitmap shellThumb = shellFile.Thumbnail.ExtraLargeBitmap)
                        {
                            IntPtr hBitMap = shellThumb.GetHbitmap();
                            ImageBrush image = new ImageBrush(Imaging.CreateBitmapSourceFromHBitmap(hBitMap,
                                                                                    IntPtr.Zero,
                                                                                    Int32Rect.Empty,
                                                                                    BitmapSizeOptions.FromEmptyOptions()));

                            image.Freeze();
                            SongsPanel.Children.Add(new SongItemControl() { SongName = item.Name, Icon = image });

                            DeleteObject(hBitMap); // prevent memory leak from Imaging.CreateBitmapSourceFromHBitmap

                        }
                        shellFile.Dispose();
                    }
                }

                SongsPanel.Children.Add(new Label() { Height = 50 });
            }

            // force garbage collect
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true);
        }
    }
}
