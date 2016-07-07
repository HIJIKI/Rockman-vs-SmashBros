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
	/// Platform2 クラス
	/// </summary>
	public class Platform2 : Entity
	{
		#region メンバーの宣言
		bool IsGoingRight;
		int FrameCounter;
		#endregion

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public Platform2(Point Position, bool IsFromMap = false, Point FromMapPosition = new Point())
		{
			this.Position = Position.ToVector2();
			this.IsFromMap = IsFromMap;
			this.FromMapPosition = FromMapPosition;
			Type = Types.Platform;
			Initialize();
		}

		/// <summary>
		/// 初期化
		/// </summary>
		public override void Initialize()
		{
			IsIgnoreGravity = true;
			IsAlive = true;
			MoveDistance = Vector2.Zero;
			FrameCounter = 0;
			RelativeCollision = new Rectangle(-32, -15, 64, 16);
		}

		/// <summary>
		/// リソースの確保
		/// </summary>
		public static void LoadContent(ContentManager Content)
		{
		}

		/// <summary>
		/// リソースの破棄
		/// </summary>
		public static void UnloadContent()
		{
		}

		/// <summary>
		/// フレームの更新
		/// </summary>
		public override void Update(GameTime GameTime)
		{
			if (IsGoingRight)
			{
				MoveDistance.X = 1f;
				MoveDistance.Y = 1f;
			}
			else
			{
				MoveDistance.X = -1f;
				MoveDistance.Y = -1f;
			}

			FrameCounter++;
			if (FrameCounter >= 60)
			{
				FrameCounter = 0;
				IsGoingRight = !IsGoingRight;
			}

			base.Update(GameTime);
		}

		/// <summary>
		/// 描画
		/// </summary>
		public override void Draw(GameTime GameTime, SpriteBatch SpriteBatch)
		{
			base.Draw(GameTime, SpriteBatch);
		}

		#region プライベート関数

		#endregion
	}
}
