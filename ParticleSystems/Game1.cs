using Microsoft.Xna.Framework;
using GeonBit;
using GeonBit.ECS;
using GeonBit.ECS.Components;
using GeonBit.ECS.Components.Graphics;
using GeonBit.ECS.Components.Misc;
using GeonBit.ECS.Components.Particles;
using GeonBit.ECS.Components.Particles.Animators;
using System.Collections.Generic;

namespace ParticleSystems
{
    /// <summary>
    /// Your main game class!
    /// </summary>
    internal class Game1 : GeonBitGame
    {
        // camera
        GameObject camera;

        // list of particle systems in demo
        List<GameObject> _systems = new List<GameObject>();

        /// <summary>
        /// Initialize your GeonBitGame properties here.
        /// </summary>
        public Game1()
        {
            InitParams.UiTheme = "hd";
            InitParams.DebugMode = true;
            InitParams.EnableVsync = true;
            InitParams.FullScreen = true;
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

            // select particle system to show
            if (Managers.GameInput.IsKeyboardKeyDown(GeonBit.Input.KeyboardKeys.D1)) { SelectParticleSystem(0); }
            if (Managers.GameInput.IsKeyboardKeyDown(GeonBit.Input.KeyboardKeys.D2)) { SelectParticleSystem(1); }
            if (Managers.GameInput.IsKeyboardKeyDown(GeonBit.Input.KeyboardKeys.D3)) { SelectParticleSystem(2); }
            if (Managers.GameInput.IsKeyboardKeyDown(GeonBit.Input.KeyboardKeys.D4)) { SelectParticleSystem(3); }
            if (Managers.GameInput.IsKeyboardKeyDown(GeonBit.Input.KeyboardKeys.D5)) { SelectParticleSystem(4); }
            if (Managers.GameInput.IsKeyboardKeyDown(GeonBit.Input.KeyboardKeys.D6)) { SelectParticleSystem(5); }
            if (Managers.GameInput.IsKeyboardKeyDown(GeonBit.Input.KeyboardKeys.D7)) { SelectParticleSystem(6); }
            if (Managers.GameInput.IsKeyboardKeyDown(GeonBit.Input.KeyboardKeys.D8)) { SelectParticleSystem(7); }
            if (Managers.GameInput.IsKeyboardKeyDown(GeonBit.Input.KeyboardKeys.D9)) { SelectParticleSystem(8); }
        }

