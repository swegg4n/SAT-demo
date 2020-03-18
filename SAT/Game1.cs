using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace SAT
{
    public class Game1 : Game
    {
        public static bool DEBUG = false;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public static Texture2D square;
        public static SpriteFont font;

        public static Point WindowSize { get { return new Point(1200, 800); } }

        List<Rectangle> rectangles = new List<Rectangle>();
        SAT sat;


        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            graphics.PreferredBackBufferWidth = WindowSize.X;
            graphics.PreferredBackBufferHeight = WindowSize.Y;
            graphics.ApplyChanges();
        }


        protected override void Initialize()
        {
            base.Initialize();
        }


        protected override void LoadContent()
        {
            square = Content.Load<Texture2D>("Square");
            font = Content.Load<SpriteFont>("font");

            spriteBatch = new SpriteBatch(GraphicsDevice);

            rectangles.Add(new Rectangle(625, 425, MathHelper.Pi / 6, 200f, 100f));
            rectangles.Add(new Rectangle(550, 330, 0, 75f, 75f));

            sat = new SAT();
        }


        protected override void Update(GameTime gameTime)
        {
            KeyMouseReader.Update();

            if (KeyMouseReader.IsPressed(Keys.D1))
            {
                DEBUG = !DEBUG;
            }

            foreach (Rectangle rect in rectangles)
            {
                rect.Update();
            }

            sat.Update(rectangles[0], rectangles[1]);

            base.Update(gameTime);
        }


        Vector2 textPos = new Vector2(25, 25);
        Color[] colors = { Color.Green, Color.Blue };
        protected override void Draw(GameTime gameTime)
        {
            if (sat.colliding)
            {
                GraphicsDevice.Clear(new Color(1.0f, 0.8f, 0.8f));
            }
            else
            {
                GraphicsDevice.Clear(Color.White);
            }

            spriteBatch.Begin();
            {
                spriteBatch.DrawString(font, "LMB - Move Rectangles", textPos, Color.Black);
                spriteBatch.DrawString(font, "RMB - Rotate Rectangles", new Vector2(textPos.X, textPos.Y + 25), Color.Black);
                spriteBatch.DrawString(font, "1 - Toggle Debug", new Vector2(textPos.X, textPos.Y + 50), Color.Black);


                for (int i = 0; i < rectangles.Count; i++)
                {
                    rectangles[i].Draw(spriteBatch,colors[i]);
                }

                sat.Draw(spriteBatch);
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
