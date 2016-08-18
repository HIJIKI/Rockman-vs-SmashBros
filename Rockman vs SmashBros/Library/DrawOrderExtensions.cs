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
	/// DrawOrderExtensions クラス
	/// </summary>
	public static class DrawOrderExtensions
	{
		/// <summary>
		/// DrawOrder を LayerDepth ( 0.0f - 1.0f の float 型) に変換する
		/// </summary>
		/// <param name="DrawOrder">変換したい DrawOrder</param>
		public static float ToLayerDepth(this Const.DrawOrder DrawOrder)
		{
			float LayerDepth = (float)DrawOrder / (float)Const.DrawOrder.MAX;
			return LayerDepth;
		}
	}
}
