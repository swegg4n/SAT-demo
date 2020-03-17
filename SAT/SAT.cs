using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAT
{
    class SAT
    {
        private readonly Point axis1Origin;
        private readonly Point axis2Origin;
        private const int AXES_LENGTH = 5000;
        private const int AXES_WIDTH = 2;

        private readonly Rectangle[] rectangles;
        private Vector2[] vectors;

        private Rectangle[] axes;

        Vector2[,,] projVertices;
        Vector2[,,] projVerticesVisual;
        Tuple<Vector2, Vector2>[,] minMaxProjVertices;
        Tuple<Vector2, Vector2>[,] minMaxProjVerticesVisual;

        public bool colliding;

        public SAT()
        {
            axis1Origin = new Point(Game1.WindowSize.X - 150, Game1.WindowSize.Y / 2);
            axis2Origin = new Point(Game1.WindowSize.X - 150, Game1.WindowSize.Y / 2);

            rectangles = new Rectangle[2];
            vectors = new Vector2[4];
            axes = new Rectangle[4];

            projVertices = new Vector2[2, 4, 4];
            projVerticesVisual = new Vector2[2, 4, 4];
            minMaxProjVertices = new Tuple<Vector2, Vector2>[2, 4];
            minMaxProjVerticesVisual = new Tuple<Vector2, Vector2>[2, 4];
        }

        public void Update(Rectangle rect1, Rectangle rect2)
        {
            UpdateProjection(rect1, rect2);
        }

        private void UpdateProjection(Rectangle rect1, Rectangle rect2)
        {
            rectangles[0] = rect1;
            rectangles[1] = rect2;

            vectors[0] = rect1.vertices[0].ToVector2() - rect1.vertices[1].ToVector2();
            vectors[1] = rect1.vertices[0].ToVector2() - rect1.vertices[2].ToVector2();
            vectors[2] = rect2.vertices[0].ToVector2() - rect2.vertices[1].ToVector2();
            vectors[3] = rect2.vertices[0].ToVector2() - rect2.vertices[2].ToVector2();

            axes[0] = new Rectangle(axis1Origin.X, axis1Origin.Y, -rect1.rotation, AXES_WIDTH, AXES_LENGTH);
            axes[1] = new Rectangle(axis1Origin.X, axis1Origin.Y, -rect1.rotation + Math.PI / 2, AXES_WIDTH, AXES_LENGTH);
            axes[2] = new Rectangle(axis2Origin.X, axis2Origin.Y, -rect2.rotation, AXES_WIDTH, AXES_LENGTH);
            axes[3] = new Rectangle(axis2Origin.X, axis2Origin.Y, -rect2.rotation + Math.PI / 2, AXES_WIDTH, AXES_LENGTH);

            Vector2 axisOrigin;

            for (int i = 0; i < rectangles.Length; i++)
            {
                for (int j = 0; j < vectors.Length; j++)
                {
                    if (j <= 1)
                        axisOrigin = axis1Origin.ToVector2();
                    else
                        axisOrigin = axis2Origin.ToVector2();

                    for (int k = 0; k < rectangles[i].vertices.Length; k++)
                    {
                        projVertices[i, j, k] = Project(rectangles[i].vertices[k], vectors[j]);
                        projVerticesVisual[i, j, k] = ProjectVisual(rectangles[i].vertices[k], vectors[j], axisOrigin);
                        minMaxProjVertices[i, j] = new Tuple<Vector2, Vector2>(projVertices[i, j, k], projVertices[i, j, k]);
                        minMaxProjVerticesVisual[i, j] = new Tuple<Vector2, Vector2>(projVerticesVisual[i, j, k], projVerticesVisual[i, j, k]);
                    }
                }
            }
            GetMinMax();
            GetMinMaxVisual();
            colliding = IsColliding();
        }

        private Vector2 Project(Point P, Vector2 v)
        {
            Vector2 A = v;
            Vector2 AP = P.ToVector2() - A;
            Vector2 u_bis = (AP.X * v.X + AP.Y * v.Y) / (float)(Math.Pow(v.X, 2) + Math.Pow(v.Y, 2)) * v;
            Vector2 u_prim = AP - u_bis;
            Vector2 Q = new Vector2(P.X - u_prim.X, P.Y - u_prim.Y);
            return new Vector2(Math.Abs(Q.X), Math.Abs(Q.Y));
        }

        private Vector2 ProjectVisual(Point P, Vector2 v, Vector2 axisOrigin)
        {
            Vector2 A = axisOrigin;
            Vector2 AP = P.ToVector2() - A;
            Vector2 u_bis = (AP.X * v.X + AP.Y * v.Y) / (float)(Math.Pow(v.X, 2) + Math.Pow(v.Y, 2)) * v;
            Vector2 u_prim = AP - u_bis;
            Vector2 Q = new Vector2(P.X - u_prim.X, P.Y - u_prim.Y);
            return Q;
        }

        private void GetMinMax()
        {
            for (int i = 0; i < rectangles.Length; i++)
            {
                for (int j = 0; j < vectors.Length; j++)
                {
                    for (int k = 0; k < rectangles[i].vertices.Length; k++)
                    {
                        float testDistance = (float)Math.Sqrt(Math.Pow(projVertices[i, j, k].X, 2) + Math.Pow(projVertices[i, j, k].Y, 2));

                        float currentMinDistance = (float)Math.Sqrt(Math.Pow(minMaxProjVertices[i, j].Item1.X, 2) + Math.Pow(minMaxProjVertices[i, j].Item1.Y, 2));
                        float currentMaxDistance = (float)Math.Sqrt(Math.Pow(minMaxProjVertices[i, j].Item2.X, 2) + Math.Pow(minMaxProjVertices[i, j].Item2.Y, 2));

                        if (testDistance < currentMinDistance)
                        {
                            minMaxProjVertices[i, j] = new Tuple<Vector2, Vector2>(projVertices[i, j, k], minMaxProjVertices[i, j].Item2);
                        }
                        if (testDistance > currentMaxDistance)
                        {
                            minMaxProjVertices[i, j] = new Tuple<Vector2, Vector2>(minMaxProjVertices[i, j].Item1, projVertices[i, j, k]);
                        }
                    }
                }
            }
        }

        private void GetMinMaxVisual()
        {
            Vector2 axisOrigin;

            for (int i = 0; i < rectangles.Length; i++)
            {
                for (int j = 0; j < vectors.Length; j++)
                {
                    if (j <= 1)
                        axisOrigin = axis1Origin.ToVector2();
                    else
                        axisOrigin = axis2Origin.ToVector2();

                    for (int k = 0; k < rectangles[i].vertices.Length; k++)
                    {
                        Vector2 projVector = projVerticesVisual[i, j, k] - axisOrigin;

                        float testDistance = (float)Math.Sqrt(Math.Pow(projVector.X, 2) + Math.Pow(projVector.Y, 2));

                        Vector2 currentMinVector = minMaxProjVerticesVisual[i, j].Item1 - axisOrigin;
                        Vector2 currentMaxVector = minMaxProjVerticesVisual[i, j].Item2 - axisOrigin;
                        float currentMinDistance = (float)Math.Sqrt(Math.Pow(currentMinVector.X, 2) + Math.Pow(currentMinVector.Y, 2));
                        float currentMaxDistance = (float)Math.Sqrt(Math.Pow(currentMaxVector.X, 2) + Math.Pow(currentMaxVector.Y, 2));

                        if (testDistance < currentMinDistance)
                        {
                            minMaxProjVerticesVisual[i, j] = new Tuple<Vector2, Vector2>(projVerticesVisual[i, j, k], minMaxProjVerticesVisual[i, j].Item2);
                        }
                        if (testDistance > currentMaxDistance)
                        {
                            minMaxProjVerticesVisual[i, j] = new Tuple<Vector2, Vector2>(minMaxProjVerticesVisual[i, j].Item1, projVerticesVisual[i, j, k]);
                        }
                    }
                }
            }
        }

        private bool IsColliding()
        {
            for (int axis = 0; axis < vectors.Length; axis++)
            {
                if (minMaxProjVertices[1, axis].Item1.X > minMaxProjVertices[0, axis].Item2.X || minMaxProjVertices[1, axis].Item1.Y > minMaxProjVertices[0, axis].Item2.Y ||
                    minMaxProjVertices[0, axis].Item1.X > minMaxProjVertices[1, axis].Item2.X || minMaxProjVertices[0, axis].Item1.Y > minMaxProjVertices[1, axis].Item2.Y)
                {
                    return false;
                }
            }

            return true;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (Game1.DEBUG)
            {
                for (int i = 0; i < axes.Length; i++)
                {
                    axes[i].Draw(spriteBatch);
                }

                foreach (var pV in projVerticesVisual)
                {
                    spriteBatch.Draw(Game1.square, pV, null, Color.Green, 0.0f, Game1.square.Bounds.Center.ToVector2(),
                            new Vector2(0.25f, 0.25f), SpriteEffects.None, 1.0f);
                }

                foreach (var mmV in minMaxProjVerticesVisual)
                {
                    spriteBatch.Draw(Game1.square, mmV.Item1, null, Color.LightBlue, 0.0f, Game1.square.Bounds.Center.ToVector2(),
                           new Vector2(0.5f, 0.5f), SpriteEffects.None, 1.0f);
                    spriteBatch.Draw(Game1.square, mmV.Item2, null, Color.DarkBlue, 0.0f, Game1.square.Bounds.Center.ToVector2(),
                            new Vector2(0.5f, 0.5f), SpriteEffects.None, 1.0f);
                }
            }
        }
    }

}
