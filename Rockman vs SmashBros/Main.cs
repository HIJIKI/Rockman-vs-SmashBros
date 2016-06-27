using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Design;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Storage;
using System.Collections.Generic;
using System;

namespace Rockman_vs_SmashBros
{
	/// <summary>
	/// ゲームクラス
	/// </summary>
	public class Main : Game
	{
		#region メンバーの宣言

		// 描画用オブジェクト
		GraphicsDeviceManager GraphicsDeviceManager;
		SpriteBatch SpriteBatch;
		public static SpriteFont Font;

		// 描画バッファの宣言
		RenderTarget2D WorldBuffer;                             // ワールド描画バッファ
		RenderTarget2D GameScreenBuffer;                        // ゲーム画面描画バッファ

		// 各メンバーを宣言
		public static Player Player = new Player();
		public static Map Map = new Map();
		public static Camera Camera = new Camera();
		public static List<Entity> Entities = new List<Entity>();

		public static Controller Controller = new Controller();

		#endregion

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public Main()
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

			// 画像リソースの読み込み
			Player.LoadContent(Content);
			Map.LoadContent(Content);
			Enemy1.LoadContent(Content);

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
			Enemy1.UnloadContent();
		}

		/// <summary>
		/// フレームの更新
		/// </summary>
		protected override void Update(GameTime GameTime)
		{
			// ESC が押されたらゲームを終了
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
			{
				Exit();
			}

			// デバッグモードトグル
			if (Controller.IsButtonPressed(Controller.Buttons.Select))
			{
				Global.Debug = !Global.Debug;
			}

			// コントローラ入力状態を更新
			Controller.Update(GameTime);

			// マップを更新
			Map.Update(GameTime);

			// プレイヤーを更新
			Player.Update(GameTime);

			// エンティティを追加(テスト)
			if (Controller.IsButtonPressed(Controller.Buttons.B))
			{
				Entity.Create("Enemy1", new Point(128, 64));
			}

			// エンティティを更新
			for (int i = 0; i < Entities.Count; i++)
			{
				Entities[i].Update(GameTime);
			}

			// 不要になったエンティティを削除
			Entities.RemoveAll(E => !E.IsAlive);

			// カメラを更新
			Camera.Update(GameTime, Player.DrawPosition, new Size(WorldBuffer.Width, WorldBuffer.Height));

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

			for (int i = 0; i < Entities.Count; i++)
			{
				if (Entities[i] != null)
				{
					Entities[i].Draw(GameTime, SpriteBatch);
				}
			}

			SpriteBatch.End();

			// ゲームバッファの描画
			GraphicsDevice.SetRenderTarget(GameScreenBuffer);
			GraphicsDevice.Clear(Color.CornflowerBlue);
			SpriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp);

			SpriteBatch.Draw(WorldBuffer, Vector2.Zero, new Rectangle(Camera.Position.X, Camera.Position.Y, Const.GameScreenWidth, Const.GameScreenHeight), Color.White);

			Controller.Draw(GameTime, SpriteBatch);

			SpriteBatch.End();

			if (Global.Debug)
			{
				SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);
				SpriteBatch.DrawRectangle(new Rectangle(0, 0, 240, 57), new Color(Color.Black, 0.5f), true);
				Vector2 Position = new Vector2(1, 1);
				SpriteBatch.DrawString(Font, "GameTime: " + GameTime.TotalGameTime, Position, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 1); Position.Y += 8;
				SpriteBatch.DrawString(Font, "Player.WorldPosition: " + Player.Position, Position, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 1); Position.Y += 8;
				SpriteBatch.DrawString(Font, "Player.WorldPosition(Draw): " + Player.DrawPosition, Position, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 1); Position.Y += 8;
				SpriteBatch.DrawString(Font, "Player.ScreenPosition: " + (Player.Position - Camera.Position.ToVector2()), Position, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 1); Position.Y += 8;
				SpriteBatch.DrawString(Font, "Player.IsInAir: " + Player.IsInAir.ToString(), Position, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 1); Position.Y += 8;
				SpriteBatch.DrawString(Font, "CameraPosition: " + (Camera.Position), Position, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 1); Position.Y += 8;
				SpriteBatch.DrawString(Font, "AllEntities: " + Entities.Count, Position, Color.Pink, 0, Vector2.Zero, 1, SpriteEffects.None, 1); Position.Y += 8;
				SpriteBatch.End();
			}

			// プレイスクリーンの描画
			GraphicsDevice.SetRenderTarget(null);
			GraphicsDevice.Clear(Color.Black);
			SpriteBatch.Begin(SpriteSortMode.FrontToBack, null, SamplerState.PointClamp);
			SpriteBatch.Draw(GameScreenBuffer, Vector2.Zero, new Rectangle(0, 0, Const.GameScreenWidth, Const.GameScreenHeight), Color.White, 0.0f, new Vector2(0, 0), (float)Global.WindowScale, SpriteEffects.None, 1);
			SpriteBatch.End();

			base.Draw(GameTime);
		}
	}
}
