using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace CollisionDetection
{
    public class SpaceShip
    {
        #region variables
        public LinkedListNode<SpaceShip> ShipNode { get; set; }
        public Octree.OctreeNode OctTreeNode { get; set; }
        public Boolean Colored { get; set; }
        static readonly Matrix _scale = Matrix.CreateScale(Size);
        public static Matrix Scale { get { return _scale; } }
        public Matrix Transform { get; private set; }
        public Vector3 Position { get { return _position; } }
        public float Radius { get { return _model.Meshes[0].BoundingSphere.Radius * SpaceShip.Size; } }
        public const float Speed = 50.0f, Size = 0.25f;
        bool _showHull, _showBall;
        int _collingMeshIndex = 0;
        KeyboardState _oldKeyState;
        Rotation _rotation;
        Vector3 _position, _direction;
        Model _model, _hullModel;
        Matrix[] _modelTransforms, _hullTransforms;
        BoundingCube _boundingCube;
        public List<Hull> ShipHulls { get; set; }
        #endregion

        public BoundingBall CollisionSphere { get; set; }
        public SpaceShip(CollisionDetection cd, Vector3 position) 
        {
            _boundingCube = cd.BoudingCube;
            _oldKeyState = Keyboard.GetState();// To avoid null checks on keyboard
            _model = cd.Content.Load<Model>("Models\\ShipModel");
            _hullModel = cd.Content.Load<Model>("Models\\ShipHull");
            _modelTransforms = new Matrix[_model.Bones.Count];
            _hullTransforms = new Matrix[_hullModel.Bones.Count];
            _position = position;
            _direction = //Vector3.Zero;
            new Vector3(
               ((float)cd.Random.NextDouble() - 0.5f) * Speed,
               ((float)cd.Random.NextDouble() - 0.5f) * Speed,
               ((float)cd.Random.NextDouble() - 0.5f) * Speed);

            //initial rotation
            _rotation = new Rotation(cd.Random);

            
            // TODO:  need to scale the guys right here!!!
            
            #region creating bounding ball
            {
                var meshPart = _model.Meshes[0].MeshParts[0];
                var vpnt = new VertexPositionNormalTexture[meshPart.VertexBuffer.VertexCount];
                meshPart.VertexBuffer.GetData<VertexPositionNormalTexture>(vpnt);
                var vertices = new Vector3[vpnt.Length];
                for (int i = 0; i < vpnt.Length; i++)
                    vertices[i] = Vector3.Transform(vpnt[i].Position, Scale);
                CollisionSphere = new BoundingBall(cd, vertices, this);
            }
            #endregion

            #region make hullobject for GJK
             //loop through each mesh of hull
            ShipHulls = new List<Hull>(_hullModel.Meshes.Count);
            foreach (var hullmesh in _hullModel.Meshes)
            {
                Vector3[] vertices = null;
                //now get the vertices and make a hull object and add it to shiphull list
                foreach (var mparts in hullmesh.MeshParts)
                {
                    int vertexStride = mparts.VertexBuffer.VertexDeclaration.VertexStride;
                    var vpnt = new VertexPositionNormalTexture[mparts.NumVertices];
                    mparts.VertexBuffer.GetData<VertexPositionNormalTexture>(vpnt);
                    vertices = new Vector3[vpnt.Length];
                    for (int i = 0; i < mparts.NumVertices; i++)
                        vertices[i] = vpnt[i].Position;
                }
                //how that i have all the vertices in a hull
                //let me add that to ship hull with index number
                ShipHulls.Add(new Hull(vertices, hullmesh.ParentBone.Index, this));
            }
            #endregion
        }

        public void HandleCollision()
        {
            _direction = -_direction;
            _rotation.Reflect();
        }

        public void Update(float elapsedTime)
        {
            // Collides against outer boundries
            if (_boundingCube.Collides(CollisionSphere))
                HandleCollision();

            //Update direction
            _position += _direction;// *1 / SpaceShip.Scale;
            //CollisionSphere.Center += _direction;// *1 / BoundingBall.Scale;

            if (Keyboard.GetState().IsKeyDown(Keys.H) && !_oldKeyState.IsKeyDown(Keys.H))
                _showHull = !_showHull;
            if (Keyboard.GetState().IsKeyDown(Keys.B) && !_oldKeyState.IsKeyDown(Keys.B))
                _showBall = !_showBall;

            _oldKeyState = Keyboard.GetState();

            Transform = Scale * _rotation.RotationMatrix * Matrix.CreateTranslation(_position);

            //update rotation
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
                _hullModel.CopyAbsoluteBoneTransformsTo(_hullTransforms);

                // Draw the model. A model can have multiple meshes, so loop.
                for (int i = 0; i < _hullModel.Meshes.Count; i++)
                {
                    // This is where the mesh orientation is set, as well as our camera and projection.
                    foreach (BasicEffect effect in _hullModel.Meshes[i].Effects)
                    {
                        effect.EnableDefaultLighting();
                        effect.World = _hullTransforms[_hullModel.Meshes[i].ParentBone.Index] * Transform;
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
                    _hullModel.Meshes[i].Draw();
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
                        effect.World = _modelTransforms[mesh.ParentBone.Index] * Transform;
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
                //if sphear intersects show balls 
                //that._showBall = true;
                //this._showBall = true;
                //run GJK on HULLs
                foreach (var thisHull in this.ShipHulls)
                {
                    foreach (var thatHull in that.ShipHulls)
                        if (GJKAlgorithm.Process(thisHull, thatHull))
                        {
                            //this._showHull = true;
                            //that._showHull = true;
                            this._collingMeshIndex = thisHull.IndexNo;
                            that._collingMeshIndex = thatHull.IndexNo;
                            this.Colored = true;
                            that.Colored = true;
                            return true;
                        }
                }
            }
            return false;
        }        
    }
}