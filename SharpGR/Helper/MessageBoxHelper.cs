using System.Windows;

namespace SharpGR.Helper
{
    /// <summary>
    /// メッセージボックスを表示するためのヘルパーです。
    /// </summary>
    public class MessageBoxHelper
    {
        /// <summary>
        /// 情報メッセージボックスを表示するためのヘルパーメソッドです。
        /// </summary>
        /// <param name="infoMessage">表示する情報メッセージの内容</param>
        public static void ShowInfoMessageBox(string infoMessage)
        {
            MessageBox.Show(infoMessage, "情報", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>
        /// 確認メッセージボックスを表示するためのヘルパーメソッドです。
        /// </summary>
        /// <param name="questionMessage">表示する確認メッセージの内容</param>
        /// <returns>ユーザーの選択結果を示す <see cref="MessageBoxResult"/> を返します。</returns>
        public static MessageBoxResult ShowQuestionMessageBox(string questionMessage)
        {
            return MessageBox.Show(questionMessage, "確認", MessageBoxButton.YesNo, MessageBoxImage.Question);
        }

        /// <summary>
        /// エラーメッセージボックスを表示するためのヘルパーメソッドです。
        /// </summary>
        /// <param name="errorMessage">表示するエラーメッセージの内容</param>
        public static void ShowErrorMessageBox(string errorMessage)
        {
            MessageBox.Show(errorMessage, "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}