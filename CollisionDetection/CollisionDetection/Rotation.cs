using Microsoft.Xna.Framework;
using System;

namespace CollisionDetection
{
    public class Rotation
    {
        // We removed some fuctionality that we don't use in the nuame of perfomance
        float Speed = 0.03f;
        float _rotation;//_rotationX, _rotationY, _rotationZ;
        Vector3 _axisToRotate;

        public Rotation(Random random)
        {
            switch (random.Next(3))
            {
                case 0:
                    _axisToRotate = Vector3.UnitX;
                    break;
                case 1:
                    _axisToRotate = Vector3.UnitY;
                    break;
                case 2:
                    _axisToRotate = Vector3.UnitZ;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("Value returned by random number generator in AxisToRotateUpon is out of bounds");
            }
        }

        public Matrix RotationMatrix
        {
            get
            {
                if (_axisToRotate == Vector3.UnitX)
                    return Matrix.CreateRotationX(_rotation);
                else if (_axisToRotate == Vector3.UnitY)
                    return Matrix.CreateRotationY(_rotation);
                else if (_axisToRotate == Vector3.UnitZ)
                    return Matrix.CreateRotationZ(_rotation);
                else
                    throw new ArgumentException("Invalid value for rotate, only valid values are: Vector3.UnitX, Vector3.UnitY, Vector3.UnitZ");
                //return Matrix.CreateRotationX(_rotationX) *
                //       Matrix.CreateRotationY(_rotationY) *
                //       Matrix.CreateRotationZ(_rotationZ);
            }
        }

        public void Reflect()
        {
            Speed = -Speed;
        }

        public void Update(float elapsedTime)
        {
            //if (_axisToRotate == Vector3.UnitX)
            //    _rotationX += elapsedTime * MathHelper.ToRadians(Speed);
            //else if (_axisToRotate == Vector3.UnitY)
            //    _rotationY += elapsedTime * MathHelper.ToRadians(Speed);
            //else if (_axisToRotate == Vector3.UnitZ)
            //    _rotationZ += elapsedTime * MathHelper.ToRadians(Speed);
            _rotation += elapsedTime * MathHelper.ToRadians(Speed);
        }
    }
}
