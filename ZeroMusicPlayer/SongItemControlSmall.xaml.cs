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
    public partial class SongItemControlSmall : ItemControl
    {

        public string Time
        {
            get { return (String)GetValue(TimeProperty); }
            set { SetValue(TimeProperty, value); }
        }

        public static readonly DependencyProperty TimeProperty = DependencyProperty.Register("Time", typeof(String), typeof(SongItemControlSmall), new PropertyMetadata(""));


        public SongItemControlSmall()
        {
            InitializeComponent();
        }
    }
}
