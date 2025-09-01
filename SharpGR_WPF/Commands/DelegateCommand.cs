using System;
using System.Windows.Input;

namespace SharpGR_WPF.Commands
{
    /// <summary>
    /// MVVMにおけるコマンドバインディングを簡素化するための、ICommandのカスタム実装です。
    /// ロジックを実行するメソッドと、実行可能かをチェックするメソッドをカプセル化します。
    /// </summary>
    public class DelegateCommand : ICommand
    {
        // コマンドが実行可能かどうかをチェックするメソッドを保持するデリゲートです。
        // bool値を返すため、Func<bool>型を使用します。
        private readonly Func<bool> canExecute;

        // コマンドが呼び出されたときに実行されるメソッドを保持するデリゲートです。
        // 値を返さないため、Action型を使用します。
        private readonly Action execute;

        /// <summary>
        /// 常に実行可能なDelegateCommandの新しいインスタンスを初期化します。
        /// 特定のCanExecuteロジックが不要なコマンドのための便利なコンストラクタです。
        /// </summary>
        /// <param name="execute">実行されるアクション。</param>
        public DelegateCommand(Action execute) : this(execute, () => true)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        /// <summary>
        /// 実行ロジックと実行可能チェックロジックの両方を持つDelegateCommandの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="execute">実行されるアクション。</param>
        /// <param name="canExecute">コマンドが実行可能かをチェックする関数。</param>
        public DelegateCommand(Action execute, Func<bool> canExecute)
        {
            // 提供されたデリゲートをプライベートフィールドに割り当てます。
            this.execute = execute;
            this.canExecute = canExecute;
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
            execute();
        }
    }
}