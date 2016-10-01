using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
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
    /// MapFile クラス
    /// </summary>
    [Serializable()]
    public class MapFile
    {
        #region メンバーの宣言

        public int SizeWidth;                                       // マップの横幅 (マス数)
        public int SizeHeight;                                      // マップの高さ (マス数)
        public AnimationTile[] AnimationTiles;                      // アニメーションタイルのデータ
        public List<Section> Sections;                              // マップ内セクションのデータ
        public int CurrentlySectionID;                              // 現在いるセクションのID
        public Tile[,] BGLayer;                                     // 背景レイヤー
        public Tile[,] LowerLayer;                                  // 下層レイヤー
        public Tile[,] UpperLayer;                                  // 上層レイヤー
        public TerrainTypes[,] TerrainLayer;                        // 地形レイヤー
        public string[,] EntityLayer;                               // エンティティレイヤー
        public int FrameCounter;                                    // フレームカウンター
        public bool StopEntitySpawn;                                // エンティティのスポーンを停止するフラグ
        public int SpawnPositionOnMapX;                             // ステージ開始時のプレイヤーX位置 (マス)
        public int SpawnPositionOnMapY;                             // ステージ開始時のプレイヤーY位置 (マス)

        // タイル1枚のデータ構造体
        [Serializable()]
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
        [Serializable()]
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
        [Serializable()]
        public struct Section
        {
            public int AreaX;                                       // セクションのX開始位置 (マス数)
            public int AreaY;                                       // セクションのY開始位置 (マス数)
            public int AreaWidth;                                   // セクションの横幅 (マス数)
            public int AreaHeight;                                  // セクションの高さ (マス数)
            public bool TopIsWall;                                  // 上の辺を壁として扱うかどうか
            public bool BottomIsWall;                               // 下の辺を壁として扱うかどうか
            public bool LeftIsWall;                                 // 左の辺を壁として扱うかどうか
            public bool RightIsWall;                                // 右の辺を壁として扱うかどうか
            public Section(int AreaX, int AreaY, int AreaWidth, int AreaHeight, bool TopIsWall, bool BottomIsWall, bool LeftIsWall, bool RightIsWall)
            {
                this.AreaX = AreaX;
                this.AreaY = AreaY;
                this.AreaWidth = AreaWidth;
                this.AreaHeight = AreaHeight;
                this.TopIsWall = TopIsWall;
                this.BottomIsWall = BottomIsWall;
                this.LeftIsWall = LeftIsWall;
                this.RightIsWall = RightIsWall;
            }
        }

        // 当たり判定ボックスの種類
        [Serializable()]
        public enum TerrainTypes
        {
            Air,                                                    // 空気
            Wall,                                                   // 壁
            Ladder,                                                 // 梯子
            OneWay,                                                 // 一方通行床
            LeftSlope1of1,                                          // 左スロープ 1/1
            RightSlope1of1,                                         // 右スロープ 1/1
            LeftSlope1of2,                                          // 左スロープ 1/2
            LeftSlope2of2,                                          // 左スロープ 2/2
            RightSlope1of2,                                         // 右スロープ 1/2
            RightSlope2of2,                                         // 右スロープ 2/2
            LeftSlope1of3,                                          // 左スロープ 1/3
            LeftSlope2of3,                                          // 左スロープ 2/3
            LeftSlope3of3,                                          // 左スロープ 3/3
            RightSlope1of3,                                         // 右スロープ 1/3
            RightSlope2of3,                                         // 右スロープ 2/3
            RightSlope3of3,                                         // 右スロープ 3/3
            LeftSlope1of4,                                          // 左スロープ 1/4
            LeftSlope2of4,                                          // 左スロープ 2/4
            LeftSlope3of4,                                          // 左スロープ 3/4
            LeftSlope4of4,                                          // 左スロープ 4/4
            RightSlope1of4,                                         // 右スロープ 1/4
            RightSlope2of4,                                         // 右スロープ 2/4
            RightSlope3of4,                                         // 右スロープ 3/4
            RightSlope4of4,                                         // 右スロープ 4/4
        }

        #endregion

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MapFile()
        {
            AnimationTiles = new AnimationTile[0];
            Sections = new List<Section>();
            BGLayer = new Tile[0, 0];
            LowerLayer = new Tile[0, 0];
            UpperLayer = new Tile[0, 0];
            TerrainLayer = new TerrainTypes[0, 0];
            EntityLayer = new string[0, 0];
        }

        /// <summary>
        /// マップをファイルに保存
        /// </summary>
        /// <param name="MapFile">保存するマップファイルオブジェクト</param>
        /// <param name="FilePath">保存先のファイルパス</param>
        public static void SaveToFile(MapFile MapFile, string FilePath)
        {
            FileStream FileStream = new FileStream(FilePath, FileMode.Create, FileAccess.Write);
            BinaryFormatter BinaryFormatter = new BinaryFormatter();
            BinaryFormatter.Serialize(FileStream, MapFile);
            FileStream.Close();
        }

        /// <summary>
        /// マップをファイルから読む込む
        /// </summary>
        /// <param name="FilePath">読み込み元のファイルパス</param>
        /// <returns>読み込んだマップファイルオブジェクト</returns>
        public static MapFile LoadFromFile(string FilePath)
        {
            FileStream FileStream = new FileStream(FilePath, FileMode.Open, FileAccess.Read);
            BinaryFormatter BinaryFormatter = new BinaryFormatter();
            object Result = BinaryFormatter.Deserialize(FileStream);
            FileStream.Close();
            return (MapFile)Result;
        }

    }
}
