using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Design;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Storage;
using System;

namespace Rockman_vs_SmashBros
{
	/// <summary>
	/// HyruleSoldier クラス
	/// </summary>
	public class HyruleSoldier : Entity
	{
		#region メンバーの宣言
		public static Texture2D Texture;                            // テクスチャ
		public static DamageDetail DamageDetail;                    // 接触時にプレイヤーに与えるダメージ

		private bool IsFaceToLeft;                                  // 左を向いているかどうか
		private int FrameCounter;                                   // フレームカウンター
		private Rectangle SearchRange;                              // プレイヤーを見つける相対範囲
		private int AnimationPattern;                               // アニメーションのパターン

		private static Sprite SearchingSprite;                      // プレイヤー捜索中のスプライト
		private static Sprite[] AttackSprites;                      // 突撃中のスプライト
		private static int[] AttackAnimationTable;                  // 突撃中のアニメーションの順番

		private Statuses Status;                                    // プレイヤーの状態
		private enum Statuses                                       // 各状態
		{
			Searching,                                              // プレイヤーを捜索中
			Attack                                                  // 突撃
		}

		private Rectangle Hitbox;                                   // 右向き時の相対的なヒットボックス
		private Rectangle AttackHitbox;                             // 右向き時の槍部分の攻撃判定ボックス (相対)
		#endregion

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public HyruleSoldier(Point Position, bool IsFromMap = false, Point FromMapPosition = new Point(), bool AttackStart = false)
		{
			this.Position = Position.ToVector2();
			this.IsFromMap = IsFromMap;
			this.FromMapPosition = FromMapPosition;
			Type = Types.Enemy;
			Health = 3;
			DamageDetail = new DamageDetail(4, DamageDetail.Types.Normal, this);

			if (AttackStart)
			{
				Status = Statuses.Attack;
			}
			else
			{
				Status = Statuses.Searching;
			}

			IsAlive = true;
			MoveDistance = Vector2.Zero;

			Hitbox = new Rectangle(-7, -23, 14, 24);
			SearchRange = new Rectangle(-80, -63, 160, 64);
			AttackHitbox = new Rectangle(7, -9, 21, 5);
		}

		/// <summary>
		/// 初期化
		/// </summary>
		public void Initialize()
		{
		}

		/// <summary>
		/// リソースの確保
		/// </summary>
		public static void LoadContent(ContentManager Content)
		{
			// テクスチャの読み込み
			Texture = Content.Load<Texture2D>("Image/Enemy/HyruleSoldier.png");

			#region 各スプライトの定義
			SearchingSprite = new Sprite(new Rectangle(0, 0, 32, 32), new Vector2(15, 28));
			AttackSprites = new Sprite[]
			{
				new Sprite(new Rectangle(48 * 0, 32, 48, 32), new Vector2(15, 28)),
				new Sprite(new Rectangle(48 * 1, 32, 48, 32), new Vector2(15, 28)),
				new Sprite(new Rectangle(48 * 2, 32, 48, 32), new Vector2(15, 28))
			};
			AttackAnimationTable = new int[] { 0, 1, 2, 1 };
			#endregion
		}

		/// <summary>
		/// リソースの破棄
		/// </summary>
		public static void UnloadContent()
		{
			Texture.Dispose();
		}

		/// <summary>
		/// フレームの更新
		/// </summary>
		public override void Update(GameTime GameTime)
		{
			Player Player = Main.Player;

			// プレイヤーを捜索中
			if (Status == Statuses.Searching)
			{
				// プレイヤーを見つけると Attack に移行
				Point DrawPosition = GetDrawPosition();
				Rectangle AbsoluteSearchRange = new Rectangle(DrawPosition.X + SearchRange.X, DrawPosition.Y + SearchRange.Y, SearchRange.Width, SearchRange.Height);
				if (AbsoluteSearchRange.Intersects(Player.GetAbsoluteHitbox()))
				{
					Status = Statuses.Attack;
				}
				// 一定時間おきに振り向く
				if (FrameCounter % 90 == 0)
				{
					IsFaceToLeft = !IsFaceToLeft;
				}
			}
			// 突撃
			else if (Status == Statuses.Attack)
			{
				float Speed = 0.8f;
				// 左右移動
				if (Math.Abs(Player.Position.X - Position.X) > 8)
				{
					if (Player.Position.X > Position.X)
					{
						MoveDistance.X = Speed;
						IsFaceToLeft = false;
					}
					else
					{
						MoveDistance.X = -Speed;
						IsFaceToLeft = true;
					}
				}
				// アニメーションを管理
				if (FrameCounter % 10 == 0)
				{
					AnimationPattern++;
					AnimationPattern = AnimationPattern % AttackAnimationTable.Length;
				}
			}

			// プレイヤーにダメージを与える
			Rectangle AbsoluteHitbox = GetAbsoluteHitbox();                 // 本体のヒットボックス
			Rectangle AbsoluteAttackHitbox = GetAbsoluteAttackHitbox();     // 槍部分のヒットボックス
			if (AbsoluteHitbox.Intersects(Player.GetAbsoluteHitbox()) ||
				AbsoluteAttackHitbox.Intersects(Player.GetAbsoluteHitbox()) && Status == Statuses.Attack && AttackAnimationTable[AnimationPattern] != 1)
			{
				Player.GiveDamage(DamageDetail);
			}
			// 落下時にデスポーン
			if (Position.Y > Map.Size.Height * Const.MapchipTileSize)
			{
				Destroy();
			}

			// 当たり判定を更新
			HitboxManagement();

			// ベースを更新
			base.Update(GameTime);

			FrameCounter++;
		}

