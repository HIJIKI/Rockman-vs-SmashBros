using System;
using System.Windows.Forms;

namespace MapEditor
{
    /// <summary>
    /// メインクラス
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// エントリーポイント
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            using (var MainWindow = new MainWindow())
            {
                Application.Run(MainWindow);
            }
        }
    }
}
