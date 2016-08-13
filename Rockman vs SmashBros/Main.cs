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
		public static Player Player;                                // プレイヤー
		public static List<Entity> Entities;                        // エンティティ

		public static Scenes Scene;                                 // シーン管理
		public enum Scenes
		{
			Play
		}

		public static Point SpawnPositionOnMap;                     // プレイヤーの開始位置 (マップ上のマス数)

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

			//TODO: リリース時には削除する
			// ニコ生で画面の中心あたりにウィンドウが来るようにする
			Window.Position = new Point(200, 150);
		}

		/// <summary>
		/// ゲームの初期化
		/// </summary>
		protected override void Initialize()
		{
			// 開始シーンを設定
			Scene = Scenes.Play;

			// 各インスタンスを初期化
			Entities = new List<Entity>();
			Map.Initialize();
			Map.InitForTest();
			SetSpawnPoint(Map.SpawnPositionOnMap);
			Map.SpawnAllEntities();
			Player = new Player();
			Player.Initialize(SpawnPositionOnMap);

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

			// 各クラスのリソースを読み込み
			Player.LoadContent(Content);
			Map.LoadContent(Content);
			HyruleSoldier.LoadContent(Content);
			Platform1.LoadContent(Content);
			Platform2.LoadContent(Content);
			RockBuster.LoadContent(Content);
			ErrorEntity.LoadContent(Content);
			CheckPoint.LoadContent(Content);
			DestroyEffect1.LoadContent(Content);
			Explosion1.LoadContent(Content);

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
			HyruleSoldier.UnloadContent();
			Platform1.UnloadContent();
			Platform2.UnloadContent();
			RockBuster.UnloadContent();
			ErrorEntity.UnloadContent();
			CheckPoint.UnloadContent();
			DestroyEffect1.UnloadContent();
			Explosion1.UnloadContent();
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

			// Play シーン
			if (Scene == Scenes.Play)
			{
				UpdatePlayScene(GameTime);
			}

			base.Update(GameTime);
		}

		/// <summary>
		/// 描画
		/// </summary>
		protected override void Draw(GameTime GameTime)
		{
			// Play シーン
			if (Scene == Scenes.Play)
			{
				DrawPlayScene(GameTime);
			}

			base.Draw(GameTime);
		}

		/// <summary>
		/// プレイヤーの開始位置を設定または変更
		/// </summary>
		/// <param name="SpawnPoint">設定したいプレイヤーの開始位置 (マップ上のマス数)</param>
		public static void SetSpawnPoint(Point SpawnPositionOnMap)
		{
			Main.SpawnPositionOnMap = SpawnPositionOnMap;
		}

		#region プライベート関数

		/// <summary>
		/// Play シーンでのフレーム更新
		/// </summary>
		private void UpdatePlayScene(GameTime GameTime)
		{
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

			// 追加予約されたエンティティを追加
			Entity.ExecuteReserv();

			// カメラを更新
			Camera.Update(GameTime, Player.GetDrawPosition());

			// プレイヤーが死亡した場合
			if (!Player.IsAlive)
			{
				Entity.DestroyAll();
				Entities.RemoveAll(E => !E.IsAlive);
				Player.Initialize(SpawnPositionOnMap);
				Map.SetSectionID(Map.GetSectionIDFromPoint(SpawnPositionOnMap));
				Camera.Update(GameTime, Player.GetDrawPosition());
				Map.SpawnAllEntities();
			}

		}

		/// <summary>
		/// Play シーンでの描画
		/// </summary>
		private void DrawPlayScene(GameTime GameTime)
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
				Messages.Add("UpdateTime: " + GameTime.ElapsedGameTime);
				//Messages.Add("Player.WorldPosition: " + Player.Position);
				//Messages.Add("Player.WorldPosition(Draw): " + Player.GetDrawPosition());
				Messages.Add("Player.Health: " + Player.Health);
				Messages.Add("Player.IsInvincible: " + Player.IsInvincible);
				//Messages.Add("Player.ScreenPosition: " + (Player.Position - Camera.Position.ToVector2()));
				Messages.Add("Player.IsInAir: " + Player.IsInAir.ToString());
				Messages.Add("Player.IsTouchTerrain: " + Player.IsTouchTerrain("Top") + ", " + Player.IsTouchTerrain("Bottom") + ", " + Player.IsTouchTerrain("Left") + ", " + Player.IsTouchTerrain("Right"));
				Messages.Add("Player.RidingEntity: " + Player.GetRidingEntityString());
				Messages.Add("Player.FrameCounter: " + Player.FrameCounter);
				Messages.Add("Player.AnimationPattern: " + Player.AnimationPattern);
				//Messages.Add("CameraPosition: " + (Camera.Position));
				Messages.Add("AllEntities: " + Entities.Count);
				//Messages.Add("RockBuster: " + RockBuster.Count);
				SpriteBatch.DrawRectangle(new Rectangle(0, 0, Const.GameScreenWidth, 8 * Messages.Count), new Color(Color.Black, 0.5f), true);
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

		}

		#endregion
	}
}
