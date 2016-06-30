using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Design;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Storage;
using System.Collections.Generic;

namespace Rockman_vs_SmashBros
{
	/// <summary>
	/// Player クラス
	/// </summary>
	public class Player : Entity
	{
		#region メンバーの宣言
		public static Texture2D Texture;                            // テクスチャ
		public Point DrawOffset;                                    // ワールド座標に対する相対的な描画座標
		public bool InChangeSection;                                // 別のセクションに移動中かどうか
		public Vector2 ChangeSectionSourcePosition;                 // セクションの移動中の元の座標
		public Vector2 ChangeSectionDestinationPosition;            // セクションの移動中の先の座標
		public int ChangeSectionFrame;                              // セクションの移動中のフレームカウンター
		#endregion

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public Player()
		{
			Type = Types.Player;
		}

		/// <summary>
		/// 初期化
		/// </summary>
		public void Initialize()
		{
			IsAlive = true;
			IsNoclip = false;
			Position.X = 32;
			Position.Y = 90;
			MoveDistance = Vector2.Zero;
			DrawOffset.X = -15;
			DrawOffset.Y = -31;
			RelativeCollision = new Rectangle(-7, -23, 14, 24);
		}

		/// <summary>
		/// リソースの確保
		/// </summary>
		public static void LoadContent(ContentManager Content)
		{
			Texture = Content.Load<Texture2D>("Image/Player.png");
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
			// 落下時のリスポーン
			if (Position.Y > Main.Map.Size.Height * Const.MapchipTileSize)
			{
				Initialize();
			}

			// 通常操作
			if (!InChangeSection)
			{
				// 左右移動
				float Speed = 1.25f;
				MoveDistance.X = 0;
				if (Keyboard.GetState().IsKeyDown(Keys.A))
				{
					MoveDistance.X -= Speed;
				}
				if (Keyboard.GetState().IsKeyDown(Keys.D))
				{
					MoveDistance.X += Speed;
				}

				// 重力付加
				if (IsInAir)
				{
					MoveDistance.Y += 0.25f;
				}

				// ジャンプ
				if (Main.Controller.IsButtonPressed(Controller.Buttons.A))
				{
					MoveDistance.Y = -5.25f;
				}

				// カメラの中央にリセット
				if (Main.Controller.IsButtonPressed(Controller.Buttons.Start))
				{
					Position = Main.Camera.Position.ToVector2();
					Position.X += Const.GameScreenWidth / 2;
					Position.Y += Const.GameScreenHeight / 2;
				}
			}

			// ベースを更新
			base.Update(GameTime);

			// セクション移動管理
			if (!InChangeSection)
			{
				// 別のセクションに触れていれば移動する
				CheckChangeSection(Main.Map);
			}
			// セクション移動中の処理
			else
			{
				ChangeSectionCalc();
			}
		}

		/// <summary>
		/// 描画
		/// </summary>
		public override void Draw(GameTime GameTime, SpriteBatch SpriteBatch)
		{
			// 描画
			SpriteBatch.Draw(Texture, new Vector2(DrawPosition.X + DrawOffset.X, DrawPosition.Y + DrawOffset.Y), new Rectangle(32, 32, 32, 32), Color.White, 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, (float)Const.DrawOrder.Player / (float)Const.DrawOrder.MAX);

			base.Draw(GameTime, SpriteBatch);
		}

		#region プライベート関数

		/// <summary>
		/// 別のセクションに触れていればそのセクションに移動する
		/// </summary>
		private void CheckChangeSection(Map Map)
		{
			// 現在のセクションID
			int CurrentlySectionID = Map.CurrentlySectionID;
			// セクション配列
			List<Map.Section> Sections = Map.Sections;

			for (int i = 0; i < Sections.Count; i++)
			{
				if (i != CurrentlySectionID)
				{
					Rectangle AreaRect = new Rectangle(Sections[i].Area.X * Const.MapchipTileSize, Sections[i].Area.Y * Const.MapchipTileSize, Sections[i].Area.Width * Const.MapchipTileSize, Sections[i].Area.Height * Const.MapchipTileSize);
					if (AbsoluteCollision.Intersects(AreaRect))
					{
						Vector2 Source = Position;
						Vector2 Destination = Source;
						// 移動後の座標を移動先セクション内に収める
						{
							// 上方向にはみ出していた場合
							if (Destination.Y + RelativeCollision.Y < AreaRect.Y)
							{
								int FitY = AreaRect.Y;
								int NewPositionY = FitY - RelativeCollision.Y;
								Destination.Y = NewPositionY;
							}
							// 下方向にはみ出していた場合
							if (Destination.Y + RelativeCollision.Y + RelativeCollision.Height > AreaRect.Y + AreaRect.Height)
							{
								int FitY = AreaRect.Y + AreaRect.Height - 1;
								int NewPositionY = FitY - (RelativeCollision.Height - 1 + RelativeCollision.Y);
								Destination.Y = NewPositionY;
							}
							// 左方向にはみ出していた場合
							if (Destination.X + RelativeCollision.X < AreaRect.X)
							{
								int FitX = AreaRect.X;
								int NewPositionX = FitX - RelativeCollision.X;
								Destination.X = NewPositionX;
							}
							// 右方向にはみ出していた場合
							if (Destination.X + RelativeCollision.X + RelativeCollision.Width > AreaRect.X + AreaRect.Width)
							{
								int FitX = AreaRect.X + AreaRect.Width - 1;
								int NewPositionX = FitX - (RelativeCollision.Width - 1 + RelativeCollision.X);
								Destination.X = NewPositionX;
							}
						}

						Map.ChangeSection(i);

						ChangeSectionSourcePosition = Source;
						ChangeSectionDestinationPosition = Destination;
						ChangeSectionFrame = 0;
						InChangeSection = true;
						IsStop = true;

						// エンティティのスポーンを停止
						Main.Map.StopEntitySpawn = true;

						// 全てのエンティティを削除
						Entity.DestroyAll();

						// カメラの移動を開始
						Main.Camera.StartChangeSection(DrawPosition);
					}
				}
			}
		}

		/// <summary>
		/// セクション移動中の処理
		/// </summary>
		private void ChangeSectionCalc()
		{
			Vector2 Source = ChangeSectionSourcePosition;
			Vector2 Destination = ChangeSectionDestinationPosition;
			int Duration = Global.ChangeSectionDuration;
			Position.X = Source.X + (Destination.X - Source.X) / Duration * ChangeSectionFrame;
			Position.Y = Source.Y + (Destination.Y - Source.Y) / Duration * ChangeSectionFrame;

			ChangeSectionFrame++;
			// セクション移動完了時の処理
			if (ChangeSectionFrame > Duration)
			{
				IsStop = false;
				InChangeSection = false;
				Position = ChangeSectionDestinationPosition;

				// エンティティのスポーンを再開
				Main.Map.StopEntitySpawn = false;

				// 画面内に配置された全てのエンティティをスポーンさせる
				Main.Map.SpawnAllEntities();
			}
		}

		#endregion
	}
}
