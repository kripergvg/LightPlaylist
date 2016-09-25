using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Android.App;
using Android.Content.PM;
using Android.Media;
using Android.Views;
using Android.Widget;
using Android.OS;
using PlayList.Android.DatabaseModels;
using PlayList.Android.Repositories;
using PlayList.Android.SyncService;
using PlayList.Android.VkService;
using Audio = PlayList.Android.VkModels.Audio;
using Environment = System.Environment;
using Path = System.IO.Path;

namespace PlayList.Android
{
    [Activity(Label = "PlayList.Android", MainLauncher = true, Icon = "@drawable/icon", LaunchMode = LaunchMode.SingleInstance)]
    public class MainActivity : Activity
    {
        private SyncRepository _syncRepository;
        private LinearLayout _lnrLayoutAudios;
        private DatabaseModels.Audio[] _playlist;
        private List<SongViewModel> _songList;
        private int _currentSongIndex = 0;
        private MediaPlayer _mediaPlayer;
        private SyncApiService _syncApiService;
        private TextView _txtViewSyncState;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            _mediaPlayer = new MediaPlayer();
            _syncApiService = new SyncApiService();

            _syncRepository = new SyncRepository();
            _playlist = _syncRepository.Get().OrderByDescending(a => a.SyncDate).ToArray();

            var mainView = LayoutInflater.Inflate(Resource.Layout.Main, null);

            var imgViewSync = mainView.FindViewById<ImageView>(Resource.Id.imgViewSync);
            imgViewSync.Click += ImgViewSync_Click;

            var btnPrev = mainView.FindViewById<Button>(Resource.Id.btnPrev);
            btnPrev.Click += BtnPrev_Click;

            var btnPlay = mainView.FindViewById<Button>(Resource.Id.btnPlay);
            btnPlay.Click += BtnPlay_Click;

            var btnNext = mainView.FindViewById<Button>(Resource.Id.btnNext);
            btnNext.Click += BtnNext_Click;

            _txtViewSyncState = mainView.FindViewById<TextView>(Resource.Id.txtViewSyncState);
            _txtViewSyncState.Text = $"{_playlist.Length}/{_playlist.Length}";

            _songList = new List<SongViewModel>();

            _lnrLayoutAudios = mainView.FindViewById<LinearLayout>(Resource.Id.lnrLayoutAudios);
            foreach (var audio in _playlist)
            {
                var lnrLayoutAudioCotnainer = CreateSongView(audio.Artist, audio.Title, audio.Id, audio.Path);
                _lnrLayoutAudios.AddView(lnrLayoutAudioCotnainer);

                _songList.Add(new SongViewModel(lnrLayoutAudioCotnainer, audio));
            }

            SetContentView(mainView);
        }

        private LinearLayout CreateSongView(string artist, string title, int localId, string path)
        {
            var lnrLayoutAudio = LayoutInflater.Inflate(Resource.Layout.AudioLayout, null);
            var audioName = lnrLayoutAudio.FindViewById<TextView>(Resource.Id.audioName);
            audioName.Text = FormatName(artist, title);

            var audioDelete = lnrLayoutAudio.FindViewById<TextView>(Resource.Id.audioDelete);
            audioDelete.Click += (s, e) => { DeleteAudio(lnrLayoutAudio, localId); };

            var audioDeleteWithRemote = lnrLayoutAudio.FindViewById<Button>(Resource.Id.audioDeleteWithRemote);
            audioDeleteWithRemote.Click += (s, e) =>
            {
                DeleteWithRemote(lnrLayoutAudio, localId);
            };

            var lnrLayoutAudioCotnainer = lnrLayoutAudio.FindViewById<LinearLayout>(Resource.Id.lnrLayoutAudioCotnainer);
            lnrLayoutAudioCotnainer.Click += (s, e) => { PlayAudio(lnrLayoutAudio, localId); };
            return lnrLayoutAudioCotnainer;
        }

        private void DisableSongView(View songView)
        {
            songView.Clickable = false;
            songView.Enabled = false;
            songView.SetBackgroundColor(global::Android.Graphics.Color.Black);
        }

        private void EnableSongView(View songView)
        {
            songView.Clickable = true;
            songView.Enabled = true;
            songView.SetBackgroundColor(global::Android.Graphics.Color.Aqua);
        }

        private void BtnNext_Click(object sender, EventArgs e)
        {
            if (!_songList.Any())
                return;

            if (_currentSongIndex == _songList.Count - 1)
            {
                _currentSongIndex = 0;
            }
            else
            {
                _currentSongIndex++;
            }

            PlaySong();
        }

        private void BtnPlay_Click(object sender, EventArgs e)
        {
            if (!_songList.Any())
                return;

            PlaySong();
        }

        private void BtnPrev_Click(object sender, EventArgs e)
        {
            if (!_songList.Any())
                return;

            if (_currentSongIndex == 0)
            {
                _currentSongIndex = _songList.Count - 1;
            }
            else
            {
                _currentSongIndex--;
            }

            PlaySong();
        }

