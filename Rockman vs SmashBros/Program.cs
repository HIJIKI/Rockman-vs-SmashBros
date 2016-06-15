using System;

namespace Rockman_vs_SmashBros
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
			using (var Game = new MyGame())
			{
				Game.Run();
			}
		}
	}
}
