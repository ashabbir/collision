using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CollisionDetection
{
    class Octree<T>
    {
        struct OctreeNode<T>
        {
            ///// <summary>
            ///// Center point of octree node
            ///// </summary>
            //public Vector3 Center { get; set; }

            ///// <summary>
            /////  Half the width of the node volume
            ///// </summary>
            //public float HalfWidth { get; set; }
            /// <summary>
            /// Children nodes
            /// </summary>
            OctreeNode<T>[] Children { get; set; }
            /// <summary>
            /// Linked list of objects containes in this node
            /// </summary>
            LinkedList<T> Objects { get; set; }
        }
    }
}
