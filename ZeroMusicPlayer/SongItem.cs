using System;

namespace ZeroMusicPlayer {

    public class SongItem
    {
        public String Name { get; set; }
        public String Path { get; set; }
        public String Time { get; set; }

        public override bool Equals(object obj)
        {
            if (obj.GetType() != base.GetType())
                return false;

            return ((SongItem)obj).Path == this.Path;
        }

        public override int GetHashCode()
        {
            return this.Path.GetHashCode();
        }
    }
}
