using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Threading;

namespace SharpGR_WPF.ViewModels
{
    /// <summary>
    /// ビューモデル基底クラス
    /// </summary>
    public class BaseViewModel : DispatcherObject, INotifyPropertyChanged
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
