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
    /// Interaction logic for SongItemControlSmall.xaml
    /// </summary>
    public partial class SongItemControlSmall : UserControl
    {
        public string SongName
        {
            get { return (String)GetValue(SongNameProperty); }
            set { SetValue(SongNameProperty, value.ToUpper()); }
        }

        public static readonly DependencyProperty SongNameProperty = DependencyProperty.Register("SongName", typeof(String), typeof(SongItemControlSmall), new PropertyMetadata(""));

        public SongItemControlSmall()
        {
            InitializeComponent();
        }
    }
}
