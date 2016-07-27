using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Design;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Storage;
using System;

namespace Rockman_vs_SmashBros
{
	/// <summary>
	/// Entity クラス
	/// </summary>
	public partial class Entity
	{
		#region メンバーの宣言
		public enum Types                                           // エンティティの種類構造体
		{
			Player,
			Enemy,
			Platform
		}
		public Types Type;                                          // エンティティの種類
		public bool IsAlive;                                        // 生存フラグ
		public bool IsNoclip;                                       // 地形を貫通するかどうか
		public bool IsInAir;                                        // 空中にいるかどうか
		public bool IsStop;                                         // 更新停止フラグ
		public Vector2 Position;                                    // 内部座標
		public Vector2 OldPosition;                                 // 1フレーム前の内部座標
		public Vector2 MoveDistance;                                // 現在のフレームで移動する量
		public Rectangle RelativeCollision;                         // 当たり判定 (相対)
		public bool IsFromMap;                                      // マップにより作成されたかどうか
		public Point FromMapPosition;                               // マップにより作成された場合に、その座標を保持
		public bool IsIgnoreGravity;                                // このエンティティが重力を無視するかどうか
		public Entity RidingEntity;                                 // このエンティティが乗っているエンティティ
		public int Health;                                          // このエンティティの体力
		#endregion

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public Entity() { }

		/// <summary>
		/// 初期化
		/// </summary>
		public virtual void Initialize() { }

		/// <summary>
		/// フレームの更新
		/// </summary>
		public virtual void Update(GameTime GameTime)
		{
			if (IsAlive && !IsStop)
			{
				// エンティティに乗っている場合は Y座標を合わせて Xの移動量を吸収
				if (RidingEntity != null)
				{
					int FitY = RidingEntity.GetAbsoluteCollision().Top - 1;
					int NewPositionY = FitY - (RelativeCollision.Height - 1 + RelativeCollision.Y);
					SetPosY(NewPositionY);
					MoveDistance.X += RidingEntity.MoveDistance.X;
				}

				MoveY();
				MoveX();
				if (IsInAir && !IsNoclip)
				{
					RidingEntity = null;
					if (!IsIgnoreGravity)
					{
						MoveDistance.Y += Global.Gravity;
					}
				}
				else
				{
					CheckInAir();
				}
			}

			// OldPosition を更新
			OldPosition = Position;
		}

		/// <summary>
		/// 描画
		/// </summary>
		public virtual void Draw(GameTime GameTime, SpriteBatch SpriteBatch)
		{
			if (IsAlive)
			{
				// デバッグ描画
				if (Global.Debug)
				{
					Point DrawPosition = GetDrawPosition();
					Rectangle AbsoluteCollision = GetAbsoluteCollision();
					SpriteBatch.DrawRectangle(new Rectangle(AbsoluteCollision.X, AbsoluteCollision.Y, AbsoluteCollision.Width, AbsoluteCollision.Height), Color.Blue * 0.2f, true);
					SpriteBatch.DrawRectangle(new Rectangle(AbsoluteCollision.X, AbsoluteCollision.Y, AbsoluteCollision.Width, AbsoluteCollision.Height), Color.Blue);
					SpriteBatch.DrawPixel(DrawPosition.ToVector2(), Color.Red);
					SpriteBatch.DrawString(Main.Font, Position.ToString(), new Vector2(DrawPosition.X, DrawPosition.Y - 8), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 1);
				}
			}
		}

		/// <summary>
		/// エンティティを削除
		/// </summary>
		public virtual void Destroy()
		{
			IsAlive = false;
		}

		/// <summary>
		/// エンティティにダメージを与える
		/// </summary>
		/// <param name="DamageDetail">与えるダメージの内容</param>
		/// <returns>ダメージが有効であったかどうかを返す。</returns>
		public virtual bool GiveDamage(DamageDetail DamageDetail)
		{
			Health -= DamageDetail.Damage;
            if (Health <= 0)
            {
                Destroy();
            }
			return true;
		}
	
		/// <summary>
		/// 指定した座標へ移動
		/// </summary>
		/// このメソッドは地形判定を考慮しません。
		public void SetPosition(Vector2 Position)
		{
			this.Position = Position;
		}

