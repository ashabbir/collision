using Microsoft.Xna.Framework;
using System;

namespace CollisionDetection
{
    public class Rotation
    {
        float Speed = 0.01f;
        float _rotationX, _rotationY, _rotationZ;
        Vector3 _axisToRotate;
        public Rotation(Vector3 axisToRotate)
        {
            if (axisToRotate == Vector3.UnitX
             || axisToRotate == Vector3.UnitY
             || axisToRotate == Vector3.UnitZ)
                _axisToRotate = axisToRotate;
            else
                throw new ArgumentException("Invalid value for rotate, only valid values are: Vector3.UnitX, Vector3.UnitY, Vector3.UnitZ");
        }

        public Matrix RotationMatrix
        {
            get
            {
                return Matrix.CreateRotationX(_rotationX) *
                       Matrix.CreateRotationY(_rotationY) *
                       Matrix.CreateRotationZ(_rotationZ);
            }
        }

        public void Reflect()
        {
            Speed = -Speed;
        }

        public void Update(float elapsedTime)
        {
            if (_axisToRotate == Vector3.UnitX)
                _rotationX += elapsedTime * MathHelper.ToRadians(Speed);
            else if (_axisToRotate == Vector3.UnitY)
                _rotationY += elapsedTime * MathHelper.ToRadians(Speed);
            else if (_axisToRotate == Vector3.UnitZ)
                _rotationZ += elapsedTime * MathHelper.ToRadians(Speed);
        }
    }
}
