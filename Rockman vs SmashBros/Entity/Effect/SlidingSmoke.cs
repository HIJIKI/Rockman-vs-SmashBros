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
	/// SlidingSmoke クラス
	/// </summary>
	public class SlidingSmoke : Entity
	{
		#region メンバーの宣言

		private static Texture2D Texture;                           // テクスチャ
		private static Sprite[] Sprites;                            // スプライト定義
		public int FrameCounter;                                    // フレームカウンター
		public int AnimationPattern;                                // アニメーションのパターン
		public bool IsFaceToLeft;                                   // 左を向いているかどうか

		#endregion

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public SlidingSmoke(Point Position, bool IsFaceToLeft)
		{
			this.Position = Position.ToVector2();
			this.IsFaceToLeft = IsFaceToLeft;
			Type = Types.Effect;
			IsAlive = true;
			RelativeHitbox = new Rectangle(-4, -4, 8, 8);
			IsIgnoreGravity = true;
			IsNoclip = true;
			FrameCounter = 0;
			AnimationPattern = 0;
		}

		/// <summary>
		/// リソースの確保
		/// </summary>
		public static void LoadContent(ContentManager Content)
		{
			Texture = Content.Load<Texture2D>("Image/Effect/SlidingSmoke.png");

			#region 各スプライトの定義

			Sprites = new Sprite[]
			{
				new Sprite(new Rectangle(8 * 0, 0, 8, 8), new Vector2(4, 4)),
				new Sprite(new Rectangle(8 * 1, 0, 8, 8), new Vector2(4, 4)),
				new Sprite(new Rectangle(8 * 2, 0, 8, 8), new Vector2(4, 4)),
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
			if (FrameCounter % 4 == 0 && FrameCounter != 0)
			{
				AnimationPattern++;
				if (AnimationPattern >= Sprites.Length)
				{
					Destroy();
				}
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
				// 描画
				Vector2 Position = GetDrawPosition().ToVector2();
				Rectangle SourceRectangle = Sprites[AnimationPattern].SourceRectangle;
				Vector2 Origin = Sprites[AnimationPattern].Origin;
				SpriteEffects SpriteEffect = IsFaceToLeft ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
				float layerDepth = (float)Const.DrawOrder.Effect / (float)Const.DrawOrder.MAX;
				// 左を向いている場合は中心座標を左右反転
				if (IsFaceToLeft)
				{
					Origin = new Vector2((SourceRectangle.Width) - Origin.X, Origin.Y);
				}
				SpriteBatch.Draw(Texture, Position, SourceRectangle, Color.White, 0.0f, Origin, 1.0f, SpriteEffect, layerDepth);
			}

			base.Draw(GameTime, SpriteBatch);
		}

	}
}