		/// <summary>
		/// 指定したX座標へ移動
		/// </summary>
		/// このメソッドは地形判定を考慮しません。
		public void SetPosX(float PosX)
		{
			Position.X = PosX;
		}

		/// <summary>
		/// 指定したY座標へ移動
		/// </summary>
		public void SetPosY(float PosY)
		{
			Position.Y = PosY;
		}

		/// <summary>
		/// 乗っているエンティティを文字列で取得
		/// </summary>
		public string GetRidingEntityString()
		{
			string Result = "null";
			if (RidingEntity != null)
			{
				Result = RidingEntity.ToString();
			}
			return Result;
		}

		/// <summary>
		/// AbsoluteCollision (ワールドに対する絶対座標での当たり判定) を取得
		/// </summary>
		public Rectangle GetAbsoluteCollision()
		{
			Point DrawPosition = GetDrawPosition();
			Rectangle AbsoluteCollision = new Rectangle(DrawPosition.X + RelativeCollision.X, DrawPosition.Y + RelativeCollision.Y, RelativeCollision.Width, RelativeCollision.Height);
			return AbsoluteCollision;
		}

		/// <summary>
		/// 1フレーム前の AbsoluteCollision (ワールドに対する絶対座標での当たり判定) を取得
		/// </summary>
		public Rectangle GetOldAbsoluteCollision()
		{
			Point OldDrawPosition = GetOldDrawPosition();
			Rectangle OldAbsoluteCollision = new Rectangle(OldDrawPosition.X + RelativeCollision.X, OldDrawPosition.Y + RelativeCollision.Y, RelativeCollision.Width, RelativeCollision.Height);
			return OldAbsoluteCollision;
		}

		/// <summary>
		/// DrawPosition を取得
		/// </summary>
		public Point GetDrawPosition()
		{
			return Position.ToPoint();
		}

		/// <summary>
		/// 1フレーム前の DrawPosition を取得
		/// </summary>
		/// <returns></returns>
		public Point GetOldDrawPosition()
		{
			return OldPosition.ToPoint();
		}

		#region プライベート関数

