﻿using Microsoft.Xna.Framework;
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
	/// DestroyEffect1 クラス
	/// </summary>
	public class DestroyEffect1 : Entity
	{
		#region メンバーの宣言

		private static Texture2D Texture;                           // テクスチャ
		public int FrameCounter;                                    // フレームカウンター
		public int AnimationPattern;                                // アニメーションのパターン

		#endregion

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public DestroyEffect1(Point Position, bool IsFromMap, Point FromMapPosition)
		{
			this.Position = Position.ToVector2();
			this.IsFromMap = IsFromMap;
			this.FromMapPosition = FromMapPosition;
			Type = Types.Effect;
			IsAlive = true;
			RelativeHitbox = new Rectangle(-16, -16, 32, 32);
			IsIgnoreGravity = true;
			IsNoclip = true;
			FrameCounter = 0;
			AnimationPattern = 0;
		}

		/// <summary>
		/// 初期化
		/// </summary>
		public void Initialize()
		{
		}

		/// <summary>
		/// リソースの確保
		/// </summary>
		public static void LoadContent(ContentManager Content)
		{
			Texture = Content.Load<Texture2D>("Image/Effect/DestroyEffect.png");
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
				if (AnimationPattern >= 4)
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
				Rectangle SourceRectangle = new Rectangle(32 * AnimationPattern, 0, 32, 32);
				Vector2 Origin = new Vector2(16, 16);
				SpriteEffects SpriteEffect = SpriteEffects.None;
				float layerDepth = (float)Const.DrawOrder.Effect / (float)Const.DrawOrder.MAX;
				SpriteBatch.Draw(Texture, Position, SourceRectangle, Color.White, 0.0f, Origin, 1.0f, SpriteEffect, layerDepth);
			}

			base.Draw(GameTime, SpriteBatch);
		}

	}
}
