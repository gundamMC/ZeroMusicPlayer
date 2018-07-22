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
    /// Interaction logic for FolderItemControl.xaml
    /// </summary>
    public partial class FolderItemControl : UserControl
    {
        public string FolderName
        {
            get { return (String)GetValue(FolderNameProperty); }
            set { SetValue(FolderNameProperty, value.ToUpper()); }
        }

        public List<Item> Items { get; set; }
        public FilePage Host { get; set; }

        public Boolean Selected
        {
            get { return (Boolean)GetValue(SelectedProperty); }
            set { SetValue(SelectedProperty, value); }
        }

        public static readonly DependencyProperty FolderNameProperty = DependencyProperty.Register("FolderName", typeof(String), typeof(FolderItemControl), new PropertyMetadata(""));
        public static readonly DependencyProperty SelectedProperty = DependencyProperty.Register("Selected", typeof(Boolean), typeof(FolderItemControl), new PropertyMetadata(false));

        public FolderItemControl()
        {
            InitializeComponent();
        }

        private void FolderItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Host.Explore(Items);
        }
    }
}
