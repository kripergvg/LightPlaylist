using System;
using SQLite;

namespace PlayList.Android.DatabaseModels
{
    public class Audio
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public int OwnerId { get; set; }

        public string Artist { get; set; }

        public string Title { get; set; }

        public int Duration { get; set; }

        public string Url { get; set; }

        public int LyricsId { get; set; }

        public int GenreId { get; set; }

        public string Path { get; set; }

        public DateTime AddedDate { get; set; }

        public DateTime SyncDate { get; set; }

        [NotNull]
        public string VkKey { get; set; }
    }
}