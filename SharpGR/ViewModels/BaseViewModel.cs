using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Threading;

namespace SharpGR.ViewModels
{
    /// <summary>
    /// ビューモデル基底クラス
    /// </summary>
    /// <remarks>
    /// <see cref="DispatcherObject"/> を継承し、<see cref="INotifyPropertyChanged"/> を実装することでプロパティ変更通知の共通処理を提供
    /// ビューにバインドするプロパティのsetでは本クラスの <see cref="SetProperty{T}"/> を利用して値の変更と通知を行ってください
    /// </remarks>
    public class BaseViewModel : DispatcherObject, INotifyPropertyChanged
    {
        /// <summary>
        /// プロパティ変更イベント
        /// </summary>
        /// <remarks>
        /// プロパティが変更されたときに発生します通常は <see cref="SetProperty{T}"/> メソッドを通じて発行されます
        /// UI スレッドでの通知を強制したい場合は <see cref="Dispatcher"/> を用いて発行する実装に変更してください
        /// </remarks>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// 指定したフィールドを新しい値に設定し、値が変化した場合に <see cref="PropertyChanged"/> イベントを発行します
        /// </summary>
        /// <typeparam name="T">フィールドの型</typeparam>
        /// <param name="field">変更対象のフィールドへの参照 (ref)</param>
        /// <param name="value">設定する新しい値</param>
        /// <param name="propertyName">
        /// 変更されたプロパティの名前デフォルトでは呼び出し元のメンバー名が自動的に設定されます（<see cref="CallerMemberName"/>）
        /// </param>
        /// <returns>
        /// 既存の値と新しい値が等しい場合は <c>false</c> を返し、変更が行われた場合は <c>true</c> を返します
        /// </returns>
        /// <remarks>
        /// 等価性の比較には <see cref="object.Equals(object, object)"/> を使用します
        /// PropertyChanged はこのメソッドを呼び出したスレッド上で発行されます
        /// 必要に応じて UI スレッドでの発行を保証するために
        /// <see cref="Dispatcher.Invoke"/> や <see cref="Dispatcher.BeginInvoke"/> を使用してください
        /// </remarks>
        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(field, value))
            {
                return false;
            }

            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            return true;
        }
    }
}