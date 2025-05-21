using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;




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
    public Vector3 CarHeight { get; set; }
    public float JumpingDistance { get; set; }
    public Model carModel { get; set; }

    public float wheelRadius = 0.1f;
    public float wheelRotation { get; set; }

    public List<Texture2D> carTextures; 
                    

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
        JumpSpeed = 7f;
                        
        Acceleration = 0.8f;
        RotationSpeed = 0.5f;
                        
        CarWorld = Matrix.Identity;
        CarPosition = Vector3.Zero;
        CarHeight = Vector3.Zero;
        JumpingDistance = 0f;
                        
    }
                    
}