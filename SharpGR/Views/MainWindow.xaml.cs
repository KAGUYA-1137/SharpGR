using MahApps.Metro.Controls;
using SharpGR.ViewModels;

namespace SharpGR
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        /// <summary>
        /// <see cref="MainWindow"/> のインスタンスを初期化します。
        /// XAML で定義された UI 要素を読み込むために <see cref="InitializeComponent"/> を呼び出します。
        /// 必要に応じてイベント登録や DataContext の設定などの追加初期化をここに記述してください。
        /// </summary>
        /// <remarks>
        /// 長時間実行される初期化処理は UI スレッドをブロックしないよう別スレッドで行い、
        /// UI の更新は <see cref="System.Windows.Threading.Dispatcher"/> 経由で行ってください。
        /// </remarks>
        public MainWindow()
        {
            // コンポーネントの初期化
            InitializeComponent();

            // DataContext の設定
            DataContext = new MainViewModel();
        }
    }
}