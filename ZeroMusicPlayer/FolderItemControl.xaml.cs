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
    public partial class FolderItemControl : ItemControl
    {

        public DirectoryItem Dir { get; set; }
        public FilePage Host { get; set; }


        public FolderItemControl()
        {
            InitializeComponent();
        }

        private void FolderItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Host.Explore(Dir);
        }

        private void FolderItem_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ((MainWindow)App.Current.MainWindow).SetSelectedItemControl(this);
        }
    }
}
