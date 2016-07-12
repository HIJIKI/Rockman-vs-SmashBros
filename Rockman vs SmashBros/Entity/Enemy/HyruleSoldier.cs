﻿using Microsoft.Xna.Framework;
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

		private bool IsFaceToLeft;                                  // 左を向いているかどうか
		private int FrameCounter;                                   // フレームカウンター
		private Rectangle SearchRange;                              // プレイヤーを見つける相対範囲
		private int AnimationPattern;                               // アニメーションのパターン

		private static Sprite SearchingSprite;                      // プレイヤー捜索中のスプライト
		private static Sprite[] AttackSprites = new Sprite[3];      // 突撃中のスプライト
		private static int[] AttackAnimationTable;                  // 突撃中のアニメーションの順番

		private Statuses Status;                                    // プレイヤーの状態
		private enum Statuses                                       // 各状態
		{
			Searching,                                              // プレイヤーを捜索中
			Attack                                                  // 突撃
		}

		#endregion

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public HyruleSoldier(Point Position, bool IsFromMap = false, Point FromMapPosition = new Point())
		{
			this.Position = Position.ToVector2();
			this.IsFromMap = IsFromMap;
			this.FromMapPosition = FromMapPosition;
			Type = Types.Enemy;
            Health = 3;
			HitDamage = 4;
			Initialize();
		}

		/// <summary>
		/// 初期化
		/// </summary>
		public override void Initialize()
		{
			IsAlive = true;
			MoveDistance = Vector2.Zero;

			SearchingSprite = new Sprite(new Rectangle(0, 0, 32, 32), new Vector2(15, 28));
			AttackSprites[0] = new Sprite(new Rectangle(48 * 0, 32, 48, 32), new Vector2(15, 28));
			AttackSprites[1] = new Sprite(new Rectangle(48 * 1, 32, 48, 32), new Vector2(15, 28));
			AttackSprites[2] = new Sprite(new Rectangle(48 * 2, 32, 48, 32), new Vector2(15, 28));
			AttackAnimationTable = new int[] { 0, 1, 2, 1 };

			RelativeCollision = new Rectangle(-7, -23, 14, 24);
			SearchRange = new Rectangle(-80, -63, 160, 64);
		}

		/// <summary>
		/// リソースの確保
		/// </summary>
		public static void LoadContent(ContentManager Content)
		{
			Texture = Content.Load<Texture2D>("Image/HyruleSoldier.png");
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
				if (AbsoluteSearchRange.Intersects(Player.GetAbsoluteCollision()))
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
				if (FrameCounter % 16 == 0)
				{
					AnimationPattern++;
					AnimationPattern = AnimationPattern % AttackAnimationTable.Length;
				}
			}

			// プレイヤーにダメージを与える
			Rectangle AbsoluteCollision = GetAbsoluteCollision();
			if (AbsoluteCollision.Intersects(Player.GetAbsoluteCollision()))
			{
				Player.GiveDamage(HitDamage);
			}

			// 落下時にデスポーン
			if (Position.Y > Map.Size.Height * Const.MapchipTileSize)
			{
				Destroy();
			}

			FrameCounter++;
			base.Update(GameTime);
		}

		/// <summary>
		/// 描画
		/// </summary>
		public override void Draw(GameTime GameTime, SpriteBatch SpriteBatch)
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
			float layerDepth = (float)Const.DrawOrder.Enemy / (float)Const.DrawOrder.MAX;
			// 左を向いている場合は中心座標を反転
			if (IsFaceToLeft)
			{
				Origin = new Vector2((CurrentlySprite.SourceRectangle.Width) - Origin.X, Origin.Y);
			}
			SpriteBatch.Draw(Texture, Position, SourceRectangle, Color.White, 0.0f, Origin, 1.0f, SpriteEffect, layerDepth);

			// デバッグ描画
			if (Global.Debug && Status == Statuses.Searching)
			{
				Point DrawPosition = GetDrawPosition();
				Rectangle AbsoluteSearchRange = new Rectangle(DrawPosition.X + SearchRange.X, DrawPosition.Y + SearchRange.Y, SearchRange.Width, SearchRange.Height);
				SpriteBatch.DrawRectangle(AbsoluteSearchRange, Color.Red * 0.2f, true);
			}

			base.Draw(GameTime, SpriteBatch);
		}

        /// <summary>
        /// エンティティにダメージを与える
        /// </summary>
        /// <param name="Damage">与えるダメージ</param>
        public override void GiveDamage(int Damage)
		{
            Status = Statuses.Attack;

			base.GiveDamage(Damage);
		}
	}
}
