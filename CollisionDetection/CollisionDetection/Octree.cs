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
        internal const int Depth = 3;
        OctreeNode _root;
        KeyboardState _oldKeyState = Keyboard.GetState();
        int keyPressed;
        SpaceShip[] _ships;

        public Octree(GraphicsDevice gd, float OuterBoundarySize, SpaceShip[] ships)
        {
            _ships = ships;
            _root = new OctreeNode(gd, OuterBoundarySize * 0.5f, Vector3.Zero, Depth, 1);
        }

        public void Update(float gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.D0) && !_oldKeyState.IsKeyDown(Keys.D0))
                keyPressed = 0;
            else if (Keyboard.GetState().IsKeyDown(Keys.D1) && !_oldKeyState.IsKeyDown(Keys.D1))
                keyPressed = 1;
            else if (Keyboard.GetState().IsKeyDown(Keys.D2) && !_oldKeyState.IsKeyDown(Keys.D2))
                keyPressed = 2;
            else if (Keyboard.GetState().IsKeyDown(Keys.D3) && !_oldKeyState.IsKeyDown(Keys.D3))
                keyPressed = 3;
            else if (Keyboard.GetState().IsKeyDown(Keys.D4) && !_oldKeyState.IsKeyDown(Keys.D4))
                keyPressed = 4;
            else if (Keyboard.GetState().IsKeyDown(Keys.D5) && !_oldKeyState.IsKeyDown(Keys.D5))
                keyPressed = 5;
            else if (Keyboard.GetState().IsKeyDown(Keys.D6) && !_oldKeyState.IsKeyDown(Keys.D6))
                keyPressed = 6;
            else if (Keyboard.GetState().IsKeyDown(Keys.D7) && !_oldKeyState.IsKeyDown(Keys.D7))
                keyPressed = 7;
            else if (Keyboard.GetState().IsKeyDown(Keys.D8) && !_oldKeyState.IsKeyDown(Keys.D8))
                keyPressed = 8;
            else if (Keyboard.GetState().IsKeyDown(Keys.D9) && !_oldKeyState.IsKeyDown(Keys.D9))
                keyPressed = 9;

            _oldKeyState = Keyboard.GetState();

            // Each spaceship updates itself
            for (int i = 0; i < _ships.Length; i++)
            {
                _ships[i].Update(gameTime);
                RemoveShip(_ships[i]);
                AddShip(_ships[i]);
            }

            _root.TestAllCollision(_root);
        }

        public void AddShip(SpaceShip ship)
        {
            _root.AddShip(ship, _root);
        }

        public void RemoveShip(SpaceShip ship)
        {
            if (ship.OctTreeNode == null)
                return;

            //// See if ship fits in current node
            //Vector3 shipCenter = ship.CollisionSphere.Center, nodeCenter = ship.OctTreeNode.Center;
            //float shipRadius = ship.CollisionSphere.Radius, nodeRadius = ship.OctTreeNode.HalfSize;
            //bool straddling = Math.Abs(nodeCenter.X) + nodeRadius < Math.Abs(shipCenter.X) + shipRadius
            //    || Math.Abs(nodeCenter.Y) + nodeRadius < Math.Abs(shipCenter.Y) + shipRadius
            //    || Math.Abs(nodeCenter.Z) + nodeRadius < Math.Abs(shipCenter.Z) + shipRadius;

            //////var delta = ship.CollisionSphere.Center - ship.OctTreeNode.Center;
            //////float radius = ship.CollisionSphere.Radius + ship.OctTreeNode.HalfSize;
            //////bool straddling = Math.Abs(delta.X) < radius
            //////    || Math.Abs(delta.Y) < radius
            //////    || Math.Abs(delta.Z) < radius;

            //var delta = ship.CollisionSphere.Center - ship.OctTreeNode.Center;
            //float distSquared = Vector3.Dot(delta, delta);
            //bool straddling = distSquared <= ship.CollisionSphere.Radius * ship.CollisionSphere.Radius;

            //if (!straddling && ship.OctTreeNode == _root)
            //{


            // So many failed algorithms, ended up using none at all ='[
                ship.OctTreeNode.Ships.Remove(ship.ShipNode);
                ship.OctTreeNode = null;
            //}
        }

        public void Draw(Camera camera)
        {
            _root.Draw(camera, keyPressed);
        }

        public class OctreeNode
        {
            BasicEffect _effect;
            VertexPositionColor[] _vertices;
            GraphicsDevice _gd;
            int _depth, _currentLevel;
            internal OctreeNode[] Children { get; set; }
            internal Vector3 Center { get; set; }
            internal LinkedList<SpaceShip> Ships { get; set; }
            internal float HalfSize { get; set; }

            internal OctreeNode(GraphicsDevice gd, float halfSize, Vector3 center, int depth, int currentLevel)
            {
                Center = center;
                HalfSize = halfSize;
                Ships = new LinkedList<SpaceShip>();
                _depth = depth;
                _currentLevel = currentLevel;

                CreateEdges(gd, halfSize, center);

                if (depth > 0)
                {
                    Children = new OctreeNode[8];
                    halfSize *= 0.5f;
                    depth--;
                    currentLevel++;
                    Vector3 offset = Vector3.Zero;
                    for (int i = 0; i < Children.Length; i++)
                    {
                        offset.X = ((i & 1) != 0 ? halfSize : -halfSize);
                        offset.Y = ((i & 2) != 0 ? halfSize : -halfSize);
                        offset.Z = ((i & 4) != 0 ? halfSize : -halfSize);
                        Children[i] = new OctreeNode(gd, halfSize, center + offset, depth, currentLevel);
                    }
                }
            }


            internal void AddShip(SpaceShip ship, OctreeNode parent)
            {
                //// Wallace's hack: if center point of the node 
                //// is contained in the sphere it will straddle with the children
                //// therefore we want to keep it on current node
                //var delta = ship.CollisionSphere.Center - this.Center;
                //float distSquared = Vector3.Dot(delta, delta);
                //bool straddling = distSquared <= ship.CollisionSphere.Radius * ship.CollisionSphere.Radius;

                var nodeTop = Math.Abs(this.Center.Y) + HalfSize;
                var nodeBottom = Math.Abs(this.Center.Y) - HalfSize;
                var nodeLeft = Math.Abs(this.Center.X) - HalfSize;
                var nodeRight = Math.Abs(this.Center.X) + HalfSize;
                var nodeFront = Math.Abs(this.Center.Z) + HalfSize;
                var nodeBack = Math.Abs(this.Center.Z) - HalfSize;

                var sphereTop = Math.Abs(ship.CollisionSphere.Center.Y) + ship.CollisionSphere.Radius;
                var sphereBottom = Math.Abs(ship.CollisionSphere.Center.Y) - ship.CollisionSphere.Radius;
                var sphereLeft = Math.Abs(ship.CollisionSphere.Center.X) - ship.CollisionSphere.Radius;
                var sphereRight = Math.Abs(ship.CollisionSphere.Center.X) + ship.CollisionSphere.Radius;
                var sphereFront = Math.Abs(ship.CollisionSphere.Center.Z) + ship.CollisionSphere.Radius;
                var sphereBack = Math.Abs(ship.CollisionSphere.Center.Z) - ship.CollisionSphere.Radius;

                bool straddles = sphereTop >= nodeTop
                    || sphereBottom <= nodeBottom
                    || sphereLeft <= nodeLeft
                    || sphereRight >= nodeRight
                    || sphereFront >= nodeFront
                    || sphereBack <= nodeBack;
                
                // Usefull for debug
                //_vertices[22] = new VertexPositionColor(ship.CollisionSphere.Center, Color.Black);
                //_vertices[23] = new VertexPositionColor(ship.CollisionSphere.Center + new Vector3(0,-ship.CollisionSphere.Radius, 0) ,  Color.Black);

                //var center = Vector3.Transform(Center,
                //Matrix.CreateTranslation(ship.CollisionSphere.Center));

                // Fully contained in existing child node; insert in that subtree
                if (!straddles && _depth > 0)
                {
                    int childOctant = 0;
                    var delta = ship.CollisionSphere.Center - this.Center;
                    if (delta.X > 0.0f) childOctant += 1;
                    if (delta.Y > 0.0f) childOctant += 2;
                    if (delta.Z > 0.0f) childOctant += 4;

                    Children[childOctant].AddShip(ship, this);
                }
                else // Straddles or no children left
                {
                    ship.OctTreeNode = parent;
                    ship.ShipNode = parent.Ships.AddLast(ship);
                }
            }

            static Stack<OctreeNode> ancesstorStack = new Stack<OctreeNode>(Octree.Depth);
            internal void TestAllCollision(OctreeNode node)
            {
                ancesstorStack.Push(node);

                foreach (var ancestor in ancesstorStack)
                    foreach (var ancestorShip in ancestor.Ships)
                        foreach (var ship in node.Ships)
                        {
                            if (ship == ancestorShip)
                                break;

                            if (ship.Collides(ancestorShip))
                            {
                                ship.HandleCollision();
                                ancestorShip.HandleCollision();
                            }
                        }
                if (_depth > 0)
                    foreach (var child in Children)
                        child.TestAllCollision(child);

                ancesstorStack.Pop();
            }

            void CreateEdges(GraphicsDevice gd, float halfSize, Vector3 center)
            {
                _gd = gd;
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

                _vertices = new VertexPositionColor[24];
                Color color = Color.Brown;
                _vertices[0] = new VertexPositionColor(frontTopLeft, color);
                _vertices[1] = new VertexPositionColor(frontTopRight, color);
                _vertices[2] = new VertexPositionColor(frontTopRight, color);
                _vertices[3] = new VertexPositionColor(frontBottomRight, color);
                _vertices[4] = new VertexPositionColor(frontBottomRight, color);
                _vertices[5] = new VertexPositionColor(frontBottomLeft, color);
                _vertices[6] = new VertexPositionColor(frontBottomLeft, color);
                _vertices[7] = new VertexPositionColor(frontTopLeft, color);
                _vertices[8] = new VertexPositionColor(backTopLeft, color);
                _vertices[9] = new VertexPositionColor(backTopRight, color);
                _vertices[10] = new VertexPositionColor(backTopRight, color);
                _vertices[11] = new VertexPositionColor(backBottomRight, color);
                _vertices[12] = new VertexPositionColor(backBottomRight, color);
                _vertices[13] = new VertexPositionColor(backBottomLeft, color);
                _vertices[14] = new VertexPositionColor(backBottomLeft, color);
                _vertices[15] = new VertexPositionColor(backTopLeft, color);
                _vertices[16] = new VertexPositionColor(frontTopLeft, color);
                _vertices[17] = new VertexPositionColor(backTopLeft, color);
                _vertices[18] = new VertexPositionColor(frontTopRight, color);
                _vertices[19] = new VertexPositionColor(backTopRight, color);
                _vertices[20] = new VertexPositionColor(frontBottomLeft, color);
                _vertices[21] = new VertexPositionColor(backBottomLeft, color);
                _vertices[22] = new VertexPositionColor(frontBottomRight, color);
                _vertices[23] = new VertexPositionColor(backBottomRight, color);
            }

            internal void Draw(Camera camera, int keyPressed)
            {
                if (keyPressed == _currentLevel || keyPressed == 9 || (keyPressed == 8 && Ships.Count > 0))
                {
                    _effect.CurrentTechnique.Passes[0].Apply();
                    _effect.View = camera.View;
                    _effect.Projection = camera.Projection;
                    _gd.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineList, _vertices, 0, 12);
                }

                if (_depth > 0)
                    foreach (var child in Children)
                        child.Draw(camera, keyPressed);
            }
        }
    }
}
