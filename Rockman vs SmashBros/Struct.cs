using Microsoft.Xna.Framework.Graphics;

namespace Rockman_vs_SmashBros
{
	/// <summary>
	/// Size 構造体
	/// </summary>
	/// 高さと横幅の管理に使用する。
	public struct Size
	{
		public int Width;
		public int Height;
		public Size(int Width, int Height)
		{
			this.Width = Width;
			this.Height = Height;
		}
	}

	/// <summary>
	/// Position 構造体
	/// </summary>
	/// 座標の管理に使用する。
	public struct Position
	{
		public int X;
		public int Y;
		public Position(int X, int Y)
		{
			this.X = X;
			this.Y = Y;
		}
	}

	/// <summary>
	/// Mapchip 構造体
	/// </summary>
	/// マップチップのテクスチャを読み込んでおく。
	public struct Mapchip
	{
		public Texture2D HyruleCastle;
	}
}
