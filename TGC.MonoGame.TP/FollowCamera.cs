using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TGC.MonoGame.TP.Zero
{

    class FollowCamera
    {
        private const float AxisDistanceToTarget = 1000f;

        private const float AngleFollowSpeed = 0.025f;

        private const float AngleThreshold = 0.75f;

        public Matrix Projection { get; private set; }

        public Matrix View { get; private set; }

        private Vector3 CurrentRightVector { get; set; } = Vector3.Right;

        private float RightVectorInterpolator { get; set; } = 0f;

        private Vector3 PastRightVector { get; set; } = Vector3.Right;
        


//Isometrica = Orthographic and from xyz angles?!
        public FollowCamera(GraphicsDevice graphicsDevice)
        {

            int screenWidth = graphicsDevice.Viewport.Width;
            int screenHeight = graphicsDevice.Viewport.Height;

            Projection = Matrix.CreateOrthographic(screenWidth, screenHeight, 0.01f, 10000f);
        }

        public void Update( Matrix followedWorld)
        {
            
            /*var elapsedTime = Convert.ToSingle(gameTime.ElapsedGameTime.TotalSeconds);*/

            var followedPosition = followedWorld.Translation;
            
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
            

            
            
            var forward = new Vector3(-1,-1,-1);
            forward.Normalize();

            var eye = followedPosition
                      - forward * AxisDistanceToTarget;
                                   
            

            var right = Vector3.Cross(forward, Vector3.Up) ;
            
            var cameraCorrectUp = Vector3.Cross(right, forward);
            
            View = Matrix.CreateLookAt(eye, followedPosition, cameraCorrectUp);
        }


        }
    
    
}