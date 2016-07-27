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
    /// DamageDetail クラス
    /// </summary>
    public class DamageDetail
    {
        #region メンバーの宣言

        public Types Type;											// ダメージの属性
        public enum Types											// ダメージの属性構造体
        {
            Normal,
			RockBuster1
        }

        public int Damage;                                          // ダメージの数値
		public Entity Owner;                                        // ダメージのオーナーエンティティ

		#endregion

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="Damage">ダメージの値</param>
		/// <param name="Type">ダメージの属性</param>
		/// <param name="Owner">ダメージのオーナーエンティティ</param>
		public DamageDetail(int Damage, Types Type = Types.Normal, Entity Owner = null)
        {
			this.Damage = Damage;
			this.Type = Type;
			this.Owner = Owner;
		}

        /// <summary>
        /// 初期化
        /// </summary>
        public void Initialize()
        {
        }

    }
}
