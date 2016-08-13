using System.Collections.Generic;
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
		#region メンバーの宣言

		private static List<ReservData> ReservDatas = new List<ReservData>();   // エンティティの追加予約
		private struct ReservData                                               // エンティティの追加予約データ構造体
		{
			public string EntityName;
			public Point Position;
			public bool IsFromMap;
			public Point FromMapPosition;
			public ReservData(string EntityName, Point Position, bool IsFromMap, Point FromMapPosition)
			{
				this.EntityName = EntityName;
				this.Position = Position;
				this.IsFromMap = IsFromMap;
				this.FromMapPosition = FromMapPosition;
			}
		}

		#endregion

		/// <summary>
		/// エンティティの追加を予約
		/// </summary>
		/// <param name="EntityName">エンティティの名前</param>
		/// <param name="Position">エンティティの座標</param>
		/// <param name="IsFromMap">エンティティがマップにより作成されたかどうか</param>
		/// <param name="FromMapPosition">エンティティがマップにより作成された場合の作成元の座標 (マップ上のマス数)</param>
		public static void AddReserv(string EntityName, Point Position, bool IsFromMap = false, Point FromMapPosition = new Point())
		{
			ReservDatas.Add(new ReservData(EntityName, Position, IsFromMap, FromMapPosition));
		}

		/// <summary>
		/// 予約されたエンティティ追加を実行
		/// </summary>
		public static void ExecuteReserv()
		{
			foreach (var Reserv in ReservDatas)
			{
				Create(Reserv.EntityName, Reserv.Position, Reserv.IsFromMap, Reserv.FromMapPosition);
			}

			ClearReserv();
		}

		/// <summary>
		/// エンティティの追加予約をクリア
		/// </summary>
		private static void ClearReserv()
		{
			ReservDatas.Clear();
		}

		/// <summary>
		/// エンティティを作成
		/// </summary>
		/// <param name="EntityName">エンティティの名前</param>
		/// <param name="Positiuon">エンティティの座標</param>
		/// <param name="IsFromMap">エンティティがマップにより作成されたかどうか</param>
		/// <param name="Position">エンティティがマップにより作成された場合の作成元の座標 (マップ上のマス数)</param>
		public static void Create(string EntityName, Point Position, bool IsFromMap = false, Point FromMapPosition = new Point())
		{
			var Entities = Main.Entities;
			// エンティティ名による場合分け
			switch (EntityName)
			{
				// 破壊エフェクト
				case "DestroyEffect1":
					Entities.Add(new DestroyEffect1(Position, IsFromMap, FromMapPosition));
					break;
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
