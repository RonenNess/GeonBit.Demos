using Microsoft.Xna.Framework;
using GeonBit;
using GeonBit.ECS;
using GeonBit.ECS.Components;
using GeonBit.ECS.Components.Graphics;
using GeonBit.ECS.Components.Misc;
using GeonBit.ECS.Components.Particles;
using GeonBit.ECS.Components.Physics;
using GeonBit.ECS.Components.Sound;

namespace EmptyGeonBitProject
{
    /// <summary>
    /// Your main game class!
    /// </summary>
    internal class Game1 : GeonBitGame
    {
        /// <summary>
        /// Initialize your GeonBitGame properties here.
        /// </summary>
        public Game1()
        {
            UiTheme = "hd";
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

            /// TBD add any custom Update functionality here.
        }

        /// <summary>
        /// Initialize to implement per main type.
        /// </summary>
        override public void Initialize()
        {
            // create the scene
            GameScene scene = new GameScene();

            // create skinned mesh object
            GameObject soldier = new GameObject("soldier");
            SkinnedModelRenderer model = new SkinnedModelRenderer("game/soldier");
            soldier.AddComponent(model, "model");
            soldier.SceneNode.Scale = Vector3.One;
            soldier.Parent = scene.Root;

            // create skybox
            Managers.GraphicsManager.CreateSkybox(null, scene.Root);

            // create camera
            GameObject camera = new GameObject();
            camera.AddComponent(new CameraEditorController());
            Camera cameraComponent = new Camera();
            camera.AddComponent(cameraComponent);
            camera.SceneNode.Position = new Vector3(0, 1, -0.6f) * 130f;
            camera.SceneNode.RotationX = -2.24f;
            camera.Parent = scene.Root;

            // set scene
            GeonBitMain.Instance.Application.LoadScene(scene);
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
