using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace CollisionDetection
{
    class SpaceShip
    {

        #region variables
        public int hits = 0;
        public Boolean Colored { get; set; }
        
        public const float Speed = 50.0f, 
                    Scale = 0.3f;
        bool _showHull, _showBall;
        int _collingMeshIndex = -1;
        KeyboardState _oldKeyState;
        Rotation _rotation;
        Vector3 _position, _direction;
        Model _model, _hull_model;
        Matrix[] _modelTransforms, _hullTransforms;
        //Vector3[][] _hullVertices;
        public List<Hull> ShipHulls { get; set; }

        #endregion

        public BoundingBall CollisionSphere { get; set; }
        public SpaceShip(CollisionDetection cd) 
        {
            _showBall = true;
            _showHull = true;
            _oldKeyState = Keyboard.GetState();// To avoid null checks on keyboard
            _model = cd.Content.Load<Model>("Models\\ShipModel");
            _hull_model = cd.Content.Load<Model>("Models\\ShipHull");
            _modelTransforms = new Matrix[_model.Bones.Count];
            _hullTransforms = new Matrix[_hull_model.Bones.Count];
            // TODO: start ship from differnt positions so they are not initally colliding
            // or give some time before testing for collision detection

            _position = new Vector3(
                    ((float)(cd.Random.Next(-100,100) * cd.Random.NextDouble())),
                    ((float)(cd.Random.Next(-100,100) * cd.Random.NextDouble())),
                    ((float)(cd.Random.Next(-10,10) * cd.Random.NextDouble()))
                    );

            _direction = new Vector3(
                   ((float)cd.Random.NextDouble() - 0.5f) * Speed,
                   ((float)cd.Random.NextDouble() - 0.5f) * Speed,
                   ((float)cd.Random.NextDouble() - 0.5f) * Speed);


            _rotation = new Rotation(AxisToRotateUpon(cd.Random));


            // Extrating vertices from model
            {// Model is only has one mesh
                var meshPart = _model.Meshes[0].MeshParts[0];
                var vpnt = new VertexPositionNormalTexture[meshPart.VertexBuffer.VertexCount];
                meshPart.VertexBuffer.GetData<VertexPositionNormalTexture>(vpnt);
                var vertices = new Vector3[vpnt.Length];
                for (int k = 0; k < vpnt.Length; k++)
                    vertices[k] = vpnt[k].Position;
                CollisionSphere = new BoundingBall(cd, vertices, _position);
            }
            
            
            //loop through each mesh of hull
            ShipHulls = new List<Hull>();
            foreach (var hullmesh in _hull_model.Meshes)
            {
                List<Vector3> hull_vertices = new List<Vector3>();
                //now get the vertices and make a hull object and add it to shiphull list
                foreach (var meshPart in hullmesh.MeshParts)
                {
                    int vertexStride = meshPart.VertexBuffer.VertexDeclaration.VertexStride;
                    
                    var vpnt = new VertexPositionNormalTexture[meshPart.NumVertices];
                    meshPart.VertexBuffer.GetData<VertexPositionNormalTexture>(vpnt);
                    for (int k = 0; k < meshPart.NumVertices; k++)
                        hull_vertices.Add(vpnt[k].Position);
                }
                //how that i have all the vertices in a hull
                //let me add that to ship hull with index number
                ShipHulls.Add(new Hull(hull_vertices , Scale , hullmesh.ParentBone.Index));
            }

           

        }

        public void Update(float elapsedTime, SpaceShip[] spaceShips, BoundingCube boundingCube)
        {
            if (Collides(spaceShips, boundingCube))
                _direction = -_direction;

            //Same scale
            _position += _direction;// *1 / SpaceShip.Scale;
            CollisionSphere.Center += _direction;// *1 / BoundingBall.Scale;
            ShipHulls.ForEach(h => h.Center += _direction);

            if (Keyboard.GetState().IsKeyDown(Keys.H) && !_oldKeyState.IsKeyDown(Keys.H))
                _showHull = !_showHull;
            if (Keyboard.GetState().IsKeyDown(Keys.B) && !_oldKeyState.IsKeyDown(Keys.B))
                _showBall = !_showBall;

            _oldKeyState = Keyboard.GetState();

            _rotation.Update(elapsedTime);
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

        /// <summary>
        /// Check if this spaceship collides with any other spaceship or the boundries
        /// </summary>
        /// <returns>Returns true if there is a collsion returns false otherwise</returns>
        private bool Collides(SpaceShip[] spaceShips, BoundingCube boundingCube)
        {
            // Collides against outer boundries
            if (boundingCube.Collides(CollisionSphere))
                return true;

            // Collides with another spacehship
            foreach (SpaceShip that in spaceShips)
            {
                // Avoid testing collsion against self
                if (this == that)
                    continue;


                //first check for spear if the spears intersect then go further into
                //gjk for ship hulls
                if (!this.CollisionSphere.Intersects(that.CollisionSphere))
                {
                    continue;
                }
                //Console.WriteLine("initial Collision detected");

                //GJK on HULLs
                foreach (var this_hull in this.ShipHulls)
                {
                    foreach (var that_hull in that.ShipHulls)
                    {
                        if (GJKAlgorithm.Intersects(this_hull, that_hull))
                        {
                            this._collingMeshIndex = this_hull.IndexNo;
                            that._collingMeshIndex = that_hull.IndexNo;
                            //Console.WriteLine("GJK Detected");
                            hits++;
                            this.Colored = true;
                            that.Colored = true;
                            return true;
                        }
                        
                    }
                    
                }

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
