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
	class Player
	{
		// メンバーの宣言
		public Texture2D Texture;                                           // テクスチャ
		public Point Position;                                              // ワールド座標
		public Point DrawOffset;                                            // ワールド座標に対する相対的な描画座標

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public Player() { }

		/// <summary>
		/// 初期化
		/// </summary>
		public void Initialize()
		{
			Position.X = Const.GameScreenWidth / 2;
			Position.Y = Const.GameScreenHeight / 2;
			DrawOffset.X = -16;
			DrawOffset.Y = -31;
		}

		/// <summary>
		/// リソースの確保
		/// </summary>
		public void ContentLoad(ContentManager Content)
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
		public void Update(GameTime GameTime)
		{
			int Speed = 2;
			if (Keyboard.GetState().IsKeyDown(Keys.W))
			{
				Position.Y -= Speed;
			}
			if (Keyboard.GetState().IsKeyDown(Keys.A))
			{
				Position.X -= Speed;
			}
			if (Keyboard.GetState().IsKeyDown(Keys.S))
			{
				Position.Y += Speed;
			}
			if (Keyboard.GetState().IsKeyDown(Keys.D))
			{
				Position.X += Speed;
			}
		}

		/// <summary>
		/// 描画
		/// </summary>
		public void Draw(GameTime GameTime, SpriteBatch SpriteBatch)
		{
			// 描画
			SpriteBatch.Draw(Texture, new Vector2(Position.X + DrawOffset.X, Position.Y + DrawOffset.Y), new Rectangle(32, 32, 32, 32), Color.White, 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, (float)Const.DrawOrder.Player / (float)Const.DrawOrder.MAX);

			// デバッグ描画
			if (Global.Debug)
			{
				SpriteBatch.DrawLine(new Vector2(Position.X - 12, Position.Y), new Vector2(Position.X + 12, Position.Y), Color.Red);
				SpriteBatch.DrawLine(new Vector2(Position.X, Position.Y - 12), new Vector2(Position.X, Position.Y + 12), Color.Red);
				SpriteBatch.DrawPixel(Position.ToVector2(), Color.Black);
			}
		}

	}
}
