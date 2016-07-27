using System;
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
	/// Bullet クラス
	/// </summary>
	public class Bullet : Entity
	{
		#region メンバーの宣言

		public bool IsRepelled;										// 弾かれた弾かどうか

		#endregion

		/// <summary>
		/// 弾が弾かれた時の処理
		/// </summary>
		public virtual void Repel()
		{
			IsRepelled = true;
			MoveDistance.Y = -Math.Abs(MoveDistance.X);
			MoveDistance.X *= -1;
		}
	}
}
