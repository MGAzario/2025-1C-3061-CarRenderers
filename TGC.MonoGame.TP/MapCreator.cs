
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TGC.MonoGame.TP.Content.Collisions;
using Microsoft.Xna.Framework.Content;

namespace TGC.MonoGame.TP.Content
{



    public static class MapCreator
    {

        public const string ContentFolder3D = "Models/";
        public const string ContentFolderEffects = "Effects/";
        public const string ContentFolderMusic = "Music/";
        public const string ContentFolderSounds = "Sounds/";
        public const string ContentFolderSpriteFonts = "SpriteFonts/";
        public const string ContentFolderTextures = "Textures/";
        
        private static readonly float trackMinX = -10000f, trackMaxX = +10000f;
        private static readonly float trackMinZ = -10000f, trackMaxZ = +10000f;

        private static List<ModelInstance> instances = new List<ModelInstance>();
        private static List<BoundingSphere> listPowerSphere = new List<BoundingSphere>();
        private static BoundingSphere powerSphere;
        static VertexPositionTexture[] floorVerts;
        public static string[] MeshNames { get; private set; }
        public static string[] Subfolders { get; private set; }
        public static Effect Effect { get; private set; }
        
        private static Effect EarthShader;

        private static Texture2D earthTexture;
        
        public static readonly Vector3 lightPosition = new Vector3(30f,30f, -30f);
        
        private static BoundingFrustum boundingFrustum = new BoundingFrustum(Matrix.Identity);
        
        
        
        


        public static void LoadAll(ContentManager content)
        {

            MeshNames = new[] { "Weapons" };
            Subfolders = new[] { "tgc-media-2023-2c" };
            // Cargo un efecto basico propio declarado en el Content pipeline.
            // En el juego no pueden usar BasicEffect de MG, deben usar siempre efectos propios.
            Effect = content.Load<Effect>(ContentFolderEffects + "BasicShader");

            string floorTexturePath = ContentFolderTextures + "/floor/tierra";
            earthTexture = content.Load<Texture2D>(floorTexturePath);
            
            string carShaderPath = ContentFolderEffects + "/Obstacle";
            EarthShader = content.Load<Effect>(carShaderPath);
        }

        
        public static (List<ModelInstance> instances, List<BoundingSphere> spheres) 
            InitializeWeapons(ContentManager content)
        {

            for (int i = 0; i < 15; i++)
            {

                string name = MeshNames[0]; // Le estamos entergando RacingCar podemos
                string assetPath = ContentFolder3D + string.Join("/", Subfolders) + "/" + name;
                var model = content.Load<Model>(assetPath);

                //en el futuro podemos manipular la parte de arriba para hacer load de todos los recursos. inc escenario.
                //model -> mesh -> mesh parts.
                foreach (var mesh in model.Meshes)
                {
                    // Un mesh puede tener mas de 1 mesh part (cada 1 puede tener su propio efecto).
                    // need a list for collision matrix centres and other stuffs. Same number of list and stuff. Jesus.

                    foreach (var meshPart in mesh.MeshParts)
                    {
                        meshPart.Effect = Effect;
                    }

                    var rnd = new Random();

                    float x = rnd.Next(-5000, 5000);
                    float z = rnd.Next(-5000, 5000);

                    var world = Matrix.CreateScale(0.05f) * Matrix.CreateRotationY((float)rnd.NextDouble() *
                                                              MathHelper.TwoPi)
                                                          * Matrix.CreateTranslation(x, 0, z);
                    instances.Add(new ModelInstance { Model = model, World = world });

                    //Aca agregamos tambien las diferentes esferas de power. USAMOS ESFERAS PARA POWERS.

                    powerSphere = OBB.CreateSphereFrom(model);
                    powerSphere.Center = new Vector3(x, 0, z); // we will check if this works or not. 
                    powerSphere.Radius = 100f; // Adjust this when needed.

                    listPowerSphere.Add(powerSphere); // we kinds need to intersect the tank with all the powers we are going to add.
                    //listaCoord.Add(new Vector3(x, 0, z)); 


                }


            }

            return (instances, listPowerSphere);
        }

        public static void EarthDraw(GraphicsDevice graphicsDevice, IsoCamera camera)
        {
            
            floorVerts = new[]
            {
                new VertexPositionTexture(new Vector3(-trackMaxX, 0, -trackMinZ), Vector2.Zero),
                new VertexPositionTexture(new Vector3(-trackMaxX, 0, trackMinZ), new Vector2(0,20)),
                new VertexPositionTexture(new Vector3(trackMaxX, 0, -trackMinZ), new Vector2(20,0)),
                        
                new VertexPositionTexture(new Vector3(trackMaxX, 0, -trackMinZ), new Vector2(20,0)),
                new VertexPositionTexture(new Vector3(-trackMaxX, 0, trackMinZ), new Vector2(0,20)),
                new VertexPositionTexture(new Vector3(trackMaxX, 0, trackMinZ), new Vector2(20,20)),
            };
            
            graphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
            graphicsDevice.Textures[0] = earthTexture;
            
            Matrix world = Matrix.Identity;
            Matrix wvp = world * camera.View * camera.Projection;
            Matrix invTransWorld = Matrix.Transpose(Matrix.Invert(world));
            
            EarthShader.Parameters["WorldViewProjection"].SetValue(wvp);
            EarthShader.Parameters["World"].SetValue(world);
            EarthShader.Parameters["InverseTransposeWorld"].SetValue(invTransWorld);
            EarthShader.Parameters["eyePosition"].SetValue(camera.eye);
            
            EarthShader.Parameters["ambientColor"].SetValue(new Vector3(0.8f, 0.8f, 0.8f));
            EarthShader.Parameters["diffuseColor"].SetValue(new Vector3(1f, 1f, 1f));
            EarthShader.Parameters["specularColor"].SetValue(new Vector3(1f, 1f, 1f));

            EarthShader.Parameters["KAmbient"].SetValue(0.5f);
            EarthShader.Parameters["KDiffuse"].SetValue(0.8f);
            EarthShader.Parameters["KSpecular"].SetValue(0.8f);
            EarthShader.Parameters["shininess"].SetValue(10.0f);
                    
            EarthShader.Parameters["lightPosition"].SetValue(lightPosition);

            EarthShader.Parameters["SceneSamp+SceneTex"].SetValue(earthTexture);
            foreach (var pass in EarthShader.CurrentTechnique.Passes)
            {
                pass.Apply();

                graphicsDevice.DrawUserPrimitives(
                    PrimitiveType.TriangleList,
                    floorVerts,
                    0,
                    2
                );
            }
        }

        public static void UpdateFrustum(BoundingFrustum fr)
        {
            boundingFrustum = fr;
        }
        public static void WeaponsDraw()
        {
            for (int i = 0; i < listPowerSphere.Count; i++) // this works!! 
            {
                
                /*if (boundingFrustum == null)
                    throw new Exception("boundingFrustum is null!");*/
                BoundingSphere auxSphere = listPowerSphere[i];
                if (boundingFrustum.Intersects(auxSphere))
                {
                    foreach (var inst in instances)
                    {
                        foreach (var mesh in inst.Model.Meshes)
                        {
                            Effect.Parameters["World"].SetValue(mesh.ParentBone.Transform * inst.World);
                            mesh.Draw();
                        }
                    }
                    /*UI.DrawTextOnXY("Detect" + i, windowCornerTR, 1 );*/
                }
            }
            
        }



    }

}