using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PlayList.Android.VkModels;
using Xamarin.Auth;

namespace PlayList.Android.VkService
{
    public class VkApiService
    {
        private readonly string _accessToken;

        public VkApiService(string accessToken)
        {
            _accessToken = accessToken;
        }

        public async Task<Audio> GetAudioById(string id)
        {
            CheckAccessToken();

            try
            {
                var audioRequest = WebRequest.Create($"https://api.vk.com/method/audio.getById?audios={id}&access_token={_accessToken}");

                using (var audioStream = new StreamReader(audioRequest.GetResponse().GetResponseStream()))
                {
                    var audioReponse = await audioStream.ReadToEndAsync();
                    var audios = JsonConvert.DeserializeObject<VkResponse<Audio[]>>(audioReponse);
                    return audios.Response.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                var auth = new OAuth2Authenticator("5641888", "audio", new Uri("https://oauth.vk.com/authorize"), new Uri("https://oauth.vk.com/blank.html"));
                throw new NeedAuthorizeException(auth);
            }
        }

        private void CheckAccessToken()
        {
            if (String.IsNullOrEmpty(_accessToken))
            {
                var auth = new OAuth2Authenticator("5641888", "audio", new Uri("https://oauth.vk.com/authorize"), new Uri("https://oauth.vk.com/blank.html"));
                throw new NeedAuthorizeException(auth);
            }
        }
    }
}