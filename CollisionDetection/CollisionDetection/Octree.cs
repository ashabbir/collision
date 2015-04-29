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
            for (int i = 0; i < _nodes.Length; i++)
            {
                offset.X = ((i & 1) != 0 ? step : -step);
                offset.Y = ((i & 2) != 0 ? step : -step);
                offset.Z = ((i & 4) != 0 ? step : -step);
                _nodes[i] = new OctreeNode<SpaceShip>(cd, step, offset);
                if (i % NumberOfChildrenNodes == 0 && i != 0)
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
            int i, SpaceShip ship, Vector3 center)
        {
            int childOctant = 0;
            // If straddling any of the dividing x, y, or z planes, exit directly

            var delta = ship.CollisionSphere.Center - _nodes[i].Center;
            float deltaX = Math.Abs(delta.X), deltaY = Math.Abs(delta.Y)
                , deltaZ = Math.Abs(delta.Z), radiiSum = _nodes[i].HalfSize + ship.CollisionSphere.Radius;
            bool straddling = deltaX < radiiSum
                || deltaY < radiiSum
                || deltaZ < radiiSum;

            if (deltaX > 0.0f) childOctant |= 1;
            if (deltaY > 0.0f) childOctant |= 2;
            if (deltaZ > 0.0f) childOctant |= 4;

            if (!straddling && childOctant < _nodes.Length)
            {
                if (_nodes[childOctant] == null)
                    _nodes[childOctant] = new OctreeNode<SpaceShip>(_cd, OuterBoundarySize * 0.5f, center);
                // Fully contained in existing child node; insert in that subtree

                float innerBoundry = OuterBoundarySize * 0.5f;
                Vector3 offset = new Vector3(((i & 1) != 0 ? innerBoundry : -innerBoundry)
                    , ((i & 2) != 0 ? innerBoundry : -innerBoundry)
                    , ((i & 4) != 0 ? innerBoundry : -innerBoundry));
                AddHelper(innerBoundry, childOctant, ship, _nodes[i].Center + offset);
            }
            else
            {
                // Straddling, or no child node to descend into, so
                // link object into linked list at this node
                if (_nodes[i].Objects == null)
                    _nodes[i].Objects = new LinkedList<SpaceShip>();
                _nodes[i].Objects.AddLast(ship);
            }
        }

        public void Add(SpaceShip ship)
        {
            _nodes[0] = new OctreeNode<SpaceShip>(_cd, _outerBoundarySize * 0.5f, Vector3.Zero);
            AddHelper(_outerBoundarySize, 0, ship, Vector3.Zero);
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
