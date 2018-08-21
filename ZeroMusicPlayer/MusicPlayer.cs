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

        private void WavePlay(String path)
        {
            StopPlayBack();

            AudioFile = new AudioFileReader(path);

            WavePlayer.Init(AudioFile);
            WavePlayer.Play();

            timer.Start();
        }

        public void PlayNext()
        {
            if (Queue.Count() < 1)
                return;

            SongItem song = GetNext();
            WavePlay(song.Path);
        }

        public void PlayNow(SongItem song)
        {
            // make the new song as the first one
            Queue.Insert(0, song);

            // skip the current song
            if (PlayMode == PLAYMODE_LOOP && Queue.Count > 2)
            {
                //Queue.Move(1, Queue.Count - 1);
            }

            for (int i = 0; i < Queue.Count; i++)
                Console.WriteLine(i + " - " + Queue[i].Name);

            Console.WriteLine(" -- ");

            WavePlay(song.Path);

        }

        private SongItem GetNext()
        {
            if (Queue.Count < 1)
                return null;

            switch (PlayMode)
            {
                case 0:
                    return Queue.First();
                case 1:
                    int rnd = new Random().Next(0, Queue.Count());
                    return Queue.ElementAt(rnd);
                case 2:
                    return Queue.First();
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
                return;

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

            History.Insert(0, item);
        }

        private void OnPlaybackStopped(object sender, StoppedEventArgs args)
        {
            AddHistory(Queue[0]);

            if (PlayMode == 0)
            {
                SongItem tmp = Queue[0];
                Queue.RemoveAt(0);
                Queue.Add(tmp);
            }

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

    }
}
