using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CollisionDetection
{
    // TODO: Don't use XNA implementation, use our own implementation
    // cheating for now so we can get the other parts of the code working
    class BoundingVolume
    {
        public Vector3 Center { get; set; }
        public float Radius { get; set; }

        // TODO: delete me
        public BoundingVolume(BoundingSphere bs)
        {
            Center = bs.Center;
            Radius = bs.Radius;
        }
    }
}
