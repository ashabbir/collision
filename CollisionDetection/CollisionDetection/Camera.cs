using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CollisionDetection
{
    class Camera
    {
        public const int ScreenWidth = 1920, ScreenHeight = 1080;
        const float AspectRatio = (float)ScreenWidth / (float)ScreenHeight,
            NearPlaneDistance = 1.0f, 
            FarPlaceDistance = 10000.0f, 
            AngularSpeed = 0.05f, 
            InitialX = 0.0f, 
            InitialY = 50.0f, 
            InitialZ = -5000.0f;

        Vector3 _position = new Vector3(InitialX,InitialY,InitialZ), 
            _up = Vector3.Up,
            _left = Vector3.Left,
            _down = Vector3.Down,
            _right = Vector3.Right;



        Matrix _projection = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.PiOver4, AspectRatio, NearPlaneDistance, FarPlaceDistance);
        public Matrix Projection { get { return _projection; } }
        public Matrix View
        {
            get
            {
                return Matrix.CreateLookAt(_position,
                    Vector3.Zero,
                    _up);
            }
        }

        public void Update()
        {
            // Reset camera's postion
            if (Keyboard.GetState().IsKeyDown(Keys.Q))
            { 
                _position = new Vector3(InitialX, InitialY, InitialZ);
                _up = Vector3.Up;
            }


            // Rotate camera around origin
            if (Keyboard.GetState().IsKeyDown(Keys.W))
            {
                _position = Vector3.Transform(_position, Matrix.CreateRotationX(AngularSpeed));
                _up = Vector3.Transform(_up, Matrix.CreateRotationX(AngularSpeed));
            }
            if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                _position = Vector3.Transform(_position, Matrix.CreateRotationY(-AngularSpeed));
                _up = Vector3.Transform(_up, Matrix.CreateRotationY(-AngularSpeed));
            }
            if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                _position = Vector3.Transform(_position, Matrix.CreateRotationX(-AngularSpeed));
                _up = Vector3.Transform(_up, Matrix.CreateRotationX(-AngularSpeed));

            }
            if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                _position = Vector3.Transform(_position, Matrix.CreateRotationY(AngularSpeed));
                _up = Vector3.Transform(_up, Matrix.CreateRotationY(AngularSpeed));
            }
            _up.Normalize();
        }
    }
}
