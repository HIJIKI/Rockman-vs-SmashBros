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
			var Entities = Main.Entities;
			// エンティティ名による場合分け
			switch (EntityName)
			{
				// チェックポイント
				case "CheckPoint":
					Entities.Add(new CheckPoint(Position, IsFromMap, FromMapPosition));
					break;
				// ハイラル兵
				case "HyruleSoldier":
					Entities.Add(new HyruleSoldier(Position, IsFromMap, FromMapPosition));
					break;
				// ハイラル兵 (攻撃モード)
				case "HyruleSoldier:Attacking":
					Entities.Add(new HyruleSoldier(Position, IsFromMap, FromMapPosition, true));
					break;
				// 足場1
				case "Platform1":
					Entities.Add(new Platform1(Position, IsFromMap, FromMapPosition));
					break;
				// 足場2
				case "Platform2":
					Entities.Add(new Platform2(Position, IsFromMap, FromMapPosition));
					break;
				// エラーエンティティ
				default:
					Entities.Add(new ErrorEntity(Position, IsFromMap, FromMapPosition));
					break;
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
			var Entities = Main.Entities;
			foreach (var Entity in Entities)
			{
				Entity.Destroy();
			}
		}

	}
}
