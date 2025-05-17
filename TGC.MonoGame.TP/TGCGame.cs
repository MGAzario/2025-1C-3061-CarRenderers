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
                

                /// <summary>
              
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
                    
                    public Vector3 CarPosition { get; set; }
                    public Vector3 CarHeight { get; set; }
                    public float JumpingDistance { get; set; }
                    

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

                    // Cargo el modelo del logo.

                    instances = new List<ModelInstance>();

                    var meshNames = new[] { "RacingCar", "Vehicle",  "Weapons" };
                    string[] subfolders = {"tgc-media-2023-2c"};
                    var rnd = new Random();

                    
                    // Cargo un efecto basico propio declarado en el Content pipeline.
                    // En el juego no pueden usar BasicEffect de MG, deben usar siempre efectos propios.
                    Effect = Content.Load<Effect>(ContentFolderEffects + "BasicShader");

                    // Asigno el efecto que cargue a cada parte del mesh.
                    // Un modelo puede tener mas de 1 mesh internamente.

                    //!!!!! aca cargar los modelos del TP!
                    
                    
                    
                    for (int i = 0; i < 150; i++)
                    {
                        
                        
                        string name = meshNames[rnd.Next(0, meshNames.Length)];
                        string assetPath = ContentFolder3D + string.Join("/", subfolders) + "/" + name ;
                        var model = Content.Load<Model>(assetPath);
                        
                        
                        foreach (var mesh in model.Meshes)
                        {
                            // Un mesh puede tener mas de 1 mesh part (cada 1 puede tener su propio efecto).
                            foreach (var meshPart in mesh.MeshParts)
                            {
                                meshPart.Effect = Effect;
                            }
                            
                            float x = rnd.Next(-100000, 100000);
                            float z = rnd.Next(-100000, 100000);
                            var world = Matrix.CreateRotationY((float)rnd.NextDouble() * MathHelper.TwoPi)
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

                    var mainCar = new Cars();
                    var standardPhysics =  new UniversePhysics();
                    
                    
                    
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
            
                    Vector3 carDirection =  new Vector3((float)Math.Sin(mainCar.CarRotation), 0, (float)Math.Cos(mainCar.CarRotation));

                    if (keyboardState.IsKeyDown((Keys.W)) || keyboardState.IsKeyDown((Keys.S)) || mainCar.Speed != 0)
                    {
                        if (keyboardState.IsKeyDown(Keys.W))
                        {
                        //Here come accelearation
                        mainCar.Speed -= mainCar.Acceleration * elapsedTime;
                        }

                        if (keyboardState.IsKeyDown(Keys.S))
                        {
                        //Here comes desacceleration
                        mainCar.Speed += mainCar.Acceleration * elapsedTime;
                        }
                
                        if (keyboardState.IsKeyUp((Keys.W)) && keyboardState.IsKeyUp((Keys.S)))
                            mainCar.Speed -= mainCar.Speed * mainCar.Acceleration * elapsedTime;
                
                        mainCar.CarPosition += carDirection * mainCar.Speed;
                
                        }

                    mainCar.CarWorld = Matrix.CreateRotationY(mainCar.CarRotation) * Matrix.CreateTranslation(mainCar.CarPosition) * Matrix.CreateTranslation(mainCar.CarHeight);
            
                    // Actualizo la camara, enviandole la matriz de mundo del auto.
                    FollowCamera.Update(gameTime, mainCar.CarWorld);
                    

                    base.Update(gameTime);
                }

                /// <summary>
                ///     Se llama cada vez que hay que refrescar la pantalla.
                ///     Escribir aqui el codigo referido al renderizado.
                /// </summary>
                protected override void Draw(GameTime gameTime)
                {
                    // Aca deberiamos poner toda la logia de renderizado del juego.
                    GraphicsDevice.Clear(Color.Black);

                    // Para dibujar le modelo necesitamos pasarle informacion que el efecto esta esperando.
                    Effect.Parameters["View"].SetValue(View);
                    Effect.Parameters["Projection"].SetValue(Projection);
                    Effect.Parameters["DiffuseColor"].SetValue(Color.Red.ToVector3());

                    foreach (var inst in instances)
                    {
                        foreach (var mesh in inst.Model.Meshes)
                        {
                            Effect.Parameters["World"].SetValue(mesh.ParentBone.Transform * inst.World);
                            mesh.Draw();
                        }
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