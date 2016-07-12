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
	/// Controller クラス
	/// </summary>
	public static class Controller
	{
		#region メンバーの宣言

		private struct ButtonState                                    // 各ボタンの入力状態 構造体
		{
			public bool Up;                                           // 十字キー 上
			public bool Down;                                         // 十字キー 下
			public bool Left;                                         // 十字キー 左
			public bool Right;                                        // 十字キー 右
			public bool A;                                            // A ボタン
			public bool B;                                            // B ボタン
			public bool Start;                                        // スタートボタン
			public bool Select;                                       // セレクトボタン
		}

		public enum Buttons
		{
			Up,                                                      // 十字キー 上
			Down,                                                    // 十字キー 下
			Left,                                                    // 十字キー 左
			Right,                                                   // 十字キー 右
			A,                                                       // A ボタン
			B,                                                       // B ボタン
			Start,                                                   // スタートボタン
			Select                                                   // セレクトボタン
		}

		private static ButtonState buttonState;                       // 現在のボタン入力状態
		private static ButtonState oldButtonState;                    // 1フレーム前のボタン入力状態
		private static KeyboardState keyboardState;                   // キーボード入力状態

		#endregion

		/// <summary>
		/// 初期化
		/// </summary>
		public static void Initialize() { }

		/// <summary>
		/// リソースの確保
		/// </summary>
		public static void LoadConten(ContentManager Content)
		{
		}

		/// <summary>
		/// リソースの破棄
		/// </summary>
		public static void UnloadContent()
		{
		}

		/// <summary>
		/// フレームの更新
		/// </summary>
		public static void Update(GameTime GameTime)
		{
			// 1フレーム前の入力状態
			oldButtonState = buttonState;

			// キーボードの入力を受付
			keyboardState = Keyboard.GetState();

			// 十字キー 上
			buttonState.Up = keyboardState.IsKeyDown(Keys.W);

			// 十字キー 下
			buttonState.Down = keyboardState.IsKeyDown(Keys.S);

			// 十字キー 左
			buttonState.Left = keyboardState.IsKeyDown(Keys.A);

			// 十字キー 右
			buttonState.Right = keyboardState.IsKeyDown(Keys.D);

			// A ボタン
			buttonState.A = keyboardState.IsKeyDown(Keys.Space);

			// B ボタン
			buttonState.B = keyboardState.IsKeyDown(Keys.E);

			// スタートボタン
			buttonState.Start = keyboardState.IsKeyDown(Keys.Enter);

			// セレクトボタン
			buttonState.Select = keyboardState.IsKeyDown(Keys.LeftShift);
		}

		/// <summary>
		/// 描画
		/// </summary>
		public static void Draw(GameTime GameTime, SpriteBatch SpriteBatch)
		{
			int BaseX = 8, BaseY = 216;
			SpriteBatch.DrawRectangle(new Rectangle(BaseX, BaseY, 32, 16), Color.Black, true);
			SpriteBatch.DrawRectangle(new Rectangle(BaseX + 1, BaseY + 1, 30, 14), new Color(160, 0, 0), true);
			SpriteBatch.DrawRectangle(new Rectangle(BaseX + 2, BaseY + 2, 13, 12), new Color(239, 175, 111), true);
			SpriteBatch.DrawRectangle(new Rectangle(BaseX + 15, BaseY + 5, 15, 9), new Color(239, 175, 111), true);
			SpriteBatch.DrawRectangle(new Rectangle(BaseX + 12, BaseY + 10, 9, 3), new Color(160, 0, 0), true);
			SpriteBatch.DrawRectangle(new Rectangle(BaseX + 6, BaseY + 3, 3, 9), Color.Black, true);
			SpriteBatch.DrawRectangle(new Rectangle(BaseX + 3, BaseY + 6, 9, 3), Color.Black, true);
			SpriteBatch.DrawRectangle(new Rectangle(BaseX + 13, BaseY + 11, 3, 1), Color.Black, true);
			SpriteBatch.DrawRectangle(new Rectangle(BaseX + 17, BaseY + 11, 3, 1), Color.Black, true);
			SpriteBatch.DrawRectangle(new Rectangle(BaseX + 22, BaseY + 9, 3, 3), Color.Black, true);
			SpriteBatch.DrawRectangle(new Rectangle(BaseX + 26, BaseY + 9, 3, 3), Color.Black, true);

			if (buttonState.Up)
			{
				SpriteBatch.DrawRectangle(new Rectangle(BaseX + 6, BaseY + 3, 3, 3), Color.AliceBlue, true);
			}
			if (buttonState.Down)
			{
				SpriteBatch.DrawRectangle(new Rectangle(BaseX + 6, BaseY + 9, 3, 3), Color.AliceBlue, true);
			}
			if (buttonState.Left)
			{
				SpriteBatch.DrawRectangle(new Rectangle(BaseX + 3, BaseY + 6, 3, 3), Color.AliceBlue, true);
			}
			if (buttonState.Right)
			{
				SpriteBatch.DrawRectangle(new Rectangle(BaseX + 9, BaseY + 6, 3, 3), Color.AliceBlue, true);
			}
			if (buttonState.A)
			{
				SpriteBatch.DrawRectangle(new Rectangle(BaseX + 26, BaseY + 9, 3, 3), Color.AliceBlue, true);
			}
			if (buttonState.B)
			{
				SpriteBatch.DrawRectangle(new Rectangle(BaseX + 22, BaseY + 9, 3, 3), Color.AliceBlue, true);
			}
			if (buttonState.Start)
			{
				SpriteBatch.DrawRectangle(new Rectangle(BaseX + 17, BaseY + 11, 3, 1), Color.AliceBlue, true);
			}
			if (buttonState.Select)
			{
				SpriteBatch.DrawRectangle(new Rectangle(BaseX + 13, BaseY + 11, 3, 1), Color.AliceBlue, true);
			}
		}

		/// <summary>
		/// 指定されたボタンが押されているかを調べる。
		/// </summary>
		/// <param name="Button">調べたいボタン</param>
		/// <returns>指定されたボタンが押されていれば true を、それ以外は false を返す。</returns>
		public static bool IsButtonDown(Buttons Button)
		{
			if (Button == Buttons.Up && buttonState.Up) { return true; }
			if (Button == Buttons.Down && buttonState.Down) { return true; }
			if (Button == Buttons.Left && buttonState.Left) { return true; }
			if (Button == Buttons.Right && buttonState.Right) { return true; }
			if (Button == Buttons.A && buttonState.A) { return true; }
			if (Button == Buttons.B && buttonState.B) { return true; }
			if (Button == Buttons.Start && buttonState.Start) { return true; }
			if (Button == Buttons.Select && buttonState.Select) { return true; }
			return false;
		}

		/// <summary>
		/// 指定されたボタンが離されているかを調べる。
		/// </summary>
		/// <param name="Button">調べたいボタン</param>
		/// <returns>指定されたボタンが離されていれば true を、それ以外は false を返す。</returns>
		public static bool IsButtonUp(Buttons Button)
		{
			if (Button == Buttons.Up && !buttonState.Up) { return true; }
			if (Button == Buttons.Down && !buttonState.Down) { return true; }
			if (Button == Buttons.Left && !buttonState.Left) { return true; }
			if (Button == Buttons.Right && !buttonState.Right) { return true; }
			if (Button == Buttons.A && !buttonState.A) { return true; }
			if (Button == Buttons.B && !buttonState.B) { return true; }
			if (Button == Buttons.Start && !buttonState.Start) { return true; }
			if (Button == Buttons.Select && !buttonState.Select) { return true; }
			return false;
		}

		/// <summary>
		/// 指定されたボタンが押された瞬間かを調べる。
		/// </summary>
		/// <param name="Button">調べたいボタン</param>
		/// <returns>指定されたボタンが押された瞬間であれば true を、それ以外は false を返す。</returns>
		public static bool IsButtonPressed(Buttons Button)
		{
			if (Button == Buttons.Up && buttonState.Up && !oldButtonState.Up) { return true; }
			if (Button == Buttons.Down && buttonState.Down && !oldButtonState.Down) { return true; }
			if (Button == Buttons.Left && buttonState.Left && !oldButtonState.Left) { return true; }
			if (Button == Buttons.Right && buttonState.Right && !oldButtonState.Right) { return true; }
			if (Button == Buttons.A && buttonState.A && !oldButtonState.A) { return true; }
			if (Button == Buttons.B && buttonState.B && !oldButtonState.B) { return true; }
			if (Button == Buttons.Start && buttonState.Start && !oldButtonState.Start) { return true; }
			if (Button == Buttons.Select && buttonState.Select && !oldButtonState.Select) { return true; }
			return false;
		}
	}
}
