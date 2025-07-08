using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TGC.MonoGame.TP;



public class UniversePhysics
{
    public float FallingSpeed { get; }
    public Vector3 JumpingDirection { get; }

    public UniversePhysics(float fallingSpeed, Vector3 jumpingDirection)
    {
                        FallingSpeed = fallingSpeed;
                        JumpingDirection = jumpingDirection;
    }

    public UniversePhysics()
    {
                        FallingSpeed = 5.0f;
                        JumpingDirection = new Vector3(0.0f, 1.0f, 0.0f);
    }
} 
                
public class Cars
{
                    
    public float Speed { get; set; }
    public float CarRotation { get; set; }
    public bool Jumping { get; set; }
    public float JumpSpeed { get; set; }
    public float Acceleration { get; set; }
    public float RotationSpeed { get; set; }
    public Matrix CarWorld { get; set; }

    public Vector3 CarPosition;
    public Vector3 CarHeight { get; set; } // wht do I even have this? I am using it for box,nothing else. 
    public float JumpingDistance { get; set; }
    public Model carModel { get; set; }

    public float wheelRadius = 0.1f;
    public float wheelRotation { get; set; }
    
    public float carDamage { get; set; }
    
    public float carHp { get; set; }

    public List<Texture2D> carTextures; 
    
    static Effect carEffect;
    
    
    Vector3 headLightPos   = new Vector3(0, 2, 5);// this keep it like this for now.
                    

    public Cars(float speed, float carRotation, bool jumping,float jumpSpeed, float acceleration,
        float rotationSpeed,  Matrix carWorld, Vector3 carPosition, Vector3 carHeight, float jumpingDistance)
    {
        Speed = speed;
        CarRotation = carRotation;
        Jumping = jumping;
        JumpSpeed = jumpSpeed;
                        
        Acceleration = acceleration;
        RotationSpeed = rotationSpeed;
                        
        CarWorld = carWorld;
        CarPosition = carPosition;
        CarHeight = carHeight;
        JumpingDistance = jumpingDistance;                
    }

    public Cars()
    {
        Speed = 0f;
                        
        CarRotation = 0f;
        Jumping = false;
        JumpSpeed = 2;
                        
        Acceleration = 2f;
        RotationSpeed = 1f;
                        
        CarWorld = Matrix.Identity;
        CarPosition = Vector3.Zero;
        CarHeight = Vector3.Zero;
        JumpingDistance = 0f;
                        
    }

    public Cars(Vector3 Position)
    {

        Speed = 0f;
        CarRotation = 0f;
        Jumping = false;
        JumpSpeed = 2f;
        Acceleration = 1.8f;
        RotationSpeed = 1f;
        
        CarWorld = Matrix.Identity;
        CarPosition = Position;
        CarHeight = Vector3.Zero;
        JumpingDistance = 0f;
    }
    
    
    public static void SetEffect(Effect fx)
    {
        carEffect = fx;
    }
    
    
    
    public void drawMainCarOnCenter(IsoCamera camera)
    {
        
        
        foreach (var mesh in carModel.Meshes)
        {
            var baseWorld = Matrix.CreateRotationY(CarRotation)
                            * Matrix.CreateTranslation(CarHeight)
                            * Matrix.CreateTranslation(CarPosition);

            Matrix world   = mesh.ParentBone.Transform * baseWorld;
            Matrix wvp     = world * camera.View * camera.Projection;
            Matrix invTrW  = Matrix.Transpose(Matrix.Invert(world));

            carEffect.Parameters["WorldViewProjection"].SetValue(wvp);
            carEffect.Parameters["World"].SetValue(world);
            carEffect.Parameters["InverseTransposeWorld"].SetValue(invTrW);
            
            
            //DON'T REALLY KNOW WHAT TO DO WITH THIS.!!!
            /*carEffect.Parameters["headLight"].SetValue(headLightPos);*/// keep it fixed for now will change it later.

            if (mesh.Name.Contains("Wheel"))
            {
                var spin = Matrix.CreateRotationX(wheelRotation * 50f /
                                                  wheelRadius);
                carEffect.Parameters["World"].SetValue(spin * mesh.ParentBone.Transform * baseWorld);
                
            }
            else
            {
                carEffect.Parameters["World"].SetValue(mesh.ParentBone.Transform * baseWorld);
            }

            foreach (var pass in carEffect.CurrentTechnique.Passes)
            {
                mesh.Draw();
            }
            
        }
    }

