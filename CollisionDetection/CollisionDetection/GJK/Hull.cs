using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CollisionDetection
{
    public class Hull
    {
        public List<Vector3> Verticecs { get; set; }
        public Vector3 Center { get; set; }
        public float Scale { get; set; }

        public Hull(List<Vector3> vertices , float scale)
        {
            this.Center = Vector3.Zero;
            this.Verticecs = vertices;
            this.Scale = scale;
        }


        //get furthest point with dot poroduct
        public Vector3 GetFurthestPoint(Vector3 direction)
        {
            float max = float.NegativeInfinity;
            Vector3 vec = Verticecs.First();
            if (direction != Vector3.Zero)
            {
                direction.Normalize();
            }

            foreach (var v in Verticecs)
            {
               
                float dot = Vector3.Dot(v, direction);
                if (dot > max)
                {
                    max = dot;
                    vec = v;
                }
            }
           
            return vec * Center * Scale;
        }

    }
}
