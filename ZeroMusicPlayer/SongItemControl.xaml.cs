using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ZeroMusicPlayer
{
    /// <summary>
    /// Interaction logic for SongItemControl.xaml
    /// </summary>
    public partial class SongItemControl : ItemControl
    {

        public string Path { get; set; }

        public string Author
        {
            get { return (String)GetValue(AuthorProperty); }
            set { SetValue(AuthorProperty, value.ToUpper()); }
        }

        public string Time
        {
            get { return (String)GetValue(TimeProperty); }
            set { SetValue(TimeProperty, value); }
        }

        public ImageBrush Icon
        {
            get { return (ImageBrush)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        public static readonly DependencyProperty AuthorProperty = DependencyProperty.Register("Author", typeof(String), typeof(SongItemControl), new PropertyMetadata(""));
        public static readonly DependencyProperty TimeProperty = DependencyProperty.Register("Time", typeof(String), typeof(SongItemControl), new PropertyMetadata(""));
        public static readonly DependencyProperty IconProperty = DependencyProperty.Register("Icon", typeof(ImageBrush), typeof(SongItemControl), new PropertyMetadata(new ImageBrush()));

        public SongItemControl()
        {
            InitializeComponent();
        }

        private void SongItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ((MainWindow)App.Current.MainWindow).AddSong(new SongItem() { Name = ItemName, Path = this.Path, Time = this.Time });
        }


        private void SongItem_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ((MainWindow)App.Current.MainWindow).SetSelectedItemControl(this);
        }
    }
}
