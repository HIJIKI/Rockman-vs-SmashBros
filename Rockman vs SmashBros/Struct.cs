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
}