		/// <summary>
		/// X 方向の移動、地形判定
		/// </summary>
		/// <param name="Map">地形判定を行う場合の対象となるマップ</param>
		private void MoveX()
		{
			// 移動量を反映
			Position.X += MoveDistance.X;
			// 現在のセクション
			Map.Section CurrentlySection = Map.Sections[Map.CurrentlySectionID];
			// ワールドに対する絶対座標での当たり判定
			Rectangle AbsoluteCollision;

			// 地形判定を行う場合
			if (!IsNoclip)
			{
				// 右側地形判定
				if (MoveDistance.X > 0)
				{
					// 当たり判定をスライスする個数 (マップチップ1枚のサイズごとにスライス)
					AbsoluteCollision = GetAbsoluteCollision();
					int SplitNumber = (int)Math.Ceiling((float)AbsoluteCollision.Height / Const.MapchipTileSize) + 1;
					for (int i = 0; i < SplitNumber; i++)
					{
						Point HitCheckPosition;
						if (i == SplitNumber - 1)
						{
							HitCheckPosition = new Point(AbsoluteCollision.Right - 1, AbsoluteCollision.Bottom - 1);
						}
						else
						{
							HitCheckPosition = new Point(AbsoluteCollision.Right - 1, AbsoluteCollision.Y + Const.MapchipTileSize * i);
						}
						// 判定実行
						if (Map.PointToCollisionIndex(HitCheckPosition) == Map.CollisionTypes.Wall)
						{
							// 接触していた地形にギリギリ接触しない位置に押し戻す
							int FitX = (HitCheckPosition.X / Const.MapchipTileSize * Const.MapchipTileSize) - 1;
							int NewPositionX = FitX - (RelativeCollision.Width - 1 + RelativeCollision.X);
							Position.X = NewPositionX;
							MoveDistance.X = 0;
						}
					}
					// 重力に従うエンティティが接地中に移動した場合、足元のスロープに移動後の高さを合わせる
					if (!IsInAir && !IsIgnoreGravity)
					{
						// 当たり判定を更新
						AbsoluteCollision = GetAbsoluteCollision();

						Point HitCheckPosition = new Point(AbsoluteCollision.Left, AbsoluteCollision.Bottom + 2);
						Map.CollisionTypes Index = Map.PointToCollisionIndex(HitCheckPosition);
						if (Map.IsSlope(Index, "right"))
						{
							Point HitCheckPositionInTile = new Point(HitCheckPosition.X - HitCheckPosition.X / Const.MapchipTileSize * Const.MapchipTileSize, HitCheckPosition.Y - HitCheckPosition.Y / Const.MapchipTileSize * Const.MapchipTileSize);
							int FloorY = Map.GetSlopeFloorY(Index, HitCheckPositionInTile.X);
							if (HitCheckPositionInTile.Y >= FloorY)
							{
								int FitY = (HitCheckPosition.Y / Const.MapchipTileSize * Const.MapchipTileSize) + FloorY - 1;
								int NewPositionY = FitY - (RelativeCollision.Height - 1 + RelativeCollision.Y);
								Position.Y = NewPositionY;
								MoveDistance.Y = 0;
								IsInAir = false;
							}
						}
						// スロープから降りる時
						else if (Index == Map.CollisionTypes.Wall)
						{
							int FitY = (HitCheckPosition.Y / Const.MapchipTileSize * Const.MapchipTileSize) - 1;
							int NewPositionY = FitY - (RelativeCollision.Height - 1 + RelativeCollision.Y);
							Position.Y = NewPositionY;
							MoveDistance.Y = 0;
							IsInAir = false;
						}
					}
				}
				// 左側地形判定
				else if (MoveDistance.X < 0)
				{
					// 当たり判定をスライスする個数 (マップチップ1枚のサイズごとにスライス)
					AbsoluteCollision = GetAbsoluteCollision();
					int SplitNumber = (int)Math.Ceiling((float)AbsoluteCollision.Height / Const.MapchipTileSize) + 1;
					for (int i = 0; i < SplitNumber; i++)
					{
						Point HitCheckPosition;
						if (i == SplitNumber - 1)
						{
							HitCheckPosition = new Point(AbsoluteCollision.X, AbsoluteCollision.Bottom - 1);
						}
						else
						{
							HitCheckPosition = new Point(AbsoluteCollision.X, AbsoluteCollision.Y + Const.MapchipTileSize * i);
						}
						// 判定実行
						if (Map.PointToCollisionIndex(HitCheckPosition) == Map.CollisionTypes.Wall)
						{
							// 接触していた地形にギリギリ接触しない位置に押し戻す
							int FitX = (AbsoluteCollision.X) / Const.MapchipTileSize * Const.MapchipTileSize + (Const.MapchipTileSize);
							int NewPositionX = FitX - RelativeCollision.X;
							Position.X = NewPositionX;
							MoveDistance.X = 0;
						}
					}
					// 重力に従うエンティティが接地中に移動した場合、足元のスロープに移動後の高さを合わせる
					if (!IsInAir && !IsIgnoreGravity)
					{
						// 当たり判定を更新
						AbsoluteCollision = GetAbsoluteCollision();

						Point HitCheckPosition = new Point(AbsoluteCollision.Right - 1, AbsoluteCollision.Bottom + 2);
						Map.CollisionTypes Index = Map.PointToCollisionIndex(HitCheckPosition);
						if (Map.IsSlope(Index, "left"))
						{
							Point HitCheckPositionInTile = new Point(HitCheckPosition.X - HitCheckPosition.X / Const.MapchipTileSize * Const.MapchipTileSize, HitCheckPosition.Y - HitCheckPosition.Y / Const.MapchipTileSize * Const.MapchipTileSize);
							int FloorY = Map.GetSlopeFloorY(Index, HitCheckPositionInTile.X);
							if (HitCheckPositionInTile.Y >= FloorY)
							{
								int FitY = (HitCheckPosition.Y / Const.MapchipTileSize * Const.MapchipTileSize) + FloorY - 1;
								int NewPositionY = FitY - (RelativeCollision.Height - 1 + RelativeCollision.Y);
								Position.Y = NewPositionY;
								MoveDistance.Y = 0;
								IsInAir = false;
							}
						}
						// スロープから降りる時
						else if (Index == Map.CollisionTypes.Wall)
						{
							int FitY = (HitCheckPosition.Y / Const.MapchipTileSize * Const.MapchipTileSize) - 1;
							int NewPositionY = FitY - (RelativeCollision.Height - 1 + RelativeCollision.Y);
							Position.Y = NewPositionY;
							MoveDistance.Y = 0;
							IsInAir = false;
						}
					}

				}
				// 現在のセクションからはみ出た場合にその方向が壁属性であれば押し戻す
				AbsoluteCollision = GetAbsoluteCollision();
				if (CurrentlySection.RightIsWall && AbsoluteCollision.Right > Const.MapchipTileSize * CurrentlySection.Area.X + Const.MapchipTileSize * CurrentlySection.Area.Width)
				{
					int FitX = Const.MapchipTileSize * CurrentlySection.Area.X + Const.MapchipTileSize * CurrentlySection.Area.Width - 1;
					int NewPositionX = FitX - (RelativeCollision.Width - 1 + RelativeCollision.X);
					Position.X = NewPositionX;
					MoveDistance.X = 0;
				}
				else if (CurrentlySection.LeftIsWall && AbsoluteCollision.X < Const.MapchipTileSize * CurrentlySection.Area.X)
				{
					int FitX = Const.MapchipTileSize * CurrentlySection.Area.X;
					int NewPositionX = FitX - RelativeCollision.X;
					Position.X = NewPositionX;
					MoveDistance.X = 0;
				}
			}
		}

