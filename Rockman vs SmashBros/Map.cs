﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Design;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Storage;
using System.Collections.Generic;

namespace Rockman_vs_SmashBros
{
	/// <summary>
	/// Map クラス
	/// </summary>
	public class Map
	{
		#region メンバーの宣言 

		public static Texture2D Texture;                            // マップチップ画像
		public Size Size;                                           // マップの縦横マス数
		public AnimationTile[] AnimationTiles;                      // アニメーションタイルのデータ
		public List<Section> Sections = new List<Section>();        // マップ内セクションのデータ
		public int CurrentlySectionID;                              // 現在いるセクションのID
		public Tile[,] BGLayer;                                     // 背景レイヤー
		public Tile[,] LowerLayer;                                  // 下層レイヤー
		public Tile[,] UpperLayer;                                  // 上層レイヤー
		public int[,] CollisionLayer;                               // 地形判定レイヤー
		public string[,] EntityLayer;                               // エンティティレイヤー
		public int FrameCounter;                                    // フレームカウンター
		public bool StopEntitySpawn;                                // エンティティのスポーンを停止するフラグ

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

		// マップ内セクションのデータ構造体
		public struct Section
		{
			public Rectangle Area;                                  // セクションの範囲 (マス数)
			public bool TopIsWall;                                  // 上の辺を壁として扱うかどうか
			public bool BottomIsWall;                               // 下の辺を壁として扱うかどうか
			public bool LeftIsWall;                                 // 左の辺を壁として扱うかどうか
			public bool RightIsWall;                                // 右の辺を壁として扱うかどうか
			public Section(Rectangle Area, bool TopIsWall, bool BottomIsWall, bool LeftIsWall, bool RightIsWall)
			{
				this.Area = Area;
				this.TopIsWall = TopIsWall;
				this.BottomIsWall = BottomIsWall;
				this.LeftIsWall = LeftIsWall;
				this.RightIsWall = RightIsWall;
			}
		}

		// 当たり判定の属性
		public enum CollisionTypes
		{
			Air,													// 空気
			Wall,													// 壁
			Ladder													// 梯子
		}

		#endregion

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public Map() { }

