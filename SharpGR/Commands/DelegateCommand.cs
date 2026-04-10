using System;
using System.Windows.Input;

namespace SharpGR.Commands
{
    /// <summary>
    /// MVVMにおけるコマンドバインディングを簡素化するための、<see cref="ICommand"/>のカスタム実装です。<br/>
    /// ロジックを実行するメソッドと、実行可能かをチェックするメソッドをカプセル化します。
    /// </summary>
    /// <remarks>
    /// 実行ロジックと実行可能チェックロジックの両方を持つ <see cref="DelegateCommand"/> の新しいインスタンスを初期化します。
    /// </remarks>
    /// <param name="execute">実行されるアクション。</param>
    /// <param name="canExecute">コマンドが実行可能かをチェックする関数。</param>
    public class DelegateCommand(Action execute, Func<bool> canExecute) : ICommand
    {
        /// <summary>
        /// コマンドが呼び出されたときに実行されるメソッドを保持するデリゲートです。
        /// 値を返さないため、Action型を使用します。
        /// </summary>
        private readonly Action _execute = execute;

        /// <summary>
        /// 常に実行可能な <see cref="DelegateCommand"/> の新しいインスタンスを初期化します。
        /// 特定のCanExecuteロジックが不要なコマンドのための便利なコンストラクタです。
        /// </summary>
        /// <param name="execute">実行されるアクション。</param>
        public DelegateCommand(Action execute) : this(execute, () => true)
        {
            _execute = execute;
        }

        /// <summary>
        /// コマンドの実行可能状態が変更された可能性のあるときに発生するイベントです。
        /// WPFのICommandインターフェースの重要な部分です。
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add
            {
                // グローバルなCommandManagerにフックすることで、
                // 定期的に全ての登録済みコマンドの実行可能状態がチェックされます。
                CommandManager.RequerySuggested += value;
            }
            remove
            {
                // イベントハンドラーの登録を解除します。
                CommandManager.RequerySuggested -= value;
            }
        }

        /// <summary>
        /// コマンドが現在の状態で実行可能かどうかを判断します。
        /// </summary>
        /// <param name="parameter">コマンドが使用するデータ。この実装では使用されません。</param>
        /// <returns>コマンドが実行可能な場合はtrue、そうでない場合はfalse。</returns>
        public bool CanExecute(object parameter)
        {
            // canExecuteデリゲートを呼び出し、コマンドの状態をチェックします。
            return canExecute();
        }

        /// <summary>
        /// コマンドのロジックを実行します。
        /// </summary>
        /// <param name="parameter">コマンドが使用するデータ。この実装では使用されません。</param>
        public void Execute(object parameter)
        {
            // executeデリゲートを呼び出し、コマンドのアクションを実行します。
            _execute();
        }
    }
}