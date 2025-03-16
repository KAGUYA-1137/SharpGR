using NAudio.Wave;

namespace SharpGR
{
    public class SettingInfo
    {
        public int Volume { get; set; } = 50;

        public PlaybackState PlaybackState { get; set; } = PlaybackState.Playing;
    }
}