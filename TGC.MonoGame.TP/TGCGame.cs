        using System;
        using System.Collections.Generic;
        using Microsoft.Xna.Framework;
        using Microsoft.Xna.Framework.Graphics;
        using Microsoft.Xna.Framework.Input;
      

        namespace TGC.MonoGame.TP
        {
            /// <summary>
            ///     Esta es la clase principal del juego.
            ///     Inicialmente puede ser renombrado o copiado para hacer mas ejemplos chicos, en el caso de copiar para que se
            ///     ejecute el nuevo ejemplo deben cambiar la clase que ejecuta Program <see cref="Program.Main()" /> linea 10.
            /// </summary>
           
            class ModelInstance
            {
            public Model Model;
            public Matrix World;
            }
            public class TGCGame : Game
            {
                public const string ContentFolder3D = "Models/";
                public const string ContentFolderEffects = "Effects/";
                public const string ContentFolderMusic = "Music/";
                public const string ContentFolderSounds = "Sounds/";
                public const string ContentFolderSpriteFonts = "SpriteFonts/";
                public const string ContentFolderTextures = "Textures/";
                private List<ModelInstance> instances;
                
                
                private IsoCamera IsoCamera { get; set; }
                private Cars mainCar;
                private UniversePhysics standardPhysics;
                
                private readonly float trackMinX = -5000f, trackMaxX = +5000f;
                private readonly float trackMinZ = -3000f, trackMaxZ = +3000f;

                BasicEffect lineEffect;
                VertexPositionColor[] borderVerts;
                
                
                /// <summary>
              
                
                
                /// </summary>
                public TGCGame()
                {
                    // Maneja la configuracion y la administracion del dispositivo grafico.
                    Graphics = new GraphicsDeviceManager(this);
                    
                    
                    
                    Graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width - 100;
                    Graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height - 100;
                    
                    // Para que el juego sea pantalla completa se puede usar Graphics IsFullScreen.
                    // Carpeta raiz donde va a estar toda la Media.
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

                    
                    // Configuramos nuestras matrices de la escena.
                    World = Matrix.Identity;
                    View = Matrix.CreateLookAt(new Vector3(0, 300000, 70000), Vector3.Zero, Vector3.Up);
                    Projection =
                        Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, GraphicsDevice.Viewport.AspectRatio, 0.1f, 1000000f);

                    IsoCamera = new IsoCamera(GraphicsDevice); 
                     mainCar = new Cars();
                     standardPhysics =  new UniversePhysics();
                    
                    
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

                    borderVerts = new[] {
                        new VertexPositionColor(new Vector3(trackMinX, 0, trackMinZ), Color.Yellow),
                        new VertexPositionColor(new Vector3(trackMaxX, 0, trackMinZ), Color.Yellow),
                        new VertexPositionColor(new Vector3(trackMaxX, 0, trackMaxZ), Color.Yellow),
                        new VertexPositionColor(new Vector3(trackMinX, 0, trackMaxZ), Color.Yellow),
                    };

                    instances = new List<ModelInstance>(); 
                    
                    
                    
                    //////////////////////////////////////////////////////////////////////////
                    string playCarPath = ContentFolder3D + "/tgc-media-2023-2c/RacingCar";
                    mainCar.carModel = Content.Load<Model>(playCarPath);
                    World = mainCar.CarWorld;
                    /*foreach (var mesh in mainCar.carModel.Meshes)
                    {
                        mainCar.carTextures.Add();
                    }*/
                    ///////////////////////////////////////////////////////////////////////
                    

                    
                    var meshNames = new[] { "Weapons" };
                    string[] subfolders = {"tgc-media-2023-2c"};
                    // Cargo un efecto basico propio declarado en el Content pipeline.
                    // En el juego no pueden usar BasicEffect de MG, deben usar siempre efectos propios.
                    Effect = Content.Load<Effect>(ContentFolderEffects + "BasicShader");
                  
                    
                   for(int i = 0; i < 6; i++ )
                   {

                       string name = meshNames[0]; // Le estamos entergando RacingCar podemos
                       string assetPath = ContentFolder3D + string.Join("/", subfolders) + "/" + name;
                       var model = Content.Load<Model>(assetPath);

                       //en el futuro podemos manipular la parte de arriba para hacer load de todos los recursos. inc escenario.

                       //model -> mesh -> mesh parts.
                       foreach (var mesh in model.Meshes)
                       {
                           // Un mesh puede tener mas de 1 mesh part (cada 1 puede tener su propio efecto).
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
                           instances.Add(new ModelInstance { Model = model, World = world });

                       }
                   }
                   base.LoadContent();
                }

                /// <summary>
                ///     Se llama en cada frame.
                ///     Se debe escribir toda la logica de computo del modelo, asi como tambien crificar entradas del usuario y reacciones
                ///     ante ellas.
                /// </summary>
                protected override void Update(GameTime gameTime)
                {
                    
                    float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
                    
                    
                    // Aca deberiamos poner toda la logica de actualizacion del juego.

                    // Capturar Input teclado
                    
                    var keyboardState = Keyboard.GetState();
                    if (keyboardState.IsKeyDown(Keys.Escape))
                    {
                        //Salgo del juego.
                        Exit();
                    }
                    
                    if (keyboardState.IsKeyDown(Keys.Space) && !mainCar.Jumping)
                    {
                        // Here comes jumping: it means it should jump and let the gravity do its work.
                        mainCar.Jumping = true;
                    }

                    if(mainCar.Jumping)
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
            
                    /*if (keyboardState.IsKeyDown(Keys.A))
                        {
                        // Here comes turn left
                        mainCar.CarRotation += mainCar.RotationSpeed * elapsedTime;
                        }
                    if (keyboardState.IsKeyDown(Keys.D))
                        {
                        //Here comes turn right
                        mainCar.CarRotation -= mainCar.RotationSpeed * elapsedTime;
                        }*/
            
                    Vector3 carDirection =  new Vector3((float)Math.Sin(mainCar.CarRotation), 0, (float)Math.Cos(mainCar.CarRotation));

                    if (keyboardState.IsKeyDown((Keys.W)) || keyboardState.IsKeyDown((Keys.S)) || mainCar.Speed != 0)
                    {
                        if (keyboardState.IsKeyDown(Keys.W))
                        {
                        //Here come accelearation
                        mainCar.Speed += mainCar.Acceleration * elapsedTime;
                        
                        if (keyboardState.IsKeyDown(Keys.A))
                        {
                            // Here comes turn left
                            mainCar.CarRotation += mainCar.RotationSpeed * elapsedTime;
                        }
                        if (keyboardState.IsKeyDown(Keys.D))
                        {
                            //Here comes turn right
                            mainCar.CarRotation -= mainCar.RotationSpeed * elapsedTime;
                        }
                        }

                        if (keyboardState.IsKeyDown(Keys.S))
                        {
                        //Here comes desacceleration
                        mainCar.Speed -= mainCar.Acceleration * elapsedTime;
                        
                        if (keyboardState.IsKeyDown(Keys.A))
                        {
                            // Here comes turn left
                            mainCar.CarRotation += mainCar.RotationSpeed * elapsedTime;
                        }
                        if (keyboardState.IsKeyDown(Keys.D))
                        {
                            //Here comes turn right
                            mainCar.CarRotation -= mainCar.RotationSpeed * elapsedTime;
                        }
                        }
                
                        if (keyboardState.IsKeyUp((Keys.W)) && keyboardState.IsKeyUp((Keys.S)))
                            mainCar.Speed -= mainCar.Speed * mainCar.Acceleration * elapsedTime;
                
                        mainCar.CarPosition += carDirection * mainCar.Speed;
                        mainCar.wheelRotation = (mainCar.Speed*elapsedTime);
                    }
            
                    // Actualizo la camara, enviandole la matriz de mundo del auto.
                    
                    
                    mainCar.CarWorld =
                        Matrix.CreateRotationY( mainCar.CarRotation)
                        * Matrix.CreateTranslation(mainCar.CarHeight)
                        * Matrix.CreateTranslation(mainCar.CarPosition);
                    
                    IsoCamera.Update( mainCar.CarWorld);

                    mainCar.CarPosition.X = MathHelper.Clamp(mainCar.CarPosition.X, trackMinX, trackMaxZ);
                    mainCar.CarPosition.Z = MathHelper.Clamp(mainCar.CarPosition.Z, trackMinZ, trackMaxZ);

                    base.Update(gameTime);
                }

                /// <summary>
                ///     Se llama cada vez que hay que refrescar la pantalla.
                ///     Escribir aqui el codigo referido al renderizado.
                /// </summary>
                protected override void Draw(GameTime gameTime)
                {
                    // Aca deberiamos poner toda la logia de renderizado del juego.
                    GraphicsDevice.Clear(Color.Blue);

                    // Para dibujar le modelo necesitamos pasarle informacion que el efecto esta esperando.
                    
                    Effect.Parameters["View"].SetValue(IsoCamera.View);
                    Effect.Parameters["Projection"].SetValue(IsoCamera.Projection);
                    Effect.Parameters["DiffuseColor"].SetValue(Color.Red.ToVector3());
                    
                    
                    foreach (var inst in instances)
                    {
                        foreach (var mesh in inst.Model.Meshes)
                        {
                            Effect.Parameters["World"].SetValue(mesh.ParentBone.Transform * inst.World  );
                            mesh.Draw();
                        }
                    }

                    foreach (var mesh in mainCar.carModel.Meshes)
                    {
                        var baseWorld = Matrix.CreateRotationY(mainCar.CarRotation)
                                        * Matrix.CreateTranslation(mainCar.CarPosition);

                        Matrix world;
                        
                        if (mesh.Name.Contains("Wheel"))
                        {
                            var spin = Matrix.CreateRotationX(mainCar.wheelRotation*10f/mainCar.wheelRadius);

                            // 3) the mesh.ParentBone.Transform already positions
                            //    the wheel correctly relative to the car’s origin.
                            //    So multiply: ParentBone → spin → car’s world
                            world =  spin * mesh.ParentBone.Transform
                                   
                                    * baseWorld;
                        }
                        else
                        {
                            world = mesh.ParentBone.Transform * baseWorld;  
                        }
                        
                        foreach (BasicEffect fx in mesh.Effects)
                        {
                            fx.World      = world;
                            fx.View       = IsoCamera.View;
                            fx.Projection = IsoCamera.Projection;
                            fx.EnableDefaultLighting();
                            fx.DiffuseColor = Color.Gray.ToVector3();
                        }
                        
                        mesh.Draw();
                    }
                    
                    
                    
                    /*mainCar.carModel.Draw(mainCar.CarWorld,IsoCamera.View, IsoCamera.Projection);*/
                    
                    lineEffect.View  = IsoCamera.View;
                    lineEffect.World = Matrix.Identity; 
                    
                    foreach (var pass in lineEffect.CurrentTechnique.Passes)
                    {
                        pass.Apply();
                        GraphicsDevice.DrawUserPrimitives(
                            PrimitiveType.LineStrip,
                            borderVerts, 0, borderVerts.Length - 1);
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