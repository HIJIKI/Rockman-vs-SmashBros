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
	/// Camera クラス
	/// </summary>
	public static class Camera
	{
		#region メンバーの宣言
		public static Point Position;                                      // カメラの座標
		public static Vector2 ScrollSourcePosition;                        // スクロール元の座標
		public static Vector2 ScrollDestinationPosition;                   // スクロール先の座標
		public static Point ViewMap;                                       // 見えているマップの範囲(マス数)
		public static Point OldViewMap;                                    // 1フレーム前の見えているマップの範囲(マス数)
		public static bool InChangeSection;                                // 別のセクションに移動中かどうか
		public static int ChangeSectionFrame;                              // セクションの移動中のフレームカウンター
		#endregion

		/// <summary>
		/// 初期化
		/// </summary>
		public static void Initialize()
		{

		}

		/// <summary>
		/// リソースの確保
		/// </summary>
		public static void LoadContent(ContentManager Content)
		{
		}

		/// <summary>
		/// リソースの破棄
		/// </summary>
		public static void UnloadContent()
		{
		}

		/// <summary>
		/// フレームの更新
		/// </summary>
		/// <param name="PlayerDrawPosition">プレイヤーの描画座標</param>
		public static void Update(GameTime GameTime, Point PlayerDrawPosition)
		{
			if (!InChangeSection)
			{
				// カメラをプレイヤーに追従
				Rectangle CurrentlySectionArea = Map.Sections[Map.CurrentlySectionID].Area;
				Position.X = PlayerDrawPosition.X - Const.GameScreenWidth / 2;
				if (Position.X < Const.MapchipTileSize * CurrentlySectionArea.X)
				{
					Position.X = Const.MapchipTileSize * CurrentlySectionArea.X;
				}
				else if (Position.X + Const.GameScreenWidth > Const.MapchipTileSize * CurrentlySectionArea.X + Const.MapchipTileSize * CurrentlySectionArea.Width)
				{
					Position.X = Const.MapchipTileSize * CurrentlySectionArea.X + Const.MapchipTileSize * CurrentlySectionArea.Width - Const.GameScreenWidth;
				}

				Position.Y = PlayerDrawPosition.Y - Const.GameScreenHeight / 2;
				if (Position.Y < Const.MapchipTileSize * CurrentlySectionArea.Y)
				{
					Position.Y = Const.MapchipTileSize * CurrentlySectionArea.Y;
				}
				else if (Position.Y + Const.GameScreenHeight > Const.MapchipTileSize * CurrentlySectionArea.Y + Const.MapchipTileSize * CurrentlySectionArea.Height)
				{
					Position.Y = Const.MapchipTileSize * CurrentlySectionArea.Y + Const.MapchipTileSize * CurrentlySectionArea.Height - Const.GameScreenHeight;
				}
			}
			// セクション移動中の処理
			else
			{
				ChangeSectionCalc();
			}

			// 見えている範囲を検出
			OldViewMap = ViewMap;
			ViewMap.X = Position.X / Const.MapchipTileSize;
			ViewMap.Y = Position.Y / Const.MapchipTileSize;
		}

		/// <summary>
		/// 描画
		/// </summary>
		public static void Draw(GameTime GameTime)
		{
		}

		/// <summary>
		/// セクション移動開始時の処理
		/// </summary>
		public static void StartChangeSection(Point PlayerDrawPosition)
		{
			ScrollSourcePosition = Position.ToVector2();

			// 移動後の座標を移動先セクション内に収める
			Rectangle CurrentlySectionArea = Map.Sections[Map.CurrentlySectionID].Area;
			ScrollDestinationPosition.X = PlayerDrawPosition.X - Const.GameScreenWidth / 2;
			if (ScrollDestinationPosition.X < Const.MapchipTileSize * CurrentlySectionArea.X)
			{
				ScrollDestinationPosition.X = Const.MapchipTileSize * CurrentlySectionArea.X;
			}
			else if (ScrollDestinationPosition.X + Const.GameScreenWidth > Const.MapchipTileSize * CurrentlySectionArea.X + Const.MapchipTileSize * CurrentlySectionArea.Width)
			{
				ScrollDestinationPosition.X = Const.MapchipTileSize * CurrentlySectionArea.X + Const.MapchipTileSize * CurrentlySectionArea.Width - Const.GameScreenWidth;
			}

			ScrollDestinationPosition.Y = PlayerDrawPosition.Y - Const.GameScreenHeight / 2;
			if (ScrollDestinationPosition.Y < Const.MapchipTileSize * CurrentlySectionArea.Y)
			{
				ScrollDestinationPosition.Y = Const.MapchipTileSize * CurrentlySectionArea.Y;
			}
			else if (ScrollDestinationPosition.Y + Const.GameScreenHeight > Const.MapchipTileSize * CurrentlySectionArea.Y + Const.MapchipTileSize * CurrentlySectionArea.Height)
			{
				ScrollDestinationPosition.Y = Const.MapchipTileSize * CurrentlySectionArea.Y + Const.MapchipTileSize * CurrentlySectionArea.Height - Const.GameScreenHeight;
			}

			ChangeSectionFrame = 0;
			InChangeSection = true;
		}

		#region プライベート関数

		/// <summary>
		/// セクション移動中の処理
		/// </summary>
		private static void ChangeSectionCalc()
		{
			Vector2 Source = ScrollSourcePosition;
			Vector2 Destination = ScrollDestinationPosition;
			Position.X = (int)(Source.X + (Destination.X - Source.X) / Global.ChangeSectionDuration * ChangeSectionFrame);
			Position.Y = (int)(Source.Y + (Destination.Y - Source.Y) / Global.ChangeSectionDuration * ChangeSectionFrame);

			ChangeSectionFrame++;
			if (ChangeSectionFrame > Global.ChangeSectionDuration)
			{
				InChangeSection = false;
				Position = Destination.ToPoint();
			}
		}

		#endregion

	}
}
