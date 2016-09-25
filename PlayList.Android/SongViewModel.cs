using Android.Views;
using PlayList.Android.DatabaseModels;

namespace PlayList.Android
{
    public class SongViewModel
    {
        public SongViewModel(View songContainer, Audio audio)
        {
            SongContainer = songContainer;
            Audio = audio;
        }

        public View SongContainer { get; set; }
        public Audio Audio { get; set; }
    }
}