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
	class Entity
	{
		#region メンバーの宣言
		public bool IsAlive;                                        // 生存フラグ
		public bool IsNoclip;                                       // 地形を貫通するかどうか
		public bool IsInAir;                                        // 空中にいるかどうか
		public Vector2 Position;                                    // 内部座標
		public Point DrawPosition;                                  // 描画座標
		public Vector2 MoveDistance;                                // 現在のフレームで移動する量
		public Rectangle RelativeCollision;                         // 当たり判定 (相対)
		public Rectangle AbsoluteCollision;                         // 当たり判定 (絶対)
		#endregion

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public Entity() { }

		/// <summary>
		/// 初期化
		/// </summary>
		public void Initialize() { }

		/// <summary>
		/// リソースの確保
		/// </summary>
		public void LoadConten(ContentManager Content)
		{
		}

		/// <summary>
		/// リソースの破棄
		/// </summary>
		public void UnloadContent()
		{
		}

		/// <summary>
		/// フレームの更新
		/// </summary>
		public void Update(GameTime GameTime, Map Map)
		{
			if (IsAlive)
			{
				MoveX(Map);
				MoveY(Map);
				CheckInAir(Map);
			}
		}

		/// <summary>
		/// 描画
		/// </summary>
		public void Draw(GameTime GameTime, SpriteBatch SpriteBatch)
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
				}
			}
		}

		/// <summary>
		/// エンティティを削除
		/// </summary>
		public void Destroy(Entity Entity)
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
			// 移動先のX座標
			float NewPositionX = Position.X + MoveDistance.X;

			// 地形判定を行う場合
			if (!IsNoclip)
			{
				// 描画座標、当たり判定を更新
				UpdateDrawPosition();
				UpdateAbsoluteCollision();

				// 移動先の仮当たり判定
				Rectangle ProvisionalCollision = new Rectangle(AbsoluteCollision.X + (int)MoveDistance.X, AbsoluteCollision.Y, AbsoluteCollision.Width, AbsoluteCollision.Height);

				// 右側地形判定
				if (MoveDistance.X > 0)
				{
					Point RightTop = new Point(ProvisionalCollision.X + ProvisionalCollision.Width - 1, ProvisionalCollision.Y);
					Point RightBotom = new Point(ProvisionalCollision.X + ProvisionalCollision.Width - 1, ProvisionalCollision.Y + ProvisionalCollision.Height - 1);
					if (PointToCollisionIndex(Map, RightTop) != 0 ||
						PointToCollisionIndex(Map, RightBotom) != 0)
					{
						// 接触した地形にギリギリ接触しない位置に移動する
						int FitX = ((ProvisionalCollision.X + ProvisionalCollision.Width - 1) / Const.MapchipTileSize * Const.MapchipTileSize) - 1;
						NewPositionX = FitX + (DrawPosition.X - (AbsoluteCollision.X + AbsoluteCollision.Width - 1));
						MoveDistance.X = 0;
					}

				}
				// 左側地形判定
				else if (MoveDistance.X < 0)
				{
					Point LeftTop = new Point(ProvisionalCollision.X, ProvisionalCollision.Y);
					Point LeftBottom = new Point(ProvisionalCollision.X, ProvisionalCollision.Y + ProvisionalCollision.Height - 1);
					if (PointToCollisionIndex(Map, LeftTop) != 0 ||
						PointToCollisionIndex(Map, LeftBottom) != 0)
					{
						// 接触した地形にギリギリ接触しない位置に移動する
						int FitX = (ProvisionalCollision.X) / Const.MapchipTileSize * Const.MapchipTileSize + (Const.MapchipTileSize);
						NewPositionX = FitX + (DrawPosition.X - AbsoluteCollision.X);
						MoveDistance.X = 0;
					}

				}

			}
			// 移動量を反映
			Position.X = NewPositionX;

			// 描画座標、当たり判定を更新
			UpdateDrawPosition();
			UpdateAbsoluteCollision();
		}

		/// <summary>
		/// Y 方向の移動、地形判定
		/// </summary>
		/// <param name="Map">地形判定を行う場合の対象となるマップ</param>
		private void MoveY(Map Map)
		{
			// 移動先のX座標
			float NewPositionY = Position.Y + MoveDistance.Y;

			// 地形判定を行う場合
			if (!IsNoclip)
			{
				// 描画座標、当たり判定を更新
				UpdateDrawPosition();
				UpdateAbsoluteCollision();

				// 移動先の仮当たり判定
				Rectangle ProvisionalCollision = new Rectangle(AbsoluteCollision.X, AbsoluteCollision.Y + (int)MoveDistance.Y, AbsoluteCollision.Width, AbsoluteCollision.Height);

				// 下側地形判定
				if (MoveDistance.Y > 0)
				{
					Point LeftBottom = new Point(ProvisionalCollision.X, ProvisionalCollision.Y + ProvisionalCollision.Height - 1);
					Point RightBottom = new Point(ProvisionalCollision.X + ProvisionalCollision.Width - 1, ProvisionalCollision.Y + ProvisionalCollision.Height - 1);
					if (PointToCollisionIndex(Map, LeftBottom) != 0 ||
						PointToCollisionIndex(Map, RightBottom) != 0)
					{
						// 接触した地形にギリギリ接触しない位置に移動する
						int FitY = ((ProvisionalCollision.Y + ProvisionalCollision.Height - 1) / Const.MapchipTileSize * Const.MapchipTileSize) - 1;
						NewPositionY = FitY + (DrawPosition.Y - (AbsoluteCollision.Y + AbsoluteCollision.Height - 1));
						MoveDistance.Y = 0;
						IsInAir = false;
					}
				}
				// 上側地形判定
				else if (MoveDistance.Y < 0)
				{
					Point LeftTop = new Point(ProvisionalCollision.X, ProvisionalCollision.Y);
					Point RightTop = new Point(ProvisionalCollision.X + ProvisionalCollision.Width - 1, ProvisionalCollision.Y);
					if (PointToCollisionIndex(Map, LeftTop) != 0 ||
						PointToCollisionIndex(Map, RightTop) != 0)
					{
						// 接触した地形にギリギリ接触しない位置に移動する
						int FitY = (ProvisionalCollision.Y) / Const.MapchipTileSize * Const.MapchipTileSize + (Const.MapchipTileSize);
						NewPositionY = FitY + (DrawPosition.Y - AbsoluteCollision.Y);
						MoveDistance.Y = 0;
					}

				}

			}
			// 移動量を反映
			Position.Y = NewPositionY;

			// 描画座標、当たり判定を更新
			UpdateDrawPosition();
			UpdateAbsoluteCollision();
		}

		/// <summary>
		/// 接地しているかを確認する
		/// </summary>
		/// <param name="Map">地形判定を行う場合の対象となるマップ</param>
		private void CheckInAir(Map Map)
		{
			Point LeftBottom = new Point(AbsoluteCollision.X, AbsoluteCollision.Y + AbsoluteCollision.Height);
			Point RightBottom = new Point(AbsoluteCollision.X + AbsoluteCollision.Width - 1, AbsoluteCollision.Y + AbsoluteCollision.Height);
			if (PointToCollisionIndex(Map, LeftBottom) != 0 ||
				PointToCollisionIndex(Map, RightBottom) != 0)
			{
				IsInAir = false;
				//MoveDistance.Y = 0;
			}
			else
			{
				IsInAir = true;
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
