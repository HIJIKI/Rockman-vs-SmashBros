namespace Rockman_vs_SmashBros
{
	/// <summary>
	/// 定数定義
	/// </summary>
	public partial class MyGame
	{
		public const int GAME_SCREEN_SIZE_W = 256;                      // ゲーム画面の内部解像度: 幅
		public const int GAME_SCREEN_SIZE_H = 240;                      // ゲーム画面の内部解像度: 高さ

		public const int MAP_CHIP_SIZE = 16;                            // マップチップ1枚のサイズ

		public enum DrawOrder                                           // 各要素の描画順 (BackToFront)
		{
			BGLayer,													// 背景レイヤー
			LowerLayer,													// 下層レイヤー
			Player,														// プレイヤー
			UpperLayer,													// 上層レイヤー
			MAX_LAYER													// レイヤーの最大数
		}
	}
}
