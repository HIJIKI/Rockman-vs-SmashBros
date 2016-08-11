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
			Platform,
			Other
		}
		public Types Type;                                          // エンティティの種類
		public bool IsAlive;                                        // 生存フラグ
		public bool IsNoclip;                                       // 地形を貫通するかどうか
		public bool IsInAir;                                        // 空中にいるかどうか
		public bool IsStop;                                         // 更新停止フラグ
		public Vector2 Position;                                    // 内部座標
		public Vector2 OldPosition;                                 // 1フレーム前の内部座標
		public Vector2 MoveDistance;                                // 現在のフレームで移動する量
		public Rectangle RelativeHitbox;                            // 当たり判定ボックス (相対)
		public bool IsFromMap;                                      // マップにより作成されたかどうか
		public Point FromMapPosition;                               // マップにより作成された場合に、その座標を保持
		public bool IsIgnoreGravity;                                // このエンティティが重力を無視するかどうか
		public Entity RidingEntity;                                 // このエンティティが乗っているエンティティ
		public int Health;                                          // このエンティティの体力
		#endregion

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
					int FitY = RidingEntity.GetAbsoluteHitbox().Top - 1;
					int NewPositionY = FitY - (RelativeHitbox.Height - 1 + RelativeHitbox.Y);
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
					Rectangle AbsoluteHitbox = GetAbsoluteHitbox();
					SpriteBatch.DrawRectangle(new Rectangle(AbsoluteHitbox.X, AbsoluteHitbox.Y, AbsoluteHitbox.Width, AbsoluteHitbox.Height), Color.Blue * 0.2f, true);
					SpriteBatch.DrawRectangle(new Rectangle(AbsoluteHitbox.X, AbsoluteHitbox.Y, AbsoluteHitbox.Width, AbsoluteHitbox.Height), Color.Blue);
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
		/// AbsoluteHitbox (ワールドに対する絶対座標での当たり判定) を取得
		/// </summary>
		public Rectangle GetAbsoluteHitbox()
		{
			Point DrawPosition = GetDrawPosition();
			Rectangle AbsoluteHitbox = new Rectangle(DrawPosition.X + RelativeHitbox.X, DrawPosition.Y + RelativeHitbox.Y, RelativeHitbox.Width, RelativeHitbox.Height);
			return AbsoluteHitbox;
		}

		/// <summary>
		/// 1フレーム前の AbsoluteHitbox (ワールドに対する絶対座標での当たり判定) を取得
		/// </summary>
		public Rectangle GetOldAbsoluteHitbox()
		{
			Point OldDrawPosition = GetOldDrawPosition();
			Rectangle OldAbsoluteHitbox = new Rectangle(OldDrawPosition.X + RelativeHitbox.X, OldDrawPosition.Y + RelativeHitbox.Y, RelativeHitbox.Width, RelativeHitbox.Height);
			return OldAbsoluteHitbox;
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

		/// <summary>
		/// 指定した方向が地形に接触しているかどうかを取得する。Platform エンティティは対象にならない。
		/// </summary>
		/// <param name="Direction">取得したい方向を "Top", "Bottom", "Left", "Bottom" のいずれかで指定する</param>
		public bool IsTouchTerrain(string Direction)
		{
			bool Result = false;
			if (Direction == "Top")
			{
				Map.Section CurrentlySection = Map.Sections[Map.CurrentlySectionID];
				Rectangle AbsoluteHitbox = GetAbsoluteHitbox();
				// セクションの端も壁属性であった場合は地形と見なす
				if (CurrentlySection.TopIsWall && AbsoluteHitbox.Y - 1 < Const.MapchipTileSize * CurrentlySection.Area.Y)
				{
					Result = true;
				}
				else
				{
					// 当たり判定をスライスする個数 (マップチップ1枚のサイズごとにスライス)
					int SplitNumber = (int)Math.Ceiling((float)AbsoluteHitbox.Width / Const.MapchipTileSize) + 1;
					for (int i = 0; i < SplitNumber; i++)
					{
						Point HitCheckPosition;
						if (i == SplitNumber - 1)
						{
							HitCheckPosition = new Point(AbsoluteHitbox.Right - 1, AbsoluteHitbox.Y - 1);
						}
						else
						{
							HitCheckPosition = new Point(AbsoluteHitbox.X + Const.MapchipTileSize * i, AbsoluteHitbox.Y - 1);
						}
						// 判定実行
						Map.TerrainTypes Index = Map.PositionToTerrainType(HitCheckPosition);
						if (Index == Map.TerrainTypes.Wall || Map.IsSlope(Index))
						{
							Result = true;
						}
					}
				}
			}
			else if (Direction == "Bottom")
			{
				if (!IsInAir)
				{
					Result = true;
				}
				else
				{
					Map.Section CurrentlySection = Map.Sections[Map.CurrentlySectionID];
					Rectangle AbsoluteHitbox = GetAbsoluteHitbox();
					// セクションの端も壁属性であった場合は地形と見なす
					if (CurrentlySection.BottomIsWall && AbsoluteHitbox.Bottom + 1 > Const.MapchipTileSize * CurrentlySection.Area.Y + Const.MapchipTileSize * CurrentlySection.Area.Height)
					{
						Result = true;
					}
					else
					{
						// 当たり判定をスライスする個数 (マップチップ1枚のサイズごとにスライス)
						int SplitNumber = (int)Math.Ceiling((float)AbsoluteHitbox.Width / Const.MapchipTileSize) + 1;
						for (int i = 0; i < SplitNumber; i++)
						{
							Point HitCheckPosition;
							if (i == SplitNumber - 1)
							{
								HitCheckPosition = new Point(AbsoluteHitbox.Right - 1, AbsoluteHitbox.Bottom);
							}
							else
							{
								HitCheckPosition = new Point(AbsoluteHitbox.X + Const.MapchipTileSize * i, AbsoluteHitbox.Bottom);
							}
							// 判定実行
							Map.TerrainTypes Index = Map.PositionToTerrainType(HitCheckPosition);
							if (Index == Map.TerrainTypes.Wall)
							{
								Result = true;
							}
							else if (Map.IsSlope(Index))
							{
								Point HitCheckPositionInTile = new Point(HitCheckPosition.X - HitCheckPosition.X / Const.MapchipTileSize * Const.MapchipTileSize, HitCheckPosition.Y - HitCheckPosition.Y / Const.MapchipTileSize * Const.MapchipTileSize);
								int FloorY = Map.GetSlopeFloorY(Index, HitCheckPositionInTile.X);
								if (HitCheckPositionInTile.Y >= FloorY)
								{
									Result = true;
								}
							}
						}
					}
				}
			}
			else if (Direction == "Left")
			{
				Map.Section CurrentlySection = Map.Sections[Map.CurrentlySectionID];
				Rectangle AbsoluteHitbox = GetAbsoluteHitbox();
				// セクションの端も壁属性であった場合は地形と見なす
				if (CurrentlySection.LeftIsWall && AbsoluteHitbox.X - 1 < Const.MapchipTileSize * CurrentlySection.Area.X)
				{
					Result = true;
				}
				else
				{
					// 当たり判定をスライスする個数 (マップチップ1枚のサイズごとにスライス)
					int SplitNumber = (int)Math.Ceiling((float)AbsoluteHitbox.Height / Const.MapchipTileSize) + 1;
					for (int i = 0; i < SplitNumber; i++)
					{
						Point HitCheckPosition;
						if (i == SplitNumber - 1)
						{
							HitCheckPosition = new Point(AbsoluteHitbox.X - 1, AbsoluteHitbox.Bottom - 1);
						}
						else
						{
							HitCheckPosition = new Point(AbsoluteHitbox.X - 1, AbsoluteHitbox.Y + Const.MapchipTileSize * i);
						}
						// 判定実行
						if (Map.PositionToTerrainType(HitCheckPosition) == Map.TerrainTypes.Wall)
						{
							Result = true;
						}
					}
				}
			}
			else if (Direction == "Right")
			{
				Map.Section CurrentlySection = Map.Sections[Map.CurrentlySectionID];
				Rectangle AbsoluteHitbox = GetAbsoluteHitbox();
				// セクションの端も壁属性であった場合は地形と見なす
				if (CurrentlySection.RightIsWall && AbsoluteHitbox.Right + 1 > Const.MapchipTileSize * CurrentlySection.Area.X + Const.MapchipTileSize * CurrentlySection.Area.Width)
				{
					Result = true;
				}
				else
				{
					// 当たり判定をスライスする個数 (マップチップ1枚のサイズごとにスライス)
					int SplitNumber = (int)Math.Ceiling((float)AbsoluteHitbox.Height / Const.MapchipTileSize) + 1;
					for (int i = 0; i < SplitNumber; i++)
					{
						Point HitCheckPosition;
						if (i == SplitNumber - 1)
						{
							HitCheckPosition = new Point(AbsoluteHitbox.Right, AbsoluteHitbox.Bottom - 1);
						}
						else
						{
							HitCheckPosition = new Point(AbsoluteHitbox.Right, AbsoluteHitbox.Y + Const.MapchipTileSize * i);
						}
						// 判定実行
						if (Map.PositionToTerrainType(HitCheckPosition) == Map.TerrainTypes.Wall)
						{
							Result = true;
						}
					}
				}
			}
			return Result;
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
			Rectangle AbsoluteHitbox;

			// 地形判定を行う場合
			if (!IsNoclip)
			{
				// 右側地形判定
				if (MoveDistance.X > 0)
				{
					// 当たり判定をスライスする個数 (マップチップ1枚のサイズごとにスライス)
					AbsoluteHitbox = GetAbsoluteHitbox();
					int SplitNumber = (int)Math.Ceiling((float)AbsoluteHitbox.Height / Const.MapchipTileSize) + 1;
					for (int i = 0; i < SplitNumber; i++)
					{
						Point HitCheckPosition;
						if (i == SplitNumber - 1)
						{
							HitCheckPosition = new Point(AbsoluteHitbox.Right - 1, AbsoluteHitbox.Bottom - 1);
						}
						else
						{
							HitCheckPosition = new Point(AbsoluteHitbox.Right - 1, AbsoluteHitbox.Y + Const.MapchipTileSize * i);
						}
						// 判定実行
						if (Map.PositionToTerrainType(HitCheckPosition) == Map.TerrainTypes.Wall)
						{
							// 接触していた地形にギリギリ接触しない位置に押し戻す
							int FitX = (HitCheckPosition.X / Const.MapchipTileSize * Const.MapchipTileSize) - 1;
							int NewPositionX = FitX - (RelativeHitbox.Width - 1 + RelativeHitbox.X);
							Position.X = NewPositionX;
							MoveDistance.X = 0;
						}
					}
					// 重力に従うエンティティが接地中に移動した場合、足元のスロープに移動後の高さを合わせる
					if (!IsInAir && !IsIgnoreGravity)
					{
						// 当たり判定を更新
						AbsoluteHitbox = GetAbsoluteHitbox();

						Point HitCheckPosition = new Point(AbsoluteHitbox.Left, AbsoluteHitbox.Bottom + 2);
						Map.TerrainTypes Index = Map.PositionToTerrainType(HitCheckPosition);
						if (Map.IsSlope(Index, "right"))
						{
							Point HitCheckPositionInTile = new Point(HitCheckPosition.X - HitCheckPosition.X / Const.MapchipTileSize * Const.MapchipTileSize, HitCheckPosition.Y - HitCheckPosition.Y / Const.MapchipTileSize * Const.MapchipTileSize);
							int FloorY = Map.GetSlopeFloorY(Index, HitCheckPositionInTile.X);
							if (HitCheckPositionInTile.Y >= FloorY)
							{
								int FitY = (HitCheckPosition.Y / Const.MapchipTileSize * Const.MapchipTileSize) + FloorY - 1;
								int NewPositionY = FitY - (RelativeHitbox.Height - 1 + RelativeHitbox.Y);
								Position.Y = NewPositionY;
								MoveDistance.Y = 0;
								IsInAir = false;
							}
						}
						// スロープから降りる時
						else if (Index == Map.TerrainTypes.Wall)
						{
							int FitY = (HitCheckPosition.Y / Const.MapchipTileSize * Const.MapchipTileSize) - 1;
							int NewPositionY = FitY - (RelativeHitbox.Height - 1 + RelativeHitbox.Y);
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
					AbsoluteHitbox = GetAbsoluteHitbox();
					int SplitNumber = (int)Math.Ceiling((float)AbsoluteHitbox.Height / Const.MapchipTileSize) + 1;
					for (int i = 0; i < SplitNumber; i++)
					{
						Point HitCheckPosition;
						if (i == SplitNumber - 1)
						{
							HitCheckPosition = new Point(AbsoluteHitbox.X, AbsoluteHitbox.Bottom - 1);
						}
						else
						{
							HitCheckPosition = new Point(AbsoluteHitbox.X, AbsoluteHitbox.Y + Const.MapchipTileSize * i);
						}
						// 判定実行
						if (Map.PositionToTerrainType(HitCheckPosition) == Map.TerrainTypes.Wall)
						{
							// 接触していた地形にギリギリ接触しない位置に押し戻す
							int FitX = (AbsoluteHitbox.X) / Const.MapchipTileSize * Const.MapchipTileSize + (Const.MapchipTileSize);
							int NewPositionX = FitX - RelativeHitbox.X;
							Position.X = NewPositionX;
							MoveDistance.X = 0;
						}
					}
					// 重力に従うエンティティが接地中に移動した場合、足元のスロープに移動後の高さを合わせる
					if (!IsInAir && !IsIgnoreGravity)
					{
						// 当たり判定を更新
						AbsoluteHitbox = GetAbsoluteHitbox();

						Point HitCheckPosition = new Point(AbsoluteHitbox.Right - 1, AbsoluteHitbox.Bottom + 2);
						Map.TerrainTypes Index = Map.PositionToTerrainType(HitCheckPosition);
						if (Map.IsSlope(Index, "left"))
						{
							Point HitCheckPositionInTile = new Point(HitCheckPosition.X - HitCheckPosition.X / Const.MapchipTileSize * Const.MapchipTileSize, HitCheckPosition.Y - HitCheckPosition.Y / Const.MapchipTileSize * Const.MapchipTileSize);
							int FloorY = Map.GetSlopeFloorY(Index, HitCheckPositionInTile.X);
							if (HitCheckPositionInTile.Y >= FloorY)
							{
								int FitY = (HitCheckPosition.Y / Const.MapchipTileSize * Const.MapchipTileSize) + FloorY - 1;
								int NewPositionY = FitY - (RelativeHitbox.Height - 1 + RelativeHitbox.Y);
								Position.Y = NewPositionY;
								MoveDistance.Y = 0;
								IsInAir = false;
							}
						}
						// スロープから降りる時
						else if (Index == Map.TerrainTypes.Wall)
						{
							int FitY = (HitCheckPosition.Y / Const.MapchipTileSize * Const.MapchipTileSize) - 1;
							int NewPositionY = FitY - (RelativeHitbox.Height - 1 + RelativeHitbox.Y);
							Position.Y = NewPositionY;
							MoveDistance.Y = 0;
							IsInAir = false;
						}
					}

				}
				// 現在のセクションからはみ出た場合にその方向が壁属性であれば押し戻す
				AbsoluteHitbox = GetAbsoluteHitbox();
				if (CurrentlySection.RightIsWall && AbsoluteHitbox.Right > Const.MapchipTileSize * CurrentlySection.Area.X + Const.MapchipTileSize * CurrentlySection.Area.Width)
				{
					int FitX = Const.MapchipTileSize * CurrentlySection.Area.X + Const.MapchipTileSize * CurrentlySection.Area.Width - 1;
					int NewPositionX = FitX - (RelativeHitbox.Width - 1 + RelativeHitbox.X);
					Position.X = NewPositionX;
					MoveDistance.X = 0;
				}
				else if (CurrentlySection.LeftIsWall && AbsoluteHitbox.X < Const.MapchipTileSize * CurrentlySection.Area.X)
				{
					int FitX = Const.MapchipTileSize * CurrentlySection.Area.X;
					int NewPositionX = FitX - RelativeHitbox.X;
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
			Rectangle AbsoluteHitbox;
			// 1フレーム前の AbsoluteHitbox
			Rectangle OldAbsoluteHitbox;

			// 地形判定を行う場合
			if (!IsNoclip)
			{
				// 下側地形判定
				if (MoveDistance.Y > 0)
				{
					// 当たり判定をスライスする個数 (マップチップ1枚のサイズごとにスライス)
					AbsoluteHitbox = GetAbsoluteHitbox();
					OldAbsoluteHitbox = GetOldAbsoluteHitbox();
					int SplitNumber = (int)Math.Ceiling((float)AbsoluteHitbox.Width / Const.MapchipTileSize) + 1;
					for (int i = 0; i < SplitNumber; i++)
					{
						Point HitCheckPosition;
						if (i == SplitNumber - 1)
						{
							HitCheckPosition = new Point(AbsoluteHitbox.Right - 1, AbsoluteHitbox.Bottom - 1);
						}
						else
						{
							HitCheckPosition = new Point(AbsoluteHitbox.X + Const.MapchipTileSize * i, AbsoluteHitbox.Bottom - 1);
						}
						// 地形判定実行, 梯子の上辺だった場合も着地したことにする
						Map.TerrainTypes Index = Map.PositionToTerrainType(HitCheckPosition);
						if (Index == Map.TerrainTypes.Wall ||
							Index == Map.TerrainTypes.OneWay && OldAbsoluteHitbox.Bottom - 1 < HitCheckPosition.Y / Const.MapchipTileSize * Const.MapchipTileSize ||
							Map.CheckPositionLadderTop(HitCheckPosition) && OldAbsoluteHitbox.Bottom - 1 < HitCheckPosition.Y / Const.MapchipTileSize * Const.MapchipTileSize)
						{
							// 接触した地形にギリギリ接触しない位置に移動する
							int FitY = (HitCheckPosition.Y / Const.MapchipTileSize * Const.MapchipTileSize) - 1;
							int NewPositionY = FitY - (RelativeHitbox.Height - 1 + RelativeHitbox.Y);
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
									if (Entity.Type == Types.Platform && Entity.GetAbsoluteHitbox().Contains(HitCheckPosition) && Entity != this &&
										OldAbsoluteHitbox.Bottom - 1 <= Entity.GetAbsoluteHitbox().Top)
									{
										// 接触したエンティティにギリギリ接触しない位置に移動する
										int FitY = Entity.GetAbsoluteHitbox().Top - 1;
										int NewPositionY = FitY - (RelativeHitbox.Height - 1 + RelativeHitbox.Y);
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
					AbsoluteHitbox = GetAbsoluteHitbox();
					int SplitNumber = (int)Math.Ceiling((float)AbsoluteHitbox.Width / Const.MapchipTileSize) + 1;
					for (int i = 0; i < SplitNumber; i++)
					{
						Point HitCheckPosition;
						if (i == SplitNumber - 1)
						{
							HitCheckPosition = new Point(AbsoluteHitbox.Right - 1, AbsoluteHitbox.Y);
						}
						else
						{
							HitCheckPosition = new Point(AbsoluteHitbox.X + Const.MapchipTileSize * i, AbsoluteHitbox.Y);
						}
						// 判定実行
						Map.TerrainTypes Index = Map.PositionToTerrainType(HitCheckPosition);
						if (Index == Map.TerrainTypes.Wall ||
							Map.IsSlope(Index))
						{
							// 接触した地形にギリギリ接触しない位置に移動する
							int FitY = HitCheckPosition.Y / Const.MapchipTileSize * Const.MapchipTileSize + (Const.MapchipTileSize);
							int NewPositionY = FitY - RelativeHitbox.Y;
							Position.Y = NewPositionY;
							MoveDistance.Y = 0;
						}
					}
				}
				// 現在のセクションからはみ出た場合にその方向が壁属性であれば押し戻す
				AbsoluteHitbox = GetAbsoluteHitbox();
				if (CurrentlySection.BottomIsWall && AbsoluteHitbox.Bottom > Const.MapchipTileSize * CurrentlySection.Area.Y + Const.MapchipTileSize * CurrentlySection.Area.Height)
				{
					int FitY = Const.MapchipTileSize * CurrentlySection.Area.Y + Const.MapchipTileSize * CurrentlySection.Area.Height - 1;
					int NewPositionY = FitY - (RelativeHitbox.Height - 1 + RelativeHitbox.Y);
					Position.Y = NewPositionY;
					MoveDistance.Y = 0;
					IsInAir = false;
				}
				else if (CurrentlySection.TopIsWall && AbsoluteHitbox.Y < Const.MapchipTileSize * CurrentlySection.Area.Y)
				{
					int FitY = Const.MapchipTileSize * CurrentlySection.Area.Y;
					int NewPositionY = FitY - RelativeHitbox.Y;
					Position.Y = NewPositionY;
					MoveDistance.Y = 0;
				}
				// スロープにめり込んでいた場合押し出す
				{
					AbsoluteHitbox = GetAbsoluteHitbox();
					int SplitNumber = (int)Math.Ceiling((float)AbsoluteHitbox.Width / Const.MapchipTileSize) + 1;
					for (int i = 0; i < SplitNumber; i++)
					{
						Point HitCheckPosition;
						if (i == SplitNumber - 1)
						{
							HitCheckPosition = new Point(AbsoluteHitbox.Right - 1, AbsoluteHitbox.Bottom - 1);
						}
						else
						{
							HitCheckPosition = new Point(AbsoluteHitbox.X + Const.MapchipTileSize * i, AbsoluteHitbox.Bottom - 1);
						}
						Map.TerrainTypes Index = Map.PositionToTerrainType(HitCheckPosition);
						if (Map.IsSlope(Index))
						{
							Point HitCheckPositionInTile = new Point(HitCheckPosition.X - HitCheckPosition.X / Const.MapchipTileSize * Const.MapchipTileSize, HitCheckPosition.Y - HitCheckPosition.Y / Const.MapchipTileSize * Const.MapchipTileSize);
							int FloorY = Map.GetSlopeFloorY(Index, HitCheckPositionInTile.X);
							if (HitCheckPositionInTile.Y >= FloorY)
							{
								int FitY = (HitCheckPosition.Y / Const.MapchipTileSize * Const.MapchipTileSize) + FloorY - 1;
								int NewPositionY = FitY - (RelativeHitbox.Height - 1 + RelativeHitbox.Y);
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

			if (!IsNoclip)
			{
				if (IsTouchTerrain("Bottom"))
				{
					IsInAir = false;
				}
				else
				{
					// 当たり判定をスライスする個数 (マップチップ1枚のサイズごとにスライス)
					Rectangle AbsoluteHitbox = GetAbsoluteHitbox();
					int SplitNumber = (int)Math.Ceiling((float)AbsoluteHitbox.Width / Const.MapchipTileSize) + 1;
					for (int i = 0; i < SplitNumber; i++)
					{
						Point HitCheckPosition;
						if (i == SplitNumber - 1)
						{
							HitCheckPosition = new Point(AbsoluteHitbox.Right - 1, AbsoluteHitbox.Bottom);
						}
						else
						{
							HitCheckPosition = new Point(AbsoluteHitbox.X + Const.MapchipTileSize * i, AbsoluteHitbox.Bottom);
						}
						// すり抜け床に対する接地判定
						Map.TerrainTypes TerrainType = Map.PositionToTerrainType(HitCheckPosition);
						if (TerrainType == Map.TerrainTypes.OneWay || Map.CheckPositionLadderTop(HitCheckPosition) )
						{
							IsInAir = false;
						}
						// Platform に対する接地判定
						if (Type != Types.Platform)
						{
							foreach (Entity Entity in Main.Entities)
							{
								// 相手のエンティティが Platform の場合
								if (Entity.Type == Types.Platform && Entity.GetAbsoluteHitbox().Contains(HitCheckPosition) && Entity != this)
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

		#endregion
	}
}