        /// <summary>
        /// Initialize to implement per main type.
        /// </summary>
        override public void Initialize()
        {
            // set default scene node type
            GameObject.DefaultSceneNodeType = SceneNodeType.Simple;

            // create the scene
            GameScene scene = new GameScene();

            // add instructions
            GeonBit.UI.Entities.Paragraph instructions = new GeonBit.UI.Entities.Paragraph("Press number keys (1-9) to change particle types.");
            scene.UserInterface.AddEntity(instructions);

            // add diagnostic data paragraph to scene
            var diagnosticData = new GeonBit.UI.Entities.Paragraph("", GeonBit.UI.Entities.Anchor.BottomLeft, offset: Vector2.One * 10f, scale: 0.7f);
            diagnosticData.BeforeDraw = (GeonBit.UI.Entities.Entity entity) =>
            {
                diagnosticData.Text = Managers.Diagnostic.GetReportString();
            };
            scene.UserInterface.AddEntity(diagnosticData);

            // create camera
            camera = new GameObject();
            Camera cameraComponent = new Camera();
            cameraComponent.LookAt = Vector3.Zero;
            camera.AddComponent(cameraComponent);
            camera.SceneNode.PositionZ = 50;
            camera.SceneNode.PositionY = 50;
            camera.AddComponent(new CameraEditorController());
            camera.Parent = scene.Root;

            // add skybox
            Managers.GraphicsManager.CreateSkybox(null, scene.Root);

            // particle system: red fountain
            {
                // define the particle
                GameObject particle = new GameObject("particle", SceneNodeType.ParticlesNode);
                ShapeRenderer shape = particle.AddComponent(new ShapeRenderer(ShapeMeshes.Sphere), "model") as ShapeRenderer;
                shape.RenderingQueue = GeonBit.Core.Graphics.RenderingQueue.Effects;
                particle.AddComponent(new TimeToLive(3f));
                particle.AddComponent(new FadeAnimator(BaseAnimatorProperties.Defaults, 1f, 0f, 2.5f, 0.5f));
                particle.AddComponent(new MotionAnimator(BaseAnimatorProperties.Defaults, Vector3.Up * 25f, acceleration: Vector3.Down * 15f, velocityDirectionJitter: Vector3.One * 5));
                particle.AddComponent(new ScaleAnimator(BaseAnimatorProperties.Defaults, 0.5f, 1.5f, 4f, 0.5f, 0.5f, 0.5f));
                particle.AddComponent(new ColorAnimator(BaseAnimatorProperties.Defaults, Color.Red, Color.Orange, 3f));

                // create particles system
                GameObject systemObject = new GameObject("system1", SceneNodeType.Simple);
                ParticleSystem system = systemObject.AddComponent(new ParticleSystem()) as ParticleSystem;
                system.AddParticleType(new ParticleType(particle, frequency: 0.85f));
                systemObject.Parent = scene.Root;
                _systems.Add(systemObject);
            }

            // particle system: gas cloud
            {
                // define the particle
                GameObject particle = new GameObject("particle");
                ShapeRenderer shape = particle.AddComponent(new ShapeRenderer(ShapeMeshes.Sphere), "model") as ShapeRenderer;
                shape.RenderingQueue = GeonBit.Core.Graphics.RenderingQueue.Effects;
                particle.AddComponent(new TimeToLive(3f));
                particle.AddComponent(new SpawnRandomizer(positionJitter: Vector3.One * 2f));
                particle.AddComponent(new FadeAnimator(BaseAnimatorProperties.Defaults, 0.75f, 0f, 2.5f, 0.5f));
                particle.AddComponent(new MotionAnimator(new BaseAnimatorProperties(speedFactor: 1.5f), Vector3.One, velocityDirectionJitter: Vector3.One));
                particle.AddComponent(new RotationAnimator(BaseAnimatorProperties.Defaults, 1f));
                particle.AddComponent(new ScaleAnimator(BaseAnimatorProperties.Defaults, 0.5f, 1.5f, 4f, 0.5f, 0.5f, 0.5f));
                particle.AddComponent(new ColorAnimator(BaseAnimatorProperties.Defaults, Color.Green, Color.Black, 3f, startColorJitter: Color.Blue));

                // create particles system
                GameObject systemObject = new GameObject("system2");
                ParticleSystem system = systemObject.AddComponent(new ParticleSystem()) as ParticleSystem;
                system.AddParticleType(new ParticleType(particle, frequency: 0.35f));
                systemObject.Parent = scene.Root;
                _systems.Add(systemObject);
            }

            // particle system: balls madness
            {
                // define the particle
                GameObject particle = new GameObject("particle");
                ShapeRenderer shape = particle.AddComponent(new ShapeRenderer(ShapeMeshes.Sphere), "model") as ShapeRenderer;
                shape.RenderingQueue = GeonBit.Core.Graphics.RenderingQueue.Solid;
                particle.AddComponent(new TimeToLive(3f));
                particle.AddComponent(new MotionAnimator(new BaseAnimatorProperties(speedFactor: 25f), Vector3.One, velocityDirectionJitter: Vector3.One));
                particle.AddComponent(new ScaleAnimator(BaseAnimatorProperties.Defaults, 0.5f, 1.5f, 4f, 0.5f, 0.5f, 0.5f));
                particle.AddComponent(new RotationAnimator(BaseAnimatorProperties.Defaults, 20f, 4f));
                particle.AddComponent(new ColorAnimator(BaseAnimatorProperties.Defaults, Color.Black, Color.Black, 1f, startColorJitter: Color.White, endColorJitter: Color.White));

                // create particles system
                GameObject systemObject = new GameObject("system3");
                ParticleSystem system = systemObject.AddComponent(new ParticleSystem()) as ParticleSystem;
                system.AddParticleType(new ParticleType(particle, frequency: 0.85f));
                systemObject.Parent = scene.Root;
                _systems.Add(systemObject);
            }

            // particle system: rain
            {
                // define the particle
                GameObject particle = new GameObject("particle");
                ShapeRenderer shape = particle.AddComponent(new ShapeRenderer(ShapeMeshes.Sphere), "model") as ShapeRenderer;
                shape.RenderingQueue = GeonBit.Core.Graphics.RenderingQueue.Solid;
                particle.SceneNode.PositionY = 50;
                particle.AddComponent(new TimeToLive(3f));
                particle.AddComponent(new SpawnRandomizer(positionJitter: new Vector3(100, 0, 40)));
                particle.AddComponent(new MotionAnimator(new BaseAnimatorProperties(speedFactor: 25f), Vector3.Down));

                // create particles system
                GameObject systemObject = new GameObject("system4");
                ParticleSystem system = systemObject.AddComponent(new ParticleSystem()) as ParticleSystem;
                system.AddParticleType(new ParticleType(particle, frequency: 0.85f));
                systemObject.Parent = scene.Root;
                _systems.Add(systemObject);
            }

            // particle system: explosions
            {
                // define the particle
                GameObject particle = new GameObject("particle");
                ShapeRenderer shape = particle.AddComponent(new ShapeRenderer(ShapeMeshes.Sphere), "model") as ShapeRenderer;
                shape.RenderingQueue = GeonBit.Core.Graphics.RenderingQueue.Effects;
                particle.AddComponent(new TimeToLive(1f));
                particle.AddComponent(new SpawnRandomizer(positionJitter: Vector3.One * 2f, minColor: new Color(200, 75, 0), maxColor: new Color(255, 225, 50)));
                particle.AddComponent(new FadeAnimator(BaseAnimatorProperties.Defaults, 1.0f, 0f, 1.0f, 0.25f));
                particle.AddComponent(new RotationAnimator(BaseAnimatorProperties.Defaults, 1f));
                particle.AddComponent(new ScaleAnimator(BaseAnimatorProperties.Defaults, 0.5f, 2.5f, 1f, 0.15f, 0.25f, 0.25f));
                particle.AddComponent(new MotionAnimator(new BaseAnimatorProperties(speedFactor: 2f), Vector3.Zero, velocityDirectionJitter: Vector3.One));
                GameObject particle2 = particle.Clone();
                particle.GetComponent<ShapeRenderer>().BlendingState = GeonBit.Core.Graphics.BlendStates.Additive;

                // create particles system
                GameObject systemObject = new GameObject("system5");
                ParticleSystem system = systemObject.AddComponent(new ParticleSystem()) as ParticleSystem;
                system.AddParticleType(new ParticleType(particle, frequency: 0.85f));
                system.AddParticleType(new ParticleType(particle2, frequency: 0.85f));
                systemObject.Parent = scene.Root;
                _systems.Add(systemObject);
            }

            // particle system: backfire
            {
                // define the particle
                GameObject particle = new GameObject("particle");
                ShapeRenderer shape = particle.AddComponent(new ShapeRenderer(ShapeMeshes.Sphere), "model") as ShapeRenderer;
                shape.RenderingQueue = GeonBit.Core.Graphics.RenderingQueue.Effects;
                particle.AddComponent(new TimeToLive(1f));
                particle.AddComponent(new SpawnRandomizer(minColor: new Color(200, 75, 0), maxColor: new Color(255, 225, 50)));
                particle.AddComponent(new FadeAnimator(BaseAnimatorProperties.Defaults, 1.0f, 0f, 0.8f));
                particle.AddComponent(new ScaleAnimator(BaseAnimatorProperties.Defaults, 1.5f, 0.1f, 1f, 0.15f, 0.15f, 0.15f));
                particle.AddComponent(new MotionAnimator(new BaseAnimatorProperties(speedFactor: 25f), Vector3.Down));

                // create an object that's rotating the particles system position around center
                GameObject rotator = new GameObject();
                rotator.AddComponent(new RotationAnimator(BaseAnimatorProperties.Defaults, 2f));
                rotator.Parent = scene.Root;

                // create particles system
                GameObject systemObject = new GameObject("system6");
                systemObject.SceneNode.PositionY = 10f;
                ParticleSystem system = systemObject.AddComponent(new ParticleSystem()) as ParticleSystem;
                system.AddParticleType(new ParticleType(particle, frequency: 1.0f));
                system.Interval = 0.015f;
                system.AddParticlesToRoot = true;
                systemObject.Parent = rotator;
                _systems.Add(systemObject);
            }

            // particle system: magic
            {
                // define the particle
                GameObject particle = new GameObject("particle");
                ShapeRenderer shape = particle.AddComponent(new ShapeRenderer(ShapeMeshes.Sphere), "model") as ShapeRenderer;
                shape.BlendingState = GeonBit.Core.Graphics.BlendStates.Additive;
                shape.RenderingQueue = GeonBit.Core.Graphics.RenderingQueue.Effects;
                particle.AddComponent(new TimeToLive(3f));
                particle.AddComponent(new SpawnRandomizer(positionJitter: Vector3.One * 2.5f, minColor: Color.Red * 0.25f, maxColor: Color.White * 0.25f));
                particle.AddComponent(new FadeAnimator(BaseAnimatorProperties.Defaults, 0.75f, 0f, 2.5f, 0.5f));
                particle.AddComponent(new MotionAnimator(new BaseAnimatorProperties(speedFactor: 1.5f), Vector3.One, velocityDirectionJitter: Vector3.One));
                particle.AddComponent(new RotationAnimator(BaseAnimatorProperties.Defaults, 1f));
                particle.AddComponent(new ScaleAnimator(BaseAnimatorProperties.Defaults, 0.5f, 2.5f, 4f, 0.5f, 0.5f, 0.5f));

                // create particles system
                GameObject systemObject = new GameObject("system7");
                ParticleSystem system = systemObject.AddComponent(new ParticleSystem()) as ParticleSystem;
                system.AddParticleType(new ParticleType(particle, frequency: 0.3f));
                systemObject.Parent = scene.Root;
                _systems.Add(systemObject);
            }

            // particle system: magic missile
            {
                // define the particle
                GameObject particle = new GameObject("particle");
                ShapeRenderer shape = particle.AddComponent(new ShapeRenderer(ShapeMeshes.Sphere), "model") as ShapeRenderer;
                shape.BlendingState = GeonBit.Core.Graphics.BlendStates.Additive;
                shape.RenderingQueue = GeonBit.Core.Graphics.RenderingQueue.Effects;
                particle.AddComponent(new TimeToLive(3f));
                particle.AddComponent(new SpawnRandomizer(positionJitter: Vector3.One * 2.5f, minColor: Color.Red * 0.25f, maxColor: Color.White * 0.25f));
                particle.AddComponent(new FadeAnimator(BaseAnimatorProperties.Defaults, 0.75f, 0f, 2.5f, 0.5f));
                particle.AddComponent(new MotionAnimator(new BaseAnimatorProperties(speedFactor: 1.5f), Vector3.One, velocityDirectionJitter: Vector3.One));
                particle.AddComponent(new ScaleAnimator(BaseAnimatorProperties.Defaults, 0.5f, 2.5f, 4f, 0.5f, 0.5f, 0.5f));

                // define another type of particle
                GameObject particle2 = new GameObject("particle2");
                ShapeRenderer shape2 = particle2.AddComponent(new ShapeRenderer(ShapeMeshes.Sphere), "model") as ShapeRenderer;
                shape2.BlendingState = GeonBit.Core.Graphics.BlendStates.Additive;
                shape2.RenderingQueue = GeonBit.Core.Graphics.RenderingQueue.Effects;
                particle2.AddComponent(new TimeToLive(2f));
                particle2.AddComponent(new ScaleAnimator(BaseAnimatorProperties.Defaults, 1.5f, 0.1f, 1.5f, 0.15f, 0.15f, 0.15f));
                particle2.AddComponent(new FadeAnimator(BaseAnimatorProperties.Defaults, 0.5f, 0f, 1.5f, 0.5f));

                // create an object that's rotating the particles system position around center
                GameObject rotator = new GameObject();
                rotator.AddComponent(new RotationAnimator(BaseAnimatorProperties.Defaults, 2f));
                rotator.Parent = scene.Root;

                // create particles system
                GameObject systemObject = new GameObject("system8");
                systemObject.SceneNode.PositionY = 10f;
                ParticleSystem system = systemObject.AddComponent(new ParticleSystem()) as ParticleSystem;
                system.AddParticleType(new ParticleType(particle, frequency: 0.3f));
                system.AddParticleType(new ParticleType(particle2, frequency: 0.5f));
                system.AddParticlesToRoot = true;
                systemObject.Parent = rotator;
                _systems.Add(systemObject);
            }

            // particle system: smoke
            {
                // define the particle
                GameObject particle = new GameObject("particle");
                ShapeRenderer shape = particle.AddComponent(new ShapeRenderer(ShapeMeshes.Sphere), "model") as ShapeRenderer;
                shape.RenderingQueue = GeonBit.Core.Graphics.RenderingQueue.Effects;
                particle.AddComponent(new TimeToLive(3f));
                particle.AddComponent(new FadeAnimator(BaseAnimatorProperties.Defaults, 1f, 0f, 2.5f, 0.5f));
                particle.AddComponent(new MotionAnimator(BaseAnimatorProperties.Defaults, Vector3.Up * 10f, velocityDirectionJitter: Vector3.One * 0.75f));
                particle.AddComponent(new ScaleAnimator(BaseAnimatorProperties.Defaults, 0.25f, 3.5f, 4f, 0.5f, 0.5f, 0.5f));
                particle.AddComponent(new ColorAnimator(BaseAnimatorProperties.Defaults, Color.Purple, Color.Black, 3f));

                // create particles system
                GameObject systemObject = new GameObject("system9");
                ParticleSystem system = systemObject.AddComponent(new ParticleSystem()) as ParticleSystem;
                system.AddParticleType(new ParticleType(particle, frequency: 0.35f));
                systemObject.Parent = scene.Root;
                _systems.Add(systemObject);
            }

            // select default system
            SelectParticleSystem(0);

            // set scene
            GeonBitMain.Instance.Application.LoadScene(scene);
        }

        /// <summary>
        /// Choose which particle system to show.
        /// </summary>
        private void SelectParticleSystem(int index)
        {
            foreach (var go in _systems)
            {
                go.Enabled = false;
            }
            _systems[index].Enabled = true;
        }

        /// <summary>
        /// Draw function to implement per main type.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        override public void Draw(GameTime gameTime)
        {
            /// TBD add any custom drawing functionality here.
            /// Note: since GeonBit handle its own drawing internally, usually you don't need to do anything here.
        }
    }
}
