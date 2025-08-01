        using System;
        using System.Collections.Generic;
        using Microsoft.Xna.Framework;
        using Microsoft.Xna.Framework.Audio;
        using Microsoft.Xna.Framework.Graphics;
        using Microsoft.Xna.Framework.Input;
        using TGC.MonoGame.TP.Content.Collisions;
        using TGC.MonoGame.TP.Content;
        using System.Linq;

        namespace TGC.MonoGame.TP
        {
            /// <summary>
            ///     Esta es la clase principal del juego.
            ///     Inicialmente puede ser renombrado o copiado para hacer mas ejemplos chicos, en el caso de copiar para que se
            ///     ejecute el nuevo ejemplo deben cambiar la clase que ejecuta Program <see cref="Program.Main()" /> linea 10.
            /// </summary>
           
            public class ModelInstance
            {
            public Model Model;
            public Matrix World;
            public bool IsTree = false;
            }
            public class TGCGame : Game
            {

                public const int PRESENTATION_0_SCREN = 0;// beginning
                public const int MAIN_1_SCREEN = 1; // main car crashing stuff.
                public const int END_SCREEN = 2;
                public int STATUS = PRESENTATION_0_SCREN;
                
                public readonly Vector3 lightPosition = new Vector3(800f,800f, 800f);
                
                public const string ContentFolder3D = "Models/";
                public const string ContentFolderEffects = "Effects/";
                public const string ContentFolderMusic = "Music/";
                public const string ContentFolderSounds = "Sounds/";
                public const string ContentFolderSpriteFonts = "SpriteFonts/";
                public const string ContentFolderTextures = "Textures/";
                private List<ModelInstance> instances;
                private List<ModelInstance> TreeInstances;
                private List<Vector3> listaCoord; // Do we need this?
                
                private IsoCamera IsoCamera { get; set; }
                private CubeMapCamera cubeMapCamera;
                private Cars mainCar;
                private Cars enemyCar1;
                private float maxSpeed = 28;
                private float minSpeed = -8;
                private UniversePhysics standardPhysics;
                private float elapsedTime;
                private float timer;
                private int finalSpeed;
                //private int finalRotationSpeed;
                
                private BoundingSphere powerSphere;
                private List<BoundingSphere> listPowerSphere;
                private List<BoundingSphere> listTreeSphere;
                //private List<BoundingBox> listTerrain;
                
                private OBB carBox;
                private Matrix carOBBWorld; // Do we even need it?
                private Matrix[] rampWorld;
                private BoxPrimitive boxPrimitive;
                
                private Matrix floorWorld;
                private Plane floorPlane;
                private RenderTargetCube environmentMap;
                
                private readonly float trackMinX = -10000f, trackMaxX = +10000f;
                private readonly float trackMinZ = -10000f, trackMaxZ = +10000f;

                private Effect carShader;
                BasicEffect lineEffect;
                VertexPositionColor[] borderVerts;
                VertexPositionTexture[] floorVerts;
                
                private Texture2D earthTexture;
                private Texture2D carTexture;
                
                private SoundEffect accelerationSound;
                private SoundEffectInstance accelerationInst;
                private SoundEffect pingSound;
                private SoundEffectInstance pingInst;
                
                private SpriteFont font;
                private KeyboardState previousKeyboardState;

                private int windowW;
                private int windowH; //perhaps change this later
                private int windowX;
                private int windowY;
                private Point windowCornerTL;   //top left
                private Point windowCornerTR;
                private Point windowCornerBL;
                private Point windowCornerBR;  //Bottom Right.

                private BoundingFrustum boundingFrustum;
                
                
                public TGCGame()
                {
                    // Maneja la configuracion y la administracion del dispositivo grafico.
                    Graphics = new GraphicsDeviceManager(this);
                    
                    Graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width - 100;
                    Graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height - 100;
                    
                    
                    Content.RootDirectory = "Content";
                    // Hace que el mouse sea visible.
                    IsMouseVisible = true;
                }

                
                private GraphicsDeviceManager Graphics { get; }
                private SpriteBatch SpriteBatch { get; set; }
                private Model Model { get; set; }
                private Effect Effect { get; set; }
                private float Rotation { get; set; }
                private Matrix World { get; set; }
                private Matrix View { get; set; }
                private Matrix Projection { get; set; }

                /// <summary>
                ///     Se llama una sola vez, al principio cuando se ejecuta el ejemplo.
                ///     Escribir aqui el codigo de inicializacion: el procesamiento que podemos pre calcular para nuestro juego.
                /// </summary>
                
                protected override void Initialize()
                {
                    // La logica de inicializacion que no depende del contenido se recomienda poner en este metodo.

                    // Apago el backface culling.
                    // Esto se hace por un problema en el diseno del modelo del logo de la materia.
                    // Una vez que empiecen su juego, esto no es mas necesario y lo pueden sacar.
                    
                    var rasterizerState = new RasterizerState();
                    rasterizerState.CullMode = CullMode.None;
                    GraphicsDevice.RasterizerState = rasterizerState;
                    // Seria hasta aca.
                    
                    borderVerts = new[] {
                        new VertexPositionColor(new Vector3(trackMinX, 0, trackMinZ), Color.Red),
                        new VertexPositionColor(new Vector3(trackMaxX, 0, trackMinZ), Color.Red),
                        new VertexPositionColor(new Vector3(trackMaxX, 0, trackMaxZ), Color.Red),
                        new VertexPositionColor(new Vector3(trackMinX, 0, trackMaxZ), Color.Red),
                    };

                    floorVerts = new[]
                    {
                        new VertexPositionTexture(new Vector3(-trackMaxX/2, 0, -trackMinZ/2), Vector2.Zero),
                        new VertexPositionTexture(new Vector3(-trackMaxX/2, 0, trackMinZ/2), new Vector2(0,20)),
                        new VertexPositionTexture(new Vector3(trackMaxX/2, 0, -trackMinZ/2), new Vector2(20,0)),
                        
                        new VertexPositionTexture(new Vector3(trackMaxX/2, 0, -trackMinZ/2), new Vector2(20,0)),
                        new VertexPositionTexture(new Vector3(-trackMaxX/2, 0, trackMinZ/2), new Vector2(0,20)),
                        new VertexPositionTexture(new Vector3(trackMaxX/2, 0, trackMinZ/2), new Vector2(20,20)),
                    };

                    rampWorld = new[] {
                        Matrix.CreateScale(800f, 4f, 200f) * Matrix.CreateTranslation(0f, 0f, 1200) * Matrix.CreateRotationX(45f),
                        Matrix.CreateScale(800f, 4f, 200f) * Matrix.CreateTranslation(0f, 0f, -1200) * Matrix.CreateRotationX(-45f),
                        Matrix.CreateScale(800f,4f,1150f) * Matrix.CreateTranslation(0f, -936f,0f),
                    };
                    
                    // Configuramos nuestras matrices de la escena.
                    
                    World = Matrix.Identity;
                    View = Matrix.CreateLookAt(new Vector3(0, 300000, 70000), Vector3.Zero, Vector3.Up);
                    Projection =
                        Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, GraphicsDevice.Viewport.AspectRatio, 0.1f, 1000000f);

                    IsoCamera = new IsoCamera(GraphicsDevice); 
                    mainCar = new Cars();
                    enemyCar1 = new Cars(new Vector3(1500f, 0, 1500f));
                    
                    standardPhysics =  new UniversePhysics();
                    
                    //the frustrum.
                    boundingFrustum = new BoundingFrustum(IsoCamera.View * IsoCamera.Projection);
                    
                    floorWorld = Matrix.CreateScale(20000f, 0.1f, 20000f);// creation of floor draw material
                    
                    floorPlane = new Plane(Vector3.UnitZ, new Vector3(0, 0, 0));

                    
                    Rectangle client = Window.ClientBounds;
                    windowH = client.Height; // entire window height
                    windowW = client.Width; // entire window width
                    windowX = client.X; // distance from window left
                    windowY = client.Y; // distance from window down

                    windowCornerTL = new Point(windowX, windowY);
                    windowCornerTR = new Point(windowW + windowX, windowY);
                    windowCornerBL = new Point(windowX, windowY + windowH);
                    windowCornerBR = new Point(windowX + windowW, windowY + windowH);
                    
                    GraphicsDevice.BlendState = BlendState.AlphaBlend;
                    base.Initialize();
                    
                }

                
                
                /// <summary>
                ///     Se llama una sola vez, al principio cuando se ejecuta el ejemplo, despues de Initialize.
                ///     Escribir aqui el codigo de inicializacion: cargar modelos, texturas, estructuras de optimizacion, el procesamiento
                ///     que podemos pre calcular para nuestro juego.
                /// </summary>
                protected override void LoadContent()
                {
                    // Aca es donde deberiamos cargar todos los contenido necesarios antes de iniciar el juego.
                    SpriteBatch = new SpriteBatch(GraphicsDevice);
                    
                    lineEffect = new BasicEffect(GraphicsDevice) {
                        VertexColorEnabled = true,
                        Projection = IsoCamera.Projection,
                    };
                    string carShaderPath = ContentFolderEffects + "/CarShader";
                    carShader = Content.Load<Effect>(carShaderPath);

                    //here we load the shader for all cars.
                    Cars.SetEffect(carShader);
                    
                    instances = new List<ModelInstance>(); 
                    listPowerSphere = new List<BoundingSphere>();
                    TreeInstances = new List<ModelInstance>();
                    listTreeSphere = new List<BoundingSphere>();
                    
                    //here we load the car models
                    string playCarPath = ContentFolder3D + "/tgc-media-2023-2c/RacingCar";
                    mainCar.carModel = Content.Load<Model>(playCarPath);
                    enemyCar1.carModel = Content.Load<Model>(playCarPath);
                    
                    environmentMap = new RenderTargetCube(GraphicsDevice, 2048, false,
                        SurfaceFormat.Color, DepthFormat.Depth24, 0, RenderTargetUsage.DiscardContents);
                    //here we get the texture from BasicEffect.... you know, maybe it works.
                    
                    string carTexturePath = ContentFolder3D + "/tgc-media-2023-2c/Vehicle_basecolor";
                    carTexture = Content.Load<Texture2D>(carTexturePath);
                    
                    //here we apply the carShader to mainCar andbelow to enemyCar1. Later we will have a list of enemyCars.
                    //a singular car is only for test change that later.
                    foreach (var modelMesh in mainCar.carModel.Meshes)
                    {
                        foreach (var meshPart in modelMesh.MeshParts)
                        {
                            
                            meshPart.Effect = carShader;
                          
                        }
                    }
                    
                    foreach (var modelMesh in enemyCar1.carModel.Meshes)
                    {
                        foreach (var meshPart in modelMesh.MeshParts)
                        {
                            meshPart.Effect = carShader;
                        }
                    }
                    
                    
                    
                    carShader.Parameters["baseTexture"].SetValue(carTexture);
                    GraphicsDevice.SamplerStates[0] = SamplerState.LinearClamp;
                    //CONSIDER USING A DIFFERENT INSTACE OF CARSHADER FOR ENEMYCARS. OR NOT.
                    //////Here we set some of the parameters of the CarShader. Maybe we move this to a different location later. 
                    /*carShader.Parameters["SceneTex"].SetValue(texture);*/
                    /*GraphicsDevice.Textures[0] = carTexture;
                    GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;*/
                    
                    
                    carShader.Parameters["ambientColor"].SetValue(new Vector3(1f, 1f, 1f));  
                    carShader.Parameters["KAmbient"].SetValue(0.5f);
                    
                    carShader.Parameters["diffuseColor"].SetValue(new Vector3(0.1f, 0.1f, 0.6f));
                    carShader.Parameters["specularColor"].SetValue(new Vector3(1f, 1f, 1f));

               
                    carShader.Parameters["KDiffuse"].SetValue(1.0f);
                    carShader.Parameters["KSpecular"].SetValue(1.5f);
                    carShader.Parameters["shininess"].SetValue(8.0f);
                    carShader.Parameters["lightPosition"].SetValue(lightPosition);
                    
                    /*World = mainCar.CarWorld;*/
                    boxPrimitive = new BoxPrimitive(GraphicsDevice, Vector3.One, earthTexture);// Apprently we do need this. ??? why?
                    MapCreator.LoadAll(Content);// loads weapons floor texture path. and other things on map.
                    (instances, listPowerSphere) = MapCreator.InitializeWeapons(Content);// what name suggest. This inlcude spheres for power.
                    (TreeInstances, listTreeSphere) = MapCreator.InitializeTree(Content);
                    
                    string carAccelerationPath = ContentFolderSounds + "/carAcceleration/car acceleration";
                    accelerationSound = Content.Load<SoundEffect>(carAccelerationPath);
                    accelerationInst       = accelerationSound.CreateInstance();
                    accelerationInst.IsLooped = true;
                    accelerationInst.Volume   = 0.1f;
                    
                    string pingPath = ContentFolderSounds + "/powerPackPing/ping";
                    pingSound = Content.Load<SoundEffect>(pingPath);
                    pingInst = pingSound.CreateInstance();
                    pingInst.IsLooped = false;
                    pingInst.Volume   = 0.2f;

                    string fontPath = ContentFolderSpriteFonts + "/rough/RoughSplash";
                    font = Content.Load<SpriteFont>(fontPath);
                    UI.Initialize(font,GraphicsDevice);
                    
                    //// we load the cars here.
                    
                   
                   //OBB box creation.
                   var temporaryCubeAABB = OBB.CreateAABBFrom(mainCar.carModel);
                   temporaryCubeAABB = OBB.Scale(temporaryCubeAABB, 0.5f);
                   
                   carBox = OBB.FromAABB(temporaryCubeAABB);
                   carBox.Center = Vector3.UnitX * 50f;
                   carBox.Orientation = Matrix.CreateRotationY(mainCar.CarRotation);

                   cubeMapCamera = new CubeMapCamera(mainCar.CarPosition, 0.1f, 1000f);
                   
                   base.LoadContent();
                }

                /// <summary>
                ///     Se llama en cada frame.
                ///     Se debe escribir toda la logica de computo del modelo, asi como tambien crificar entradas del usuario y reacciones
                ///     ante ellas.
                /// </summary>
                
                protected override void Update(GameTime gameTime)
                {
                    
                    
                    elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
                    boundingFrustum.Matrix = IsoCamera.View * IsoCamera.Projection; // continous update of the bouding box via camera.
                    
                    // Aca deberiamos poner toda la logica de actualizacion del juego.

                    // Capturar Input teclado
                    
                    var keyboardState = Keyboard.GetState();
                    if (keyboardState.IsKeyDown(Keys.Escape))
                    {
                        //Salgo del juego.
                        Exit();
                    }

                    switch (STATUS)
                    {
                        
                        case PRESENTATION_0_SCREN:
                            if (keyboardState.IsKeyDown(Keys.Space))
                                STATUS = MAIN_1_SCREEN;
                            break;
                        case END_SCREEN:
                            if (keyboardState.IsKeyDown(Keys.Space))
                                STATUS = MAIN_1_SCREEN;
                            if (keyboardState.IsKeyDown(Keys.Enter))
                                STATUS = PRESENTATION_0_SCREN;
                            break;
                        default:
                        {

                            timer += elapsedTime;
                            if (keyboardState.IsKeyDown(Keys.Space) && !mainCar.Jumping)
                            {
                                // Here comes jumping: it means it should jump and let the gravity do its work.
                                mainCar.Jumping = true;
                            }

                            if (mainCar.Jumping)
                            {
                                mainCar.JumpSpeed -= standardPhysics.FallingSpeed * elapsedTime;
                                mainCar.CarHeight += standardPhysics.JumpingDirection * mainCar.JumpSpeed;
                                mainCar.JumpingDistance += mainCar.JumpSpeed * elapsedTime;

                                if (mainCar.JumpingDistance <= 0f)
                                {
                                    mainCar.JumpingDistance = 0f;
                                    mainCar.Jumping = false;
                                    mainCar.JumpSpeed = 7f;
                                }

                            }

                            Vector3 carDirection = new Vector3((float)Math.Sin(mainCar.CarRotation), 0,
                                (float)Math.Cos(mainCar.CarRotation));

                            if (keyboardState.IsKeyDown((Keys.W)) || keyboardState.IsKeyDown((Keys.S)) ||
                                mainCar.Speed != 0)
                            {
                                if (keyboardState.IsKeyDown(Keys.W)) //Here come accelearation
                                    mainCar.Speed += mainCar.Acceleration * elapsedTime;


                                if (keyboardState.IsKeyDown(Keys.S)) //Here comes desacceleration
                                    mainCar.Speed -= mainCar.Acceleration * elapsedTime;

                                if (keyboardState.IsKeyUp((Keys.W)) && keyboardState.IsKeyUp((Keys.S)))
                                    mainCar.Speed -= mainCar.Speed * mainCar.Acceleration * elapsedTime;


                                if (keyboardState.IsKeyDown(Keys.A)) // Here comes turn left
                                    mainCar.CarRotation += mainCar.RotationSpeed * elapsedTime *
                                                           Math.Min(1f, mainCar.Speed / 2);

                                if (keyboardState.IsKeyDown(Keys.D)) //Here comes turn right
                                    mainCar.CarRotation -= mainCar.RotationSpeed * elapsedTime *
                                                           Math.Min(1f, mainCar.Speed / 2);


                                finalSpeed = (int)Math.Clamp(mainCar.Speed, minSpeed, maxSpeed);
                                mainCar.CarPosition += carDirection * finalSpeed;
                                mainCar.wheelRotation = (mainCar.Speed * elapsedTime);
                            }

                            //sound part

                            if (keyboardState.IsKeyDown(Keys.W) && !previousKeyboardState.IsKeyDown(Keys.W))
                                accelerationInst.Play();

                            if (!keyboardState.IsKeyDown(Keys.W) && previousKeyboardState.IsKeyDown(Keys.W))
                                accelerationInst.Stop();

                            previousKeyboardState = keyboardState;

                            // Actualizo la camara, enviandole la matriz de mundo del auto.
                            

                            mainCar.CarWorld =
                                Matrix.CreateRotationY(mainCar.CarRotation)
                                * Matrix.CreateTranslation(mainCar.CarHeight)
                                * Matrix.CreateTranslation(mainCar.CarPosition);

                            IsoCamera.Update(mainCar.CarWorld);
                            
                            
                            /*carShader.Parameters["eyePosition"].SetValue(IsoCamera.eye);*/

                            
                            var rotation = Matrix.CreateRotationY(mainCar.CarRotation);
                            var translation = Matrix.CreateTranslation(mainCar.CarPosition);
                            carBox.Orientation = rotation;
                            var height = mainCar.CarHeight.X;
                            carBox.Center = mainCar.CarPosition + new Vector3(0, height, 0);

                            // Create an OBB World-matrix so we can draw a cube representing it or something like that. Do we even need to draw it?
                            carOBBWorld = Matrix.CreateScale(carBox.Extents * 2f) *
                                          carBox.Orientation *
                                          translation;

                            //power up car intersection.
                            for (int i = 0; i < listPowerSphere.Count; i++) // Lista pequeña.
                            {
                                BoundingSphere auxSphere = listPowerSphere[i];
                                if (carBox.Intersects(auxSphere))
                                {
                                    ///// Que pasa aca si tocan? Logica/Grafica.
                                    instances.RemoveAt(i); // Parte grafica. Borrar de la lista.
                                    listPowerSphere.RemoveAt(i);
                                    pingSound.Play();
                                    i--;
                                }
                            }

                            enemyCar1.CarWorld = enemyCar1.movementAI(mainCar, elapsedTime);
                            mainCar.CarPosition.X = MathHelper.Clamp(mainCar.CarPosition.X, trackMinX, trackMaxZ);
                            mainCar.CarPosition.Z = MathHelper.Clamp(mainCar.CarPosition.Z, trackMinZ, trackMaxZ);

                            break;
                        }
                    }

                    base.Update(gameTime);
                }
             
                
                protected override void Draw(GameTime gameTime)
                {
                    // Aca deberiamos poner toda la logia de renderizado del juego. 
                    //clean the screen blue before drawing.
                    GraphicsDevice.Clear(Color.Blue);
                    
                    // Para dibujar le modelo necesitamos pasarle informacion que el efecto esta esperando.
                    /*this.Effect = MapCreator.Effect;
                    Effect.Parameters["View"].SetValue(IsoCamera.View);
                    Effect.Parameters["Projection"].SetValue(IsoCamera.Projection);
                    Effect.Parameters["DiffuseColor"].SetValue(Color.Red.ToVector3());*/


                    switch (STATUS)
                    {

                        case PRESENTATION_0_SCREN:{
                            

                            // un auto flotando y letras?! Eso seria una cosa 3.
                            IsoCamera.Update(mainCar.CarWorld);
                            
                            SpriteBatch.Begin(
                                samplerState: SamplerState.LinearWrap  
                            );
                            Point windowCenter= new Point(windowCornerBR.X/2, windowCornerBR.Y);
                            UI.DrawTextOnXY("Testing Hello", windowCenter,1);
                            SpriteBatch.End();
                            
                            mainCar.drawMainCarOnCenter(IsoCamera);
                            enemyCar1.drawCars(IsoCamera);
                            break;
                        }
                        
                        case END_SCREEN:{

                            // Solo letras no tengo ganas de muchas cosas.
                            //not idea what's going to be here hehehehehehe When does the game end?!
                            break;
                        }

                    default:
                        {
                            
                            //cache for camera
                            var realView       = IsoCamera.View;       
                            var realProjection = IsoCamera.Projection; 
                            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
                            //drawing cubeMap
                            foreach (CubeMapFace face in Enum.GetValues(typeof(CubeMapFace)))
                            {
                                
                                GraphicsDevice.SetRenderTarget(environmentMap, face);
                                GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.CornflowerBlue, 1f, 0);

                                // Orient and build the view for this face
                                cubeMapCamera.SetFace(face);
                                
                                
                                //draws only take Isocamera so we jam in cubeMapCamera
                                IsoCamera.View       = cubeMapCamera.View;      
                                IsoCamera.Projection = cubeMapCamera.Projection; 

                                // Draw your world geometry.
                                MapCreator.EarthDraw(GraphicsDevice, IsoCamera);// let's do this by ISOCAMERA FOR NOW. AHHHHH JESUS.
                                MapCreator.UpdateFrustum(new BoundingFrustum(cubeMapCamera.View * cubeMapCamera.Projection));
                                MapCreator.ObstacleDraw(GraphicsDevice,IsoCamera, instances, listPowerSphere);
                                MapCreator.ObstacleDraw(GraphicsDevice,IsoCamera, TreeInstances, listTreeSphere);

                                BasicEffect auxRampCube = new BasicEffect(GraphicsDevice);
                                auxRampCube.View = IsoCamera.View;
                                auxRampCube.Projection = IsoCamera.Projection;
                                auxRampCube.EnableDefaultLighting();

                                Effect effectAuxCube = auxRampCube;
                                for (int i = 0; i < rampWorld.Length; i++)
                                {
                                    auxRampCube.World = rampWorld[i];
                                    boxPrimitive.Draw(effectAuxCube);

                                }
                                
                            }
                            //get the cameras back
                            IsoCamera.View       = realView;       
                            IsoCamera.Projection = realProjection; 
                            
                            GraphicsDevice.SetRenderTarget(null);
                            
                            carShader.Parameters["environmentMap"].SetValue(environmentMap);
                            GraphicsDevice.SamplerStates[1] = SamplerState.LinearClamp;

                            // 4) Pass the current camera position into the shader
                            carShader.Parameters["eyePosition"].SetValue(IsoCamera.eye);
                            
                            /*carShader.Parameters["EnvTex"].SetValue(environmentMap);*/ /// these don't seem to work at all.
                            
                            MapCreator.EarthDraw(GraphicsDevice,IsoCamera);
                            MapCreator.UpdateFrustum(boundingFrustum);
                            MapCreator.ObstacleDraw(GraphicsDevice,IsoCamera, instances, listPowerSphere);
                            MapCreator.ObstacleDraw(GraphicsDevice,IsoCamera, TreeInstances, listTreeSphere);
                            
                            //this draws car on center. Shocking, I know.
                            mainCar.drawMainCarOnCenter(IsoCamera);
                            enemyCar1.drawCars(IsoCamera);
                           
                            ////////////////////////
                            /// THIS IS NO LONGER DRAWING A RAMP, WONDER WHY.
                            //supposed to draw the ramp.
                            BasicEffect auxRamp = new BasicEffect(GraphicsDevice);
                            auxRamp.View = IsoCamera.View;
                            auxRamp.Projection = IsoCamera.Projection;
                            auxRamp.EnableDefaultLighting();

                            Effect effect = auxRamp;
                            for (int i = 0; i < rampWorld.Length; i++)
                            {
                                auxRamp.World = rampWorld[i];
                                boxPrimitive.Draw(effect);

                            }
                            ///////////////////////////////////

                            /*mainCar.carModel.Draw(mainCar.CarWorld,IsoCamera.View, IsoCamera.Projection);*/
                            lineEffect.View = IsoCamera.View;
                            lineEffect.World = Matrix.Identity;
                            foreach (var pass in lineEffect.CurrentTechnique.Passes)
                            {
                                pass.Apply();
                                GraphicsDevice.DrawUserPrimitives(
                                    PrimitiveType.LineStrip,
                                    borderVerts, 0, borderVerts.Length - 1);
                            }

                            
                            Point aux = new Point(windowCornerBR.X / 2, windowCornerBR.Y);
                            UI.DrawTextOnXY("Speed:" + finalSpeed*5, aux, 1); // THIS NEEDS FURTHER WORK maybe change from screen to viewport.
                            UI.DrawTextOnXY("TIME:" + (int)timer, windowCornerBL,1);

                            
                        }
                        
                            break;
                    }
                }
                
                /// <summary>
                ///     Libero los recursos que se cargaron en el juego.
                /// </summary>
                protected override void UnloadContent()
                {
                    // Libero los recursos.
                    Content.Unload();

                    base.UnloadContent();
                }
            }
            
            
        }
        
        