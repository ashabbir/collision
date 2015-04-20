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
        const float Speed = 50.0f, 
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
            _position = position * 1 / Scale;
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
            // TODO: delete me
            _boudingBall = new BoundingBall(_position, _model.Meshes[0].BoundingSphere.Radius * 1.1f, cd);
        }

        public void Update(float elapsedTime, SpaceShip[] spaceShips, BoundingCube boundingCube)
        {
            if (Collides(spaceShips, boundingCube))
                _direction = -_direction;

            _position += _direction * 1/Scale;
            _boudingBall.Center += _direction * 1 / BoundingBall.Scale;


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
                        * _rotation.RotationMatrix
                        * Matrix.CreateTranslation(_position)
                        * Matrix.CreateScale(Scale);
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
            Vector3 center = _boudingBall.Center;
            float radius = _boudingBall.Radius;

            // Collides against outer boundries
            if (boundingCube.Collides(_boudingBall))
                return true;

            // Collides with another spacehship
            foreach (SpaceShip ship in spaceShips)
            {
                // Avoid testing collsion against self
                if (ship == this)
                    continue;

                // We compute the distanace squred to avoid the expensive squred root calculation 
                Vector3 distance = center - ship._boudingBall.Center;
                float distaceSquared = Vector3.Dot(distance, distance);
                float radiiSumSquared = radius + ship._boudingBall.Radius;
                // Need the square of the radius since we use the squre of the distance
                radiiSumSquared *= radiiSumSquared;
                // Checking bounding volumes for collision
                bool SpheresIntersect = distaceSquared <= radiiSumSquared;

                if (SpheresIntersect && GjkIntersects(ship))
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
