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
using System.Windows.Shapes;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace ZeroMusicPlayer
{
    /// <summary>
    /// Interaction logic for PathMessageBox.xaml
    /// </summary>
    public partial class PathMessageBox : Window
    {
        public PathMessageBox()
        {
            InitializeComponent();
        }

        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            App.Current.Shutdown();
        }

        private void View_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new CommonOpenFileDialog
            {
                IsFolderPicker = true
            };

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                Path_Textbox.Text = dialog.FileName;
            }
            
        }

        private void Continue_Click(object sender, RoutedEventArgs e)
        {

            if (String.IsNullOrWhiteSpace(Path_Textbox.Text) || !System.IO.Directory.Exists(Path_Textbox.Text))
            {
                MessageBox.Show("Illegal directory");
                return;
            }

            Properties.Settings.Default.DirectoryPath = Path_Textbox.Text;

            Properties.Settings.Default.Save();

            this.Close();
        }
    }
}
