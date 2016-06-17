using System;
using System.Diagnostics;
using System.IO;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Rockman_vs_SmashBros
{
	/// <summary>
	/// MapData クラス
	/// </summary>
	public class MapData
	{
		public string MapchipFile;                                  // マップチップのファイル名
		public Size Size;                                           // マップの縦横マス数
		public int[,] BGLayer;                                      // 背景レイヤー
		public int[,] LowerLayer;                                   // 下層レイヤー
		public int[,] UpperLayer;                                   // 上層レイヤー
		public string[,] EntityLayer;                               // エンティティレイヤー

		public int[,] Map1;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public MapData() { }
		public MapData(string MapchipFile, int Width, int Height)
		{
			this.MapchipFile = MapchipFile;
			this.Size = new Size(Width, Height);
			this.BGLayer = new int[Size.Width, Size.Height];
			this.LowerLayer = new int[Size.Width, Size.Height];
			this.UpperLayer = new int[Size.Width, Size.Height];
			this.EntityLayer = new string[Size.Width, Size.Height];

			Map1 = new int[15, 16]
			{
				{ 2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2 },
				{ 2,3,4,2,2,2,2,2,2,2,2,19,20,2,2,2 },
				{ 17,18,17,18,17,18,17,18,17,18,17,18,17,18,17,18 },
				{ 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1 },
				{ 1,1,1,21,22,1,1,1,1,1,1,1,1,5,6,1 },
				{ 1,1,1,1,1,1,1,1,21,22,1,1,1,1,1,1 },
				{ 1,5,6,1,1,1,1,1,1,1,1,1,1,1,5,6 },
				{ 1,1,1,1,1,1,1,1,1,1,1,1,1,48,149,1 },
				{ 1,1,1,1,1,1,1,35,36,37,1,1,1,48,149,1 },
				{ 1,1,1,1,1,1,1,51,52,53,1,1,1,48,149,1 },
				{ 32,33,34,1,1,1,64,65,65,66,34,1,1,48,149,1 },
				{ 49,64,65,66,1,32,80,81,81,82,49,34,64,65,66,32 },
				{ 49,80,81,82,33,49,80,81,81,82,49,49,80,81,82,49 },
				{ 49,80,81,82,49,49,80,81,81,82,49,49,80,81,82,49 },
				{ 49,80,81,82,49,49,80,81,81,82,49,49,80,81,82,49 }
			};
		}
		/// <summary>
		/// デストラクタ
		/// </summary>
		~MapData()
		{
		}
	}
}