		/// <summary>
		/// 描画
		/// </summary>
		public override void Draw(GameTime GameTime, SpriteBatch SpriteBatch)
		{
			// 被ダメージ点滅中は点滅させる
			if (!IsDamageStopDrawing)
			{
				// 描画するスプライト
				Sprite CurrentlySprite = new Sprite();

				// 捜索中
				if (Status == Statuses.Searching)
				{
					CurrentlySprite = SearchingSprite;
				}
				else if (Status == Statuses.Attack)
				{
					CurrentlySprite = AttackSprites[AttackAnimationTable[AnimationPattern]];
				}

				// 描画
				Vector2 Position = GetDrawPosition().ToVector2();
				Rectangle SourceRectangle = CurrentlySprite.SourceRectangle;
				Vector2 Origin = CurrentlySprite.Origin;
				SpriteEffects SpriteEffect = IsFaceToLeft ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
				float LayerDepth = Const.DrawOrder.Enemy.ToLayerDepth();
				// 左を向いている場合は中心座標を反転
				if (IsFaceToLeft)
				{
					Origin = new Vector2(SourceRectangle.Width - 1 - Origin.X, Origin.Y);
				}
				SpriteBatch.Draw(Texture, Position, SourceRectangle, Color.White, 0.0f, Origin, 1.0f, SpriteEffect, LayerDepth);

			}

			// デバッグ描画
			if (Global.Debug && Status == Statuses.Searching)
			{
				Point DrawPosition = GetDrawPosition();
				Rectangle AbsoluteSearchRange = new Rectangle(DrawPosition.X + SearchRange.X, DrawPosition.Y + SearchRange.Y, SearchRange.Width, SearchRange.Height);
				SpriteBatch.DrawRectangle(AbsoluteSearchRange, Color.Red * 0.15f, true);
			}
			else if (Global.Debug && Status == Statuses.Attack && AttackAnimationTable[AnimationPattern] != 1)
			{
				Rectangle AbsoluteAttackHitbox = GetAbsoluteAttackHitbox();
				SpriteBatch.DrawRectangle(AbsoluteAttackHitbox, Color.Red);
			}

			base.Draw(GameTime, SpriteBatch);
		}

		/// <summary>
		/// エンティティにダメージを与える
		/// </summary>
		/// <param name="Damage">与えるダメージ</param>
		public override bool GiveDamage(DamageDetail DamageDetail)
		{
			Status = Statuses.Attack;
			base.GiveDamage(DamageDetail);
			// 体力が 0 以下になった場合
			if (Health <= 0)
			{
				Point EffectPosition = new Point((int)Position.X, (int)Position.Y - 12);
				AddReserv(Entity.Names.Explosion1, EffectPosition);
			}
			return true;
		}

		#region プライベート関数

		/// <summary>
		/// 当たり判定ボックスの管理
		/// </summary>
		private void HitboxManagement()
		{
			Rectangle NewHitbox = Hitbox;
			// 左を向いている場合はヒットボックスを左右反転
			if (IsFaceToLeft)
			{
				NewHitbox = new Rectangle(1 - (NewHitbox.X + NewHitbox.Width), NewHitbox.Y, NewHitbox.Width, NewHitbox.Height);
			}
			RelativeHitbox = NewHitbox;
		}

		/// <summary>
		/// 絶対座標で槍部分の攻撃判定ボックスを取得
		/// </summary>
		private Rectangle GetAbsoluteAttackHitbox()
		{
			Rectangle RelativeAttackHitbox = AttackHitbox;
			// 左を向いている場合は攻撃ヒットボックスを左右反転
			if (IsFaceToLeft)
			{
				RelativeAttackHitbox = new Rectangle(1 - (RelativeAttackHitbox.X + RelativeAttackHitbox.Width), RelativeAttackHitbox.Y, RelativeAttackHitbox.Width, RelativeAttackHitbox.Height);
			}

			Point DrawPosition = GetDrawPosition();
			Rectangle AbsoluteAttackHitbox = new Rectangle(DrawPosition.X + RelativeAttackHitbox.X, DrawPosition.Y + RelativeAttackHitbox.Y, RelativeAttackHitbox.Width, RelativeAttackHitbox.Height);
			return AbsoluteAttackHitbox;
		}

		#endregion
	}
}
