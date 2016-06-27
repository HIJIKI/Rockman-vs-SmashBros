using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Design;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Storage;

namespace Rockman_vs_SmashBros
{
	/// <summary>
	/// Const クラス
	/// </summary>
	/// グローバルな定数を定義するクラス
	static class Const
	{
		/// <summary>
		/// 定数定義
		/// </summary>
		public const int GameScreenWidth = 256;                         // ゲーム画面の内部解像度(横幅)
		public const int GameScreenHeight = 240;                        // ゲーム画面の内部解像度(高さ)

		public const int MapchipTileSize = 16;                          // マップチップタイル1枚のピクセル数

		public enum DrawOrder                                           // 各要素の描画順 (FrontToBack)
		{
			BGLayer,                                                    // 背景レイヤー
			LowerLayer,                                                 // 下層レイヤー
			Enemy,                                                      // エネミー
			Player,                                                     // プレイヤー
			UpperLayer,                                                 // 上層レイヤー
			MAX                                                         // レイヤーの最大数
		}

	}
}
