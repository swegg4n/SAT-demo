using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace SAT
{
    class Rectangle
    {
        internal enum Vertex { TopLeft, TopRight, BottomLeft, BottomRight };

        private const float VERTEX_SCALE = 0.25f;

        public readonly Point[] vertices;

        private Vector2 position;
        public double rotation;
        private readonly double scaleX, scaleY;

        int top, bottom, left, right;

        bool canMove;
        Vector2 delta;


        public Rectangle(int posX, int posY, double rotation, double width, double height)
        {
            this.position.X = posX;
            this.position.Y = posY;
            this.rotation = -rotation;
            this.scaleX = width / 16;
            this.scaleY = height / 16;

            vertices = new Point[4];

            UpdateVertices();
            UpdateBounds();
        }

        private void UpdateVertices()
        {
            vertices[0] = new Point((int)(position.X - scaleX * 16 / 2), (int)(position.Y - scaleY * 16 / 2));
            vertices[1] = new Point((int)(position.X + scaleX * 16 / 2), (int)(position.Y - scaleY * 16 / 2));
            vertices[2] = new Point((int)(position.X - scaleX * 16 / 2), (int)(position.Y + scaleY * 16 / 2));
            vertices[3] = new Point((int)(position.X + scaleX * 16 / 2), (int)(position.Y + scaleY * 16 / 2));

            for (int i = 0; i < vertices.Length; i++)
            {
                double tempX = vertices[i].X - position.X;
                double tempY = vertices[i].Y - position.Y;
                double rotatedX = tempX * Math.Cos(this.rotation) - tempY * Math.Sin(this.rotation);
                double rotatedY = tempX * Math.Sin(this.rotation) + tempY * Math.Cos(this.rotation);
                vertices[i].X = (int)(rotatedX + position.X);
                vertices[i].Y = (int)(rotatedY + position.Y);
            }
        }


        public void Update()
        {
            if (KeyMouseReader.LMB_Click() && Contains(KeyMouseReader.mousePos))
            {
                delta = KeyMouseReader.mousePos.ToVector2() - position;
                canMove = true;
            }
            if (KeyMouseReader.LMB_Hold() && canMove)
            {
                position = KeyMouseReader.mousePos.ToVector2() - delta;
                UpdateVertices();
                UpdateBounds();
            }
            else
            {
                canMove = false;
            }

            if (KeyMouseReader.RMB_Hold() && Contains(KeyMouseReader.mousePos))
            {
                rotation += 0.01;
                UpdateVertices();
                UpdateBounds();
            }
        }

        private void UpdateBounds()
        {
            top = int.MaxValue;
            bottom = int.MinValue;
            left = int.MaxValue;
            right = int.MinValue;

            foreach (var vertex in vertices)
            {
                if (vertex.Y <= top)
                    top = vertex.Y;

                if (vertex.Y >= bottom)
                    bottom = vertex.Y;

                if (vertex.X <= left)
                    left = vertex.X;

                if (vertex.X >= right)
                    right = vertex.X;
            }
        }


        private bool Contains(Point point)
        {
            if (point.X < left || point.X > right || point.Y < top || point.Y > bottom)
                return false;

            return true;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Game1.square, position, null, Color.Gray, (float)rotation, Game1.square.Bounds.Center.ToVector2(),
                new Vector2((float)scaleX, (float)scaleY), SpriteEffects.None, 1.0f);

            if (Game1.DEBUG)
            {
                for (int i = 0; i < vertices.Length; i++)
                {
                    spriteBatch.Draw(Game1.square, vertices[i].ToVector2(), null, Color.Red, 0.0f, Game1.square.Bounds.Center.ToVector2(),
                        new Vector2(VERTEX_SCALE, VERTEX_SCALE), SpriteEffects.None, 1.0f);
                }
            }
        }
    }
}
