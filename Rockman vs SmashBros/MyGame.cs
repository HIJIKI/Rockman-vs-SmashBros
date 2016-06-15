using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Rockman_vs_SmashBros
{
	/// <summary>
	/// ゲームクラス
	/// </summary>
	public partial class MyGame : Game
	{
		// 描画バッファの宣言
		RenderTarget2D WorldBuffer;                             // ワールド描画バッファ
		RenderTarget2D GameScreenBuffer;                        // ゲーム画面描画バッファ

		// 描画用オブジェクト
		GraphicsDeviceManager GraphicsDeviceManager;
		SpriteBatch SpriteBatch;

		// テスト用
		Texture2D map;
		Texture2D texture;
		Vector2 PlayerPos = new Vector2(GAME_SCREEN_SIZE_W / 2, GAME_SCREEN_SIZE_H / 2);

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public MyGame()
		{
			GraphicsDeviceManager = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
			GraphicsDeviceManager.PreferredBackBufferWidth = GAME_SCREEN_SIZE_W * WindowScale;
			GraphicsDeviceManager.PreferredBackBufferHeight = GAME_SCREEN_SIZE_H * WindowScale;
		}

		/// <summary>
		/// ゲームの初期化
		/// </summary>
		protected override void Initialize()
		{
			// ここに初期化ロジックを追加

			// MonoGame コンポーネントを初期化
			base.Initialize();
		}

		/// <summary>
		/// リソースの確保
		/// </summary>
		protected override void LoadContent()
		{
			WorldBuffer = new RenderTarget2D(GraphicsDevice, GAME_SCREEN_SIZE_W * 2, GAME_SCREEN_SIZE_H * 2);
			GameScreenBuffer = new RenderTarget2D(GraphicsDevice, GAME_SCREEN_SIZE_W, GAME_SCREEN_SIZE_H);

			// SpriteBatch を新規作成
			SpriteBatch = new SpriteBatch(GraphicsDevice);

			// ここに画像の読み込み処理を追加
			map = Content.Load<Texture2D>("map.png");
			texture = Content.Load<Texture2D>("Player.png");
		}

		/// <summary>
		/// リソースの開放
		/// </summary>
		protected override void UnloadContent()
		{
			// 確保したリソースを開放
			WorldBuffer.Dispose();
			GameScreenBuffer.Dispose();
			SpriteBatch.Dispose();
			texture.Dispose();
		}

		/// <summary>
		/// フレームの更新
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update(GameTime gameTime)
		{
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
			{
				Exit();
			}

			// ここに計算処理を追加

			if (Keyboard.GetState().IsKeyDown(Keys.W))
			{
				PlayerPos.Y -= 2;
			}
			if (Keyboard.GetState().IsKeyDown(Keys.A))
			{
				PlayerPos.X -= 2;
			}
			if (Keyboard.GetState().IsKeyDown(Keys.S))
			{
				PlayerPos.Y += 2;
			}
			if (Keyboard.GetState().IsKeyDown(Keys.D))
			{
				PlayerPos.X += 2;
			}

			base.Update(gameTime);
		}

		/// <summary>
		/// 描画
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime)
		{
			// ワールドバッファの描画
			GraphicsDevice.SetRenderTarget(WorldBuffer);
			GraphicsDevice.Clear(Color.CornflowerBlue);
			SpriteBatch.Begin();
			SpriteBatch.Draw(map, Vector2.Zero, new Rectangle(0, 0, GAME_SCREEN_SIZE_W * 2, GAME_SCREEN_SIZE_H * 2), Color.White);
			SpriteBatch.Draw(texture, PlayerPos, new Rectangle(32 * 1, 32 * 1, 32, 32), Color.White, 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 1);
			SpriteBatch.End();

			// ゲームバッファの描画
			GraphicsDevice.SetRenderTarget(GameScreenBuffer);
			GraphicsDevice.Clear(Color.CornflowerBlue);
			SpriteBatch.Begin();
			SpriteBatch.Draw(WorldBuffer, Vector2.Zero, new Rectangle((int)PlayerPos.X - GAME_SCREEN_SIZE_W / 2, (int)PlayerPos.Y - GAME_SCREEN_SIZE_H / 2, GAME_SCREEN_SIZE_W, GAME_SCREEN_SIZE_H), Color.White);
			SpriteBatch.End();

			// プレイスクリーンの描画
			GraphicsDevice.SetRenderTarget(null);
			GraphicsDevice.Clear(Color.Black);
			SpriteBatch.Begin(SpriteSortMode.BackToFront, null, SamplerState.PointClamp);
			SpriteBatch.Draw(GameScreenBuffer, Vector2.Zero, new Rectangle(0, 0, GAME_SCREEN_SIZE_W, GAME_SCREEN_SIZE_H), Color.White, 0.0f, new Vector2(0, 0), (float)WindowScale, SpriteEffects.None, 1);
			SpriteBatch.End();

			base.Draw(gameTime);
		}
	}
}
