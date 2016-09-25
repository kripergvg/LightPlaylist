using Newtonsoft.Json;
using SQLite;

namespace PlayList.Android.VkModels
{
    public class Audio
    {
        [JsonProperty("aid")]
        public int Id { get; set; }

        [JsonProperty("owner_id")]
        public int OwnerId { get; set; }

        public string Artist { get; set; }

        public string Title { get; set; }

        public int Duration { get; set; }

        public string Url { get; set; }

        [JsonProperty("lyrics_id")]
        public int LyricsId { get; set; }

        [JsonProperty("genre")]
        public int GenreId { get; set; }
    }
}