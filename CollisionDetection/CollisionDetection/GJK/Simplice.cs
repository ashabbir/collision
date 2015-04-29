using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;


namespace CollisionDetection
{
    public class Simplice
    {

        public List<Vector3> Vertices { get; set; }

        public Simplice(Vector3 v)
            : this(new List<Vector3>() { v })
        {

        }

        public Simplice( List<Vector3> vertices)
        {
            Vertices = new List<Vector3>();
            vertices.ForEach(v => Vertices.Add(v));
        }
    }
}
