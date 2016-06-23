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
		public bool IsLive;                                         // 生存フラグ
		public bool IsHitTerrain;                                   // 地形判定を行うかどうか
		public bool IsAir;                                          // 空中にいるかどうか
		public Vector2 Position;                                    // 座標
		public Vector2 MoveDistance;                                // 現在のフレームで移動する量
		public Rectangle Collision;                                 // 当たり判定 (相対座標)
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
		public void Update(GameTime GameTime)
		{
			if (IsLive)
			{
				Position += MoveDistance;
			}
		}

		/// <summary>
		/// 描画
		/// </summary>
		public void Draw(GameTime GameTime, SpriteBatch SpriteBatch)
		{
			if (IsLive)
			{
				// デバッグ描画
				if (Global.Debug)
				{
					SpriteBatch.DrawRectangle(new Rectangle(Collision.X + (int)Position.X, Collision.Y + (int)Position.Y, Collision.Width, Collision.Height), Color.Blue * 0.2f, true);
					SpriteBatch.DrawRectangle(new Rectangle(Collision.X + (int)Position.X, Collision.Y + (int)Position.Y, Collision.Width, Collision.Height), Color.Blue);
					SpriteBatch.DrawPixel(Position, Color.Red);
				}
			}
		}

		/// <summary>
		/// エンティティを削除
		/// </summary>
		public void Destroy(Entity Entity)
		{
			IsLive = false;
		}

		/// <summary>
		/// 指定した座標へ移動
		/// </summary>
		public void SetPosition(Vector2 Position)
		{
			this.Position = Position;
		}

		/// <summary>
		/// 指定したX座標へ移動
		/// </summary>
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
	}
}
