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
				// 左側地形判定
				else if (MoveDistance.X < 0)
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
							// 描画座標、当たり判定を更新
							UpdateDrawPosition();
							UpdateAbsoluteCollision();
						}
					}
				}
				// 上側地形判定
				else if (MoveDistance.Y < 0)
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
				MoveDistance.Y = 0;
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
