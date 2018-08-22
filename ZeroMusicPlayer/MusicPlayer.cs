using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Timers;

namespace ZeroMusicPlayer {
    public class MusicPlayer
    {

        private AudioFileReader AudioFile;
        private WaveOutEvent WavePlayer = new WaveOutEvent();

        private ObservableCollection<SongItem> Queue = new ObservableCollection<SongItem>();
        // currently playing = Queue[0]
        private ObservableCollection<SongItem> History = new ObservableCollection<SongItem>();
        private readonly int MaxHistory;

        public const int PLAYMODE_LOOP = 0;
        public const int PLAYMODE_SHUFFLE = 1;
        public const int PLAYMODE_SINGLE = 2;
        private int PlayMode { get; set; } = PLAYMODE_LOOP;

        private Boolean StoppedByUser = false;

        private Timer timer = new Timer() { Interval = 100 };

        public MusicPlayer(System.Windows.Controls.ItemsControl queue_panel, System.Windows.Controls.ItemsControl history_panel)
        {
            WavePlayer.PlaybackStopped += OnPlaybackStopped;
            timer.Elapsed += Timer_Elapsed;

            queue_panel.ItemsSource = this.Queue;
            history_panel.ItemsSource = this.History;

            MaxHistory = 20;
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if(AudioFile != null)
            {
                String time = FormatTimeSpan(AudioFile.TotalTime.Subtract(AudioFile.CurrentTime));
                Double percent = ((double)(AudioFile.CurrentTime.TotalSeconds) / (double)(AudioFile.TotalTime.TotalSeconds)) * 100;

                App.Current.Dispatcher.Invoke(() => {
                    ((MainWindow)App.Current.MainWindow).TimeLeft.Content = time;
                    ((MainWindow)App.Current.MainWindow).TimeProgressBar.Value = percent;
                    });
            }
            else
            {
                timer.Stop();
            }
        }

        public static string FormatTimeSpan(TimeSpan ts)
        {
            return string.Format("{0:D2}:{1:D2}", (int)ts.TotalMinutes, ts.Seconds);
        }

        private void StopPlayBack()
        {
            if (WavePlayer.PlaybackState != PlaybackState.Stopped)
            {
                StoppedByUser = true;
                WavePlayer.Stop();
            }
        }

        private void WavePlay(SongItem song)
        {
            StopPlayBack();

            AddHistory(song);

            AudioFile = new AudioFileReader(song.Path);

            WavePlayer.Init(AudioFile);
            WavePlayer.Play();

            timer.Start();
        }

        public void PlayNext()
        {

            if (Queue.Count() < 1)
                return;

            SongItem song = GetNext();
            WavePlay(song);
        }

        public void PlayNow(SongItem song)
        {
            WavePlay(song);

            // make the new song as the first one
            if (Queue.Contains(song))
                Queue.Move(Queue.IndexOf(song), 0);
            else
                Queue.Insert(0, song);

            for (int i = 0; i < Queue.Count; i++)
                Console.WriteLine(i + " - " + Queue[i].Name);

            Console.WriteLine(" -- ");
        }

        private SongItem GetNext()
        {
            if (Queue.Count < 1)
                return null;

            switch (PlayMode)
            {
                case 0:
                    if(Queue.Count > 1)
                    {
                        // since .Move cannot move to the last index
                        // (learned this the hard way)
                        SongItem tmp = Queue[0];
                        Queue.RemoveAt(0);
                        Queue.Add(tmp);
                    }
                    return Queue[0];
                case 1:
                    int rnd = new Random().Next(0, Queue.Count());
                    return Queue.ElementAt(rnd);
                case 2:
                    return Queue[0];
            }

            return null;
        }

        public void Pause()
        {
            if (AudioFile == null || WavePlayer.PlaybackState != PlaybackState.Playing)
                return;

            WavePlayer.Pause();
        }

        public void Resume()
        {
            if (AudioFile == null || WavePlayer.PlaybackState != PlaybackState.Paused)
                PlayNext();
            else
                WavePlayer.Play();
        }

        public void Add(SongItem song)
        {
            if (Queue.Contains(song))
                Queue.Move(Queue.IndexOf(song), 0);
            else
                Queue.Add(song);
        }

        public void SetVolume(int Volume)
        {
            WavePlayer.Volume = Volume / 100f;
        }

        private void AddHistory(SongItem item)
        {
            if (History.Count == MaxHistory)
                History.RemoveAt(MaxHistory - 1);

            if (History.Contains(item))
                History.Remove(item);

            History.Insert(0, item);
        }

        private void OnPlaybackStopped(object sender, StoppedEventArgs args)
        {
            
            if (StoppedByUser)
            {
                // don't automatically play the next song if it is manually stopped
                StoppedByUser = false;
            }
            else
            {
                PlayNext();
            }
        }

        public int State()
        {
            switch (WavePlayer.PlaybackState)
            {
                case PlaybackState.Playing:
                    return 0;
                case PlaybackState.Paused:
                    return 1;
                default:
                    return -1;
            }
        }

        public void Dispose()
        {
            timer?.Dispose();
            WavePlayer?.Dispose();
            AudioFile?.Dispose();
        }

    }
}