		/// <summary>
		/// Y 方向の移動、地形判定
		/// </summary>
		/// <param name="Map">地形判定を行う場合の対象となるマップ</param>
		private void MoveY()
		{
			// 移動量を反映
			Position.Y += MoveDistance.Y;
			// 現在のセクション
			Map.Section CurrentlySection = Map.Sections[Map.CurrentlySectionID];
			// ワールドに対する絶対座標での当たり判定
			Rectangle AbsoluteCollision;
			// 1フレーム前の AbsoluteCollision
			Rectangle OldAbsoluteCollision;

			// 地形判定を行う場合
			if (!IsNoclip)
			{
				// 下側地形判定
				if (MoveDistance.Y > 0)
				{
					// 当たり判定をスライスする個数 (マップチップ1枚のサイズごとにスライス)
					AbsoluteCollision = GetAbsoluteCollision();
					OldAbsoluteCollision = GetOldAbsoluteCollision();
					int SplitNumber = (int)Math.Ceiling((float)AbsoluteCollision.Width / Const.MapchipTileSize) + 1;
					for (int i = 0; i < SplitNumber; i++)
					{
						Point HitCheckPosition;
						if (i == SplitNumber - 1)
						{
							HitCheckPosition = new Point(AbsoluteCollision.Right - 1, AbsoluteCollision.Bottom - 1);
						}
						else
						{
							HitCheckPosition = new Point(AbsoluteCollision.X + Const.MapchipTileSize * i, AbsoluteCollision.Bottom - 1);
						}
						// 地形判定実行, 梯子の上辺だった場合も着地したことにする
						Map.CollisionTypes Index = Map.PointToCollisionIndex(HitCheckPosition);
						if (Index == Map.CollisionTypes.Wall ||
							Index == Map.CollisionTypes.OneWay && OldAbsoluteCollision.Bottom - 1 < HitCheckPosition.Y / Const.MapchipTileSize * Const.MapchipTileSize ||
							Map.CheckPointLadderTop(HitCheckPosition) && OldAbsoluteCollision.Bottom - 1 < HitCheckPosition.Y / Const.MapchipTileSize * Const.MapchipTileSize)
						{
							// 接触した地形にギリギリ接触しない位置に移動する
							int FitY = (HitCheckPosition.Y / Const.MapchipTileSize * Const.MapchipTileSize) - 1;
							int NewPositionY = FitY - (RelativeCollision.Height - 1 + RelativeCollision.Y);
							Position.Y = NewPositionY;
							MoveDistance.Y = 0;
							IsInAir = false;
						}
						// Platform エンティティとの当たり判定
						else if (RidingEntity == null)
						{
							// Platform が Platform に乗ることはない
							if (Type != Types.Platform)
							{
								foreach (Entity Entity in Main.Entities)
								{
									// エンティティの属性が Platform の場合
									if (Entity.Type == Types.Platform && Entity.GetAbsoluteCollision().Contains(HitCheckPosition) && Entity != this &&
										OldAbsoluteCollision.Bottom - 1 <= Entity.GetAbsoluteCollision().Top)
									{
										// 接触したエンティティにギリギリ接触しない位置に移動する
										int FitY = Entity.GetAbsoluteCollision().Top - 1;
										int NewPositionY = FitY - (RelativeCollision.Height - 1 + RelativeCollision.Y);
										Position.Y = NewPositionY;
										MoveDistance.Y = 0;
										IsInAir = false;
										RidingEntity = Entity;
										break;
									}
								}
							}
						}
					}
				}
				// 上側地形判定
				else if (MoveDistance.Y < 0)
				{
					// 当たり判定をスライスする個数 (マップチップ1枚のサイズごとにスライス)
					AbsoluteCollision = GetAbsoluteCollision();
					int SplitNumber = (int)Math.Ceiling((float)AbsoluteCollision.Width / Const.MapchipTileSize) + 1;
					for (int i = 0; i < SplitNumber; i++)
					{
						Point HitCheckPosition;
						if (i == SplitNumber - 1)
						{
							HitCheckPosition = new Point(AbsoluteCollision.Right - 1, AbsoluteCollision.Y);
						}
						else
						{
							HitCheckPosition = new Point(AbsoluteCollision.X + Const.MapchipTileSize * i, AbsoluteCollision.Y);
						}
						// 判定実行
						Map.CollisionTypes Index = Map.PointToCollisionIndex(HitCheckPosition);
						if (Index == Map.CollisionTypes.Wall ||
							Map.IsSlope(Index))
						{
							// 接触した地形にギリギリ接触しない位置に移動する
							int FitY = HitCheckPosition.Y / Const.MapchipTileSize * Const.MapchipTileSize + (Const.MapchipTileSize);
							int NewPositionY = FitY - RelativeCollision.Y;
							Position.Y = NewPositionY;
							MoveDistance.Y = 0;
						}
					}
				}
				// 現在のセクションからはみ出た場合にその方向が壁属性であれば押し戻す
				AbsoluteCollision = GetAbsoluteCollision();
				if (CurrentlySection.BottomIsWall && AbsoluteCollision.Bottom > Const.MapchipTileSize * CurrentlySection.Area.Y + Const.MapchipTileSize * CurrentlySection.Area.Height)
				{
					int FitY = Const.MapchipTileSize * CurrentlySection.Area.Y + Const.MapchipTileSize * CurrentlySection.Area.Height - 1;
					int NewPositionY = FitY - (RelativeCollision.Height - 1 + RelativeCollision.Y);
					Position.Y = NewPositionY;
					MoveDistance.Y = 0;
					IsInAir = false;
				}
				else if (CurrentlySection.TopIsWall && AbsoluteCollision.Y < Const.MapchipTileSize * CurrentlySection.Area.Y)
				{
					int FitY = Const.MapchipTileSize * CurrentlySection.Area.Y;
					int NewPositionY = FitY - RelativeCollision.Y;
					Position.Y = NewPositionY;
					MoveDistance.Y = 0;
				}
				// スロープにめり込んでいた場合押し出す
				{
					AbsoluteCollision = GetAbsoluteCollision();
					int SplitNumber = (int)Math.Ceiling((float)AbsoluteCollision.Width / Const.MapchipTileSize) + 1;
					for (int i = 0; i < SplitNumber; i++)
					{
						Point HitCheckPosition;
						if (i == SplitNumber - 1)
						{
							HitCheckPosition = new Point(AbsoluteCollision.Right - 1, AbsoluteCollision.Bottom - 1);
						}
						else
						{
							HitCheckPosition = new Point(AbsoluteCollision.X + Const.MapchipTileSize * i, AbsoluteCollision.Bottom - 1);
						}
						Map.CollisionTypes Index = Map.PointToCollisionIndex(HitCheckPosition);
						if (Map.IsSlope(Index))
						{
							Point HitCheckPositionInTile = new Point(HitCheckPosition.X - HitCheckPosition.X / Const.MapchipTileSize * Const.MapchipTileSize, HitCheckPosition.Y - HitCheckPosition.Y / Const.MapchipTileSize * Const.MapchipTileSize);
							int FloorY = Map.GetSlopeFloorY(Index, HitCheckPositionInTile.X);
							if (HitCheckPositionInTile.Y >= FloorY)
							{
								int FitY = (HitCheckPosition.Y / Const.MapchipTileSize * Const.MapchipTileSize) + FloorY - 1;
								int NewPositionY = FitY - (RelativeCollision.Height - 1 + RelativeCollision.Y);
								Position.Y = NewPositionY;
								MoveDistance.Y = 0;
								IsInAir = false;
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// 接地しているかを確認する
		/// </summary>
		/// <param name="Map">地形判定を行う場合の対象となるマップ</param>
		private void CheckInAir()
		{
			IsInAir = true;
			RidingEntity = null;
			// 現在のセクション
			Map.Section CurrentlySection = Map.Sections[Map.CurrentlySectionID];
			// ワールドに対する絶対座標での当たり判定
			Rectangle AbsoluteCollision;

			if (!IsNoclip)
			{
				// セクションの端が壁属性であれば着地していることにする
				AbsoluteCollision = GetAbsoluteCollision();
				if (CurrentlySection.BottomIsWall && AbsoluteCollision.Bottom >= Const.MapchipTileSize * CurrentlySection.Area.Y + Const.MapchipTileSize * CurrentlySection.Area.Height)
				{
					IsInAir = false;
				}
				else
				{
					// 当たり判定をスライスする個数 (マップチップ1枚のサイズごとにスライス)
					int SplitNumber = (int)Math.Ceiling((float)AbsoluteCollision.Width / Const.MapchipTileSize) + 1;
					for (int i = 0; i < SplitNumber; i++)
					{
						Point HitCheckPosition;
						if (i == SplitNumber - 1)
						{
							HitCheckPosition = new Point(AbsoluteCollision.Right - 1, AbsoluteCollision.Bottom);
						}
						else
						{
							HitCheckPosition = new Point(AbsoluteCollision.X + Const.MapchipTileSize * i, AbsoluteCollision.Bottom);
						}
						// 地形判定実行
						Map.CollisionTypes Index = Map.PointToCollisionIndex(HitCheckPosition);
						if (Index == Map.CollisionTypes.Wall ||
							Index == Map.CollisionTypes.OneWay ||
							Map.CheckPointLadderTop(HitCheckPosition))
						{
							IsInAir = false;
						}
						// スロープとの当たり判定
						else if (Map.IsSlope(Index))
						{
							Point HitCheckPositionInTile = new Point(HitCheckPosition.X - HitCheckPosition.X / Const.MapchipTileSize * Const.MapchipTileSize, HitCheckPosition.Y - HitCheckPosition.Y / Const.MapchipTileSize * Const.MapchipTileSize);
							int FloorY = Map.GetSlopeFloorY(Index, HitCheckPositionInTile.X);
							if (HitCheckPositionInTile.Y >= FloorY)
							{
								IsInAir = false;
							}
						}
						// Platform エンティティとの当たり判定
						else
						{
							// Platform が Platform に乗ることはない
							if (Type != Types.Platform)
							{
								foreach (Entity Entity in Main.Entities)
								{
									// エンティティの属性が Platform の場合
									if (Entity.Type == Types.Platform && Entity.GetAbsoluteCollision().Contains(HitCheckPosition) && Entity != this)
									{
										IsInAir = false;
										RidingEntity = Entity;
										break;
									}
								}
							}
						}
					}
				}
			}
		}

		#endregion
	}
}
