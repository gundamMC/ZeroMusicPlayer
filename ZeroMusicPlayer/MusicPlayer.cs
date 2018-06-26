using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace ZeroMusicPlayer
{
    class MusicPlayer
    {

        private AudioFileReader AudioFile;
        private WaveOutEvent WavePlayer = new WaveOutEvent();


        private LinkedList<SongItem> Queue = new LinkedList<SongItem>();
        private int PlayMode = 0;
        // 0 for loop, 1 for shuffle, 2 for single

        private Boolean StoppedByUser = false;

        private Timer timer = new Timer() { Interval = 1000 };

        public MusicPlayer()
        {
            WavePlayer.PlaybackStopped += OnPlaybackStopped;
            timer.Elapsed += Timer_Elapsed;
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
            else // AudioFile == null
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

        public void PlayNext()
        {
            if (Queue.Count() < 1)
                return;

            StopPlayBack();

            SongItem song = GetNext();
            AudioFile = new AudioFileReader(song.Path);

            WavePlayer.Init(AudioFile);
            WavePlayer.Play();

            timer.Start();

        }

        public void PlayNow(SongItem song)
        {
            StopPlayBack();
            
            AudioFile = new AudioFileReader(song.Path);

            WavePlayer.Init(AudioFile);
            WavePlayer.Play();

            timer.Start();
        }

        private SongItem GetNext()
        {
            if (Queue.Count < 1)
                return null;

            switch (PlayMode)
            {
                case 0:
                    SongItem result = Queue.First();
                    Queue.RemoveFirst();
                    Queue.AddLast(result);
                    return result;
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

        public void Play()
        {
            if (AudioFile == null || WavePlayer.PlaybackState != PlaybackState.Paused)
                return;

            WavePlayer.Play();
        }

        public void Add(SongItem song)
        {
            Queue.AddLast(song);
        }

        public void SetVolume(int Volume)
        {
            WavePlayer.Volume = Volume / 100f;
        }

        private void OnPlaybackStopped(object sender, StoppedEventArgs args)
        {

            if (StoppedByUser)
            {
                StoppedByUser = false;
            }
            else
            {
                PlayNext();
            }
        }

    }
}
