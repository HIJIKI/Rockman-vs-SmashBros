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
	/// ErrorEntity クラス
	/// </summary>
	public class ErrorEntity : Entity
	{
		#region メンバーの宣言
		public static Texture2D Texture;                            // テクスチャ
		#endregion

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ErrorEntity(Point Position, bool IsFromMap, Point FromMapPosition)
		{
			this.Position = Position.ToVector2();
			this.IsFromMap = IsFromMap;
			this.FromMapPosition = FromMapPosition;
			Type = Types.Other;
			IsAlive = true;
			RelativeHitbox = new Rectangle(-8, -15, 16, 16);
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
			// テクスチャの読み込み
			Texture = Content.Load<Texture2D>("Image/ErrorEntity.png");
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
			Vector2 Position = GetDrawPosition().ToVector2();
			Rectangle SourceRectangle = new Rectangle(0, 0, 16, 16);
			Vector2 Origin = new Vector2(8, 15);
			SpriteEffects SpriteEffect = SpriteEffects.None;
			float layerDepth = (float)Const.DrawOrder.Enemy / (float)Const.DrawOrder.MAX;
			SpriteBatch.Draw(Texture, Position, SourceRectangle, Color.White, 0.0f, Origin, 1.0f, SpriteEffect, layerDepth);
			base.Draw(GameTime, SpriteBatch);
		}

	}
}
