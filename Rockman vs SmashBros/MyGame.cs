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
		// 描画用オブジェクト
		GraphicsDeviceManager GraphicsDeviceManager;
		SpriteBatch SpriteBatch;

		// 描画バッファの宣言
		RenderTarget2D WorldBuffer;                             // ワールド描画バッファ
		RenderTarget2D GameScreenBuffer;                        // ゲーム画面描画バッファ

		// テスト用
		Texture2D texture;
		Vector2 PlayerPos = new Vector2(Const.GameScreenWidth / 2, Const.GameScreenHeight / 2);
		Map Map = new Map();

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public MyGame()
		{
			GraphicsDeviceManager = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
			GraphicsDeviceManager.PreferredBackBufferWidth = Const.GameScreenWidth * WindowScale;
			GraphicsDeviceManager.PreferredBackBufferHeight = Const.GameScreenHeight * WindowScale;
		}

		/// <summary>
		/// ゲームの初期化
		/// </summary>
		protected override void Initialize()
		{
			// ここに初期化ロジックを追加
			Map.InitForTest();

			// MonoGame コンポーネントを初期化
			base.Initialize();
		}

		/// <summary>
		/// リソースの確保
		/// </summary>
		protected override void LoadContent()
		{
			// 描画に使用する SpriteBatch を初期化
			SpriteBatch = new SpriteBatch(GraphicsDevice);

			// 裏描画バッファの初期化
			WorldBuffer = new RenderTarget2D(GraphicsDevice, Map.Size.Width * Const.MapchipTileSize, Map.Size.Height * Const.MapchipTileSize);
			GameScreenBuffer = new RenderTarget2D(GraphicsDevice, Const.GameScreenWidth, Const.GameScreenHeight);

			Map.ContentLoad(Content);

			// 画像リソースの読み込み
			texture = Content.Load<Texture2D>("Images/Player.png");
		}

		/// <summary>
		/// リソースの破棄
		/// </summary>
		protected override void UnloadContent()
		{
			// 確保したリソースを開放
			WorldBuffer.Dispose();
			GameScreenBuffer.Dispose();

			Map.UnloadContent();

			// テスト用
			texture.Dispose();
		}

		/// <summary>
		/// フレームの更新
		/// </summary>
		protected override void Update(GameTime GameTime)
		{
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
			{
				Exit();
			}

			// ここに計算処理を追加

			Map.Update(GameTime);

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

			base.Update(GameTime);
		}

		/// <summary>
		/// 描画
		/// </summary>
		protected override void Draw(GameTime GameTime)
		{
			// ワールドバッファの描画
			GraphicsDevice.SetRenderTarget(WorldBuffer);
			GraphicsDevice.Clear(Color.CornflowerBlue);
			SpriteBatch.Begin(SpriteSortMode.FrontToBack, null, SamplerState.PointClamp);
			Map.Draw(GameTime, SpriteBatch);
			SpriteBatch.Draw(texture, PlayerPos, new Rectangle(32 * 1, 32 * 1, 32, 32), Color.White, 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, (float)Const.DrawOrder.Player / (float)Const.DrawOrder.MAX);
			SpriteBatch.End();

			// ゲームバッファの描画
			GraphicsDevice.SetRenderTarget(GameScreenBuffer);
			GraphicsDevice.Clear(Color.CornflowerBlue);
			SpriteBatch.Begin(SpriteSortMode.FrontToBack, null, SamplerState.PointClamp);
			SpriteBatch.Draw(WorldBuffer, Vector2.Zero, new Rectangle(0, 0, Const.GameScreenWidth, Const.GameScreenHeight), Color.White);
			SpriteBatch.End();

			// プレイスクリーンの描画
			GraphicsDevice.SetRenderTarget(null);
			GraphicsDevice.Clear(Color.Black);
			SpriteBatch.Begin(SpriteSortMode.FrontToBack, null, SamplerState.PointClamp);
			SpriteBatch.Draw(GameScreenBuffer, Vector2.Zero, new Rectangle(0, 0, Const.GameScreenWidth, Const.GameScreenHeight), Color.White, 0.0f, new Vector2(0, 0), (float)WindowScale, SpriteEffects.None, 1);
			SpriteBatch.End();

			base.Draw(GameTime);
		}
	}
}
