﻿using Microsoft.Xna.Framework;
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

        Vector2[,,] projVerticesVisual;
        Tuple<Vector2, Vector2>[,] minMaxProjVerticesVisual;

        Rectangle[] visualProjections;

        public bool colliding;

        public SAT()
        {
            axis1Origin = new Point(Game1.WindowSize.X - 150, Game1.WindowSize.Y / 2);
            axis2Origin = new Point(Game1.WindowSize.X - 150, Game1.WindowSize.Y / 2);

            rectangles = new Rectangle[2];
            vectors = new Vector2[4];
            axes = new Rectangle[4];

            projVerticesVisual = new Vector2[2, 4, 4];
            minMaxProjVerticesVisual = new Tuple<Vector2, Vector2>[2, 4];

            visualProjections = new Rectangle[8];
        }

        public void Update(Rectangle rect1, Rectangle rect2)
        {
            UpdateProjection(rect1, rect2);
        }


        private void UpdateProjection(Rectangle rect1, Rectangle rect2)
        {
            rectangles[0] = rect1;
            rectangles[1] = rect2;

            vectors[0] = Vector2.Normalize(rect1.vertices[0].ToVector2() - rect1.vertices[1].ToVector2());
            vectors[1] = Vector2.Normalize(rect1.vertices[0].ToVector2() - rect1.vertices[2].ToVector2());
            vectors[2] = Vector2.Normalize(rect2.vertices[0].ToVector2() - rect2.vertices[1].ToVector2());
            vectors[3] = Vector2.Normalize(rect2.vertices[0].ToVector2() - rect2.vertices[2].ToVector2());

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
                        projVerticesVisual[i, j, k] = ProjectVisual(rectangles[i].vertices[k], vectors[j], axisOrigin);
                        minMaxProjVerticesVisual[i, j] = new Tuple<Vector2, Vector2>(projVerticesVisual[i, j, k], projVerticesVisual[i, j, k]);
                    }
                }
            }
            GetMinMaxVisual();
            colliding = IsColliding();
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
                        if (projVerticesVisual[i, j, k].X < minMaxProjVerticesVisual[i, j].Item1.X)
                        {
                            minMaxProjVerticesVisual[i, j] = new Tuple<Vector2, Vector2>(projVerticesVisual[i, j, k], minMaxProjVerticesVisual[i, j].Item2);
                        }
                        if (projVerticesVisual[i, j, k].X > minMaxProjVerticesVisual[i, j].Item2.X)
                        {
                            minMaxProjVerticesVisual[i, j] = new Tuple<Vector2, Vector2>(minMaxProjVerticesVisual[i, j].Item1, projVerticesVisual[i, j, k]);
                        }
                    }
                }
            }

            int index = 0;
            foreach (var minMaxProj in minMaxProjVerticesVisual)
            {
                Vector2 pos = (minMaxProj.Item1 + minMaxProj.Item2) / 2;
                float width = Vector2.Distance(minMaxProj.Item1, minMaxProj.Item2);

                visualProjections[index] = new Rectangle(pos.ToPoint().X, pos.ToPoint().Y, -axes[index % 4].rotation + Math.PI / 2, 10, width);
                index++;
            }
        }

        private bool IsColliding()
        {
            for (int axis = 0; axis < vectors.Length; axis++)
            {
                float sigmaDistance = Vector2.Distance(minMaxProjVerticesVisual[0, axis].Item1, minMaxProjVerticesVisual[0, axis].Item2) +
                    Vector2.Distance(minMaxProjVerticesVisual[1, axis].Item1, minMaxProjVerticesVisual[1, axis].Item2);

                if (Vector2.Distance(minMaxProjVerticesVisual[0, axis].Item1, minMaxProjVerticesVisual[1, axis].Item2) > sigmaDistance ||
                    Vector2.Distance(minMaxProjVerticesVisual[1, axis].Item1, minMaxProjVerticesVisual[0, axis].Item2) > sigmaDistance)
                {
                    return false;
                }
            }

            return true;
        }


        Color[] colors = { Color.Green, Color.Blue };
        public void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < axes.Length; i++)
            {
                axes[i].Draw(spriteBatch, colors[i / 2]);
            }

            if (Game1.DEBUG)
            {
                foreach (var minMaxProj in minMaxProjVerticesVisual)
                {
                    spriteBatch.Draw(Game1.square, minMaxProj.Item1, null, Color.LightGray, 0.0f, Game1.square.Bounds.Center.ToVector2(),
                           new Vector2(0.75f, 0.75f), SpriteEffects.None, 1.0f);
                    spriteBatch.Draw(Game1.square, minMaxProj.Item2, null, Color.Black, 0.0f, Game1.square.Bounds.Center.ToVector2(),
                            new Vector2(0.75f, 0.75f), SpriteEffects.None, 1.0f);
                }

                foreach (var pV in projVerticesVisual)
                {
                    spriteBatch.Draw(Game1.square, pV, null, Color.Gray, 0.0f, Game1.square.Bounds.Center.ToVector2(),
                            new Vector2(0.35f, 0.35f), SpriteEffects.None, 1.0f);
                }
            }
            else
            {
                foreach (var visualProj in visualProjections)
                {
                    visualProj.Draw(spriteBatch, new Color(0, 0, 0, 100));
                }
            }
        }
    }

}
