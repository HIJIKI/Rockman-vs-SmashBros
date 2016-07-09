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
	/// Platform1 クラス
	/// </summary>
	public class Platform1 : Entity
	{
		#region メンバーの宣言
		public static Texture2D Texture;                            // テクスチャ

		private Sprite Sprite = new Sprite(new Rectangle(48, 160, 32, 16), new Vector2());

		bool IsGoingRight;
		int FrameCounter;
		#endregion

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public Platform1(Point Position, bool IsFromMap = false, Point FromMapPosition = new Point())
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
			RelativeCollision = new Rectangle(0, 0, 32, 16);
		}

		/// <summary>
		/// リソースの確保
		/// </summary>
		public static void LoadContent(ContentManager Content)
		{
			Texture = Content.Load<Texture2D>("Image/link_stage_mapchip.png");
		}

		/// <summary>
		/// リソースの破棄
		/// </summary>
		public static void UnloadContent()
		{
			Texture.Dispose();
		}

		/// <summary>
		/// フレームの更新
		/// </summary>
		public override void Update(GameTime GameTime)
		{
			if (IsGoingRight)
			{
				MoveDistance.X = 0.5f;
			}
			else
			{
				MoveDistance.X = -0.5f;
			}

			FrameCounter++;
			if (FrameCounter >= 90)
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
			Vector2 Position = GetDrawPosition().ToVector2();
			Rectangle SourceRectangle = Sprite.SourceRectangle;
			Vector2 Origin = Sprite.Origin;
			float LayerDepth = (float)Const.DrawOrder.Enemy / (float)Const.DrawOrder.MAX;

			SpriteBatch.Draw(Texture, Position, SourceRectangle, Color.White, 0.0f, Sprite.Origin, 1.0f, SpriteEffects.None, LayerDepth);

			base.Draw(GameTime, SpriteBatch);
		}

		#region プライベート関数

		#endregion
	}
}
