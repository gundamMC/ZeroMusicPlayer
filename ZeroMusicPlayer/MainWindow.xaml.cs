using Microsoft.WindowsAPICodePack.Shell;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
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
            Queue_Panel.Children.Add(new SongItemControlSmall() { SongName = song.Name });
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

            if (String.IsNullOrWhiteSpace(Properties.Settings.Default.DirectoryPath) || 
                !Directory.Exists(Properties.Settings.Default.DirectoryPath))
            {
                new PathMessageBox().ShowDialog();
            }

            ContentFrame.Navigate(new FilePage());
        }

        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            switch (Player.State())
            {
                case 0:
                    Player.Pause();
                    break;
                case 1:
                    Player.Resume();
                    break;
                case -1:
                    Player.PlayNow(new SongItem() { Name = SelectedSongControl.Name, Path = SelectedSongControl.Path });
                    break;
            }

            
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e) {
            App.Current.Shutdown();
        }
    }
}
