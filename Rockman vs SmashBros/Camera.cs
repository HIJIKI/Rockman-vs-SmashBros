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
	class Camera
	{
		// メンバーの宣言
		public Vector2 Position;                                      // カメラの座標

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
		public void Update(GameTime GameTime, Vector2 PlayerPosition, Size WorldSize)
		{
			Position.X = (int)PlayerPosition.X - Const.GameScreenWidth / 2;
			if (Position.X < 0)
			{
				Position.X = 0;
			}
			else if (Position.X + Const.GameScreenWidth > WorldSize.Width)
			{
				Position.X = WorldSize.Width - Const.GameScreenWidth;
			}

			Position.Y = (int)PlayerPosition.Y - Const.GameScreenHeight / 2;
			if (Position.Y < 0)
			{
				Position.Y = 0;
			}
			else if (Position.Y + Const.GameScreenHeight > WorldSize.Height)
			{
				Position.Y = WorldSize.Height - Const.GameScreenHeight;
			}
		}

		/// <summary>
		/// 描画
		/// </summary>
		public void Draw(GameTime GameTime)
		{
		}

	}
}
