using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Rockman_vs_SmashBros
{
    /// <summary>
    /// ゲームクラス
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D texture;
        Vector2 PlayerPos;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// 初期化
        /// </summary>
        protected override void Initialize()
        {
            // ここに初期化ロジックを追加

            // MonoGame コンポーネントを初期化
            base.Initialize();
        }

        /// <summary>
        /// リソースの確保
        /// </summary>
        protected override void LoadContent()
        {
            // SpriteBatch を新規作成
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // ここに画像の読み込み処理を追加
            texture = Content.Load<Texture2D>("Player.png");
        }

        /// <summary>
        /// リソースの開放
        /// </summary>
        protected override void UnloadContent()
        {
            // ここに確保したリソースを開放する処理を追加
            texture.Dispose();
        }

        /// <summary>
        /// フレームの更新
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            // ここに計算処理を追加

            if (Keyboard.GetState().IsKeyDown(Keys.W))
            {
                PlayerPos.Y -= 3;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                PlayerPos.X -= 3;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                PlayerPos.Y += 3;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                PlayerPos.X += 3;
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// 描画
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin(
                SpriteSortMode.BackToFront,
                null,
                SamplerState.PointClamp
                );

            // ここに描画処理を追加
            spriteBatch.Draw(texture, PlayerPos, new Rectangle(32*1, 32*1, 32, 32), Color.White, 0.0f, new Vector2(0,0), 2.0f, SpriteEffects.None, 1);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
