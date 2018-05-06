using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeroMusicPlayer
{
    class MusicPlayer
    {

        private AudioFileReader AudioFile;
        private WaveOutEvent WavePlayer = new WaveOutEvent();


        private LinkedList<SongItem> Queue = new LinkedList<SongItem>();
        private int PlayMode = 0;
        // 0 for loop, 1 for shuffle, 2 for single

        private Boolean CalledByPlayNext = false;

        public MusicPlayer()
        {
            WavePlayer.PlaybackStopped += OnPlaybackStopped;
        }


        public static string FormatTimeSpan(TimeSpan ts)
        {
            return string.Format("{0:D2}:{1:D2}", (int)ts.TotalMinutes, ts.Seconds);
        }

        public void PlayNext()
        {
            if (Queue.Count() < 1)
                return;

            if (WavePlayer.PlaybackState == PlaybackState.Playing)
            {
                CalledByPlayNext = true;
                WavePlayer.Stop();
            }

            SongItem song = GetNext();
            AudioFile = new AudioFileReader(song.Path);

            WavePlayer.Init(AudioFile);
            WavePlayer.Play();

        }

        private SongItem GetNext()
        {
            if (Queue.Count < 1)
                return null;

            SongItem result;

            switch (PlayMode)
            {
                case 0:
                    result = Queue.First();
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

        public void PlayNow(SongItem song)
        {
            if (WavePlayer.PlaybackState == PlaybackState.Playing)
            {
                CalledByPlayNext = true;
                WavePlayer.Stop();
            }

            AudioFile = new AudioFileReader(song.Path);

            WavePlayer.Init(AudioFile);
            WavePlayer.Play();
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
            AudioFile.Dispose();
            AudioFile = null;

            if (CalledByPlayNext)
            {
                CalledByPlayNext = false;
            }
            else
            {
                PlayNext();
            }

        }

        public String test()
        {
            String result = "";
            foreach(SongItem i in Queue)
            {
                result += i.Path + "\n";
            }

            return result;
        }

    }
}
