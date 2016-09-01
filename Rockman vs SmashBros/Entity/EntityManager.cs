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

		// エンティティ名の登録
		public struct Names
		{
			#region プレイヤー関連

			public const string RockBuster1_Left = "RockBuster1_Left";
			public const string RockBuster1_Right = "RockBuster1_Right";

			#endregion ----------------------------------------------

			#region ザコ敵キャラクター

			public const string HyruleSoldier = "HyruleSoldier";
			public const string HyruleSoldier_Attacking = "HyruleSoldier_Attacking";

			#endregion----------------------------------------------

			#region ステージギミック

			public const string Platform1 = "Platform1";
			public const string Platform2 = "Platform2";

			#endregion----------------------------------------------

			#region エフェクト

			public const string Explosion1 = "Explosion1";
			public const string DestroyEffect1 = "DestroyEffect1";
			public const string SlidingSmoke_Left = "SlidingSmoke_Left";
			public const string SlidingSmoke_Right = "SlidingSmoke_Right";

			#endregion----------------------------------------------

			#region その他

			public const string CheckPoint = "CheckPoint";

			#endregion----------------------------------------------
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
				#region プレイヤー関連

				// ロックバスター
				case Names.RockBuster1_Left:
					Entities.Add(new RockBuster(Position, true));
					break;
				case Names.RockBuster1_Right:
					Entities.Add(new RockBuster(Position, false));
					break;

				#endregion

				#region ザコ敵キャラクター

				// ハイラル兵
				case Names.HyruleSoldier:
					Entities.Add(new HyruleSoldier(Position, IsFromMap, FromMapPosition));
					break;
				// ハイラル兵 (攻撃モード)
				case Names.HyruleSoldier_Attacking:
					Entities.Add(new HyruleSoldier(Position, IsFromMap, FromMapPosition, true));
					break;

				#endregion

				#region ステージギミック

				// 足場1
				case Names.Platform1:
					Entities.Add(new Platform1(Position, IsFromMap, FromMapPosition));
					break;
				// 足場2
				case Names.Platform2:
					Entities.Add(new Platform2(Position, IsFromMap, FromMapPosition));
					break;

				#endregion

				#region エフェクト

				// 爆発エフェクト1
				case Names.Explosion1:
					Entities.Add(new Explosion1(Position, IsFromMap, FromMapPosition));
					break;
				// 破壊エフェクト
				case Names.DestroyEffect1:
					Entities.Add(new DestroyEffect1(Position, IsFromMap, FromMapPosition));
					break;
				// スライディングエフェクト
				case Names.SlidingSmoke_Left:
					Entities.Add(new SlidingSmoke(Position, true));
					break;
				case Names.SlidingSmoke_Right:
					Entities.Add(new SlidingSmoke(Position, false));
					break;

				#endregion

				#region その他

				// チェックポイント
				case Names.CheckPoint:
					Entities.Add(new CheckPoint(Position, IsFromMap, FromMapPosition));
					break;

				// エラーエンティティ
				default:
					Entities.Add(new ErrorEntity(Position, IsFromMap, FromMapPosition));
					break;

					#endregion
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
