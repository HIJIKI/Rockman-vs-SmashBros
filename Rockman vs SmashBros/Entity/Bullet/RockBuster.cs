using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Design;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Storage;

namespace Rockman_vs_SmashBros
{
	/// <summary>
	/// RockBuster クラス
	/// </summary>
	class RockBuster : Bullet
	{
		#region メンバーの宣言

		public static Texture2D Texture;                            // テクスチャ
		public bool IsFaceToLeft;                                   // 左を向いているかどうか
		public DamageDetail DamageDetail;                           // 敵に接触した時に与えるダメージ
		public static int Count;                                    // 画面内に存在するロックバスターの数

		private static SpritesStruct Sprites;                       // 各スプライト
		private struct SpritesStruct                                // 各スプライト管理構造体
		{
			public Sprite Normal;                                   // 豆玉
			public Sprite[] Charge1;                                // 溜め1
			public Sprite[] Charge2;                                // 溜め2
		}

		#endregion

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public RockBuster(Point Position, bool FaceToLeft)
		{
			IsAlive = true;
			IsNoclip = true;
			this.Position = Position.ToVector2();
			IsFaceToLeft = FaceToLeft;
			DamageDetail = new DamageDetail(1, DamageDetail.Types.RockBuster1, this);

			RelativeCollision = new Rectangle(-4, -4, 8, 8);

			Count++;
		}

		/// <summary>
		/// 初期化
		/// </summary>
		public override void Initialize()
		{

		}

		/// <summary>
		/// リソースの確保
		/// </summary>
		public static void LoadContent(ContentManager Content)
		{
			Texture = Content.Load<Texture2D>("Image/Player_RockBuster.png");

			#region 各スプライトの定義
			Sprites.Normal = new Sprite(new Rectangle(0, 0, 8, 8), new Vector2(4, 4));
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
			float Speed = 4.0f;
			// 左向き
			if (IsFaceToLeft)
			{
				MoveDistance.X = -Speed;
			}
			// 右向き
			else
			{
				MoveDistance.X = Speed;
			}

			// カメラ外に出たらデスポーン
			Rectangle CameraViewRange = new Rectangle(Camera.Position.X, Camera.Position.Y, Const.GameScreenWidth, Const.GameScreenHeight);
			if (!CameraViewRange.Intersects(GetAbsoluteCollision()))
			{
				Destroy();
			}

			// 敵に接触したらダメージを与える
			if (!IsRepelled)
			{
				Rectangle AbsoluteCollision = GetAbsoluteCollision();
				foreach (var Entity in Main.Entities)
				{
					if (Entity != this && Entity.Type == Types.Enemy && AbsoluteCollision.Intersects(Entity.GetAbsoluteCollision()))
					{
						if (Entity.GiveDamage(DamageDetail))
						{
							Destroy();
						}
						else
						{
							Repel();
						}
						break;
					}
				}
			}

			base.Update(GameTime);
		}

		/// <summary>
		/// 描画
		/// </summary>
		public override void Draw(GameTime GameTime, SpriteBatch SpriteBatch)
		{
			if (IsAlive)
			{
				// 描画するスプライト
				Sprite CurrentlySprite = new Sprite();

				CurrentlySprite = Sprites.Normal;

				// 描画
				Vector2 Position = GetDrawPosition().ToVector2();
				Rectangle SourceRectangle = CurrentlySprite.SourceRectangle;
				Vector2 Origin = CurrentlySprite.Origin;
				SpriteEffects SpriteEffect = IsFaceToLeft ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
				float layerDepth = (float)Const.DrawOrder.PlayerShot / (float)Const.DrawOrder.MAX;
				// 左を向いている場合は中心座標を反転
				if (IsFaceToLeft)
				{
					Origin = new Vector2((CurrentlySprite.SourceRectangle.Width) - Origin.X, Origin.Y);
				}
				SpriteBatch.Draw(Texture, Position, SourceRectangle, Color.White, 0.0f, Origin, 1.0f, SpriteEffect, layerDepth);

			}

			base.Draw(GameTime, SpriteBatch);
		}

		/// <summary>
		/// 削除
		/// </summary>
		public override void Destroy()
		{
			Count--;
			base.Destroy();
		}

		public override void Repel()
		{
			IsRepelled = true;
			MoveDistance.Y = -Math.Abs(MoveDistance.X);
			IsFaceToLeft = !IsFaceToLeft;
		}

		#region プライベート関数

		#endregion
	}
}
