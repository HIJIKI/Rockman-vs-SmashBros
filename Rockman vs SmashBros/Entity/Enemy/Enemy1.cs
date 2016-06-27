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
	/// Enemy1 クラス
	/// </summary>
	public class Enemy1 : Entity
	{
		#region メンバーの宣言
		public static Texture2D Texture;                            // テクスチャ
		public Point DrawOffset;                                  // ワールド座標に対する相対的な描画座標
		#endregion

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public Enemy1(Point Position, bool IsFromMap = false, Point FromMapPosition = new Point())
		{
			this.Position = Position.ToVector2();
			this.IsFromMap = IsFromMap;
			this.FromMapPosition = FromMapPosition;
			Type = Types.Enemy;
			Initialize();
		}

		/// <summary>
		/// 初期化
		/// </summary>
		public override void Initialize()
		{
			IsAlive = true;
			IsNoclip = false;
			MoveDistance = Vector2.Zero;
			DrawOffset.X = -15;
			DrawOffset.Y = -29;
			RelativeCollision = new Rectangle(-7, -23, 14, 24);
		}

		/// <summary>
		/// リソースの確保
		/// </summary>
		public static void LoadContent(ContentManager Content)
		{
			Texture = Content.Load<Texture2D>("Image/hairaru_hei.png");
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
			float Speed = 0.5f;
			MoveDistance.X = 0;

			if (Main.Player.Position.X > Position.X)
			{
				MoveDistance.X += Speed;
			}
			else
			{
				MoveDistance.X -= Speed;
			}

			if (IsInAir)
			{
				MoveDistance.Y += 0.25f;
			}

			if (Position.Y > Main.Map.Size.Height * Const.MapchipTileSize)
			{
				Destroy(this);
			}

			/*
			if (Main.Controller.IsButtonPressed(Controller.Buttons.A))
			{
				MoveDistance.Y = -4.25f;
			}
			//*/

			base.Update(GameTime);
		}

		/// <summary>
		/// 描画
		/// </summary>
		public override void Draw(GameTime GameTime, SpriteBatch SpriteBatch)
		{
			// 描画
			SpriteBatch.Draw(Texture, new Vector2(DrawPosition.X + DrawOffset.X, DrawPosition.Y + DrawOffset.Y), new Rectangle(0, 0, 32, 32), Color.White, 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, (float)Const.DrawOrder.Enemy / (float)Const.DrawOrder.MAX);

			base.Draw(GameTime, SpriteBatch);
		}
	}
}
