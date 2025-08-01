using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TGC.MonoGame.TP
{

    public class IsoCamera
    {
        private const float AxisDistanceToTarget = 10000f;
        
        public  Matrix Projection { get; set; }

        public  Matrix View { get; set; }
        
        public  Vector3 eye;
        


//Isometrica = Orthographic and from xyz angles?!
        public IsoCamera(GraphicsDevice graphicsDevice)
        {

            int screenWidth = graphicsDevice.Viewport.Width;
            int screenHeight = graphicsDevice.Viewport.Height;

            Projection = Matrix.CreateOrthographic(screenWidth, screenHeight, 0.001f, 100000f);
        }

        public void Update( Matrix followedWorld)
        {
            
            //Interpolator no deberia ser necesario para nuestro caso.

            var followedPosition = followedWorld.Translation; // la posicion del objetivo de la camara.
            
            
            const float Elevation = 0.7f;        /// vamos a tener que incrementar esto un poco despues.
            const float BackDistance = 1.0f;   

            var forward = new Vector3(BackDistance, -Elevation, BackDistance);
            forward.Normalize();

            eye = followedPosition - forward * AxisDistanceToTarget; // la posicion del ojo(camara)

            var right = Vector3.Cross(forward, Vector3.Up);
            
            var cameraCorrectUp = Vector3.Cross(right, forward);
            
            View = Matrix.CreateLookAt(eye, followedPosition, cameraCorrectUp);
        }


        }
    
    
}