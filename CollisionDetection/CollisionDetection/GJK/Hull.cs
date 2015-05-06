using Microsoft.Xna.Framework;
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
        public Vector3[] Verticecs { get; set; }
        public Vector3 Center { get { return _ship.Position; } }
        public int IndexNo { get; set; }
        SpaceShip _ship;

        public Hull(Vector3[] vertices, int index, SpaceShip ship)
        {
            this.Verticecs = vertices;
            this.IndexNo = index;
            _ship = ship;
        }


        //get furthest point with dot product in a direction
        public Vector3 GetFurthestPoint(Vector3 direction)
        {
            float max = float.NegativeInfinity;
            Vector3 vec = Verticecs.First();
            if (direction != Vector3.Zero)
                direction.Normalize();

            Matrix invertedWorld = Matrix.Invert(_ship.Transform);//Rot.RotationMatrix * Matrix.CreateTranslation(Center) * ScaleMatrix;
            Vector3 invertedDirection = Vector3.Transform(direction, invertedWorld);

            foreach (var v in Verticecs)
            {
                float dot = Vector3.Dot(v, invertedDirection);
                if (dot > max)
                {
                    max = dot;
                    vec = v;
                }
            }
            return vec;
        }


        //get furthest point with dot product in a direction
        // trying to optimize get F p 
        // instead of transformation on world do it on dir then take the max n transform that
        public Vector3 GetFurthestPoint_2(Vector3 direction)
        {
            Matrix world = Rot.RotationMatrix * Matrix.CreateTranslation(Center) * ScaleMatrix;
            float max = float.NegativeInfinity;
            Vector3 vec = Verticecs.First();
            Vector3 dir = Vector3.Transform(direction, world);
            foreach (var v in Verticecs)
            {

                
                float dot = Vector3.Dot(v, dir);
                if (dot > max)
                {
                    max = dot;
                    vec = v;
                }
            }
            return Vector3.Transform(vec , world);
            //return vec * Scale;
        }
    }
}
