using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SharpGR_MAUI.FileIO;
using System.Windows.Input;

namespace SharpGR_MAUI.ViewModels
{
    public partial class RadioViewModel : ObservableObject
    {
        [ObservableProperty]
        private string streamUrl = "https://stream.gensokyoradio.net/3"; // ここに再生先のURLを設定

        [ObservableProperty]
        private string songInfoUrl = "https://gensokyoradio.net/api/station/playing/";

        [ObservableProperty]
        private string albumArtPrefix = "https://gensokyoradio.net/images/albums/500/";

        [ObservableProperty]
        private string albumArtPlaceholder = "https://gensokyoradio.net/images/assets/gr-logo-placeholder.png";

        [ObservableProperty]
        private string trackTitle = "未読み込み";

        [ObservableProperty]
        private string artistName = "未読み込み";

        [ObservableProperty]
        private string albumName = "未読み込み";

        [ObservableProperty]
        private string playedText = "00:00:00";

        [ObservableProperty]
        private string durationText = "00:00:00";

        [ObservableProperty]
        private Image albumArt = new Image();

        //[ObservableProperty]
        //private int durationValue;

        //[ObservableProperty]
        //private int playedValue;

        [ObservableProperty]
        private int volume = 100;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(PlayButtonText))]
        private bool isPlaying = false;

        private static readonly HttpClient httpClient = new HttpClient();

#nullable enable
        private SongAPI? songAPI = new SongAPI();
#nullable disable

        public string PlayButtonText => IsPlaying ? "停止" : "再生";

        private TimeSpan played;
        public TimeSpan Played
        {
            get => played;
            set
            {
                if (SetProperty(ref played, value))
                {
                    PlayedText = value.ToString(@"hh\:mm\:ss");
                }
            }
        }

        private TimeSpan duration;
        public TimeSpan Duration
        {
            get => duration;
            set
            {
                if (SetProperty(ref duration, value))
                {
                    DurationText = value.ToString(@"hh\:mm\:ss");
                }
            }
        }

        public ICommand PlayPauseCommand { get; }

        // View に再生/停止を依頼するための Action
        public Action RequestPlayMedia { get; set; }
        public Action RequestPauseMedia { get; set; }
        public Action RequestStopMedia { get; set; } // 必要であればStopも

        // 再生状態を管理するためのプロパティ (MediaElementコントロール用)
        // このプロパティをXAMLから直接バインドするのではなく、MediaElementのStateChangedイベントでViewModelのIsPlayingを更新します。
        // そして、ViewModelのIsPlayingプロパティの変更通知を通じてボタンのテキストが更新されます。
        // MediaElementの制御はコマンドを通じて行います。
        private CommunityToolkit.Maui.Core.Primitives.MediaElementState currentState = CommunityToolkit.Maui.Core.Primitives.MediaElementState.None;
        public CommunityToolkit.Maui.Core.Primitives.MediaElementState CurrentState
        {
            get => currentState;
            set => SetProperty(ref currentState, value); // UIから直接操作されることは想定しない
        }


        public RadioViewModel()
        {
            PlayPauseCommand = new RelayCommand(ExecutePlayPauseCommand);
            // ここで楽曲情報を取得する初期処理などを呼び出せます
            LoadTrackInfo();
        }

        private void ExecutePlayPauseCommand()
        {
            if (IsPlaying)
            {
                RequestPauseMedia?.Invoke();
                // RequestStopMedia?.Invoke(); // または Stop を使用する場合
            }
            else
            {
                RequestPlayMedia?.Invoke();
            }
        }

        public void UpdateMediaState(CommunityToolkit.Maui.Core.Primitives.MediaElementState newState)
        {
            CurrentState = newState;
            switch (newState)
            {
                case CommunityToolkit.Maui.Core.Primitives.MediaElementState.Playing:
                    IsPlaying = true;
                    break;
                case CommunityToolkit.Maui.Core.Primitives.MediaElementState.Paused:
                case CommunityToolkit.Maui.Core.Primitives.MediaElementState.Stopped:
                case CommunityToolkit.Maui.Core.Primitives.MediaElementState.Failed:
                    IsPlaying = false;
                    break;
            }
        }

        // 楽曲情報を取得・更新するメソッドの例
        public async void LoadTrackInfo()
        {
            using HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, SongInfoUrl);
            using HttpResponseMessage responseMessage = await httpClient.SendAsync(requestMessage);

            if (responseMessage.IsSuccessStatusCode)
            {
                string resString = await responseMessage.Content.ReadAsStringAsync();
                songAPI = JsonUtility.ReadJson<SongAPI>(resString);

                if (songAPI != null)
                {
                    string title = songAPI.SONGINFO.TITLE;
                    string artistName = songAPI.SONGINFO.ARTIST;
                    string albumName = songAPI.SONGINFO.ALBUM;
                    string albumArt = songAPI.MISC.ALBUMART;
                    TimeSpan durationTime = TimeSpan.FromSeconds(songAPI.SONGTIMES.DURATION);
                    TimeSpan playedTime = TimeSpan.FromSeconds(songAPI.SONGTIMES.PLAYED);
                    //int durationValue = songAPI.SONGTIMES.DURATION;
                    //int playedValue = songAPI.SONGTIMES.PLAYED;

                    if (string.IsNullOrWhiteSpace(title))
                    {
                        TrackTitle = "タイトルはありません";
                    }
                    TrackTitle = title;

                    if (string.IsNullOrWhiteSpace(artistName))
                    {
                        ArtistName = "アーティスト名はありません";
                    }
                    ArtistName = artistName;

                    if (string.IsNullOrWhiteSpace(albumName))
                    {
                        AlbumName = "アルバム名はありません";
                    }
                    AlbumName = albumName;

                    if (string.IsNullOrWhiteSpace(albumArt))
                    {
                        AlbumArt.Source = (ImageSource)new ImageSourceConverter().ConvertFromString(AlbumArtPlaceholder);
                    }
                    AlbumArt.Source = (ImageSource)new ImageSourceConverter().ConvertFromString($"{AlbumArtPrefix}{albumArt}");

                    //DurationValue = durationValue;
                    //PlayedValue = playedValue;
                    Duration = durationTime;
                    Played = playedTime;
                }
            }
        }
    }
}
