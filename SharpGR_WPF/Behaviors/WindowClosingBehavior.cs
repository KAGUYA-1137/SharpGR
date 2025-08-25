using System.Windows;
using System.Windows.Input;

namespace SharpGR_WPF.Behaviors
{
    public static class WindowClosingBehavior
    {
        public static readonly DependencyProperty ClosingCommandProperty =
            DependencyProperty.RegisterAttached("ClosingCommand", typeof(ICommand), typeof(WindowClosingBehavior), new PropertyMetadata(null, OnClosingCommandChanged));

        public static ICommand GetClosingCommand(DependencyObject obj) => (ICommand)obj.GetValue(ClosingCommandProperty);
        public static void SetClosingCommand(DependencyObject obj, ICommand value) => obj.SetValue(ClosingCommandProperty, value);

        private static void OnClosingCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Window window)
            {
                if (e.NewValue != null)
                {
                    window.Closing += OnWindowClosing;
                }
                else
                {
                    window.Closing -= OnWindowClosing;
                }
            }
        }

        private static void OnWindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (sender is Window window && GetClosingCommand(window) is ICommand command && command.CanExecute(null))
            {
                command.Execute(null);
            }
        }
    }
}
