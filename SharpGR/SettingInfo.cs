using NAudio.Wave;

namespace SharpGR
{
    public class SettingInfo
    {
        public PlaybackState PlaybackState { get; set; } = PlaybackState.Playing;

        public int Volume { get; set; } = 50;
    }
}