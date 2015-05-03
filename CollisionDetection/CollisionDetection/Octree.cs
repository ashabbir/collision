using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CollisionDetection
{
    class Octree
    {
        const int Depth = 3, NumberOfChildrenNodes = 8;
        OctreeNode<SpaceShip>[] _nodes;
        CollisionDetection _cd;
        readonly float _outerBoundarySize;

        // Statically Create
        public Octree(CollisionDetection cd, float OuterBoundarySize, SpaceShip[] ships)
        {
            _nodes = new OctreeNode<SpaceShip>[(int)Math.Pow(NumberOfChildrenNodes, Depth)];
            Vector3 offset = Vector3.Zero;
            float step = OuterBoundarySize * 0.5f;
            _nodes[0] = new OctreeNode<SpaceShip>(cd, step, offset);
            for (int i = 1; i < _nodes.Length; i+= 1)
            {
                offset.X = ((i & 1) != 0 ? step : -step);
                offset.Y = ((i & 2) != 0 ? step : -step);
                offset.Z = ((i & 4) != 0 ? step : -step);
                //_nodes[i] = new OctreeNode<SpaceShip>(cd, step, offset);
                if (i % NumberOfChildrenNodes == 0)
                    step *= 0.5f;
            }
        }

        public void Draw(Camera camera)
        {
            foreach (var node in _nodes)
                if (node != null)
                    node.Draw(camera);
        }

        // Dinamically Create
        public Octree(CollisionDetection cd, float outerBoundrySize)
        {
            _cd = cd;
            _outerBoundarySize = outerBoundrySize;
            _nodes = new OctreeNode<SpaceShip>[(int)Math.Pow(NumberOfChildrenNodes, Depth)];
        }

        // Dynamically create
        private void AddHelper(float OuterBoundarySize,
            int childOctant, SpaceShip ship, Vector3 center)
        {
            //int childOctant = 0;
            // If straddling any of the dividing x, y, or z planes, exit directly

            var delta = ship.CollisionSphere.Center - _nodes[childOctant].Center;
            float radiiSum = _nodes[childOctant].HalfSize + ship.CollisionSphere.Radius;
            bool straddling = Math.Abs(delta.X) > radiiSum
                ||  Math.Abs(delta.Y) > radiiSum
                || Math.Abs(delta.Z) > radiiSum;

            if (delta.X > 0.0f) childOctant += 1;
            if (delta.Y > 0.0f) childOctant += 2;
            if (delta.Z > 0.0f) childOctant += 4;

            // Fully contained in existing child node; insert in that subtree
            if (!straddling && childOctant < _nodes.Length)
            {
                

                float innerBoundry = OuterBoundarySize * 0.5f;
                Vector3 childCenterOffset =
                    new Vector3(((childOctant & 1) != 0 ? innerBoundry : -innerBoundry)
                    , ((childOctant & 2) != 0 ? innerBoundry : -innerBoundry)
                    , ((childOctant & 4) != 0 ? innerBoundry : -innerBoundry));

                if (_nodes[childOctant] == null)
                    _nodes[childOctant] = new OctreeNode<SpaceShip>(_cd, OuterBoundarySize * 0.5f, center + childCenterOffset);

                AddHelper(innerBoundry, childOctant, ship, _nodes[childOctant].Center + childCenterOffset);
            }
            else
            {
                // Straddling, or no child node to descend into, so
                // link object into linked list at this node
                if (_nodes[childOctant].Objects == null)
                    _nodes[childOctant].Objects = new LinkedList<SpaceShip>();
                _nodes[childOctant].Objects.AddLast(ship);
            }
        }

        public void Add(SpaceShip ship)
        {
            _nodes[0] = new OctreeNode<SpaceShip>(_cd, _outerBoundarySize * 0.5f, Vector3.Zero);
            AddHelper(_outerBoundarySize, 0, ship, Vector3.Zero);
        }

        public void Add(SpaceShip[] _spaceShips)
        {
            if (_nodes[0] == null)
                _nodes[0] = new OctreeNode<SpaceShip>(_cd, _outerBoundarySize * 0.5f, Vector3.Zero);
        }

        class OctreeNode<T>
        {
            BoundingCube _boundinghCube;
            //SpaceShip _spaceShip;

            internal OctreeNode(CollisionDetection cd, float halfSize, Vector3 center)
            {
                Center = center;
                _boundinghCube = new BoundingCube(cd, halfSize * 2, center);
                HalfSize = halfSize;
                //_spaceShip = new SpaceShip(cd, center);
            }

            int Key { get; set; }
            byte ChildrenBitmask { get; set; }

            internal Vector3 Center { get; set; }
            internal LinkedList<T> Objects { get; set; }

            internal float HalfSize { get; set; }

            internal void Draw(Camera camera)
            {                
                _boundinghCube.Draw(camera);
                //_spaceShip.Draw(camera);
            }
        }


    }
}
