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

		/// <summary>
		/// テスト用初期化
		/// </summary>
		public void InitForTest()
		{
			Size = new Size(24, 24);
			BGLayer = new Tile[Size.Width, Size.Height];
			LowerLayer = new Tile[Size.Width, Size.Height];
			UpperLayer = new Tile[Size.Width, Size.Height];
			EntityLayer = new string[Size.Width, Size.Height];
			CollisionLayer = new int[Size.Width, Size.Height];

			#region 背景レイヤーの作成
			int[,] BGL = new int[24, 24]
			{
				{ 2,2,2,2,2,2,2,2,2,2,2,2,2,3,4,2,2,2,2,2,2,2,2,2 },
				{ 2,3,4,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2 },
				{ 2,2,2,2,2,19,20,2,2,2,2,2,2,2,2,19,20,2,2,2,2,2,3,4 },
				{ 17,17,17,17,17,18,18,17,17,17,17,18,18,17,17,17,17,18,17,17,18,18,17,18 },
				{ 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1 },
				{ 1,1,5,6,1,1,1,1,1,1,5,6,1,1,1,1,1,1,1,1,1,1,1,1 },
				{ 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,21,22,1,1 },
				{ 1,21,22,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1 },
				{ 1,1,1,1,1,5,6,1,1,1,1,1,1,1,1,1,21,22,1,1,1,1,1,1 },
				{ 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1 },
				{ 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1 },
				{ 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1 },
				{ 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1 },
				{ 1,64,66,1,1,1,1,1,1,1,72,73,74,1,1,1,1,1,1,1,1,1,1,1 },
				{ 1,80,82,1,1,1,1,1,1,1,88,89,90,1,1,1,1,1,1,1,1,1,1,1 },
				{ 1,80,82,32,33,33,34,1,1,1,88,89,90,1,35,36,37,1,1,32,33,34,1,1 },
				{ 1,80,82,49,49,49,49,33,34,32,88,89,90,1,51,52,53,1,32,49,50,49,33,33 },
				{ 32,80,82,49,50,49,49,49,49,64,65,65,65,66,49,49,49,33,161,49,49,64,65,65 },
				{ 49,80,82,49,49,49,49,49,49,80,81,96,97,82,49,49,49,49,49,49,49,80,81,96 },
				{ 49,80,82,49,49,64,65,65,65,66,81,112,113,82,49,49,64,65,65,65,65,66,81,112 },
				{ 49,80,82,49,49,80,96,97,81,82,81,81,81,82,49,49,80,81,96,97,81,82,81,81 },
				{ 49,80,82,49,49,80,112,113,81,82,81,81,81,82,49,49,80,81,112,113,81,82,81,81 },
				{ 49,80,82,49,49,80,81,81,81,82,81,81,81,82,49,49,80,81,81,81,81,82,81,81 },
				{ 49,80,82,49,49,80,81,81,81,82,81,81,81,82,49,49,80,81,81,81,81,82,81,81 }
			};
			#endregion

			#region 下層レイヤーの作成
			int[,] LL = new int[24, 24]
			{
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
				{ 0,0,0,0,0,0,0,0,0,78,79,0,78,79,0,0,0,0,0,0,0,0,0,0 },
				{ 0,0,0,0,0,0,0,0,0,94,95,0,94,95,0,0,0,0,0,0,0,0,0,0 },
				{ 0,0,0,0,0,0,0,0,0,110,111,0,110,111,0,0,0,0,0,0,0,0,0,0 },
				{ 0,0,0,0,0,0,0,0,0,126,127,0,126,127,0,0,0,0,0,0,0,0,0,0 },
				{ 0,0,0,0,0,0,0,0,0,142,143,0,142,143,0,0,0,0,0,0,0,0,0,0 },
				{ 0,0,0,0,0,0,0,0,0,158,159,0,158,159,0,0,0,0,0,0,0,0,0,0 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 }
			};
			#endregion

			#region 上層レイヤーの作成
			int[,] UL = new int[24, 24]
			{
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
				{ 0,68,67,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,86,149,87,0,0,0,0 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,102,149,103,0,0,0,0 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,160,177,162,0,0,0,0 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,176,149,178,0,0,0,0 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 }
			};
			#endregion

			#region 当たり判定レイヤー
			int[,] CL = new int[24, 24]
			{
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
				{ 0,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
				{ 0,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
				{ 0,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0 },
				{ 0,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0 },
				{ 0,1,1,0,0,0,0,0,0,1,1,1,1,1,0,0,0,0,0,0,0,1,1,1 },
				{ 0,1,1,0,0,0,0,0,0,1,1,1,1,1,0,0,0,0,0,0,0,1,1,1 },
				{ 0,1,1,0,0,1,1,1,1,1,1,1,1,1,0,0,1,1,1,1,1,1,1,1 },
				{ 0,1,1,0,0,1,1,1,1,1,1,1,1,1,0,0,1,1,1,1,1,1,1,1 },
				{ 0,1,1,0,0,1,1,1,1,1,1,1,1,1,0,0,1,1,1,1,1,1,1,1 },
				{ 0,1,1,0,0,1,1,1,1,1,1,1,1,1,0,0,1,1,1,1,1,1,1,1 },
				{ 0,1,1,0,0,1,1,1,1,1,1,1,1,1,0,0,1,1,1,1,1,1,1,1 }
			};
			#endregion

			for (int x = 0; x < Size.Width; x++)
			{
				for (int y = 0; y < Size.Height; y++)
				{
					BGLayer[x, y].Index = BGL[y, x];
					LowerLayer[x, y].Index = LL[y, x];
					UpperLayer[x, y].Index = UL[y, x];
					CollisionLayer[x, y] = CL[y, x];
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
		public void LoadContent(ContentManager Content)
		{
			Texture = Content.Load<Texture2D>("Image/link_stage_mapchip.png");
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
					Point Position = new Point(x * Const.MapchipTileSize, y * Const.MapchipTileSize);

					//各レイヤーを描画
					DrawTile(SpriteBatch, BGLayer[x, y], Position, (float)Const.DrawOrder.BGLayer / (float)Const.DrawOrder.MAX);
					DrawTile(SpriteBatch, LowerLayer[x, y], Position, (float)Const.DrawOrder.LowerLayer / (float)Const.DrawOrder.MAX);
					DrawTile(SpriteBatch, UpperLayer[x, y], Position, (float)Const.DrawOrder.UpperLayer / (float)Const.DrawOrder.MAX);

					// デバッグ描画 (地形判定)
					if (Global.Debug)
					{
						if (CollisionLayer[x, y] == 1)
						{
							SpriteBatch.DrawRectangle(new Rectangle(Const.MapchipTileSize * x, Const.MapchipTileSize * y, Const.MapchipTileSize, Const.MapchipTileSize), Color.Lime * 0.2f);
							SpriteBatch.DrawRectangle(new Rectangle(Const.MapchipTileSize * x, Const.MapchipTileSize * y, Const.MapchipTileSize, Const.MapchipTileSize), Color.Lime * 0.2f, true);
						}
					}
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
		private void DrawTile(SpriteBatch SpriteBatch, Tile Tile, Point Position, float DrawOrder)
		{
			int DrawChipIndex = 0;                                  // 描画するマップチップID

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
