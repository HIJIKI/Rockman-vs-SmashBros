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
		public static GraphicsDeviceManager GraphicsDeviceManager;
		public static SpriteBatch SpriteBatch;
		public static SpriteFont Font;

		// 描画バッファの宣言
		public static RenderTarget2D WorldBuffer;                   // ワールド描画バッファ
		public static RenderTarget2D GameScreenBuffer;              // ゲーム画面描画バッファ

		// 各メンバーを宣言
		public static Player Player = new Player();                 // プレイヤー
		public static List<Entity> Entities = new List<Entity>();   // エンティティ

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
			Map.SpawnAllEntities();

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
			HyruleSoldier.LoadContent(Content);
			Platform1.LoadContent(Content);
			Platform2.LoadContent(Content);

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
			HyruleSoldier.UnloadContent();
			Platform1.UnloadContent();
			Platform2.UnloadContent();
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

			// Platform エンティティを更新
			foreach (Entity Entity in Entities)
			{
				if (Entity.Type == Entity.Types.Platform)
				{
					Entity.Update(GameTime);
				}
			}

			// プレイヤーを更新
			Player.Update(GameTime);

			// Platform 以外のエンティティを更新
			foreach (Entity Entity in Entities)
			{
				if (Entity.Type != Entity.Types.Platform)
				{
					Entity.Update(GameTime);
				}
			}

			// 不要になったエンティティを削除
			Entities.RemoveAll(E => !E.IsAlive);

			// カメラを更新
			Camera.Update(GameTime, Player.GetDrawPosition());

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

			SpriteBatch.End();

			// デバッグ描画
			if (Global.Debug)
			{
				SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);

				List<string> Messages = new List<string>();
				//Messages.Add("GameTime: " + GameTime.TotalGameTime);
				//Messages.Add("Player.WorldPosition: " + Player.Position);
				//Messages.Add("Player.WorldPosition(Draw): " + Player.GetDrawPosition());
				Messages.Add("Player.Health: " + Player.Health);
				Messages.Add("Player.IsInvincible: " + Player.IsInvincible);
				//Messages.Add("Player.ScreenPosition: " + (Player.Position - Camera.Position.ToVector2()));
				Messages.Add("Player.IsInAir: " + Player.IsInAir.ToString());
				Messages.Add("Player.RidingEntity: " + Player.GetRidingEntityString());
				//Messages.Add("CameraPosition: " + (Camera.Position));
				Messages.Add("AllEntities: " + Entities.Count);
				SpriteBatch.DrawRectangle(new Rectangle(0, 0, 240, 8 * Messages.Count), new Color(Color.Black, 0.5f), true);
				for (int i = 0; i < Messages.Count; i++)
				{
					Vector2 Position = new Vector2(1, 1 + i * 8);
					SpriteBatch.DrawString(Font, Messages[i], Position, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 1);
				}

				Controller.Draw(GameTime, SpriteBatch);

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

		/// <summary>
		/// ワールド描画バッファをリサイズ
		/// </summary>
		/// <param name="Size"></param>
		public static void ResizeWorldBuffer(Size Size)
		{
			WorldBuffer = new RenderTarget2D(WorldBuffer.GraphicsDevice, Size.Width, Size.Height);
		}
	}
}
