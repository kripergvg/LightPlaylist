using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using PlayList.Android.DatabaseModels;
using PlayList.Android.Repositories;

namespace PlayList.Android
{
    [Activity(Label = "PlayerActivity")]
    public class PlayerActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Player);

            var syncRepository = new SyncRepository();
            var audios = syncRepository.Get();
            var indexToPlay = 0;

            var player = new MediaPlayer();

            var btnPlayNext = FindViewById<Button>(Resource.Id.btnPlayNext);
            btnPlayNext.Click += (s, e) =>
            {
                player.Reset();
                player.SetDataSource(audios[indexToPlay].Path);
                player.Prepare();
                player.Start();

                if (audios.Length - 1 > indexToPlay)
                {
                    indexToPlay++;
                }
                else
                {
                    indexToPlay = 0;
                }
            };
        }
    }
}