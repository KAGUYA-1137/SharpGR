using SharpGR_WPF.ViewModels;
using System.Windows;

namespace SharpGR_WPF
{
    /// <summary>
    /// Form1.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel(this);
        }
    }
}