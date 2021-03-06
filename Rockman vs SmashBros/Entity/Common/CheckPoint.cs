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
	/// CheckPoint クラス
	/// </summary>
	public class CheckPoint : Entity
	{
		#region メンバーの宣言
		private static Texture2D Texture;							// テクスチャ
		private static Sprite Sprite;								// スプライト定義
		#endregion

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public CheckPoint(Point Position, bool IsFromMap, Point FromMapPosition)
		{
			IsAlive = true;
			this.Position = Position.ToVector2();
			this.IsFromMap = IsFromMap;
			this.FromMapPosition = FromMapPosition;
			Type = Types.Other;

			Main.SetSpawnPoint(FromMapPosition);

			// 2 度目以降は出現しないようにする
			if (IsFromMap)
			{
				Map.SetInvalidEntity(FromMapPosition);
			}
		}

		/// <summary>
		/// リソースの確保
		/// </summary>
		public static void LoadContent(ContentManager Content)
		{
			Texture = Content.Load<Texture2D>("Image/Common/CheckPoint.png");

			Sprite = new Sprite(new Rectangle(0, 0, 16, 16), new Vector2(8, 15));
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
		}

		/// <summary>
		/// 描画
		/// </summary>
		public override void Draw(GameTime GameTime, SpriteBatch SpriteBatch)
		{
			if (Global.Debug)
			{
				Vector2 Position = GetDrawPosition().ToVector2();
				Rectangle SourceRectangle = Sprite.SourceRectangle;
				Vector2 Origin = Sprite.Origin;
				SpriteEffects SpriteEffect = SpriteEffects.None;
				float LayerDepth = Const.DrawOrder.Enemy.ToLayerDepth();
				SpriteBatch.Draw(Texture, Position, SourceRectangle, Color.White, 0.0f, Origin, 1.0f, SpriteEffect, LayerDepth);
			}
			base.Draw(GameTime, SpriteBatch);
		}

	}
}
