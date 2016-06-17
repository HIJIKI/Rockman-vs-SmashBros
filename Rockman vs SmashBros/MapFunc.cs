using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Rockman_vs_SmashBros
{
	/// <summary>
	/// マップ関係の処理
	/// </summary>
	public partial class MyGame
	{
		/// <summary>
		/// マップを描画
		/// </summary>
		public void DrawMap(MapData MapData)
		{
			for (int x = 0; x < MapData.Size.Width; x++)
			{
				for (int y = 0; y < MapData.Size.Height; y++)
				{
					//TODO: テスト用コードを修正する。現状は Mapchip が固定になっている。
					Position Position = new Position(x * MAP_CHIP_SIZE, y * MAP_CHIP_SIZE);

					//各レイヤーを描画
					DrawMapchip(Mapchip.HyruleCastle, MapData.Map1[y, x], Position, (float)DrawOrder.BGLayer / (float)DrawOrder.MAX_LAYER);
					//DrawMapchip(Mapchip.HyruleCastle, MapData.LowerLayer[x, y], Position, DrawOrder.LowerLayer);
					//DrawMapchip(Mapchip.HyruleCastle, MapData.UpperLayer[x, y], Position, DrawOrder.UpperLayer);
				}
			}
		}

		/// <summary>
		/// マップチップを描画
		/// </summary>
		public void DrawMapchip(Texture2D Mapchip, int Index, Position Position, float DrawOrder)
		{
			// マップチップのIndexをRectangleに変換する
			Size MapchipSize = new Size(Mapchip.Width / MAP_CHIP_SIZE, Mapchip.Height / MAP_CHIP_SIZE);             //マップチップの縦横枚数
			Rectangle SourceRect = new Rectangle((Index % MapchipSize.Width) * MAP_CHIP_SIZE, (Index / MapchipSize.Width) * MAP_CHIP_SIZE, MAP_CHIP_SIZE, MAP_CHIP_SIZE);

			SpriteBatch.Draw(Mapchip, new Vector2(Position.X, Position.Y), SourceRect, Color.White, 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, DrawOrder);
		}
	}
}
