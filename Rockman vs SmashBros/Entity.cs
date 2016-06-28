﻿using Microsoft.Xna.Framework;
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
			Enemy
		}
		public Types Type;                                          // エンティティの種類
		public bool IsAlive;                                        // 生存フラグ
		public bool IsNoclip;                                       // 地形を貫通するかどうか
		public bool IsInAir;                                        // 空中にいるかどうか
		public Vector2 Position;                                    // 内部座標
		public Point DrawPosition;                                  // 描画座標
		public Vector2 MoveDistance;                                // 現在のフレームで移動する量
		public Rectangle RelativeCollision;                         // 当たり判定 (相対)
		public Rectangle AbsoluteCollision;                         // 当たり判定 (絶対)
		public bool IsFromMap;                                      // マップにより作成されたかどうか
		public Point FromMapPosition;                               // マップにより作成された場合に、その座標を保持
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
			if (IsAlive)
			{
				MoveY(Main.Map);
				MoveX(Main.Map);
				if (!IsInAir)
				{
					CheckInAir(Main.Map);
				}
			}
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
					UpdateDrawPosition();
					UpdateAbsoluteCollision();
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
		public virtual void Destroy(Entity Entity)
		{
			IsAlive = false;
		}

		/// <summary>
		/// 指定した座標へ移動
		/// </summary>
		/// このメソッドは地形判定を考慮しません。
		public void SetPosition(Vector2 Position)
		{
			this.Position = Position;

			// 描画座標、当たり判定を更新
			UpdateDrawPosition();
			UpdateAbsoluteCollision();
		}

		/// <summary>
		/// 指定したX座標へ移動
		/// </summary>
		/// このメソッドは地形判定を考慮しません。
		public void SetPosX(float PosX)
		{
			Position.X = PosX;

			// 描画座標、当たり判定を更新
			UpdateDrawPosition();
			UpdateAbsoluteCollision();
		}

		/// <summary>
		/// 指定したY座標へ移動
		/// </summary>
		public void SetPosY(float PosY)
		{
			Position.Y = PosY;

			// 描画座標、当たり判定を更新
			UpdateDrawPosition();
			UpdateAbsoluteCollision();
		}

		#region プライベート関数

		/// <summary>
		/// X 方向の移動、地形判定
		/// </summary>
		/// <param name="Map">地形判定を行う場合の対象となるマップ</param>
		private void MoveX(Map Map)
		{
			// 移動量を反映
			Position.X += MoveDistance.X;
			// 描画座標、当たり判定を更新
			UpdateDrawPosition();
			UpdateAbsoluteCollision();

			// 地形判定を行う場合
			if (!IsNoclip)
			{
				// 右側地形判定
				if (MoveDistance.X > 0)
				{
					// マップからはみ出た場合にその方向が壁属性であれば押し戻す
					if (Map.RightEdge.IsWall && AbsoluteCollision.X + AbsoluteCollision.Width - 1 > Const.MapchipTileSize * Map.Size.Width - 1)
					{
						int FitX = Const.MapchipTileSize * Map.Size.Width - 1;
						int NewPositionX = FitX + (DrawPosition.X - (AbsoluteCollision.X + AbsoluteCollision.Width - 1));
						Position.X = NewPositionX;
						MoveDistance.X = 0;
						// 描画座標、当たり判定を更新
						UpdateDrawPosition();
						UpdateAbsoluteCollision();
					}
					else
					{
						// 当たり判定をスライスする個数 (マップチップ1枚のサイズごとにスライス)
						int SplitNumber = (int)Math.Ceiling((float)AbsoluteCollision.Height / Const.MapchipTileSize) + 1;
						for (int i = 0; i < SplitNumber; i++)
						{
							Point HitCheckPosition;
							if (i == SplitNumber - 1)
							{
								HitCheckPosition = new Point(AbsoluteCollision.X + AbsoluteCollision.Width - 1, AbsoluteCollision.Y + AbsoluteCollision.Height - 1);
							}
							else
							{
								HitCheckPosition = new Point(AbsoluteCollision.X + AbsoluteCollision.Width - 1, AbsoluteCollision.Y + Const.MapchipTileSize * i);
							}
							// 判定実行
							if (PointToCollisionIndex(Map, HitCheckPosition) != 0)
							{
								// 接触していた地形にギリギリ接触しない位置に押し戻す
								int FitX = (HitCheckPosition.X / Const.MapchipTileSize * Const.MapchipTileSize) - 1;
								int NewPositionX = FitX + (DrawPosition.X - HitCheckPosition.X);
								Position.X = NewPositionX;
								MoveDistance.X = 0;
								// 描画座標、当たり判定を更新
								UpdateDrawPosition();
								UpdateAbsoluteCollision();
							}
						}
					}
				}
				// 左側地形判定
				else if (MoveDistance.X < 0)
				{
					// マップからはみ出た場合にその方向が壁属性であれば押し戻す
					if (Map.LeftEdge.IsWall && AbsoluteCollision.X < 0)
					{
						int FitX = 0;
						int NewPositionX = FitX + (DrawPosition.X - AbsoluteCollision.X);
						Position.X = NewPositionX;
						MoveDistance.X = 0;
						// 描画座標、当たり判定を更新
						UpdateDrawPosition();
						UpdateAbsoluteCollision();
					}
					else
					{
						// 当たり判定をスライスする個数 (マップチップ1枚のサイズごとにスライス)
						int SplitNumber = (int)Math.Ceiling((float)AbsoluteCollision.Height / Const.MapchipTileSize) + 1;
						for (int i = 0; i < SplitNumber; i++)
						{
							Point HitCheckPosition;
							if (i == SplitNumber - 1)
							{
								HitCheckPosition = new Point(AbsoluteCollision.X, AbsoluteCollision.Y + AbsoluteCollision.Height - 1);
							}
							else
							{
								HitCheckPosition = new Point(AbsoluteCollision.X, AbsoluteCollision.Y + Const.MapchipTileSize * i);
							}
							// 判定実行
							if (PointToCollisionIndex(Map, HitCheckPosition) != 0)
							{
								// 接触していた地形にギリギリ接触しない位置に押し戻す
								int FitX = (AbsoluteCollision.X) / Const.MapchipTileSize * Const.MapchipTileSize + (Const.MapchipTileSize);
								int NewPositionX = FitX + (DrawPosition.X - HitCheckPosition.X);
								Position.X = NewPositionX;
								MoveDistance.X = 0;
								// 描画座標、当たり判定を更新
								UpdateDrawPosition();
								UpdateAbsoluteCollision();
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// Y 方向の移動、地形判定
		/// </summary>
		/// <param name="Map">地形判定を行う場合の対象となるマップ</param>
		private void MoveY(Map Map)
		{
			// 移動量を反映
			Position.Y += MoveDistance.Y;
			// 描画座標、当たり判定を更新
			UpdateDrawPosition();
			UpdateAbsoluteCollision();

			// 地形判定を行う場合
			if (!IsNoclip)
			{
				// 下側地形判定
				if (MoveDistance.Y > 0)
				{
					// マップからはみ出た場合にその方向が壁属性であれば押し戻す
					if (Map.BottomEdge.IsWall && AbsoluteCollision.Y + AbsoluteCollision.Height - 1 > Const.MapchipTileSize * Map.Size.Height - 1)
					{
						int FitY = Const.MapchipTileSize * Map.Size.Height - 1;
						int NewPositionY = FitY + (DrawPosition.Y - (AbsoluteCollision.Y + AbsoluteCollision.Height - 1));
						Position.Y = NewPositionY;
						MoveDistance.Y = 0;
						IsInAir = false;
						// 描画座標、当たり判定を更新
						UpdateDrawPosition();
						UpdateAbsoluteCollision();
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
								HitCheckPosition = new Point(AbsoluteCollision.X + AbsoluteCollision.Width - 1, AbsoluteCollision.Y + AbsoluteCollision.Height - 1);
							}
							else
							{
								HitCheckPosition = new Point(AbsoluteCollision.X + Const.MapchipTileSize * i, AbsoluteCollision.Y + AbsoluteCollision.Height - 1);
							}
							// 判定実行
							if (PointToCollisionIndex(Map, HitCheckPosition) != 0)
							{
								// 接触した地形にギリギリ接触しない位置に移動する
								int FitY = (HitCheckPosition.Y / Const.MapchipTileSize * Const.MapchipTileSize) - 1;
								int NewPositionY = FitY + (DrawPosition.Y - HitCheckPosition.Y);
								Position.Y = NewPositionY;
								MoveDistance.Y = 0;
								IsInAir = false;
								// 描画座標、当たり判定を更新
								UpdateDrawPosition();
								UpdateAbsoluteCollision();
							}
						}
					}
				}
				// 上側地形判定
				else if (MoveDistance.Y < 0)
				{
					// マップからはみ出た場合にその方向が壁属性であれば押し戻す
					if (Map.TopEdge.IsWall && AbsoluteCollision.Y < 0)
					{
						int FitY = 0;
						int NewPositionY = FitY + (DrawPosition.Y - AbsoluteCollision.Y);
						Position.Y = NewPositionY;
						MoveDistance.Y = 0;
						// 描画座標、当たり判定を更新
						UpdateDrawPosition();
						UpdateAbsoluteCollision();
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
								HitCheckPosition = new Point(AbsoluteCollision.X + AbsoluteCollision.Width - 1, AbsoluteCollision.Y);
							}
							else
							{
								HitCheckPosition = new Point(AbsoluteCollision.X + Const.MapchipTileSize * i, AbsoluteCollision.Y);
							}
							// 判定実行
							if (PointToCollisionIndex(Map, HitCheckPosition) != 0)
							{
								// 接触した地形にギリギリ接触しない位置に移動する
								int FitY = HitCheckPosition.Y / Const.MapchipTileSize * Const.MapchipTileSize + (Const.MapchipTileSize);
								int NewPositionY = FitY + (DrawPosition.Y - HitCheckPosition.Y);
								Position.Y = NewPositionY;
								MoveDistance.Y = 0;
								// 描画座標、当たり判定を更新
								UpdateDrawPosition();
								UpdateAbsoluteCollision();
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
		private void CheckInAir(Map Map)
		{
			IsInAir = true;
			// 描画座標、当たり判定を更新
			UpdateDrawPosition();
			UpdateAbsoluteCollision();
			// マップの端が壁属性であれば着地していることにする
			if (Map.BottomEdge.IsWall && AbsoluteCollision.Y + AbsoluteCollision.Height > Const.MapchipTileSize * Map.Size.Height - 1)
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
						HitCheckPosition = new Point(AbsoluteCollision.X + AbsoluteCollision.Width - 1, AbsoluteCollision.Y + AbsoluteCollision.Height);
					}
					else
					{
						HitCheckPosition = new Point(AbsoluteCollision.X + Const.MapchipTileSize * i, AbsoluteCollision.Y + AbsoluteCollision.Height);
					}
					// 判定実行
					if (PointToCollisionIndex(Map, HitCheckPosition) != 0)
					{
						IsInAir = false;
					}
				}
			}
		}

		/// <summary>
		/// AbsoluteCollision を更新
		/// </summary>
		private void UpdateAbsoluteCollision()
		{
			AbsoluteCollision = new Rectangle(DrawPosition.X + RelativeCollision.X, DrawPosition.Y + RelativeCollision.Y, RelativeCollision.Width, RelativeCollision.Height);
		}

		/// <summary>
		/// DrawPosition を更新
		/// </summary>
		private void UpdateDrawPosition()
		{
			DrawPosition = Position.ToPoint();
		}

		/// <summary>
		/// Map 上における指定した座標の当たり判定IDを返す
		/// </summary>
		/// <param name="Point">当たり判定IDを取得したい Map 上のワールド座標</param>
		/// <returns>指定した座標の当たり判定ID。マップ外を取得しようとした場合は常に 0 を返す。</returns>
		private int PointToCollisionIndex(Map Map, Point WorldPosition)
		{
			int CollisionIndex = 0;
			Point MapPosition = new Point(WorldPosition.X / Const.MapchipTileSize, WorldPosition.Y / Const.MapchipTileSize);
			if (MapPosition.X >= 0 && MapPosition.X < Map.Size.Width && MapPosition.Y >= 0 && MapPosition.Y < Map.Size.Height)
			{
				CollisionIndex = Map.CollisionLayer[MapPosition.X, MapPosition.Y];
			}
			return CollisionIndex;
		}

		#endregion
	}
}
