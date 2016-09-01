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
		public int AnimationPattern;                                // アニメーションのパターン
		private bool IsFaceToLeft;                                  // 左を向いているかどうか
		private bool IsLadderBend;                                  // はしご掴み中に登りかけかどうか
		private int InvincibleBlinkFrame;                           // 無敵点滅の残り時間 (フレーム数)
		private bool IsShooting;                                    // ショットモーション中かどうか
		private int ShootingFrameCounter;                           // ショットモーション中のフレームカウンター

		private static sprites Sprites;                             // 各スプライト
		private struct sprites                                      // 各スプライト管理構造体
		{
			public Sprite[] Neutral;                                // ニュートラル
			public Sprite StandingShoot;                            // 立ちショット
			public Sprite[] Walk;                                   // 歩き
			public Sprite[] WalkShoot;                              // 歩きショット
			public Sprite Jump;                                     // ジャンプ
			public Sprite JumpShoot;                                // ジャンプショット
			public Sprite Sliding;                                  // スライディング
			public Sprite[] Ladder;                                 // はしご掴まり
			public Sprite LadderShoot;                              // はしご掴まりショット
			public Sprite LadderBend;                               // はしご登りかけ
			public Sprite[] Damage;                                 // 被ダメージ
		}

		private Statuses Status;                                    // 状態
		private enum Statuses                                       // 各状態名
		{
			Neutral,                                                // ニュートラル
			Walk,                                                   // 歩き
			Jump,                                                   // ジャンプ
			Sliding,                                                // スライディング
			Ladder,                                                 // はしご掴まり中
			Damage                                                  // 被ダメージ
		}

		private HitboxesStruct Hitboxes;                            // 状態ごとの当たり判定ボックス (右向き, 相対)
		private struct HitboxesStruct                               // 状態ごとの当たり判定ボックス構造体
		{
			public Rectangle Neutral;                               // ニュートラル (立ち,  歩き, ジャンプ, はしご掴まり, 被ダメージ)
			public Rectangle Sliding;                               // スライディング
		}

		// 以下定数
		private const float WalkSpeed = 1.35f;                      // 歩行速度
		private const float JumpSpeed = -4.8f;                      // ジャンプの初速
		private const float LadderSpeed = 1.3f;                     // はしご昇降速度
		private const float SlidingSpeed = 2.5f;                    // スライディング速度
		private const int SlidingDuration = 24;                     // スライディングの長さ (フレーム数)
		private const int InvincibleBlinkDuration = 60;             // 被ダメージ後の無敵点滅の長さ (フレーム数)
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
		/// <param name="SpawnPoint">開始位置 (マップ上のマス数)</param>
		public void Initialize(Point SpawnPositionOnMap)
		{
			Health = 8;
			IsAlive = true;
			Position.X = SpawnPositionOnMap.X * Const.MapchipTileSize + Const.MapchipTileSize / 2;
			Position.Y = SpawnPositionOnMap.Y * Const.MapchipTileSize + Const.MapchipTileSize - 1;
			MoveDistance = Vector2.Zero;
			Map.SetSectionID(0);

			// 当たり判定の定義
			Hitboxes = new HitboxesStruct();
			Hitboxes.Neutral = new Rectangle(-5, -23, 12, 24);      // ニュートラル
			Hitboxes.Sliding = new Rectangle(-5, -14, 12, 15);      // スライディング
		}

		/// <summary>
		/// リソースの確保
		/// </summary>
		public static void LoadContent(ContentManager Content)
		{
			Texture = Content.Load<Texture2D>("Image/Player/Player.png");

			#region 各スプライトの定義

			// ニュートラル
			Sprites.Neutral = new Sprite[]
			{
				new Sprite(new Rectangle(32 * 1, 32, 32, 32), new Vector2(15, 30)),
				new Sprite(new Rectangle(32 * 2, 32, 32, 32), new Vector2(15, 30)),
			};
			// 立ちショット
			Sprites.StandingShoot = new Sprite(new Rectangle(32 * 1, 32 * 2, 32, 32), new Vector2(11, 30));
			// 歩き
			Sprites.Walk = new Sprite[]
			{
				new Sprite(new Rectangle(32 * 4, 32, 32, 32), new Vector2(15, 30)),
				new Sprite(new Rectangle(32 * 5, 32, 32, 32), new Vector2(15, 30)),
				new Sprite(new Rectangle(32 * 6, 32, 32, 32), new Vector2(15, 30)),
				new Sprite(new Rectangle(32 * 5, 32, 32, 32), new Vector2(15, 30)),
			};
			// 歩きショット
			Sprites.WalkShoot = new Sprite[]
			{
				new Sprite(new Rectangle(32 * 4, 32 * 2, 32, 32), new Vector2(15, 30)),
				new Sprite(new Rectangle(32 * 5, 32 * 2, 32, 32), new Vector2(15, 30)),
				new Sprite(new Rectangle(32 * 6, 32 * 2, 32, 32), new Vector2(15, 30)),
				new Sprite(new Rectangle(32 * 5, 32 * 2, 32, 32), new Vector2(15, 30)),
			};
			// ジャンプ
			Sprites.Jump = new Sprite(new Rectangle(32 * 7, 32, 32, 32), new Vector2(15, 24));
			// ジャンプショット
			Sprites.JumpShoot = new Sprite(new Rectangle(32 * 7, 32 * 2, 32, 32), new Vector2(15, 24));
			// スライディング
			Sprites.Sliding = new Sprite(new Rectangle(0, 32, 32, 32), new Vector2(15, 28));
			// はしご掴まり
			Sprites.Ladder = new Sprite[]
			{
				new Sprite(new Rectangle(32 * 9, 32, 32, 32), new Vector2(16, 30)),
				new Sprite(new Rectangle(32 * 10, 32, 32, 32), new Vector2(16, 30)),
			};
			// はしご捕まりショット
			Sprites.LadderShoot = new Sprite(new Rectangle(32 * 9, 32 * 2, 32, 32), new Vector2(16, 30));
			// はしご登りかけ
			Sprites.LadderBend = new Sprite(new Rectangle(32 * 11, 32, 32, 32), new Vector2(16, 30));
			// 被ダメージ
			Sprites.Damage = new Sprite[]
			{
				new Sprite(new Rectangle(32 * 8, 32 * 1, 32, 32), new Vector2(15, 30)),
				new Sprite(new Rectangle(32 * 8, 32 * 2, 32, 32), new Vector2(15, 30)),
			};

			#endregion
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
			if (Position.Y > Map.Size.Height * Const.MapchipTileSize)
			{
				Destroy();
			}

			// スタートボタンで向きを変更
			if (Controller.IsButtonPressed(Controller.Buttons.Start))
			{
				IsFaceToLeft = !IsFaceToLeft;
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
						Vector2 NewPosition = new Vector2(Camera.Position.X + MouseState.X / Global.WindowScale, Camera.Position.Y + MouseState.Y / Global.WindowScale + RelativeHitbox.Height / 2);
						SetPosition(NewPosition);
					}
				}
			}
			#endregion

			// セクション移動中および更新停止中は処理を行わない
			if (!IsInChangeSection && !IsStop)
			{
				// 状態に応じて更新処理を場合分け
				switch (Status)
				{
					case Statuses.Neutral:      // ニュートラル
					case Statuses.Walk:         // 歩き
					case Statuses.Jump:         // ジャンプ
						UpdateStandard();
						break;
					case Statuses.Sliding:      // スライディング
						UpdateSliding();
						break;
					case Statuses.Ladder:       // はしご掴まり
						UpdateLadder();
						break;
					case Statuses.Damage:       // 被ダメージ
						UpdateDamage();
						break;
				}
			}

			// 当たり判定を更新
			HitboxManagement();

			// ベースを更新
			base.Update(GameTime);

			// 無敵時間の管理
			InvincibleDurationManager();

			// セクション移動管理
			if (!IsInChangeSection)
			{
				// 別のセクションに触れていれば移動する
				CheckChangeSection();
			}
			// セクション移動中の処理
			else
			{
				UpdateChangeSection();
			}

			if (!IsInChangeSection)
			{
				FrameCounter++;
			}
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
						CurrentlySprite = IsShooting ? Sprites.StandingShoot : Sprites.Neutral[AnimationPattern];
					}
					else if (Status == Statuses.Walk)
					{
						CurrentlySprite = IsShooting ? Sprites.WalkShoot[AnimationPattern] : Sprites.Walk[AnimationPattern];
					}
					// ジャンプ
					else if (Status == Statuses.Jump)
					{
						CurrentlySprite = IsShooting ? Sprites.JumpShoot : Sprites.Jump;
					}
					// スライディング
					else if (Status == Statuses.Sliding)
					{
						CurrentlySprite = Sprites.Sliding;
					}
					// はしご掴まり中
					else if (Status == Statuses.Ladder)
					{
						// ショット中かどうか
						if (IsShooting)
						{
							CurrentlySprite = Sprites.LadderShoot;
						}
						else
						{
							CurrentlySprite = IsLadderBend ? Sprites.LadderBend : CurrentlySprite = Sprites.Ladder[AnimationPattern];
						}
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
					float LayerDepth = Const.DrawOrder.Player.ToLayerDepth();
					// 左を向いている場合は基点座標を左右反転
					if (IsFaceToLeft)
					{
						Origin = new Vector2(SourceRectangle.Width - 1 - Origin.X, Origin.Y);
					}
					SpriteBatch.Draw(Texture, Position, SourceRectangle, Color.White, 0.0f, Origin, 1.0f, SpriteEffect, LayerDepth);
				}
			}

			base.Draw(GameTime, SpriteBatch);
		}

		/// <summary>
		/// ダメージを受ける
		/// </summary>
		/// <param name="Damage">ダメージ量</param>
		public override bool GiveDamage(DamageDetail DamageDetail)
		{
			if (IsAlive && !IsInvincible)
			{
				base.GiveDamage(DamageDetail);

				if (Health <= 0)
				{
					Destroy();
				}
				else
				{
					IsShooting = false;
					IsInvincible = true;
					MoveDistance.Y = 0;
					SetStatus(Statuses.Damage);
				}
			}
			return true;
		}

		#region プライベート関数

		/// <summary>
		/// 当たり判定ボックスの管理
		/// </summary>
		private void HitboxManagement()
		{
			Rectangle NewHitbox;

			// ステータスに応じてヒットボックスを調整
			switch (Status)
			{
				case Statuses.Neutral:  // ニュートラル
				case Statuses.Walk:     // 歩き
				case Statuses.Jump:     // ジャンプ
				case Statuses.Ladder:   // はしご掴まり
				case Statuses.Damage:   // 被ダメージ
				default:                // その他
					NewHitbox = Hitboxes.Neutral;
					break;
				case Statuses.Sliding:  // スライディング
					NewHitbox = Hitboxes.Sliding;
					break;
			}

			// 左を向いている場合はヒットボックスを左右反転
			if (IsFaceToLeft)
			{
				NewHitbox = new Rectangle(1 - (NewHitbox.X + NewHitbox.Width), NewHitbox.Y, NewHitbox.Width, NewHitbox.Height);
			}
			RelativeHitbox = NewHitbox;
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
				if (InvincibleBlinkFrame <= 0)
				{
					InvincibleBlinkFrame = 0;
					IsInvincible = false;
				}
				else
				{
					InvincibleBlinkFrame--;
					IsInvincible = true;
				}
			}
		}

		/// <summary>
		/// 通常の更新処理
		/// </summary>
		private void UpdateStandard()
		{
			// ショット開始
			if (Controller.IsButtonPressed(Controller.Buttons.B) && RockBuster.Count < RockBuster.MaxNumber)
			{
				if (IsFaceToLeft)
				{
					Point ShotPosition = GetDrawPosition() + new Point(-16, -11);
					Entity.AddReserv(Entity.Names.RockBuster1_Left, ShotPosition);
				}
				else
				{
					Point ShotPosition = GetDrawPosition() + new Point(16, -11);
					Entity.AddReserv(Entity.Names.RockBuster1_Right, ShotPosition);
				}
				IsShooting = true;
				ShootingFrameCounter = 0;
			}

			// 接地している場合
			if (!IsInAir)
			{
				// スライディング開始
				if (Controller.IsButtonDown(Controller.Buttons.Down) && Controller.IsButtonPressed(Controller.Buttons.A))
				{
					// 目の前に壁がないかチェック
					Rectangle AbsoluteHitbox = GetAbsoluteHitbox();
					Point HitCheckPosition = IsFaceToLeft ? new Point(AbsoluteHitbox.Left - 1, AbsoluteHitbox.Bottom - 1) : new Point(AbsoluteHitbox.Right, AbsoluteHitbox.Bottom - 1);
					if (Map.PositionToTerrainType(HitCheckPosition) != Map.TerrainTypes.Wall)
					{
						// 煙エフェクトを発生させる
						if (IsFaceToLeft)
						{
							Point EffectPosition = GetDrawPosition() + new Point(6, 0);
							Entity.AddReserv(Entity.Names.SlidingSmoke_Left, EffectPosition);
						}
						else
						{
							Point EffectPosition = GetDrawPosition() + new Point(-6, 0);
							Entity.AddReserv(Entity.Names.SlidingSmoke_Right, EffectPosition);
						}
						IsShooting = false;
						SetStatus(Statuses.Sliding);
						return;
					}
				}
				// ジャンプ開始
				else if (Controller.IsButtonPressed(Controller.Buttons.A))
				{
					SetStatus(Statuses.Jump);
					MoveDistance.Y = JumpSpeed;
					IsInAir = true;
				}
				// ハシゴに捕まる
				else if (Controller.IsButtonDown(Controller.Buttons.Up) && CheckGrabLadder())
				{
					Point DrawPosition = GetDrawPosition();
					Vector2 NewPosition = new Vector2(DrawPosition.X / Const.MapchipTileSize * Const.MapchipTileSize + Const.MapchipTileSize / 2, DrawPosition.Y);
					GrabLadder(NewPosition);
					return;
				}
				// 足元のハシゴに捕まる
				else if (Controller.IsButtonDown(Controller.Buttons.Down) && CheckBottomLadder())
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
				if (MoveDistance.Y < 0 && Controller.IsButtonUp(Controller.Buttons.A))
				{
					MoveDistance.Y = 0;
				}
				// はしごに捕まる
				if ((Controller.IsButtonDown(Controller.Buttons.Up) || Controller.IsButtonDown(Controller.Buttons.Down)) && CheckGrabLadder())
				{
					Point DrawPosition = GetDrawPosition();
					Vector2 NewPosition = new Vector2(DrawPosition.X / Const.MapchipTileSize * Const.MapchipTileSize + Const.MapchipTileSize / 2, DrawPosition.Y);
					GrabLadder(NewPosition);
					return;
				}
			}

			// 左右移動
			MoveDistance.X = 0;
			if (Controller.IsButtonDown(Controller.Buttons.Left))
			{
				MoveDistance.X -= WalkSpeed;
				IsFaceToLeft = true;
			}
			else if (Controller.IsButtonDown(Controller.Buttons.Right))
			{
				MoveDistance.X += WalkSpeed;
				IsFaceToLeft = false;
			}

			// スプライト管理
			if (IsInAir)
			{
				// ジャンプ
				SetStatus(Statuses.Jump);
			}
			else if (MoveDistance.X != 0)
			{
				// 歩きモーション
				SetStatus(Statuses.Walk);
				if (FrameCounter % 8 == 0)
				{
					AnimationPattern++;
					AnimationPattern = AnimationPattern % Sprites.Walk.Length;
				}
			}
			else if (MoveDistance.X == 0)
			{
				// ニュートラルモーション
				SetStatus(Statuses.Neutral);
				int FrameCounterLast2Digits = FrameCounter.ToLastDigits(2);
				if (FrameCounterLast2Digits >= 0 && FrameCounterLast2Digits <= 90)
				{
					AnimationPattern = 0;
				}
				else
				{
					AnimationPattern = 1;
				}
			}

			// ショットモーション管理
			if (IsShooting)
			{
				ShootingFrameCounter++;
				if (ShootingFrameCounter >= 32)
				{
					IsShooting = false;
				}
			}
		}

		/// <summary>
		/// スライディング中の更新処理
		/// </summary>
		private void UpdateSliding()
		{
			// 向いている方向に前進する
			MoveDistance.X = IsFaceToLeft ? -SlidingSpeed : SlidingSpeed;

			// 一定時間続くとキャンセル
			if (FrameCounter >= SlidingDuration && IsSlidingCancelable())
			{
				MoveDistance = Vector2.Zero;
				SetStatus(Statuses.Neutral);
				return;
			}

			// 向いている方向と反対方向を押すとキャンセル
			if (Controller.IsButtonDown(Controller.Buttons.Left))
			{
				if (!IsFaceToLeft && IsSlidingCancelable())
				{
					SetStatus(Statuses.Walk);
					return;
				}
				IsFaceToLeft = true;
			}
			else if (Controller.IsButtonDown(Controller.Buttons.Right))
			{
				if (IsFaceToLeft && IsSlidingCancelable())
				{
					SetStatus(Statuses.Walk);
					return;
				}
				IsFaceToLeft = false;
			}

			// 進行方向に壁があるとキャンセル
			if (IsFaceToLeft && IsTouchTerrain("Left") && IsSlidingCancelable())
			{
				SetStatus(Statuses.Neutral);
				return;
			}
			else if (!IsFaceToLeft && IsTouchTerrain("Right") && IsSlidingCancelable())
			{
				SetStatus(Statuses.Neutral);
				return;
			}

			// 足元に地形がなくなるとキャンセル
			if (IsInAir)
			{
				MoveDistance = Vector2.Zero;
				SetStatus(Statuses.Jump);
				return;
			}

			// ジャンプボタンが押されるとジャンプキャンセル
			if (Controller.IsButtonPressed(Controller.Buttons.A) && IsSlidingCancelable())
			{
				SetStatus(Statuses.Jump);
				MoveDistance.Y = JumpSpeed;
				IsInAir = true;
				return;
			}
		}

		/// <summary>
		/// はしご掴まり中の処理
		/// </summary>
		private void UpdateLadder()
		{
			MoveDistance = Vector2.Zero;

			// ショット開始
			if (Controller.IsButtonPressed(Controller.Buttons.B) && RockBuster.Count < 3)
			{
				// 押している方向に向く
				if (Controller.IsButtonDown(Controller.Buttons.Left))
				{
					IsFaceToLeft = true;
				}
				else if (Controller.IsButtonDown(Controller.Buttons.Right))
				{
					IsFaceToLeft = false;
				}
				if (IsFaceToLeft)
				{
					Point ShotPosition = Position.ToPoint() + new Point(-8, -10);
					Entity.AddReserv(Entity.Names.RockBuster1_Left, ShotPosition);
				}
				else
				{
					Point ShotPosition = Position.ToPoint() + new Point(8, -10);
					Entity.AddReserv(Entity.Names.RockBuster1_Right, ShotPosition);
				}
				IsShooting = true;
				ShootingFrameCounter = 0;
			}

			// ジャンプが押されたらはしごを離す
			if (Controller.IsButtonPressed(Controller.Buttons.A) && Controller.IsButtonUp(Controller.Buttons.Up) && Controller.IsButtonUp(Controller.Buttons.Down))
			{
				IsShooting = false;
				IsInAir = true;
				SetStatus(Statuses.Jump);
			}

			// ショット中は昇降できない
			if (!IsShooting)
			{
				// 昇降移動
				IsLadderBend = false;
				if (Controller.IsButtonDown(Controller.Buttons.Up))
				{
					MoveDistance.Y = -LadderSpeed;
				}
				else if (Controller.IsButtonDown(Controller.Buttons.Down))
				{
					MoveDistance.Y = LadderSpeed;
				}
				// 接地したらはしごを離す
				if (!IsInAir)
				{
					SetStatus(Statuses.Neutral);
				}
				// 掴める範囲にはしごがなければはしごを離す
				if (!CheckGrabLadder())
				{
					SetStatus(Statuses.Jump);
				}
				// 登りかけかどうかを調べる
				Point DrawPosition = GetDrawPosition();
				Point LadderBendCheckPoint = new Point(DrawPosition.X, DrawPosition.Y - 16);
				Point LadderBendCheckPoint2 = new Point(DrawPosition.X, DrawPosition.Y + RelativeHitbox.Y);
				if (Map.PositionToTerrainType(LadderBendCheckPoint) != Map.TerrainTypes.Ladder &&
					Map.PositionToTerrainType(LadderBendCheckPoint2) != Map.TerrainTypes.Ladder)
				{
					IsLadderBend = true;
				}
				// はしごを登り切る
				DrawPosition = GetDrawPosition();
				Point LadderFinishCheckPoint = new Point(DrawPosition.X, DrawPosition.Y - 9);
				Point LadderFinishCheckPoint2 = new Point(DrawPosition.X, DrawPosition.Y + RelativeHitbox.Y);
				if (MoveDistance.Y < 0 &&
					Map.PositionToTerrainType(LadderFinishCheckPoint) != Map.TerrainTypes.Ladder &&
					Map.PositionToTerrainType(LadderFinishCheckPoint2) != Map.TerrainTypes.Ladder)
				{
					SetStatus(Statuses.Neutral);
					int NewPosY = (DrawPosition.Y / Const.MapchipTileSize - 1) * Const.MapchipTileSize + Const.MapchipTileSize - 1;
					SetPosY(NewPosY);
					MoveDistance.Y = 0;
					IsInAir = false;
				}

			}
			// ショットモーション管理
			else
			{
				ShootingFrameCounter++;
				if (ShootingFrameCounter >= 32)
				{
					IsShooting = false;
				}
			}

			// スプライト管理
			if (Status == Statuses.Ladder && MoveDistance.Y != 0 && FrameCounter % 8 == 0)
			{
				AnimationPattern++;
				AnimationPattern = AnimationPattern % Sprites.Ladder.Length;
			}
		}

		/// <summary>
		/// 被ダメージ中の処理
		/// </summary>
		private void UpdateDamage()
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
				InvincibleBlinkFrame = InvincibleBlinkDuration;
				if (!IsInAir)
				{
					SetStatus(Statuses.Neutral);
				}
				else
				{
					SetStatus(Statuses.Jump);
				}
			}
		}

		/// <summary>
		/// 足元の掴める位置に梯子があるかどうかを調べる
		/// </summary>
		/// <param name="Map"></param>
		/// <returns></returns>
		private bool CheckBottomLadder()
		{
			Point DrawPosition = GetDrawPosition();
			Point CheckPoint = DrawPosition; CheckPoint.Y += 1;   // プレイヤーの足元の1ドット下
			if (Map.PositionToTerrainType(CheckPoint) == Map.TerrainTypes.Ladder)
			{
				return true;
			}
			return false;
		}

		/// <summary>
		/// 掴める範囲に梯子があるかどうかを調べる
		/// </summary>
		private bool CheckGrabLadder()
		{
			Point DrawPosition = GetDrawPosition();
			Point Top = DrawPosition; Top.Y += RelativeHitbox.Y;                    // 上辺
			Point Middle = DrawPosition; Middle.Y -= RelativeHitbox.Height / 2;     // 中心
			Point Bottom = DrawPosition;                                            // 下辺
			if (Map.PositionToTerrainType(Top) == Map.TerrainTypes.Ladder ||
				Map.PositionToTerrainType(Middle) == Map.TerrainTypes.Ladder ||
				Map.PositionToTerrainType(Bottom) == Map.TerrainTypes.Ladder)
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
			IsShooting = false;
			SetPosition(AfterPosition);
			MoveDistance = Vector2.Zero;
			SetStatus(Statuses.Ladder);
			IsInAir = true;
		}

		/// <summary>
		/// スライディングをキャンセル可能かどうかを取得する (頭上に地形がないかどうか)
		/// </summary>
		private bool IsSlidingCancelable()
		{
			bool Result = false;

			Rectangle RelativeStandingHitbox = Hitboxes.Neutral;
			// 左を向いている場合はヒットボックスを左右反転
			if (IsFaceToLeft)
			{
				RelativeStandingHitbox = new Rectangle(1 - (RelativeStandingHitbox.X + RelativeStandingHitbox.Width), RelativeStandingHitbox.Y, RelativeStandingHitbox.Width, RelativeStandingHitbox.Height);
			}

			Point DrawPosition = GetDrawPosition();
			Rectangle AbsoluteStandingHitbox = new Rectangle(DrawPosition.X + RelativeStandingHitbox.X, DrawPosition.Y + RelativeStandingHitbox.Y, RelativeStandingHitbox.Width, RelativeStandingHitbox.Height);

			Point HitCheckPoint1 = new Point(AbsoluteStandingHitbox.Left, AbsoluteStandingHitbox.Top);
			Point HitCheckPoint2 = new Point(AbsoluteStandingHitbox.Right - 1, AbsoluteStandingHitbox.Top);

			if (Map.PositionToTerrainType(HitCheckPoint1) != Map.TerrainTypes.Wall &&
				Map.PositionToTerrainType(HitCheckPoint2) != Map.TerrainTypes.Wall)
			{
				Result = true;
			}
			return Result;
		}

		/// <summary>
		/// 別のセクションに触れていればそのセクションに移動する
		/// </summary>
		private void CheckChangeSection()
		{
			// 現在のセクションID
			int CurrentlySectionID = Map.CurrentlySectionID;
			// セクション配列
			List<Map.Section> Sections = Map.Sections;

			for (int i = 0; i < Sections.Count; i++)
			{
				if (i != CurrentlySectionID)
				{
					Rectangle AbsoluteHitbox = GetAbsoluteHitbox();
					Rectangle AreaRect = new Rectangle(Sections[i].Area.X * Const.MapchipTileSize, Sections[i].Area.Y * Const.MapchipTileSize, Sections[i].Area.Width * Const.MapchipTileSize, Sections[i].Area.Height * Const.MapchipTileSize);
					if (AbsoluteHitbox.Intersects(AreaRect))
					{
						Vector2 Source = Position;
						Vector2 Destination = Source;
						// 移動後の座標を移動先セクション内に収める
						{
							// 上方向にはみ出していた場合
							if (Destination.Y + RelativeHitbox.Y < AreaRect.Y)
							{
								int FitY = AreaRect.Y + 4;
								int NewPositionY = FitY - RelativeHitbox.Y;
								Destination.Y = NewPositionY;
							}
							// 下方向にはみ出していた場合
							if (Destination.Y + RelativeHitbox.Y + RelativeHitbox.Height > AreaRect.Y + AreaRect.Height)
							{
								int FitY = AreaRect.Y + AreaRect.Height - 5;
								int NewPositionY = FitY - (RelativeHitbox.Height - 1 + RelativeHitbox.Y);
								Destination.Y = NewPositionY;
							}
							// 左方向にはみ出していた場合
							if (Destination.X + RelativeHitbox.X < AreaRect.X)
							{
								int FitX = AreaRect.X + 4;
								int NewPositionX = FitX - RelativeHitbox.X;
								Destination.X = NewPositionX;
							}
							// 右方向にはみ出していた場合
							if (Destination.X + RelativeHitbox.X + RelativeHitbox.Width > AreaRect.X + AreaRect.Width)
							{
								int FitX = AreaRect.X + AreaRect.Width - 5;
								int NewPositionX = FitX - (RelativeHitbox.Width - 1 + RelativeHitbox.X);
								Destination.X = NewPositionX;
							}
						}

						Map.SetSectionID(i);

						ChangeSectionSourcePosition = Source;
						ChangeSectionDestinationPosition = Destination;
						ChangeSectionFrame = 0;
						IsInChangeSection = true;
						IsStop = true;

						// エンティティのスポーンを停止
						Map.StopEntitySpawn = true;

						// 全てのエンティティを削除
						Entity.DestroyAll();

						// カメラの移動を開始
						Point DrawPosition = GetDrawPosition();
						Camera.StartChangeSection(DrawPosition);
					}
				}
			}
		}

		/// <summary>
		/// セクション移動中の処理
		/// </summary>
		private void UpdateChangeSection()
		{
			Vector2 Source = ChangeSectionSourcePosition;
			Vector2 Destination = ChangeSectionDestinationPosition;
			int Duration = Const.ChangeSectionDuration;
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
				Map.StopEntitySpawn = false;

				// 画面内に配置された全てのエンティティをスポーンさせる
				Map.SpawnAllEntities();
			}
		}

		/// <summary>
		/// ステータスを変更
		/// </summary>
		/// <param name="Status">変更先のステータス</param>
		private void SetStatus(Statuses Status)
		{
			if (this.Status != Status)
			{
				AnimationPattern = 0;
				FrameCounter = 0;
				this.Status = Status;
			}
		}

		#endregion
	}
}
