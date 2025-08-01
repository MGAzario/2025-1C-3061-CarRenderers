
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
        public const string ContentFolderTree = "lowpoly_tree/";
        public const string ContentFolderBox = "weapon_box_2/";
        
        
        private static readonly float trackMinX = -10000f, trackMaxX = +10000f;
        private static readonly float trackMinZ = -10000f, trackMaxZ = +10000f;

        private static List<ModelInstance> instances = new List<ModelInstance>();
        private static List<BoundingSphere> listPowerSphere = new List<BoundingSphere>();
        private static List<ModelInstance> treeInstances = new List<ModelInstance>();
        private static List<BoundingSphere> listTreeSphere = new List<BoundingSphere>();
        private static BoundingSphere powerSphere;
        static VertexPositionNormalTexture[] floorVerts;
        public static string[] MeshNames { get; private set; }
        public static string[] Subfolders { get; private set; }
        public static Effect Effect { get; private set; }
        
        private static Effect EarthShader;

        private static Model TreeModel;
        private static Model Box;
        
        private static Texture2D BoxDiffuseTexture;
        private static Texture2D BoxNormalTexture;
        private static Texture2D BoxSpecularTexture;

        private static Texture2D earthTexture;
        
        private static Texture2D treeDiffuseTexture;
        private static Texture2D treeSpecularTexture;
        private static Texture2D treeNormalTexture;
        
        public static readonly Vector3 lightPosition = new Vector3(800f,800f, 800f);
        
        private static BoundingFrustum boundingFrustum = new BoundingFrustum(Matrix.Identity);
        
        



        public static void LoadAll(ContentManager content)
        {

            MeshNames = new[] { "Weapons" };
            Subfolders = new[] { "tgc-media-2023-2c" };
            // Cargo un efecto basico propio declarado en el Content pipeline.
            // En el juego no pueden usar BasicEffect de MG, deben usar siempre efectos propios.

            string boxModelPath = ContentFolderBox + "/weapon_box";
            Box = content.Load<Model>(boxModelPath);

            string boxDiffuseTexturePath = ContentFolderBox + "/diffuse_camo_jungle";
            BoxDiffuseTexture = content.Load<Texture2D>(boxDiffuseTexturePath);
            string boxNormalTexturePath = ContentFolderBox + "/normal";
            BoxNormalTexture = content.Load<Texture2D>(boxNormalTexturePath);
            string boxSpecularTexturePath = ContentFolderBox + "/specular";
            BoxSpecularTexture = content.Load<Texture2D>(boxSpecularTexturePath);

            string floorTexturePath = ContentFolderTextures + "/floor/tierra";
            earthTexture = content.Load<Texture2D>(floorTexturePath);
            
            
            string treePath = ContentFolderTree + "/fbx/huge_tree";
            TreeModel = content.Load<Model>(treePath);
            
            string treeDiffuseTexturePath = ContentFolderTree + "/fbx/diffuse";
            treeDiffuseTexture = content.Load<Texture2D>(treeDiffuseTexturePath);
            string treeSpecularTexturePath = ContentFolderTree + "/fbx/specular";
            treeSpecularTexture = content.Load<Texture2D>(treeSpecularTexturePath);
            string treeNormalTexturePath = ContentFolderTree + "/fbx/normal";
            treeNormalTexture = content.Load<Texture2D>(treeNormalTexturePath);
            
            //ignore the shader file name.
            string earthShaderPath = ContentFolderEffects + "/LootShader";
            EarthShader = content.Load<Effect>(earthShaderPath);
            
            string carShaderPath = ContentFolderEffects + "/Obstacle";
            Effect = content.Load<Effect>(carShaderPath);
            
            floorVerts = new[]
            {
                /*new VertexPositionTexture(new Vector3(-trackMaxX, 0, -trackMinZ), Vector2.Zero),
                new VertexPositionTexture(new Vector3(-trackMaxX, 0, trackMinZ), new Vector2(0,20)),
                new VertexPositionTexture(new Vector3(trackMaxX, 0, -trackMinZ), new Vector2(20,0)),
                        
                new VertexPositionTexture(new Vector3(trackMaxX, 0, -trackMinZ), new Vector2(20,0)),
                new VertexPositionTexture(new Vector3(-trackMaxX, 0, trackMinZ), new Vector2(0,20)),
                new VertexPositionTexture(new Vector3(trackMaxX, 0, trackMinZ), new Vector2(20,20)),*/
                new VertexPositionNormalTexture(new Vector3(-trackMaxX, 0, -trackMinZ), Vector3.Up,new Vector2(  0,  0)),
                new VertexPositionNormalTexture(
                    new Vector3(-trackMaxX, 0,  trackMinZ),
                    Vector3.Up,
                    new Vector2(  0, 20)
                ),
                new VertexPositionNormalTexture(
                    new Vector3( trackMaxX, 0, -trackMinZ),
                    Vector3.Up,
                    new Vector2( 20,  0)
                ),

                new VertexPositionNormalTexture(
                    new Vector3( trackMaxX, 0, -trackMinZ),
                    Vector3.Up,
                    new Vector2( 20,  0)
                ),
                new VertexPositionNormalTexture(
                    new Vector3(-trackMaxX, 0,  trackMinZ),
                    Vector3.Up,
                    new Vector2(  0, 20)
                ),
                new VertexPositionNormalTexture(
                    new Vector3( trackMaxX, 0,  trackMinZ),
                    Vector3.Up,
                    new Vector2( 20, 20)
                ),
            };
            
            
        }

        
        public static void SetObstacleShaderGlobals(IsoCamera camera)
        {
     
            Effect.Parameters["ambientColor"].SetValue(new Vector3(1f, 1f, 1f));
            Effect.Parameters["KAmbient"].SetValue(0.5f);

            Effect.Parameters["diffuseColor"].SetValue(new Vector3(0.1f, 0.1f, 0.6f));
            Effect.Parameters["KDiffuse"].SetValue(1f);
            

            Effect.Parameters["specularColor"].SetValue(new Vector3(1f, 1f, 1f));
            Effect.Parameters["KSpecular"].SetValue(0.8f);
            Effect.Parameters["shininess"].SetValue(8.0f);

            Effect.Parameters["lightPosition"].SetValue(lightPosition);
            Effect.Parameters["eyePosition"].SetValue(camera.eye);
            
        }
        
        public static void SetEarthShaderGlobals(IsoCamera camera)
        {
            
            EarthShader.Parameters["ambientColor"].SetValue(new Vector3(1f, 1f, 1f));
            EarthShader.Parameters["KAmbient"].SetValue(0.5f);

            EarthShader.Parameters["diffuseColor"].SetValue(new Vector3(0.1f, 0.1f, 0.6f));
            EarthShader.Parameters["KDiffuse"].SetValue(1f);
            

            EarthShader.Parameters["specularColor"].SetValue(new Vector3(1f, 1f, 1f));
            EarthShader.Parameters["KSpecular"].SetValue(0.8f);
            EarthShader.Parameters["shininess"].SetValue(8.0f);

            EarthShader.Parameters["lightPosition"].SetValue(lightPosition);
            EarthShader.Parameters["eyePosition"].SetValue(camera.eye);
     
            
            
        }

        
        
        //hahahaha I am copy pasting the weapon one here too. THIS CODE SHOULD GET SHOT.
        public static (List<ModelInstance> treeInstances, List<BoundingSphere> spheres) 
            InitializeTree(ContentManager content)
        {
             for (int i = 0; i < 10; i++)
            {
                
                foreach (var mesh in TreeModel.Meshes)
                {
                    
                    foreach (var meshPart in mesh.MeshParts)
                    {
                        meshPart.Effect = Effect;
                    }

                    var rnd = new Random();

                    float x = rnd.Next(-5000, 5000);
                    float z = rnd.Next(-5000, 5000);

                    var world = Matrix.CreateScale(0.5f) * Matrix.CreateRotationY((float)rnd.NextDouble() *
                                                              MathHelper.TwoPi)
                                                          * Matrix.CreateTranslation(x, 0, z);
                    treeInstances.Add(new ModelInstance { Model = TreeModel, World = world, IsTree = true });
                    
                    //Aca agregamos tambien las diferentes esferas de power. USAMOS ESFERAS PARA POWERS.

                    powerSphere = OBB.CreateSphereFrom(TreeModel);
                    powerSphere.Center = new Vector3(x, 0, z); // we will check if this works or not. 
                    powerSphere.Radius = 500f; // Adjust this when needed.

                    listTreeSphere.Add(powerSphere); // we kinds need to intersect the tank with all the powers we are going to add.
                    //listaCoord.Add(new Vector3(x, 0, z)); 


                }


            }
            
            return (treeInstances, listTreeSphere);
        }

        
        public static (List<ModelInstance> instances, List<BoundingSphere> spheres) 
            InitializeWeapons(ContentManager content)
        {

            for (int i = 0; i < 15; i++)
            {
                

                //en el futuro podemos manipular la parte de arriba para hacer load de todos los recursos. inc escenario.
                //model -> mesh -> mesh parts.
                foreach (var mesh in Box.Meshes)
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

                    var world = Matrix.CreateScale(0.5f) * Matrix.CreateRotationY((float)rnd.NextDouble() *
                                                              MathHelper.TwoPi)
                                                          * Matrix.CreateTranslation(x, 0, z);
                    instances.Add(new ModelInstance { Model = Box, World = world, IsTree = false });

                    //Aca agregamos tambien las diferentes esferas de power. USAMOS ESFERAS PARA POWERS.

                    powerSphere = OBB.CreateSphereFrom(Box);
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
            // 1) Set global Obstacle shader uniforms
            SetObstacleShaderGlobals(camera);

            // 2) Bind the earth texture
            
            /*graphicsDevice.Textures[0]      = earthTexture;*/
            graphicsDevice.Textures[0]      = earthTexture;
            graphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
            EarthShader.Parameters["baseTexture"].SetValue(earthTexture);
            
            SetEarthShaderGlobals(camera);
            // 4) Compute per‑object matrices
            
            var world          = Matrix.Identity;
            var wvp            = world * camera.View * camera.Projection;
            var invTransWorld  = Matrix.Transpose(Matrix.Invert(world));

            EarthShader.Parameters["World"].SetValue(world);
            EarthShader.Parameters["WorldViewProjection"].SetValue(wvp);
            EarthShader.Parameters["InverseTransposeWorld"].SetValue(invTransWorld);

            // 5) Draw the floor
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
        
        
        public static void ObstacleDraw(GraphicsDevice graphicsDevice, IsoCamera camera, 
            List<ModelInstance> list, 
            List<BoundingSphere> sList)
        {
            // First, update all global uniforms
            

            // Then draw each instance whose sphere is visible
            for (int i = 0; i < sList.Count; i++)
            {
                if (!boundingFrustum.Intersects(sList[i])) 
                    continue;

                var inst = list[i];
                foreach (var mesh in inst.Model.Meshes)
                {
                    
                    // Compute per‑object matrices
                    
                    

                    if (list[i].IsTree)
                    {
                        Effect.Parameters["baseTexture"].SetValue(treeDiffuseTexture);
                    
                        Effect.Parameters["normalMap"].SetValue(treeNormalTexture);
                        
                        Effect.Parameters["specularMap"].SetValue(treeSpecularTexture);
                    }
                    else
                    {
                        
                        Effect.Parameters["baseTexture"].SetValue(BoxDiffuseTexture);
                        
                        Effect.Parameters["normalMap"].SetValue(BoxNormalTexture);
                        
                        Effect.Parameters["specularMap"].SetValue(BoxSpecularTexture);
                    }
                    
                    SetObstacleShaderGlobals(camera);
 
                    // wht is this thing here? Multiply by white?
                    /*Effect.Parameters["diffuseColor"].SetValue(new Vector3(1f, 1f, 1f));*/ 
                    
                    var world = mesh.ParentBone.Transform * inst.World;
                    var wvp   = world * camera.View * camera.Projection;
                    var inv   = Matrix.Transpose(Matrix.Invert(world));

                    // Set them
                    Effect.Parameters["World"].SetValue(world);
                    Effect.Parameters["WorldViewProjection"].SetValue(wvp);
                    Effect.Parameters["InverseTransposeWorld"].SetValue(inv);

                    // Draw
                    foreach (var pass in Effect.CurrentTechnique.Passes)
                    {
                        pass.Apply();
                        mesh.Draw();
                    }
                }
            }
        }



    }

}