using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Design;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Storage;
using C3.XNA;

namespace Rockman_vs_SmashBros
{
	/// <summary>
	/// Primitives2DWrapper クラス
	/// </summary>
	static class Primitives2DWrapper
	{

		/// <summary>
		/// 点を描画する
		/// </summary>
		/// <param name="SpriteBatch">描画に使用する SpriteBatch</param>
		/// <param name="Point">点の座標</param>
		/// <param name="Color">点の色</param>
		public static void DrawPixel(this SpriteBatch SpriteBatch, Vector2 Point, Color Color)
		{
			Primitives2D.PutPixel(SpriteBatch, Point, Color);
		}

		/// <summary>
		/// 2点を結ぶ直線を描画する
		/// </summary>
		/// <param name="SpriteBatch">描画に使用する SpriteBatch</param>
		/// <param name="StartPoint">線の始点</param>
		/// <param name="EndPoint">線の終点</param>
		/// <param name="Color">線の色</param>
		/// <param name="DrawEndPointPixel">終点部分の1ピクセルを描画するか</param>
		public static void DrawLine(this SpriteBatch SpriteBatch, Vector2 StartPoint, Vector2 EndPoint, Color Color, bool DrawEndPointPixel = false)
		{
			//必要に応じて終点のピクセルを描画
			if (DrawEndPointPixel)
			{
				DrawPixel(SpriteBatch, EndPoint, Color);
			}

			// Primitives2D クラスの DrawLine メソッドで X 座標が +1 ドットずれる問題を回避
			StartPoint.X += 1;
			EndPoint.X += 1;
			Primitives2D.DrawLine(SpriteBatch, StartPoint, EndPoint, Color);
		}

		/// <summary>
		/// 矩形を描画する
		/// </summary>
		/// <param name="SpriteBatch">描画に使用する SpriteBatch</param>
		/// <param name="Rectangle">矩形の位置と大きさ</param>
		/// <param name="Color">矩形の色</param>
		/// <param name="Fill">矩形を塗りつぶすか</param>
		public static void DrawRectangle(this SpriteBatch SpriteBatch, Rectangle Rectangle, Color Color, bool Fill = false)
		{
			// 縦横いずれかのサイズが 1 未満の場合は描画しない
			if (Rectangle.Width < 1 || Rectangle.Height < 1)
			{
				return;
			}
			// 塗りつぶす場合
			if (Fill)
			{
				Primitives2D.FillRectangle(SpriteBatch, Rectangle, Color);
			}
			// 塗りつぶさない場合
			else
			{
				// Primitives2D クラスの DrawRectangle メソッドで Width, Height ともに +1 ドットずれる問題を回避
				Rectangle.Width -= 1;
				Rectangle.Height -= 1;
				Primitives2D.DrawRectangle(SpriteBatch, Rectangle, Color);
			}
		}

		/// <summary>
		/// 円を描画する
		/// </summary>
		/// <param name="SpriteBatch">描画に使用する SpriteBatch</param>
		/// <param name="Position">円の中心座標</param>
		/// <param name="Radius">円の半径</param>
		/// <param name="Definition">円の精細度 (描画時の頂点の数)</param>
		/// <param name="Color">円の色</param>
		public static void DrawCircle(this SpriteBatch SpriteBatch, Vector2 Position, float Radius, int Definition, Color Color)
		{
			Primitives2D.DrawCircle(SpriteBatch, Position, Radius, Definition, Color);
		}

	}
}
