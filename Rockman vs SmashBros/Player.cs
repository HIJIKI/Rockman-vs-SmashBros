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
	class Player : Entity
	{
		#region メンバーの宣言
		public Texture2D Texture;									// テクスチャ
		public Vector2 DrawOffset;                                  // ワールド座標に対する相対的な描画座標
		#endregion

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public Player() { }

		/// <summary>
		/// 初期化
		/// </summary>
		public new void Initialize()
		{
			IsAlive = true;
			IsNoclip = false;
			Position.X = Const.GameScreenWidth / 2;
			Position.Y = Const.GameScreenHeight / 2;
			MoveDistance = Vector2.Zero;
			DrawOffset.X = -15;
			DrawOffset.Y = -31;
			RelativeCollision = new Rectangle(-7, -23, 14, 24);
		}

		/// <summary>
		/// リソースの確保
		/// </summary>
		public void LoadContent(ContentManager Content)
		{
			Texture = Content.Load<Texture2D>("Image/Player.png");
		}

		/// <summary>
		/// リソースの破棄
		/// </summary>
		public void UnloadContent()
		{
			Texture.Dispose();
		}

		/// <summary>
		/// フレームの更新
		/// </summary>
		public new void Update(GameTime GameTime, Map Map, Controller Controller)
		{
			float Speed = 2;
			MoveDistance.X = 0;
			/*
			if (Keyboard.GetState().IsKeyDown(Keys.W))
			{
				MoveDistance.Y -= Speed;
			}
			if (Keyboard.GetState().IsKeyDown(Keys.S))
			{
				MoveDistance.Y += Speed;
			}
			//*/
			if (Keyboard.GetState().IsKeyDown(Keys.A))
			{
				MoveDistance.X -= Speed;
			}
			if (Keyboard.GetState().IsKeyDown(Keys.D))
			{
				MoveDistance.X += Speed;
			}

			if (IsInAir)
			{
				MoveDistance.Y += 0.25f;
			}

			if (Position.Y > Map.Size.Height * Const.MapchipTileSize)
			{
				Initialize();
			}

			if (Controller.IsButtonPressed(Controller.Buttons.A))
			{
				MoveDistance.Y = -4.25f;
			}

			base.Update(GameTime, Map);
		}

		/// <summary>
		/// 描画
		/// </summary>
		public new void Draw(GameTime GameTime, SpriteBatch SpriteBatch)
		{
			// 描画
			SpriteBatch.Draw(Texture, new Vector2((int)Position.X + (int)DrawOffset.X, (int)Position.Y + (int)DrawOffset.Y), new Rectangle(32, 32, 32, 32), Color.White, 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, (float)Const.DrawOrder.Player / (float)Const.DrawOrder.MAX);

			base.Draw(GameTime, SpriteBatch);
		}

	}
}
