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
	/// Enemy クラス
	/// </summary>
	class Enemy : Entity
	{
		#region メンバーの宣言

		#endregion

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public Enemy() { }

		/// <summary>
		/// 初期化
		/// </summary>
		public void Initialize(Vector2 Position, Rectangle Collision)
		{
			IsLive = true;
			this.Position = Position;
			this.Collision = Collision;
		}

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
		public void Update(GameTime GameTime, Player Player)
		{
			Rectangle EnemyCollision = new Rectangle(Collision.X + (int)Position.X, Collision.Y + (int)Position.Y, Collision.Width, Collision.Height);
			Rectangle PlayerCollision = new Rectangle(Player.Collision.X + (int)Player.Position.X, Player.Collision.Y + (int)Player.Position.Y, Player.Collision.Width, Player.Collision.Height);
			if (EnemyCollision.Intersects(PlayerCollision))
			{
				Destroy(this);
			}
		}

		/// <summary>
		/// 描画
		/// </summary>
		/*
		public void Draw(GameTime GameTime, SpriteBatch SpriteBatch)
		{

		}
		//*/
	}
}
