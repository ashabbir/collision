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
        public const float Speed = 50.0f, 
                    Scale = 0.3f;
        Rotation _rotation;
        Vector3 _position, _direction;
        Model _model;
        Matrix[] _transforms;
        BoundingBall _boudingBall;

        public BoundingBall CollisionSphere { get { return _boudingBall; } }
        
        public SpaceShip(CollisionDetection cd, Vector3 position) 
        {
            _model = cd.Content.Load<Model>("Models\\ShipModel");
            _transforms = new Matrix[_model.Bones.Count];
            // TODO: start ship from differnt positions so they are not initally colliding
            // or give some time before testing for collision detection
            _position = position;
            _rotation = new Rotation(AxisToRotateUpon(cd.Random));
            if (_position == Vector3.Zero)
            {
                _direction = new Vector3(
                    ((float)cd.Random.NextDouble() - 0.5f) * Speed,
                    ((float)cd.Random.NextDouble() - 0.5f) * Speed,
                    ((float)cd.Random.NextDouble() - 0.5f) * Speed);
            }
            else
                _direction = Vector3.Zero;

            // Extrating vertices
            ModelMeshPart part = _model.Meshes[0].MeshParts[0];
            VertexPositionNormalTexture[]  vertices = new VertexPositionNormalTexture[part.VertexBuffer.VertexCount];
            part.VertexBuffer.GetData<VertexPositionNormalTexture>(vertices);
            _boudingBall = new BoundingBall(cd, vertices, _position);
        }

        public void Update(float elapsedTime, SpaceShip[] spaceShips, BoundingCube boundingCube)
        {
            if (Collides(spaceShips, boundingCube))
                _direction = -_direction;

            //Same scale
            _position += _direction;// *1 / SpaceShip.Scale;
            _boudingBall.Center += _direction;// *1 / BoundingBall.Scale;

            _rotation.Update(elapsedTime);
        }

        public void DrawBoundingVolume(Camera camera)
        {
            _boudingBall.Draw(camera);
        }

        public void Draw(Camera camera)
        {
            // Copy any parent transforms.
            _model.CopyAbsoluteBoneTransformsTo(_transforms);

            // Draw the model. A model can have multiple meshes, so loop.
            foreach (ModelMesh mesh in _model.Meshes)
            {
                // This is where the mesh orientation is set, as well as our camera and projection.
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.World = _transforms[mesh.ParentBone.Index]
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

        /// <summary>
        /// Check if this spaceship collides with any other spaceship or the boundries
        /// </summary>
        /// <returns>Returns true if there is a collsion returns false otherwise</returns>
        private bool Collides(SpaceShip[] spaceShips, BoundingCube boundingCube)
        {
            // Collides against outer boundries
            if (boundingCube.Collides(_boudingBall))
                return true;

            // Collides with another spacehship
            foreach (SpaceShip that in spaceShips)
            {
                // Avoid testing collsion against self
                if (this == that)
                    continue;

                if (this.CollisionSphere.Intersects(that.CollisionSphere) && GjkIntersects(that))
                    return true;
            }

            return false;
        }

        private bool GjkIntersects(SpaceShip other)
        {
            var thisShip = _model.Meshes[0].MeshParts;
            var otherShip = other._model.Meshes[0].MeshParts;

            // TODO: split spaceship into convex polygons

            // TODO: Implement GJK
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
