using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SharpGR_MAUI.ViewModels
{
    /// <summary>
    /// ビューモデル基底クラス
    /// </summary>
    public class BaseViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// プロパティ変更イベント
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// プロパティのセット
        /// </summary>
        /// <param name="propertyName"></param>
        protected void SetProperty([CallerMemberName] string propertyName = null)
        {
            // プロパティ変更イベントを発生させる
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
