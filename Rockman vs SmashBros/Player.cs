﻿using Microsoft.Xna.Framework;
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
	/// Player クラス
	/// </summary>
	public class Player : Entity
	{
		#region メンバーの宣言
		public static Texture2D Texture;                            // テクスチャ
		public bool IsInChangeSection;                              // 別のセクションに移動中かどうか
		public Vector2 ChangeSectionSourcePosition;                 // セクションの移動中の元の座標
		public Vector2 ChangeSectionDestinationPosition;            // セクションの移動中の先の座標
		public int ChangeSectionFrame;                              // セクションの移動中のフレームカウンター
		public bool IsInvincible;                                   // 無敵かどうか
		public int FrameCounter;                                    // フレームカウンター
		private int AnimationPattern;                               // アニメーションのパターン
		private bool IsFaceToLeft;                                  // 左を向いているかどうか
		private bool IsLadderBend;                                  // はしご掴み中に登りかけかどうか
		private float WalkSpeed;                                    // プレイヤーの歩行速度
		private float JumpSpeed;                                    // プレイヤーのジャンプの初速
		private float LadderSpeed;                                  // プレイヤーのはしご昇降速度
		private float SlidingSpeed;                                 // プレイヤーのスライディング速度
		private Rectangle Collision;                                // 右向き時の相対当たり判定
		private int InvincibleBlinkDuration;                        // 無敵点滅の残り時間 (フレーム)

		SpritesStruct Sprites;                                      // スプライト
		private struct SpritesStruct                                // 各スプライト管理構造体
		{
			public Sprite[] Neutral;                                // ニュートラル
			public Sprite[] Walk;									// 歩き
			public Sprite Jump;                                     // ジャンプ
			public Sprite Sliding;                                  // スライディング
			public Sprite[] Ladder;                                 // はしご掴まり中
			public Sprite LadderBend;                               // はしご登りかけ
			public Sprite[] Damage;                                 // 被ダメージ
		}

		private Statuses Status;                                    // プレイヤーの状態
		private enum Statuses                                       // 各プレイヤーの状態
		{
			Neutral,                                                // ニュートラル
			Walk,                                                   // 歩き
			Jump,                                                   // ジャンプ
			Sliding,                                                // スライディング
			Ladder,                                                 // はしご掴まり中
			Damage                                                  // 被ダメージ
		}
		#endregion

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public Player()
		{
			Type = Types.Player;

			#region 各スプライトの定義

			// ニュートラル
			Sprites.Neutral = new Sprite[]
			{
				new Sprite(new Rectangle(32 * 1, 32, 32, 32), new Vector2(15, 30)),
				new Sprite(new Rectangle(32 * 2, 32, 32, 32), new Vector2(15, 30)),
			};
			// 歩き
			Sprites.Walk = new Sprite[]
			{
				new Sprite(new Rectangle(32 * 4, 32, 32, 32), new Vector2(15, 30)),
				new Sprite(new Rectangle(32 * 5, 32, 32, 32), new Vector2(15, 30)),
				new Sprite(new Rectangle(32 * 6, 32, 32, 32), new Vector2(15, 30)),
				new Sprite(new Rectangle(32 * 5, 32, 32, 32), new Vector2(15, 30)),
			};
			// ジャンプ
			Sprites.Jump = new Sprite(new Rectangle(32 * 7, 32, 32, 32), new Vector2(15, 24));
			// スライディング
			Sprites.Sliding = new Sprite(new Rectangle(0, 32, 32, 32), new Vector2(15, 30));
			// はしご掴まり中
			Sprites.Ladder = new Sprite[]
			{
				new Sprite(new Rectangle(32 * 9, 32, 32, 32), new Vector2(15, 30)),
				new Sprite(new Rectangle(32 * 10, 32, 32, 32), new Vector2(15, 30)),
			};
			// はしご登りかけ
			Sprites.LadderBend = new Sprite(new Rectangle(32 * 11, 32, 32, 32), new Vector2(15, 30));
			// 被ダメージ
			Sprites.Damage = new Sprite[]
			{
				new Sprite(new Rectangle(32 * 8, 32 * 1, 32, 32), new Vector2(15, 30)),
				new Sprite(new Rectangle(32 * 8, 32 * 2, 32, 32), new Vector2(15, 30)),
			};

			#endregion
		}

		/// <summary>
		/// 初期化
		/// </summary>
		public override void Initialize()
		{
			Health = 28;
			IsAlive = true;
			Position.X = 3 * Const.MapchipTileSize;
			Position.Y = 4 * Const.MapchipTileSize;
			MoveDistance = Vector2.Zero;
			Collision = new Rectangle(-7, -23, 15, 24);
			WalkSpeed = 1.35f;
			JumpSpeed = -4.8f;
			LadderSpeed = 1.3f;
			SlidingSpeed = 2.5f;
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
			#region テストコード
			// 落下時のリスポーン
			if (Position.Y > Main.Map.Size.Height * Const.MapchipTileSize)
			{
				Initialize();
			}

			if (Global.Debug)
			{
				// マウスクリックでリスポーン
				MouseState MouseState = Mouse.GetState();
				if (MouseState.LeftButton == ButtonState.Pressed)
				{
					Rectangle ScreenArea = new Rectangle(0, 0, Const.GameScreenWidth * Global.WindowScale, Const.GameScreenHeight * Global.WindowScale);
					Point ClickPoint = new Point(MouseState.X, MouseState.Y);
					if (ScreenArea.Contains(ClickPoint))
					{
						MoveDistance = Vector2.Zero;
						IsInAir = true;
						RidingEntity = null;
						Vector2 NewPosition = new Vector2(Main.Camera.Position.X + MouseState.X / Global.WindowScale, Main.Camera.Position.Y + MouseState.Y / Global.WindowScale + RelativeCollision.Height / 2);
						SetPosition(NewPosition);
					}
				}
			}
			#endregion

			// セクション移動中および更新停止中は処理を行わない
			if (!IsInChangeSection && !IsStop)
			{
				// 通常移動の処理
				if (Status == Statuses.Neutral || Status == Statuses.Walk || Status == Statuses.Jump)
				{
					StandardOperation(Main.Map);
				}
				// スライディング中の処理
				else if (Status == Statuses.Sliding)
				{
					// SlidingOperation(Main.Map);
				}
				// ハシゴ掴まり中の処理
				else if (Status == Statuses.Ladder)
				{
					LadderOperation(Main.Map);
				}
				// 被ダメージ中の処理
				else if (Status == Statuses.Damage)
				{
					DamageOperation();
				}
			}

			// ベースを更新
			base.Update(GameTime);

			// 当たり判定を更新
			CollisionManager();

			// 無敵時間の管理
			InvincibleDurationManager();

			// セクション移動管理
			if (!IsInChangeSection)
			{
				// 別のセクションに触れていれば移動する
				CheckChangeSection(Main.Map);
			}
			// セクション移動中の処理
			else
			{
				ChangeSectionCalc();
			}

			FrameCounter++;
		}

		/// <summary>
		/// 描画
		/// </summary>
		public override void Draw(GameTime GameTime, SpriteBatch SpriteBatch)
		{
			if (IsAlive)
			{
				// 無敵中は点滅させる
				if (Status == Statuses.Damage || !IsInvincible || IsInvincible && (FrameCounter / 2) % 2 == 1)
				{
					// 描画するスプライト
					Sprite CurrentlySprite = new Sprite();

					// ニュートラル
					if (Status == Statuses.Neutral)
					{
						CurrentlySprite = Sprites.Neutral[AnimationPattern];
					}
					else if (Status == Statuses.Walk)
					{
						CurrentlySprite = Sprites.Walk[AnimationPattern];
					}
					// ジャンプ
					else if (Status == Statuses.Jump)
					{
						CurrentlySprite = Sprites.Jump;
					}
					// スライディング
					else if (Status == Statuses.Sliding)
					{
						CurrentlySprite = Sprites.Sliding;
					}
					// はしご掴まり中
					else if (Status == Statuses.Ladder)
					{
						CurrentlySprite = Sprites.Ladder[AnimationPattern];
					}
					// 被ダメージ
					else if (Status == Statuses.Damage)
					{
						CurrentlySprite = Sprites.Damage[AnimationPattern];
					}

					// 描画
					Vector2 Position = GetDrawPosition().ToVector2();
					Rectangle SourceRectangle = CurrentlySprite.SourceRectangle;
					Vector2 Origin = CurrentlySprite.Origin;
					SpriteEffects SpriteEffect = IsFaceToLeft ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
					float layerDepth = (float)Const.DrawOrder.Player / (float)Const.DrawOrder.MAX;
					// 左を向いている場合は中心座標を反転
					if (IsFaceToLeft)
					{
						Origin = new Vector2((CurrentlySprite.SourceRectangle.Width) - Origin.X, Origin.Y);
					}
					SpriteBatch.Draw(Texture, Position, SourceRectangle, Color.White, 0.0f, Origin, 1.0f, SpriteEffect, layerDepth);
				}
			}

			base.Draw(GameTime, SpriteBatch);
		}

		/// <summary>
		/// ダメージを受ける
		/// </summary>
		/// <param name="Damage">ダメージ量</param>
		public override void GiveDamage(int Damage)
		{
			if (IsAlive && !IsInvincible)
			{
				base.GiveDamage(Damage);

				if (Health <= 0)
				{
					Destroy();
				}
				else
				{
					IsInvincible = true;
					MoveDistance.Y = 0;
					ChangeStatus(Statuses.Damage);
				}
			}
		}

		#region プライベート関数

		/// <summary>
		/// 当たり判定の管理
		/// </summary>
		private void CollisionManager()
		{
			Rectangle NewCollision;
			if (IsFaceToLeft)
			{
				NewCollision = new Rectangle(1 - (Collision.X + Collision.Width), Collision.Y, Collision.Width, Collision.Height);
			}
			else
			{
				NewCollision = Collision;
			}
			RelativeCollision = NewCollision;
		}

		/// <summary>
		/// 無敵時間の管理
		/// </summary>
		private void InvincibleDurationManager()
		{
			if (Status == Statuses.Damage)
			{
				IsInvincible = true;
			}
			else
			{
				if (InvincibleBlinkDuration <= 0)
				{
					InvincibleBlinkDuration = 0;
					IsInvincible = false;
				}
				else
				{
					InvincibleBlinkDuration--;
					IsInvincible = true;
				}
			}
		}

		/// <summary>
		/// 通常の処理
		/// </summary>
		private void StandardOperation(Map Map)
		{
			// 接地している場合
			if (!IsInAir)
			{
				// スライディング開始
				if (false)
				{
					ChangeStatus(Statuses.Sliding);
					return;
				}
				// ジャンプ開始
				if (Main.Controller.IsButtonPressed(Controller.Buttons.A))
				{
					ChangeStatus(Statuses.Jump);
					MoveDistance.Y = JumpSpeed;
					IsInAir = true;
				}
				// ハシゴに捕まる
				else if (Main.Controller.IsButtonDown(Controller.Buttons.Up) && CheckGrabLadder(Map))
				{
					Point DrawPosition = GetDrawPosition();
					Vector2 NewPosition = new Vector2(DrawPosition.X / Const.MapchipTileSize * Const.MapchipTileSize + Const.MapchipTileSize / 2, DrawPosition.Y);
					GrabLadder(NewPosition);
					return;
				}
				// 足元のハシゴに捕まる
				else if (Main.Controller.IsButtonDown(Controller.Buttons.Down) && CheckBottomLadder(Map))
				{
					Point DrawPosition = GetDrawPosition();
					Vector2 NewPosition = new Vector2(DrawPosition.X / Const.MapchipTileSize * Const.MapchipTileSize + Const.MapchipTileSize / 2, DrawPosition.Y + 9);
					GrabLadder(NewPosition);
					return;
				}
			}
			// 接地していない場合
			else
			{
				// ショートジャンプ
				if (MoveDistance.Y < 0 && Main.Controller.IsButtonUp(Controller.Buttons.A))
				{
					MoveDistance.Y = 0;
				}
				// はしごに捕まる
				if ((Main.Controller.IsButtonDown(Controller.Buttons.Up) || Main.Controller.IsButtonDown(Controller.Buttons.Down)) && CheckGrabLadder(Map))
				{
					Point DrawPosition = GetDrawPosition();
					Vector2 NewPosition = new Vector2(DrawPosition.X / Const.MapchipTileSize * Const.MapchipTileSize + Const.MapchipTileSize / 2, DrawPosition.Y);
					GrabLadder(NewPosition);
					return;
				}
			}

			// 左右移動
			MoveDistance.X = 0;
			if (Keyboard.GetState().IsKeyDown(Keys.A))
			{
				MoveDistance.X -= WalkSpeed;
				IsFaceToLeft = true;
			}
			else if (Keyboard.GetState().IsKeyDown(Keys.D))
			{
				MoveDistance.X += WalkSpeed;
				IsFaceToLeft = false;
			}

			// スプライト管理
			if (IsInAir)
			{
				if (Status != Statuses.Jump)
				{
					ChangeStatus(Statuses.Jump);
				}
			}
			else if (MoveDistance.X != 0 && Status != Statuses.Walk)
			{
				ChangeStatus(Statuses.Walk);
			}
			else if(MoveDistance.X == 0 && Status != Statuses.Neutral)
			{
				ChangeStatus(Statuses.Neutral);
			}
		}

		/// <summary>
		/// はしご掴まり中の処理
		/// </summary>
		private void LadderOperation(Map Map)
		{
			// 昇降移動
			MoveDistance = Vector2.Zero;
			IsLadderBend = false;
			if (Main.Controller.IsButtonDown(Controller.Buttons.Up))
			{
				MoveDistance.Y = -LadderSpeed;
			}
			else if (Main.Controller.IsButtonDown(Controller.Buttons.Down))
			{
				MoveDistance.Y = LadderSpeed;
			}
			// ジャンプが押されたらはしごを離す
			else if (Main.Controller.IsButtonPressed(Controller.Buttons.A))
			{
				ChangeStatus(Statuses.Jump);
				IsInAir = true;
			}
			// 接地したらはしごを離す
			if (!IsInAir)
			{
				ChangeStatus(Statuses.Neutral);
			}
			// 掴める範囲にはしごがなければはしごを離す
			if (!CheckGrabLadder(Map))
			{
				ChangeStatus(Statuses.Jump);
			}
			// 登りかけかどうかを調べる
			Point DrawPosition = GetDrawPosition();
			Point LadderBendCheckPoint = new Point(DrawPosition.X, DrawPosition.Y - 16);
			Point LadderBendCheckPoint2 = new Point(DrawPosition.X, DrawPosition.Y + RelativeCollision.Y);
			if (Map.PointToCollisionIndex(LadderBendCheckPoint) != Map.CollisionTypes.Ladder &&
				Map.PointToCollisionIndex(LadderBendCheckPoint2) != Map.CollisionTypes.Ladder)
			{
				IsLadderBend = true;
			}
			// はしごを登り切る
			DrawPosition = GetDrawPosition();
			Point LadderFinishCheckPoint = new Point(DrawPosition.X, DrawPosition.Y - 9);
			Point LadderFinishCheckPoint2 = new Point(DrawPosition.X, DrawPosition.Y + RelativeCollision.Y);
			if (MoveDistance.Y < 0 &&
				Map.PointToCollisionIndex(LadderFinishCheckPoint) != Map.CollisionTypes.Ladder &&
				Map.PointToCollisionIndex(LadderFinishCheckPoint2) != Map.CollisionTypes.Ladder)
			{
				ChangeStatus(Statuses.Neutral);
				int NewPosY = (DrawPosition.Y / Const.MapchipTileSize - 1) * Const.MapchipTileSize + Const.MapchipTileSize - 1;
				SetPosY(NewPosY);
				MoveDistance.Y = 0;
				IsInAir = false;
			}
		}

		/// <summary>
		/// 被ダメージ中の処理
		/// </summary>
		private void DamageOperation()
		{
			float Speed = 0.5f;
			MoveDistance.X = IsFaceToLeft ? Speed : -Speed;

			if (FrameCounter % 2 == 1)
			{
				AnimationPattern++;
				AnimationPattern = AnimationPattern % Sprites.Damage.Length;
			}
			if (FrameCounter >= 30)
			{
				InvincibleBlinkDuration = 60;
				if (!IsInAir)
				{
					ChangeStatus(Statuses.Neutral);
				}
				else
				{
					ChangeStatus(Statuses.Jump);
				}
			}
		}

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
					Rectangle AbsoluteCollision = GetAbsoluteCollision();
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
								int FitY = AreaRect.Y + 4;
								int NewPositionY = FitY - RelativeCollision.Y;
								Destination.Y = NewPositionY;
							}
							// 下方向にはみ出していた場合
							if (Destination.Y + RelativeCollision.Y + RelativeCollision.Height > AreaRect.Y + AreaRect.Height)
							{
								int FitY = AreaRect.Y + AreaRect.Height - 5;
								int NewPositionY = FitY - (RelativeCollision.Height - 1 + RelativeCollision.Y);
								Destination.Y = NewPositionY;
							}
							// 左方向にはみ出していた場合
							if (Destination.X + RelativeCollision.X < AreaRect.X)
							{
								int FitX = AreaRect.X + 4;
								int NewPositionX = FitX - RelativeCollision.X;
								Destination.X = NewPositionX;
							}
							// 右方向にはみ出していた場合
							if (Destination.X + RelativeCollision.X + RelativeCollision.Width > AreaRect.X + AreaRect.Width)
							{
								int FitX = AreaRect.X + AreaRect.Width - 5;
								int NewPositionX = FitX - (RelativeCollision.Width - 1 + RelativeCollision.X);
								Destination.X = NewPositionX;
							}
						}

						Map.ChangeSection(i);

						ChangeSectionSourcePosition = Source;
						ChangeSectionDestinationPosition = Destination;
						ChangeSectionFrame = 0;
						IsInChangeSection = true;
						IsStop = true;

						// エンティティのスポーンを停止
						Main.Map.StopEntitySpawn = true;

						// 全てのエンティティを削除
						Entity.DestroyAll();

						// カメラの移動を開始
						Point DrawPosition = GetDrawPosition();
						Main.Camera.StartChangeSection(DrawPosition);
					}
				}
			}
		}

		/// <summary>
		/// 足元の掴める位置に梯子があるかどうかを調べる
		/// </summary>
		/// <param name="Map"></param>
		/// <returns></returns>
		private bool CheckBottomLadder(Map Map)
		{
			Point DrawPosition = GetDrawPosition();
			Point CheckPoint = DrawPosition; CheckPoint.Y += 1;   // プレイヤーの足元の1ドット下
			if (Map.PointToCollisionIndex(CheckPoint) == Map.CollisionTypes.Ladder)
			{
				return true;
			}
			return false;
		}

		/// <summary>
		/// 掴める範囲に梯子があるかどうかを調べる
		/// </summary>
		private bool CheckGrabLadder(Map Map)
		{
			Point DrawPosition = GetDrawPosition();
			Point Top = DrawPosition; Top.Y += RelativeCollision.Y;                 // 上辺
			Point Middle = DrawPosition; Middle.Y -= RelativeCollision.Height / 2;  // 中心
			Point Bottom = DrawPosition;                                            // 下辺
			if (Map.PointToCollisionIndex(Top) == Map.CollisionTypes.Ladder ||
				Map.PointToCollisionIndex(Middle) == Map.CollisionTypes.Ladder ||
				Map.PointToCollisionIndex(Bottom) == Map.CollisionTypes.Ladder)
			{
				return true;
			}
			return false;
		}

		/// <summary>
		/// はしごに捕まる
		/// </summary>
		/// <param name="AfterPosition">はしごを掴んだあとの座標</param>
		private void GrabLadder(Vector2 AfterPosition)
		{
			SetPosition(AfterPosition);
			MoveDistance = Vector2.Zero;
			ChangeStatus(Statuses.Ladder);
			IsInAir = true;
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
				IsInChangeSection = false;
				Position = ChangeSectionDestinationPosition;

				// エンティティのスポーンを再開
				Main.Map.StopEntitySpawn = false;

				// 画面内に配置された全てのエンティティをスポーンさせる
				Main.Map.SpawnAllEntities();
			}
		}

		/// <summary>
		/// ステータスを変更
		/// </summary>
		/// <param name="Status">変更先のステータス</param>
		private void ChangeStatus(Statuses Status)
		{
			AnimationPattern = 0;
			FrameCounter = 0;
			this.Status = Status;
		}

		#endregion
	}
}
