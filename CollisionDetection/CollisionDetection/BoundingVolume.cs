using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CollisionDetection
{
    // TODO: Don't use XNA implementation, use our own implementation
    // cheating for now so we can get the other parts of the code working
    class BoundingVolume
    {
        readonly float Scale = 5f;
        Model _model;
        Matrix[] _transforms;
        public Vector3 Center { get; set; }
        public float Radius { get; set; }

        // TODO: delete me
        public BoundingVolume(Vector3 center, float radius, CollisionDetection cd)
        {
            Center = center;
            Center += new Vector3(-30f, -10f, -10f);
            Radius = radius;

            _model = cd.Content.Load<Model>("Models\\Sphere");
            _transforms = new Matrix[_model.Bones.Count];
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
                        * Matrix.CreateTranslation(Center)
                        * Matrix.CreateScale(Scale);
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
