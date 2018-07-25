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
using ZeroMusicPlayer.Model;

namespace ZeroMusicPlayer
{
    /// <summary>
    /// Interaction logic for AddressBarButton.xaml
    /// </summary>
    public partial class AddressBarButton : UserControl
    {

        public string Text
        {
            get { return (String)GetValue(TextProperty); }
            set { SetValue(TextProperty, value.ToUpper()); }
        }
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(String), typeof(AddressBarButton), new PropertyMetadata(""));

        public List<DirectoryItem> Subfolders { get; set; }
        public DirectoryItem Dir { get; set; }

        private FilePage Host;

        public AddressBarButton(FilePage host)
        {
            Host = host;

            InitializeComponent();
        }

        private void AddressButton_Click(object sender, RoutedEventArgs e)
        {
            Host.Explore(Dir, Host.AddressPanel.Children.IndexOf(this) + 1);
        }
    }
}
