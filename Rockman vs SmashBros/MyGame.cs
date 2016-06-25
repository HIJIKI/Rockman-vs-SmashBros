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
	/// ゲームクラス
	/// </summary>
	public partial class MyGame : Game
	{
		/// <summary>
		/// メンバーの宣言
		/// </summary>

		// 描画用オブジェクト
		GraphicsDeviceManager GraphicsDeviceManager;
		SpriteBatch SpriteBatch;
		SpriteFont Font;

		// 描画バッファの宣言
		RenderTarget2D WorldBuffer;                             // ワールド描画バッファ
		RenderTarget2D GameScreenBuffer;                        // ゲーム画面描画バッファ

		// 各クラスをインスタンス化
		Player Player = new Player();
		Map Map = new Map();
		Camera Camera = new Camera();
		Enemy[] Enemies = new Enemy[10];

		// Test
		KeyboardState OldKeyState;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public MyGame()
		{
			GraphicsDeviceManager = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
			GraphicsDeviceManager.PreferredBackBufferWidth = Const.GameScreenWidth * Global.WindowScale;
			GraphicsDeviceManager.PreferredBackBufferHeight = Const.GameScreenHeight * Global.WindowScale;
			IsMouseVisible = true;
		}

		/// <summary>
		/// ゲームの初期化
		/// </summary>
		protected override void Initialize()
		{
			// ここに初期化ロジックを追加
			Player.Initialize();
			Map.Initialize();
			Map.InitForTest();

			/*
			for (int i = 0; i < Enemies.Length; i++)
			{
				System.Random Random = new System.Random(System.Environment.TickCount+i);
				Vector2 Position = new Vector2(Random.Next(Const.GameScreenWidth * 2), Random.Next(Const.GameScreenHeight * 2));
				Rectangle Collision = new Rectangle(0, 0, Random.Next(64), Random.Next(64));
				Enemies[i] = new Enemy();
				Enemies[i].Initialize(Position, Collision);
			}
			//*/

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

			// 各オブジェクトのリソース読み込み
			Player.LoadContent(Content);
			Map.LoadContent(Content);

			// テストフォント
			Font = Content.Load<SpriteFont>("Font/myfont");
		}

		/// <summary>
		/// リソースの破棄
		/// </summary>
		protected override void UnloadContent()
		{
			// 確保したリソースを開放
			WorldBuffer.Dispose();
			GameScreenBuffer.Dispose();

			Player.UnloadContent();
			Map.UnloadContent();
		}

		/// <summary>
		/// フレームの更新
		/// </summary>
		protected override void Update(GameTime GameTime)
		{
			KeyboardState NewKeyState = Keyboard.GetState();

			// ESC が押されたらゲームを終了
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
			{
				Exit();
			}

			// デバッグモードトグル
			if (NewKeyState.IsKeyDown(Keys.F11) && OldKeyState.IsKeyUp(Keys.F11))
			{
				Global.Debug = !Global.Debug;
			}

			Map.Update(GameTime);
			Player.Update(GameTime, Map);
			Camera.Update(GameTime, Player.Position, new Size(WorldBuffer.Width, WorldBuffer.Height));

			/*
			foreach (var Enemy in Enemies)
			{
				Enemy.Update(GameTime, Player);
			}
			//*/

			OldKeyState = NewKeyState;

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
			SpriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp);

			Map.Draw(GameTime, SpriteBatch);
			Player.Draw(GameTime, SpriteBatch);

			/*
			foreach (var Enemy in Enemies)
			{
				Enemy.Draw(GameTime, SpriteBatch);
			}
			//*/

			SpriteBatch.End();

			// ゲームバッファの描画
			GraphicsDevice.SetRenderTarget(GameScreenBuffer);
			GraphicsDevice.Clear(Color.CornflowerBlue);
			SpriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp);

			SpriteBatch.Draw(WorldBuffer, Vector2.Zero, new Rectangle((int)Camera.Position.X, (int)Camera.Position.Y, Const.GameScreenWidth, Const.GameScreenHeight), Color.White);

			SpriteBatch.End();

			if (Global.Debug)
			{
				SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);
				SpriteBatch.DrawRectangle(new Rectangle(0, 0, 240, 49), new Color(Color.Black, 0.5f), true);
				Vector2 Position = new Vector2(1, 1);
				SpriteBatch.DrawString(Font, "GameTime: " + GameTime.TotalGameTime, Position, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 1); Position.Y += 8;
				SpriteBatch.DrawString(Font, "Player.WorldPosition: " + Player.Position, Position, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 1); Position.Y += 8;
				SpriteBatch.DrawString(Font, "Player.WorldPosition(Draw): " + Player.DrawPosition, Position, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 1); Position.Y += 8;
				SpriteBatch.DrawString(Font, "Player.ScreenPosition: " + (Player.Position - Camera.Position), Position, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 1); Position.Y += 8;
				SpriteBatch.DrawString(Font, "Player.IsInAir: " + Player.IsInAir.ToString(), Position, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 1); Position.Y += 8;
				SpriteBatch.DrawString(Font, "CameraPosition: " + (Camera.Position), Position, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 1); Position.Y += 8;
				SpriteBatch.End();
				//*/
			}

			// プレイスクリーンの描画
			GraphicsDevice.SetRenderTarget(null);
			GraphicsDevice.Clear(Color.Black);
			SpriteBatch.Begin(SpriteSortMode.FrontToBack, null, SamplerState.PointClamp);

			SpriteBatch.Draw(GameScreenBuffer, Vector2.Zero, new Rectangle(0, 0, Const.GameScreenWidth, Const.GameScreenHeight), Color.White, 0.0f, new Vector2(0, 0), (float)Global.WindowScale, SpriteEffects.None, 1);

			SpriteBatch.End();

			//*/

			/*
			SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);
			// Finds the center of the string in coordinates inside the text rectangle
			string Text = "ロックマン";
			Vector2 textMiddlePoint = Font.MeasureString(Text) / 2;
			Vector2 position = new Vector2(Window.ClientBounds.Width / 2, Window.ClientBounds.Height / 2);
			SpriteBatch.DrawString(Font, Text, position, Color.White, 0, textMiddlePoint, 1.0f, SpriteEffects.None, 0.5f);
			SpriteBatch.End();
			*/

			base.Draw(GameTime);
		}
	}
}
