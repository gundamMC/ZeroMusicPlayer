using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ZeroMusicPlayer
{
    /// <summary>
    /// Interaction logic for SongItemControl.xaml
    /// </summary>
    public partial class SongItemControl : UserControl
    {

        public string SongName
        {
            get { return (String)GetValue(SongNameProperty); }
            set { SetValue(SongNameProperty, value.ToUpper()); }
        }

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

        public static readonly DependencyProperty SongNameProperty = DependencyProperty.Register("SongName", typeof(String), typeof(SongItemControl), new PropertyMetadata(""));
        public static readonly DependencyProperty AuthorProperty = DependencyProperty.Register("Author", typeof(String), typeof(SongItemControl), new PropertyMetadata(""));
        public static readonly DependencyProperty TimeProperty = DependencyProperty.Register("Time", typeof(String), typeof(SongItemControl), new PropertyMetadata(""));
        public static readonly DependencyProperty IconProperty = DependencyProperty.Register("Icon", typeof(ImageBrush), typeof(SongItemControl), new PropertyMetadata(new ImageBrush()));

        public SongItemControl()
        {
            InitializeComponent();
        }
    }
}
