using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using LightPlaylist.Models;
using Newtonsoft.Json;

namespace PlayList.Android.SyncService
{
    public class SyncApiService
    {
        public async Task<Song[]> GetNewSongs(DateTime? from)
        {
            string requestUrl = "http://kriperplaylist.azurewebsites.net/api/sync";

            if (from.HasValue)
            {
                requestUrl += $"?date={@from.Value.ToString("MM.dd.yyyy hh:mm:ss", CultureInfo.InvariantCulture)}";
            }

            var lastTracksRequest = WebRequest.Create(requestUrl);
            using (var sr = new StreamReader(lastTracksRequest.GetResponse().GetResponseStream()))
            {
                var response = await sr.ReadToEndAsync();
                return JsonConvert.DeserializeObject<Song[]>(response);
            }
        }

        public void DeleteSong(string vkKey)
        {
            string requestUrl = $"http://kriperplaylist.azurewebsites.net/api/songs?vkId={vkKey}";

            var delteSongRequest = WebRequest.Create(requestUrl);
            delteSongRequest.Method = "DELETE";
            using (var sr = new StreamReader(delteSongRequest.GetResponse().GetResponseStream()))
            {
            }
        }
    }
}