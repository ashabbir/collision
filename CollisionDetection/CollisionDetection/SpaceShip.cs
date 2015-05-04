using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace CollisionDetection
{
    class SpaceShip
    {
        #region variables
        public int hits = 0;
        public LinkedListNode<SpaceShip> ShipNode { get; set; }
        public Octree.OctreeNode OctTreeNode { get; set; }
        public Boolean Colored { get; set; }
        static int shipCount = 0;
        static float shipPos = 1000;
        public const float Speed = 50.0f, 

                    Scale = 0.25f;
        bool _showHull, _showBall;
        int _collingMeshIndex = 0;
        KeyboardState _oldKeyState;
        Rotation _rotation;
        Vector3 _position, _direction;
        Model _model, _hull_model;
        Matrix[] _modelTransforms, _hullTransforms;
        BoundingCube _boundingCube;
        public List<Hull> ShipHulls { get; set; }
        #endregion

        public BoundingBall CollisionSphere { get; set; }
        public SpaceShip(CollisionDetection cd) 
        {
            _boundingCube = cd.BoudingCube;
            _oldKeyState = Keyboard.GetState();// To avoid null checks on keyboard
            _model = cd.Content.Load<Model>("Models\\ShipModel");
            _hull_model = cd.Content.Load<Model>("Models\\ShipHull");
            _modelTransforms = new Matrix[_model.Bones.Count];
            _hullTransforms = new Matrix[_hull_model.Bones.Count];

            //_position = Vector3.Zero;
            //_position.X = ((shipCount & 1) != 0 ? shipPos : -shipPos);
            //_position.Y = ((shipCount & 2) != 0 ? shipPos : -shipPos);
            //_position.Z = ((shipCount & 4) != 0 ? shipPos : -shipPos);
            //shipCount++;
            //if (shipCount % 8 == 0)
            //    shipPos -= 800f;
            //initial poistion
            _position = new Vector3(
                    ((float)(cd.Random.Next(-5000, 5000) * cd.Random.NextDouble())),
                    ((float)(cd.Random.Next(-5000, 5000) * cd.Random.NextDouble())),
                    ((float)(cd.Random.Next(-500, 500) * cd.Random.NextDouble()))
                    );

            _direction = //Vector3.Zero;
                new Vector3(
                   ((float)cd.Random.NextDouble() - 0.5f) * Speed,
                   ((float)cd.Random.NextDouble() - 0.5f) * Speed,
                   ((float)cd.Random.NextDouble() - 0.5f) * Speed);

            //initial rotation
            _rotation = new Rotation(AxisToRotateUpon(cd.Random));

            #region creating bounding ball
            var meshPart = _model.Meshes[0].MeshParts[0];
            var vpnt = new VertexPositionNormalTexture[meshPart.VertexBuffer.VertexCount];
            meshPart.VertexBuffer.GetData<VertexPositionNormalTexture>(vpnt);
            var vertices = new Vector3[vpnt.Length];
            for (int k = 0; k < vpnt.Length; k++)
            {
                vertices[k] = vpnt[k].Position;
            }
            CollisionSphere = new BoundingBall(cd, vertices, _position);
            #endregion



            #region make hullobject for GJK
             //loop through each mesh of hull
            ShipHulls = new List<Hull>();
            foreach (var hullmesh in _hull_model.Meshes)
            {
                List<Vector3> hull_vertices = new List<Vector3>();
                //now get the vertices and make a hull object and add it to shiphull list
                foreach (var mparts in hullmesh.MeshParts)
                {
                    int vertexStride = mparts.VertexBuffer.VertexDeclaration.VertexStride;
                    
                    var vpnt_hull = new VertexPositionNormalTexture[mparts.NumVertices];
                    mparts.VertexBuffer.GetData<VertexPositionNormalTexture>(vpnt_hull);
                    for (int k = 0; k < mparts.NumVertices; k++)
                        hull_vertices.Add(vpnt_hull[k].Position);
                }
                //how that i have all the vertices in a hull
                //let me add that to ship hull with index number
                ShipHulls.Add(new Hull(hull_vertices , Scale , hullmesh.ParentBone.Index , _rotation));
            }
            #endregion
        }

        public void HandleCollision()
        {
            _direction = -_direction;
            //_rotation.Reflect();
        }

        public void Update(float elapsedTime)
        {
            // Collides against outer boundries
            if (_boundingCube.Collides(CollisionSphere))
                HandleCollision();

            //Update direction
            _position += _direction;// *1 / SpaceShip.Scale;
            CollisionSphere.Center += _direction;// *1 / BoundingBall.Scale;
            ShipHulls.ForEach(h => h.Center += _direction);

            if (Keyboard.GetState().IsKeyDown(Keys.H) && !_oldKeyState.IsKeyDown(Keys.H))
                _showHull = !_showHull;
            if (Keyboard.GetState().IsKeyDown(Keys.B) && !_oldKeyState.IsKeyDown(Keys.B))
                _showBall = !_showBall;

            _oldKeyState = Keyboard.GetState();


            //update rotation
            _rotation.Update(elapsedTime);
            ShipHulls.ForEach(h => h.Rot.Update(elapsedTime));
        }

        public void DrawBoundingVolume(Camera camera)
        {
            if(_showBall)
                CollisionSphere.Draw(camera);
        }

        public void Draw(Camera camera)
        {
            if (_showHull)
            {
                // Copy any parent transforms.
                _hull_model.CopyAbsoluteBoneTransformsTo(_hullTransforms);

                // Draw the model. A model can have multiple meshes, so loop.
                for (int i = 0; i < _hull_model.Meshes.Count; i++)
                {
                    // This is where the mesh orientation is set, as well as our camera and projection.
                    foreach (BasicEffect effect in _hull_model.Meshes[i].Effects)
                    {
                        effect.EnableDefaultLighting();
                        effect.World = _hullTransforms[_hull_model.Meshes[i].ParentBone.Index]
                             * Matrix.CreateScale(Scale)
                             * _rotation.RotationMatrix
                             * Matrix.CreateTranslation(_position);
                        effect.View = camera.View;
                        effect.Projection = camera.Projection;
                        if (i == _collingMeshIndex && Colored)
                        {
                           effect.DiffuseColor = Color.Maroon.ToVector3();
                        }
                        else
                        {
                            effect.DiffuseColor = Color.Green.ToVector3();
                        }
                    }
                    // Draw the mesh, using the effects set above.
                    _hull_model.Meshes[i].Draw();
                }
                
            }
            else
            {
                // Copy any parent transforms.
                _model.CopyAbsoluteBoneTransformsTo(_modelTransforms);

                // Draw the model. A model can have multiple meshes, so loop.
                foreach (ModelMesh mesh in _model.Meshes)
                {
                    // This is where the mesh orientation is set, as well as our camera and projection.
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.EnableDefaultLighting();
                        effect.World = _modelTransforms[mesh.ParentBone.Index]
                             * Matrix.CreateScale(Scale)
                             * _rotation.RotationMatrix
                             * Matrix.CreateTranslation(_position);
                        effect.View = camera.View;
                        effect.Projection = camera.Projection;
                    }
                    // Draw the mesh, using the effects set above.
                    mesh.Draw();
                }
            }
        }

        public bool Collides(SpaceShip that)
        {
            // Avoid testing collsion against self
            if (this == that)
                return false;

            // if we have collision on the sphere 
            if (this.CollisionSphere.Intersects(that.CollisionSphere))
            {
                //if spear intersects show balls 
                that._showBall = true;
                this._showBall = true;
                //run GJK on HULLs
                foreach (var this_hull in this.ShipHulls)
                {
                    foreach (var that_hull in that.ShipHulls)
                        if (GJKAlgorithm.Process(this_hull, that_hull))
                        {
                            this._showHull = true;
                            that._showHull = true;
                            this._collingMeshIndex = this_hull.IndexNo;
                            that._collingMeshIndex = that_hull.IndexNo;
                            this.Colored = true;
                            that.Colored = true;
                            hits++;
                            return true;
                        }
                }

            }
            else 
            {
                return false;
            }

            return false;
        }

        /// <summary>
        /// Randomly decides axis to rotate ship upon
        /// </summary>
        /// <param name="random">Random number generator</param>
        /// <returns>Unit vector representing axis to rotate upon</returns>
        private Vector3 AxisToRotateUpon(Random random)
        {
            switch (random.Next(3))
            {
                case 0:
                    return Vector3.UnitX;
                case 1:
                    return Vector3.UnitY;
                case 2:
                    return Vector3.UnitZ;
                default:
                    throw new ArgumentOutOfRangeException("Value returned by random number generator in AxisToRotateUpon is out of bounds");
            }
        }
    }
}