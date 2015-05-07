using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace CollisionDetection
{
    public class BoundingBall
    {
        Model _model;
        Matrix[] _transforms;
        Vector3 _center;
        public Vector3 Center
        {
            get { return _center; }
            set 
            {
                _center = value;
                if (Color.Y < 1)
                    Color = new Vector3(1, Color.Y + 0.005f, Color.Z + 0.005f);
                    //Color = Vector3.Lerp(Color, Vector3.One, 0.005f);
            }
        }
        public float Radius { get; set; }
        SpaceShip _ship;
        public Vector3 Color { get; set; }

        public BoundingBall(CollisionDetection cd, Vector3[] vertices, SpaceShip ship)
        {
            _ship = ship;
            // Find the most separated point pair defining the encompassing AABB
            var mostDistantPoints = MostSeparatedPointsOnAABB(vertices);
            int min = mostDistantPoints.Item1, max = mostDistantPoints.Item2;

            // Set up sphere to just encompass these two points
            Center = (vertices[min] + vertices[max]) * 0.5f;

            Radius = (float)Math.Sqrt(Vector3.Dot(vertices[max] - Center, vertices[max] - Center));

            // Grow sphere to include all points
            for (int i = 0; i < vertices.Length; i++)
                SphereOfSphereAndPt(vertices[i]);

            // Loading the sphere's model and saving the bone transform to make it easier to draw
            _model = cd.Content.Load<Model>("Models\\Sphere");
            _transforms = new Matrix[_model.Bones.Count];
            Center = _ship.Position; // Ship is slightly off center, but radius is good
            Color = Vector3.One;
        }

        // Given Sphere s and Point p, update s (if needed) to just encompass p
        void SphereOfSphereAndPt(Vector3 p)
        {
            // Compute squared distance between point and sphere center
            Vector3 d = p - Center;
            float distnaceSqured = Vector3.Dot(d, d);
            // Only update s if point p is outside it
            if (distnaceSqured > Radius * Radius)
            {
                float dist = (float)Math.Sqrt(distnaceSqured);
                float newRadius = (Radius + dist) * 0.5f;
                float k = (newRadius - Radius) / dist;
                Radius = newRadius;
                Center += d * k;
            }
        }

        // Compute indices to the two most separated points of the (up to) six points
        // defining the AABB encompassing the point set. Return these as min and max.
        Tuple<int, int> MostSeparatedPointsOnAABB(Vector3[] vertices)
        {
            // First find most extreme points along principal axes
            int minx = 0, maxx = 0, miny = 0, maxy = 0, minz = 0, maxz = 0;
            for (int i = 1; i < vertices.Length; i++)
            {
                if (vertices[i].X < vertices[minx].X) minx = i;
                if (vertices[i].X > vertices[maxx].X) maxx = i;
                if (vertices[i].Y < vertices[miny].Y) miny = i;
                if (vertices[i].Y > vertices[maxy].Y) maxy = i;
                if (vertices[i].Z < vertices[minz].Z) minz = i;
                if (vertices[i].Z > vertices[maxz].Z) maxz = i;
            }

            // Compute the squared distances for the three pairs of points
            float XDistanceSquared =Vector3.Dot(vertices[maxx] - vertices[minx], vertices[maxx] - vertices[minx]);
            float YDistanceSquared =Vector3.Dot(vertices[maxy] - vertices[miny], vertices[maxy] - vertices[miny]);
            float ZDistanceSquared =Vector3.Dot(vertices[maxz] - vertices[minz], vertices[maxz] - vertices[minz]);
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
                    effect.World = _transforms[mesh.ParentBone.Index] * _ship.Transform;
                    effect.View = camera.View;
                    effect.Projection = camera.Projection;
                    effect.Alpha = 0.5f;
                    effect.DiffuseColor = Color;
                }
                // Draw the mesh, using the effects set above.
                mesh.Draw();
            }
        }
    }
}
