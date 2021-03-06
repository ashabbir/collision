﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CollisionDetection
{
    /// <summary>
    /// hull just to hold vertices and has get guthest point that will be used by gjk algo
    /// </summary>
        public class Hull
        {
            public List<Vector3> Verticecs { get; set; }
            public Vector3 Center { get; set; }
            public float Scale { get; set; }
            public Matrix ScaleMatrix { get; set; }
            public int IndexNo { get; set; }
            public Rotation Rot { get; set; }

            public Hull(List<Vector3> vertices, float scale, int indexno, Rotation rot)
            {
                this.Center = Vector3.Zero;
                this.Verticecs = vertices;
                this.Scale = scale;
                this.IndexNo = indexno;
                this.Rot = rot;
                ScaleMatrix = Matrix.CreateScale(scale);
            }


            //get furthest point with dot product in a direction
            public Vector3 GetFurthestPoint(Vector3 direction)
            {
                Matrix world = Rot.RotationMatrix * Matrix.CreateTranslation(Center) * ScaleMatrix;
                float max = float.NegativeInfinity;
                Vector3 vec = Vector3.Transform(Verticecs.First(), world);
                if (direction != Vector3.Zero)
                {
                    direction.Normalize();
                }

                foreach (var v in Verticecs)
                {

                    Vector3 temp = Vector3.Transform(v, world);
                    float dot = Vector3.Dot(temp, direction);
                    if (dot > max)
                    {
                        max = dot;
                        vec = temp;
                    }
                }
                return vec;
                //return vec * Scale;
            }


            //get furthest point with dot product in a direction
            // trying to optimize get F p 
            // instead of transformation on world do it on dir then take the max n transform that
            public Vector3 GetFurthestPoint_2(Vector3 direction)
            {
                float max = float.NegativeInfinity;
                Vector3 vec = Verticecs.First();
                Matrix world = Rot.RotationMatrix * Matrix.CreateTranslation(Center) * ScaleMatrix;
                Vector3 dir = Vector3.TransformNormal(direction, world);
                 
                foreach (var v in Verticecs)
                {
                    float dot = Vector3.Dot(v, dir);
                    if (dot > max)
                    {
                        max = dot;
                        vec = v;
                    }
                }
                return Vector3.Transform(vec, world);
                //return vec * Scale;
            }
        }
    }
