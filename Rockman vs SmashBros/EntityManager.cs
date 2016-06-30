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
	/// Entity クラスの Manager
	/// </summary>
	public partial class Entity
	{
		/// <summary>
		/// エンティティを作成
		/// </summary>
		/// <param name="EntityName">作成するエンティティの名前</param>
		/// <param name="Positiuon">作成する座標</param>
		public static void Create(string EntityName, Point Position, bool IsFromMap, Point FromMapPosition)
		{
			if (EntityName == "Enemy1")
			{
				Main.Entities.Add(new Enemy1(Position, IsFromMap, FromMapPosition));
			}
		}
		public static void Create(string EntityName, Point Position)
		{
			Create(EntityName, Position, false, new Point());
		}

		/// <summary>
		/// 全てのエンティティを削除
		/// </summary>
		public static void DestroyAll()
		{
			while (Main.Entities.Count > 0)
			{
				Main.Entities.RemoveAt(0);
			}
		}

	}
}
