using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
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
        KeyboardState _oldKeyState = Keyboard.GetState();
        int _showLevelMin, _showLevelMax;

        // Statically Create
        public Octree(GraphicsDevice gd, float OuterBoundarySize, SpaceShip[] ships)
        {
            _nodes = new OctreeNode<SpaceShip>[(int)Math.Pow(NumberOfChildrenNodes, Depth) - 1];
            Vector3 offset = Vector3.Zero;
            int nextLevel = 0, currentLevel = 1;
            for (int i = 0; i < _nodes.Length; i++)
            {
                _nodes[i] = new OctreeNode<SpaceShip>(gd, OuterBoundarySize, offset);
                if (i == nextLevel)
                {
                    OuterBoundarySize *= 0.5f;
                    nextLevel = (int)Math.Pow(NumberOfChildrenNodes, currentLevel++);
                }
                offset.X = ((i & 1) != 0 ? OuterBoundarySize : -OuterBoundarySize);
                offset.Y = ((i & 2) != 0 ? OuterBoundarySize : -OuterBoundarySize);
                offset.Z = ((i & 4) != 0 ? OuterBoundarySize : -OuterBoundarySize);
            }
        }

        public void Update()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.D0) && !_oldKeyState.IsKeyDown(Keys.D0))
            {
                _showLevelMin = _nodes.Length;
                _showLevelMax = _nodes.Length;
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.D1) && !_oldKeyState.IsKeyDown(Keys.D1))
            {
                _showLevelMin =  0;
                _showLevelMax =  Math.Min( _nodes.Length, NumberOfChildrenNodes);
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.D2) && !_oldKeyState.IsKeyDown(Keys.D2))
            {
                _showLevelMin = Math.Min(_nodes.Length, NumberOfChildrenNodes);
                _showLevelMax =  Math.Min( _nodes.Length, (int)Math.Pow(NumberOfChildrenNodes, 2));
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.D3) && !_oldKeyState.IsKeyDown(Keys.D3))
            {
                _showLevelMin =  Math.Min( _nodes.Length, (int)Math.Pow(NumberOfChildrenNodes, 2));
                _showLevelMax =  Math.Min( _nodes.Length, (int)Math.Pow(NumberOfChildrenNodes, 3));
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.D4) && !_oldKeyState.IsKeyDown(Keys.D4))
            {
                _showLevelMin =  Math.Min( _nodes.Length, (int)Math.Pow(NumberOfChildrenNodes, 3));
                _showLevelMax =  Math.Min( _nodes.Length, (int)Math.Pow(NumberOfChildrenNodes, 4));
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.D5) && !_oldKeyState.IsKeyDown(Keys.D5))
            {
                _showLevelMin =  Math.Min( _nodes.Length, (int)Math.Pow(NumberOfChildrenNodes, 4));
                _showLevelMax =  Math.Min( _nodes.Length, (int)Math.Pow(NumberOfChildrenNodes, 5));
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.D6) && !_oldKeyState.IsKeyDown(Keys.D6))
            {
                _showLevelMin =  Math.Min( _nodes.Length, (int)Math.Pow(NumberOfChildrenNodes, 5));
                _showLevelMax =  Math.Min( _nodes.Length, (int)Math.Pow(NumberOfChildrenNodes, 6));
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.D7) && !_oldKeyState.IsKeyDown(Keys.D7))
            {
                _showLevelMin =  Math.Min( _nodes.Length, (int)Math.Pow(NumberOfChildrenNodes, 6));
                _showLevelMax =  Math.Min( _nodes.Length, (int)Math.Pow(NumberOfChildrenNodes, 7));
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.D8) && !_oldKeyState.IsKeyDown(Keys.D8))
            {
                _showLevelMin =  Math.Min( _nodes.Length, (int)Math.Pow(NumberOfChildrenNodes, 7));
                _showLevelMax =  Math.Min( _nodes.Length, (int)Math.Pow(NumberOfChildrenNodes, 8));
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.D9) && !_oldKeyState.IsKeyDown(Keys.D9))
            {
                _showLevelMin = 0;
                _showLevelMax = _nodes.Length;
            }
            _oldKeyState = Keyboard.GetState();
        }

        public void Draw(Camera camera)
        {
            for (int i = _showLevelMin; i < _showLevelMax; i++)
                if (_nodes[i] != null)
                    _nodes[i].Draw(camera);
        }

        //CollisionDetection _cd;
        //readonly float _outerBoundarySize;
        //// Dinamically Create
        //public Octree(CollisionDetection cd, float outerBoundrySize)
        //{
        //    _cd = cd;
        //    _outerBoundarySize = outerBoundrySize;
        //    _nodes = new OctreeNode<SpaceShip>[(int)Math.Pow(NumberOfChildrenNodes, Depth)];
        //}

        //// Dynamically create
        //private void AddHelper(float OuterBoundarySize,
        //    int childOctant, SpaceShip ship, Vector3 center)
        //{
        //    //int childOctant = 0;
        //    // If straddling any of the dividing x, y, or z planes, exit directly

        //    var delta = ship.CollisionSphere.Center - _nodes[childOctant].Center;
        //    float radiiSum = _nodes[childOctant].HalfSize + ship.CollisionSphere.Radius;
        //    bool straddling = Math.Abs(delta.X) > radiiSum
        //        ||  Math.Abs(delta.Y) > radiiSum
        //        || Math.Abs(delta.Z) > radiiSum;

        //    if (delta.X > 0.0f) childOctant += 1;
        //    if (delta.Y > 0.0f) childOctant += 2;
        //    if (delta.Z > 0.0f) childOctant += 4;

        //    // Fully contained in existing child node; insert in that subtree
        //    if (!straddling && childOctant < _nodes.Length)
        //    {
                

        //        float innerBoundry = OuterBoundarySize * 0.5f;
        //        Vector3 childCenterOffset =
        //            new Vector3(((childOctant & 1) != 0 ? innerBoundry : -innerBoundry)
        //            , ((childOctant & 2) != 0 ? innerBoundry : -innerBoundry)
        //            , ((childOctant & 4) != 0 ? innerBoundry : -innerBoundry));

        //        if (_nodes[childOctant] == null)
        //            _nodes[childOctant] = new OctreeNode<SpaceShip>(_cd, OuterBoundarySize * 0.5f, center + childCenterOffset);

        //        AddHelper(innerBoundry, childOctant, ship, _nodes[childOctant].Center + childCenterOffset);
        //    }
        //    else
        //    {
        //        // Straddling, or no child node to descend into, so
        //        // link object into linked list at this node
        //        if (_nodes[childOctant].Objects == null)
        //            _nodes[childOctant].Objects = new LinkedList<SpaceShip>();
        //        _nodes[childOctant].Objects.AddLast(ship);
        //    }
        //}

        //public void Add(SpaceShip ship)
        //{
        //    _nodes[0] = new OctreeNode<SpaceShip>(_cd, _outerBoundarySize * 0.5f, Vector3.Zero);
        //    AddHelper(_outerBoundarySize, 0, ship, Vector3.Zero);
        //}

        //public void Add(SpaceShip[] _spaceShips)
        //{
        //    if (_nodes[0] == null)
        //        _nodes[0] = new OctreeNode<SpaceShip>(_cd, _outerBoundarySize * 0.5f, Vector3.Zero);
        //}

        class OctreeNode<T>
        {
            BasicEffect _effect;
            VertexPositionColor[] vertices;
            GraphicsDevice _gd;
            internal Vector3 Center { get; set; }
            internal LinkedList<T> Objects { get; set; }
            internal float HalfSize { get; set; }

            internal OctreeNode(GraphicsDevice gd, float halfSize, Vector3 center)
            {
                _gd = gd;
                Center = center;
                HalfSize = halfSize;

                _effect = new BasicEffect(_gd);
                _effect.VertexColorEnabled = true;
                _effect.World = Matrix.Identity;

                Vector3 frontTopLeft = center + new Vector3(-halfSize, halfSize, halfSize),
                    frontTopRight = center + new Vector3(halfSize, halfSize, halfSize),
                    frontBottomLeft = center + new Vector3(-halfSize, -halfSize, halfSize),
                    frontBottomRight = center + new Vector3(halfSize, -halfSize, halfSize),
                    backTopLeft = center + new Vector3(-halfSize, halfSize, -halfSize),
                    backTopRight = center + new Vector3(halfSize, halfSize, -halfSize),
                    backBottomLeft = center + new Vector3(-halfSize, -halfSize, -halfSize),
                    backBottomRight = center + new Vector3(halfSize, -halfSize, -halfSize);

                vertices = new VertexPositionColor[24];
                Color color = Color.Brown;
                vertices[0] = new VertexPositionColor(frontTopLeft, color);
                vertices[1] = new VertexPositionColor(frontTopRight, color);
                vertices[2] = new VertexPositionColor(frontTopRight, color);
                vertices[3] = new VertexPositionColor(frontBottomRight, color);
                vertices[4] = new VertexPositionColor(frontBottomRight, color);
                vertices[5] = new VertexPositionColor(frontBottomLeft, color);
                vertices[6] = new VertexPositionColor(frontBottomLeft, color);
                vertices[7] = new VertexPositionColor(frontTopLeft, color);
                vertices[8] = new VertexPositionColor(backTopLeft, color);
                vertices[9] = new VertexPositionColor(backTopRight, color);
                vertices[10] = new VertexPositionColor(backTopRight, color);
                vertices[11] = new VertexPositionColor(backBottomRight, color);
                vertices[12] = new VertexPositionColor(backBottomRight, color);
                vertices[13] = new VertexPositionColor(backBottomLeft, color);
                vertices[14] = new VertexPositionColor(backBottomLeft, color);
                vertices[15] = new VertexPositionColor(backTopLeft, color);
                vertices[16] = new VertexPositionColor(frontTopLeft, color);
                vertices[17] = new VertexPositionColor(backTopLeft, color);
                vertices[18] = new VertexPositionColor(frontTopRight, color);
                vertices[19] = new VertexPositionColor(backTopRight, color);
                vertices[20] = new VertexPositionColor(frontBottomLeft, color);
                vertices[21] = new VertexPositionColor(backBottomLeft, color);
                vertices[22] = new VertexPositionColor(frontBottomRight, color);
                vertices[23] = new VertexPositionColor(backBottomRight, color);
            }

            internal void Draw(Camera camera)
            {
                _effect.CurrentTechnique.Passes[0].Apply();
                _effect.View = camera.View;
                _effect.Projection = camera.Projection;
                _gd.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineList, vertices, 0, 12);
            }
        }
    }
}