		/// <summary>
		/// テスト用初期化
		/// </summary>
		public void InitForTest()
		{
			Size = new Size(32, 45);
			BGLayer = new Tile[Size.Width, Size.Height];
			LowerLayer = new Tile[Size.Width, Size.Height];
			UpperLayer = new Tile[Size.Width, Size.Height];
			EntityLayer = new string[Size.Width, Size.Height];
			CollisionLayer = new int[Size.Width, Size.Height];

			#region 背景レイヤーの作成
			int[,] BGL = new int[45, 32]
			{
				{ 2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,80,81 },
				{ 17,17,18,18,17,17,18,17,17,18,18,18,18,18,17,17,18,18,17,17,18,17,17,18,18,18,18,18,17,18,80,81 },
				{ 1,1,1,1,1,21,22,1,1,1,5,6,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,80,81 },
				{ 1,5,6,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,5,6,1,1,1,1,5,6,1,21,22,1,80,81 },
				{ 1,1,1,1,1,1,1,1,64,66,1,1,1,21,22,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,80,81 },
				{ 1,1,1,1,1,1,1,1,80,82,1,1,1,1,1,1,1,1,1,1,1,21,22,1,1,1,1,1,1,1,80,81 },
				{ 65,65,65,65,65,48,65,65,81,82,48,65,1,1,65,65,65,65,65,1,1,1,1,1,1,35,36,37,1,1,80,81 },
				{ 89,89,89,89,89,48,89,90,1,1,48,1,1,1,1,1,1,1,1,1,1,1,1,1,1,51,52,53,1,1,80,81 },
				{ 104,105,89,89,89,48,89,90,1,32,64,66,1,1,1,1,1,1,1,1,1,32,34,1,32,49,49,49,34,1,80,81 },
				{ 120,121,89,89,89,48,89,90,32,50,80,82,34,1,1,1,1,32,34,1,32,49,49,33,49,49,50,49,49,33,80,81 },
				{ 89,89,89,89,89,48,89,90,64,66,81,82,49,33,34,32,33,49,49,33,49,50,49,49,49,50,49,49,49,49,80,81 },
				{ 89,89,89,89,89,48,89,90,80,81,81,82,50,49,49,49,49,50,49,49,49,49,49,49,49,49,49,49,49,49,80,81 },
				{ 65,65,65,65,65,65,65,65,81,96,97,81,65,65,65,65,65,65,65,65,65,65,65,65,65,65,65,65,48,65,81,81 },
				{ 81,81,81,81,81,81,81,81,81,112,113,81,81,81,81,81,81,81,81,81,81,81,81,81,81,81,81,81,48,81,81,81 },
				{ 81,64,65,65,66,81,81,81,81,81,81,64,65,65,66,81,81,81,96,97,81,81,81,96,97,81,81,81,48,81,81,81 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,81,81,112,113,81,81,81,112,113,81,81,81,48,81,81,81 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,81,81,81,81,81,81,81,81,81,81,81,81,48,81,81,81 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,81,81,81,81,81,81,81,81,81,81,81,81,48,81,81,81 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,82,89,89,89,89,89,89,89,89,89,89,89,89,89,89,80 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,82,89,89,89,89,89,89,89,89,89,89,89,89,89,89,80 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,82,89,104,105,89,89,89,104,105,89,89,89,104,105,89,80 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,82,89,120,121,89,89,89,120,121,89,89,89,120,121,89,80 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,82,89,89,89,89,89,89,89,89,89,89,89,89,89,89,80 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,82,89,89,89,89,89,89,89,89,89,89,89,89,89,89,80 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,82,89,89,89,89,89,89,89,89,89,89,89,89,89,89,80 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,82,89,89,89,48,64,65,65,65,65,65,65,65,65,65,81 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,82,89,89,89,48,80,81,81,81,81,81,81,81,81,81,81 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,82,89,89,89,48,89,89,89,89,89,89,89,89,89,89,80 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,82,89,89,89,48,89,89,89,89,89,89,89,89,89,89,80 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,82,89,104,105,89,89,89,104,105,89,89,89,104,105,89,80 },
				{ 2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,80,82,89,120,121,89,89,89,120,121,89,89,89,120,121,89,80 },
				{ 17,18,17,18,18,72,73,73,74,17,17,17,18,18,17,80,82,89,89,89,89,89,89,89,89,89,89,89,89,89,89,80 },
				{ 1,1,1,1,1,88,89,89,90,1,1,1,1,1,1,80,82,89,89,89,89,89,89,89,89,89,89,89,89,89,89,80 },
				{ 1,5,6,1,1,88,104,105,90,1,1,1,21,22,1,80,82,89,89,89,89,89,89,89,89,89,89,89,89,89,89,80 },
				{ 1,1,1,1,1,88,120,121,90,6,1,1,1,1,1,80,81,65,65,65,65,65,65,65,65,65,66,48,89,89,89,80 },
				{ 1,1,1,21,22,88,89,89,90,1,1,1,1,1,1,80,81,81,81,81,81,81,81,81,81,81,82,48,89,89,89,80 },
				{ 1,1,1,1,1,88,89,89,90,1,1,35,36,37,1,1,88,89,89,89,89,89,89,89,89,89,89,48,89,89,89,80 },
				{ 1,1,1,1,1,88,89,89,90,1,1,51,52,53,1,1,88,89,89,89,89,89,89,89,89,89,89,48,89,89,89,80 },
				{ 1,1,1,1,1,88,89,89,90,34,32,49,49,49,34,1,88,89,104,105,89,89,89,104,105,89,89,89,104,105,89,80 },
				{ 1,1,32,33,34,88,89,89,90,49,49,49,50,49,49,33,88,89,120,121,89,89,89,120,121,89,89,89,120,121,89,80 },
				{ 1,32,49,49,49,88,89,89,90,49,49,49,49,50,49,49,88,89,89,89,89,89,89,89,89,89,89,89,89,89,89,80 },
				{ 32,49,50,49,49,88,89,89,90,49,49,49,49,49,49,49,88,89,89,89,89,89,89,89,89,89,89,89,89,89,89,80 },
				{ 65,65,65,65,65,65,65,65,65,65,65,65,65,65,65,65,65,65,65,65,65,65,65,65,65,65,65,65,65,65,65,81 },
				{ 81,81,81,81,81,81,81,81,81,81,81,81,81,81,81,81,81,81,81,81,81,81,81,81,81,81,81,81,81,81,81,81 },
				{ 81,81,96,97,81,81,81,96,97,81,81,81,96,97,81,81,81,81,96,97,81,81,81,96,97,81,81,81,96,97,81,81 }
			};
			#endregion

			#region 下層レイヤーの作成
			int[,] LL = new int[45, 32]
			{
				{ 78,79,0,0,0,0,78,79,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
				{ 94,95,0,0,0,0,94,95,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
				{ 110,111,0,0,0,0,110,111,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
				{ 126,127,0,0,0,0,126,127,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
				{ 142,143,0,0,0,0,142,143,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
				{ 158,159,0,0,0,0,158,159,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,79,48,0,48,0,48,0,78,79,0,0,0,0 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,94,95,48,0,0,0,48,0,94,95,0,0,0,0 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,110,111,0,0,48,0,48,0,110,111,0,0,0,0 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,126,127,0,0,0,0,0,0,126,127,0,0,0,0 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,142,143,48,0,0,0,48,0,142,143,0,0,0,0 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,158,159,48,0,48,0,0,0,158,159,0,0,0,0 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
				{ 0,0,0,0,0,76,158,159,75,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
				{ 0,0,0,0,0,163,164,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
				{ 0,0,0,0,0,0,0,163,164,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
				{ 0,0,0,0,0,163,164,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 }
			};
			#endregion

			#region 上層レイヤーの作成
			int[,] UL = new int[45, 32]
			{
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
				{ 0,0,70,71,0,0,0,0,0,0,0,0,70,71,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
				{ 0,0,86,87,0,0,0,0,0,0,0,0,86,87,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
				{ 0,0,102,103,0,0,0,0,0,0,0,0,102,103,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
				{ 0,0,118,119,0,0,0,0,0,0,0,0,118,119,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
				{ 0,0,134,135,0,0,0,0,0,0,0,0,134,135,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
				{ 0,68,150,151,67,0,0,0,0,0,0,68,150,151,67,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 }
			};
			#endregion

			#region 当たり判定レイヤー
			int[,] CL = new int[45, 32]
			{
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1 },
				{ 0,0,0,0,0,0,0,0,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1 },
				{ 0,0,0,0,0,0,0,0,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1 },
				{ 1,1,1,1,1,2,1,1,1,1,2,1,0,0,1,1,1,1,1,0,2,0,2,0,2,0,0,0,0,0,1,1 },
				{ 0,0,0,0,0,2,0,0,0,0,2,0,0,0,0,0,0,0,0,0,2,0,0,0,2,0,0,0,0,0,1,1 },
				{ 0,0,0,0,0,2,0,0,0,0,1,1,0,0,0,0,0,0,0,0,0,0,2,0,2,0,0,0,0,0,1,1 },
				{ 0,0,0,0,0,2,0,0,0,0,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1 },
				{ 0,0,0,0,0,2,0,0,1,1,1,1,0,0,0,0,0,0,0,0,2,0,0,0,2,0,0,0,0,0,1,1 },
				{ 0,0,0,0,0,2,0,0,1,1,1,1,0,0,0,0,0,0,0,0,2,0,2,0,0,0,0,0,0,0,1,1 },
				{ 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,2,1,1,1 },
				{ 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,2,1,1,1 },
				{ 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,2,1,1,1 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,2,1,1,1 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,2,1,1,1 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,2,1,1,1 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,2,1,1,1,1,1,1,1,1,1,1,1 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,2,1,1,1,1,1,1,1,1,1,1,1 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,2,0,0,0,0,0,0,0,0,0,0,1 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,2,0,0,0,0,0,0,0,0,0,0,1 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
				{ 0,0,0,0,0,2,2,0,0,0,0,0,0,0,0,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,2,0,0,0,1 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,2,0,0,0,1 },
				{ 0,0,0,0,0,0,0,2,2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,2,0,0,0,1 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,2,0,0,0,1 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
				{ 0,0,0,0,0,2,2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
				{ 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
				{ 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1 },
				{ 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1 },
				{ 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1 }
			};
			#endregion

			EntityLayer[9, 3] = "Enemy1";
			//EntityLayer[20, 11] = "Enemy1";
			//EntityLayer[25, 11] = "Enemy1";
			EntityLayer[21, 24] = "Enemy1";
			EntityLayer[26, 33] = "Enemy1";
			EntityLayer[21, 41] = "Enemy1";
			//EntityLayer[7, 41] = "Enemy1";

			Sections.Add(new Section(new Rectangle(0, 0, 16, 15), false, true, true, false));
			Sections.Add(new Section(new Rectangle(16, 0, 16, 15), false, false, false, true));
			Sections.Add(new Section(new Rectangle(16, 15, 16, 30), false, true, false, true));
			Sections.Add(new Section(new Rectangle(0, 30, 16, 15), false, true, true, false));
			CurrentlySectionID = 1;
			//*/

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
			//*/

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
		public static void LoadContent(ContentManager Content)
		{
			Texture = Content.Load<Texture2D>("Image/link_stage_mapchip.png");
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
		public void Update(GameTime GameTime)
		{
			if (!StopEntitySpawn)
			{
				// 画面に入ったマスに配置されているエンティティを作成
				Point ViewMap = Main.Camera.ViewMap;
				Point OldViewMap = Main.Camera.OldViewMap;
				Rectangle CurrentlySectionArea = Sections[CurrentlySectionID].Area;
				if (ViewMap != OldViewMap)
				{
					// 1フレーム前に描画されていたマップ範囲
					Rectangle OldViewMapRange = new Rectangle(OldViewMap.X, OldViewMap.Y, (Const.GameScreenWidth / Const.MapchipTileSize) + 1, (Const.GameScreenHeight / Const.MapchipTileSize) + 1);

					// 画面内のマスに配置されているエンティティ
					for (int x = ViewMap.X; x < ViewMap.X + (Const.GameScreenWidth / Const.MapchipTileSize) + 1; x++)
					{
						for (int y = ViewMap.Y; y < ViewMap.Y + (Const.GameScreenHeight / Const.MapchipTileSize) + 1; y++)
						{
							// 現在のセクション内で、且つ新たに画面に入った範囲か確認する
							if (!OldViewMapRange.Contains(x, y) && CurrentlySectionArea.Contains(x, y))
							{
								if (EntityLayer[x, y] != "" && EntityLayer[x, y] != null)
								{
									string EntityName = EntityLayer[x, y];
									Point SpawnPosition = new Point(x * Const.MapchipTileSize + Const.MapchipTileSize / 2, y * Const.MapchipTileSize + (Const.MapchipTileSize - 1));
									// このマスより生成されたエンティティがいなければ作成する
									if (!Main.Entities.Exists(E => E.IsFromMap && E.FromMapPosition == new Point(x, y)))
									{
										Entity.Create(EntityName, SpawnPosition, true, new Point(x, y));
									}
								}
							}
						}
					}
				}
			}

			// タイルアニメーションの管理
			if (AnimationTiles != null)
			{
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
			}
			FrameCounter++;
		}

		/// <summary>
		/// 描画
		/// </summary>
		public void Draw(GameTime GameTime, SpriteBatch SpriteBatch)
		{
			Point ViewMap = Main.Camera.ViewMap;
			Point OldViewMap = Main.Camera.OldViewMap;
			for (int x = ViewMap.X; x < ViewMap.X + (Const.GameScreenWidth / Const.MapchipTileSize) + 1; x++)
			{
				for (int y = ViewMap.Y; y < ViewMap.Y + (Const.GameScreenHeight / Const.MapchipTileSize) + 1; y++)
				{
					// 画面外を読んでしまわないようにする
					if (x >= 0 && x < Size.Width && y >= 0 && y < Size.Height)
					{
						Point Position = new Point(x * Const.MapchipTileSize, y * Const.MapchipTileSize);

						//各レイヤーを描画
						DrawTile(SpriteBatch, BGLayer[x, y], Position, (float)Const.DrawOrder.BGLayer / (float)Const.DrawOrder.MAX);
						DrawTile(SpriteBatch, LowerLayer[x, y], Position, (float)Const.DrawOrder.LowerLayer / (float)Const.DrawOrder.MAX);
						DrawTile(SpriteBatch, UpperLayer[x, y], Position, (float)Const.DrawOrder.UpperLayer / (float)Const.DrawOrder.MAX);

						// デバッグ描画 (地形判定)
						if (Global.Debug)
						{
							if (CollisionLayer[x, y] == (int)CollisionTypes.Wall)
							{
								SpriteBatch.DrawRectangle(new Rectangle(Const.MapchipTileSize * x, Const.MapchipTileSize * y, Const.MapchipTileSize, Const.MapchipTileSize), Color.Green);
								SpriteBatch.DrawRectangle(new Rectangle(Const.MapchipTileSize * x, Const.MapchipTileSize * y, Const.MapchipTileSize, Const.MapchipTileSize), Color.Lime * 0.2f, true);
							}
							if (CollisionLayer[x, y] == (int)CollisionTypes.Ladder)
							{
								SpriteBatch.DrawRectangle(new Rectangle(Const.MapchipTileSize * x, Const.MapchipTileSize * y, Const.MapchipTileSize, Const.MapchipTileSize), Color.Orange);
								SpriteBatch.DrawRectangle(new Rectangle(Const.MapchipTileSize * x, Const.MapchipTileSize * y, Const.MapchipTileSize, Const.MapchipTileSize), Color.Yellow * 0.2f, true);
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// 別のセクションへ移動
		/// </summary>
		/// <param name="TargetSectionID">移動先のセクションID</param>
		public void ChangeSection(int TargetSectionID)
		{
			CurrentlySectionID = TargetSectionID;
		}

		/// <summary>
		/// 画面内に配置された全てのエンティティをスポーンさせる
		/// </summary>
		public void SpawnAllEntities()
		{
			if (!StopEntitySpawn)
			{
				Point ViewMap = Main.Camera.ViewMap;
				Rectangle CurrentlySectionArea = Sections[CurrentlySectionID].Area;
				// 画面内のマスに配置されているエンティティ
				for (int x = ViewMap.X; x < ViewMap.X + (Const.GameScreenWidth / Const.MapchipTileSize) + 1; x++)
				{
					for (int y = ViewMap.Y; y < ViewMap.Y + (Const.GameScreenHeight / Const.MapchipTileSize) + 1; y++)
					{
						// 現在のセクション内で、且つ新たに画面に入った範囲か確認する
						if (CurrentlySectionArea.Contains(x, y))
						{
							if (EntityLayer[x, y] != "" && EntityLayer[x, y] != null)
							{
								string EntityName = EntityLayer[x, y];
								Point SpawnPosition = new Point(x * Const.MapchipTileSize + Const.MapchipTileSize / 2, y * Const.MapchipTileSize + (Const.MapchipTileSize - 1));
								Entity.Create(EntityName, SpawnPosition, true, new Point(x, y));
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// Map 上における指定した座標の当たり判定IDを返す
		/// </summary>
		/// <param name="Point">当たり判定IDを取得したい Map 上のワールド座標</param>
		/// <returns>指定した座標の当たり判定ID。マップ外を取得しようとした場合は常に 0 を返す。</returns>
		public CollisionTypes PointToCollisionIndex(Point WorldPosition)
		{
			CollisionTypes CollisionIndex = CollisionTypes.Air;
			Point MapPosition = new Point(WorldPosition.X / Const.MapchipTileSize, WorldPosition.Y / Const.MapchipTileSize);
			if (MapPosition.X >= 0 && MapPosition.X < Size.Width && MapPosition.Y >= 0 && MapPosition.Y < Size.Height)
			{
				CollisionIndex = (CollisionTypes)CollisionLayer[MapPosition.X, MapPosition.Y];
			}
			return CollisionIndex;
		}

		/// <summary>
		/// Map 上における指定した座標が梯子の上辺かどうかを調べる
		/// </summary>
		/// <param name="Point">梯子の上辺かどうかを調べたい Map 上のワールド座標</param>
		/// <returns>指定した座標が梯子の上辺かどうか。マップ外を取得しようとした場合は常に false を返す。</returns>
		public bool CheckPointLadderTop(Point WorldPosition)
		{
			bool Result = false;
			CollisionTypes CollisionIndex = CollisionTypes.Air;
			Point MapPosition = new Point(WorldPosition.X / Const.MapchipTileSize, WorldPosition.Y / Const.MapchipTileSize);
			if (MapPosition.X >= 0 && MapPosition.X < Size.Width && MapPosition.Y >= 0 && MapPosition.Y < Size.Height)
			{
				CollisionIndex = (CollisionTypes)CollisionLayer[MapPosition.X, MapPosition.Y];
			}
			if (CollisionIndex == CollisionTypes.Ladder)
			{
				MapPosition.Y -= 1;
				CollisionIndex = (CollisionTypes)CollisionLayer[MapPosition.X, MapPosition.Y];
				if (CollisionIndex == CollisionTypes.Air)
				{
					Result = true;
				}
			}
			return Result;
		}

		#region プライベート関数

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

		#endregion

	}
}
