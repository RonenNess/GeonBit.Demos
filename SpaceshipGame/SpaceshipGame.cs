using Microsoft.Xna.Framework;
using GeonBit;
using GeonBit.ECS;
using GeonBit.ECS.Components;
using GeonBit.ECS.Components.Graphics;
using GeonBit.ECS.Components.Misc;
using GeonBit.ECS.Components.Particles;
using GeonBit.ECS.Components.Physics;
using GeonBit.ECS.Components.Sound;
using GeonBit.ECS.Components.Particles.Animators;

namespace SpaceshipGame
{
    /// <summary>
    /// A spaceship navigation game.
    /// </summary>
    internal class SpaceshipGame : GeonBitGame
    {
        // paragraph to show diagnostic data
        GeonBit.UI.Entities.Paragraph DiagnosticData;

        /// <summary>
        /// Initialize your GeonBitGame properties here.
        /// </summary>
        public SpaceshipGame()
        {
            UiTheme = "editor";
            DebugMode = true;
            EnableVsync = true;
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        override public void Update(GameTime gameTime)
        {
            /// exit application on escape
            if (Managers.GameInput.IsKeyDown(GeonBit.Input.GameKeys.Escape))
            {
                Exit();
            }
        }

        /// <summary>
        /// Initialize to implement per main type.
        /// </summary>
        override public void Initialize()
        {
            // make fullscreen
            MakeFullscreen(false);

            // create a new empty scene
            GameScene scene = new GameScene();

            // Init ui
            InitUI(scene);

            // create camera
            InitCamera(scene);

            // Init background and music
            InitAmbience(scene);

            // create the player
            InitPlayer(scene);

            // set to default top-down controls
            Managers.GameInput.SetDefaultTopDownControls();

            // add diagnostic data paragraph to scene
            DiagnosticData = new GeonBit.UI.Entities.Paragraph("", GeonBit.UI.Entities.Anchor.BottomLeft, offset: new Vector2(20, 0));
            scene.UserInterface.AddEntity(DiagnosticData);

            // load our scene (eg make it active)
            scene.Load();
        }

        /// <summary>
        /// Init background and music.
        /// </summary>
        /// <param name="scene">Game main scene.</param>
        private void InitAmbience(GameScene scene)
        {
            // create background object
            GameObject background = Managers.GraphicsManager.CreateBackground("game/background", scene.Root);
            background.GetComponent<SceneBackground>().DrawMode = BackgroundDrawMode.Tiled;

            // creates a background animator component to animate the background
            background.AddComponent(new BackgroundAnimator());

            // create an invisible floor to prevent stuff from falling to the endless abyss.
            GameObject floor = new GameObject();
            floor.AddComponent(new PhysicalBody(new EndlessPlaneInfo(Vector3.Up)));
            floor.GetComponent<PhysicalBody>().CollisionGroup = (short)CollisionGroups.Terrain;
            floor.Parent = scene.Root;

            // start background music
            Managers.SoundManager.PlayMusic("game/back_music", true, 0.5f);
        }
        
        /// <summary>
        /// Init the UI and player status components of the game.
        /// </summary>
        /// <param name="scene">Game main scene.</param>
        private void InitUI(GameScene scene)
        { 
            // get UI manager from scene
            GeonBit.UI.UserInterface UI = scene.UserInterface;

            // disable cursor
            UI.ShowCursor = false;

            // create the progressbar to display player hp
            GeonBit.UI.Entities.ProgressBar hpShow = new GeonBit.UI.Entities.ProgressBar(0, 5, new Vector2(300, -1), GeonBit.UI.Entities.Anchor.TopLeft);
            hpShow.Locked = true;
            UI.AddEntity(hpShow);
            PlayerStatus.HpShow = hpShow;

            // create the progressbar to display player ammo
            GeonBit.UI.Entities.ProgressBar ammoShow = new GeonBit.UI.Entities.ProgressBar(0, 100, new Vector2(200, -1), GeonBit.UI.Entities.Anchor.TopLeft, new Vector2(0, 50));
            ammoShow.Locked = true;
            ammoShow.ProgressFill.FillColor = Color.Red;
            UI.AddEntity(ammoShow);
            PlayerStatus.AmmoShow = ammoShow;

            // pagraph to show current score
            GeonBit.UI.Entities.Paragraph scoreShow = new GeonBit.UI.Entities.Paragraph("", GeonBit.UI.Entities.Anchor.TopCenter);
            UI.AddEntity(scoreShow);
            PlayerStatus.ScoreShow = scoreShow;

            // reset player status
            PlayerStatus.Reset();
        }

        /// <summary>
        /// Create the player object and components.
        /// </summary>
        /// <param name="scene">Game main scene.</param>
        private void InitPlayer(GameScene scene)
        {
            // first create the player root object
            GameObject player = new GameObject("player", SceneNodeType.Simple);

            // create a physical body for the player (note: make it start height in the air so the player ship will "drop" into scene).
            Vector3 bodySize = new Vector3(1f, 4, 3.5f);
            PhysicalBody shipPhysics = new PhysicalBody(new BoxInfo(bodySize), mass: 10f, inertia: 0f);
            shipPhysics.Position = Vector3.UnitY * 30;
            shipPhysics.SetDamping(0.95f, 0.95f);
            shipPhysics.Gravity = Vector3.Down * 50f;
            shipPhysics.CollisionGroup = (short)CollisionGroups.Player;
            player.AddComponent(shipPhysics);

            // add player controls component to the player object
            player.AddComponent(new PlayerControls());

            // create a particle type of the ship thrust fire effect
            AddBackfireEffect(player);

            // create an object to render the player spaceship model and add it as a child of player's object
            GameObject shipModel = new GameObject("model", SceneNodeType.Simple);
            shipModel.Parent = player;

            // add the ship model renderer
            shipModel.AddComponent(new ModelRenderer("game/spaceship"));
            shipModel.SceneNode.RotationY = (float)System.Math.PI;
            shipModel.SceneNode.Scale = Vector3.One * 0.45f;

            // add player object to scene
            player.Parent = scene.Root;

            // create the player bullets prototype - these are the objects player spawn when he shoots
            // first, create the bullet game object and add shape renderer to it
            GameObject bullet = new GameObject("bullet", SceneNodeType.Simple);
            bullet.SceneNode.Scale = new Vector3(0.2f, 0.2f, 1f);
            ShapeRenderer bulletShape = new ShapeRenderer(ShapeMeshes.SphereLowPoly);
            bulletShape.MaterialOverride.DiffuseColor = Color.Red;
            bulletShape.RenderingQueue = GeonBit.Core.Graphics.RenderingQueue.Solid;
            bullet.AddComponent(bulletShape);

            // add time-to-live component to remove bullets after a while
            bullet.AddComponent(new TimeToLive(1f));
            
            // attach physical body to the bullet so it can collide stuff
            PhysicalBody bulletBody = new PhysicalBody(new SphereInfo(1f), mass: 1f);
            bulletBody.CollisionGroup = (short)CollisionGroups.FriendProjectiles;
            bulletBody.CollisionMask = (short)(CollisionGroups.Enemies);
            bulletBody.IsEthereal = true;
            bulletBody.Gravity = Vector3.Zero;
            bulletBody.ConstVelocity = Vector3.Forward * 40f;
            bullet.AddComponent(bulletBody);

            // add sound effect when bullet spawns
            SoundEffect laserSound = new SoundEffect("game/laser");
            laserSound.PlayOnSpawn = true;
            bullet.AddComponent(laserSound);

            // register the bullet game object into the prototypes manager, so we can instanciate it later
            Managers.Prototypes.Register(bullet);

        }

        /// <summary>
        /// Create thrusts backfire effect and attach to target gameobject.
        /// </summary>
        /// <param name="target">Object to add effect to.</param>
        private void AddBackfireEffect(GameObject target)
        {
            // create a particle type of the ship thrust fire effect
            BaseAnimatorProperties prop = BaseAnimatorProperties.Defaults;
            GameObject backfire = new GameObject("backfire");

            // create a sphere renderer for the backfire particle
            backfire.AddComponent(new ShapeRenderer(ShapeMeshes.Sphere));
            backfire.GetComponent<ShapeRenderer>().RenderingQueue = GeonBit.Core.Graphics.RenderingQueue.Effects;

            // add animators, jitter and time-to-live
            backfire.AddComponent(new TimeToLive(0.5f));
            backfire.AddComponent(new SpawnRandomizer(minColor: Color.Red, maxColor: Color.Yellow));
            backfire.AddComponent(new FadeAnimator(prop, 1.0f, 0f, 0.5f));
            backfire.AddComponent(new ScaleAnimator(prop, 0.5f, 0.1f, 0.5f));
            backfire.AddComponent(new MotionAnimator(prop, Vector3.Backward * 15));

            // create particles system for the backfire effect
            GameObject backfireSystem = new GameObject("backfire-system");
            backfireSystem.SceneNode.PositionZ = 1f;
            ParticleSystem system = backfireSystem.AddComponent(new ParticleSystem()) as ParticleSystem;

            // add our backfire particle and set frequency
            system.AddParticleType(new ParticleType(backfire, frequency: 1.0f));
            system.Interval = 0.025f;
            system.AddParticlesToRoot = true;

            // attach particle system to player
            backfireSystem.Parent = target;
        }

        /// <summary>
        /// Create the scene camera.
        /// </summary>
        /// <param name="scene">Game main scene.</param>
        private void InitCamera(GameScene scene)
        {
            // create camera
            GameObject camera = new GameObject();
            Camera cameraComponent = new Camera();
            camera.AddComponent(cameraComponent);

            // set camera position and direction
            camera.SceneNode.PositionY = 35;
            camera.SceneNode.RotationX = -(float)System.Math.PI / 2f;

            // add camera to scene
            camera.Parent = scene.Root;
        }

        /// <summary>
        /// Draw function to implement per main type.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        override public void Draw(GameTime gameTime)
        {
            // update diagnostic data text
            DiagnosticData.Text = Managers.Diagnostic.GetReportString();
        }
    }
}
