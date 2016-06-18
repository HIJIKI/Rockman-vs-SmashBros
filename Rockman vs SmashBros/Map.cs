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
	/// Map クラス
	/// </summary>
	class Map
	{
		// メンバーの宣言
		public Texture2D Texture;                                   // マップチップ画像
		public Size Size;                                           // マップの縦横マス数
		public int[,] BGLayer;                                      // 背景レイヤー
		public int[,] LowerLayer;                                   // 下層レイヤー
		public int[,] UpperLayer;                                   // 上層レイヤー
		public string[,] EntityLayer;                               // エンティティレイヤー

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public Map() { }
		public void InitForTest()
		{
			this.Size = new Size(16, 15);
			this.BGLayer = new int[Size.Width, Size.Height];
			this.LowerLayer = new int[Size.Width, Size.Height];
			this.UpperLayer = new int[Size.Width, Size.Height];
			this.EntityLayer = new string[Size.Width, Size.Height];
			BGLayer = new int[16, 15]
			{
				{ 2,2,17,1,1,1,1,1,1,1,32,49,49,49,49 },
				{ 2,3,18,1,1,1,5,1,1,1,33,64,80,80,80 },
				{ 2,4,17,1,1,1,6,1,1,1,34,65,81,81,81 },
				{ 2,2,18,1,21,1,1,1,1,1,1,66,82,82,82 },
				{ 2,2,17,1,22,1,1,1,1,1,1,1,33,49,49 },
				{ 2,2,18,1,1,1,1,1,1,1,1,32,49,49,49 },
				{ 2,2,17,1,1,1,1,1,1,1,64,80,80,80,80 },
				{ 2,2,18,1,1,1,1,1,35,51,65,81,81,81,81 },
				{ 2,2,17,1,1,21,1,1,36,52,65,81,81,81,81 },
				{ 2,2,18,1,1,22,1,1,37,53,66,82,82,82,82 },
				{ 2,2,17,1,1,1,1,1,1,1,34,49,49,49,49 },
				{ 2,19,18,1,1,1,1,1,1,1,1,34,49,49,49 },
				{ 2,20,17,1,1,1,1,1,1,1,1,64,80,80,80 },
				{ 2,2,18,1,5,1,1,48,48,48,48,65,81,81,81 },
				{ 2,2,17,1,6,1,5,149,149,149,149,66,82,82,82 },
				{ 2,2,18,1,1,1,6,1,1,1,1,32,49,49,49 }
			};
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
		public void ContentLoad(ContentManager Content)
		{
			Texture = Content.Load<Texture2D>("Images/link_stage_mapchip.png");
		}

		/// <summary>
		/// リソースの破棄
		/// </summary>
		public void UnloadContent()
		{
			Texture.Dispose();
		}

		/// <summary>
		/// フレームの更新
		/// </summary>
		public void Update(GameTime GameTime)
		{

		}

		/// <summary>
		/// 描画
		/// </summary>
		public void Draw(GameTime GameTime, SpriteBatch SpriteBatch)
		{
			for (int x = 0; x < Size.Width; x++)
			{
				for (int y = 0; y < Size.Height; y++)
				{
					Position Position = new Position(x * Const.MapchipTileSize, y * Const.MapchipTileSize);

					//各レイヤーを描画
					DrawTile(SpriteBatch, BGLayer[x, y], Position, (float)Const.DrawOrder.BGLayer / (float)Const.DrawOrder.MAX);
					DrawTile(SpriteBatch, LowerLayer[x, y], Position, (float)Const.DrawOrder.LowerLayer / (float)Const.DrawOrder.MAX);
					DrawTile(SpriteBatch, UpperLayer[x, y], Position, (float)Const.DrawOrder.UpperLayer / (float)Const.DrawOrder.MAX);
				}
			}

			//DrawTile();
		}

		/// <summary>
		/// マップチップを描画
		/// </summary>
		/// <param name="SpriteBatch">描画に使用する SpriteBatch</param>
		/// <param name="Index">マップチップの ID</param>
		/// <param name="Position">描画する座標</param>
		/// <param name="DrawOrder">描画震度</param>
		private void DrawTile(SpriteBatch SpriteBatch, int Index, Position Position, float DrawOrder)
		{
			// マップチップのIndexをRectangleに変換
			Size MapchipSize = new Size(Texture.Width / Const.MapchipTileSize, Texture.Height / Const.MapchipTileSize);             //マップチップの縦横枚数
			Rectangle SourceRect = new Rectangle((Index % MapchipSize.Width) * Const.MapchipTileSize, (Index / MapchipSize.Width) * Const.MapchipTileSize, Const.MapchipTileSize, Const.MapchipTileSize);

			// マップチップを描画
			SpriteBatch.Draw(Texture, new Vector2(Position.X, Position.Y), SourceRect, Color.White, 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, DrawOrder);
		}
	}
}
