using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace CollisionDetection
{
    class BoundingCube
    {
        Boundary[] _boundaries;

        public BoundingCube(CollisionDetection cd, float size)
        {
            float boundaryCenter = size / 2;
            // Outer boundry cube
            _boundaries = new Boundary[6];
            // Top
            _boundaries[0] = new Boundary(Vector3.Up * boundaryCenter, Vector3.Down,
                Vector3.Backward, size, size, cd);
            // Bottom
            _boundaries[1] = new Boundary(Vector3.Down * boundaryCenter, Vector3.Up,
                Vector3.Forward, size, size, cd);
            // Left
            _boundaries[2] = new Boundary(Vector3.Left * boundaryCenter, Vector3.Right,
                Vector3.Up, size, size, cd);
            // Right
            _boundaries[3] = new Boundary(Vector3.Right * boundaryCenter, Vector3.Left,
                Vector3.Up, size, size, cd);
            // Front
            _boundaries[4] = new Boundary(Vector3.Forward * boundaryCenter, Vector3.Backward,
                Vector3.Up, size, size, cd);
            // Back
            _boundaries[5] = new Boundary(Vector3.Backward * boundaryCenter, Vector3.Forward,
                Vector3.Up, size, size, cd);
        }

        public bool Collides(BoundingVolume bv)
        {
            foreach (var boundary in _boundaries)
                if (boundary.Collides(bv))
                    return true;
            return false;
        }

        public void Draw(Camera camera)
        {
            foreach (var boundary in _boundaries)
                boundary.Draw(camera);
        }

        class Boundary
        {
            Vector3 _origin, _upperLeft, _lowerLeft, _upperRight, _lowerRight, _normal, _up, left;
            VertexPositionNormalTexture[] _vertices;
            float _distanceFromOrigin;
            short[] _indices;
            Texture2D _texture;
            BasicEffect _effect;
            CollisionDetection _cd;

            public Boundary(Vector3 origin, Vector3 normal, Vector3 up,
                float width, float height, CollisionDetection cd)
            {
                _vertices = new VertexPositionNormalTexture[4];
                _indices = new short[6];
                _origin = origin;
                _normal = normal;
                _up = up;
                _cd = cd;
                _distanceFromOrigin = _origin.Length();
                left = Vector3.Cross(normal, _up);
                Vector3 uppercenter = (_up * height / 2) + origin;
                _upperLeft = uppercenter + (left * width / 2);
                _upperRight = uppercenter - (left * width / 2);
                _lowerLeft = _upperLeft - (_up * height);
                _lowerRight = _upperRight - (_up * height);

                FillVertices();
                LoadTexture();
            }

            private void LoadTexture()
            {
                _texture = _cd.Content.Load<Texture2D>("Textures\\Glass");
                _effect = new BasicEffect(_cd.GraphicsDevice);
            }

            private void FillVertices()
            {
                // Fill in texture coordinates to display full texture
                // on Boundary
                Vector2 textureUpperLeft = new Vector2(0.0f, 0.0f);
                Vector2 textureUpperRight = new Vector2(1.0f, 0.0f);
                Vector2 textureLowerLeft = new Vector2(0.0f, 1.0f);
                Vector2 textureLowerRight = new Vector2(1.0f, 1.0f);

                // Provide a normal for each vertex
                for (int i = 0; i < _vertices.Length; i++)
                    _vertices[i].Normal = _normal;

                // Set the position and texture coordinate for each vertex
                _vertices[0].Position = _lowerLeft;
                _vertices[0].TextureCoordinate = textureLowerLeft;
                _vertices[1].Position = _upperLeft;
                _vertices[1].TextureCoordinate = textureUpperLeft;
                _vertices[2].Position = _lowerRight;
                _vertices[2].TextureCoordinate = textureLowerRight;
                _vertices[3].Position = _upperRight;
                _vertices[3].TextureCoordinate = textureUpperRight;

                // Set the index buffer for each vertex, using clockwise winding
                _indices[0] = 0;
                _indices[1] = 1;
                _indices[2] = 2;
                _indices[3] = 2;
                _indices[4] = 1;
                _indices[5] = 3;
            }

            public void Draw(Camera camera)
            {
                _effect.EnableDefaultLighting();
                _effect.World = Matrix.Identity;
                _effect.View = camera.View;
                _effect.Projection = camera.Projection;
                _effect.TextureEnabled = true;
                _effect.Texture = _texture;
                _effect.Alpha = 0.5f;

                foreach (EffectPass pass in _effect.CurrentTechnique.Passes)
                {
                    pass.Apply();

                    _cd.GraphicsDevice.DrawUserIndexedPrimitives
                        <VertexPositionNormalTexture>(
                        PrimitiveType.TriangleList,
                        _vertices, 0, 4,
                        _indices, 0, 2);
                }
            }

            public bool Collides(BoundingVolume bv)
            {
                // For a normalized plane (|p.n| = 1), evaluating the plane equation
                // for a point gives the signed distance of the point to the plane
                float distance = Vector3.Dot(bv.Center, _normal) - _distanceFromOrigin;
                // If sphere center within +/-radius from plane, plane intersects sphere
                return Math.Abs(distance) <= bv.Radius;
            }
        }
       
    }
}
