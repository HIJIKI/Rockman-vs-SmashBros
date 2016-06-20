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
		public AnimationTile[] AnimationTiles;                      // アニメーションタイルのデータ
		public Tile[,] BGLayer;                                     // 背景レイヤー
		public Tile[,] LowerLayer;                                  // 下層レイヤー
		public Tile[,] UpperLayer;                                  // 上層レイヤー
		public int[,] CollisionLayer;                               // 地形判定レイヤー
		public string[,] EntityLayer;                               // エンティティレイヤー
		public int FrameCounter;                                    // フレームカウンター

		// タイル1枚のデータ構造体
		public struct Tile
		{
			public bool IsAnimation;                                // アニメーションタイルかどうか
			public int Index;                                       // マップチップID または アニメーションタイルID
			public Tile(int Index = 0, bool IsAnimation = false)
			{
				this.Index = Index;
				this.IsAnimation = IsAnimation;
			}
		}

		// アニメーションタイルのデータ構造体
		public struct AnimationTile
		{
			public int[] AnimationTable;                            // アニメーションの順番(タイルID)
			public int Interval;                                    // 1枚あたりの長さ(フレーム数)
			public int CurrentlyFrame;                              // 現在どのコマかを指す
			public AnimationTile(int[] AnimationTable = null, int Interval = 1)
			{
				this.AnimationTable = AnimationTable;
				this.Interval = Interval;
				this.CurrentlyFrame = 0;
			}
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public Map() { }
		public void InitForTest()
		{
			Size = new Size(16, 15);
			BGLayer = new Tile[Size.Width, Size.Height];
			LowerLayer = new Tile[Size.Width, Size.Height];
			UpperLayer = new Tile[Size.Width, Size.Height];
			EntityLayer = new string[Size.Width, Size.Height];

			// 下層レイヤーの作成
			int[,] LL = new int[16, 15]
			{
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
				{ 0,0,0,0,0,70,0,0,0,0,0,0,0,0,0 },
				{ 0,0,0,0,0,71,0,0,0,0,0,0,0,0,0 },
				{ 0,0,0,0,0,39,0,0,0,0,0,0,0,0,0 },
				{ 0,0,0,0,0,40,0,0,0,0,0,0,0,0,0 },
				{ 0,0,0,0,0,41,0,0,0,0,0,0,0,0,0 },
				{ 0,0,0,0,0,73,0,0,0,0,0,0,0,0,0 },
				{ 0,0,0,0,0,74,0,0,99,115,0,0,0,0,0 },
				{ 0,0,0,0,0,51,69,85,100,116,132,148,0,0,0 },
				{ 0,0,4,20,36,52,68,84,101,117,133,133,0,0,0 },
				{ 0,0,0,0,0,53,0,87,101,117,133,133,0,0,0 },
				{ 0,0,0,0,0,0,69,85,102,118,134,150,0,0,0 },
				{ 0,0,88,0,0,0,0,0,103,119,0,0,0,0,0 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 }
			};

			// 上層レイヤーの作成
			int[,] UL = new int[16,15]
			{
				{ 2,2,2,6,0,0,0,0,0,0,0,0,0,0,0 },
				{ 2,2,2,6,0,0,0,0,0,0,0,0,0,0,0 },
				{ 2,2,2,7,55,0,0,0,0,0,0,0,0,0,0 },
				{ 2,2,6,0,0,0,0,0,0,0,0,0,0,0,0 },
				{ 2,2,7,0,0,0,0,0,0,0,0,0,0,0,0 },
				{ 4,7,0,0,0,0,0,0,0,0,0,0,0,0,0 },
				{ 0,0,0,0,57,0,0,0,0,0,0,0,0,0,0 },
				{ 0,0,0,0,0,0,0,86,0,0,0,0,0,0,0 },
				{ 0,0,0,0,0,0,0,86,0,0,0,0,0,0,0 },
				{ 0,0,0,0,0,0,0,86,0,8,10,0,0,3,2 },
				{ 0,0,0,0,0,0,0,86,0,9,11,0,2,2,2 },
				{ 0,0,0,0,0,0,0,86,0,0,0,1,2,2,2 },
				{ 0,0,0,0,0,0,0,86,0,0,0,1,2,2,2 },
				{ 0,0,0,0,0,0,0,0,0,0,0,1,2,2,2 },
				{ 0,0,0,0,0,0,0,0,0,0,0,1,2,2,2 },
				{ 0,0,0,0,0,0,0,0,0,0,0,1,2,2,2 }
			};

			UpperLayer[5, 0].IsAnimation = true;
			UpperLayer[5, 1].IsAnimation = true;
			UpperLayer[4, 2].IsAnimation = true;
			UpperLayer[3, 2].IsAnimation = true;
			UpperLayer[2, 3].IsAnimation = true;
			UpperLayer[1, 3].IsAnimation = true;
			UpperLayer[0, 3].IsAnimation = true;
			UpperLayer[9, 9].IsAnimation = true;
			UpperLayer[10, 9].IsAnimation = true;
			UpperLayer[9, 10].IsAnimation = true;
			UpperLayer[10, 10].IsAnimation = true;
			UpperLayer[10, 11].IsAnimation = true;
			UpperLayer[11, 11].IsAnimation = true;
			UpperLayer[12, 11].IsAnimation = true;
			UpperLayer[13, 11].IsAnimation = true;
			UpperLayer[14, 11].IsAnimation = true;
			UpperLayer[15, 11].IsAnimation = true;
			UpperLayer[9, 12].IsAnimation = true;
			UpperLayer[9, 13].IsAnimation = true;
			UpperLayer[8, 14].IsAnimation = true;

			for (int x = 0; x < BGLayer.GetLength(0); x++)
			{
				for (int y = 0; y < BGLayer.GetLength(1); y++)
				{
					BGLayer[x, y].Index = 1;
				}
			}

			for (int x = 0; x < LL.GetLength(0); x++)
			{
				for (int y = 0; y < LL.GetLength(1); y++)
				{
					LowerLayer[x, y].Index = LL[x, y];
					UpperLayer[x, y].Index = UL[x, y];
				}
			}

			// アニメーションタイルの定義
			AnimationTiles = new AnimationTile[12];
			AnimationTiles[0] = new AnimationTile(new int[] { 16, 64, 112, 64 }, 16);
			AnimationTiles[1] = new AnimationTile(new int[] { 17, 65, 113, 65 }, 16);
			AnimationTiles[2] = new AnimationTile(new int[] { 18, 66, 114, 66 }, 16);
			AnimationTiles[3] = new AnimationTile(new int[] { 32, 80, 128, 80 }, 16);
			AnimationTiles[4] = new AnimationTile(new int[] { 34, 82, 130, 82 }, 16);
			AnimationTiles[5] = new AnimationTile(new int[] { 48, 96, 144, 96 }, 16);
			AnimationTiles[6] = new AnimationTile(new int[] { 49, 97, 145, 97 }, 16);
			AnimationTiles[7] = new AnimationTile(new int[] { 50, 98, 146, 98 }, 16);
			AnimationTiles[8] = new AnimationTile(new int[] { 160, 162, 164 }, 12);
			AnimationTiles[9] = new AnimationTile(new int[] { 161, 163, 165 }, 12);
			AnimationTiles[10] = new AnimationTile(new int[] { 176, 178, 180 }, 12);
			AnimationTiles[11] = new AnimationTile(new int[] { 177, 179, 181 }, 12);
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
			Texture = Content.Load<Texture2D>("Images/mario_stage_mapchip.png");
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
			// アニメーションタイルの管理
			for (int i = 0; i < AnimationTiles.Length; i++)
			{
				if (FrameCounter % AnimationTiles[i].Interval == 0)
				{
					AnimationTiles[i].CurrentlyFrame++;
					if (AnimationTiles[i].CurrentlyFrame >= AnimationTiles[i].AnimationTable.Length)
					{
						AnimationTiles[i].CurrentlyFrame = 0;
					}
				}
			}
			FrameCounter++;
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
		}

		/// <summary>
		/// マップチップを描画
		/// </summary>
		/// <param name="SpriteBatch">描画に使用する SpriteBatch</param>
		/// <param name="Tile">描画対象のタイル</param>
		/// <param name="Position">描画する座標</param>
		/// <param name="DrawOrder">描画震度</param>
		private void DrawTile(SpriteBatch SpriteBatch, Tile Tile, Position Position, float DrawOrder)
		{
			int DrawChipIndex = 0;									// 描画するマップチップID

			// アニメーションタイルの場合
			if (Tile.IsAnimation)
			{
				AnimationTile AnimationTile = AnimationTiles[Tile.Index];
				DrawChipIndex = AnimationTile.AnimationTable[AnimationTile.CurrentlyFrame];
			}
			else
			{
				DrawChipIndex = Tile.Index;
			}

			// マップチップのIndexをRectangleに変換
			Size MapchipSize = new Size(Texture.Width / Const.MapchipTileSize, Texture.Height / Const.MapchipTileSize);             //マップチップの縦横枚数
			Rectangle SourceRect = new Rectangle((DrawChipIndex % MapchipSize.Width) * Const.MapchipTileSize, (DrawChipIndex / MapchipSize.Width) * Const.MapchipTileSize, Const.MapchipTileSize, Const.MapchipTileSize);

			// マップチップを描画
			SpriteBatch.Draw(Texture, new Vector2(Position.X, Position.Y), SourceRect, Color.White, 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, DrawOrder);
		}
	}
}
