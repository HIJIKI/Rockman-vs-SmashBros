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
	/// Camera クラス
	/// </summary>
	public class Camera
	{
		#region メンバーの宣言
		public Point Position;                                      // カメラの座標
		public Point ViewMap;                                       // 見えているマップの範囲(マス数)
		public Point OldViewMap;                                    // 1フレーム前の見えているマップの範囲(マス数)
		#endregion

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public Camera() { }

		/// <summary>
		/// 初期化
		/// </summary>
		public void Initialize()
		{

		}

		/// <summary>
		/// リソースの確保
		/// </summary>
		public void LoadContent(ContentManager Content)
		{
		}

		/// <summary>
		/// リソースの破棄
		/// </summary>
		public void UnloadContent()
		{
		}

		/// <summary>
		/// フレームの更新
		/// </summary>
		/// <param name="PlayerDrawPosition">プレイヤーの描画座標</param>
		public void Update(GameTime GameTime, Point PlayerDrawPosition, Size WorldSize)
		{
			// カメラ座標を更新
			Position.X = PlayerDrawPosition.X - Const.GameScreenWidth / 2;
			if (Position.X < 0)
			{
				Position.X = 0;
			}
			else if (Position.X + Const.GameScreenWidth > WorldSize.Width)
			{
				Position.X = WorldSize.Width - Const.GameScreenWidth;
			}

			Position.Y = PlayerDrawPosition.Y - Const.GameScreenHeight / 2;
			if (Position.Y < 0)
			{
				Position.Y = 0;
			}
			else if (Position.Y + Const.GameScreenHeight > WorldSize.Height)
			{
				Position.Y = WorldSize.Height - Const.GameScreenHeight;
			}

			// 見えている範囲を検出
			OldViewMap = ViewMap;
			ViewMap.X = Position.X / Const.MapchipTileSize;
			ViewMap.Y = Position.Y / Const.MapchipTileSize;
		}

		/// <summary>
		/// 描画
		/// </summary>
		public void Draw(GameTime GameTime)
		{
		}

	}
}
