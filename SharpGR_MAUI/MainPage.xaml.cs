#if ANDROID
using Android.Media;
#endif
using CommunityToolkit.Maui.Core.Primitives;
using SharpGR_MAUI.ViewModels;
using System.Timers;

namespace SharpGR_MAUI
{
    public partial class MainPage : ContentPage
    {
#if ANDROID
        private readonly MediaPlayer mediaPlayer = new MediaPlayer();
#endif

        private readonly RadioViewModel viewModel;

        private System.Timers.Timer timer = new();

        public MainPage()
        {
            InitializeComponent();
            viewModel = new RadioViewModel();
            BindingContext = viewModel;
            timer.Interval = 1000;
            timer.Start();
            timer.Elapsed += Timer_Elapsed;

            viewModel.RequestPlayMedia = mediaElement.Play;
            viewModel.RequestPauseMedia = mediaElement.Pause;
            // viewModel.RequestStopMedia = mediaElement.Stop(); // Stopを使う場合

#if ANDROID
            mediaPlayer?.SetVolume(0.5f, 0.5f);
#endif
        }

        private void Timer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            viewModel.LoadTrackInfo();
        }

        private void MediaElement_StateChanged(object sender, MediaStateChangedEventArgs e)
        {
            viewModel.UpdateMediaState(e.NewState);
            if (e.NewState == MediaElementState.Playing)
            {
                viewModel.LoadTrackInfo();
            }
            // 必要であれば、他の状態変化時の処理もここに追加
        }

        private void MediaElement_PositionChanged(object sender, MediaPositionChangedEventArgs e)
        {
            viewModel.Played = e.Position;
        }

        private void MediaElement_MediaOpened(object sender, EventArgs e)
        {
            viewModel.Duration = mediaElement.Duration;
        }

        private void Slider_ValueChanged(object sender, ValueChangedEventArgs e)
        {
#if ANDROID
            mediaPlayer.SetVolume((float)Math.Floor(e.NewValue), (float)Math.Floor(e.NewValue));
#endif
        }

        //        private void OnVolumeEntryTextChanged(object sender, TextChangedEventArgs e)
        //        {
        //            string NewTextValue = e.NewTextValue.Replace("%", string.Empty);

        //            if (!string.IsNullOrWhiteSpace(NewTextValue) && Regex.IsMatch(NewTextValue, "^[0-9]+$"))
        //            {
        //                if (int.TryParse(NewTextValue, out int volumePercent))
        //                {
        //                    if (volumePercent >= 0 && volumePercent <= 100)
        //                    {
        //                        double volume = volumePercent / 100.0;
        //#if ANDROID
        //                        mediaPlayer?.SetVolume((float)volume, (float)volume);
        //#endif
        //                        VolumeSlider.Value = volume;
        //                    }
        //                    else
        //                    {
        //                        VolumeEntry.Text = $"{VolumeSlider.Value:P0}";
        //                    }
        //                }
        //            }
        //            else if (!string.IsNullOrWhiteSpace(e.OldTextValue))
        //            {
        //                VolumeEntry.Text = e.OldTextValue;
        //            }
        //        }
    }
}