        private void PlaySong()
        {
            _mediaPlayer.Release();
            _mediaPlayer = new MediaPlayer();
            _mediaPlayer.SetDataSource(_songList[_currentSongIndex].Audio.Url);
            _mediaPlayer.Prepare();
            _mediaPlayer.Start();
            _mediaPlayer.Completion += (s, e) =>
            {
                BtnNext_Click(s, e);
            };
        }

        private async void ImgViewSync_Click(object sender, EventArgs e)
        {
            var syncImage = (ImageView)sender;
            syncImage.Clickable = false;
            syncImage.SetBackgroundColor(global::Android.Graphics.Color.Aqua);

            var keyValueRepository = new KeyValueRepository();
            var token = keyValueRepository.GetById(Constants.ACCESS_TOKEN);
            var vkApiService = new VkApiService(token?.Value);

            var lastDate = _syncRepository.GetLastDate();
            var newSongs = await _syncApiService.GetNewSongs(lastDate);
            _txtViewSyncState.Text = $"{_songList.Count}/{newSongs.Length}";

            try
            {
                foreach (var newSong in newSongs)
                {


                    Audio audio = await vkApiService.GetAudioById(newSong.Key);
                    //TODO Разобраться почему null
                    if (audio != null)
                    {
                        var trackDestination = CreateTrackPath(audio.Id);

                        await new WebClient().DownloadFileTaskAsync(new Uri(audio.Url), trackDestination);

                        var song = new DatabaseModels.Audio
                        {
                            AddedDate = newSong.AddedDate,
                            SyncDate = DateTime.Now,
                            Artist = audio.Artist,
                            Duration = audio.Duration,
                            GenreId = audio.GenreId,
                            LyricsId = audio.LyricsId,
                            OwnerId = audio.OwnerId,
                            Path = trackDestination,
                            Title = audio.Title,
                            Url = audio.Url,
                            VkKey = newSong.Key
                        };

                        _syncRepository.Add(song);

                        var lnrLayoutAudioCotnainer = CreateSongView(audio.Artist, audio.Title, song.Id, song.Path);
                        DisableSongView(lnrLayoutAudioCotnainer);

                        RunOnUiThread(() => _lnrLayoutAudios.AddView(lnrLayoutAudioCotnainer, 0));

                        RunOnUiThread(() =>
                        {
                            EnableSongView(lnrLayoutAudioCotnainer);
                            _songList.Insert(0, new SongViewModel(lnrLayoutAudioCotnainer, song));
                            _currentSongIndex++;
                        });
                    }

                    _txtViewSyncState.Text = $"{_songList.Count}/{newSongs.Length}";
                }
            }
            catch (NeedAuthorizeException ex)
            {
                ex.Oauth.Completed += (oS, oE) =>
                {
                    keyValueRepository.Update(Constants.ACCESS_TOKEN, new KeyValue
                    {
                        Key = Constants.ACCESS_TOKEN,
                        Value = oE.Account.Properties["access_token"]
                    });
                    RunOnUiThread(() => StartActivity(this.GetType()));
                };
                RunOnUiThread(() =>
                {
                    var intent = ex.Oauth.GetUI(this);
                    StartActivity(intent);
                });

            }
            syncImage.Clickable = true;
            syncImage.SetBackgroundColor(global::Android.Graphics.Color.Transparent);
        }

        private static string CreateTrackPath(int audioId)
        {
            DirectoryInfo musicFolder = GetMusicFolder();
            return Path.Combine(musicFolder.FullName, audioId.ToString());
        }

        private void PlayAudio(View audioView, int audioId)
        {
            for (int songIndex = 0; songIndex < _songList.Count; songIndex++)
            {
                var songViewModel = _songList[songIndex];
                if (songViewModel.Audio.Id == audioId)
                {
                    _currentSongIndex = songIndex;

                    PlaySong();
                    return;
                }
            }
        }

        private void DeleteWithRemote(View audioView, int localAudioId)
        {
            var audio = _syncRepository.GetById(localAudioId);
            _syncApiService.DeleteSong(audio.VkKey);
            DeleteAudio(audioView, localAudioId);
        }

        private void DeleteAudio(View audioView, int localAudioId)
        {
            _lnrLayoutAudios.RemoveView(audioView);

            for (int indexToDelete = 0; indexToDelete < _songList.Count; indexToDelete++)
            {
                if (_songList[indexToDelete].Audio.Id == localAudioId)
                {
                    if (indexToDelete == _currentSongIndex)
                    {

                    }
                    else if (indexToDelete < _currentSongIndex)
                    {

                    }
                    else
                    {
                        _currentSongIndex--;
                    }

                    _songList.RemoveAt(indexToDelete);
                    break;
                }
            }

            var audio = _syncRepository.GetById(localAudioId);
            File.Delete(audio.Path);
            _syncRepository.Delete(localAudioId);
        }

        private static DirectoryInfo GetMusicFolder()
        {
            string musicFolder = "music";
            var musciPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), musicFolder);

            var musicDirectory = new DirectoryInfo(musciPath);

            if (!musicDirectory.Exists)
            {
                musicDirectory = Directory.CreateDirectory(musciPath);
            }

            return musicDirectory;
        }

        private string FormatName(string artist, string title)
        {
            return $"{artist} {title}";
        }
    }
}

