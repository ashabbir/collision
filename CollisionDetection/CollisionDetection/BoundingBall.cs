using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CollisionDetection
{
    class BoundingBall
    {
        public const float Scale = SpaceShip.Scale; // Needed because of floating point errors
        Model _model;
        Matrix[] _transforms;
        public Vector3 Center { get; set; }
        public float Radius { get; set; }

        public BoundingBall(CollisionDetection cd, VertexPositionNormalTexture[] vertices, Vector3 position)
        {
            // Find the most separated point pair defining the encompassing AABB
            var mostDistantPoints = MostSeparatedPointsOnAABB(vertices);
            int min = mostDistantPoints.Item1, max = mostDistantPoints.Item2;

            // Set up sphere to just encompass these two points
            // Too much floating point error
            //Center = ((vertices[min].Position + vertices[max].Position) * 0.5f * SpaceShip.Scale) + position;
            Center = position; //* SpaceShip.Scale + position;
            //position = Center;
            Radius = (float)Math.Sqrt(Vector3.Dot(vertices[max].Position - Center, vertices[max].Position - Center)) * SpaceShip.Scale;
            // Loading the sphere's model and saving the bone transform to make it easier to draw
            _model = cd.Content.Load<Model>("Models\\Sphere");
            _transforms = new Matrix[_model.Bones.Count];
        }

        // Compute indices to the two most separated points of the (up to) six points
        // defining the AABB encompassing the point set. Return these as min and max.
        Tuple<int, int> MostSeparatedPointsOnAABB(VertexPositionNormalTexture[] vertices)
        {
            // First find most extreme points along principal axes
            int minx = 0, maxx = 0, miny = 0, maxy = 0, minz = 0, maxz = 0;
            for (int i = 1; i < vertices.Length; i++)
            {
                if (vertices[i].Position.X < vertices[minx].Position.X) minx = i;
                if (vertices[i].Position.X > vertices[maxx].Position.X) maxx = i;
                if (vertices[i].Position.Y < vertices[miny].Position.Y) miny = i;
                if (vertices[i].Position.Y > vertices[maxy].Position.Y) maxy = i;
                if (vertices[i].Position.Z < vertices[minz].Position.Z) minz = i;
                if (vertices[i].Position.Z > vertices[maxz].Position.Z) maxz = i;
            }

            // Compute the squared distances for the three pairs of points
            float XDistanceSquared =Vector3.Dot(vertices[maxx].Position - vertices[minx].Position, vertices[maxx].Position - vertices[minx].Position);
            float YDistanceSquared =Vector3.Dot(vertices[maxy].Position - vertices[miny].Position, vertices[maxy].Position - vertices[miny].Position);
            float ZDistanceSquared =Vector3.Dot(vertices[maxz].Position - vertices[minz].Position, vertices[maxz].Position - vertices[minz].Position);
            // Pick the pair (min,max) of points most distant
            int min = minx;
            int max = maxx;
            if (YDistanceSquared > XDistanceSquared && YDistanceSquared > ZDistanceSquared)
            {
                max = maxy;
                min = miny;
            }
            if (ZDistanceSquared > XDistanceSquared && ZDistanceSquared > YDistanceSquared)
            {
                max = maxz;
                min = minz;
            }

            return Tuple.Create(min, max);
        }

        public bool Intersects(BoundingBall that)
        {
            // We compute the distanace squred to avoid the expensive squred root calculation 
            Vector3 distance = this.Center - that.Center;
            float distaceSquared = Vector3.Dot(distance, distance);
            float radiiSumSquared = this.Radius + that.Radius;
            // Need the square of the radius since we use the squre of the distance
            radiiSumSquared *= radiiSumSquared;
            // Checking bounding volumes for collision
            return distaceSquared <= radiiSumSquared;
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
                        * Matrix.CreateTranslation(Center);
                    effect.View = camera.View;
                    effect.Projection = camera.Projection;
                    effect.Alpha = 0.5f;
                }
                // Draw the mesh, using the effects set above.
                mesh.Draw();
            }
        }
    }
}
