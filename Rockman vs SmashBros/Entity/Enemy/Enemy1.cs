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
	/// Enemy1 クラス
	/// </summary>
	public class Enemy1 : Entity
	{
		#region メンバーの宣言
		public static Texture2D Texture;                            // テクスチャ
		public Point OriginPosition;                                // ワールド座標に対する相対的な描画座標
		private bool FaceToRight;                                   // 右を向いているかどうか
		#endregion

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public Enemy1(Point Position, bool IsFromMap = false, Point FromMapPosition = new Point())
		{
			this.Position = Position.ToVector2();
			this.IsFromMap = IsFromMap;
			this.FromMapPosition = FromMapPosition;
			Type = Types.Enemy;
			Initialize();
		}

		/// <summary>
		/// 初期化
		/// </summary>
		public override void Initialize()
		{
			IsAlive = true;
			IsNoclip = false;
			MoveDistance = Vector2.Zero;
			OriginPosition.X = 15;
			OriginPosition.Y = 29;
			RelativeCollision = new Rectangle(-7, -23, 14, 24);
		}

		/// <summary>
		/// リソースの確保
		/// </summary>
		public static void LoadContent(ContentManager Content)
		{
			Texture = Content.Load<Texture2D>("Image/hairaru_hei.png");
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
			float Speed = 0.5f;
			MoveDistance.X = 0;

			// 左右移動
			if (Math.Abs(Main.Player.Position.X - Position.X) > 16)
			{
				if (Main.Player.Position.X > Position.X)
				{
					MoveDistance.X += Speed;
					FaceToRight = true;
				}
				else
				{
					MoveDistance.X -= Speed;
					FaceToRight = false;
				}
			}

			// 重力付加
			if (IsInAir)
			{
				MoveDistance.Y += 0.25f;
			}

			// 落下時にデスポーン
			if (Position.Y > Main.Map.Size.Height * Const.MapchipTileSize)
			{
				Destroy(this);
			}

			base.Update(GameTime);
		}

		/// <summary>
		/// 描画
		/// </summary>
		public override void Draw(GameTime GameTime, SpriteBatch SpriteBatch)
		{
			// 描画
			Vector2 position = DrawPosition.ToVector2();
			Rectangle sourceRectangle = new Rectangle(0, 0, 32, 32);
			Vector2 origin = FaceToRight ? OriginPosition.ToVector2() : new Vector2(32 - OriginPosition.X, OriginPosition.Y);
			SpriteEffects SpriteEffect = FaceToRight ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
			float layerDepth = (float)Const.DrawOrder.Enemy / (float)Const.DrawOrder.MAX;

			SpriteBatch.Draw(Texture, position, sourceRectangle, Color.White, 0.0f, origin, 1.0f, SpriteEffect, layerDepth);

			base.Draw(GameTime, SpriteBatch);
		}
	}
}