    public void drawCars(IsoCamera camera)
    {
        
        foreach (var mesh in carModel.Meshes)
        {
            var baseWorld = Matrix.CreateRotationY(CarRotation)
                            * Matrix.CreateTranslation(CarHeight)
                            * Matrix.CreateTranslation(CarPosition);

            Matrix world   = mesh.ParentBone.Transform * baseWorld;
            Matrix wvp     = world * camera.View * camera.Projection;
            Matrix invTrW  = Matrix.Transpose(Matrix.Invert(world));

            carEffect.Parameters["WorldViewProjection"].SetValue(wvp);
            carEffect.Parameters["World"].SetValue(world);
            carEffect.Parameters["InverseTransposeWorld"].SetValue(invTrW);
            
            
            //DON'T REALLY KNOW WHAT TO DO WITH THIS.!!!
            /*carEffect.Parameters["headLight"].SetValue(headLightPos);*/// keep it fixed for now will change it later.

            if (mesh.Name.Contains("Wheel"))
            {
                var spin = Matrix.CreateRotationX(wheelRotation * 10f /
                                                  wheelRadius);
                carEffect.Parameters["World"].SetValue(spin * mesh.ParentBone.Transform * baseWorld);
                
            }
            else
            {
                carEffect.Parameters["World"].SetValue(mesh.ParentBone.Transform * baseWorld);
            }

            foreach (var pass in carEffect.CurrentTechnique.Passes)
            {
                mesh.Draw();
            }
            
        }
        
    }


    public float steerTargetAI(Cars targetCar, float elapsedTime)
    {
        Vector3 toTargetPath = this.toTargetPath(targetCar);
        float desiredYaw =  (float)Math.Atan2(toTargetPath.X, toTargetPath.Z);
        
        float delta = desiredYaw - CarRotation;
        
        if (delta >  Math.PI) delta -= 2 * (float)Math.PI;
        if (delta < -Math.PI) delta += 2 * (float)Math.PI;
        
        float maxThisFrame = (Speed/2) * elapsedTime;
        delta = Math.Clamp(delta, -maxThisFrame, +maxThisFrame);//the car turns too fast. Fix this later.

        return delta;

    }

    public Vector3 toTargetPath(Cars targetCar)
    {
        Vector3 auxPath = targetCar.CarPosition - CarPosition;
        auxPath.Normalize();
        return auxPath;
    }

    public Vector3 advanceForward(float elapsedTime)
    {
        Vector3 carDirectionForward = new Vector3((float)Math.Sin(CarRotation), 0,
            (float)Math.Cos(CarRotation));

        Speed += Acceleration;
        
        Vector3 finalCarposition = carDirectionForward * (float)Math.Clamp(Speed, -5, 10);
        // we are hard coding 20 as max speed. This is somehow broke, I think it's elapsedTime.

        return finalCarposition;
    } 

    public Matrix movementAI(Cars targetCar, float elapsedTime)
    {
        
        /*with steerTargetAI you can know the final destination of the car.
            but how to reach that BS.*/

        float turnDelta = steerTargetAI(targetCar, elapsedTime);
        CarRotation += turnDelta;
        CarPosition += advanceForward(elapsedTime);
        CarWorld = Matrix.CreateRotationY(CarRotation)
                   * Matrix.CreateTranslation(CarPosition)
                   * Matrix.CreateTranslation(CarHeight);

        //CarWorld = Matrix.CreateRotationY(steerAI(targetCar))

        return CarWorld;
    }
                    
}



