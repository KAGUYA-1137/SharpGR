using NAudio.Wave;

namespace SharpGR
{
    /// <summary>
    /// 設定ファイルのデータ構造定義クラス
    /// </summary>
    public class SettingInfo
    {
        public PlaybackState PlaybackState { get; set; } = PlaybackState.Playing;

        public int Volume { get; set; } = 50;
    }
}