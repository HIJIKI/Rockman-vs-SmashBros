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
	/// CheckPoint クラス
	/// </summary>
	public class CheckPoint : Entity
	{
		#region メンバーの宣言

		#endregion

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public CheckPoint(Point Position, bool IsFromMap, Point FromMapPosition)
		{
			IsAlive = true;
			this.Position = Position.ToVector2();
			this.IsFromMap = IsFromMap;
			this.FromMapPosition = FromMapPosition;

			Main.SetSpawnPoint(FromMapPosition);
			Destroy();
		}

	}
}
