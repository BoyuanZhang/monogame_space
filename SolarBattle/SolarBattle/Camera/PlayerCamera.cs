using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using SolarBattle.Sprites;
using Microsoft.Xna.Framework.Input;

namespace SolarBattle.Camera
{
    public class PlayerCamera
    {
        public Matrix m_transform;
        
        private Viewport m_view;

        private Vector2 m_cameraFocus;

        private float m_scale;
        private float m_rotation;

        public PlayerCamera( Viewport view, float scale)
        {
            m_view = view;
            m_transform = Matrix.Identity;

            m_cameraFocus = Vector2.Zero;
            m_scale = scale;
            m_rotation = 0;
        }

        public void Update( PlayerShip player)
        {
            //Z, and X used to manipulate scale value of the camera for zooming
            if (Keyboard.GetState().IsKeyDown(Keys.Z))
            {
                if (m_scale < 1.2)
                    m_scale += 0.003f;
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.X))
            {
                if (m_scale > 0.4)
                    m_scale -= 0.003f;
            }

            //C, and V used to manipulate the orientation of the camera
            if (Keyboard.GetState().IsKeyDown(Keys.C))
                m_rotation += 0.005f;

            if (Keyboard.GetState().IsKeyDown(Keys.V))
                m_rotation -= 0.005f;

            if (m_rotation >= 2.0f * (float)Math.PI || m_rotation <= -2.0f * (float)Math.PI)
                m_rotation = 0;

            m_cameraFocus = player.GetCenter();
            //Translate once to focus position, rotate, scale, then translate again to center the camera on the focus. 
            m_transform = Matrix.CreateTranslation(new Vector3(-m_cameraFocus.X, -m_cameraFocus.Y, 1.0f)) *
                            Matrix.CreateRotationZ(m_rotation) *
                            Matrix.CreateScale(new Vector3(m_scale, m_scale, 0)) *
                            Matrix.CreateTranslation(new Vector3( m_view.Width/2, m_view.Height/2, 0));
        }

        public Matrix getTransform()
        {
            return m_transform;
        }

        public Vector2 getCameraFocus()
        {
            return m_cameraFocus;
        }

        public float getScale()
        {
            return m_scale;
        }
    }
}
