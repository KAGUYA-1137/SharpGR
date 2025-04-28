using NAudio.Wave;

namespace SharpGR
{
    /// <summary>
    /// 設定ファイルのデータ構造定義クラス
    /// </summary>
    public class SettingInfo
    {
        /// <summary>
        /// 再生状態
        /// </summary>
        public PlaybackState PlaybackState { get; set; } = PlaybackState.Playing;

        /// <summary>
        /// 音量
        /// </summary>
        public int Volume { get; set; } = 50;
    }
}