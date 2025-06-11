using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TGC.MonoGame.TP
{

    class IsoCamera
    {
        private const float AxisDistanceToTarget = 10000f;

        private const float AngleFollowSpeed = 0.025f;

        private const float AngleThreshold = 0.75f;

        public Matrix Projection { get; private set; }

        public Matrix View { get; private set; }

        private Vector3 CurrentRightVector { get; set; } = Vector3.Right;

        private float RightVectorInterpolator { get; set; } = 0f;

        private Vector3 PastRightVector { get; set; } = Vector3.Right;
        


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
            
            
            /*var elapsedTime = Convert.ToSingle(gameTime.ElapsedGameTime.TotalSeconds);*/

            var followedPosition = followedWorld.Translation; // la posicion del objetivo de la camara.
            
            /*var followedRight = followedWorld.Right;*/
            
            /*if (Vector3.Dot(followedRight, PastRightVector) > AngleThreshold)
            {
                RightVectorInterpolator += elapsedTime * AngleFollowSpeed;
                
                RightVectorInterpolator = MathF.Min(RightVectorInterpolator, 1f);
                
                CurrentRightVector = Vector3.Lerp(CurrentRightVector, followedRight, RightVectorInterpolator * RightVectorInterpolator);
            }
            else
                RightVectorInterpolator = 0f;

       
            PastRightVector = followedRight;*/
            
            
            const float Elevation = 0.7f;     
            const float BackDistance = 1.0f;   

            var forward = new Vector3(BackDistance, -Elevation, BackDistance);
            forward.Normalize();

            var eye = followedPosition - forward * AxisDistanceToTarget; // la posicion del ojo(camara)

            var right = Vector3.Cross(forward, Vector3.Up) ;
            
            var cameraCorrectUp = Vector3.Cross(right, forward);
            
            View = Matrix.CreateLookAt(eye, followedPosition, cameraCorrectUp);
        }


        }
    
    
}